using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Debug;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Game.Vehicles;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class HouseholdFindPropertySystem : GameSystemBase
{
	public struct CachedPropertyInformation
	{
		public GenericApartmentQuality quality;

		public int free;
	}

	public struct GenericApartmentQuality
	{
		public float apartmentSize;

		public float2 educationBonus;

		public float welfareBonus;

		public float score;

		public int level;
	}

	[BurstCompile]
	private struct PreparePropertyJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingProperties;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_Abandoneds;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_Parks;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<ParkData> m_ParkDatas;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyData;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverages;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> m_Crimes;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<Locked> m_Locked;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumers;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumers;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducers;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducers;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoiseMap;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverages;

		public HealthcareParameterData m_HealthcareParameters;

		public ParkParameterData m_ParkParameters;

		public EducationParameterData m_EducationParameters;

		public TelecomParameterData m_TelecomParameters;

		public GarbageParameterData m_GarbageParameters;

		public PoliceConfigurationData m_PoliceParameters;

		public CitizenHappinessParameterData m_CitizenHappinessParameterData;

		public Entity m_City;

		public ParallelWriter<Entity, CachedPropertyInformation> m_PropertyData;

		private int CalculateFree(Entity property)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			Entity prefab = m_Prefabs[property].m_Prefab;
			int num = 0;
			if (m_BuildingDatas.HasComponent(prefab) && (m_Abandoneds.HasComponent(property) || (m_Parks.HasComponent(property) && m_ParkDatas[prefab].m_AllowHomeless)))
			{
				num = BuildingUtils.GetShelterHomelessCapacity(prefab, ref m_BuildingDatas, ref m_BuildingPropertyData) - m_Renters[property].Length;
			}
			else if (m_BuildingProperties.HasComponent(prefab))
			{
				BuildingPropertyData buildingPropertyData = m_BuildingProperties[prefab];
				DynamicBuffer<Renter> val = m_Renters[property];
				num = buildingPropertyData.CountProperties(AreaType.Residential);
				for (int i = 0; i < val.Length; i++)
				{
					Entity renter = val[i].m_Renter;
					if (m_Households.HasComponent(renter))
					{
						num--;
					}
				}
			}
			return num;
		}

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				int num = CalculateFree(val);
				if (num > 0)
				{
					Entity prefab = m_Prefabs[val].m_Prefab;
					Building buildingData = m_Buildings[val];
					Entity healthcareServicePrefab = m_HealthcareParameters.m_HealthcareServicePrefab;
					Entity parkServicePrefab = m_ParkParameters.m_ParkServicePrefab;
					Entity educationServicePrefab = m_EducationParameters.m_EducationServicePrefab;
					Entity telecomServicePrefab = m_TelecomParameters.m_TelecomServicePrefab;
					Entity garbageServicePrefab = m_GarbageParameters.m_GarbageServicePrefab;
					Entity policeServicePrefab = m_PoliceParameters.m_PoliceServicePrefab;
					DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
					GenericApartmentQuality genericApartmentQuality = PropertyUtils.GetGenericApartmentQuality(val, prefab, ref buildingData, ref m_BuildingProperties, ref m_BuildingDatas, ref m_SpawnableDatas, ref m_Crimes, ref m_ServiceCoverages, ref m_Locked, ref m_ElectricityConsumers, ref m_WaterConsumers, ref m_GarbageProducers, ref m_MailProducers, ref m_Transforms, ref m_Abandoneds, m_PollutionMap, m_AirPollutionMap, m_NoiseMap, m_TelecomCoverages, cityModifiers, healthcareServicePrefab, parkServicePrefab, educationServicePrefab, telecomServicePrefab, garbageServicePrefab, policeServicePrefab, m_CitizenHappinessParameterData, m_GarbageParameters);
					m_PropertyData.TryAdd(val, new CachedPropertyInformation
					{
						free = num,
						quality = genericApartmentQuality
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FindPropertyJob : IJob
	{
		public NativeList<Entity> m_HomelessHouseholdEntities;

		public NativeList<Entity> m_MovedInHouseholdEntities;

		public NativeParallelHashMap<Entity, CachedPropertyInformation> m_CachedPropertyInfo;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<PropertyOnMarket> m_PropertiesOnMarket;

		[ReadOnly]
		public BufferLookup<PathInformations> m_PathInformationBuffers;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> m_Students;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingProperties;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_Availabilities;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverages;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHouseholds;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> m_Crimes;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<Locked> m_Lockeds;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblems;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_Parks;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_Abandoneds;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumers;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumers;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducers;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducers;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> m_CurrentBuildings;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransports;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformations;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_CitizenBuffers;

		public ComponentLookup<PropertySeeker> m_PropertySeekers;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoiseMap;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverages;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		[ReadOnly]
		public CountResidentialPropertySystem.ResidentialPropertyData m_ResidentialPropertyData;

		[ReadOnly]
		public HealthcareParameterData m_HealthcareParameters;

		[ReadOnly]
		public ParkParameterData m_ParkParameters;

		[ReadOnly]
		public EducationParameterData m_EducationParameters;

		[ReadOnly]
		public TelecomParameterData m_TelecomParameters;

		[ReadOnly]
		public GarbageParameterData m_GarbageParameters;

		[ReadOnly]
		public PoliceConfigurationData m_PoliceParameters;

		[ReadOnly]
		public CitizenHappinessParameterData m_CitizenHappinessParameterData;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameters;

		[ReadOnly]
		public uint m_SimulationFrame;

		public EntityCommandBuffer m_CommandBuffer;

		[ReadOnly]
		public Entity m_City;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<RentAction> m_RentActionQueue;

		private void StartHomeFinding(Entity household, Entity commuteCitizen, Entity targetLocation, Entity oldHome, float minimumScore, bool targetIsOrigin, DynamicBuffer<HouseholdCitizen> citizens)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathInformation>(household, new PathInformation
			{
				m_State = PathFlags.Pending
			});
			Household household2 = m_Households[household];
			PathfindWeights weights = default(PathfindWeights);
			Citizen citizen = default(Citizen);
			if (m_Citizens.TryGetComponent(commuteCitizen, ref citizen))
			{
				weights = CitizenUtils.GetPathfindWeights(citizen, household2, citizens.Length);
			}
			else
			{
				for (int i = 0; i < citizens.Length; i++)
				{
					ref float4 value = ref weights.m_Value;
					value += CitizenUtils.GetPathfindWeights(citizen, household2, citizens.Length).m_Value;
				}
				ref float4 value2 = ref weights.m_Value;
				value2 *= 1f / (float)citizens.Length;
			}
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(111.111115f),
				m_WalkSpeed = float2.op_Implicit(1.6666667f),
				m_Weights = weights,
				m_Methods = (PathMethod.Pedestrian | PathMethod.PublicTransportDay),
				m_MaxCost = CitizenBehaviorSystem.kMaxPathfindCost,
				m_PathfindFlags = (PathfindFlags.Simplified | PathfindFlags.IgnorePath)
			};
			SetupQueueTarget a = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = PathMethod.Pedestrian,
				m_Entity = targetLocation
			};
			SetupQueueTarget b = new SetupQueueTarget
			{
				m_Type = SetupTargetType.FindHome,
				m_Methods = PathMethod.Pedestrian,
				m_Entity = household,
				m_Entity2 = oldHome,
				m_Value2 = minimumScore
			};
			DynamicBuffer<OwnedVehicle> val = default(DynamicBuffer<OwnedVehicle>);
			if (m_OwnedVehicles.TryGetBuffer(household, ref val) && val.Length != 0)
			{
				parameters.m_Methods |= (PathMethod)(targetIsOrigin ? 8194 : 8198);
				parameters.m_ParkingSize = float2.op_Implicit(float.MinValue);
				parameters.m_IgnoredRules |= RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidSlowTraffic;
				a.m_Methods |= PathMethod.Road | PathMethod.MediumRoad;
				a.m_RoadTypes |= RoadTypes.Car;
				b.m_Methods |= PathMethod.Road | PathMethod.MediumRoad;
				b.m_RoadTypes |= RoadTypes.Car;
			}
			if (targetIsOrigin)
			{
				parameters.m_MaxSpeed.y = 277.77777f;
				parameters.m_Methods |= PathMethod.Taxi | PathMethod.PublicTransportNight;
				parameters.m_SecondaryIgnoredRules = VehicleUtils.GetIgnoredPathfindRulesTaxiDefaults();
			}
			else
			{
				CommonUtils.Swap(ref a, ref b);
			}
			parameters.m_MaxResultCount = 10;
			parameters.m_PathfindFlags |= (PathfindFlags)(targetIsOrigin ? 256 : 128);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<PathInformations>(household).Add(new PathInformations
			{
				m_State = PathFlags.Pending
			});
			SetupQueueItem setupQueueItem = new SetupQueueItem(household, parameters, a, b);
			m_PathfindQueue.Enqueue(setupQueueItem);
		}

		private Entity GetFirstWorkplaceOrSchool(DynamicBuffer<HouseholdCitizen> citizens, ref Entity citizen)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < citizens.Length; i++)
			{
				citizen = citizens[i].m_Citizen;
				if (m_Workers.HasComponent(citizen))
				{
					return m_Workers[citizen].m_Workplace;
				}
				if (m_Students.HasComponent(citizen))
				{
					return m_Students[citizen].m_School;
				}
			}
			return Entity.Null;
		}

		private Entity GetCurrentLocation(DynamicBuffer<HouseholdCitizen> citizens)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			CurrentTransport currentTransport = default(CurrentTransport);
			for (int i = 0; i < citizens.Length; i++)
			{
				if (m_CurrentBuildings.TryGetComponent(citizens[i].m_Citizen, ref currentBuilding))
				{
					return currentBuilding.m_CurrentBuilding;
				}
				if (m_CurrentTransports.TryGetComponent(citizens[i].m_Citizen, ref currentTransport))
				{
					return currentTransport.m_CurrentTransport;
				}
			}
			return Entity.Null;
		}

		public void Execute()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_HomelessHouseholdEntities.Length; i++)
			{
				Entity householdEntity = m_HomelessHouseholdEntities[i];
				if (ProcessFindHome(householdEntity))
				{
					num++;
				}
				if (num >= kMaxProcessEntitiesPerUpdate / 2)
				{
					break;
				}
			}
			for (int j = 0; j < m_MovedInHouseholdEntities.Length; j++)
			{
				Entity householdEntity2 = m_MovedInHouseholdEntities[j];
				if (ProcessFindHome(householdEntity2))
				{
					num++;
				}
				if (num >= kMaxProcessEntitiesPerUpdate)
				{
					break;
				}
			}
		}

		private bool ProcessFindHome(Entity householdEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<HouseholdCitizen> val = m_CitizenBuffers[householdEntity];
			if (val.Length == 0)
			{
				return false;
			}
			PropertySeeker propertySeeker = m_PropertySeekers[householdEntity];
			int householdIncome = EconomyUtils.GetHouseholdIncome(val, ref m_Workers, ref m_Citizens, ref m_HealthProblems, ref m_EconomyParameters, m_TaxRates);
			DynamicBuffer<PathInformations> pathInformations = default(DynamicBuffer<PathInformations>);
			if (m_PathInformationBuffers.TryGetBuffer(householdEntity, ref pathInformations))
			{
				ProcessPathInformations(householdEntity, pathInformations, propertySeeker, val, householdIncome);
				return false;
			}
			if (propertySeeker.m_LastPropertySeekFrame + kFindPropertyCoolDown > m_SimulationFrame)
			{
				if (m_PathInformations[householdEntity].m_State != PathFlags.Pending && math.csum(m_ResidentialPropertyData.m_FreeProperties) < 10)
				{
					CitizenUtils.HouseholdMoveAway(m_CommandBuffer, householdEntity);
				}
				return false;
			}
			Entity householdHomeBuilding = BuildingUtils.GetHouseholdHomeBuilding(householdEntity, ref m_PropertyRenters, ref m_HomelessHouseholds);
			float bestPropertyScore = ((householdHomeBuilding != Entity.Null) ? PropertyUtils.GetPropertyScore(householdHomeBuilding, householdEntity, val, ref m_PrefabRefs, ref m_BuildingProperties, ref m_Buildings, ref m_BuildingDatas, ref m_Households, ref m_Citizens, ref m_Students, ref m_Workers, ref m_SpawnableDatas, ref m_Crimes, ref m_ServiceCoverages, ref m_Lockeds, ref m_ElectricityConsumers, ref m_WaterConsumers, ref m_GarbageProducers, ref m_MailProducers, ref m_Transforms, ref m_Abandoneds, ref m_Parks, ref m_Availabilities, m_TaxRates, m_PollutionMap, m_AirPollutionMap, m_NoiseMap, m_TelecomCoverages, m_CityModifiers[m_City], m_HealthcareParameters.m_HealthcareServicePrefab, m_ParkParameters.m_ParkServicePrefab, m_EducationParameters.m_EducationServicePrefab, m_TelecomParameters.m_TelecomServicePrefab, m_GarbageParameters.m_GarbageServicePrefab, m_PoliceParameters.m_PoliceServicePrefab, m_CitizenHappinessParameterData, m_GarbageParameters) : float.NegativeInfinity);
			Entity citizen = Entity.Null;
			Entity firstWorkplaceOrSchool = GetFirstWorkplaceOrSchool(val, ref citizen);
			bool flag = firstWorkplaceOrSchool == Entity.Null;
			Entity val2 = (flag ? GetCurrentLocation(val) : firstWorkplaceOrSchool);
			if (val2 == Entity.Null)
			{
				CitizenUtils.HouseholdMoveAway(m_CommandBuffer, householdEntity);
				return false;
			}
			propertySeeker.m_TargetProperty = firstWorkplaceOrSchool;
			propertySeeker.m_BestProperty = householdHomeBuilding;
			propertySeeker.m_BestPropertyScore = bestPropertyScore;
			propertySeeker.m_LastPropertySeekFrame = m_SimulationFrame;
			m_PropertySeekers[householdEntity] = propertySeeker;
			StartHomeFinding(householdEntity, citizen, val2, householdHomeBuilding, propertySeeker.m_BestPropertyScore, flag, val);
			return true;
		}

		private void ProcessPathInformations(Entity householdEntity, DynamicBuffer<PathInformations> pathInformations, PropertySeeker propertySeeker, DynamicBuffer<HouseholdCitizen> citizens, int income)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			PathInformations pathInformations2 = pathInformations[num];
			if ((pathInformations2.m_State & PathFlags.Pending) != 0)
			{
				return;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<PathInformations>(householdEntity);
			bool flag = propertySeeker.m_TargetProperty != Entity.Null;
			Entity val = (flag ? pathInformations2.m_Origin : pathInformations2.m_Destination);
			bool flag2 = false;
			while (!m_CachedPropertyInfo.ContainsKey(val) || m_CachedPropertyInfo[val].free <= 0)
			{
				num++;
				if (pathInformations.Length > num)
				{
					pathInformations2 = pathInformations[num];
					val = (flag ? pathInformations2.m_Origin : pathInformations2.m_Destination);
					continue;
				}
				val = Entity.Null;
				flag2 = true;
				break;
			}
			if (flag2 && pathInformations.Length != 0 && pathInformations[0].m_Destination != Entity.Null)
			{
				return;
			}
			float num2 = float.NegativeInfinity;
			if (val != Entity.Null && m_CachedPropertyInfo.ContainsKey(val) && m_CachedPropertyInfo[val].free > 0)
			{
				num2 = PropertyUtils.GetPropertyScore(val, householdEntity, citizens, ref m_PrefabRefs, ref m_BuildingProperties, ref m_Buildings, ref m_BuildingDatas, ref m_Households, ref m_Citizens, ref m_Students, ref m_Workers, ref m_SpawnableDatas, ref m_Crimes, ref m_ServiceCoverages, ref m_Lockeds, ref m_ElectricityConsumers, ref m_WaterConsumers, ref m_GarbageProducers, ref m_MailProducers, ref m_Transforms, ref m_Abandoneds, ref m_Parks, ref m_Availabilities, m_TaxRates, m_PollutionMap, m_AirPollutionMap, m_NoiseMap, m_TelecomCoverages, m_CityModifiers[m_City], m_HealthcareParameters.m_HealthcareServicePrefab, m_ParkParameters.m_ParkServicePrefab, m_EducationParameters.m_EducationServicePrefab, m_TelecomParameters.m_TelecomServicePrefab, m_GarbageParameters.m_GarbageServicePrefab, m_PoliceParameters.m_PoliceServicePrefab, m_CitizenHappinessParameterData, m_GarbageParameters);
			}
			if (num2 < propertySeeker.m_BestPropertyScore)
			{
				val = propertySeeker.m_BestProperty;
			}
			bool flag3 = (m_Households[householdEntity].m_Flags & HouseholdFlags.MovedIn) != 0;
			bool flag4 = val != Entity.Null && BuildingUtils.IsHomelessShelterBuilding(val, ref m_Parks, ref m_Abandoneds);
			bool flag5 = CitizenUtils.IsHouseholdNeedSupport(citizens, ref m_Citizens, ref m_Students);
			bool flag6 = m_PropertiesOnMarket.HasComponent(val) && (flag5 || m_PropertiesOnMarket[val].m_AskingRent < income);
			int num3;
			if (m_PropertyRenters.HasComponent(householdEntity))
			{
				PropertyRenter propertyRenter = m_PropertyRenters[householdEntity];
				num3 = ((!((Entity)(ref propertyRenter.m_Property)).Equals(val)) ? 1 : 0);
			}
			else
			{
				num3 = 1;
			}
			bool flag7 = (byte)num3 != 0;
			Entity householdHomeBuilding = BuildingUtils.GetHouseholdHomeBuilding(householdEntity, ref m_PropertyRenters, ref m_HomelessHouseholds);
			if (householdHomeBuilding != Entity.Null && householdHomeBuilding == val)
			{
				if (!m_HomelessHouseholds.HasComponent(householdEntity) && !flag5 && income < m_PropertyRenters[householdEntity].m_Rent)
				{
					CitizenUtils.HouseholdMoveAway(m_CommandBuffer, householdEntity);
				}
				else
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(householdEntity, false);
				}
			}
			else if ((flag6 && flag7) || (flag3 && flag4))
			{
				m_RentActionQueue.Enqueue(new RentAction
				{
					m_Property = val,
					m_Renter = householdEntity
				});
				if (m_CachedPropertyInfo.ContainsKey(val))
				{
					CachedPropertyInformation cachedPropertyInformation = m_CachedPropertyInfo[val];
					cachedPropertyInformation.free--;
					m_CachedPropertyInfo[val] = cachedPropertyInformation;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(householdEntity, false);
			}
			else if (val == Entity.Null && (!m_HomelessHouseholds.HasComponent(householdEntity) || m_HomelessHouseholds[householdEntity].m_TempHome == Entity.Null))
			{
				CitizenUtils.HouseholdMoveAway(m_CommandBuffer, householdEntity);
			}
			else
			{
				propertySeeker.m_BestProperty = default(Entity);
				propertySeeker.m_BestPropertyScore = float.NegativeInfinity;
				m_PropertySeekers[householdEntity] = propertySeeker;
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkData> __Game_Prefabs_ParkData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> __Game_Buildings_CrimeProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailProducer> __Game_Buildings_MailProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyOnMarket> __Game_Buildings_PropertyOnMarket_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PathInformations> __Game_Pathfind_PathInformations_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		public ComponentLookup<PropertySeeker> __Game_Agents_PropertySeeker_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_BuildingPropertyData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_ParkData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkData>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Buildings_Park_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Park>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Net_ServiceCoverage_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(true);
			__Game_Buildings_CrimeProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeProducer>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(true);
			__Game_Buildings_MailProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailProducer>(true);
			__Game_Buildings_PropertyOnMarket_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyOnMarket>(true);
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
			__Game_Pathfind_PathInformations_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathInformations>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentBuilding>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Agents_PropertySeeker_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertySeeker>(false);
		}
	}

	public bool debugDisableHomeless;

	private const int UPDATE_INTERVAL = 16;

	public static readonly int kMaxProcessEntitiesPerUpdate = 128;

	public static readonly int kFindPropertyCoolDown = 5000;

	[DebugWatchValue]
	private DebugWatchDistribution m_DefaultDistribution;

	[DebugWatchValue]
	private DebugWatchDistribution m_EvaluateDistributionLow;

	[DebugWatchValue]
	private DebugWatchDistribution m_EvaluateDistributionMedium;

	[DebugWatchValue]
	private DebugWatchDistribution m_EvaluateDistributionHigh;

	[DebugWatchValue]
	private DebugWatchDistribution m_EvaluateDistributionLowrent;

	private EntityQuery m_HouseholdQuery;

	private EntityQuery m_HomelessHouseholdQuery;

	private EntityQuery m_FreePropertyQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_DemandParameterQuery;

	private EndFrameBarrier m_EndFrameBarrier;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private TaxSystem m_TaxSystem;

	private TriggerSystem m_TriggerSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private CitySystem m_CitySystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private SimulationSystem m_SimulationSystem;

	private PropertyProcessingSystem m_PropertyProcessingSystem;

	private CountResidentialPropertySystem m_CountResidentialPropertySystem;

	private EntityQuery m_HealthcareParameterQuery;

	private EntityQuery m_ParkParameterQuery;

	private EntityQuery m_EducationParameterQuery;

	private EntityQuery m_TelecomParameterQuery;

	private EntityQuery m_GarbageParameterQuery;

	private EntityQuery m_PoliceParameterQuery;

	private EntityQuery m_CitizenHappinessParameterQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Expected O, but got Unknown
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PropertyProcessingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PropertyProcessingSystem>();
		m_CountResidentialPropertySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountResidentialPropertySystem>();
		m_HomelessHouseholdQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[9]
		{
			ComponentType.ReadWrite<HomelessHousehold>(),
			ComponentType.ReadWrite<PropertySeeker>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.Exclude<MovingAway>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<CommuterHousehold>(),
			ComponentType.Exclude<CurrentBuilding>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_HouseholdQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadWrite<Household>(),
			ComponentType.ReadWrite<PropertySeeker>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.Exclude<HomelessHousehold>(),
			ComponentType.Exclude<MovingAway>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<CommuterHousehold>(),
			ComponentType.Exclude<CurrentBuilding>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		m_HealthcareParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_ParkParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ParkParameterData>() });
		m_EducationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EducationParameterData>() });
		m_TelecomParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TelecomParameterData>() });
		m_GarbageParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		m_PoliceParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PoliceConfigurationData>() });
		m_CitizenHappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Building>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Abandoned>(),
			ComponentType.ReadOnly<Game.Buildings.Park>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		EntityQueryDesc val2 = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PropertyOnMarket>(),
			ComponentType.ReadOnly<ResidentialProperty>(),
			ComponentType.ReadOnly<Building>()
		};
		val.None = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Abandoned>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Condemned>()
		};
		EntityQueryDesc val3 = val;
		m_FreePropertyQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[2] { val2, val3 });
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_HealthcareParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_ParkParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EducationParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_TelecomParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_HouseholdQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_DemandParameterQuery);
		m_DefaultDistribution = new DebugWatchDistribution(persistent: true, relative: true);
		m_EvaluateDistributionLow = new DebugWatchDistribution(persistent: true, relative: true);
		m_EvaluateDistributionMedium = new DebugWatchDistribution(persistent: true, relative: true);
		m_EvaluateDistributionHigh = new DebugWatchDistribution(persistent: true, relative: true);
		m_EvaluateDistributionLowrent = new DebugWatchDistribution(persistent: true, relative: true);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_DefaultDistribution.Dispose();
		m_EvaluateDistributionLow.Dispose();
		m_EvaluateDistributionMedium.Dispose();
		m_EvaluateDistributionHigh.Dispose();
		m_EvaluateDistributionLowrent.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0910: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0943: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_0965: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_0998: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		NativeParallelHashMap<Entity, CachedPropertyInformation> cachedPropertyInfo = default(NativeParallelHashMap<Entity, CachedPropertyInformation>);
		cachedPropertyInfo._002Ector(((EntityQuery)(ref m_FreePropertyQuery)).CalculateEntityCount(), AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		NativeArray<GroundPollution> map = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeArray<AirPollution> map2 = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies2);
		JobHandle dependencies3;
		NativeArray<NoisePollution> map3 = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies3);
		JobHandle dependencies4;
		CellMapData<TelecomCoverage> data = m_TelecomCoverageSystem.GetData(readOnly: true, out dependencies4);
		PreparePropertyJob obj = new PreparePropertyJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingProperties = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkDatas = InternalCompilerInterface.GetComponentLookup<ParkData>(ref __TypeHandle.__Game_Prefabs_ParkData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Abandoneds = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Parks = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyData = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverages = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Crimes = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Locked = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumers = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumers = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducers = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducers = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionMap = map,
			m_AirPollutionMap = map2,
			m_NoiseMap = map3,
			m_TelecomCoverages = data,
			m_HealthcareParameters = ((EntityQuery)(ref m_HealthcareParameterQuery)).GetSingleton<HealthcareParameterData>(),
			m_ParkParameters = ((EntityQuery)(ref m_ParkParameterQuery)).GetSingleton<ParkParameterData>(),
			m_EducationParameters = ((EntityQuery)(ref m_EducationParameterQuery)).GetSingleton<EducationParameterData>(),
			m_TelecomParameters = ((EntityQuery)(ref m_TelecomParameterQuery)).GetSingleton<TelecomParameterData>(),
			m_GarbageParameters = ((EntityQuery)(ref m_GarbageParameterQuery)).GetSingleton<GarbageParameterData>(),
			m_PoliceParameters = ((EntityQuery)(ref m_PoliceParameterQuery)).GetSingleton<PoliceConfigurationData>(),
			m_CitizenHappinessParameterData = ((EntityQuery)(ref m_CitizenHappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>(),
			m_City = m_CitySystem.City,
			m_PropertyData = cachedPropertyInfo.AsParallelWriter()
		};
		JobHandle val = default(JobHandle);
		JobHandle val2 = default(JobHandle);
		JobHandle deps;
		FindPropertyJob findPropertyJob = new FindPropertyJob
		{
			m_HomelessHouseholdEntities = ((EntityQuery)(ref m_HomelessHouseholdQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_MovedInHouseholdEntities = ((EntityQuery)(ref m_HouseholdQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
			m_CachedPropertyInfo = cachedPropertyInfo,
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertiesOnMarket = InternalCompilerInterface.GetComponentLookup<PropertyOnMarket>(ref __TypeHandle.__Game_Buildings_PropertyOnMarket_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Availabilities = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingProperties = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationBuffers = InternalCompilerInterface.GetBufferLookup<PathInformations>(ref __TypeHandle.__Game_Pathfind_PathInformations_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverages = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholds = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Crimes = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lockeds = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Abandoneds = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Parks = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumers = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumers = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducers = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducers = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildings = InternalCompilerInterface.GetComponentLookup<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransports = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformations = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenBuffers = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertySeekers = InternalCompilerInterface.GetComponentLookup<PropertySeeker>(ref __TypeHandle.__Game_Agents_PropertySeeker_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionMap = map,
			m_AirPollutionMap = map2,
			m_NoiseMap = map3,
			m_TelecomCoverages = data,
			m_ResidentialPropertyData = m_CountResidentialPropertySystem.GetResidentialPropertyData(),
			m_HealthcareParameters = ((EntityQuery)(ref m_HealthcareParameterQuery)).GetSingleton<HealthcareParameterData>(),
			m_ParkParameters = ((EntityQuery)(ref m_ParkParameterQuery)).GetSingleton<ParkParameterData>(),
			m_EducationParameters = ((EntityQuery)(ref m_EducationParameterQuery)).GetSingleton<EducationParameterData>(),
			m_TelecomParameters = ((EntityQuery)(ref m_TelecomParameterQuery)).GetSingleton<TelecomParameterData>(),
			m_GarbageParameters = ((EntityQuery)(ref m_GarbageParameterQuery)).GetSingleton<GarbageParameterData>(),
			m_PoliceParameters = ((EntityQuery)(ref m_PoliceParameterQuery)).GetSingleton<PoliceConfigurationData>(),
			m_CitizenHappinessParameterData = ((EntityQuery)(ref m_CitizenHappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>(),
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_RentActionQueue = m_PropertyProcessingSystem.GetRentActionQueue(out deps).AsParallelWriter(),
			m_City = m_CitySystem.City,
			m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 80, 16).AsParallelWriter(),
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<PreparePropertyJob>(obj, m_FreePropertyQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies3, dependencies2, dependencies4, deps));
		((SystemBase)this).Dependency = IJobExtensions.Schedule<FindPropertyJob>(findPropertyJob, JobHandle.CombineDependencies(val3, val2, val));
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_PathfindSetupSystem.AddQueueWriter(((SystemBase)this).Dependency);
		m_AirPollutionSystem.AddReader(((SystemBase)this).Dependency);
		m_NoisePollutionSystem.AddReader(((SystemBase)this).Dependency);
		m_GroundPollutionSystem.AddReader(((SystemBase)this).Dependency);
		m_TelecomCoverageSystem.AddReader(((SystemBase)this).Dependency);
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
		m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
		m_TaxSystem.AddReader(((SystemBase)this).Dependency);
		cachedPropertyInfo.Dispose(((SystemBase)this).Dependency);
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
	public HouseholdFindPropertySystem()
	{
	}
}
