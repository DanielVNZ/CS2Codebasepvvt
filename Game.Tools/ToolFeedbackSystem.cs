using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Serialization;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ToolFeedbackSystem : GameSystemBase, IPostDeserialize
{
	private struct RecentKey : IEquatable<RecentKey>
	{
		private Entity m_Entity;

		private int m_Type;

		public RecentKey(Entity entity, CoverageService coverageService)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Type = (int)coverageService | 0x100;
		}

		public RecentKey(Entity entity, FeedbackType feedbackType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Type = (int)feedbackType | 0x200;
		}

		public RecentKey(Entity entity, LocalModifierType localModifierType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Type = (int)(localModifierType | (LocalModifierType)768);
		}

		public RecentKey(Entity entity, CityModifierType cityModifierType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Type = (int)(cityModifierType | (CityModifierType)1024);
		}

		public RecentKey(Entity entity, MaintenanceType maintenanceType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Type = (int)maintenanceType | 0x500;
		}

		public RecentKey(Entity entity, TransportType transportType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Type = (int)(transportType | (TransportType)1536);
		}

		public bool Equals(RecentKey other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (m_Entity == other.m_Entity)
			{
				return m_Type == other.m_Type;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Entity)/*cast due to .constrained prefix*/).GetHashCode() * 1792 + m_Type;
		}
	}

	private struct RecentValue
	{
		public uint m_UpdateFrame;

		public float m_FeedbackDelta;
	}

	private struct RecentUpdate
	{
		public RecentKey m_Key;

		public float m_Delta;
	}

	private enum FeedbackType : byte
	{
		GarbageVehicles,
		HospitalAmbulances,
		HospitalHelicopters,
		HospitalCapacity,
		DeathcareHearses,
		DeathcareCapacity,
		Electricity,
		Transformer,
		WaterCapacity,
		SewageCapacity,
		TransportDispatch,
		PublicTransport,
		CargoTransport,
		GroundPollution,
		AirPollution,
		NoisePollution,
		PostFacilityVehicles,
		PostFacilityCapacity,
		TelecomCoverage,
		ElementarySchoolCapacity,
		HighSchoolCapacity,
		CollegeCapacity,
		UniversityCapacity,
		ParkingSpaces,
		FireStationEngines,
		FireStationHelicopters,
		PoliceStationCars,
		PoliceStationHelicopters,
		PoliceStationCapacity,
		PrisonVehicles,
		PrisonCapacity,
		Attractiveness
	}

	[BurstCompile]
	private struct SetupCoverageSearchJob : IJob
	{
		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public ComponentLookup<BackSide> m_BackSideData;

		[ReadOnly]
		public ComponentLookup<CoverageData> m_PrefabCoverageData;

		public CoverageAction m_Action;

		public PathfindTargetSeeker<PathfindTargetBuffer> m_TargetSeeker;

		public void Execute()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_TargetSeeker.m_PrefabRef[m_Entity];
			CoverageData coverageData = default(CoverageData);
			if (!m_PrefabCoverageData.TryGetComponent(prefabRef.m_Prefab, ref coverageData))
			{
				coverageData.m_Range = 25000f;
			}
			Building building = default(Building);
			if (m_TargetSeeker.m_Building.TryGetComponent(m_Entity, ref building))
			{
				Transform transform = m_TargetSeeker.m_Transform[m_Entity];
				if (building.m_RoadEdge != Entity.Null)
				{
					BuildingData buildingData = m_TargetSeeker.m_BuildingData[prefabRef.m_Prefab];
					float3 comparePosition = transform.m_Position;
					Owner owner = default(Owner);
					if (!m_TargetSeeker.m_Owner.TryGetComponent(building.m_RoadEdge, ref owner) || owner.m_Owner != m_Entity)
					{
						comparePosition = BuildingUtils.CalculateFrontPosition(transform, buildingData.m_LotSize.y);
					}
					Random random = m_TargetSeeker.m_RandomSeed.GetRandom(m_Entity.Index);
					m_TargetSeeker.AddEdgeTargets(ref random, m_Entity, 0f, EdgeFlags.DefaultMask, building.m_RoadEdge, comparePosition, 0f, allowLaneGroupSwitch: true, allowAccessRestriction: false);
				}
			}
			else
			{
				m_TargetSeeker.FindTargets(m_Entity, 0f);
			}
			BackSide backSide = default(BackSide);
			if (m_BackSideData.TryGetComponent(m_Entity, ref backSide))
			{
				Transform transform2 = m_TargetSeeker.m_Transform[m_Entity];
				if (backSide.m_RoadEdge != Entity.Null)
				{
					BuildingData buildingData2 = m_TargetSeeker.m_BuildingData[prefabRef.m_Prefab];
					float3 comparePosition2 = transform2.m_Position;
					Owner owner2 = default(Owner);
					if (!m_TargetSeeker.m_Owner.TryGetComponent(backSide.m_RoadEdge, ref owner2) || owner2.m_Owner != m_Entity)
					{
						comparePosition2 = BuildingUtils.CalculateFrontPosition(transform2, -buildingData2.m_LotSize.y);
					}
					Random random2 = m_TargetSeeker.m_RandomSeed.GetRandom(m_Entity.Index);
					m_TargetSeeker.AddEdgeTargets(ref random2, m_Entity, 0f, EdgeFlags.DefaultMask, backSide.m_RoadEdge, comparePosition2, 0f, allowLaneGroupSwitch: true, allowAccessRestriction: false);
				}
			}
			m_Action.data.m_Parameters = new CoverageParameters
			{
				m_Methods = m_TargetSeeker.m_PathfindParameters.m_Methods,
				m_Range = coverageData.m_Range
			};
		}
	}

	[BurstCompile]
	private struct FillCoverageMapJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<CoverageElement> m_CoverageElements;

		public ParallelWriter<Entity, float2> m_CoverageMap;

		public void Execute(int index)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			CoverageElement coverageElement = m_CoverageElements[index];
			m_CoverageMap.TryAdd(coverageElement.m_Edge, coverageElement.m_Cost);
		}
	}

	[BurstCompile]
	private struct TargetCheckJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<GarbageProducer> m_GarbageProducerType;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> m_ElectricityConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> m_WaterConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<MailProducer> m_MailProducerType;

		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> m_CrimeProducerType;

		[ReadOnly]
		public ComponentLookup<CoverageData> m_PrefabCoverageData;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> m_PrefabGarbageFacilityData;

		[ReadOnly]
		public ComponentLookup<HospitalData> m_PrefabHospitalData;

		[ReadOnly]
		public ComponentLookup<DeathcareFacilityData> m_PrefabDeathcareFacilityData;

		[ReadOnly]
		public ComponentLookup<PowerPlantData> m_PrefabPowerPlantData;

		[ReadOnly]
		public ComponentLookup<WindPoweredData> m_PrefabWindPoweredData;

		[ReadOnly]
		public ComponentLookup<SolarPoweredData> m_PrefabSolarPoweredData;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.TransformerData> m_PrefabTransformerData;

		[ReadOnly]
		public ComponentLookup<WaterPumpingStationData> m_PrefabWaterPumpingStationData;

		[ReadOnly]
		public ComponentLookup<SewageOutletData> m_PrefabSewageOutletData;

		[ReadOnly]
		public ComponentLookup<WastewaterTreatmentPlantData> m_PrefabWastewaterTreatmentPlantData;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_PrefabTransportDepotData;

		[ReadOnly]
		public ComponentLookup<TransportStationData> m_PrefabTransportStationData;

		[ReadOnly]
		public ComponentLookup<PublicTransportStationData> m_PrefabPublicTransportStationData;

		[ReadOnly]
		public ComponentLookup<CargoTransportStationData> m_PrefabCargoTransportStationData;

		[ReadOnly]
		public ComponentLookup<TransportStopData> m_PrefabTransportStopData;

		[ReadOnly]
		public ComponentLookup<PostFacilityData> m_PrefabPostFacilityData;

		[ReadOnly]
		public ComponentLookup<TelecomFacilityData> m_PrefabTelecomFacilityData;

		[ReadOnly]
		public ComponentLookup<SchoolData> m_PrefabSchoolData;

		[ReadOnly]
		public ComponentLookup<ParkingFacilityData> m_PrefabParkingFacilityData;

		[ReadOnly]
		public ComponentLookup<MaintenanceDepotData> m_PrefabMaintenanceDepotData;

		[ReadOnly]
		public ComponentLookup<FireStationData> m_PrefabFireStationData;

		[ReadOnly]
		public ComponentLookup<PoliceStationData> m_PrefabPoliceStationData;

		[ReadOnly]
		public ComponentLookup<PrisonData> m_PrefabPrisonData;

		[ReadOnly]
		public ComponentLookup<PollutionData> m_PrefabPollutionData;

		[ReadOnly]
		public ComponentLookup<AttractionData> m_PrefabAttractionData;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverages;

		[ReadOnly]
		public BufferLookup<LocalModifierData> m_PrefabLocalModifierDatas;

		[ReadOnly]
		public BufferLookup<CityModifierData> m_PrefabCityModifierDatas;

		[ReadOnly]
		public Feedback m_FeedbackData;

		[ReadOnly]
		public DynamicBuffer<ExtraFeedback> m_ExtraFeedbacks;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeParallelHashMap<Entity, float2> m_CoverageMap;

		[ReadOnly]
		public NativeParallelHashMap<RecentKey, RecentValue> m_RecentMap;

		[ReadOnly]
		public FeedbackConfigurationData m_FeedbackConfigurationData;

		[ReadOnly]
		public DynamicBuffer<FeedbackLocalEffectFactor> m_FeedbackLocalEffectFactors;

		[ReadOnly]
		public DynamicBuffer<FeedbackCityEffectFactor> m_FeedbackCityEffectFactors;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverageData;

		public IconCommandBuffer m_IconCommandBuffer;

		public ParallelWriter<RecentUpdate> m_RecentUpdates;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_074a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0752: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0901: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0992: Unknown result type (might be due to invalid IL or missing references)
			//IL_0997: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_13b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_13c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_14a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_152e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1728: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_17e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_17ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_17f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_17f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1818: Unknown result type (might be due to invalid IL or missing references)
			//IL_181d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1822: Unknown result type (might be due to invalid IL or missing references)
			//IL_1824: Unknown result type (might be due to invalid IL or missing references)
			//IL_1826: Unknown result type (might be due to invalid IL or missing references)
			//IL_182b: Unknown result type (might be due to invalid IL or missing references)
			//IL_182d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1857: Unknown result type (might be due to invalid IL or missing references)
			//IL_185c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1861: Unknown result type (might be due to invalid IL or missing references)
			//IL_1866: Unknown result type (might be due to invalid IL or missing references)
			//IL_186b: Unknown result type (might be due to invalid IL or missing references)
			//IL_186d: Unknown result type (might be due to invalid IL or missing references)
			//IL_186f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1874: Unknown result type (might be due to invalid IL or missing references)
			//IL_1879: Unknown result type (might be due to invalid IL or missing references)
			//IL_187b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1793: Unknown result type (might be due to invalid IL or missing references)
			//IL_16a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_16c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_18a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_188e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1897: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e48: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_18cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_18b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_18c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_18e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_18eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1925: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b26: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b36: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b51: Unknown result type (might be due to invalid IL or missing references)
			//IL_19a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_19ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_19e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_1023: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b63: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b82: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b84: Unknown result type (might be due to invalid IL or missing references)
			//IL_1b8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ab1: Unknown result type (might be due to invalid IL or missing references)
			//IL_19fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1ade: Unknown result type (might be due to invalid IL or missing references)
			//IL_1a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1132: Unknown result type (might be due to invalid IL or missing references)
			//IL_117c: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_10dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_124f: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_1325: Unknown result type (might be due to invalid IL or missing references)
			//IL_1390: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Building> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			NativeArray<GarbageProducer> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageProducer>(ref m_GarbageProducerType);
			NativeArray<ElectricityConsumer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ElectricityConsumer>(ref m_ElectricityConsumerType);
			NativeArray<WaterConsumer> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterConsumer>(ref m_WaterConsumerType);
			NativeArray<MailProducer> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MailProducer>(ref m_MailProducerType);
			NativeArray<CrimeProducer> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CrimeProducer>(ref m_CrimeProducerType);
			CoverageData coverageData = default(CoverageData);
			if (m_PrefabCoverageData.HasComponent(m_FeedbackData.m_MainPrefab))
			{
				coverageData = m_PrefabCoverageData[m_FeedbackData.m_MainPrefab];
				if (m_FeedbackData.m_Prefab != m_FeedbackData.m_MainPrefab)
				{
					coverageData.m_Magnitude = 1f;
					coverageData.m_Service = CoverageService.Count;
				}
				else
				{
					coverageData.m_Magnitude = 1f / math.max(0.001f, coverageData.m_Magnitude);
				}
			}
			else
			{
				coverageData.m_Range = 25000f;
				coverageData.m_Magnitude = 1f;
				coverageData.m_Service = CoverageService.Count;
			}
			GarbageFacilityData garbageFacilityData = default(GarbageFacilityData);
			m_PrefabGarbageFacilityData.TryGetComponent(m_FeedbackData.m_Prefab, ref garbageFacilityData);
			HospitalData hospitalData = default(HospitalData);
			m_PrefabHospitalData.TryGetComponent(m_FeedbackData.m_Prefab, ref hospitalData);
			DeathcareFacilityData deathcareFacilityData = default(DeathcareFacilityData);
			m_PrefabDeathcareFacilityData.TryGetComponent(m_FeedbackData.m_Prefab, ref deathcareFacilityData);
			PowerPlantData powerPlantData = default(PowerPlantData);
			m_PrefabPowerPlantData.TryGetComponent(m_FeedbackData.m_Prefab, ref powerPlantData);
			WindPoweredData windPoweredData = default(WindPoweredData);
			m_PrefabWindPoweredData.TryGetComponent(m_FeedbackData.m_Prefab, ref windPoweredData);
			SolarPoweredData solarPoweredData = default(SolarPoweredData);
			m_PrefabSolarPoweredData.TryGetComponent(m_FeedbackData.m_Prefab, ref solarPoweredData);
			WaterPumpingStationData waterPumpingStationData = default(WaterPumpingStationData);
			m_PrefabWaterPumpingStationData.TryGetComponent(m_FeedbackData.m_Prefab, ref waterPumpingStationData);
			SewageOutletData sewageOutletData = default(SewageOutletData);
			m_PrefabSewageOutletData.TryGetComponent(m_FeedbackData.m_Prefab, ref sewageOutletData);
			WastewaterTreatmentPlantData wastewaterTreatmentPlantData = default(WastewaterTreatmentPlantData);
			m_PrefabWastewaterTreatmentPlantData.TryGetComponent(m_FeedbackData.m_Prefab, ref wastewaterTreatmentPlantData);
			TransportDepotData transportDepotData = default(TransportDepotData);
			m_PrefabTransportDepotData.TryGetComponent(m_FeedbackData.m_Prefab, ref transportDepotData);
			TransportStationData transportStationData = default(TransportStationData);
			m_PrefabTransportStationData.TryGetComponent(m_FeedbackData.m_Prefab, ref transportStationData);
			TransportStopData transportStopData = default(TransportStopData);
			m_PrefabTransportStopData.TryGetComponent(m_FeedbackData.m_Prefab, ref transportStopData);
			PostFacilityData postFacilityData = default(PostFacilityData);
			m_PrefabPostFacilityData.TryGetComponent(m_FeedbackData.m_Prefab, ref postFacilityData);
			TelecomFacilityData telecomFacilityData = default(TelecomFacilityData);
			m_PrefabTelecomFacilityData.TryGetComponent(m_FeedbackData.m_Prefab, ref telecomFacilityData);
			SchoolData schoolData = default(SchoolData);
			m_PrefabSchoolData.TryGetComponent(m_FeedbackData.m_Prefab, ref schoolData);
			ParkingFacilityData parkingFacilityData = default(ParkingFacilityData);
			bool flag = m_PrefabParkingFacilityData.TryGetComponent(m_FeedbackData.m_Prefab, ref parkingFacilityData);
			MaintenanceDepotData maintenanceDepotData = default(MaintenanceDepotData);
			m_PrefabMaintenanceDepotData.TryGetComponent(m_FeedbackData.m_Prefab, ref maintenanceDepotData);
			FireStationData fireStationData = default(FireStationData);
			m_PrefabFireStationData.TryGetComponent(m_FeedbackData.m_Prefab, ref fireStationData);
			PoliceStationData policeStationData = default(PoliceStationData);
			m_PrefabPoliceStationData.TryGetComponent(m_FeedbackData.m_Prefab, ref policeStationData);
			PrisonData prisonData = default(PrisonData);
			m_PrefabPrisonData.TryGetComponent(m_FeedbackData.m_Prefab, ref prisonData);
			PollutionData pollutionData = default(PollutionData);
			m_PrefabPollutionData.TryGetComponent(m_FeedbackData.m_Prefab, ref pollutionData);
			AttractionData attractionData = default(AttractionData);
			m_PrefabAttractionData.TryGetComponent(m_FeedbackData.m_Prefab, ref attractionData);
			bool flag2 = m_PrefabTransformerData.HasComponent(m_FeedbackData.m_Prefab);
			bool flag3 = m_PrefabPublicTransportStationData.HasComponent(m_FeedbackData.m_Prefab);
			bool flag4 = m_PrefabCargoTransportStationData.HasComponent(m_FeedbackData.m_Prefab);
			float num = 0f;
			NativeList<LocalModifierData> tempModifierList = default(NativeList<LocalModifierData>);
			NativeList<CityModifierData> tempModifierList2 = default(NativeList<CityModifierData>);
			DynamicBuffer<LocalModifierData> localModifiers = default(DynamicBuffer<LocalModifierData>);
			if (m_PrefabLocalModifierDatas.TryGetBuffer(m_FeedbackData.m_Prefab, ref localModifiers))
			{
				tempModifierList._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
				LocalEffectSystem.InitializeTempList(tempModifierList, localModifiers);
			}
			DynamicBuffer<CityModifierData> cityModifiers = default(DynamicBuffer<CityModifierData>);
			if (m_PrefabCityModifierDatas.TryGetBuffer(m_FeedbackData.m_Prefab, ref cityModifiers))
			{
				tempModifierList2._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
				CityModifierUpdateSystem.InitializeTempList(tempModifierList2, cityModifiers);
			}
			if (m_FeedbackData.m_Prefab == m_FeedbackData.m_MainPrefab)
			{
				GarbageFacilityData otherData = default(GarbageFacilityData);
				HospitalData otherData2 = default(HospitalData);
				DeathcareFacilityData otherData3 = default(DeathcareFacilityData);
				PowerPlantData otherData4 = default(PowerPlantData);
				WindPoweredData otherData5 = default(WindPoweredData);
				SolarPoweredData otherData6 = default(SolarPoweredData);
				WaterPumpingStationData otherData7 = default(WaterPumpingStationData);
				SewageOutletData otherData8 = default(SewageOutletData);
				WastewaterTreatmentPlantData otherData9 = default(WastewaterTreatmentPlantData);
				TransportDepotData otherData10 = default(TransportDepotData);
				TransportStationData otherData11 = default(TransportStationData);
				PostFacilityData otherData12 = default(PostFacilityData);
				TelecomFacilityData otherData13 = default(TelecomFacilityData);
				SchoolData otherData14 = default(SchoolData);
				ParkingFacilityData otherData15 = default(ParkingFacilityData);
				MaintenanceDepotData otherData16 = default(MaintenanceDepotData);
				FireStationData otherData17 = default(FireStationData);
				PoliceStationData otherData18 = default(PoliceStationData);
				PrisonData otherData19 = default(PrisonData);
				PollutionData otherData20 = default(PollutionData);
				AttractionData otherData21 = default(AttractionData);
				DynamicBuffer<LocalModifierData> localModifiers2 = default(DynamicBuffer<LocalModifierData>);
				DynamicBuffer<CityModifierData> cityModifiers2 = default(DynamicBuffer<CityModifierData>);
				for (int i = 0; i < m_ExtraFeedbacks.Length; i++)
				{
					Entity prefab = m_ExtraFeedbacks[i].m_Prefab;
					if (m_PrefabGarbageFacilityData.TryGetComponent(prefab, ref otherData))
					{
						garbageFacilityData.Combine(otherData);
					}
					if (m_PrefabHospitalData.TryGetComponent(prefab, ref otherData2))
					{
						hospitalData.Combine(otherData2);
					}
					if (m_PrefabDeathcareFacilityData.TryGetComponent(prefab, ref otherData3))
					{
						deathcareFacilityData.Combine(otherData3);
					}
					if (m_PrefabPowerPlantData.TryGetComponent(prefab, ref otherData4))
					{
						powerPlantData.Combine(otherData4);
					}
					if (m_PrefabWindPoweredData.TryGetComponent(prefab, ref otherData5))
					{
						windPoweredData.Combine(otherData5);
					}
					if (m_PrefabSolarPoweredData.TryGetComponent(prefab, ref otherData6))
					{
						solarPoweredData.Combine(otherData6);
					}
					if (m_PrefabWaterPumpingStationData.TryGetComponent(prefab, ref otherData7))
					{
						waterPumpingStationData.Combine(otherData7);
					}
					if (m_PrefabSewageOutletData.TryGetComponent(prefab, ref otherData8))
					{
						sewageOutletData.Combine(otherData8);
					}
					if (m_PrefabWastewaterTreatmentPlantData.TryGetComponent(prefab, ref otherData9))
					{
						wastewaterTreatmentPlantData.Combine(otherData9);
					}
					if (m_PrefabTransportDepotData.TryGetComponent(prefab, ref otherData10))
					{
						transportDepotData.Combine(otherData10);
					}
					if (m_PrefabTransportStationData.TryGetComponent(prefab, ref otherData11))
					{
						transportStationData.Combine(otherData11);
					}
					flag3 |= m_PrefabPublicTransportStationData.HasComponent(prefab);
					flag4 |= m_PrefabCargoTransportStationData.HasComponent(prefab);
					if (m_PrefabPostFacilityData.TryGetComponent(prefab, ref otherData12))
					{
						postFacilityData.Combine(otherData12);
					}
					if (m_PrefabTelecomFacilityData.TryGetComponent(prefab, ref otherData13))
					{
						telecomFacilityData.Combine(otherData13);
					}
					if (m_PrefabSchoolData.TryGetComponent(prefab, ref otherData14))
					{
						schoolData.Combine(otherData14);
					}
					if (m_PrefabParkingFacilityData.TryGetComponent(prefab, ref otherData15))
					{
						parkingFacilityData.Combine(otherData15);
						flag = true;
					}
					if (m_PrefabMaintenanceDepotData.TryGetComponent(prefab, ref otherData16))
					{
						maintenanceDepotData.Combine(otherData16);
					}
					if (m_PrefabFireStationData.TryGetComponent(prefab, ref otherData17))
					{
						fireStationData.Combine(otherData17);
					}
					if (m_PrefabPoliceStationData.TryGetComponent(prefab, ref otherData18))
					{
						policeStationData.Combine(otherData18);
					}
					if (m_PrefabPrisonData.TryGetComponent(prefab, ref otherData19))
					{
						prisonData.Combine(otherData19);
					}
					if (m_PrefabPollutionData.TryGetComponent(prefab, ref otherData20))
					{
						pollutionData.Combine(otherData20);
					}
					if (m_PrefabAttractionData.TryGetComponent(prefab, ref otherData21))
					{
						attractionData.Combine(otherData21);
					}
					if (m_PrefabLocalModifierDatas.TryGetBuffer(m_FeedbackData.m_Prefab, ref localModifiers2))
					{
						LocalEffectSystem.AddToTempList(tempModifierList, localModifiers2, disabled: false);
					}
					if (m_PrefabCityModifierDatas.TryGetBuffer(m_FeedbackData.m_Prefab, ref cityModifiers2))
					{
						CityModifierUpdateSystem.AddToTempList(tempModifierList2, cityModifiers2);
					}
				}
			}
			else
			{
				bool flag5 = telecomFacilityData.m_Range >= 1f;
				bool flag6 = telecomFacilityData.m_NetworkCapacity >= 1f;
				if (flag5 || flag6)
				{
					TelecomFacilityData telecomFacilityData2 = default(TelecomFacilityData);
					m_PrefabTelecomFacilityData.TryGetComponent(m_FeedbackData.m_MainPrefab, ref telecomFacilityData2);
					TelecomFacilityData otherData22 = default(TelecomFacilityData);
					for (int j = 0; j < m_ExtraFeedbacks.Length; j++)
					{
						Entity prefab2 = m_ExtraFeedbacks[j].m_Prefab;
						if (m_PrefabTelecomFacilityData.TryGetComponent(prefab2, ref otherData22))
						{
							telecomFacilityData2.Combine(otherData22);
						}
					}
					if (flag5)
					{
						telecomFacilityData.m_NetworkCapacity += telecomFacilityData2.m_NetworkCapacity;
					}
					telecomFacilityData.m_Range += telecomFacilityData2.m_Range;
					if (flag5 && !flag6)
					{
						num = telecomFacilityData2.m_Range;
					}
				}
				bool flag7 = fireStationData.m_FireEngineCapacity != 0;
				bool flag8 = fireStationData.m_FireHelicopterCapacity != 0;
				bool flag9 = fireStationData.m_DisasterResponseCapacity != 0;
				bool flag10 = fireStationData.m_VehicleEfficiency != 0f;
				if (flag7 || flag8 || flag9 || flag10)
				{
					FireStationData fireStationData2 = default(FireStationData);
					m_PrefabFireStationData.TryGetComponent(m_FeedbackData.m_MainPrefab, ref fireStationData2);
					FireStationData otherData23 = default(FireStationData);
					for (int k = 0; k < m_ExtraFeedbacks.Length; k++)
					{
						Entity prefab3 = m_ExtraFeedbacks[k].m_Prefab;
						if (m_PrefabFireStationData.TryGetComponent(prefab3, ref otherData23))
						{
							fireStationData2.Combine(otherData23);
						}
					}
					if (flag7 || flag8)
					{
						fireStationData.m_VehicleEfficiency += fireStationData2.m_VehicleEfficiency;
					}
					if (flag9 || flag10)
					{
						fireStationData.m_FireEngineCapacity += fireStationData2.m_FireEngineCapacity;
						fireStationData.m_FireHelicopterCapacity += fireStationData2.m_FireHelicopterCapacity;
					}
				}
				bool flag11 = (float)maintenanceDepotData.m_VehicleCapacity != 0f;
				bool flag12 = maintenanceDepotData.m_VehicleEfficiency != 0f;
				if (flag11 || flag12)
				{
					MaintenanceDepotData maintenanceDepotData2 = default(MaintenanceDepotData);
					m_PrefabMaintenanceDepotData.TryGetComponent(m_FeedbackData.m_MainPrefab, ref maintenanceDepotData2);
					MaintenanceDepotData otherData24 = default(MaintenanceDepotData);
					for (int l = 0; l < m_ExtraFeedbacks.Length; l++)
					{
						Entity prefab4 = m_ExtraFeedbacks[l].m_Prefab;
						if (m_PrefabMaintenanceDepotData.TryGetComponent(prefab4, ref otherData24))
						{
							maintenanceDepotData2.Combine(otherData24);
						}
					}
					if (flag11)
					{
						maintenanceDepotData.m_VehicleEfficiency += maintenanceDepotData2.m_VehicleEfficiency;
					}
					if (flag12)
					{
						maintenanceDepotData.m_VehicleCapacity += maintenanceDepotData2.m_VehicleCapacity;
					}
				}
				if (transportDepotData.m_DispatchCenter)
				{
					TransportDepotData transportDepotData2 = default(TransportDepotData);
					m_PrefabTransportDepotData.TryGetComponent(m_FeedbackData.m_MainPrefab, ref transportDepotData2);
					TransportDepotData otherData25 = default(TransportDepotData);
					for (int m = 0; m < m_ExtraFeedbacks.Length; m++)
					{
						Entity prefab5 = m_ExtraFeedbacks[m].m_Prefab;
						if (m_PrefabTransportDepotData.TryGetComponent(prefab5, ref otherData25))
						{
							transportDepotData2.Combine(otherData25);
						}
					}
					transportDepotData.m_VehicleCapacity += transportDepotData2.m_VehicleCapacity;
				}
				if (tempModifierList.IsCreated)
				{
					DynamicBuffer<LocalModifierData> localModifiers3 = default(DynamicBuffer<LocalModifierData>);
					if (m_PrefabLocalModifierDatas.TryGetBuffer(m_FeedbackData.m_MainPrefab, ref localModifiers3))
					{
						AddToTempListForUpgrade(tempModifierList, localModifiers3);
					}
					DynamicBuffer<LocalModifierData> localModifiers4 = default(DynamicBuffer<LocalModifierData>);
					for (int n = 0; n < m_ExtraFeedbacks.Length; n++)
					{
						Entity prefab6 = m_ExtraFeedbacks[n].m_Prefab;
						if (m_PrefabLocalModifierDatas.TryGetBuffer(prefab6, ref localModifiers4))
						{
							AddToTempListForUpgrade(tempModifierList, localModifiers4);
						}
					}
				}
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeList<RecentUpdate> updateList = default(NativeList<RecentUpdate>);
			float2 val2 = default(float2);
			DynamicBuffer<Game.Net.ServiceCoverage> val3 = default(DynamicBuffer<Game.Net.ServiceCoverage>);
			for (int num2 = 0; num2 < nativeArray.Length; num2++)
			{
				Entity val = nativeArray[num2];
				Transform transform = nativeArray2[num2];
				Building building = nativeArray3[num2];
				if (val == m_FeedbackData.m_MainEntity)
				{
					continue;
				}
				float3 total = float3.op_Implicit(0f);
				float num4;
				float num3;
				if (m_CoverageMap.IsCreated && m_CoverageMap.TryGetValue(building.m_RoadEdge, ref val2))
				{
					num3 = math.lerp(val2.x, val2.y, building.m_CurvePosition);
					num4 = math.max(0f, 1f - num3 * num3);
					num3 *= coverageData.m_Range;
					if (num4 != 0f)
					{
						if (coverageData.m_Service != CoverageService.Count && m_ServiceCoverages.TryGetBuffer(building.m_RoadEdge, ref val3) && val3.Length != 0)
						{
							Game.Net.ServiceCoverage serviceCoverage = val3[(int)coverageData.m_Service];
							float num5 = 1f + math.lerp(serviceCoverage.m_Coverage.x, serviceCoverage.m_Coverage.y, building.m_CurvePosition) * coverageData.m_Magnitude;
							float delta = num4 / math.max(num4, num5 * num5);
							AddEffect(ref updateList, ref total, new RecentKey(val, coverageData.m_Service), delta, num3);
						}
						if (garbageFacilityData.m_VehicleCapacity != 0 && nativeArray4.Length != 0)
						{
							float delta2 = num4 * math.saturate((float)nativeArray4[num2].m_Garbage * m_FeedbackConfigurationData.m_GarbageProducerGarbageFactor) * math.saturate((float)garbageFacilityData.m_VehicleCapacity * m_FeedbackConfigurationData.m_GarbageVehicleFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.GarbageVehicles), delta2, num3);
						}
						if (hospitalData.m_AmbulanceCapacity != 0)
						{
							float delta3 = num4 * math.saturate((float)hospitalData.m_AmbulanceCapacity * m_FeedbackConfigurationData.m_HospitalAmbulanceFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.HospitalAmbulances), delta3, num3);
						}
						if (hospitalData.m_PatientCapacity != 0)
						{
							float delta4 = num4 * math.saturate((float)hospitalData.m_PatientCapacity * m_FeedbackConfigurationData.m_HospitalCapacityFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.HospitalCapacity), delta4, num3);
						}
						if (deathcareFacilityData.m_HearseCapacity != 0)
						{
							float delta5 = num4 * math.saturate((float)deathcareFacilityData.m_HearseCapacity * m_FeedbackConfigurationData.m_DeathcareHearseFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.DeathcareHearses), delta5, num3);
						}
						if (deathcareFacilityData.m_StorageCapacity != 0 || deathcareFacilityData.m_ProcessingRate != 0f)
						{
							float delta6 = num4 * math.saturate((float)deathcareFacilityData.m_StorageCapacity * m_FeedbackConfigurationData.m_DeathcareCapacityFactor + deathcareFacilityData.m_ProcessingRate * m_FeedbackConfigurationData.m_DeathcareProcessingFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.DeathcareCapacity), delta6, num3);
						}
						if (flag2 && nativeArray5.Length != 0)
						{
							ElectricityConsumer electricityConsumer = nativeArray5[num2];
							float num6 = math.saturate((float)electricityConsumer.m_WantedConsumption * m_FeedbackConfigurationData.m_ElectricityConsumptionFactor);
							float num7 = num4 * math.select(num6, 1f, !electricityConsumer.electricityConnected);
							num7 *= math.saturate(1f - num3 / m_FeedbackConfigurationData.m_TransformerRadius);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.Transformer), num7, num3);
						}
						if (transportDepotData.m_DispatchCenter)
						{
							float delta7 = num4 * math.saturate((float)transportDepotData.m_VehicleCapacity * m_FeedbackConfigurationData.m_TransportDispatchCenterFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.TransportDispatch), delta7, num3);
						}
						if (flag3)
						{
							float num8 = num4 * math.saturate(0.5f + transportStationData.m_ComfortFactor * 0.5f);
							num8 *= math.saturate(1f - num3 / m_FeedbackConfigurationData.m_TransportStationRange);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PublicTransport), num8, num3);
						}
						if (flag4)
						{
							float num9 = num4 * math.saturate(0.5f + transportStationData.m_LoadingFactor * 0.5f);
							num9 *= math.saturate(1f - num3 / m_FeedbackConfigurationData.m_TransportStationRange);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.CargoTransport), num9, num3);
						}
						if (transportStopData.m_PassengerTransport)
						{
							float num10 = num4 * math.saturate(0.5f + transportStopData.m_ComfortFactor * 0.5f);
							num10 *= math.saturate(1f - num3 / m_FeedbackConfigurationData.m_TransportStopRange);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PublicTransport), num10, num3);
						}
						if ((postFacilityData.m_PostVanCapacity != 0 || postFacilityData.m_PostTruckCapacity != 0) && nativeArray7.Length != 0)
						{
							MailProducer mailProducer = nativeArray7[num2];
							float delta8 = num4 * math.saturate((float)(mailProducer.receivingMail + mailProducer.m_SendingMail) * m_FeedbackConfigurationData.m_MailProducerMailFactor) * math.saturate((float)postFacilityData.m_PostVanCapacity * m_FeedbackConfigurationData.m_PostFacilityVanFactor + (float)postFacilityData.m_PostTruckCapacity * m_FeedbackConfigurationData.m_PostFacilityTruckFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PostFacilityVehicles), delta8, num3);
						}
						if ((postFacilityData.m_MailCapacity != 0 || postFacilityData.m_SortingRate != 0) && nativeArray7.Length != 0)
						{
							MailProducer mailProducer2 = nativeArray7[num2];
							float delta9 = num4 * math.saturate((float)(mailProducer2.receivingMail + mailProducer2.m_SendingMail) * m_FeedbackConfigurationData.m_MailProducerMailFactor) * math.saturate((float)postFacilityData.m_MailCapacity * m_FeedbackConfigurationData.m_PostFacilityCapacityFactor + (float)postFacilityData.m_SortingRate * m_FeedbackConfigurationData.m_PostFacilityProcessingFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PostFacilityCapacity), delta9, num3);
						}
						if (schoolData.m_StudentCapacity != 0)
						{
							float num11 = 0f;
							FeedbackType feedbackType = FeedbackType.GarbageVehicles;
							switch ((SchoolLevel)schoolData.m_EducationLevel)
							{
							case SchoolLevel.Elementary:
								num11 = m_FeedbackConfigurationData.m_ElementarySchoolCapacityFactor;
								feedbackType = FeedbackType.ElementarySchoolCapacity;
								break;
							case SchoolLevel.HighSchool:
								num11 = m_FeedbackConfigurationData.m_HighSchoolCapacityFactor;
								feedbackType = FeedbackType.HighSchoolCapacity;
								break;
							case SchoolLevel.College:
								num11 = m_FeedbackConfigurationData.m_CollegeCapacityFactor;
								feedbackType = FeedbackType.CollegeCapacity;
								break;
							case SchoolLevel.University:
								num11 = m_FeedbackConfigurationData.m_UniversityCapacityFactor;
								feedbackType = FeedbackType.UniversityCapacity;
								break;
							}
							if (num11 != 0f)
							{
								float delta10 = num4 * math.saturate((float)schoolData.m_StudentCapacity * num11);
								AddEffect(ref updateList, ref total, new RecentKey(val, feedbackType), delta10, num3);
							}
						}
						if (flag)
						{
							float num12 = num4 * math.saturate(0.5f + parkingFacilityData.m_ComfortFactor * 0.5f);
							num12 *= math.saturate(1f - num3 / m_FeedbackConfigurationData.m_ParkingFacilityRange);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.ParkingSpaces), num12, num3);
						}
						if (maintenanceDepotData.m_VehicleCapacity != 0)
						{
							float num13 = (float)maintenanceDepotData.m_VehicleCapacity * maintenanceDepotData.m_VehicleEfficiency;
							float delta11 = num4 * math.saturate(num13 * m_FeedbackConfigurationData.m_MaintenanceVehicleFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, maintenanceDepotData.m_MaintenanceType), delta11, num3);
						}
						if (fireStationData.m_FireEngineCapacity != 0)
						{
							float num14 = (float)fireStationData.m_FireEngineCapacity * fireStationData.m_VehicleEfficiency;
							num14 += (float)math.min(fireStationData.m_FireEngineCapacity, fireStationData.m_DisasterResponseCapacity);
							float delta12 = num4 * math.saturate(num14 * m_FeedbackConfigurationData.m_FireStationEngineFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.FireStationEngines), delta12, num3);
						}
						if (policeStationData.m_PatrolCarCapacity != 0 && nativeArray8.Length != 0)
						{
							float delta13 = num4 * math.saturate(nativeArray8[num2].m_Crime * m_FeedbackConfigurationData.m_CrimeProducerCrimeFactor) * math.saturate((float)policeStationData.m_PatrolCarCapacity * m_FeedbackConfigurationData.m_PoliceStationCarFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PoliceStationCars), delta13, num3);
						}
						if (policeStationData.m_JailCapacity != 0 && nativeArray8.Length != 0)
						{
							float delta14 = num4 * math.saturate(nativeArray8[num2].m_Crime * m_FeedbackConfigurationData.m_CrimeProducerCrimeFactor) * math.saturate((float)policeStationData.m_JailCapacity * m_FeedbackConfigurationData.m_PoliceStationCapacityFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PoliceStationCapacity), delta14, num3);
						}
						if (prisonData.m_PrisonVanCapacity != 0 && nativeArray8.Length != 0)
						{
							float delta15 = num4 * math.saturate(nativeArray8[num2].m_Crime * m_FeedbackConfigurationData.m_CrimeProducerCrimeFactor) * math.saturate((float)prisonData.m_PrisonVanCapacity * m_FeedbackConfigurationData.m_PrisonVehicleFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PrisonVehicles), delta15, num3);
						}
						if (prisonData.m_PrisonerCapacity != 0 && nativeArray8.Length != 0)
						{
							float delta16 = num4 * math.saturate(nativeArray8[num2].m_Crime * m_FeedbackConfigurationData.m_CrimeProducerCrimeFactor) * math.saturate((float)prisonData.m_PrisonerCapacity * m_FeedbackConfigurationData.m_PrisonCapacityFactor);
							AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PrisonCapacity), delta16, num3);
						}
					}
				}
				num4 = 1f;
				num3 = math.distance(((float3)(ref transform.m_Position)).xz, ((float3)(ref m_FeedbackData.m_Position)).xz);
				if (hospitalData.m_MedicalHelicopterCapacity != 0)
				{
					float delta17 = num4 * math.saturate((float)hospitalData.m_MedicalHelicopterCapacity * m_FeedbackConfigurationData.m_HospitalHelicopterFactor);
					AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.HospitalHelicopters), delta17, num3);
				}
				if ((powerPlantData.m_ElectricityProduction != 0 || windPoweredData.m_Production != 0 || solarPoweredData.m_Production != 0) && nativeArray5.Length != 0)
				{
					ElectricityConsumer electricityConsumer2 = nativeArray5[num2];
					float num15 = math.saturate((float)electricityConsumer2.m_WantedConsumption * m_FeedbackConfigurationData.m_ElectricityConsumptionFactor);
					float num16 = powerPlantData.m_ElectricityProduction + windPoweredData.m_Production + solarPoweredData.m_Production;
					float delta18 = num4 * math.select(num15, 1f, !electricityConsumer2.electricityConnected) * math.saturate(num16 * m_FeedbackConfigurationData.m_ElectricityProductionFactor);
					AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.Electricity), delta18, num3);
				}
				if (waterPumpingStationData.m_Capacity != 0 && nativeArray6.Length != 0)
				{
					WaterConsumer waterConsumer = nativeArray6[num2];
					float num17 = math.saturate((float)waterConsumer.m_WantedConsumption * m_FeedbackConfigurationData.m_WaterConsumptionFactor);
					float num18 = waterPumpingStationData.m_Capacity;
					float delta19 = num4 * math.select(num17, 1f, !waterConsumer.waterConnected) * math.saturate(num18 * m_FeedbackConfigurationData.m_WaterCapacityFactor);
					AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.WaterCapacity), delta19, num3);
				}
				if ((sewageOutletData.m_Capacity != 0 || wastewaterTreatmentPlantData.m_Capacity != 0) && nativeArray6.Length != 0)
				{
					WaterConsumer waterConsumer2 = nativeArray6[num2];
					float num19 = math.saturate((float)waterConsumer2.m_WantedConsumption * m_FeedbackConfigurationData.m_WaterConsumerSewageFactor);
					float num20 = sewageOutletData.m_Capacity + wastewaterTreatmentPlantData.m_Capacity;
					float delta20 = num4 * math.select(num19, 1f, !waterConsumer2.sewageConnected) * math.saturate(num20 * m_FeedbackConfigurationData.m_SewageCapacityFactor);
					AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.SewageCapacity), delta20, num3);
				}
				if (transportDepotData.m_VehicleCapacity != 0)
				{
					float delta21 = num4 * math.saturate((float)transportDepotData.m_VehicleCapacity * m_FeedbackConfigurationData.m_TransportVehicleCapacityFactor);
					AddEffect(ref updateList, ref total, new RecentKey(val, transportDepotData.m_TransportType), delta21, num3);
				}
				if (telecomFacilityData.m_NetworkCapacity >= 1f && telecomFacilityData.m_Range >= 1f)
				{
					float num21 = num4 * math.saturate(telecomFacilityData.m_NetworkCapacity * m_FeedbackConfigurationData.m_TelecomCapacityFactor);
					num21 *= math.saturate(1f - num3 / telecomFacilityData.m_Range);
					if (num >= 1f)
					{
						num21 *= math.saturate(num3 / num);
					}
					if (num21 != 0f)
					{
						float num22 = 1f + TelecomCoverage.SampleNetworkQuality(m_TelecomCoverageData, transform.m_Position);
						num21 /= math.max(num21, num22);
						AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.TelecomCoverage), num21, num3);
					}
				}
				if (fireStationData.m_FireHelicopterCapacity != 0)
				{
					float num23 = (float)fireStationData.m_FireHelicopterCapacity * fireStationData.m_VehicleEfficiency;
					num23 += (float)math.min(fireStationData.m_FireHelicopterCapacity, fireStationData.m_DisasterResponseCapacity);
					float delta22 = num4 * math.saturate(num23 * m_FeedbackConfigurationData.m_FireStationHelicopterFactor);
					AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.FireStationHelicopters), delta22, num3);
				}
				if (policeStationData.m_PoliceHelicopterCapacity != 0 && nativeArray8.Length != 0)
				{
					float delta23 = num4 * math.saturate(nativeArray8[num2].m_Crime * m_FeedbackConfigurationData.m_CrimeProducerCrimeFactor) * math.saturate((float)policeStationData.m_PoliceHelicopterCapacity * m_FeedbackConfigurationData.m_PoliceStationHelicopterFactor);
					AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.PoliceStationHelicopters), delta23, num3);
				}
				if (pollutionData.m_GroundPollution != 0f || pollutionData.m_AirPollution != 0f || pollutionData.m_NoisePollution != 0f)
				{
					float3 val4 = num4 * new float3(pollutionData.m_GroundPollution, pollutionData.m_AirPollution, pollutionData.m_NoisePollution);
					val4 *= new float3(m_FeedbackConfigurationData.m_GroundPollutionFactor, m_FeedbackConfigurationData.m_AirPollutionFactor, m_FeedbackConfigurationData.m_NoisePollutionFactor);
					val4 = math.saturate(val4);
					val4 *= 1f - num3 / new float3(m_FeedbackConfigurationData.m_GroundPollutionRadius, m_FeedbackConfigurationData.m_AirPollutionRadius, m_FeedbackConfigurationData.m_NoisePollutionRadius);
					val4 = -math.saturate(val4);
					if (val4.x != 0f)
					{
						AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.GroundPollution), val4.x, num3);
					}
					if (val4.y != 0f)
					{
						AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.AirPollution), val4.y, num3);
					}
					if (val4.z != 0f)
					{
						AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.NoisePollution), val4.z, num3);
					}
				}
				if (attractionData.m_Attractiveness != 0)
				{
					float delta24 = num4 * math.saturate((float)attractionData.m_Attractiveness * m_FeedbackConfigurationData.m_AttractivenessFactor);
					AddEffect(ref updateList, ref total, new RecentKey(val, FeedbackType.Attractiveness), delta24, num3);
				}
				if (tempModifierList.IsCreated)
				{
					for (int num24 = 0; num24 < tempModifierList.Length; num24++)
					{
						LocalModifierData localModifierData = tempModifierList[num24];
						if (m_FeedbackLocalEffectFactors.Length > (int)localModifierData.m_Type)
						{
							float factor = m_FeedbackLocalEffectFactors[(int)localModifierData.m_Type].m_Factor;
							factor = math.select(math.sign(factor), factor, localModifierData.m_Mode == ModifierValueMode.Absolute);
							float num25 = num4 * math.clamp(localModifierData.m_Delta.max * factor, -1f, 1f);
							num25 *= math.saturate(1f - num3 / localModifierData.m_Radius.max);
							if (localModifierData.m_Radius.min != 0f)
							{
								num25 *= math.saturate(num3 / localModifierData.m_Radius.min);
							}
							if (num25 != 0f)
							{
								AddEffect(ref updateList, ref total, new RecentKey(val, localModifierData.m_Type), num25, num3);
							}
						}
					}
				}
				if (tempModifierList2.IsCreated)
				{
					for (int num26 = 0; num26 < tempModifierList2.Length; num26++)
					{
						CityModifierData cityModifierData = tempModifierList2[num26];
						if (m_FeedbackCityEffectFactors.Length > (int)cityModifierData.m_Type)
						{
							float factor2 = m_FeedbackCityEffectFactors[(int)cityModifierData.m_Type].m_Factor;
							factor2 = math.select(math.sign(factor2), factor2, cityModifierData.m_Mode == ModifierValueMode.Absolute);
							float num27 = num4 * math.clamp(cityModifierData.m_Range.max * factor2, -1f, 1f);
							if (num27 != 0f)
							{
								AddEffect(ref updateList, ref total, new RecentKey(val, cityModifierData.m_Type), num27, num3);
							}
						}
					}
				}
				if (((Random)(ref random)).NextFloat(1f) < math.abs(total.x))
				{
					bool flag13 = total.x > 0f;
					num3 = total.y / total.z;
					Entity prefab7 = (flag13 ? m_FeedbackConfigurationData.m_HappyFaceNotification : m_FeedbackConfigurationData.m_SadFaceNotification);
					float delay = num3 * 0.001f + ((Random)(ref random)).NextFloat(0.1f);
					m_IconCommandBuffer.Add(val, prefab7, IconPriority.Info, IconClusterLayer.Transaction, (IconFlags)0, Entity.Null, isTemp: false, isHidden: false, disallowCluster: false, delay);
					if (updateList.IsCreated)
					{
						for (int num28 = 0; num28 < updateList.Length; num28++)
						{
							RecentUpdate recentUpdate = updateList[num28];
							if (recentUpdate.m_Delta > 0f == flag13)
							{
								m_RecentUpdates.Enqueue(recentUpdate);
							}
						}
					}
				}
				if (updateList.IsCreated)
				{
					updateList.Clear();
				}
			}
			if (updateList.IsCreated)
			{
				updateList.Dispose();
			}
			if (tempModifierList.IsCreated)
			{
				tempModifierList.Dispose();
			}
			if (tempModifierList2.IsCreated)
			{
				tempModifierList2.Dispose();
			}
		}

		private void AddToTempListForUpgrade(NativeList<LocalModifierData> tempModifierList, DynamicBuffer<LocalModifierData> localModifiers)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < localModifiers.Length; i++)
			{
				LocalModifierData localModifierData = localModifiers[i];
				for (int j = 0; j < tempModifierList.Length; j++)
				{
					LocalModifierData localModifierData2 = tempModifierList[j];
					if (localModifierData2.m_Type != localModifierData.m_Type)
					{
						continue;
					}
					bool flag = localModifierData2.m_Radius.max > 0f;
					bool flag2 = localModifierData2.m_Delta.max != 0f;
					if (flag)
					{
						localModifierData2.m_Delta.max += localModifierData.m_Delta.max;
					}
					switch (localModifierData2.m_RadiusCombineMode)
					{
					case ModifierRadiusCombineMode.Additive:
						if (flag && !flag2)
						{
							localModifierData2.m_Radius.min += localModifierData.m_Radius.max;
						}
						localModifierData2.m_Radius.max += localModifierData.m_Radius.max;
						break;
					case ModifierRadiusCombineMode.Maximal:
						if (flag && !flag2)
						{
							localModifierData2.m_Radius.min = math.max(localModifierData2.m_Radius.min, localModifierData.m_Radius.max);
						}
						localModifierData2.m_Radius.max = math.max(localModifierData2.m_Radius.max, localModifierData.m_Radius.max);
						break;
					}
					tempModifierList[j] = localModifierData2;
					break;
				}
			}
		}

		private void AddEffect(ref NativeList<RecentUpdate> updateList, ref float3 total, RecentKey recentKey, float delta, float distance)
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			delta = math.select(delta, 0f - delta, m_FeedbackData.m_IsDeleted);
			RecentUpdate recentUpdate = new RecentUpdate
			{
				m_Key = recentKey,
				m_Delta = math.sign(delta)
			};
			RecentValue recentValue = default(RecentValue);
			if (m_RecentMap.TryGetValue(recentKey, ref recentValue))
			{
				float num = delta - recentValue.m_FeedbackDelta;
				delta = math.select(math.clamp(num, delta, 0f), math.clamp(num, 0f, delta), delta > 0f);
			}
			if (delta != 0f)
			{
				float num2 = math.abs(delta);
				total += new float3(delta, distance * num2, num2);
				if (!updateList.IsCreated)
				{
					updateList = new NativeList<RecentUpdate>(10, AllocatorHandle.op_Implicit((Allocator)2));
				}
				updateList.Add(ref recentUpdate);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateRecentMapJob : IJob
	{
		public NativeParallelHashMap<RecentKey, RecentValue> m_RecentMap;

		public NativeQueue<RecentUpdate> m_RecentUpdates;

		public uint m_SimulationFrame;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<RecentKey> keyArray = m_RecentMap.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < keyArray.Length; i++)
			{
				RecentKey recentKey = keyArray[i];
				RecentValue recentValue = m_RecentMap[recentKey];
				float num = (float)(m_SimulationFrame - recentValue.m_UpdateFrame) * 0.0001f;
				recentValue.m_UpdateFrame = m_SimulationFrame;
				recentValue.m_FeedbackDelta = math.select(math.min(0f, recentValue.m_FeedbackDelta + num), math.max(0f, recentValue.m_FeedbackDelta - num), recentValue.m_FeedbackDelta > 0f);
				if (recentValue.m_FeedbackDelta != 0f)
				{
					m_RecentMap[recentKey] = recentValue;
				}
				else
				{
					m_RecentMap.Remove(recentKey);
				}
			}
			keyArray.Dispose();
			RecentUpdate recentUpdate = default(RecentUpdate);
			RecentValue recentValue2 = default(RecentValue);
			while (m_RecentUpdates.TryDequeue(ref recentUpdate))
			{
				if (m_RecentMap.TryGetValue(recentUpdate.m_Key, ref recentValue2))
				{
					recentValue2.m_FeedbackDelta += recentUpdate.m_Delta;
					if (recentValue2.m_FeedbackDelta != 0f)
					{
						m_RecentMap[recentUpdate.m_Key] = recentValue2;
					}
					else
					{
						m_RecentMap.Remove(recentUpdate.m_Key);
					}
				}
				else
				{
					m_RecentMap.Add(recentUpdate.m_Key, new RecentValue
					{
						m_UpdateFrame = m_SimulationFrame,
						m_FeedbackDelta = recentUpdate.m_Delta
					});
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<BackSide> __Game_Buildings_BackSide_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CoverageData> __Game_Prefabs_CoverageData_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MailProducer> __Game_Buildings_MailProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> __Game_Buildings_CrimeProducer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> __Game_Prefabs_GarbageFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HospitalData> __Game_Prefabs_HospitalData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DeathcareFacilityData> __Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PowerPlantData> __Game_Prefabs_PowerPlantData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WindPoweredData> __Game_Prefabs_WindPoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SolarPoweredData> __Game_Prefabs_SolarPoweredData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Prefabs.TransformerData> __Game_Prefabs_TransformerData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPumpingStationData> __Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SewageOutletData> __Game_Prefabs_SewageOutletData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WastewaterTreatmentPlantData> __Game_Prefabs_WastewaterTreatmentPlantData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> __Game_Prefabs_TransportDepotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportStationData> __Game_Prefabs_TransportStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransportStationData> __Game_Prefabs_PublicTransportStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportStationData> __Game_Prefabs_CargoTransportStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportStopData> __Game_Prefabs_TransportStopData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PostFacilityData> __Game_Prefabs_PostFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TelecomFacilityData> __Game_Prefabs_TelecomFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SchoolData> __Game_Prefabs_SchoolData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingFacilityData> __Game_Prefabs_ParkingFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceDepotData> __Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FireStationData> __Game_Prefabs_FireStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PoliceStationData> __Game_Prefabs_PoliceStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrisonData> __Game_Prefabs_PrisonData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PollutionData> __Game_Prefabs_PollutionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AttractionData> __Game_Prefabs_AttractionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LocalModifierData> __Game_Prefabs_LocalModifierData_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifierData> __Game_Prefabs_CityModifierData_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_BackSide_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BackSide>(true);
			__Game_Prefabs_CoverageData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CoverageData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarbageProducer>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConsumer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterConsumer>(true);
			__Game_Buildings_MailProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MailProducer>(true);
			__Game_Buildings_CrimeProducer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CrimeProducer>(true);
			__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageFacilityData>(true);
			__Game_Prefabs_HospitalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HospitalData>(true);
			__Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeathcareFacilityData>(true);
			__Game_Prefabs_PowerPlantData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PowerPlantData>(true);
			__Game_Prefabs_WindPoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WindPoweredData>(true);
			__Game_Prefabs_SolarPoweredData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SolarPoweredData>(true);
			__Game_Prefabs_TransformerData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Prefabs.TransformerData>(true);
			__Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPumpingStationData>(true);
			__Game_Prefabs_SewageOutletData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SewageOutletData>(true);
			__Game_Prefabs_WastewaterTreatmentPlantData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WastewaterTreatmentPlantData>(true);
			__Game_Prefabs_TransportDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportDepotData>(true);
			__Game_Prefabs_TransportStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportStationData>(true);
			__Game_Prefabs_PublicTransportStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportStationData>(true);
			__Game_Prefabs_CargoTransportStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportStationData>(true);
			__Game_Prefabs_TransportStopData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportStopData>(true);
			__Game_Prefabs_PostFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PostFacilityData>(true);
			__Game_Prefabs_TelecomFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TelecomFacilityData>(true);
			__Game_Prefabs_SchoolData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SchoolData>(true);
			__Game_Prefabs_ParkingFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingFacilityData>(true);
			__Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceDepotData>(true);
			__Game_Prefabs_FireStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FireStationData>(true);
			__Game_Prefabs_PoliceStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceStationData>(true);
			__Game_Prefabs_PrisonData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrisonData>(true);
			__Game_Prefabs_PollutionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionData>(true);
			__Game_Prefabs_AttractionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AttractionData>(true);
			__Game_Net_ServiceCoverage_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(true);
			__Game_Prefabs_LocalModifierData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalModifierData>(true);
			__Game_Prefabs_CityModifierData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifierData>(true);
		}
	}

	private const float INFINITE_RANGE = 25000f;

	private IconCommandSystem m_IconCommandSystem;

	private PathfindQueueSystem m_PathfindQueueSystem;

	private AirwaySystem m_AirwaySystem;

	private SimulationSystem m_SimulationSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private List<Entity> m_FeedbackContainers;

	private List<Entity> m_PendingContainers;

	private NativeParallelHashMap<RecentKey, RecentValue> m_RecentMap;

	private PathfindTargetSeekerData m_TargetSeekerData;

	private EntityQuery m_ConfigurationQuery;

	private EntityQuery m_AppliedQuery;

	private EntityQuery m_TargetQuery;

	private EntityQuery m_EventQuery;

	private JobHandle m_RecentDeps;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_AirwaySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirwaySystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_FeedbackContainers = new List<Entity>();
		m_PendingContainers = new List<Entity>();
		m_RecentMap = new NativeParallelHashMap<RecentKey, RecentValue>(1000, AllocatorHandle.op_Implicit((Allocator)4));
		m_TargetSeekerData = new PathfindTargetSeekerData((SystemBase)(object)this);
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FeedbackConfigurationData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Applied>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Game.Objects.Object>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Game.Routes.TransportStop>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Game.Objects.Object>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Game.Routes.TransportStop>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Abandoned>(),
			ComponentType.ReadOnly<Condemned>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[1] = val;
		m_AppliedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_TargetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Game.Buildings.ServiceUpgrade>(),
			ComponentType.Exclude<Abandoned>(),
			ComponentType.Exclude<Condemned>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Updated>(),
			ComponentType.Exclude<Temp>()
		});
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<CoverageUpdated>()
		});
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_RecentDeps)).Complete();
		m_RecentMap.Dispose();
		base.OnDestroy();
	}

	public void PostDeserialize(Context context)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_RecentDeps)).Complete();
		m_RecentMap.Clear();
		for (int i = 0; i < m_PendingContainers.Count; i++)
		{
			Entity item = m_PendingContainers[i];
			ListExtensions.RemoveAtSwapBack<Entity>(m_PendingContainers, i--);
			m_FeedbackContainers.Add(item);
		}
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (!((EntityQuery)(ref m_AppliedQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_ConfigurationQuery)).IsEmptyIgnoreFilter)
		{
			ProcessModifications();
		}
		if (m_PendingContainers.Count != 0 && !((EntityQuery)(ref m_EventQuery)).IsEmptyIgnoreFilter)
		{
			UpdatePending();
		}
	}

	private void ProcessModifications()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_AppliedQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		PathfindParameters pathfindParameters = new PathfindParameters
		{
			m_MaxSpeed = float2.op_Implicit(111.111115f),
			m_WalkSpeed = float2.op_Implicit(5.555556f),
			m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
			m_PathfindFlags = (PathfindFlags.Stable | PathfindFlags.IgnoreFlow),
			m_IgnoredRules = (RuleFlags.HasBlockage | RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
		};
		SetupQueueTarget setupQueueTarget = default(SetupQueueTarget);
		EntityManager entityManager;
		while (m_FeedbackContainers.Count < val.Length)
		{
			List<Entity> list = m_FeedbackContainers;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			list.Add(((EntityManager)(ref entityManager)).CreateEntity((ComponentType[])(object)new ComponentType[3]
			{
				ComponentType.ReadWrite<Feedback>(),
				ComponentType.ReadWrite<ExtraFeedback>(),
				ComponentType.ReadWrite<CoverageElement>()
			}));
		}
		m_TargetSeekerData.Update((SystemBase)(object)this, m_AirwaySystem.GetAirwayData());
		SetupCoverageSearchJob setupCoverageSearchJob = new SetupCoverageSearchJob
		{
			m_BackSideData = InternalCompilerInterface.GetComponentLookup<BackSide>(ref __TypeHandle.__Game_Buildings_BackSide_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCoverageData = InternalCompilerInterface.GetComponentLookup<CoverageData>(ref __TypeHandle.__Game_Prefabs_CoverageData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		FeedbackConfigurationData feedbackConfigurationData = default(FeedbackConfigurationData);
		IconCommandBuffer iconCommandBuffer = default(IconCommandBuffer);
		JobHandle val2 = default(JobHandle);
		bool flag = false;
		Owner owner = default(Owner);
		DynamicBuffer<InstalledUpgrade> val6 = default(DynamicBuffer<InstalledUpgrade>);
		CoverageServiceType coverageServiceType = default(CoverageServiceType);
		for (int i = 0; i < val.Length; i++)
		{
			Entity val3 = val[i];
			Entity val4 = val3;
			while (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val4, ref owner))
			{
				val4 = owner.m_Owner;
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			bool flag2 = ((EntityManager)(ref entityManager)).HasComponent<Deleted>(val3);
			if (val4 != val3)
			{
				if (flag2)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Deleted>(val4))
					{
						continue;
					}
				}
				else
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Applied>(val4))
					{
						continue;
					}
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Transform componentData = ((EntityManager)(ref entityManager)).GetComponentData<Transform>(val4);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			PrefabRef componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val3);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			PrefabRef componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val4);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(componentData2.m_Prefab))
			{
				if (flag2)
				{
					if (feedbackConfigurationData.m_HappyFaceNotification == Entity.Null)
					{
						feedbackConfigurationData = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingleton<FeedbackConfigurationData>();
						iconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
					}
					iconCommandBuffer.Add(val3, feedbackConfigurationData.m_SadFaceNotification, IconPriority.Info, IconClusterLayer.Transaction);
				}
				continue;
			}
			Entity val5 = m_FeedbackContainers[m_FeedbackContainers.Count - 1];
			m_FeedbackContainers.RemoveAt(m_FeedbackContainers.Count - 1);
			m_PendingContainers.Add(val5);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<Feedback>(val5, new Feedback
			{
				m_Position = componentData.m_Position,
				m_MainEntity = val4,
				m_Prefab = componentData2.m_Prefab,
				m_MainPrefab = componentData3.m_Prefab,
				m_IsDeleted = flag2
			});
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<ExtraFeedback> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ExtraFeedback>(val5, false);
			buffer.Clear();
			if (EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, val4, true, ref val6))
			{
				for (int j = 0; j < val6.Length; j++)
				{
					InstalledUpgrade installedUpgrade = val6[j];
					if (!BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive))
					{
						ExtraFeedback extraFeedback = default(ExtraFeedback);
						entityManager = ((ComponentSystemBase)this).EntityManager;
						extraFeedback.m_Prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(installedUpgrade.m_Upgrade).m_Prefab;
						buffer.Add(extraFeedback);
					}
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).GetBuffer<CoverageElement>(val5, false).Clear();
			if (!EntitiesExtensions.TryGetSharedComponent<CoverageServiceType>(((ComponentSystemBase)this).EntityManager, val4, ref coverageServiceType))
			{
				coverageServiceType.m_Service = CoverageService.Count;
			}
			Game.Simulation.ServiceCoverageSystem.SetupPathfindMethods(coverageServiceType.m_Service, ref pathfindParameters, ref setupQueueTarget);
			CoverageAction action = new CoverageAction((Allocator)4);
			setupCoverageSearchJob.m_Entity = val4;
			setupCoverageSearchJob.m_TargetSeeker = new PathfindTargetSeeker<PathfindTargetBuffer>(m_TargetSeekerData, pathfindParameters, setupQueueTarget, action.data.m_Sources.AsParallelWriter(), RandomSeed.Next(), isStartTarget: true);
			setupCoverageSearchJob.m_Action = action;
			JobHandle val7 = IJobExtensions.Schedule<SetupCoverageSearchJob>(setupCoverageSearchJob, ((SystemBase)this).Dependency);
			val2 = JobHandle.CombineDependencies(val2, val7);
			m_PathfindQueueSystem.Enqueue(action, val5, val7, uint.MaxValue, this, default(PathEventData), highPriority: true);
			flag = true;
		}
		val.Dispose();
		if (flag)
		{
			((SystemBase)this).Dependency = val2;
		}
	}

	private void UpdatePending()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<CoverageUpdated> val = ((EntityQuery)(ref m_EventQuery)).ToComponentDataArray<CoverageUpdated>(AllocatorHandle.op_Implicit((Allocator)3));
		Entity singletonEntity = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingletonEntity();
		TargetCheckJob targetCheckJob = new TargetCheckJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducerType = InternalCompilerInterface.GetComponentTypeHandle<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumerType = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumerType = InternalCompilerInterface.GetComponentTypeHandle<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducerType = InternalCompilerInterface.GetComponentTypeHandle<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeProducerType = InternalCompilerInterface.GetComponentTypeHandle<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCoverageData = InternalCompilerInterface.GetComponentLookup<CoverageData>(ref __TypeHandle.__Game_Prefabs_CoverageData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGarbageFacilityData = InternalCompilerInterface.GetComponentLookup<GarbageFacilityData>(ref __TypeHandle.__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHospitalData = InternalCompilerInterface.GetComponentLookup<HospitalData>(ref __TypeHandle.__Game_Prefabs_HospitalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDeathcareFacilityData = InternalCompilerInterface.GetComponentLookup<DeathcareFacilityData>(ref __TypeHandle.__Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPowerPlantData = InternalCompilerInterface.GetComponentLookup<PowerPlantData>(ref __TypeHandle.__Game_Prefabs_PowerPlantData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWindPoweredData = InternalCompilerInterface.GetComponentLookup<WindPoweredData>(ref __TypeHandle.__Game_Prefabs_WindPoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSolarPoweredData = InternalCompilerInterface.GetComponentLookup<SolarPoweredData>(ref __TypeHandle.__Game_Prefabs_SolarPoweredData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransformerData = InternalCompilerInterface.GetComponentLookup<Game.Prefabs.TransformerData>(ref __TypeHandle.__Game_Prefabs_TransformerData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWaterPumpingStationData = InternalCompilerInterface.GetComponentLookup<WaterPumpingStationData>(ref __TypeHandle.__Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSewageOutletData = InternalCompilerInterface.GetComponentLookup<SewageOutletData>(ref __TypeHandle.__Game_Prefabs_SewageOutletData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWastewaterTreatmentPlantData = InternalCompilerInterface.GetComponentLookup<WastewaterTreatmentPlantData>(ref __TypeHandle.__Game_Prefabs_WastewaterTreatmentPlantData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportDepotData = InternalCompilerInterface.GetComponentLookup<TransportDepotData>(ref __TypeHandle.__Game_Prefabs_TransportDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportStationData = InternalCompilerInterface.GetComponentLookup<TransportStationData>(ref __TypeHandle.__Game_Prefabs_TransportStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPublicTransportStationData = InternalCompilerInterface.GetComponentLookup<PublicTransportStationData>(ref __TypeHandle.__Game_Prefabs_PublicTransportStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCargoTransportStationData = InternalCompilerInterface.GetComponentLookup<CargoTransportStationData>(ref __TypeHandle.__Game_Prefabs_CargoTransportStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportStopData = InternalCompilerInterface.GetComponentLookup<TransportStopData>(ref __TypeHandle.__Game_Prefabs_TransportStopData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPostFacilityData = InternalCompilerInterface.GetComponentLookup<PostFacilityData>(ref __TypeHandle.__Game_Prefabs_PostFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTelecomFacilityData = InternalCompilerInterface.GetComponentLookup<TelecomFacilityData>(ref __TypeHandle.__Game_Prefabs_TelecomFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSchoolData = InternalCompilerInterface.GetComponentLookup<SchoolData>(ref __TypeHandle.__Game_Prefabs_SchoolData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingFacilityData = InternalCompilerInterface.GetComponentLookup<ParkingFacilityData>(ref __TypeHandle.__Game_Prefabs_ParkingFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMaintenanceDepotData = InternalCompilerInterface.GetComponentLookup<MaintenanceDepotData>(ref __TypeHandle.__Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabFireStationData = InternalCompilerInterface.GetComponentLookup<FireStationData>(ref __TypeHandle.__Game_Prefabs_FireStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPoliceStationData = InternalCompilerInterface.GetComponentLookup<PoliceStationData>(ref __TypeHandle.__Game_Prefabs_PoliceStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPrisonData = InternalCompilerInterface.GetComponentLookup<PrisonData>(ref __TypeHandle.__Game_Prefabs_PrisonData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPollutionData = InternalCompilerInterface.GetComponentLookup<PollutionData>(ref __TypeHandle.__Game_Prefabs_PollutionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAttractionData = InternalCompilerInterface.GetComponentLookup<AttractionData>(ref __TypeHandle.__Game_Prefabs_AttractionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverages = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLocalModifierDatas = InternalCompilerInterface.GetBufferLookup<LocalModifierData>(ref __TypeHandle.__Game_Prefabs_LocalModifierData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCityModifierDatas = InternalCompilerInterface.GetBufferLookup<CityModifierData>(ref __TypeHandle.__Game_Prefabs_CityModifierData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		targetCheckJob.m_FeedbackConfigurationData = ((EntityManager)(ref entityManager)).GetComponentData<FeedbackConfigurationData>(singletonEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		targetCheckJob.m_FeedbackLocalEffectFactors = ((EntityManager)(ref entityManager)).GetBuffer<FeedbackLocalEffectFactor>(singletonEntity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		targetCheckJob.m_FeedbackCityEffectFactors = ((EntityManager)(ref entityManager)).GetBuffer<FeedbackCityEffectFactor>(singletonEntity, true);
		targetCheckJob.m_RecentMap = m_RecentMap;
		TargetCheckJob targetCheckJob2 = targetCheckJob;
		NativeQueue<RecentUpdate> recentUpdates = default(NativeQueue<RecentUpdate>);
		JobHandle val2 = default(JobHandle);
		bool flag = false;
		NativeParallelHashMap<Entity, float2> coverageMap = default(NativeParallelHashMap<Entity, float2>);
		for (int i = 0; i < m_PendingContainers.Count; i++)
		{
			Entity val3 = m_PendingContainers[i];
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<CoverageElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CoverageElement>(val3, true);
			if (buffer.Length != 0)
			{
				ListExtensions.RemoveAtSwapBack<Entity>(m_PendingContainers, i--);
				m_FeedbackContainers.Add(val3);
				coverageMap._002Ector(buffer.Length, AllocatorHandle.op_Implicit((Allocator)3));
				FillCoverageMapJob obj = new FillCoverageMapJob
				{
					m_CoverageElements = buffer.AsNativeArray(),
					m_CoverageMap = coverageMap.AsParallelWriter()
				};
				entityManager = ((ComponentSystemBase)this).EntityManager;
				targetCheckJob2.m_FeedbackData = ((EntityManager)(ref entityManager)).GetComponentData<Feedback>(val3);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				targetCheckJob2.m_ExtraFeedbacks = ((EntityManager)(ref entityManager)).GetBuffer<ExtraFeedback>(val3, true);
				targetCheckJob2.m_RandomSeed = RandomSeed.Next();
				targetCheckJob2.m_CoverageMap = coverageMap;
				if (!flag)
				{
					recentUpdates._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
					targetCheckJob2.m_TelecomCoverageData = m_TelecomCoverageSystem.GetData(readOnly: true, out var dependencies);
					targetCheckJob2.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
					targetCheckJob2.m_RecentUpdates = recentUpdates.AsParallelWriter();
					val2 = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_RecentDeps, dependencies);
				}
				JobHandle val4 = IJobParallelForExtensions.Schedule<FillCoverageMapJob>(obj, buffer.Length, 4, default(JobHandle));
				JobHandle val5 = JobChunkExtensions.ScheduleParallel<TargetCheckJob>(targetCheckJob2, m_TargetQuery, JobHandle.CombineDependencies(val2, val4));
				coverageMap.Dispose(val5);
				val2 = val5;
				flag = true;
			}
		}
		for (int j = 0; j < val.Length; j++)
		{
			CoverageUpdated coverageUpdated = val[j];
			for (int k = 0; k < m_PendingContainers.Count; k++)
			{
				Entity val6 = m_PendingContainers[k];
				if (val6 == coverageUpdated.m_Owner)
				{
					ListExtensions.RemoveAtSwapBack<Entity>(m_PendingContainers, k--);
					m_FeedbackContainers.Add(val6);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					targetCheckJob2.m_FeedbackData = ((EntityManager)(ref entityManager)).GetComponentData<Feedback>(val6);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					targetCheckJob2.m_ExtraFeedbacks = ((EntityManager)(ref entityManager)).GetBuffer<ExtraFeedback>(val6, true);
					targetCheckJob2.m_RandomSeed = RandomSeed.Next();
					targetCheckJob2.m_CoverageMap = default(NativeParallelHashMap<Entity, float2>);
					if (!flag)
					{
						recentUpdates._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
						targetCheckJob2.m_TelecomCoverageData = m_TelecomCoverageSystem.GetData(readOnly: true, out var dependencies2);
						targetCheckJob2.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
						targetCheckJob2.m_RecentUpdates = recentUpdates.AsParallelWriter();
						val2 = JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_RecentDeps, dependencies2);
					}
					val2 = JobChunkExtensions.ScheduleParallel<TargetCheckJob>(targetCheckJob2, m_TargetQuery, val2);
					flag = true;
					break;
				}
			}
		}
		val.Dispose();
		if (flag)
		{
			m_IconCommandSystem.AddCommandBufferWriter(val2);
			m_TelecomCoverageSystem.AddReader(val2);
			((SystemBase)this).Dependency = val2;
			UpdateRecentMapJob updateRecentMapJob = new UpdateRecentMapJob
			{
				m_RecentMap = m_RecentMap,
				m_RecentUpdates = recentUpdates,
				m_SimulationFrame = m_SimulationSystem.frameIndex
			};
			m_RecentDeps = IJobExtensions.Schedule<UpdateRecentMapJob>(updateRecentMapJob, val2);
			recentUpdates.Dispose(m_RecentDeps);
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
	public ToolFeedbackSystem()
	{
	}
}
