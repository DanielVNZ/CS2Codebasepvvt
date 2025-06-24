using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.City;
using Game.Companies;
using Game.Debug;
using Game.Prefabs;
using Game.Prefabs.Modes;
using Game.Reflection;
using Game.Triggers;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ResidentialDemandSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct UpdateResidentialDemandJob : IJob
	{
		[ReadOnly]
		public NativeList<Entity> m_UnlockedZonePrefabs;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneDatas;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> m_ZonePropertiesDatas;

		[ReadOnly]
		public NativeList<DemandParameterData> m_DemandParameters;

		[ReadOnly]
		public NativeArray<int> m_StudyPositions;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		[ReadOnly]
		public float m_UnemploymentRate;

		public Entity m_City;

		public NativeValue<int> m_HouseholdDemand;

		public NativeValue<int3> m_BuildingDemand;

		public NativeArray<int> m_LowDemandFactors;

		public NativeArray<int> m_MediumDemandFactors;

		public NativeArray<int> m_HighDemandFactors;

		public CountHouseholdDataSystem.HouseholdData m_HouseholdCountData;

		public CountResidentialPropertySystem.ResidentialPropertyData m_ResidentialPropertyData;

		public Workplaces m_FreeWorkplaces;

		public Workplaces m_TotalWorkplaces;

		public NativeQueue<TriggerAction> m_TriggerQueue;

		public float2 m_ResidentialDemandWeightsSelector;

		public void Execute()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_080b: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0823: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0843: Unknown result type (might be due to invalid IL or missing references)
			//IL_0848: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_086d: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			bool3 val = default(bool3);
			for (int i = 0; i < m_UnlockedZonePrefabs.Length; i++)
			{
				ZoneData zoneData = m_ZoneDatas[m_UnlockedZonePrefabs[i]];
				if (zoneData.m_AreaType == AreaType.Residential)
				{
					ZonePropertiesData zonePropertiesData = m_ZonePropertiesDatas[m_UnlockedZonePrefabs[i]];
					switch (PropertyUtils.GetZoneDensity(zoneData, zonePropertiesData))
					{
					case ZoneDensity.Low:
						val.x = true;
						break;
					case ZoneDensity.Medium:
						val.y = true;
						break;
					case ZoneDensity.High:
						val.z = true;
						break;
					}
				}
			}
			int3 val2 = m_ResidentialPropertyData.m_FreeProperties;
			int3 val3 = m_ResidentialPropertyData.m_TotalProperties;
			DemandParameterData demandParameterData = m_DemandParameters[0];
			int num = 0;
			for (int j = 1; j <= 4; j++)
			{
				num += m_StudyPositions[j];
			}
			Population population = m_Populations[m_City];
			float factorValue = 20f - math.smoothstep(0f, 20f, (float)population.m_Population / 20000f);
			int num2 = math.max(demandParameterData.m_MinimumHappiness, population.m_AverageHappiness);
			float num3 = 0f;
			for (int k = 0; k < 5; k++)
			{
				num3 += (float)(-(TaxSystem.GetResidentialTaxRate(k, m_TaxRates) - 10));
			}
			num3 = demandParameterData.m_TaxEffect.x * (num3 / 5f);
			float factorValue2 = demandParameterData.m_HappinessEffect * (float)(num2 - demandParameterData.m_NeutralHappiness);
			float num4 = (0f - demandParameterData.m_HomelessEffect) * math.clamp(2f * (float)m_HouseholdCountData.m_HomelessHouseholdCount / (float)demandParameterData.m_NeutralHomelessness, 0f, 2f);
			num4 = math.min(num4, (float)kMaxFactorEffect);
			float num5 = demandParameterData.m_HomelessEffect * math.clamp(2f * (float)m_HouseholdCountData.m_HomelessHouseholdCount / (float)demandParameterData.m_NeutralHomelessness, 0f, 2f);
			num5 = math.clamp(num5, 0f, (float)kMaxFactorEffect);
			float num6 = demandParameterData.m_AvailableWorkplaceEffect * ((float)m_FreeWorkplaces.SimpleWorkplacesCount - (float)m_TotalWorkplaces.SimpleWorkplacesCount * demandParameterData.m_NeutralAvailableWorkplacePercentage / 100f);
			num6 = math.clamp(num6, 0f, 40f);
			float num7 = demandParameterData.m_AvailableWorkplaceEffect * ((float)m_FreeWorkplaces.ComplexWorkplacesCount - (float)m_TotalWorkplaces.ComplexWorkplacesCount * demandParameterData.m_NeutralAvailableWorkplacePercentage / 100f);
			num7 = math.clamp(num7, 0f, 20f);
			float factorValue3 = demandParameterData.m_StudentEffect * math.clamp((float)num / 200f, 0f, 5f);
			float factorValue4 = demandParameterData.m_NeutralUnemployment - m_UnemploymentRate;
			factorValue = GetFactorValue(factorValue, m_ResidentialDemandWeightsSelector);
			factorValue2 = GetFactorValue(factorValue2, m_ResidentialDemandWeightsSelector);
			num4 = GetFactorValue(num4, m_ResidentialDemandWeightsSelector);
			num5 = GetFactorValue(num5, m_ResidentialDemandWeightsSelector);
			num3 = GetFactorValue(num3, m_ResidentialDemandWeightsSelector);
			num6 = GetFactorValue(num6, m_ResidentialDemandWeightsSelector);
			num7 = GetFactorValue(num7, m_ResidentialDemandWeightsSelector);
			factorValue3 = GetFactorValue(factorValue3, m_ResidentialDemandWeightsSelector);
			factorValue4 = GetFactorValue(factorValue4, m_ResidentialDemandWeightsSelector);
			m_HouseholdDemand.value = (int)math.min(200f, factorValue + factorValue2 + num4 + num3 + factorValue4 + factorValue3 + math.max(num6, num7));
			int num8 = Mathf.RoundToInt(100f * (float)(demandParameterData.m_FreeResidentialRequirement.x - val2.x) / (float)demandParameterData.m_FreeResidentialRequirement.x);
			int num9 = Mathf.RoundToInt(100f * (float)(demandParameterData.m_FreeResidentialRequirement.y - val2.y) / (float)demandParameterData.m_FreeResidentialRequirement.y);
			int num10 = Mathf.RoundToInt(100f * (float)(demandParameterData.m_FreeResidentialRequirement.z - val2.z) / (float)demandParameterData.m_FreeResidentialRequirement.z);
			m_LowDemandFactors[7] = Mathf.RoundToInt(factorValue2);
			m_LowDemandFactors[6] = Mathf.RoundToInt(num6) / 2;
			m_LowDemandFactors[5] = Mathf.RoundToInt(factorValue4);
			m_LowDemandFactors[11] = Mathf.RoundToInt(num3);
			m_LowDemandFactors[13] = num8;
			m_MediumDemandFactors[7] = Mathf.RoundToInt(factorValue2);
			m_MediumDemandFactors[6] = Mathf.RoundToInt(num6);
			m_MediumDemandFactors[5] = Mathf.RoundToInt(factorValue4);
			m_MediumDemandFactors[11] = Mathf.RoundToInt(num3);
			m_MediumDemandFactors[12] = Mathf.RoundToInt(factorValue3);
			m_MediumDemandFactors[13] = num9;
			m_HighDemandFactors[7] = Mathf.RoundToInt(factorValue2);
			m_HighDemandFactors[8] = Mathf.RoundToInt(num5);
			m_HighDemandFactors[6] = Mathf.RoundToInt(num6);
			m_HighDemandFactors[5] = Mathf.RoundToInt(factorValue4);
			m_HighDemandFactors[11] = Mathf.RoundToInt(num3);
			m_HighDemandFactors[12] = Mathf.RoundToInt(factorValue3);
			m_HighDemandFactors[13] = num10;
			int num11 = ((m_LowDemandFactors[13] >= 0) ? (m_LowDemandFactors[7] + m_LowDemandFactors[11] + m_LowDemandFactors[6] + m_LowDemandFactors[5] + m_LowDemandFactors[13]) : 0);
			int num12 = ((m_MediumDemandFactors[13] >= 0) ? (m_MediumDemandFactors[7] + m_MediumDemandFactors[11] + m_MediumDemandFactors[6] + m_MediumDemandFactors[12] + m_MediumDemandFactors[5] + m_MediumDemandFactors[13]) : 0);
			int num13 = ((m_HighDemandFactors[13] >= 0) ? (m_HighDemandFactors[7] + m_HighDemandFactors[8] + m_HighDemandFactors[11] + m_HighDemandFactors[6] + m_HighDemandFactors[12] + m_HighDemandFactors[5] + m_HighDemandFactors[13]) : 0);
			m_LowDemandFactors[13] = ((val3.x > 0) ? num8 : 0);
			m_MediumDemandFactors[13] = ((val3.y > 0) ? num9 : 0);
			m_HighDemandFactors[13] = ((val3.z > 0) ? num10 : 0);
			if (m_TotalWorkplaces.SimpleWorkplacesCount + m_TotalWorkplaces.ComplexWorkplacesCount <= 0)
			{
				m_LowDemandFactors[6] = ((m_LowDemandFactors[6] <= 0) ? m_LowDemandFactors[6] : 0);
				m_MediumDemandFactors[6] = ((m_MediumDemandFactors[6] <= 0) ? m_MediumDemandFactors[6] : 0);
				m_HighDemandFactors[6] = ((m_HighDemandFactors[6] <= 0) ? m_HighDemandFactors[6] : 0);
			}
			if (population.m_Population == 0)
			{
				m_LowDemandFactors[5] = 0;
				m_MediumDemandFactors[5] = 0;
				m_HighDemandFactors[5] = 0;
			}
			m_BuildingDemand.value = new int3(math.clamp(m_HouseholdDemand.value / 2 + num8 + num11, 0, 100), math.clamp(m_HouseholdDemand.value / 2 + num9 + num12, 0, 100), math.clamp(m_HouseholdDemand.value / 2 + num10 + num13, 0, 100));
			m_BuildingDemand.value = math.select(default(int3), m_BuildingDemand.value, val);
			m_TriggerQueue.Enqueue(new TriggerAction(TriggerType.ResidentialDemand, Entity.Null, (val3.x + val3.y + val3.z > 100) ? ((float)(m_BuildingDemand.value.x + m_BuildingDemand.value.y + m_BuildingDemand.value.z) / 100f) : 0f));
		}

		private int GetFactorValue(float factorValue, float2 weightSelector)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (!(factorValue < 0f))
			{
				return (int)(factorValue * weightSelector.y);
			}
			return (int)(factorValue * weightSelector.x);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> __Game_Prefabs_ZonePropertiesData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZonePropertiesData>(true);
		}
	}

	public static readonly int kMaxFactorEffect = 15;

	private TaxSystem m_TaxSystem;

	private CountStudyPositionsSystem m_CountStudyPositionsSystem;

	private CountWorkplacesSystem m_CountWorkplacesSystem;

	private CountHouseholdDataSystem m_CountHouseholdDataSystem;

	private CountResidentialPropertySystem m_CountResidentialPropertySystem;

	private CitySystem m_CitySystem;

	private TriggerSystem m_TriggerSystem;

	private EntityQuery m_DemandParameterGroup;

	private EntityQuery m_UnlockedZonePrefabQuery;

	private EntityQuery m_GameModeSettingQuery;

	[DebugWatchValue(color = "#27ae60")]
	private NativeValue<int> m_HouseholdDemand;

	[DebugWatchValue(color = "#117a65")]
	private NativeValue<int3> m_BuildingDemand;

	[EnumArray(typeof(DemandFactor))]
	[DebugWatchValue]
	private NativeArray<int> m_LowDemandFactors;

	[EnumArray(typeof(DemandFactor))]
	[DebugWatchValue]
	private NativeArray<int> m_MediumDemandFactors;

	[EnumArray(typeof(DemandFactor))]
	[DebugWatchValue]
	private NativeArray<int> m_HighDemandFactors;

	[DebugWatchDeps]
	private JobHandle m_WriteDependencies;

	private JobHandle m_ReadDependencies;

	private int m_LastHouseholdDemand;

	private int3 m_LastBuildingDemand;

	private float2 m_ResidentialDemandWeightsSelector;

	private TypeHandle __TypeHandle;

	public int householdDemand => m_LastHouseholdDemand;

	public int3 buildingDemand => m_LastBuildingDemand;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 10;
	}

	public NativeArray<int> GetLowDensityDemandFactors(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_LowDemandFactors;
	}

	public NativeArray<int> GetMediumDensityDemandFactors(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_MediumDemandFactors;
	}

	public NativeArray<int> GetHighDensityDemandFactors(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_HighDemandFactors;
	}

	public void AddReader(JobHandle reader)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, reader);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			m_ResidentialDemandWeightsSelector = new float2(1f, 1f);
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		if (singleton.m_Enable)
		{
			m_ResidentialDemandWeightsSelector = singleton.m_ResidentialDemandWeightsSelector;
		}
		else
		{
			m_ResidentialDemandWeightsSelector = new float2(1f, 1f);
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_DemandParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		m_UnlockedZonePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ZoneData>(),
			ComponentType.ReadOnly<ZonePropertiesData>(),
			ComponentType.Exclude<Locked>()
		});
		m_GameModeSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_CountStudyPositionsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountStudyPositionsSystem>();
		m_CountWorkplacesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountWorkplacesSystem>();
		m_CountHouseholdDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountHouseholdDataSystem>();
		m_CountResidentialPropertySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountResidentialPropertySystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_HouseholdDemand = new NativeValue<int>((Allocator)4);
		m_BuildingDemand = new NativeValue<int3>((Allocator)4);
		m_LowDemandFactors = new NativeArray<int>(18, (Allocator)4, (NativeArrayOptions)1);
		m_MediumDemandFactors = new NativeArray<int>(18, (Allocator)4, (NativeArrayOptions)1);
		m_HighDemandFactors = new NativeArray<int>(18, (Allocator)4, (NativeArrayOptions)1);
		m_ResidentialDemandWeightsSelector = new float2(1f, 1f);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_HouseholdDemand.Dispose();
		m_BuildingDemand.Dispose();
		m_LowDemandFactors.Dispose();
		m_MediumDemandFactors.Dispose();
		m_HighDemandFactors.Dispose();
		base.OnDestroy();
	}

	public void SetDefaults(Context context)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		m_HouseholdDemand.value = 0;
		m_BuildingDemand.value = default(int3);
		CollectionUtils.Fill<int>(m_LowDemandFactors, 0);
		CollectionUtils.Fill<int>(m_MediumDemandFactors, 0);
		CollectionUtils.Fill<int>(m_HighDemandFactors, 0);
		m_LastHouseholdDemand = 0;
		m_LastBuildingDemand = default(int3);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		int value = m_HouseholdDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value);
		int3 value2 = m_BuildingDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value2);
		int length = m_LowDemandFactors.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		NativeArray<int> val = m_LowDemandFactors;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		NativeArray<int> val2 = m_MediumDemandFactors;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
		NativeArray<int> val3 = m_HighDemandFactors;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val3);
		int num = m_LastHouseholdDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int3 val4 = m_LastBuildingDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		int value = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value);
		m_HouseholdDemand.value = value;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.residentialDemandSplit)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_BuildingDemand.value = new int3(num / 3, num / 3, num / 3);
		}
		else
		{
			int3 value2 = default(int3);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref value2);
			m_BuildingDemand.value = value2;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.demandFactorCountSerialization)
		{
			NativeArray<int> val = default(NativeArray<int>);
			val._002Ector(13, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val2 = val;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
			CollectionUtils.CopySafe<int>(val, m_LowDemandFactors);
			val.Dispose();
		}
		else
		{
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			if (num2 == m_LowDemandFactors.Length)
			{
				NativeArray<int> val3 = m_LowDemandFactors;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val3);
				NativeArray<int> val4 = m_MediumDemandFactors;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val4);
				NativeArray<int> val5 = m_HighDemandFactors;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val5);
			}
			else
			{
				NativeArray<int> val6 = default(NativeArray<int>);
				val6._002Ector(num2, (Allocator)2, (NativeArrayOptions)1);
				NativeArray<int> val7 = val6;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val7);
				CollectionUtils.CopySafe<int>(val6, m_LowDemandFactors);
				NativeArray<int> val8 = val6;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val8);
				CollectionUtils.CopySafe<int>(val6, m_MediumDemandFactors);
				NativeArray<int> val9 = val6;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val9);
				CollectionUtils.CopySafe<int>(val6, m_HighDemandFactors);
				val6.Dispose();
			}
		}
		ref int reference = ref m_LastHouseholdDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.residentialDemandSplit)
		{
			int num3 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			m_LastBuildingDemand = new int3(num3 / 3, num3 / 3, num3 / 3);
		}
		else
		{
			ref int3 reference2 = ref m_LastBuildingDemand;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_DemandParameterGroup)).IsEmptyIgnoreFilter)
		{
			m_LastHouseholdDemand = m_HouseholdDemand.value;
			m_LastBuildingDemand = m_BuildingDemand.value;
			JobHandle val = default(JobHandle);
			JobHandle val2 = default(JobHandle);
			JobHandle deps;
			UpdateResidentialDemandJob updateResidentialDemandJob = new UpdateResidentialDemandJob
			{
				m_UnlockedZonePrefabs = ((EntityQuery)(ref m_UnlockedZonePrefabQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ZoneDatas = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ZonePropertiesDatas = InternalCompilerInterface.GetComponentLookup<ZonePropertiesData>(ref __TypeHandle.__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DemandParameters = ((EntityQuery)(ref m_DemandParameterGroup)).ToComponentDataListAsync<DemandParameterData>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
				m_StudyPositions = m_CountStudyPositionsSystem.GetStudyPositionsByEducation(out deps),
				m_FreeWorkplaces = m_CountWorkplacesSystem.GetFreeWorkplaces(),
				m_TotalWorkplaces = m_CountWorkplacesSystem.GetTotalWorkplaces(),
				m_HouseholdCountData = m_CountHouseholdDataSystem.GetHouseholdCountData(),
				m_ResidentialPropertyData = m_CountResidentialPropertySystem.GetResidentialPropertyData(),
				m_TaxRates = m_TaxSystem.GetTaxRates(),
				m_City = m_CitySystem.City,
				m_HouseholdDemand = m_HouseholdDemand,
				m_BuildingDemand = m_BuildingDemand,
				m_LowDemandFactors = m_LowDemandFactors,
				m_MediumDemandFactors = m_MediumDemandFactors,
				m_HighDemandFactors = m_HighDemandFactors,
				m_UnemploymentRate = m_CountHouseholdDataSystem.UnemploymentRate,
				m_ResidentialDemandWeightsSelector = m_ResidentialDemandWeightsSelector,
				m_TriggerQueue = m_TriggerSystem.CreateActionBuffer()
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<UpdateResidentialDemandJob>(updateResidentialDemandJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, m_ReadDependencies, val2, deps, val));
			m_WriteDependencies = ((SystemBase)this).Dependency;
			m_CountStudyPositionsSystem.AddReader(((SystemBase)this).Dependency);
			m_TaxSystem.AddReader(((SystemBase)this).Dependency);
			m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ResidentialDemandSystem()
	{
	}
}
