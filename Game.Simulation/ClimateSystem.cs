using System;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Effects;
using Game.Prefabs;
using Game.Prefabs.Climate;
using Game.Rendering;
using Game.Serialization;
using Game.Triggers;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

public class ClimateSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPreSerialize, IPostDeserialize
{
	[Serializable]
	public class SeasonInfo : IJsonWritable, IJsonReadable
	{
		public SeasonPrefab m_Prefab;

		public string m_NameID;

		public string m_IconPath;

		public float m_StartTime;

		public float2 m_TempNightDay = new float2(5f, 20f);

		public float2 m_TempDeviationNightDay = new float2(4f, 7f);

		public float m_CloudChance = 50f;

		public float m_CloudAmount = 40f;

		public float m_CloudAmountDeviation = 20f;

		public float m_PrecipitationChance = 30f;

		public float m_PrecipitationAmount = 40f;

		public float m_PrecipitationAmountDeviation = 30f;

		public float m_Turbulence = 0.2f;

		public float m_AuroraAmount = 1f;

		public float m_AuroraChance = 10f;

		public void Write(IJsonWriter writer)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin("Season");
			writer.PropertyName("name");
			writer.Write(m_NameID);
			writer.PropertyName("startTime");
			writer.Write(m_StartTime);
			writer.PropertyName("tempNightDay");
			MathematicsWriters.Write(writer, m_TempNightDay);
			writer.PropertyName("tempDeviationNightDay");
			MathematicsWriters.Write(writer, m_TempDeviationNightDay);
			writer.PropertyName("cloudChance");
			writer.Write(m_CloudChance);
			writer.PropertyName("cloudAmount");
			writer.Write(m_CloudAmount);
			writer.PropertyName("cloudAmountDeviation");
			writer.Write(m_CloudAmountDeviation);
			writer.PropertyName("precipitationChance");
			writer.Write(m_PrecipitationChance);
			writer.PropertyName("precipitationAmount");
			writer.Write(m_PrecipitationAmount);
			writer.PropertyName("precipitationAmountDeviation");
			writer.Write(m_PrecipitationAmountDeviation);
			writer.PropertyName("turbulence");
			writer.Write(m_Turbulence);
			writer.PropertyName("auroraAmount");
			writer.Write(m_AuroraAmount);
			writer.PropertyName("auroraChance");
			writer.Write(m_AuroraChance);
			writer.TypeEnd();
		}

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("name");
			reader.Read(ref m_NameID);
			reader.ReadProperty("startTime");
			reader.Read(ref m_StartTime);
			reader.ReadProperty("tempNightDay");
			MathematicsReaders.Read(reader, ref m_TempNightDay);
			reader.ReadProperty("tempDeviationNightDay");
			MathematicsReaders.Read(reader, ref m_TempDeviationNightDay);
			reader.ReadProperty("cloudChance");
			reader.Read(ref m_CloudChance);
			reader.ReadProperty("cloudAmount");
			reader.Read(ref m_CloudAmount);
			reader.ReadProperty("cloudAmountDeviation");
			reader.Read(ref m_CloudAmountDeviation);
			reader.ReadProperty("precipitationChance");
			reader.Read(ref m_PrecipitationChance);
			reader.ReadProperty("precipitationAmount");
			reader.Read(ref m_PrecipitationAmount);
			reader.ReadProperty("precipitationAmountDeviation");
			reader.Read(ref m_PrecipitationAmountDeviation);
			reader.ReadProperty("turbulence");
			reader.Read(ref m_Turbulence);
			reader.ReadProperty("auroraAmount");
			reader.Read(ref m_AuroraAmount);
			reader.ReadProperty("auroraChance");
			reader.Read(ref m_AuroraChance);
			reader.ReadMapEnd();
		}
	}

	public enum WeatherClassification
	{
		Irrelevant,
		Clear,
		Few,
		Scattered,
		Broken,
		Overcast,
		Stormy
	}

	public struct ClimateSample
	{
		public float temperature;

		public float precipitation;

		public float cloudiness;

		public float aurora;

		public float fog;
	}

	private struct WeatherTempData : IComparable<WeatherTempData>
	{
		public Entity m_Entity;

		public float m_Priority;

		public int CompareTo(WeatherTempData other)
		{
			return m_Priority.CompareTo(other.m_Priority);
		}
	}

	public OverridableProperty<float> thunder = new OverridableProperty<float>();

	private TriggerSystem m_TriggerSystem;

	private PrefabSystem m_PrefabSystem;

	private TimeSystem m_TimeSystem;

	private ClimateRenderSystem m_ClimateRenderSystem;

	private PlanetarySystem m_PlanetarySystem;

	private EntityQuery m_ClimateQuery;

	private OverridableProperty<float> m_Date;

	private Entity m_CurrentClimate;

	private NativeList<Entity> m_CurrentWeatherEffects;

	private NativeList<Entity> m_NextWeatherEffects;

	private float m_TemperatureBaseHeight;

	private SeasonInfo m_CurrentSeason;

	private static readonly int[,] kLut = new int[12, 5]
	{
		{ 33, 15, 32, 10, 10 },
		{ 31, 18, 31, 10, 10 },
		{ 31, 21, 28, 10, 10 },
		{ 23, 18, 30, 10, 19 },
		{ 22, 20, 23, 10, 25 },
		{ 21, 19, 24, 10, 26 },
		{ 19, 18, 26, 10, 27 },
		{ 18, 22, 23, 10, 27 },
		{ 25, 23, 24, 10, 18 },
		{ 29, 19, 32, 10, 10 },
		{ 30, 16, 34, 10, 10 },
		{ 34, 15, 31, 10, 10 }
	};

	private static readonly int[] kSampleTimes = new int[3] { 7, 13, 19 };

	public float2 wind { get; private set; } = new float2(0.0275f, 0.0275f);

	public float hail { get; set; }

	public float rainbow { get; set; }

	public float aerosolDensity { get; private set; }

	public float seasonTemperature { get; private set; }

	public float seasonPrecipitation { get; private set; }

	public float seasonCloudiness { get; private set; }

	public OverridableProperty<float> currentDate => m_Date;

	public OverridableProperty<float> precipitation { get; } = new OverridableProperty<float>();

	public OverridableProperty<float> temperature { get; } = new OverridableProperty<float>();

	public OverridableProperty<float> cloudiness { get; } = new OverridableProperty<float>();

	public OverridableProperty<float> aurora { get; } = new OverridableProperty<float>();

	public OverridableProperty<float> fog { get; } = new OverridableProperty<float>();

	public Entity currentClimate
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_CurrentClimate;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Assert.AreNotEqual<Entity>(Entity.Null, value);
			m_CurrentClimate = value;
			ClimatePrefab prefab = m_PrefabSystem.GetPrefab<ClimatePrefab>(m_CurrentClimate);
			prefab.EnsureSeasonsOrder(force: true);
			averageTemperature = CalculateTemperatureAverage(prefab);
			UpdateSeason(prefab, m_Date);
			if (m_CurrentWeatherEffects.Length == 0 && m_NextWeatherEffects.Length == 0)
			{
				UpdateWeather(prefab);
			}
		}
	}

	public float temperatureBaseHeight => m_TemperatureBaseHeight;

	public float snowTemperatureHeightScale => 0.01f;

	public float averageTemperature { get; private set; }

	public float freezingTemperature { get; private set; }

	public bool isRaining
	{
		get
		{
			if ((float)precipitation > 0f)
			{
				return (float)temperature > freezingTemperature;
			}
			return false;
		}
	}

	public bool isSnowing
	{
		get
		{
			if ((float)precipitation > 0f)
			{
				return (float)temperature <= freezingTemperature;
			}
			return false;
		}
	}

	public bool isPrecipitating => (float)precipitation > 0f;

	public WeatherClassification classification { get; private set; }

	public Entity currentSeason
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (m_CurrentSeason == null)
			{
				return Entity.Null;
			}
			return m_PrefabSystem.GetEntity(m_CurrentSeason.m_Prefab);
		}
	}

	public string currentSeasonNameID => m_CurrentSeason?.m_NameID;

	public void PatchReferences(ref PrefabReferences references)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		m_CurrentClimate = references.Check(((ComponentSystemBase)this).EntityManager, m_CurrentClimate);
		for (int i = 0; i < m_CurrentWeatherEffects.Length; i++)
		{
			m_CurrentWeatherEffects[i] = references.Check(((ComponentSystemBase)this).EntityManager, m_CurrentWeatherEffects[i]);
		}
		for (int j = 0; j < m_NextWeatherEffects.Length; j++)
		{
			m_NextWeatherEffects[j] = references.Check(((ComponentSystemBase)this).EntityManager, m_NextWeatherEffects[j]);
		}
	}

	private float CalculateMeanTemperatureStandard(ClimatePrefab prefab, int resolutionPerDay, out float meanMin, out float meanMax)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (prefab.m_Seasons != null)
		{
			int length = prefab.m_Temperature.length;
			int num = m_TimeSystem.daysPerYear * resolutionPerDay;
			float2 val = float2.zero;
			for (int i = 0; i < m_TimeSystem.daysPerYear; i++)
			{
				float2 zero = float2.zero;
				for (int j = 0; j < resolutionPerDay; j++)
				{
					float num2 = (float)(i + j) / (float)num * (float)length;
					float num3 = prefab.m_Temperature.Evaluate(num2);
					zero.x = math.min(zero.x, num3);
					zero.y = math.max(zero.y, num3);
				}
				val += zero;
			}
			float2 val2 = val / (float)m_TimeSystem.daysPerYear;
			meanMin = val2.x;
			meanMax = val2.y;
			return (val2.x + val2.y) * 0.5f;
		}
		meanMin = 0f;
		meanMax = 0f;
		return 0f;
	}

	private float CalculateMeanTemperatureEkholmModen(ClimatePrefab prefab, int resolutionPerDay)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		Assert.AreEqual(12, m_TimeSystem.daysPerYear);
		if (prefab.m_Seasons != null)
		{
			int daysPerYear = m_TimeSystem.daysPerYear;
			int num = daysPerYear + (kSampleTimes.Length + 2);
			float num2 = 0f;
			for (int i = 0; i < daysPerYear; i++)
			{
				float num3 = 0f;
				for (int j = 0; j < kSampleTimes.Length; j++)
				{
					float num4 = (float)(kSampleTimes[j] + i) / (float)num * (float)daysPerYear;
					float num5 = prefab.m_Temperature.Evaluate(num4);
					num3 += num5 * (float)kLut[i, j];
				}
				float2 zero = float2.zero;
				for (int k = 0; k < resolutionPerDay; k++)
				{
					float num6 = (float)(i + k - 5) / (float)num * (float)daysPerYear;
					float num7 = prefab.m_Temperature.Evaluate(num6);
					zero.x = math.min(zero.x, num7);
					zero.y = math.max(zero.y, num7);
				}
				num3 += zero.x * (float)kLut[i, 3];
				num3 += zero.y * (float)kLut[i, 3];
				num2 += num3 / 100f;
			}
			return num2 / (float)m_TimeSystem.daysPerYear;
		}
		return 0f;
	}

	private float CalculateMeanPrecipitation(ClimatePrefab prefab, int resolutionPerDay = 48, float startRange = 0f, float endRange = 1f)
	{
		int daysPerYear = m_TimeSystem.daysPerYear;
		float num = startRange * (float)daysPerYear;
		float num2 = endRange * (float)daysPerYear;
		int num3 = (int)math.round((num2 - num) * (float)resolutionPerDay);
		float num4 = 0f;
		for (int i = 0; i < num3; i++)
		{
			float num5 = (float)i / (float)num3;
			num4 += prefab.m_Precipitation.Evaluate(math.lerp(num, num2, num5));
		}
		return num4 / (float)num3;
	}

	private float CalculateMeanTemperature(ClimatePrefab prefab, int resolutionPerDay = 48, float startRange = 0f, float endRange = 1f)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		int daysPerYear = m_TimeSystem.daysPerYear;
		float num = startRange * (float)daysPerYear;
		float num2 = endRange * (float)daysPerYear;
		int num3 = (int)math.round((num2 - num) * (float)resolutionPerDay);
		float2 zero = float2.zero;
		for (int i = 0; i < num3; i++)
		{
			float num4 = (float)i / (float)num3;
			float num5 = prefab.m_Temperature.Evaluate(math.lerp(num, num2, num4));
			zero.x = math.min(zero.x, num5);
			zero.y = math.max(zero.y, num5);
		}
		return (zero.x + zero.y) * 0.5f;
	}

	private float CalculateMeanCloudiness(ClimatePrefab prefab, int resolutionPerDay = 48, float startRange = 0f, float endRange = 1f)
	{
		int daysPerYear = m_TimeSystem.daysPerYear;
		float num = startRange * (float)daysPerYear;
		float num2 = endRange * (float)daysPerYear;
		int num3 = (int)math.round((num2 - num) * (float)resolutionPerDay);
		float num4 = 0f;
		for (int i = 0; i < num3; i++)
		{
			float num5 = (float)i / (float)num3;
			num4 += prefab.m_Cloudiness.Evaluate(math.lerp(num, num2, num5));
		}
		return num4 / (float)num3;
	}

	private float CalculateTemperatureAverage(ClimatePrefab prefab, int resolutionPerDay = 48)
	{
		freezingTemperature = prefab.m_FreezingTemperature;
		if (m_TimeSystem.daysPerYear == 12)
		{
			return CalculateMeanTemperatureEkholmModen(prefab, resolutionPerDay);
		}
		float meanMin;
		float meanMax;
		return CalculateMeanTemperatureStandard(prefab, resolutionPerDay, out meanMin, out meanMax);
	}

	private float CalculateTemperatureBaseHeight()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		TerrainSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		WaterSystem orCreateSystemManaged2 = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		MapTileSystem orCreateSystemManaged3 = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTileSystem>();
		TerrainHeightData terrainData = orCreateSystemManaged.GetHeightData();
		JobHandle deps;
		WaterSurfaceData data = orCreateSystemManaged2.GetSurfaceData(out deps);
		((JobHandle)(ref deps)).Complete();
		NativeList<Entity> startTiles = orCreateSystemManaged3.GetStartTiles();
		float num = 0f;
		float num2 = 0f;
		DynamicBuffer<Node> val = default(DynamicBuffer<Node>);
		for (int i = 0; i < startTiles.Length; i++)
		{
			if (EntitiesExtensions.TryGetBuffer<Node>(((ComponentSystemBase)this).EntityManager, startTiles[i], true, ref val))
			{
				for (int j = 0; j < val.Length; j++)
				{
					num += WaterUtils.SampleHeight(ref data, ref terrainData, val[j].m_Position);
					num2 += 1f;
				}
			}
		}
		if (!(num2 > 0f))
		{
			return 0f;
		}
		return num / num2;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_ClimateRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateRenderSystem>();
		m_PlanetarySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PlanetarySystem>();
		m_CurrentWeatherEffects = new NativeList<Entity>(0, AllocatorHandle.op_Implicit((Allocator)4));
		m_NextWeatherEffects = new NativeList<Entity>(0, AllocatorHandle.op_Implicit((Allocator)4));
		m_ClimateQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ClimateData>() });
		m_Date = new OverridableProperty<float>(() => m_TimeSystem.normalizedDate);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_CurrentWeatherEffects.Dispose();
		m_NextWeatherEffects.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (currentClimate != Entity.Null)
		{
			ClimatePrefab prefab = m_PrefabSystem.GetPrefab<ClimatePrefab>(currentClimate);
			ClimateSample climateSample = SampleClimate(prefab, m_Date);
			temperature.value = climateSample.temperature;
			precipitation.value = climateSample.precipitation;
			cloudiness.value = climateSample.cloudiness;
			aurora.value = climateSample.aurora;
			fog.value = climateSample.fog;
			UpdateSeason(prefab, m_Date);
			UpdateWeather(prefab);
		}
		if (((ComponentSystemBase)m_TriggerSystem).Enabled)
		{
			HandleTriggers();
		}
	}

	private void HandleTriggers()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<TriggerAction> val = m_TriggerSystem.CreateActionBuffer();
		val.Enqueue(new TriggerAction(TriggerType.Temperature, Entity.Null, temperature));
		bool flag = hail > 0.001f;
		bool flag2 = classification == WeatherClassification.Overcast;
		bool flag3 = m_TimeSystem.normalizedTime >= EffectFlagSystem.kDayBegin && m_TimeSystem.normalizedTime < EffectFlagSystem.kNightBegin;
		bool flag4 = classification == WeatherClassification.Stormy;
		if (flag || flag4)
		{
			val.Enqueue(new TriggerAction(TriggerType.WeatherStormy, Entity.Null, 0f));
		}
		else if (!flag && isRaining)
		{
			val.Enqueue(new TriggerAction(((float)temperature > 0f) ? TriggerType.WeatherRainy : TriggerType.WeatherSnowy, Entity.Null, 0f));
		}
		else if (!flag && !isRaining && !flag2 && flag3)
		{
			val.Enqueue(new TriggerAction(((float)temperature > 15f) ? TriggerType.WeatherSunny : TriggerType.WeatherClear, Entity.Null, 0f));
		}
		else if (flag2)
		{
			val.Enqueue(new TriggerAction(TriggerType.WeatherCloudy, Entity.Null, 0f));
		}
		if (!flag3)
		{
			val.Enqueue(new TriggerAction(TriggerType.AuroraBorealis, Entity.Null, aurora));
		}
	}

	public void PreSerialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)((Context)(ref context)).purpose == 3)
		{
			m_TemperatureBaseHeight = CalculateTemperatureBaseHeight();
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_CurrentClimate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		NativeList<Entity> currentWeatherEffects = m_CurrentWeatherEffects;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(currentWeatherEffects);
		NativeList<Entity> nextWeatherEffects = m_NextWeatherEffects;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nextWeatherEffects);
		float num = m_TemperatureBaseHeight;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
	}

	public void SetDefaults(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_CurrentClimate = Entity.Null;
		m_CurrentWeatherEffects.ResizeUninitialized(0);
		m_NextWeatherEffects.ResizeUninitialized(0);
		m_TemperatureBaseHeight = 0f;
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		ref Entity reference = ref m_CurrentClimate;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		NativeList<Entity> currentWeatherEffects = m_CurrentWeatherEffects;
		((IReader)reader/*cast due to .constrained prefix*/).Read(currentWeatherEffects);
		NativeList<Entity> nextWeatherEffects = m_NextWeatherEffects;
		((IReader)reader/*cast due to .constrained prefix*/).Read(nextWeatherEffects);
		ref float reference2 = ref m_TemperatureBaseHeight;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
	}

	public void PostDeserialize(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (m_CurrentClimate == Entity.Null || !m_PrefabSystem.TryGetPrefab<ClimatePrefab>(m_CurrentClimate, out var _))
		{
			if (m_CurrentClimate != Entity.Null)
			{
				COSystemBase.baseLog.Error((object)"Missing climate prefab, reverting to default climate");
			}
			NativeArray<Entity> val = ((EntityQuery)(ref m_ClimateQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				if (val.Length > 0)
				{
					m_CurrentClimate = val[0];
				}
			}
			finally
			{
				((IDisposable)val/*cast due to .constrained prefix*/).Dispose();
			}
		}
		if (m_CurrentClimate != Entity.Null)
		{
			ClimatePrefab prefab2 = m_PrefabSystem.GetPrefab<ClimatePrefab>(m_CurrentClimate);
			prefab2.EnsureSeasonsOrder(force: true);
			averageTemperature = CalculateTemperatureAverage(prefab2);
			UpdateSeason(prefab2, m_Date);
			if (AreEffectsInvalid(m_CurrentWeatherEffects) || m_CurrentWeatherEffects.Length == 0 || AreEffectsInvalid(m_NextWeatherEffects) || m_NextWeatherEffects.Length == 0)
			{
				UpdateWeather(prefab2);
			}
			else
			{
				ApplyWeatherEffects();
			}
			m_PlanetarySystem.latitude = prefab2.m_Latitude;
			m_PlanetarySystem.longitude = prefab2.m_Longitude;
		}
	}

	private bool AreEffectsInvalid(NativeList<Entity> list)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i] == Entity.Null)
			{
				list.ResizeUninitialized(0);
				return true;
			}
		}
		return false;
	}

	public ClimateSample SampleClimate(ClimatePrefab prefab, float t)
	{
		float num = t * (float)m_TimeSystem.daysPerYear;
		float num2 = prefab.m_Temperature.Evaluate(num);
		float num3 = prefab.m_Precipitation.Evaluate(num);
		float num4 = prefab.m_Cloudiness.Evaluate(num);
		float num5 = prefab.m_Aurora.Evaluate(num);
		float num6 = prefab.m_Aurora.Evaluate(num);
		return new ClimateSample
		{
			temperature = num2,
			precipitation = num3,
			cloudiness = num4,
			aurora = num5,
			fog = num6
		};
	}

	public ClimateSample SampleClimate(float t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (m_CurrentClimate != Entity.Null)
		{
			ClimatePrefab prefab = m_PrefabSystem.GetPrefab<ClimatePrefab>(m_CurrentClimate);
			ClimateSample result = SampleClimate(prefab, t);
			if (temperature.overrideState)
			{
				result.temperature = temperature.overrideValue;
			}
			if (precipitation.overrideState)
			{
				result.precipitation = precipitation.overrideValue;
			}
			if (cloudiness.overrideState)
			{
				result.cloudiness = cloudiness.overrideValue;
			}
			if (aurora.overrideState)
			{
				result.aurora = aurora.overrideValue;
			}
			if (fog.overrideState)
			{
				result.fog = fog.overrideValue;
			}
			return result;
		}
		return default(ClimateSample);
	}

	private void UpdateSeason(ClimatePrefab prefab, float normalizedDate)
	{
		SeasonInfo seasonInfo = m_CurrentSeason;
		float startRange;
		float endRange;
		(m_CurrentSeason, startRange, endRange) = prefab.FindSeasonByTime(normalizedDate);
		if (seasonInfo != m_CurrentSeason)
		{
			seasonTemperature = CalculateMeanTemperature(prefab, 48, startRange, endRange);
			seasonPrecipitation = CalculateMeanPrecipitation(prefab, 48, startRange, endRange);
			seasonCloudiness = CalculateMeanCloudiness(prefab, 48, startRange, endRange);
		}
	}

	private bool SelectDefaultWeather(ClimatePrefab prefab, ref NativeList<WeatherTempData> currentWeathers, ref NativeList<WeatherTempData> nextWeathers)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)prefab.m_DefaultWeather != (Object)null)
		{
			WeatherTempData weatherTempData = new WeatherTempData
			{
				m_Entity = m_PrefabSystem.GetEntity(prefab.m_DefaultWeather),
				m_Priority = -1001f
			};
			currentWeathers.Add(ref weatherTempData);
			nextWeathers.Add(ref weatherTempData);
			return true;
		}
		return false;
	}

	private bool SelectWeatherPlaceholder(ClimatePrefab prefab, out WeatherPrefab current, out WeatherPrefab next)
	{
		if (prefab.m_DefaultWeathers != null)
		{
			float num = float.MaxValue;
			int num2 = 0;
			for (int i = 0; i < prefab.m_DefaultWeathers.Length; i++)
			{
				WeatherPrefab weatherPrefab = prefab.m_DefaultWeathers[i];
				float num3 = math.max(weatherPrefab.m_CloudinessRange.x - (float)cloudiness, (float)cloudiness - weatherPrefab.m_CloudinessRange.y);
				if (num3 < num)
				{
					num2 = i;
					num = num3;
				}
			}
			current = prefab.m_DefaultWeathers[math.max(num2 - 1, 0)];
			next = prefab.m_DefaultWeathers[num2];
			return true;
		}
		current = null;
		next = null;
		return false;
	}

	private void SelectRandomWeather(WeatherPrefab weather, ref NativeList<WeatherTempData> weathers)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		WeatherTempData weatherTempData = default(WeatherTempData);
		weatherTempData.m_Entity = m_PrefabSystem.GetEntity(weather);
		weatherTempData.m_Priority = -1000f;
		weathers.Add(ref weatherTempData);
		DynamicBuffer<PlaceholderObjectElement> val = default(DynamicBuffer<PlaceholderObjectElement>);
		if (!EntitiesExtensions.TryGetBuffer<PlaceholderObjectElement>(((ComponentSystemBase)this).EntityManager, weatherTempData.m_Entity, true, ref val))
		{
			return;
		}
		WeatherTempData weatherTempData2 = default(WeatherTempData);
		DynamicBuffer<ObjectRequirementElement> val2 = default(DynamicBuffer<ObjectRequirementElement>);
		for (int i = 0; i < val.Length; i++)
		{
			weatherTempData2.m_Entity = val[i].m_Object;
			weatherTempData2.m_Priority = 0f;
			if (EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, weatherTempData2.m_Entity, true, ref val2))
			{
				int num = -1;
				bool flag = true;
				for (int j = 0; j < val2.Length; j++)
				{
					ObjectRequirementElement objectRequirementElement = val2[j];
					if ((objectRequirementElement.m_Type & ObjectRequirementType.SelectOnly) != 0)
					{
						continue;
					}
					if (objectRequirementElement.m_Group != num)
					{
						if (!flag)
						{
							break;
						}
						num = objectRequirementElement.m_Group;
						flag = false;
					}
					flag |= objectRequirementElement.m_Requirement == currentSeason;
					weatherTempData2.m_Priority = 1000f;
				}
				if (!flag)
				{
					continue;
				}
			}
			WeatherPrefab prefab = m_PrefabSystem.GetPrefab<WeatherPrefab>(weatherTempData2.m_Entity);
			if ((float)aurora > 0f && prefab.m_RandomizationLayer == WeatherPrefab.RandomizationLayer.Aurora)
			{
				weatherTempData2.m_Priority = 500f;
				weathers.Add(ref weatherTempData2);
			}
			else if (prefab.m_RandomizationLayer == WeatherPrefab.RandomizationLayer.Cloudiness)
			{
				weatherTempData2.m_Priority = 250f;
				weathers.Add(ref weatherTempData2);
			}
			else if (prefab.m_RandomizationLayer == WeatherPrefab.RandomizationLayer.Season)
			{
				weatherTempData2.m_Priority = 300f;
				weathers.Add(ref weatherTempData2);
			}
		}
	}

	private bool ResetWeatherEffects(ref NativeList<Entity> weatherEffects)
	{
		bool result = weatherEffects.Length != 0;
		weatherEffects.Clear();
		return result;
	}

	private bool SortAndCheckUpdate(ref NativeList<WeatherTempData> weatherEffects, ref NativeList<Entity> reference)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		NativeSortExtension.Sort<WeatherTempData>(weatherEffects);
		if (weatherEffects.Length != reference.Length)
		{
			flag = true;
			reference.ResizeUninitialized(weatherEffects.Length);
			for (int i = 0; i < weatherEffects.Length; i++)
			{
				reference[i] = weatherEffects[i].m_Entity;
			}
		}
		else
		{
			for (int j = 0; j < weatherEffects.Length; j++)
			{
				flag |= reference[j] != weatherEffects[j].m_Entity;
				reference[j] = weatherEffects[j].m_Entity;
			}
		}
		return flag;
	}

	private void UpdateWeather(ClimatePrefab prefab)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		NativeList<WeatherTempData> currentWeathers = default(NativeList<WeatherTempData>);
		currentWeathers._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
		NativeList<WeatherTempData> nextWeathers = default(NativeList<WeatherTempData>);
		nextWeathers._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
		if (SelectDefaultWeather(prefab, ref currentWeathers, ref nextWeathers))
		{
			if (SelectWeatherPlaceholder(prefab, out var current, out var next))
			{
				SelectRandomWeather(current, ref currentWeathers);
				SelectRandomWeather(next, ref nextWeathers);
				flag |= SortAndCheckUpdate(ref currentWeathers, ref m_CurrentWeatherEffects);
				flag |= SortAndCheckUpdate(ref nextWeathers, ref m_NextWeatherEffects);
			}
		}
		else
		{
			flag |= ResetWeatherEffects(ref m_CurrentWeatherEffects);
			flag |= ResetWeatherEffects(ref m_NextWeatherEffects);
		}
		currentWeathers.Dispose();
		nextWeathers.Dispose();
		if (flag)
		{
			ApplyWeatherEffects();
		}
	}

	private void ApplyWeatherEffects()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		m_ClimateRenderSystem.Clear();
		for (int i = 0; i < m_CurrentWeatherEffects.Length; i++)
		{
			WeatherPrefab prefab = m_PrefabSystem.GetPrefab<WeatherPrefab>(m_CurrentWeatherEffects[i]);
			if (prefab.m_Classification != WeatherClassification.Irrelevant)
			{
				classification = prefab.m_Classification;
			}
			m_ClimateRenderSystem.ScheduleFrom(prefab);
		}
		for (int j = 0; j < m_NextWeatherEffects.Length; j++)
		{
			WeatherPrefab prefab2 = m_PrefabSystem.GetPrefab<WeatherPrefab>(m_NextWeatherEffects[j]);
			if (prefab2.m_Classification != WeatherClassification.Irrelevant)
			{
				classification = prefab2.m_Classification;
			}
			m_ClimateRenderSystem.ScheduleTo(prefab2);
		}
	}

	[Preserve]
	public ClimateSystem()
	{
	}//IL_000b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0010: Unknown result type (might be due to invalid IL or missing references)

}
