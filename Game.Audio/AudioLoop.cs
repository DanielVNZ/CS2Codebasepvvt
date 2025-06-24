using System.Threading.Tasks;
using Colossal.IO.AssetDatabase;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio;

public class AudioLoop
{
	private const string kMenuCutoffProperty = "MenuCutoff";

	private AudioAsset m_Asset;

	private AudioSource[] m_AudioSource;

	private int m_ActiveAudioSource;

	private double m_NextCheck = -1.0;

	private AudioMixerGroup m_group;

	private AudioMixer m_Mixer;

	private float m_FadeOutTime;

	public float volume
	{
		get
		{
			return m_AudioSource[0].volume;
		}
		set
		{
			m_AudioSource[0].volume = value;
			if (m_AudioSource.Length > 1 && (Object)(object)m_AudioSource[1] != (Object)null)
			{
				m_AudioSource[1].volume = value;
			}
		}
	}

	public bool isPlaying
	{
		get
		{
			if ((Object)(object)m_AudioSource[m_ActiveAudioSource] != (Object)null)
			{
				return m_AudioSource[m_ActiveAudioSource].isPlaying;
			}
			return false;
		}
	}

	public double elapsedTime => (double)m_AudioSource[m_ActiveAudioSource].timeSamples / (double)m_AudioSource[m_ActiveAudioSource].clip.frequency;

	public AudioLoop(AudioAsset asset, AudioMixer mixer, AudioMixerGroup group)
	{
		m_Asset = asset;
		m_group = group;
		m_Mixer = mixer;
	}

	public async Task Start(bool useAlternativeStart = false)
	{
		m_FadeOutTime = 0f;
		m_Mixer.SetFloat("MenuCutoff", 22000f);
		AudioClip val = await m_Asset.LoadAsync(false, true, (AudioType)14);
		if (!((Object)(object)val != (Object)null))
		{
			return;
		}
		m_NextCheck = -1.0;
		m_ActiveAudioSource = 0;
		if (m_AudioSource == null)
		{
			m_AudioSource = (AudioSource[])(object)new AudioSource[(!m_Asset.hasLoop) ? 1 : 2];
			GameObject go = new GameObject("MenuAudioSource");
			m_AudioSource[0] = go.AddComponent<AudioSource>();
			m_AudioSource[0].outputAudioMixerGroup = m_group;
			m_AudioSource[0].dopplerLevel = 0f;
			m_AudioSource[0].playOnAwake = false;
			m_AudioSource[0].spatialBlend = 0f;
			m_AudioSource[0].loop = !m_Asset.hasLoop;
			m_AudioSource[0].clip = val;
			if (m_Asset.hasLoop)
			{
				AudioClip clip = await m_Asset.LoadAsync(false, true, (AudioType)14);
				m_AudioSource[1] = go.AddComponent<AudioSource>();
				m_AudioSource[1].outputAudioMixerGroup = m_group;
				m_AudioSource[1].dopplerLevel = 0f;
				m_AudioSource[1].playOnAwake = false;
				m_AudioSource[1].spatialBlend = 0f;
				m_AudioSource[1].loop = false;
				m_AudioSource[1].clip = clip;
			}
		}
		m_AudioSource[0].volume = 1f;
		if (useAlternativeStart && m_Asset.hasAlternativeStart)
		{
			m_AudioSource[0].timeSamples = (int)(m_Asset.alternativeStart * (double)m_AudioSource[0].clip.frequency);
		}
		if (m_Asset.hasLoop)
		{
			m_AudioSource[1].volume = 1f;
			m_NextCheck = AudioSettings.dspTime + m_Asset.loopEnd;
			if (useAlternativeStart && m_Asset.hasAlternativeStart)
			{
				m_NextCheck -= m_Asset.alternativeStart;
			}
		}
		m_AudioSource[0].PlayScheduled(AudioSettings.dspTime);
	}

	public void Update(double deltaTime)
	{
		if (m_AudioSource != null)
		{
			if (m_Asset.hasLoop && m_NextCheck != -1.0 && AudioSettings.dspTime > m_NextCheck - 5.0)
			{
				int num = 1 - m_ActiveAudioSource;
				m_AudioSource[m_ActiveAudioSource].SetScheduledEndTime(m_NextCheck);
				m_AudioSource[num].timeSamples = (int)(m_Asset.loopStart * (double)m_AudioSource[num].clip.frequency);
				m_AudioSource[num].PlayScheduled(m_NextCheck);
				m_ActiveAudioSource = num;
				m_NextCheck += m_Asset.loopDuration;
			}
			if (m_FadeOutTime > 0f)
			{
				m_FadeOutTime -= (float)deltaTime;
				m_Mixer.SetFloat("MenuCutoff", math.lerp(400f, 22000f, math.saturate(math.pow(m_FadeOutTime, 3f))));
				volume = ((m_Asset.fadeoutTime > 0f) ? (m_FadeOutTime / m_Asset.fadeoutTime) : 0f);
			}
			else if (m_FadeOutTime < 0f)
			{
				Dispose();
			}
		}
	}

	public void FadeOut()
	{
		m_FadeOutTime = m_Asset.fadeoutTime;
	}

	public void Stop()
	{
		if (m_AudioSource == null)
		{
			return;
		}
		AudioSource[] audioSource = m_AudioSource;
		foreach (AudioSource val in audioSource)
		{
			if ((Object)(object)val != (Object)null)
			{
				val.Stop();
			}
		}
	}

	public void Dispose()
	{
		Stop();
		m_NextCheck = -1.0;
		if (m_AudioSource != null)
		{
			if (m_AudioSource.Length > 1 && (Object)(object)m_AudioSource[1] != (Object)null)
			{
				Object.Destroy((Object)(object)m_AudioSource[1].clip);
			}
			if ((Object)(object)m_AudioSource[0] != (Object)null)
			{
				Object.Destroy((Object)(object)m_AudioSource[0].clip);
				Object.Destroy((Object)(object)((Component)m_AudioSource[0]).gameObject);
			}
			m_AudioSource = null;
		}
	}
}
