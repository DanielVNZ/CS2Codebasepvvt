using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Entities;
using Colossal.Logging;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Net;
using Game.Objects;
using Game.Policies;
using Game.Prefabs;
using Game.Prefabs.Climate;
using Game.Routes;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Achievements;

[CompilerGenerated]
public class AchievementTriggerSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public class ProgressBuffer
	{
		private AchievementId m_Achievement;

		private int m_IncrementStep;

		private IndicateType m_Type;

		public int m_Progress;

		public ProgressBuffer(AchievementId achievement, int incrementStep, IndicateType type)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			m_Achievement = achievement;
			m_IncrementStep = incrementStep;
			m_Type = type;
			m_Progress = 0;
		}

		public void AddProgress(int progress)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Invalid comparison between Unknown and I4
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			m_Progress += progress;
			if ((int)m_Type == 0)
			{
				int num = m_Progress / m_IncrementStep;
				if (num > 0)
				{
					m_Progress %= m_IncrementStep;
					PlatformManager.instance.IndicateAchievementProgress(m_Achievement, num * m_IncrementStep, m_Type);
				}
			}
			else if ((int)m_Type == 1)
			{
				int num2 = m_Progress / m_IncrementStep;
				PlatformManager.instance.IndicateAchievementProgress(m_Achievement, num2 * m_IncrementStep, m_Type);
			}
		}
	}

	public class UserDataProgressBuffer : ProgressBuffer, IDisposable
	{
		private string m_ID;

		private static byte[] sBuffer = new byte[4];

		public UserDataProgressBuffer(AchievementId achievement, int incrementStep, IndicateType type, string id)
			: base(achievement, incrementStep, type)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			m_ID = id;
			Sync();
		}

		private void Sync()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (PlatformManager.instance.UserDataLoad(m_ID, (byte[])null) && PlatformManager.instance.UserDataLoad(m_ID, sBuffer))
				{
					m_Progress = BinaryPrimitives.ReadInt32LittleEndian(ReadOnlySpan<byte>.op_Implicit(sBuffer));
				}
			}
			catch (Exception ex)
			{
				sLog.Error(ex);
			}
		}

		private void Store()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				BinaryPrimitives.WriteInt32LittleEndian(Span<byte>.op_Implicit(sBuffer), m_Progress);
				PlatformManager.instance.UserDataStore(m_ID, sBuffer);
			}
			catch (Exception ex)
			{
				sLog.Error(ex);
			}
		}

		public void Dispose()
		{
			Store();
		}
	}

	[BurstCompile]
	private struct ProcessDependencyDataJob : IJob
	{
		[ReadOnly]
		public NativeArray<long> m_ProducedResources;

		public NativeArray<long> m_ResourceProducedArray;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			m_ResourceProducedArray.CopyFrom(m_ProducedResources);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<CityStatistic> __Game_City_CityStatistic_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OfficeBuilding> __Game_Prefabs_OfficeBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Followed> __Game_Citizens_Followed_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<AchievementFilterData> __Game_Prefabs_AchievementFilterData_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<SignatureBuildingData> __Game_Prefabs_SignatureBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Policy> __Game_Policies_Policy_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			__Game_City_CityStatistic_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityStatistic>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_OfficeBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OfficeBuilding>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_Followed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Followed>(true);
			__Game_Prefabs_AchievementFilterData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AchievementFilterData>(true);
			__Game_Prefabs_SignatureBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SignatureBuildingData>(true);
			__Game_Policies_Policy_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Policy>(true);
		}
	}

	private static ILog sLog = LogManager.GetLogger("Platforms");

	private ToolSystem m_ToolSystem;

	private CitySystem m_CitySystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private SimulationSystem m_SimulationSystem;

	private ClimateSystem m_ClimateSystem;

	private TimeSystem m_TimeSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_CreatedObjectQuery;

	private EntityQuery m_ObjectAchievementQuery;

	private EntityQuery m_UnlockQuery;

	private EntityQuery m_ParkQuery;

	private EntityQuery m_CreatedParkQuery;

	private EntityQuery m_LockedServiceQuery;

	private EntityQuery m_ServiceQuery;

	private EntityQuery m_LockedBuildingQuery;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_TransportLineQuery;

	private EntityQuery m_CreatedTransportLineQuery;

	private EntityQuery m_UniqueServiceBuildingPrefabQuery;

	private EntityQuery m_UniqueServiceBuildingQuery;

	private EntityQuery m_CreatedUniqueServiceBuildingQuery;

	private EntityQuery m_PolicyModificationQuery;

	private EntityQuery m_DistrictQuery;

	private EntityQuery m_ServiceDistrictBuildingQuery;

	private EntityQuery m_FossilEnergyProducersQuery;

	private EntityQuery m_RenewableEnergyProducersQuery;

	private EntityQuery m_EnergyProducersQuery;

	private EntityQuery m_WaterPumpingStationQuery;

	private EntityQuery m_ResidentialBuildingsQuery;

	private EntityQuery m_CommercialBuildingsQuery;

	private EntityQuery m_IndustrialBuildingsQuery;

	private EntityQuery m_FollowedCitizensQuery;

	private EntityQuery m_InfoviewQuery;

	private EntityQuery m_CreatedUniqueBuildingQuery;

	private EntityQuery m_UniqueBuildingQuery;

	private EntityQuery m_PlantQuery;

	private EntityQuery m_CreatedPlantQuery;

	private EntityQuery m_TimeDataQuery;

	private EntityQuery m_TimeSettingsQuery;

	private EntityQuery m_ProduceResourceCompaniesQuery;

	private EntityQuery m_CreatedAggregateElementQuery;

	private EntityQuery m_AggregateElementQuery;

	public NativeCounter m_PatientsTreatedCounter;

	public NativeCounter m_ProducedFishCounter;

	public NativeCounter m_OffshoreOilProduceCounter;

	private JobHandle m_TransportWriteDeps;

	private NativeQueue<TransportedResource> m_TransportedResourceQueue;

	private int m_CachedPatientsTreatedCount;

	private int m_CachedPopulationCount;

	private int m_CachedHappiness;

	private int m_CachedAttractiveness;

	private int m_CachedTouristCount;

	private bool m_CheckUnlocks;

	private uint m_LastCheckFrameIndex;

	private static readonly int kMinCityEffectPopulation = 1000;

	private static readonly int kAllSmilesHappiness = 75;

	private static readonly int kThisIsNotMyHappyPlaceHappiness = 25;

	private static readonly int kSimplyIrresistibleAttractiveness = 90;

	private static readonly int kZeroEmissionMinProduction = 5000000;

	private static readonly int kColossalGardenerLimit = 100;

	private static readonly int kTheDeepEndLoanAmount = 200000;

	private HashSet<InfoviewPrefab> m_ViewedInfoviews = new HashSet<InfoviewPrefab>();

	private Dictionary<AchievementId, int> m_IncrementalObjectAchievementProgress = new Dictionary<AchievementId, int>();

	private List<AchievementId> m_AbsoluteObjectAchievements = new List<AchievementId>();

	public ProgressBuffer m_LittleBitOfTLCBuffer;

	public ProgressBuffer m_HowMuchIsTheFishBuffer;

	public ProgressBuffer m_ADifferentPlatformerBuffer;

	public UserDataProgressBuffer m_SquasherDownerBuffer;

	public UserDataProgressBuffer m_ShipItBuffer;

	private TypeHandle __TypeHandle;

	public NativeQueue<TransportedResource> GetTransportedResourceQueue()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_TransportedResourceQueue;
	}

	public void AddWriter(JobHandle writer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_TransportWriteDeps = JobHandle.CombineDependencies(m_TransportWriteDeps, writer);
	}

	public bool GetDebugData(AchievementId achievement, out string data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (achievement == Achievements.ALittleBitofTLC)
		{
			data = $"{m_LittleBitOfTLCBuffer?.m_Progress ?? 0}";
			return true;
		}
		if (achievement == Achievements.HowMuchIsTheFish)
		{
			data = $"{m_HowMuchIsTheFishBuffer?.m_Progress ?? 0}";
			return true;
		}
		if (achievement == Achievements.ADifferentPlatformer)
		{
			data = $"{m_ADifferentPlatformerBuffer?.m_Progress ?? 0}";
			return true;
		}
		if (achievement == Achievements.OneofEverything)
		{
			data = $"{CountUniqueServiceBuildings()}/{CountUniqueServiceBuildingPrefabs()}";
			return true;
		}
		data = string.Empty;
		return false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0868: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_0909: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0922: Unknown result type (might be due to invalid IL or missing references)
		//IL_0929: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_093a: Unknown result type (might be due to invalid IL or missing references)
		//IL_093f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Unknown result type (might be due to invalid IL or missing references)
		//IL_0953: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_095d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0962: Unknown result type (might be due to invalid IL or missing references)
		//IL_0971: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0980: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LittleBitOfTLCBuffer = new ProgressBuffer(Achievements.ALittleBitofTLC, 1000, (IndicateType)1);
		m_SquasherDownerBuffer = new UserDataProgressBuffer(Achievements.SquasherDowner, 10, (IndicateType)0, "SquasherDowner");
		m_PatientsTreatedCounter = new NativeCounter((Allocator)4);
		m_ProducedFishCounter = new NativeCounter((Allocator)4);
		m_OffshoreOilProduceCounter = new NativeCounter((Allocator)4);
		m_TransportedResourceQueue = new NativeQueue<TransportedResource>(AllocatorHandle.op_Implicit((Allocator)4));
		m_HowMuchIsTheFishBuffer = new ProgressBuffer(Achievements.HowMuchIsTheFish, 100000, (IndicateType)1);
		m_ADifferentPlatformerBuffer = new ProgressBuffer(Achievements.ADifferentPlatformer, 10000, (IndicateType)1);
		m_ShipItBuffer = new UserDataProgressBuffer(Achievements.ShipIt, 1000000, (IndicateType)0, "ShipIt");
		m_CachedPatientsTreatedCount = 0;
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CreatedObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<ObjectAchievement>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ObjectAchievementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<ObjectAchievement>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_UnlockQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		m_ParkQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Buildings.Park>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Extension>(),
			ComponentType.Exclude<Game.Buildings.ServiceUpgrade>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CreatedParkQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Game.Buildings.Park>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Extension>(),
			ComponentType.Exclude<Game.Buildings.ServiceUpgrade>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_LockedServiceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ServiceData>(),
			ComponentType.ReadOnly<Locked>()
		});
		m_ServiceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceData>() });
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingData>() });
		m_LockedBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.ReadOnly<Locked>()
		});
		m_TransportLineQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<TransportLine>(),
			ComponentType.ReadOnly<Route>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CreatedTransportLineQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<TransportLine>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_UniqueServiceBuildingPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<UniqueObjectData>(),
			ComponentType.ReadOnly<CollectedServiceBuildingBudgetData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_UniqueServiceBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Objects.UniqueObject>(),
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_CreatedUniqueServiceBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Objects.UniqueObject>(),
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_PolicyModificationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Modify>() });
		m_DistrictQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<District>(),
			ComponentType.ReadOnly<Policy>()
		});
		m_ServiceDistrictBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<ServiceDistrict>(),
			ComponentType.Exclude<Deleted>()
		});
		m_FossilEnergyProducersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<ElectricityProducer>(),
			ComponentType.Exclude<RenewableElectricityProduction>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_RenewableEnergyProducersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<ElectricityProducer>(),
			ComponentType.ReadOnly<RenewableElectricityProduction>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_EnergyProducersQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ElectricityProducer>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_WaterPumpingStationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.WaterPumpingStation>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ResidentialBuildingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<ResidentialProperty>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_CommercialBuildingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<CommercialProperty>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_IndustrialBuildingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<IndustrialProperty>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_FollowedCitizensQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Followed>(),
			ComponentType.ReadOnly<Citizen>()
		});
		m_InfoviewQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfoviewData>() });
		m_UniqueBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Objects.UniqueObject>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CreatedUniqueBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Objects.UniqueObject>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PlantQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Plant>(),
			ComponentType.Exclude<Owner>(),
			ComponentType.Exclude<Native>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CreatedPlantQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Plant>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Owner>(),
			ComponentType.Exclude<Native>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ProduceResourceCompaniesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CreatedAggregateElementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<AggregateElement>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_AggregateElementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<AggregateElement>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		m_TimeSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventInfoviewChanged = (Action<InfoviewPrefab>)Delegate.Combine(toolSystem.EventInfoviewChanged, new Action<InfoviewPrefab>(OnInfoviewChanged));
	}

	private void Reset()
	{
		m_CachedHappiness = 50;
		m_CachedAttractiveness = 0;
		m_CheckUnlocks = true;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		Reset();
		PlatformManager instance = PlatformManager.instance;
		instance.achievementsEnabled &= m_CityConfigurationSystem.usedMods.Count == 0 && !m_CityConfigurationSystem.unlimitedMoney && !m_CityConfigurationSystem.unlockAll && !m_CityConfigurationSystem.unlockMapTiles;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (!m_ToolSystem.actionMode.IsEditor() && PlatformManager.instance.achievementsEnabled && GameManager.instance.state != GameManager.State.Loading && GameManager.instance.gameMode.IsGameOrEditor())
		{
			CheckInGameAchievements();
		}
	}

	private int CountAbsoluteObjectAchievementProgress(AchievementId achID)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		NativeArray<Entity> val = ((EntityQuery)(ref m_ObjectAchievementQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<ObjectAchievementData> val2 = default(DynamicBuffer<ObjectAchievementData>);
		Curve curve = default(Curve);
		for (int i = 0; i < val.Length; i++)
		{
			if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val[i], ref prefabRef) || !EntitiesExtensions.TryGetBuffer<ObjectAchievementData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val2))
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < val2.Length; j++)
			{
				if (val2[j].m_ID == achID)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			if (achID == Achievements.Pierfect || achID == Achievements.ItsPronouncedKey)
			{
				if (EntitiesExtensions.TryGetComponent<Curve>(((ComponentSystemBase)this).EntityManager, val[i], ref curve))
				{
					num += curve.m_Length;
				}
			}
			else
			{
				num += 1f;
			}
		}
		val.Dispose();
		return (int)num;
	}

	private void CheckInGameAchievements()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_094c: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_CreatedObjectQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val = ((EntityQuery)(ref m_CreatedObjectQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			PrefabRef prefabRef = default(PrefabRef);
			DynamicBuffer<ObjectAchievementData> val2 = default(DynamicBuffer<ObjectAchievementData>);
			for (int i = 0; i < val.Length; i++)
			{
				if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val[i], ref prefabRef) || !EntitiesExtensions.TryGetBuffer<ObjectAchievementData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val2))
				{
					continue;
				}
				for (int j = 0; j < val2.Length; j++)
				{
					if (val2[j].m_BypassCounter)
					{
						PlatformManager.instance.UnlockAchievement(val2[j].m_ID);
					}
					else if (val2[j].m_AbsoluteCounter)
					{
						if (!m_AbsoluteObjectAchievements.Contains(val2[j].m_ID))
						{
							m_AbsoluteObjectAchievements.Add(val2[j].m_ID);
						}
					}
					else
					{
						int valueOrDefault = CollectionExtensions.GetValueOrDefault<AchievementId, int>((IReadOnlyDictionary<AchievementId, int>)m_IncrementalObjectAchievementProgress, val2[j].m_ID, 0);
						valueOrDefault++;
						m_IncrementalObjectAchievementProgress[val2[j].m_ID] = valueOrDefault;
					}
				}
			}
			IAchievement val3 = default(IAchievement);
			foreach (KeyValuePair<AchievementId, int> item in m_IncrementalObjectAchievementProgress)
			{
				if (PlatformManager.instance.GetAchievement(item.Key, ref val3))
				{
					int num = val3.maxProgress - val3.progress;
					int num2 = Mathf.Min(item.Value, num);
					PlatformManager.instance.IndicateAchievementProgress(item.Key, num2, (IndicateType)0);
				}
			}
			IAchievement val4 = default(IAchievement);
			foreach (AchievementId item2 in m_AbsoluteObjectAchievements)
			{
				if (PlatformManager.instance.GetAchievement(item2, ref val4) && !val4.achieved)
				{
					int num3 = CountAbsoluteObjectAchievementProgress(item2);
					num3 = Mathf.Min(num3, val4.maxProgress);
					PlatformManager.instance.IndicateAchievementProgress(item2, num3, (IndicateType)1);
				}
			}
			m_IncrementalObjectAchievementProgress.Clear();
			m_AbsoluteObjectAchievements.Clear();
			val.Dispose();
		}
		int num4;
		if (!((EntityQuery)(ref m_CreatedParkQuery)).IsEmptyIgnoreFilter)
		{
			num4 = CountParks();
			PlatformManager.instance.IndicateAchievementProgress(Achievements.Groundskeeper, num4, (IndicateType)1);
		}
		if (m_CheckUnlocks || !((EntityQuery)(ref m_UnlockQuery)).IsEmptyIgnoreFilter)
		{
			m_CheckUnlocks = false;
			CheckUnlockingAchievements();
		}
		Loan loan = default(Loan);
		if (EntitiesExtensions.TryGetComponent<Loan>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, ref loan) && loan.m_LastModified >= m_LastCheckFrameIndex)
		{
			PlatformManager.instance.IndicateAchievementProgress(Achievements.TheDeepEnd, Mathf.Min(loan.m_Amount, kTheDeepEndLoanAmount), (IndicateType)1);
		}
		Population population = default(Population);
		if (EntitiesExtensions.TryGetComponent<Population>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, ref population))
		{
			int num5 = ((population.m_Population >= 10000) ? 10000 : 1000);
			int num6 = population.m_Population / num5 * num5;
			if (num6 != m_CachedPopulationCount)
			{
				m_CachedPopulationCount = num6;
				PlatformManager.instance.IndicateAchievementProgress(Achievements.SixFigures, num6, (IndicateType)1);
			}
		}
		if (!((EntityQuery)(ref m_CreatedTransportLineQuery)).IsEmptyIgnoreFilter || !((EntityQuery)(ref m_PolicyModificationQuery)).IsEmptyIgnoreFilter)
		{
			num4 = 0;
			NativeArray<Route> val5 = ((EntityQuery)(ref m_TransportLineQuery)).ToComponentDataArray<Route>(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				for (int k = 0; k < val5.Length; k++)
				{
					if (!RouteUtils.CheckOption(val5[k], RouteOption.Inactive))
					{
						num4++;
					}
				}
			}
			finally
			{
				val5.Dispose();
			}
			PlatformManager.instance.IndicateAchievementProgress(Achievements.GoAnywhere, num4, (IndicateType)1);
			PlatformManager.instance.IndicateAchievementProgress(Achievements.Spiderwebbing, num4, (IndicateType)1);
		}
		if (population.m_Population >= kMinCityEffectPopulation)
		{
			if (m_CachedHappiness < kAllSmilesHappiness && population.m_AverageHappiness >= kAllSmilesHappiness)
			{
				PlatformManager.instance.UnlockAchievement(Achievements.AllSmiles);
			}
			if (m_CachedHappiness > kThisIsNotMyHappyPlaceHappiness && population.m_AverageHappiness <= kThisIsNotMyHappyPlaceHappiness)
			{
				PlatformManager.instance.UnlockAchievement(Achievements.ThisIsNotMyHappyPlace);
			}
			m_CachedHappiness = population.m_AverageHappiness;
			Tourism tourism = default(Tourism);
			if (EntitiesExtensions.TryGetComponent<Tourism>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, ref tourism))
			{
				if (m_CachedAttractiveness < kSimplyIrresistibleAttractiveness && tourism.m_Attractiveness >= kSimplyIrresistibleAttractiveness)
				{
					PlatformManager.instance.UnlockAchievement(Achievements.SimplyIrresistible);
				}
				m_CachedAttractiveness = tourism.m_Attractiveness;
			}
		}
		if (!((EntityQuery)(ref m_CreatedUniqueServiceBuildingQuery)).IsEmptyIgnoreFilter && CheckOneOfEverything())
		{
			PlatformManager.instance.UnlockAchievement(Achievements.OneofEverything);
		}
		BufferLookup<CityStatistic> bufferLookup = InternalCompilerInterface.GetBufferLookup<CityStatistic>(ref __TypeHandle.__Game_City_CityStatistic_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		num4 = m_CityStatisticsSystem.GetStatisticValue(bufferLookup, StatisticType.TouristCount) / 1000 * 1000;
		if (m_CachedTouristCount != num4)
		{
			PlatformManager.instance.IndicateAchievementProgress(Achievements.WelcomeOneandAll, num4, (IndicateType)1);
		}
		m_CachedTouristCount = num4;
		int statisticValue = m_CityStatisticsSystem.GetStatisticValue(bufferLookup, StatisticType.EducationCount);
		int statisticValue2 = m_CityStatisticsSystem.GetStatisticValue(bufferLookup, StatisticType.EducationCount, 1);
		int statisticValue3 = m_CityStatisticsSystem.GetStatisticValue(bufferLookup, StatisticType.EducationCount, 2);
		int statisticValue4 = m_CityStatisticsSystem.GetStatisticValue(bufferLookup, StatisticType.EducationCount, 3);
		int statisticValue5 = m_CityStatisticsSystem.GetStatisticValue(bufferLookup, StatisticType.EducationCount, 4);
		int num7 = statisticValue + statisticValue2 + statisticValue3 + statisticValue4 + statisticValue5;
		if (num7 > 0 && (float)statisticValue5 / (float)num7 >= 0.15f)
		{
			PlatformManager.instance.UnlockAchievement(Achievements.TopoftheClass);
		}
		if (!((EntityQuery)(ref m_ServiceDistrictBuildingQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val6 = ((EntityQuery)(ref m_ServiceDistrictBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				DynamicBuffer<ServiceDistrict> val7 = default(DynamicBuffer<ServiceDistrict>);
				for (int l = 0; l < val6.Length; l++)
				{
					if (EntitiesExtensions.TryGetBuffer<ServiceDistrict>(((ComponentSystemBase)this).EntityManager, val6[l], true, ref val7) && val7.Length > 0)
					{
						PlatformManager.instance.UnlockAchievement(Achievements.HappytobeofService);
					}
				}
			}
			finally
			{
				val6.Dispose();
			}
		}
		int num8 = CalculateEnergyProduction(m_RenewableEnergyProducersQuery);
		int num9 = CalculateEnergyProduction(m_FossilEnergyProducersQuery);
		if (num8 >= kZeroEmissionMinProduction && num9 <= 0)
		{
			PlatformManager.instance.UnlockAchievement(Achievements.ZeroEmission);
		}
		int num10 = ((EntityQuery)(ref m_ResidentialBuildingsQuery)).CalculateEntityCount();
		int num11 = ((EntityQuery)(ref m_CommercialBuildingsQuery)).CalculateEntityCount();
		bool flag = false;
		bool flag2 = false;
		NativeArray<Entity> val8 = ((EntityQuery)(ref m_IndustrialBuildingsQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentLookup<PrefabRef> componentLookup = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<OfficeBuilding> componentLookup2 = InternalCompilerInterface.GetComponentLookup<OfficeBuilding>(ref __TypeHandle.__Game_Prefabs_OfficeBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		try
		{
			for (int m = 0; m < val8.Length; m++)
			{
				if (componentLookup2.HasComponent(componentLookup[val8[m]].m_Prefab))
				{
					flag2 = true;
				}
				else
				{
					flag = true;
				}
				if (flag && flag2)
				{
					break;
				}
			}
		}
		finally
		{
			val8.Dispose();
		}
		if (flag && flag2 && num10 > 0 && num11 > 0)
		{
			PlatformManager.instance.UnlockAchievement(Achievements.StrengthThroughDiversity);
		}
		if (!((EntityQuery)(ref m_FollowedCitizensQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val9 = ((EntityQuery)(ref m_FollowedCitizensQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			ComponentLookup<Citizen> componentLookup3 = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<Followed> componentLookup4 = InternalCompilerInterface.GetComponentLookup<Followed>(ref __TypeHandle.__Game_Citizens_Followed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			try
			{
				for (int n = 0; n < val9.Length; n++)
				{
					if (componentLookup3.HasComponent(val9[n]) && componentLookup3[val9[n]].GetAge() == CitizenAge.Elderly && componentLookup4.HasComponent(val9[n]) && componentLookup4[val9[n]].m_StartedFollowingAsChild)
					{
						PlatformManager.instance.UnlockAchievement(Achievements.YouLittleStalker);
					}
				}
			}
			finally
			{
				val9.Dispose();
			}
		}
		if (!((EntityQuery)(ref m_PolicyModificationQuery)).IsEmptyIgnoreFilter)
		{
			CheckPolicyAchievements();
		}
		if (((NativeCounter)(ref m_PatientsTreatedCounter)).Count > m_CachedPatientsTreatedCount)
		{
			int progress = ((NativeCounter)(ref m_PatientsTreatedCounter)).Count - m_CachedPatientsTreatedCount;
			m_LittleBitOfTLCBuffer.AddProgress(progress);
			m_CachedPatientsTreatedCount = ((NativeCounter)(ref m_PatientsTreatedCounter)).Count;
		}
		if (ShouldCheckOffshoreOilProduce() && ((NativeCounter)(ref m_OffshoreOilProduceCounter)).Count > 0)
		{
			m_ADifferentPlatformerBuffer.AddProgress(((NativeCounter)(ref m_OffshoreOilProduceCounter)).Count);
			((NativeCounter)(ref m_OffshoreOilProduceCounter)).Count = 0;
		}
		if (ShouldCheckProducedFish() && ((NativeCounter)(ref m_ProducedFishCounter)).Count > 0)
		{
			m_HowMuchIsTheFishBuffer.AddProgress(((NativeCounter)(ref m_ProducedFishCounter)).Count);
			((NativeCounter)(ref m_ProducedFishCounter)).Count = 0;
		}
		if (!((EntityQuery)(ref m_CreatedUniqueBuildingQuery)).IsEmptyIgnoreFilter)
		{
			int num12 = CountSignatureBuildings();
			PlatformManager.instance.IndicateAchievementProgress(Achievements.MakingAMark, num12, (IndicateType)1);
			PlatformManager.instance.IndicateAchievementProgress(Achievements.TheArchitect, num12, (IndicateType)1);
		}
		if (!((EntityQuery)(ref m_ResidentialBuildingsQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_CommercialBuildingsQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_IndustrialBuildingsQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_EnergyProducersQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_WaterPumpingStationQuery)).IsEmptyIgnoreFilter)
		{
			PlatformManager.instance.UnlockAchievement(Achievements.MyFirstCity);
		}
		if (!((EntityQuery)(ref m_CreatedPlantQuery)).IsEmptyIgnoreFilter && ((EntityQuery)(ref m_PlantQuery)).CalculateEntityCount() >= kColossalGardenerLimit)
		{
			PlatformManager.instance.UnlockAchievement(Achievements.ColossalGardener);
		}
		if (CheckFourSeasons())
		{
			PlatformManager.instance.UnlockAchievement(Achievements.FourSeasons);
		}
		IAchievement val10 = default(IAchievement);
		if (!((EntityQuery)(ref m_CreatedAggregateElementQuery)).IsEmptyIgnoreFilter && PlatformManager.instance.GetAchievement(Achievements.DrawMeLikeOneOfYourLiftBridges, ref val10) && !val10.achieved)
		{
			int num13 = CountLiftBridge();
			PlatformManager.instance.IndicateAchievementProgress(Achievements.DrawMeLikeOneOfYourLiftBridges, num13, (IndicateType)1);
		}
		if (!m_TransportedResourceQueue.IsEmpty())
		{
			IAchievement val11 = default(IAchievement);
			if (PlatformManager.instance.GetAchievement(Achievements.ShipIt, ref val11) && !val11.achieved && ((JobHandle)(ref m_TransportWriteDeps)).IsCompleted)
			{
				CheckTransportedResources();
			}
			else
			{
				m_TransportedResourceQueue.Clear();
			}
		}
		m_LastCheckFrameIndex = m_SimulationSystem.frameIndex;
	}

	private void CheckTransportedResources()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		TransportedResource transportedResource = default(TransportedResource);
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<AchievementFilterData> datas = default(DynamicBuffer<AchievementFilterData>);
		while (m_TransportedResourceQueue.TryDequeue(ref transportedResource))
		{
			BufferLookup<AchievementFilterData> bufferLookup = InternalCompilerInterface.GetBufferLookup<AchievementFilterData>(ref __TypeHandle.__Game_Prefabs_AchievementFilterData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, transportedResource.m_CargoTransport, ref prefabRef) && bufferLookup.TryGetBuffer(prefabRef.m_Prefab, ref datas) && CheckFilter(datas, Achievements.ShipIt))
			{
				num += transportedResource.m_Amount;
			}
		}
		if (num > 0)
		{
			m_ShipItBuffer.AddProgress(num);
		}
	}

	private bool ShouldCheckOffshoreOilProduce()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_ProduceResourceCompaniesQuery)).IsEmptyIgnoreFilter)
		{
			return false;
		}
		IAchievement val = default(IAchievement);
		if (PlatformManager.instance.GetAchievement(Achievements.ADifferentPlatformer, ref val) && !val.achieved)
		{
			return true;
		}
		return false;
	}

	private bool ShouldCheckProducedFish()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_ProduceResourceCompaniesQuery)).IsEmptyIgnoreFilter)
		{
			return false;
		}
		IAchievement val = default(IAchievement);
		if (PlatformManager.instance.GetAchievement(Achievements.HowMuchIsTheFish, ref val) && !val.achieved)
		{
			return true;
		}
		return false;
	}

	private bool CheckFourSeasons()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		IAchievement val = default(IAchievement);
		if (!PlatformManager.instance.GetAchievement(Achievements.FourSeasons, ref val) || val.achieved)
		{
			return false;
		}
		Entity currentClimate = m_ClimateSystem.currentClimate;
		if (currentClimate == Entity.Null)
		{
			return false;
		}
		ClimatePrefab prefab = m_PrefabSystem.GetPrefab<ClimatePrefab>(currentClimate);
		if ((Object)(object)prefab == (Object)null)
		{
			return false;
		}
		ClimateSystem.SeasonInfo[] seasons = prefab.m_Seasons;
		if ((seasons != null && seasons.Length < 4) || prefab.temperatureRange.min > 0f)
		{
			return false;
		}
		TimeData singleton = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>();
		TimeSettingsData singleton2 = ((EntityQuery)(ref m_TimeSettingsQuery)).GetSingleton<TimeSettingsData>();
		float startingDate = m_TimeSystem.GetStartingDate(singleton2, singleton);
		float elapsedYears = m_TimeSystem.GetElapsedYears(singleton2, singleton);
		return prefab.CountElapsedSeasons(startingDate, elapsedYears) >= prefab.m_Seasons?.Length;
	}

	private int CalculateEnergyProduction(EntityQuery entityQuery)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		NativeArray<ElectricityProducer> val = ((EntityQuery)(ref entityQuery)).ToComponentDataArray<ElectricityProducer>(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			num += val[i].m_Capacity;
		}
		val.Dispose();
		return num;
	}

	private int CountLiftBridge()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_AggregateElementQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		int num = 0;
		DynamicBuffer<AggregateElement> val2 = default(DynamicBuffer<AggregateElement>);
		PrefabRef prefabRef = default(PrefabRef);
		NetGeometryData netGeometryData = default(NetGeometryData);
		for (int i = 0; i < val.Length; i++)
		{
			if (!EntitiesExtensions.TryGetBuffer<AggregateElement>(((ComponentSystemBase)this).EntityManager, val[i], true, ref val2) || val2.Length <= 0)
			{
				continue;
			}
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Owner>(val2[0].m_Edge))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Road>(val2[0].m_Edge) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val2[0].m_Edge, ref prefabRef) && EntitiesExtensions.TryGetComponent<NetGeometryData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref netGeometryData) && (netGeometryData.m_Flags & Game.Net.GeometryFlags.StraightEdges) == 0 && (netGeometryData.m_IntersectLayers & Layer.Waterway) != Layer.None)
				{
					num++;
				}
			}
		}
		val.Dispose();
		return num;
	}

	private int CountSignatureBuildings()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<PrefabRef> val = ((EntityQuery)(ref m_UniqueBuildingQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentLookup<SignatureBuildingData> componentLookup = InternalCompilerInterface.GetComponentLookup<SignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_SignatureBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		int num = 0;
		for (int i = 0; i < val.Length; i++)
		{
			if (componentLookup.HasComponent(val[i].m_Prefab))
			{
				num++;
			}
		}
		val.Dispose();
		return num;
	}

	private int CountParks()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		NativeArray<PrefabRef> val = ((EntityQuery)(ref m_ParkQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
		BufferLookup<AchievementFilterData> bufferLookup = InternalCompilerInterface.GetBufferLookup<AchievementFilterData>(ref __TypeHandle.__Game_Prefabs_AchievementFilterData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		DynamicBuffer<AchievementFilterData> datas = default(DynamicBuffer<AchievementFilterData>);
		for (int i = 0; i < val.Length; i++)
		{
			Entity prefab = val[i].m_Prefab;
			if (!bufferLookup.TryGetBuffer(prefab, ref datas) || CheckFilter(datas, Achievements.Groundskeeper, defaultResult: true))
			{
				num++;
			}
		}
		val.Dispose();
		return num;
	}

	private bool CheckOneOfEverything()
	{
		int num = CountUniqueServiceBuildingPrefabs();
		return CountUniqueServiceBuildings() == num;
	}

	private int CountUniqueServiceBuildingPrefabs()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		BufferLookup<AchievementFilterData> bufferLookup = InternalCompilerInterface.GetBufferLookup<AchievementFilterData>(ref __TypeHandle.__Game_Prefabs_AchievementFilterData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		int num = 0;
		NativeArray<Entity> val = ((EntityQuery)(ref m_UniqueServiceBuildingPrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		DynamicBuffer<AchievementFilterData> datas = default(DynamicBuffer<AchievementFilterData>);
		for (int i = 0; i < val.Length; i++)
		{
			if (!bufferLookup.TryGetBuffer(val[i], ref datas) || CheckFilter(datas, Achievements.OneofEverything, defaultResult: true))
			{
				num++;
			}
		}
		val.Dispose();
		return num;
	}

	private int CountUniqueServiceBuildings()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		BufferLookup<AchievementFilterData> bufferLookup = InternalCompilerInterface.GetBufferLookup<AchievementFilterData>(ref __TypeHandle.__Game_Prefabs_AchievementFilterData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		int num = 0;
		NativeArray<PrefabRef> val = ((EntityQuery)(ref m_UniqueServiceBuildingQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
		DynamicBuffer<AchievementFilterData> datas = default(DynamicBuffer<AchievementFilterData>);
		for (int i = 0; i < val.Length; i++)
		{
			Entity prefab = val[i].m_Prefab;
			if (!bufferLookup.TryGetBuffer(prefab, ref datas) || CheckFilter(datas, Achievements.OneofEverything, defaultResult: true))
			{
				num++;
			}
		}
		val.Dispose();
		return num;
	}

	private bool CheckFilter(DynamicBuffer<AchievementFilterData> datas, AchievementId achievementID, bool defaultResult = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < datas.Length; i++)
		{
			if (datas[i].m_AchievementID == achievementID)
			{
				return datas[i].m_Allow;
			}
		}
		return defaultResult;
	}

	private void CheckUnlockingAchievements()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ServiceQuery)).IsEmptyIgnoreFilter && ((EntityQuery)(ref m_LockedServiceQuery)).IsEmpty)
		{
			PlatformManager.instance.UnlockAchievement(Achievements.RoyalFlush);
		}
		if (!((EntityQuery)(ref m_BuildingQuery)).IsEmptyIgnoreFilter && ((EntityQuery)(ref m_LockedBuildingQuery)).IsEmpty)
		{
			PlatformManager.instance.UnlockAchievement(Achievements.KeyToTheCity);
		}
	}

	private void OnInfoviewChanged(InfoviewPrefab infoview)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_ViewedInfoviews.Add(infoview);
		if (m_ViewedInfoviews.Count == ((EntityQuery)(ref m_InfoviewQuery)).CalculateEntityCount())
		{
			PlatformManager.instance.UnlockAchievement(Achievements.TheInspector);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int num = m_LittleBitOfTLCBuffer.m_Progress;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int num2 = m_HowMuchIsTheFishBuffer.m_Progress;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		int num3 = m_ADifferentPlatformerBuffer.m_Progress;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.TLCAchievement)
		{
			ref int reference = ref m_LittleBitOfTLCBuffer.m_Progress;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		}
		context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.BPDLCAchievement))
		{
			ref int reference2 = ref m_HowMuchIsTheFishBuffer.m_Progress;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
			ref int reference3 = ref m_ADifferentPlatformerBuffer.m_Progress;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
		}
		else
		{
			m_HowMuchIsTheFishBuffer.m_Progress = 0;
			m_ADifferentPlatformerBuffer.m_Progress = 0;
		}
	}

	public void SetDefaults(Context context)
	{
		Reset();
		m_LittleBitOfTLCBuffer.m_Progress = 0;
		m_HowMuchIsTheFishBuffer.m_Progress = 0;
		m_ADifferentPlatformerBuffer.m_Progress = 0;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		((NativeCounter)(ref m_PatientsTreatedCounter)).Dispose();
		((NativeCounter)(ref m_ProducedFishCounter)).Dispose();
		((NativeCounter)(ref m_OffshoreOilProduceCounter)).Dispose();
		m_SquasherDownerBuffer.Dispose();
		m_ShipItBuffer.Dispose();
		m_TransportedResourceQueue.Dispose();
	}

	private void CheckPolicyAchievements()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_DistrictQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		int num = 0;
		NativeArray<Entity> val = ((EntityQuery)(ref m_DistrictQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		BufferLookup<Policy> bufferLookup = InternalCompilerInterface.GetBufferLookup<Policy>(ref __TypeHandle.__Game_Policies_Policy_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		for (int i = 0; i < val.Length; i++)
		{
			DynamicBuffer<Policy> val2 = bufferLookup[val[i]];
			for (int j = 0; j < val2.Length; j++)
			{
				if ((val2[j].m_Flags & PolicyFlags.Active) != 0)
				{
					num++;
					break;
				}
			}
		}
		val.Dispose();
		if (num > 0)
		{
			PlatformManager.instance.UnlockAchievement(Achievements.ExecutiveDecision);
		}
		PlatformManager.instance.IndicateAchievementProgress(Achievements.WideVariety, num, (IndicateType)1);
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
	public AchievementTriggerSystem()
	{
	}
}
