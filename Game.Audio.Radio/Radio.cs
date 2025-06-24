using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Colossal;
using Colossal.Annotations;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.Logging;
using Colossal.PSI.Common;
using Colossal.Randomization;
using Colossal.UI.Binding;
using Game.Assets;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Settings;
using Game.Simulation;
using Game.Triggers;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Game.Audio.Radio;

public class Radio
{
	public struct ClipInfo
	{
		public AudioAsset m_Asset;

		public SegmentType m_SegmentType;

		public Entity m_Emergency;

		public Entity m_EmergencyTarget;

		public Task<AudioClip> m_LoadTask;

		public int m_ResumeAtPosition;

		public bool m_Replaying;
	}

	public delegate void OnRadioEvent(Radio radio);

	public delegate void OnClipChanged(Radio radio, AudioAsset asset);

	public delegate void OnDemandClips(RuntimeSegment segment);

	public class RadioChannel : IContentPrerequisite
	{
		public string name;

		[CanBeNull]
		public string nameId;

		public string description;

		public string icon;

		public int uiPriority;

		public string network;

		public Program[] programs;

		public string[] contentPrerequisites { get; set; }

		public RuntimeRadioChannel CreateRuntime(string path)
		{
			RuntimeRadioChannel runtimeRadioChannel = new RuntimeRadioChannel();
			runtimeRadioChannel.name = name;
			runtimeRadioChannel.description = description;
			runtimeRadioChannel.icon = icon;
			runtimeRadioChannel.uiPriority = uiPriority;
			runtimeRadioChannel.network = network;
			runtimeRadioChannel.Initialize(this, name + " (" + path + ")");
			return runtimeRadioChannel;
		}
	}

	public class RuntimeRadioChannel : IComparable<RuntimeRadioChannel>, IJsonWritable
	{
		public string name;

		public string description;

		public string icon;

		public int uiPriority;

		public string network;

		private readonly RuntimeProgram kNoProgram = new RuntimeProgram
		{
			name = "No program"
		};

		public RuntimeProgram currentProgram { get; private set; }

		public RuntimeProgram[] schedule { get; private set; }

		public void Initialize(RadioChannel radioChannel, string path)
		{
			BuildRuntimePrograms(radioChannel.programs, path);
		}

		public bool Update(int timeOfDaySeconds)
		{
			bool result = false;
			bool flag = false;
			for (int i = 0; i < schedule.Length; i++)
			{
				RuntimeProgram runtimeProgram = schedule[i];
				if (timeOfDaySeconds >= runtimeProgram.startTime && timeOfDaySeconds < runtimeProgram.endTime && (runtimeProgram.loopProgram || !runtimeProgram.hasEnded))
				{
					if (currentProgram != runtimeProgram || (runtimeProgram.hasEnded && runtimeProgram.loopProgram))
					{
						log.DebugFormat("Channel {1} - Program changed to {0}", (object)runtimeProgram.name, (object)name);
						result = true;
						runtimeProgram.Reset();
					}
					runtimeProgram.active = true;
					currentProgram = runtimeProgram;
					flag = true;
				}
				else
				{
					runtimeProgram.active = false;
				}
			}
			if (!flag)
			{
				currentProgram = null;
			}
			return result;
		}

		private bool IsValidTimestamp(int start, int end)
		{
			if (start != -1 && end != -1)
			{
				return start <= end;
			}
			return false;
		}

		private RuntimeProgram CreateRuntimeProgram(Program p, int startSecs, int endSecs, string path)
		{
			RuntimeProgram runtimeProgram = new RuntimeProgram();
			runtimeProgram.name = p.name;
			runtimeProgram.description = p.description;
			runtimeProgram.startTime = startSecs;
			runtimeProgram.endTime = endSecs;
			runtimeProgram.loopProgram = p.loopProgram;
			runtimeProgram.BuildRuntimeSegments(p, path);
			return runtimeProgram;
		}

		private RuntimeProgram ShallowCopyRuntimeProgram(RuntimeProgram p, int startSecs, int endSecs)
		{
			return new RuntimeProgram
			{
				name = p.name,
				description = p.description,
				startTime = startSecs,
				endTime = endSecs,
				segments = p.segments,
				loopProgram = p.loopProgram
			};
		}

		private void AddRuntimeProgram(Program p, int startSecs, int endSecs, List<RuntimeProgram> schedule, string path)
		{
			if (schedule.Count == 0)
			{
				schedule.Add(CreateRuntimeProgram(p, startSecs, endSecs, path));
				return;
			}
			for (int i = 0; i < schedule.Count; i++)
			{
				RuntimeProgram runtimeProgram = schedule[i];
				if (startSecs > runtimeProgram.startTime && endSecs < runtimeProgram.endTime)
				{
					RuntimeProgram runtimeProgram2 = CreateRuntimeProgram(p, startSecs, endSecs, path);
					schedule.Insert(++i, runtimeProgram2);
					schedule.Insert(++i, ShallowCopyRuntimeProgram(runtimeProgram, runtimeProgram2.endTime, runtimeProgram.endTime));
					runtimeProgram.endTime = runtimeProgram2.startTime;
					return;
				}
				if (startSecs < runtimeProgram.startTime && endSecs > runtimeProgram.startTime)
				{
					RuntimeProgram runtimeProgram3 = CreateRuntimeProgram(p, startSecs, endSecs, path);
					log.WarnFormat("Program '{0}' overlaps with '{1}' in radio channel '{2}'", (object)runtimeProgram3.name, (object)runtimeProgram.name, (object)path);
					return;
				}
				if (startSecs < runtimeProgram.startTime && endSecs < runtimeProgram.startTime)
				{
					RuntimeProgram item = CreateRuntimeProgram(p, startSecs, endSecs, path);
					schedule.Insert(i, item);
					return;
				}
			}
			schedule.Add(CreateRuntimeProgram(p, startSecs, endSecs, path));
		}

		private void BuildRuntimePrograms(Program[] programs, string path)
		{
			if (programs != null)
			{
				List<RuntimeProgram> list = new List<RuntimeProgram>();
				foreach (Program program in programs)
				{
					int num = FormatUtils.ParseTimeToSeconds(program.startTime);
					int num2 = FormatUtils.ParseTimeToSeconds(program.endTime);
					if (IsValidTimestamp(num, num2))
					{
						if (num == num2)
						{
							num2 += 86400;
							if (num2 > 86400)
							{
								AddRuntimeProgram(program, 0, num2 - 86400, list, path);
								AddRuntimeProgram(program, num, 86400, list, path);
							}
							else
							{
								AddRuntimeProgram(program, num, num2, list, path);
							}
						}
						else
						{
							AddRuntimeProgram(program, num, num2, list, path);
						}
					}
					else
					{
						log.WarnFormat("Program '{0}' has invalid timestamps ({3} ({1})->{4} ({2})) in radio channel '{5}' and was ignored!", new object[6]
						{
							program.name,
							num,
							num2,
							FormatUtils.FormatTimeDebug(num),
							FormatUtils.FormatTimeDebug(num2),
							path
						});
					}
				}
				schedule = list.ToArray();
			}
			else
			{
				log.WarnFormat("No program founds in radio channel '{0}'", (object)path);
			}
		}

		public int CompareTo(RuntimeRadioChannel other)
		{
			return uiPriority.CompareTo(other.uiPriority);
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("name");
			writer.Write(name);
			writer.PropertyName("description");
			writer.Write(description);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.PropertyName("network");
			writer.Write(network);
			writer.PropertyName("currentProgram");
			JsonWriterExtensions.Write<RuntimeProgram>(writer, currentProgram ?? kNoProgram);
			writer.PropertyName("schedule");
			JsonWriterExtensions.ArrayBegin(writer, schedule.Length);
			for (int i = 0; i < schedule.Length; i++)
			{
				JsonWriterExtensions.Write<RuntimeProgram>(writer, schedule[i]);
			}
			writer.ArrayEnd();
			writer.TypeEnd();
		}
	}

	public class RadioNetwork : IComparable<RadioNetwork>, IJsonWritable, IContentPrerequisite
	{
		public string name;

		[CanBeNull]
		public string nameId;

		public string description;

		public string descriptionId;

		public string icon;

		public bool allowAds;

		public int uiPriority;

		public string[] contentPrerequisites { get; set; }

		public int CompareTo(RadioNetwork other)
		{
			return uiPriority.CompareTo(other.uiPriority);
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("name");
			writer.Write(name);
			writer.PropertyName("nameId");
			writer.Write(nameId);
			writer.PropertyName("description");
			writer.Write(description);
			writer.PropertyName("descriptionId");
			writer.Write(descriptionId);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.TypeEnd();
		}
	}

	public class RadioPlayer : IDisposable
	{
		private AudioMixerGroup m_RadioGroup;

		private AudioSource m_AudioSource;

		private Stopwatch m_Timer = new Stopwatch();

		private double m_Elapsed;

		private Spectrum m_Spectrum;

		public bool isCreated => (Object)(object)m_AudioSource != (Object)null;

		public bool isPlaying => m_AudioSource.isPlaying;

		public int playbackPosition => m_AudioSource.timeSamples;

		public bool muted
		{
			get
			{
				if (!((Object)(object)m_AudioSource != (Object)null))
				{
					return false;
				}
				return m_AudioSource.volume == 0f;
			}
			set
			{
				if ((Object)(object)m_AudioSource != (Object)null)
				{
					m_AudioSource.volume = (value ? 0f : 1f);
				}
			}
		}

		public Texture equalizerTexture => m_Spectrum?.equalizerTexture;

		public string currentClipName
		{
			get
			{
				if (!isCreated || !((Object)(object)m_AudioSource.clip != (Object)null))
				{
					return "None";
				}
				return ((Object)m_AudioSource.clip).name;
			}
		}

		public AudioClip currentClip => m_AudioSource.clip;

		public void Pause()
		{
			if ((Object)(object)m_AudioSource != (Object)null)
			{
				m_AudioSource.Pause();
			}
			m_Timer.Stop();
		}

		public void Unpause()
		{
			if ((Object)(object)m_AudioSource != (Object)null)
			{
				m_AudioSource.UnPause();
			}
			m_Timer.Start();
		}

		public RadioPlayer(AudioMixerGroup radioGroup)
		{
			m_RadioGroup = radioGroup;
		}

		public void SetSpectrumSettings(bool enabled, int numSamples, FFTWindow fftWindow, Spectrum.BandType bandType, float spacing, float padding)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (m_Spectrum != null)
			{
				if (enabled)
				{
					m_Spectrum.Enable(numSamples, fftWindow, bandType, spacing, padding);
				}
				else
				{
					m_Spectrum.Disable();
				}
			}
		}

		public void UpdateSpectrum()
		{
			if ((Object)(object)m_AudioSource != (Object)null)
			{
				m_Spectrum.Update(m_AudioSource);
			}
		}

		private AudioSource CreateAudioSource(GameObject listener)
		{
			AudioSource obj = listener.AddComponent<AudioSource>();
			obj.outputAudioMixerGroup = m_RadioGroup;
			obj.playOnAwake = false;
			obj.spatialBlend = 0f;
			return obj;
		}

		public void Create(GameObject listener)
		{
			m_AudioSource = CreateAudioSource(listener);
			m_Spectrum = new Spectrum();
		}

		public void Dispose()
		{
			if (m_Spectrum != null)
			{
				m_Spectrum.Disable();
			}
			if ((Object)(object)m_AudioSource != (Object)null)
			{
				m_AudioSource.Stop();
				Object.Destroy((Object)(object)m_AudioSource);
				m_AudioSource = null;
			}
		}

		public static double GetDuration(AudioClip clip)
		{
			return (double)clip.samples / (double)clip.frequency;
		}

		public double GetAudioSourceDuration()
		{
			if (isCreated)
			{
				if (!((Object)(object)m_AudioSource.clip != (Object)null))
				{
					return 0.0;
				}
				return GetDuration(m_AudioSource.clip);
			}
			return 0.0;
		}

		public double GetAudioSourceTimeElapsed()
		{
			if (isCreated)
			{
				if (!((Object)(object)m_AudioSource.clip != (Object)null))
				{
					return 0.0;
				}
				return (double)m_AudioSource.timeSamples / (double)m_AudioSource.clip.frequency;
			}
			return 0.0;
		}

		public double GetAudioSourceTimeRemaining()
		{
			if (isCreated)
			{
				return GetAudioSourceDuration() - (m_Elapsed + (double)((float)m_Timer.ElapsedMilliseconds / 1000f));
			}
			return 0.0;
		}

		public void Rewind()
		{
			if ((Object)(object)m_AudioSource.clip != (Object)null)
			{
				m_AudioSource.timeSamples = 0;
				if (m_AudioSource.isPlaying)
				{
					m_AudioSource.Play();
				}
				m_Elapsed = GetAudioSourceTimeElapsed();
				m_Timer.Restart();
			}
		}

		public void Play(AudioClip clip, int timeSamples = 0)
		{
			if (!((Object)(object)m_AudioSource == (Object)null))
			{
				m_AudioSource.clip = clip;
				m_AudioSource.timeSamples = timeSamples;
				m_AudioSource.Play();
				m_Elapsed = GetAudioSourceTimeElapsed();
				m_Timer.Restart();
			}
		}
	}

	public class Program
	{
		public string name;

		public string description;

		public string icon;

		public string startTime;

		public string endTime;

		public bool loopProgram;

		public bool pairIntroOutro;

		public Segment[] segments;
	}

	public class RuntimeProgram : IJsonWritable
	{
		public string name;

		public string description;

		public int startTime;

		public int endTime;

		public bool loopProgram;

		public bool active;

		public bool hasEnded;

		private int m_CurrentSegmentId;

		private List<RuntimeSegment> m_Segments = new List<RuntimeSegment>();

		public int duration => endTime - startTime;

		public RuntimeSegment currentSegment
		{
			get
			{
				if (m_CurrentSegmentId < m_Segments.Count)
				{
					return m_Segments[m_CurrentSegmentId];
				}
				return null;
			}
		}

		public IReadOnlyList<RuntimeSegment> segments
		{
			get
			{
				return m_Segments;
			}
			set
			{
				m_Segments = (List<RuntimeSegment>)value;
			}
		}

		public bool GoToNextSegment()
		{
			m_CurrentSegmentId++;
			if (m_CurrentSegmentId >= m_Segments.Count)
			{
				log.DebugFormat("Program {0} has ended (last segment)", (object)name);
				hasEnded = true;
				return false;
			}
			return true;
		}

		public void Reset()
		{
			m_CurrentSegmentId = 0;
			hasEnded = false;
		}

		private AudioAsset[] GetClips(int count, Func<int, int> rand, List<AudioAsset> clips)
		{
			AudioAsset[] array = (AudioAsset[])(object)new AudioAsset[count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = clips[rand(i)];
			}
			return array;
		}

		public void BuildRuntimeSegments(Program program, string path)
		{
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			if (program.segments == null)
			{
				return;
			}
			Segment[] array = program.segments;
			foreach (Segment segment in array)
			{
				if (segment.type == SegmentType.Commercial || segment.type == SegmentType.PSA)
				{
					RuntimeSegment item = new RuntimeSegment
					{
						type = segment.type,
						tags = segment.tags,
						clipsCap = segment.clipsCap
					};
					m_Segments.Add(item);
					continue;
				}
				List<AudioAsset> list = new List<AudioAsset>();
				if (segment.clips != null)
				{
					list.Capacity += segment.clips.Length;
					list.AddRange(segment.clips);
				}
				if (segment.tags != null)
				{
					IEnumerable<AudioAsset> assets = AssetDatabase.global.GetAssets<AudioAsset>(SearchFilter<AudioAsset>.ByCondition((Func<AudioAsset, bool>)((AudioAsset asset) => ((IEnumerable<string>)segment.tags).All((Func<string, bool>)((AssetData)asset).ContainsTag)), false));
					list.AddRange(assets);
				}
				if (list.Count > 0)
				{
					AudioAsset[] clips;
					switch (segment.type)
					{
					case SegmentType.Playlist:
					{
						Random rnd = new Random();
						List<int> randomNumbers = (from x in Enumerable.Range(0, list.Count)
							orderby rnd.Next()
							select x).Take(list.Count).ToList();
						clips = GetClips(list.Count, (int index) => randomNumbers[index], list);
						break;
					}
					case SegmentType.Talkshow:
					case SegmentType.News:
						clips = GetClips(list.Count, (int result) => result, list);
						break;
					default:
						clips = Array.Empty<AudioAsset>();
						break;
					}
					RuntimeSegment item2 = new RuntimeSegment
					{
						type = segment.type,
						tags = segment.tags,
						clipsCap = segment.clipsCap,
						clips = clips
					};
					m_Segments.Add(item2);
				}
				else
				{
					log.WarnFormat("No clips found in a segment '{2}' of program '{0}' founds in radio channel '{1}'. Tags: {3}", (object)program.name, (object)path, (object)segment.type, (object)string.Join(", ", segment.tags));
				}
			}
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("name");
			writer.Write(name);
			writer.PropertyName("description");
			writer.Write(description);
			writer.PropertyName("startTime");
			writer.Write(startTime / 60);
			writer.PropertyName("endTime");
			writer.Write(endTime / 60);
			writer.PropertyName("duration");
			writer.Write(duration / 60);
			writer.PropertyName("active");
			writer.Write(active);
			writer.TypeEnd();
		}
	}

	public enum SegmentType
	{
		Playlist,
		Talkshow,
		PSA,
		Weather,
		News,
		Commercial,
		Emergency
	}

	public class Segment
	{
		public SegmentType type;

		public AudioAsset[] clips;

		public string[] tags;

		public int clipsCap;
	}

	public class RuntimeSegment
	{
		public SegmentType type;

		public string[] tags;

		public int clipsCap;

		private int m_CapCount;

		private int m_CurrentClipId;

		private IReadOnlyList<AudioAsset> m_Clips;

		public AudioAsset currentClip
		{
			get
			{
				if (m_CurrentClipId < m_Clips.Count)
				{
					return m_Clips[m_CurrentClipId];
				}
				return null;
			}
		}

		public bool isSetUp { get; private set; }

		public IReadOnlyList<AudioAsset> clips
		{
			get
			{
				return m_Clips;
			}
			set
			{
				if (m_Clips == value)
				{
					return;
				}
				m_Clips = value;
				durationMs = 0.0;
				foreach (AudioAsset clip in m_Clips)
				{
					durationMs += clip.durationMs;
				}
			}
		}

		public double durationMs { get; private set; }

		public bool GoToNextClip()
		{
			m_CapCount++;
			m_CurrentClipId++;
			if (m_CurrentClipId >= m_Clips.Count)
			{
				log.Debug((object)"Segment has ended (last clip)");
				Reset();
				return false;
			}
			if (m_CapCount >= clipsCap)
			{
				log.DebugFormat("Segment has ended (cap count reached {0}/{1})", (object)m_CapCount, (object)clipsCap);
				m_CapCount = 0;
				return false;
			}
			return true;
		}

		public bool GoToPreviousClip()
		{
			m_CurrentClipId--;
			if (m_CurrentClipId < 0)
			{
				m_CurrentClipId = 0;
				return false;
			}
			return true;
		}

		public void Reset()
		{
			m_CurrentClipId = 0;
			m_CapCount = 0;
			isSetUp = false;
		}

		public void Setup(OnDemandClips clipsCallback = null)
		{
			if (!isSetUp)
			{
				isSetUp = true;
				clipsCallback?.Invoke(this);
			}
		}
	}

	public class Spectrum
	{
		public enum BandType
		{
			FourBand,
			FourBandVisual,
			EightBand,
			TenBand,
			TwentySixBand,
			ThirtyOneBand
		}

		[BurstCompile]
		private struct CreateLevels : IJob
		{
			private NativeArray<float> m_FrequenciesForBands;

			private float m_BandWidth;

			private float m_Falldown;

			private float m_Filter;

			private int m_SpectrumLength;

			private float m_OutputSampleRate;

			[NativeDisableUnsafePtrRestriction]
			private unsafe void* m_SpectrumData;

			private int m_LevelsLength;

			[NativeDisableUnsafePtrRestriction]
			private unsafe void* m_Levels;

			public unsafe CreateLevels(ref NativeArray<float> frequenciesForBands, float bandWidth, float[] spectrumData, Vector4[] levels, float fallSpeed, float sensitivity)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				m_FrequenciesForBands = frequenciesForBands;
				m_BandWidth = bandWidth;
				m_SpectrumLength = spectrumData.Length;
				m_SpectrumData = UnsafeUtility.AddressOf<float>(ref spectrumData[0]);
				m_LevelsLength = levels.Length;
				m_Levels = UnsafeUtility.AddressOf<Vector4>(ref levels[0]);
				m_Falldown = fallSpeed * Time.deltaTime;
				m_Filter = Mathf.Exp((0f - sensitivity) * Time.deltaTime);
				m_OutputSampleRate = AudioSettings.outputSampleRate;
			}

			private int FrequencyToSpectrumIndex(float f)
			{
				return math.clamp((int)math.floor(f / m_OutputSampleRate * 2f * (float)m_SpectrumLength), 0, m_SpectrumLength - 1);
			}

			public unsafe void Execute()
			{
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				Vector4 val = default(Vector4);
				for (int i = 0; i < m_LevelsLength; i++)
				{
					int num = FrequencyToSpectrumIndex(m_FrequenciesForBands[i] / m_BandWidth);
					int num2 = FrequencyToSpectrumIndex(m_FrequenciesForBands[i] * m_BandWidth);
					float num3 = 0f;
					for (int j = num; j <= num2; j++)
					{
						num3 = math.max(num3, UnsafeUtility.ReadArrayElement<float>(m_SpectrumData, j));
					}
					val.x = num3;
					val.y = num3 - (num3 - UnsafeUtility.ReadArrayElement<Vector4>(m_Levels, i).y) * m_Filter;
					val.z = math.max(UnsafeUtility.ReadArrayElement<Vector4>(m_Levels, i).z - m_Falldown, num3);
					val.w = 0f;
					UnsafeUtility.WriteArrayElement<Vector4>(m_Levels, i, val);
				}
			}
		}

		private static readonly float[][] kMiddleFrequenciesForBands = new float[6][]
		{
			new float[4] { 125f, 500f, 1000f, 2000f },
			new float[4] { 250f, 400f, 600f, 800f },
			new float[8] { 63f, 125f, 500f, 1000f, 2000f, 4000f, 6000f, 8000f },
			new float[10] { 31.5f, 63f, 125f, 250f, 500f, 1000f, 2000f, 4000f, 8000f, 16000f },
			new float[26]
			{
				25f, 31.5f, 40f, 50f, 63f, 80f, 100f, 125f, 160f, 200f,
				250f, 315f, 400f, 500f, 630f, 800f, 1000f, 1250f, 1600f, 2000f,
				2500f, 3150f, 4000f, 5000f, 6300f, 8000f
			},
			new float[31]
			{
				20f, 25f, 31.5f, 40f, 50f, 63f, 80f, 100f, 125f, 160f,
				200f, 250f, 315f, 400f, 500f, 630f, 800f, 1000f, 1250f, 1600f,
				2000f, 2500f, 3150f, 4000f, 5000f, 6300f, 8000f, 10000f, 12500f, 16000f,
				20000f
			}
		};

		private static readonly float[] kBandwidthForBands = new float[6] { 1.414f, 1.26f, 1.414f, 1.414f, 1.122f, 1.122f };

		private static readonly string[] kKeywords = new string[6] { "EQ_FOUR_BAND", "EQ_FOUR_BAND_VISUAL", "EQ_HEIGHT_BAND", "EQ_TEN_BAND", "EQ_TWENTYSIX_BAND", "EQ_THIRTYONE_BAND" };

		private float[] m_SpectrumData;

		private NativeArray<float> m_Frequencies;

		private float m_Bandwidth;

		private Vector4[] m_Levels;

		private FFTWindow m_FFTWindow;

		private BandType m_BandType;

		private RenderTexture m_VURender;

		private Material m_Equalizer;

		private RenderTargetIdentifier m_VURenderId;

		private const int kTexWidth = 96;

		private const int kTexHeight = 36;

		public Texture equalizerTexture => (Texture)(object)m_VURender;

		public void Enable(int samplesCount, FFTWindow fftWindow, BandType bandType, float spacing = 10f, float padding = 2f)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Expected O, but got Unknown
			if (m_Frequencies.IsCreated)
			{
				Disable();
			}
			m_FFTWindow = fftWindow;
			m_BandType = bandType;
			m_SpectrumData = new float[samplesCount];
			int num = kMiddleFrequenciesForBands[(int)m_BandType].Length;
			m_Frequencies = new NativeArray<float>(kMiddleFrequenciesForBands[(int)m_BandType], (Allocator)4);
			m_Bandwidth = kBandwidthForBands[(int)m_BandType];
			m_Levels = (Vector4[])(object)new Vector4[num];
			m_VURender = new RenderTexture(96, 36, 0, (GraphicsFormat)8, 0)
			{
				name = "RadioEqualizer",
				hideFlags = (HideFlags)61
			};
			m_VURender.Create();
			m_VURenderId = new RenderTargetIdentifier((Texture)(object)m_VURender);
			m_Equalizer = new Material(Shader.Find("Hidden/HDRP/Radio/Equalizer"));
			m_Equalizer.SetFloat("_Padding", padding / 96f);
			m_Equalizer.SetFloat("_Spacing", spacing / 96f);
			m_Equalizer.EnableKeyword(kKeywords[(int)m_BandType]);
			RenderPipelineManager.beginFrameRendering += SpectrumBlit;
		}

		public void Disable()
		{
			RenderPipelineManager.beginFrameRendering -= SpectrumBlit;
			if ((Object)(object)m_Equalizer != (Object)null)
			{
				Object.Destroy((Object)(object)m_Equalizer);
			}
			if ((Object)(object)m_VURender != (Object)null)
			{
				m_VURender.Release();
				Object.Destroy((Object)(object)m_VURender);
			}
			m_SpectrumData = null;
			if (m_Frequencies.IsCreated)
			{
				m_Frequencies.Dispose();
			}
			m_Levels = null;
		}

		private void SpectrumBlit(ScriptableRenderContext context, Camera[] camera)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			CommandBuffer val = CommandBufferPool.Get("BlitRadio");
			m_Equalizer.SetVectorArray("_Levels", m_Levels);
			val.Blit((Texture)null, m_VURenderId, m_Equalizer);
			((ScriptableRenderContext)(ref context)).ExecuteCommandBuffer(val);
			CommandBufferPool.Release(val);
			((Texture)m_VURender).IncrementUpdateCount();
		}

		public void Update(AudioSource source)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)source != (Object)null && m_Frequencies.IsCreated)
			{
				source.GetSpectrumData(m_SpectrumData, 0, m_FFTWindow);
				CreateLevels createLevels = new CreateLevels(ref m_Frequencies, m_Bandwidth, m_SpectrumData, m_Levels, 0.25f, 4f);
				JobHandle val = default(JobHandle);
				val = IJobExtensions.Schedule<CreateLevels>(createLevels, val);
				((JobHandle)(ref val)).Complete();
			}
		}
	}

	public OnRadioEvent Reloaded;

	public OnRadioEvent SettingsChanged;

	public OnRadioEvent ProgramChanged;

	public OnClipChanged ClipChanged;

	private const float kSimulationSecondsPerDay = 4369.067f;

	private const float kSimtimeToRealtime = 0.050567903f;

	private static readonly string kAlertsTag = "type:Alerts";

	private static readonly string kAlertsIntroTag = "type:Alerts Intro";

	private static ILog log = LogManager.GetLogger("Radio");

	private const int kSecondsPerDay = 86400;

	private Dictionary<string, RadioNetwork> m_Networks = new Dictionary<string, RadioNetwork>();

	private Dictionary<string, RuntimeRadioChannel> m_RadioChannels = new Dictionary<string, RuntimeRadioChannel>();

	private RuntimeRadioChannel m_CurrentChannel;

	private bool m_Paused;

	private bool m_Muted;

	private static readonly int kMaxHistoryLength = 5;

	private int m_ReplayIndex;

	private List<ClipInfo> m_PlaylistHistory = new List<ClipInfo>(kMaxHistoryLength);

	private List<ClipInfo> m_Queue = new List<ClipInfo>(2);

	private RadioPlayer m_RadioPlayer;

	private bool m_IsEnabled;

	private string m_LastSaveRadioChannel;

	private bool m_LastSaveRadioAdsState;

	private bool m_IsActive;

	private RuntimeRadioChannel[] m_CachedRadioChannelDescriptors;

	private Dictionary<SegmentType, OnDemandClips> m_OnDemandClips;

	private const string kUniqueNameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

	public RuntimeRadioChannel currentChannel
	{
		get
		{
			return m_CurrentChannel;
		}
		set
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurrentChannel != value)
			{
				m_CurrentChannel = value;
				if (currentClip.m_Emergency == Entity.Null)
				{
					FinishCurrentClip();
				}
				SetupOrSkipSegment();
				ClearQueue();
				m_ReplayIndex = 0;
			}
		}
	}

	public bool paused
	{
		get
		{
			return m_Paused;
		}
		set
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			m_Paused = value;
			if (currentClip.m_Emergency == Entity.Null)
			{
				if (m_Paused)
				{
					m_RadioPlayer.Pause();
				}
				else
				{
					m_RadioPlayer.Unpause();
				}
			}
		}
	}

	public bool skipAds { get; set; }

	public bool muted
	{
		get
		{
			return m_Muted;
		}
		set
		{
			m_Muted = value;
			m_RadioPlayer.muted = value;
		}
	}

	public ClipInfo currentClip { get; private set; }

	public bool isEnabled => m_IsEnabled;

	public bool isActive
	{
		get
		{
			return m_IsActive;
		}
		set
		{
			if (m_IsActive != value)
			{
				m_IsActive = value;
				SettingsChanged?.Invoke(this);
			}
			if (!value)
			{
				OnDisabled();
			}
		}
	}

	public RadioNetwork[] networkDescriptors => GetSortedUIDescriptor(m_Networks);

	public RuntimeRadioChannel[] radioChannelDescriptors
	{
		get
		{
			if (m_CachedRadioChannelDescriptors == null)
			{
				m_CachedRadioChannelDescriptors = GetSortedUIDescriptor(m_RadioChannels);
			}
			return m_CachedRadioChannelDescriptors;
		}
	}

	public string currentlyPlayingClipName => m_RadioPlayer.currentClipName;

	public double currentlyPlayingDuration => m_RadioPlayer.GetAudioSourceDuration();

	public double currentlyPlayingElapsed => m_RadioPlayer.GetAudioSourceTimeElapsed();

	public double currentlyPlayingRemaining => m_RadioPlayer.GetAudioSourceTimeRemaining();

	public double nextTimeCheck => 0.0;

	public AudioAsset pendingClip
	{
		get
		{
			if (m_Queue.Count <= 0)
			{
				return null;
			}
			return m_Queue[0].m_Asset;
		}
	}

	public bool hasEmergency => currentClip.m_Emergency != Entity.Null;

	public Entity emergency => currentClip.m_Emergency;

	public Entity emergencyTarget => currentClip.m_EmergencyTarget;

	public Texture equalizerTexture => m_RadioPlayer.equalizerTexture;

	public int GetActiveSource()
	{
		return 0;
	}

	public double GetAudioSourceDuration(int i)
	{
		return m_RadioPlayer.GetAudioSourceDuration();
	}

	public double GetAudioSourceTimeElapsed(int i)
	{
		return m_RadioPlayer.GetAudioSourceTimeElapsed();
	}

	public double GetAudioSourceTimeRemaining(int i)
	{
		return m_RadioPlayer.GetAudioSourceTimeRemaining();
	}

	private T[] GetSortedUIDescriptor<T>(Dictionary<string, T> desc) where T : IComparable<T>
	{
		T[] array = desc.Values.ToArray();
		Array.Sort(array);
		return array;
	}

	public RuntimeRadioChannel GetRadioChannel(string name)
	{
		if (m_RadioChannels.TryGetValue(name, out var value))
		{
			return value;
		}
		return null;
	}

	public Radio(AudioMixerGroup radioGroup)
	{
		m_OnDemandClips = new Dictionary<SegmentType, OnDemandClips>();
		m_OnDemandClips[SegmentType.Commercial] = GetCommercialClips;
		m_OnDemandClips[SegmentType.PSA] = GetPSAClips;
		m_OnDemandClips[SegmentType.Playlist] = GetPlaylistClips;
		m_OnDemandClips[SegmentType.Talkshow] = GetTalkShowClips;
		m_OnDemandClips[SegmentType.News] = GetNewsClips;
		m_OnDemandClips[SegmentType.Weather] = GetWeatherClips;
		m_RadioPlayer = new RadioPlayer(radioGroup);
		SettingsChanged = (OnRadioEvent)Delegate.Combine(SettingsChanged, new OnRadioEvent(OnSettingsChanged));
	}

	private void OnSettingsChanged(Radio radio)
	{
		if (radio.isActive)
		{
			if ((Object)(object)GameManager.instance != (Object)null && GameManager.instance.gameMode == GameMode.Game && (Object)(object)Camera.main != (Object)null && radio.radioChannelDescriptors.Length != 0)
			{
				radio.Enable(((Component)Camera.main).gameObject);
			}
		}
		else
		{
			Disable();
		}
	}

	public void ForceRadioPause(bool pause)
	{
		if (pause)
		{
			m_RadioPlayer.Pause();
		}
		else
		{
			m_RadioPlayer.Unpause();
		}
	}

	public void SetSpectrumSettings(bool enabled, int numSamples, FFTWindow fftWindow, Spectrum.BandType bandType, float spacing, float padding)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_RadioPlayer.SetSpectrumSettings(enabled, numSamples, fftWindow, bandType, spacing, padding);
	}

	private bool CheckEntitlement(IContentPrerequisite target)
	{
		if (target.contentPrerequisites != null)
		{
			return target.contentPrerequisites.All(delegate(string x)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				DlcId dlcId = PlatformManager.instance.GetDlcId(x);
				return PlatformManager.instance.IsDlcOwned(dlcId);
			});
		}
		return true;
	}

	private void LoadRadio(bool enable)
	{
		try
		{
			Clear();
			PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
			{
				log.DebugFormat("Loaded radio configuration in {0}ms", (object)t.TotalMilliseconds);
			});
			try
			{
				AssetDatabase.global.LoadSettings<RadioNetwork>("Radio Network", (Action<RadioNetwork, SourceMeta>)delegate(RadioNetwork network, SourceMeta meta)
				{
					if (CheckEntitlement(network))
					{
						m_Networks.Add(network.name, network);
					}
				});
				AssetDatabase.global.LoadSettings<RadioChannel>("Radio Channel", (Action<RadioChannel, SourceMeta>)delegate(RadioChannel channel, SourceMeta meta)
				{
					//IL_004b: Unknown result type (might be due to invalid IL or missing references)
					if (CheckEntitlement(channel))
					{
						string text = channel.name;
						while (m_RadioChannels.ContainsKey(text))
						{
							text = text + "_" + MakeUniqueRandomName(text, 4);
						}
						log.InfoFormat("Radio channel id '{0}' added", (object)text);
						m_RadioChannels.Add(text, channel.CreateRuntime(meta.path));
					}
				});
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			LogRadio();
			if (enable)
			{
				Enable(((Component)Camera.main).gameObject);
			}
		}
		catch (Exception ex)
		{
			log.Error(ex);
		}
	}

	private void Clear()
	{
		m_CachedRadioChannelDescriptors = null;
		m_Networks.Clear();
		m_RadioChannels.Clear();
		currentChannel = null;
		OnDisabled();
	}

	public void Reload(bool enable = true)
	{
		LoadRadio(enable);
		Reloaded?.Invoke(this);
	}

	public void RestoreRadioSettings(string savedChannel, bool savedAds)
	{
		m_LastSaveRadioChannel = savedChannel;
		m_LastSaveRadioAdsState = savedAds;
	}

	public void Enable(GameObject listener)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (currentChannel == null)
		{
			if (m_LastSaveRadioChannel != null && m_RadioChannels.TryGetValue(m_LastSaveRadioChannel, out var value))
			{
				currentChannel = value;
				skipAds = m_LastSaveRadioAdsState;
			}
			else
			{
				skipAds = false;
				currentChannel = radioChannelDescriptors[0];
			}
		}
		if (m_IsActive && !m_IsEnabled && (Object)(object)listener != (Object)null)
		{
			m_RadioPlayer.Create(listener);
			m_RadioPlayer.muted = m_Muted;
			SetSpectrumSettings(SharedSettings.instance.radio.enableSpectrum, SharedSettings.instance.radio.spectrumNumSamples, SharedSettings.instance.radio.fftWindowType, SharedSettings.instance.radio.bandType, SharedSettings.instance.radio.equalizerBarSpacing, SharedSettings.instance.radio.equalizerSidesPadding);
			m_IsEnabled = true;
		}
	}

	public void Disable()
	{
		m_RadioPlayer?.Dispose();
		m_IsEnabled = false;
		OnDisabled();
	}

	private void OnDisabled()
	{
		FinishCurrentClip();
		ClearQueue(clearEmergencies: true);
		m_ReplayIndex = 0;
		if (currentClip.m_LoadTask != null)
		{
			((AssetData)currentClip.m_Asset).Unload(false);
		}
		currentClip = default(ClipInfo);
		m_PlaylistHistory.Clear();
	}

	public void Update(float normalizedTime)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		if (!isActive || !isEnabled)
		{
			ClearEmergencyQueue();
			return;
		}
		try
		{
			m_RadioPlayer.UpdateSpectrum();
			int timeOfDaySeconds = Mathf.RoundToInt(normalizedTime * 24f * 3600f);
			bool flag = false;
			bool flag2 = false;
			RuntimeRadioChannel[] array = radioChannelDescriptors;
			foreach (RuntimeRadioChannel obj in array)
			{
				bool flag3 = obj.Update(timeOfDaySeconds);
				if (obj == currentChannel)
				{
					flag = flag3;
				}
				flag2 = flag2 || flag3;
			}
			if (flag)
			{
				log.DebugFormat("Program changed callback for on-demand clips initialization", Array.Empty<object>());
				SetupOrSkipSegment();
			}
			QueueEmergencyClips();
			ValidateQueue();
			if (m_Queue.Count > 0)
			{
				ClipInfo clipInfo = m_Queue[0];
				if (currentClip.m_Emergency == Entity.Null && clipInfo.m_Emergency != Entity.Null)
				{
					if (clipInfo.m_LoadTask != null && clipInfo.m_LoadTask.IsCompleted)
					{
						m_RadioPlayer.Unpause();
						ClipInfo clip = currentClip;
						clip.m_ResumeAtPosition = m_RadioPlayer.playbackPosition;
						m_RadioPlayer.Play(clipInfo.m_LoadTask.Result);
						currentClip = clipInfo;
						m_Queue.RemoveAt(0);
						QueueClip(clip);
						InvokeClipcallback(currentClip.m_Asset);
					}
				}
				else if (m_RadioPlayer.GetAudioSourceTimeRemaining() <= 0.0 && clipInfo.m_LoadTask != null && clipInfo.m_LoadTask.IsCompleted)
				{
					if (clipInfo.m_SegmentType == SegmentType.Commercial && skipAds)
					{
						((AssetData)clipInfo.m_Asset).Unload(false);
						m_Queue.RemoveAt(0);
					}
					else
					{
						m_RadioPlayer.Play(clipInfo.m_LoadTask.Result, (clipInfo.m_ResumeAtPosition >= 0) ? clipInfo.m_ResumeAtPosition : 0);
						if (currentClip.m_LoadTask != null)
						{
							((AssetData)currentClip.m_Asset).Unload(false);
						}
						currentClip = clipInfo;
						if (currentClip.m_SegmentType == SegmentType.Playlist && currentClip.m_ResumeAtPosition < 0 && !currentClip.m_Replaying)
						{
							ClipInfo item = currentClip;
							item.m_LoadTask = null;
							m_PlaylistHistory.Insert(0, item);
							while (m_PlaylistHistory.Count > kMaxHistoryLength)
							{
								m_PlaylistHistory.RemoveAt(m_PlaylistHistory.Count - 1);
							}
						}
						if (!currentClip.m_Replaying)
						{
							m_ReplayIndex = 0;
						}
						m_Queue.RemoveAt(0);
						if (paused && currentClip.m_Emergency == Entity.Null)
						{
							m_RadioPlayer.Pause();
						}
						InvokeClipcallback(currentClip.m_Asset);
					}
				}
			}
			QueueNextClip();
			if (flag2)
			{
				ProgramChanged(this);
			}
		}
		catch (Exception ex)
		{
			log.Fatal(ex);
		}
	}

	private void QueueClip(ClipInfo clip, bool pushToFront = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (clip.m_Emergency != Entity.Null || clip.m_ResumeAtPosition >= 0 || pushToFront)
		{
			int num = m_Queue.FindIndex((ClipInfo info) => info.m_Emergency == Entity.Null);
			m_Queue.Insert((num < 0) ? m_Queue.Count : num, clip);
		}
		else
		{
			m_Queue.Add(clip);
		}
	}

	private void ValidateQueue()
	{
		for (int i = 0; i < m_Queue.Count; i++)
		{
			while (i < m_Queue.Count && (m_Queue[i].m_LoadTask == null || m_Queue[i].m_LoadTask.IsFaulted || m_Queue[i].m_LoadTask.IsCanceled))
			{
				if (m_Queue[i].m_LoadTask != null)
				{
					((AssetData)m_Queue[i].m_Asset).Unload(false);
				}
				m_Queue.RemoveAt(i);
			}
		}
	}

	private void ClearQueue(bool clearEmergencies = false)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Queue.Count; i++)
		{
			if (m_Queue[i].m_LoadTask != null && (clearEmergencies || m_Queue[i].m_Emergency == Entity.Null))
			{
				((AssetData)m_Queue[i].m_Asset).Unload(false);
			}
		}
		if (clearEmergencies)
		{
			m_Queue.Clear();
			return;
		}
		m_Queue.RemoveAll((ClipInfo clip) => clip.m_Emergency == Entity.Null);
	}

	private void ClearEmergencyQueue()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeQueue<RadioTag> emergencyQueue = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RadioTagSystem>().GetEmergencyQueue(out deps);
		((JobHandle)(ref deps)).Complete();
		emergencyQueue.Clear();
	}

	private void QueueEmergencyClips()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		NativeQueue<RadioTag> emergencyQueue = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RadioTagSystem>().GetEmergencyQueue(out deps);
		((JobHandle)(ref deps)).Complete();
		while (emergencyQueue.Count > 0)
		{
			RadioTag tag = emergencyQueue.Dequeue();
			if (IsEmergencyClipInQueue(tag))
			{
				continue;
			}
			List<AudioAsset> list = new List<AudioAsset>();
			PrefabBase prefab = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>().GetPrefab<PrefabBase>(tag.m_Event);
			foreach (AudioAsset asset in AssetDatabase.global.GetAssets<AudioAsset>(SearchFilter<AudioAsset>.ByCondition((Func<AudioAsset, bool>)((AudioAsset asset) => ((AssetData)asset).ContainsTag(kAlertsTag)), false)))
			{
				if (asset.GetMetaTag((Metatag)8) == ((Object)prefab).name)
				{
					list.Add(asset);
				}
			}
			if (list.Count > 0)
			{
				if (!AlertPlayingOrQueued())
				{
					QueueEmergencyIntroClip(tag.m_Event, tag.m_Target);
				}
				Random val = new Random((uint)DateTime.Now.Ticks);
				AudioAsset val2 = list[((Random)(ref val)).NextInt(0, list.Count)];
				QueueClip(new ClipInfo
				{
					m_Asset = val2,
					m_Emergency = tag.m_Event,
					m_EmergencyTarget = tag.m_Target,
					m_SegmentType = SegmentType.Emergency,
					m_LoadTask = val2.LoadAsync(true, true, (AudioType)14),
					m_ResumeAtPosition = -1
				});
			}
		}
		emergencyQueue.Clear();
	}

	private void QueueEmergencyIntroClip(Entity emergency, Entity emergencyTarget)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		List<AudioAsset> list = new List<AudioAsset>();
		foreach (AudioAsset asset in AssetDatabase.global.GetAssets<AudioAsset>(SearchFilter<AudioAsset>.ByCondition((Func<AudioAsset, bool>)((AudioAsset asset) => ((AssetData)asset).ContainsTag(kAlertsIntroTag)), false)))
		{
			list.Add(asset);
		}
		if (list.Count > 0)
		{
			Random val = new Random((uint)DateTime.Now.Ticks);
			AudioAsset val2 = list[((Random)(ref val)).NextInt(0, list.Count)];
			QueueClip(new ClipInfo
			{
				m_Asset = val2,
				m_Emergency = emergency,
				m_EmergencyTarget = emergencyTarget,
				m_SegmentType = SegmentType.Emergency,
				m_LoadTask = val2.LoadAsync(true, true, (AudioType)14),
				m_ResumeAtPosition = -1
			});
		}
	}

	private bool IsEmergencyClipInQueue(RadioTag tag)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (currentClip.m_Emergency != Entity.Null && currentClip.m_Emergency == tag.m_Event)
		{
			return true;
		}
		for (int i = 0; i < m_Queue.Count; i++)
		{
			if (m_Queue[i].m_Emergency != Entity.Null && m_Queue[i].m_Emergency == tag.m_Event)
			{
				return true;
			}
		}
		return false;
	}

	private bool AlertPlayingOrQueued()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!(currentClip.m_Emergency != Entity.Null))
		{
			if (m_Queue.Count > 0)
			{
				return m_Queue[0].m_Emergency != Entity.Null;
			}
			return false;
		}
		return true;
	}

	private void QueueNextClip()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (m_Queue.Count == 0 && (AssetData)(object)currentChannel?.currentProgram?.currentSegment?.currentClip != (IAssetData)null)
		{
			QueueClip(new ClipInfo
			{
				m_Asset = currentChannel.currentProgram.currentSegment.currentClip,
				m_SegmentType = currentChannel.currentProgram.currentSegment.type,
				m_Emergency = Entity.Null,
				m_LoadTask = currentChannel.currentProgram.currentSegment.currentClip.LoadAsync(true, true, (AudioType)14),
				m_ResumeAtPosition = -1
			});
			SetupNextClip();
		}
	}

	private void GetPSAClips(RuntimeSegment segment)
	{
		if (m_Networks.TryGetValue(currentChannel.network, out var value) && !value.allowAds)
		{
			List<AudioAsset> eventClips = GetEventClips(segment, (Metatag)7);
			segment.clips = eventClips;
			Log(segment);
		}
	}

	private void GetNewsClips(RuntimeSegment segment)
	{
		List<AudioAsset> eventClips = GetEventClips(segment, (Metatag)9);
		segment.clips = eventClips;
		Log(segment);
	}

	private void GetWeatherClips(RuntimeSegment segment)
	{
		List<AudioAsset> eventClips = GetEventClips(segment, (Metatag)10, newestFirst: true, flush: true);
		segment.clips = eventClips;
		Log(segment);
	}

	private List<AudioAsset> GetEventClips(RuntimeSegment segment, Metatag metatag, bool newestFirst = false, bool flush = false)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		RadioTagSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<RadioTagSystem>();
		PrefabSystem orCreateSystemManaged = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
		List<AudioAsset> list = new List<AudioAsset>(segment.clipsCap);
		List<AudioAsset> list2 = new List<AudioAsset>();
		RadioTag radioTag;
		while (list.Count < segment.clipsCap && existingSystemManaged.TryPopEvent(segment.type, newestFirst, out radioTag))
		{
			list2.Clear();
			foreach (AudioAsset asset in AssetDatabase.global.GetAssets<AudioAsset>(SearchFilter<AudioAsset>.ByCondition((Func<AudioAsset, bool>)((AudioAsset asset) => ((IEnumerable<string>)segment.tags).All((Func<string, bool>)((AssetData)asset).ContainsTag)), false)))
			{
				if (asset.GetMetaTag(metatag) == ((Object)orCreateSystemManaged.GetPrefab<PrefabBase>(radioTag.m_Event)).name)
				{
					list2.Add(asset);
				}
			}
			if (list2.Count > 0)
			{
				Random val = new Random((uint)DateTime.Now.Ticks);
				list.Add(list2[((Random)(ref val)).NextInt(0, list2.Count)]);
			}
		}
		if (flush)
		{
			existingSystemManaged.FlushEvents(segment.type);
		}
		return list;
	}

	private void GetCommercialClips(RuntimeSegment segment)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Networks.TryGetValue(currentChannel.network, out var value) || !value.allowAds)
		{
			return;
		}
		WeightedRandom<AudioAsset> val = new WeightedRandom<AudioAsset>();
		Dictionary<string, List<AudioAsset>> dictionary = new Dictionary<string, List<AudioAsset>>();
		foreach (AudioAsset asset in AssetDatabase.global.GetAssets<AudioAsset>(SearchFilter<AudioAsset>.ByCondition((Func<AudioAsset, bool>)((AudioAsset asset) => ((IEnumerable<string>)segment.tags).All((Func<string, bool>)((AssetData)asset).ContainsTag)), false)))
		{
			string metaTag = asset.GetMetaTag((Metatag)4);
			if (metaTag != null)
			{
				if (!dictionary.TryGetValue(metaTag, out var value2))
				{
					value2 = new List<AudioAsset>();
					dictionary.Add(metaTag, value2);
				}
				value2.Add(asset);
			}
			else
			{
				log.ErrorFormat("Asset {0} ({1}) does not contain a brand metatag (for Commercial segment)", (object)((AssetData)asset).id, (object)(asset.GetMetaTag((Metatag)0) ?? "<No title>"));
			}
		}
		LogMap(dictionary);
		JobHandle dependency;
		NativeList<BrandPopularitySystem.BrandPopularity> brandPopularity = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BrandPopularitySystem>().ReadBrandPopularity(out dependency);
		((JobHandle)(ref dependency)).Complete();
		LogBrandPopularity(brandPopularity);
		for (int num = 0; num < brandPopularity.Length; num++)
		{
			if (World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>().TryGetPrefab<BrandPrefab>(brandPopularity[num].m_BrandPrefab, out var prefab) && dictionary.TryGetValue(((Object)prefab).name, out var value3))
			{
				val.AddRange((IEnumerable<AudioAsset>)value3, brandPopularity[num].m_Popularity);
			}
		}
		List<AudioAsset> list = new List<AudioAsset>();
		for (int num2 = 0; num2 < segment.clipsCap; num2++)
		{
			AudioAsset val2 = val.NextAndRemove();
			if ((AssetData)(object)val2 != (IAssetData)null)
			{
				list.Add(val2);
			}
		}
		segment.clips = list;
		Log(segment);
	}

	private void GetPlaylistClips(RuntimeSegment segment)
	{
		segment.clips = GetSegmentAudioClip(segment.clipsCap, segment.tags, segment.type);
	}

	private void GetTalkShowClips(RuntimeSegment segment)
	{
		segment.clips = GetSegmentAudioClip(segment.clipsCap, segment.tags, segment.type);
	}

	private AudioAsset[] GetSegmentAudioClip(int clipsCap, string[] requiredTags, SegmentType segmentType)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<AudioAsset> assets = AssetDatabase.global.GetAssets<AudioAsset>(SearchFilter<AudioAsset>.ByCondition((Func<AudioAsset, bool>)((AudioAsset asset) => ((IEnumerable<string>)requiredTags).All((Func<string, bool>)((AssetData)asset).ContainsTag)), false));
		List<AudioAsset> list = new List<AudioAsset>();
		list.AddRange(assets);
		Random rnd = new Random();
		List<int> list2 = (from x in Enumerable.Range(0, list.Count)
			orderby rnd.Next()
			select x).Take(clipsCap).ToList();
		AudioAsset[] array = (AudioAsset[])(object)new AudioAsset[clipsCap];
		for (int num = 0; num < array.Length; num++)
		{
			array[num] = list[list2[num]];
		}
		return array;
	}

	private void LogMap(Dictionary<string, List<AudioAsset>> map)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!log.isDebugEnabled)
		{
			return;
		}
		string text = "Audio asset map:\n";
		foreach (KeyValuePair<string, List<AudioAsset>> item in map)
		{
			text = text + item.Key + "\n";
			foreach (AudioAsset item2 in item.Value)
			{
				text += $"  {item2.GetMetaTag((Metatag)0)} ({((AssetData)item2).id})\n";
			}
		}
		log.Verbose((object)text);
	}

	private void LogBrandPopularity(NativeList<BrandPopularitySystem.BrandPopularity> brandPopularity)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (log.isDebugEnabled)
		{
			string text = "Brands popularity:\n";
			PrefabSystem orCreateSystemManaged = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
			for (int i = 0; i < brandPopularity.Length; i++)
			{
				string prefabName = orCreateSystemManaged.GetPrefabName(brandPopularity[i].m_BrandPrefab);
				text += $"{prefabName} - {brandPopularity[i].m_Popularity}\n";
			}
			log.Verbose((object)text);
		}
	}

	private bool SetupNextClip()
	{
		if (currentChannel?.currentProgram?.currentSegment == null)
		{
			return false;
		}
		if (!currentChannel.currentProgram.currentSegment.GoToNextClip())
		{
			currentChannel.currentProgram.GoToNextSegment();
			if (!SetupOrSkipSegment())
			{
				return false;
			}
		}
		return true;
	}

	private bool SetupOrSkipSegment()
	{
		if (currentChannel?.currentProgram == null)
		{
			return false;
		}
		RuntimeProgram currentProgram = currentChannel.currentProgram;
		while (true)
		{
			RuntimeSegment currentSegment = currentProgram.currentSegment;
			if (currentSegment == null)
			{
				return false;
			}
			if (m_OnDemandClips.TryGetValue(currentSegment.type, out var value))
			{
				value(currentSegment);
			}
			if (currentSegment.clips.Count != 0)
			{
				break;
			}
			if (!currentProgram.GoToNextSegment())
			{
				return false;
			}
		}
		return true;
	}

	private void InvokeClipcallback(AudioAsset currentAsset)
	{
		try
		{
			ClipChanged?.Invoke(this, currentAsset);
		}
		catch (Exception ex)
		{
			log.Critical(ex);
		}
	}

	public void NextSong()
	{
		if (m_ReplayIndex > 0)
		{
			m_ReplayIndex--;
			ClipInfo clip = m_PlaylistHistory[m_ReplayIndex];
			clip.m_Replaying = true;
			clip.m_LoadTask = clip.m_Asset.LoadAsync(true, true, (AudioType)14);
			QueueClip(clip, pushToFront: true);
		}
		FinishCurrentClip();
	}

	public void PreviousSong()
	{
		if (m_RadioPlayer.GetAudioSourceTimeElapsed() > 2.0 || m_ReplayIndex >= m_PlaylistHistory.Count - 1)
		{
			m_RadioPlayer.Rewind();
			return;
		}
		m_ReplayIndex++;
		ClipInfo clip = m_PlaylistHistory[m_ReplayIndex];
		clip.m_Replaying = true;
		clip.m_LoadTask = clip.m_Asset.LoadAsync(true, true, (AudioType)14);
		QueueClip(clip, pushToFront: true);
		FinishCurrentClip();
	}

	private void FinishCurrentClip()
	{
		m_RadioPlayer.Play(null);
	}

	private static void SupportValueTypesForAOT()
	{
		JSON.SupportTypeForAOT<SegmentType>();
	}

	private static string MakeUniqueName(string name, int length)
	{
		char[] array = new char[length];
		for (int i = 0; i < name.Length; i++)
		{
			array[i % (length - 1)] += name[i];
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"[array[j] % "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Length];
		}
		return new string(array);
	}

	private static string MakeUniqueRandomName(string name, int length)
	{
		char[] array = new char[length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"[Random.Range(0, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Length) % "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Length];
		}
		return new string(array);
	}

	private void Log(RadioNetwork network)
	{
		log.Debug((object)("name: " + network.name));
		Indent scoped = log.indent.scoped;
		try
		{
			log.Verbose((object)("description: " + network.description));
			log.Verbose((object)("icon: " + network.icon));
			log.Verbose((object)$"uiPriority: {network.uiPriority}");
			log.Verbose((object)$"allowAds: {network.allowAds}");
		}
		finally
		{
			((IDisposable)scoped)?.Dispose();
		}
	}

	private void Log(RuntimeRadioChannel channel)
	{
		log.Debug((object)("name: " + channel.name));
		Indent scoped = log.indent.scoped;
		try
		{
			log.Verbose((object)("description: " + channel.description));
			log.Verbose((object)("icon: " + channel.icon));
			log.Verbose((object)$"uiPriority: {channel.uiPriority}");
			log.Verbose((object)("network: " + channel.network));
			log.DebugFormat("Programs ({0})", (object)channel.schedule.Length);
			Indent scoped2 = log.indent.scoped;
			try
			{
				RuntimeProgram[] schedule = channel.schedule;
				foreach (RuntimeProgram program in schedule)
				{
					Log(program);
				}
			}
			finally
			{
				((IDisposable)scoped2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)scoped)?.Dispose();
		}
	}

	private void Log(AudioAsset clip)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((AssetData)(object)clip == (IAssetData)null)
		{
			log.Debug((object)"id: <missing>");
		}
		else
		{
			log.Debug((object)string.Format("id: {0} tags: {1} duration: {2}", ((AssetData)clip).id, string.Join(", ", ((AssetData)clip).tags), FormatUtils.FormatTimeDebug(clip.durationMs)));
		}
	}

	private void Log(RuntimeProgram program)
	{
		log.Debug((object)("name: " + program.name + " [" + FormatUtils.FormatTimeDebug(program.startTime) + " -> " + FormatUtils.FormatTimeDebug(program.endTime) + "]"));
		Indent scoped = log.indent.scoped;
		try
		{
			log.Verbose((object)("description: " + program.description));
			log.Verbose((object)$"estimatedStart: {FormatUtils.FormatTimeDebug(program.startTime)} ({program.startTime}s)");
			log.Verbose((object)$"estimatedEnd: {FormatUtils.FormatTimeDebug(program.endTime)} ({program.endTime}s)");
			log.Verbose((object)$"loopProgram: {program.loopProgram}");
			log.Verbose((object)$"estimatedDuration: {FormatUtils.FormatTimeDebug(program.duration)} ({program.duration}s) (realtime at x1: {FormatUtils.FormatTimeDebug((int)((float)program.duration * 0.050567903f))})");
			log.DebugFormat("Segments ({0})", (object)program.segments.Count);
			Indent scoped2 = log.indent.scoped;
			try
			{
				foreach (RuntimeSegment segment in program.segments)
				{
					Log(segment);
				}
			}
			finally
			{
				((IDisposable)scoped2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)scoped)?.Dispose();
		}
	}

	private void Log(RuntimeSegment segment)
	{
		log.Debug((object)$"type: {segment.type}");
		Indent scoped = log.indent.scoped;
		try
		{
			if (segment.tags != null)
			{
				log.Debug((object)("tags: " + string.Join(", ", segment.tags)));
			}
			if (segment.clips == null)
			{
				return;
			}
			log.Verbose((object)$"duration total: {segment.durationMs}ms ({FormatUtils.FormatTimeDebug(segment.durationMs)})");
			log.DebugFormat("Clips ({0})", (object)segment.clips.Count);
			Indent scoped2 = log.indent.scoped;
			try
			{
				foreach (AudioAsset clip in segment.clips)
				{
					Log(clip);
				}
			}
			finally
			{
				((IDisposable)scoped2)?.Dispose();
			}
			log.DebugFormat("Clips cap: {0}", (object)segment.clipsCap);
		}
		finally
		{
			((IDisposable)scoped)?.Dispose();
		}
	}

	private void LogRadio()
	{
		if (!log.isDebugEnabled)
		{
			return;
		}
		log.DebugFormat("Networks ({0})", (object)m_Networks.Count);
		Indent scoped = log.indent.scoped;
		try
		{
			foreach (RadioNetwork value in m_Networks.Values)
			{
				Log(value);
			}
		}
		finally
		{
			((IDisposable)scoped)?.Dispose();
		}
		log.DebugFormat("Channels ({0})", (object)m_RadioChannels.Count);
		scoped = log.indent.scoped;
		try
		{
			foreach (RuntimeRadioChannel value2 in m_RadioChannels.Values)
			{
				Log(value2);
			}
		}
		finally
		{
			((IDisposable)scoped)?.Dispose();
		}
	}
}
