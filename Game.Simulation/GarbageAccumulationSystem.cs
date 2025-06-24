using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Serialization;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class GarbageAccumulationSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPreDeserialize
{
	[BurstCompile]
	private struct GarbageAccumulationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		public ComponentTypeHandle<GarbageProducer> m_GarbageProducerType;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> m_GarbageCollectionRequestData;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> m_ConsumptionDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneDatas;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> m_HomelessHousehold;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<Employee> m_Employees;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public BufferLookup<Game.Buildings.Student> m_Students;

		[ReadOnly]
		public BufferLookup<Occupant> m_Occupants;

		[ReadOnly]
		public BufferLookup<Patient> m_Patients;

		[ReadOnly]
		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public int m_UpdateFrame;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_CollectionRequestArchetype;

		[ReadOnly]
		public GarbageParameterData m_GarbageParameters;

		[ReadOnly]
		public float m_GarbageEfficiencyPenalty;

		public ParallelWriter m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public NativeArray<long> m_GarbageAccumulation;

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
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<GarbageProducer> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarbageProducer>(ref m_GarbageProducerType);
			NativeArray<CurrentDistrict> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentDistrict>(ref m_CurrentDistrictType);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			long num = 0L;
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			ConsumptionData data = default(ConsumptionData);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				ref GarbageProducer reference = ref CollectionUtils.ElementAt<GarbageProducer>(nativeArray2, i);
				CurrentDistrict currentDistrict = nativeArray3[i];
				Entity prefab = m_Prefabs[val].m_Prefab;
				m_ConsumptionDatas.TryGetComponent(prefab, ref data);
				if (bufferAccessor.Length != 0)
				{
					UpgradeUtils.CombineStats<ConsumptionData>(ref data, bufferAccessor[i], ref m_Prefabs, ref m_ConsumptionDatas);
				}
				GetGarbageAccumulation(val, prefab, ref data, currentDistrict, cityModifiers, m_Citizens, m_SpawnableDatas, m_ZoneDatas, m_HomelessHousehold, m_HouseholdCitizens, m_Renters, m_Employees, m_Students, m_Occupants, m_Patients, m_DistrictModifiers, ref m_GarbageParameters);
				GarbageCollectionRequestFlags garbageCollectionRequestFlags = (GarbageCollectionRequestFlags)0;
				if (m_SpawnableDatas.HasComponent(prefab))
				{
					SpawnableBuildingData spawnableBuildingData = m_SpawnableDatas[prefab];
					if (m_ZoneDatas.HasComponent(spawnableBuildingData.m_ZonePrefab) && m_ZoneDatas[spawnableBuildingData.m_ZonePrefab].m_AreaType == Game.Zones.AreaType.Industrial)
					{
						garbageCollectionRequestFlags |= GarbageCollectionRequestFlags.IndustrialWaste;
					}
				}
				int garbage = reference.m_Garbage;
				int num2 = MathUtils.RoundToIntRandom(ref random, data.m_GarbageAccumulation / (float)kUpdatesPerDay);
				reference.m_Garbage += num2;
				reference.m_Garbage = math.min(reference.m_Garbage, m_GarbageParameters.m_MaxGarbageAccumulation);
				num += num2;
				RequestCollectionIfNeeded(unfilteredChunkIndex, val, ref reference, garbageCollectionRequestFlags);
				AddWarningIfNeeded(val, ref reference, garbage);
				if (garbage >= m_GarbageParameters.m_RequestGarbageLimit != reference.m_Garbage >= m_GarbageParameters.m_RequestGarbageLimit || garbage >= m_GarbageParameters.m_WarningGarbageLimit != reference.m_Garbage >= m_GarbageParameters.m_WarningGarbageLimit)
				{
					QuantityUpdated(unfilteredChunkIndex, val);
				}
				if (bufferAccessor2.Length != 0)
				{
					float garbageEfficiencyFactor = GetGarbageEfficiencyFactor(reference.m_Garbage, m_GarbageParameters, m_GarbageEfficiencyPenalty);
					BuildingUtils.SetEfficiencyFactor(bufferAccessor2[i], EfficiencyFactor.Garbage, garbageEfficiencyFactor);
				}
			}
			AddGarbageAccumulation(num);
		}

		private void QuantityUpdated(int jobIndex, Entity buildingEntity, bool updateAll = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (!m_SubObjects.TryGetBuffer(buildingEntity, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				bool updateAll2 = false;
				if (updateAll || m_QuantityData.HasComponent(subObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, subObject, default(BatchesUpdated));
					updateAll2 = true;
				}
				QuantityUpdated(jobIndex, subObject, updateAll2);
			}
		}

		private unsafe void AddGarbageAccumulation(long accumulation)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			long* unsafePtr = (long*)NativeArrayUnsafeUtility.GetUnsafePtr<long>(m_GarbageAccumulation);
			unsafePtr += m_UpdateFrame;
			Interlocked.Add(ref *unsafePtr, accumulation);
		}

		private void RequestCollectionIfNeeded(int jobIndex, Entity entity, ref GarbageProducer garbage, GarbageCollectionRequestFlags flags)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			GarbageCollectionRequest garbageCollectionRequest = default(GarbageCollectionRequest);
			if (garbage.m_Garbage > m_GarbageParameters.m_RequestGarbageLimit && (!m_GarbageCollectionRequestData.TryGetComponent(garbage.m_CollectionRequest, ref garbageCollectionRequest) || (!(garbageCollectionRequest.m_Target == entity) && garbageCollectionRequest.m_DispatchIndex != garbage.m_DispatchIndex)))
			{
				garbage.m_CollectionRequest = Entity.Null;
				garbage.m_DispatchIndex = 0;
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_CollectionRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<GarbageCollectionRequest>(jobIndex, val, new GarbageCollectionRequest(entity, garbage.m_Garbage, flags));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
			}
		}

		private void AddWarningIfNeeded(Entity entity, ref GarbageProducer garbage, int oldGarbage)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (garbage.m_Garbage > m_GarbageParameters.m_WarningGarbageLimit && oldGarbage <= m_GarbageParameters.m_WarningGarbageLimit)
			{
				m_IconCommandBuffer.Add(entity, m_GarbageParameters.m_GarbageNotificationPrefab, IconPriority.Problem);
				garbage.m_Flags |= GarbageProducerFlags.GarbagePilingUpWarning;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RW_BufferTypeHandle;

		public ComponentTypeHandle<GarbageProducer> __Game_Buildings_GarbageProducer_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<GarbageCollectionRequest> __Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Quantity> __Game_Objects_Quantity_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> __Game_Prefabs_ConsumptionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Buildings.Student> __Game_Buildings_Student_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Occupant> __Game_Buildings_Occupant_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Patient> __Game_Buildings_Patient_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<DistrictModifier> __Game_Areas_DistrictModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_Efficiency_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(false);
			__Game_Buildings_GarbageProducer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarbageProducer>(false);
			__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageCollectionRequest>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Objects_Quantity_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Quantity>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ConsumptionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConsumptionData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HomelessHousehold>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Buildings_Student_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Buildings.Student>(true);
			__Game_Buildings_Occupant_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Occupant>(true);
			__Game_Buildings_Patient_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Patient>(true);
			__Game_Areas_DistrictModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DistrictModifier>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 16;

	private SimulationSystem m_SimulationSystem;

	private IconCommandSystem m_IconCommandSystem;

	private CitySystem m_CitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_GarbageProducerQuery;

	private EntityArchetype m_CollectionRequestArchetype;

	private NativeArray<long> m_GarbageAccumulation;

	private JobHandle m_AccumulationDeps;

	private long m_Accumulation;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_2138252455_0;

	private EntityQuery __query_2138252455_1;

	public long garbageAccumulation => m_Accumulation * kUpdatesPerDay;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	public static void GetGarbage(ref ConsumptionData consumption, Entity building, Entity prefab, BufferLookup<Renter> renters, BufferLookup<Game.Buildings.Student> students, BufferLookup<Occupant> occupants, ComponentLookup<HomelessHousehold> homelessHouseholds, BufferLookup<HouseholdCitizen> householdCitizens, ComponentLookup<Citizen> citizens, BufferLookup<Employee> employees, BufferLookup<Patient> patients, ComponentLookup<SpawnableBuildingData> spawnableDatas, ComponentLookup<CurrentDistrict> currentDistricts, BufferLookup<DistrictModifier> districtModifiers, ComponentLookup<ZoneData> zoneDatas, DynamicBuffer<CityModifier> cityModifiers, ref GarbageParameterData garbageParameter)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		CurrentDistrict currentDistrict = currentDistricts[building];
		GetGarbageAccumulation(building, prefab, ref consumption, currentDistrict, cityModifiers, citizens, spawnableDatas, zoneDatas, homelessHouseholds, householdCitizens, renters, employees, students, occupants, patients, districtModifiers, ref garbageParameter);
	}

	public static void GetGarbageAccumulation(Entity building, Entity prefab, ref ConsumptionData consumption, CurrentDistrict currentDistrict, DynamicBuffer<CityModifier> cityModifiers, ComponentLookup<Citizen> citizens, ComponentLookup<SpawnableBuildingData> spawnableDatas, ComponentLookup<ZoneData> zoneDatas, ComponentLookup<HomelessHousehold> homelessHousehold, BufferLookup<HouseholdCitizen> householdCitizens, BufferLookup<Renter> renters, BufferLookup<Employee> employees, BufferLookup<Game.Buildings.Student> students, BufferLookup<Occupant> occupants, BufferLookup<Patient> patients, BufferLookup<DistrictModifier> districtModifiers, ref GarbageParameterData garbageParameter)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		int num2 = 0;
		float num3 = 0f;
		if (renters.HasBuffer(building))
		{
			DynamicBuffer<Renter> val = renters[building];
			for (int i = 0; i < val.Length; i++)
			{
				Entity renter = val[i].m_Renter;
				if (householdCitizens.HasBuffer(renter))
				{
					DynamicBuffer<HouseholdCitizen> val2 = householdCitizens[renter];
					if (homelessHousehold.HasComponent(renter))
					{
						num2 += val2.Length;
						continue;
					}
					for (int j = 0; j < val2.Length; j++)
					{
						Entity citizen = val2[j].m_Citizen;
						if (citizens.HasComponent(citizen))
						{
							num3 += (float)citizens[citizen].GetEducationLevel();
							num += 1f;
						}
					}
				}
				else
				{
					if (!employees.HasBuffer(renter))
					{
						continue;
					}
					DynamicBuffer<Employee> val3 = employees[renter];
					for (int k = 0; k < val3.Length; k++)
					{
						Entity worker = val3[k].m_Worker;
						if (citizens.HasComponent(worker))
						{
							num3 += (float)citizens[worker].GetEducationLevel();
							num += 1f;
						}
					}
				}
			}
			if (employees.HasBuffer(building))
			{
				DynamicBuffer<Employee> val4 = employees[building];
				for (int l = 0; l < val4.Length; l++)
				{
					Entity worker2 = val4[l].m_Worker;
					if (citizens.HasComponent(worker2))
					{
						num3 += (float)citizens[worker2].GetEducationLevel();
						num += 1f;
					}
				}
			}
		}
		else
		{
			if (employees.HasBuffer(building))
			{
				DynamicBuffer<Employee> val5 = employees[building];
				for (int m = 0; m < val5.Length; m++)
				{
					Entity worker3 = val5[m].m_Worker;
					if (citizens.HasComponent(worker3))
					{
						num3 += (float)citizens[worker3].GetEducationLevel();
						num += 1f;
					}
				}
			}
			if (students.HasBuffer(building))
			{
				DynamicBuffer<Game.Buildings.Student> val6 = students[building];
				for (int n = 0; n < val6.Length; n++)
				{
					Entity val7 = val6[n];
					if (citizens.HasComponent(val7))
					{
						num3 += (float)citizens[val7].GetEducationLevel();
						num += 1f;
					}
				}
			}
			if (occupants.HasBuffer(building))
			{
				DynamicBuffer<Occupant> val8 = occupants[building];
				for (int num4 = 0; num4 < val8.Length; num4++)
				{
					Entity val9 = val8[num4];
					if (citizens.HasComponent(val9))
					{
						num3 += (float)citizens[val9].GetEducationLevel();
						num += 1f;
					}
				}
			}
			if (patients.HasBuffer(building))
			{
				DynamicBuffer<Patient> val10 = patients[building];
				for (int num5 = 0; num5 < val10.Length; num5++)
				{
					Entity patient = val10[num5].m_Patient;
					if (citizens.HasComponent(patient))
					{
						num3 += (float)citizens[patient].GetEducationLevel();
						num += 1f;
					}
				}
			}
		}
		float num6 = 0f;
		if (spawnableDatas.HasComponent(prefab))
		{
			num6 = (int)spawnableDatas[prefab].m_Level;
		}
		float num7 = 0f;
		num7 = ((!(num > 0f)) ? (consumption.m_GarbageAccumulation - num6 * garbageParameter.m_BuildingLevelBalance) : (math.max(0f, consumption.m_GarbageAccumulation - (num6 * garbageParameter.m_BuildingLevelBalance + num3 / num * garbageParameter.m_EducationBalance)) * num));
		if (num2 > 0)
		{
			num7 += (float)(garbageParameter.m_HomelessGarbageProduce * num2);
		}
		if (districtModifiers.HasBuffer(currentDistrict.m_District))
		{
			DynamicBuffer<DistrictModifier> modifiers = districtModifiers[currentDistrict.m_District];
			AreaUtils.ApplyModifier(ref num7, modifiers, DistrictModifierType.GarbageProduction);
		}
		if (spawnableDatas.HasComponent(prefab))
		{
			SpawnableBuildingData spawnableBuildingData = spawnableDatas[prefab];
			if (zoneDatas.HasComponent(spawnableBuildingData.m_ZonePrefab) && zoneDatas[spawnableBuildingData.m_ZonePrefab].m_AreaType == Game.Zones.AreaType.Industrial && (zoneDatas[spawnableBuildingData.m_ZonePrefab].m_ZoneFlags & ZoneFlags.Office) == 0)
			{
				CityUtils.ApplyModifier(ref num7, cityModifiers, CityModifierType.IndustrialGarbage);
			}
		}
		consumption.m_GarbageAccumulation = num7;
	}

	public static float GetGarbageEfficiencyFactor(int garbage, GarbageParameterData garbageParameters, float maxPenalty)
	{
		float num = math.saturate((float)(garbage - garbageParameters.m_WarningGarbageLimit) / (float)(garbageParameters.m_MaxGarbageAccumulation - garbageParameters.m_WarningGarbageLimit));
		return 1f - maxPenalty * num;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_GarbageAccumulation = new NativeArray<long>(16, (Allocator)4, (NativeArrayOptions)1);
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<GarbageProducer>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<UpdateFrame>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_GarbageProducerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_CollectionRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<GarbageCollectionRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_GarbageProducerQuery);
		((ComponentSystemBase)this).RequireForUpdate<GarbageParameterData>();
		((ComponentSystemBase)this).RequireForUpdate<BuildingEfficiencyParameterData>();
		Assert.IsTrue((long)(262144 / kUpdatesPerDay) >= 512L);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_GarbageAccumulation.Dispose();
	}

	public void PreDeserialize(Context context)
	{
		m_Accumulation = 0L;
		for (int i = 0; i < m_GarbageAccumulation.Length; i++)
		{
			m_GarbageAccumulation[i] = 0L;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		((JobHandle)(ref m_AccumulationDeps)).Complete();
		long num = 0L;
		for (int i = 0; i < 16; i++)
		{
			num += m_GarbageAccumulation[i];
		}
		m_Accumulation = num;
		m_GarbageAccumulation[(int)updateFrame] = 0L;
		GarbageParameterData singleton = ((EntityQuery)(ref __query_2138252455_0)).GetSingleton<GarbageParameterData>();
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, singleton.m_GarbageServicePrefab))
		{
			((EntityQuery)(ref m_GarbageProducerQuery)).ResetFilter();
			((EntityQuery)(ref m_GarbageProducerQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(updateFrame));
			GarbageAccumulationJob garbageAccumulationJob = new GarbageAccumulationJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentDistrictType = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GarbageProducerType = InternalCompilerInterface.GetComponentTypeHandle<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_GarbageCollectionRequestData = InternalCompilerInterface.GetComponentLookup<GarbageCollectionRequest>(ref __TypeHandle.__Game_Simulation_GarbageCollectionRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_QuantityData = InternalCompilerInterface.GetComponentLookup<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConsumptionDatas = InternalCompilerInterface.GetComponentLookup<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ZoneDatas = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HomelessHousehold = InternalCompilerInterface.GetComponentLookup<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Students = InternalCompilerInterface.GetBufferLookup<Game.Buildings.Student>(ref __TypeHandle.__Game_Buildings_Student_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Occupants = InternalCompilerInterface.GetBufferLookup<Occupant>(ref __TypeHandle.__Game_Buildings_Occupant_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Patients = InternalCompilerInterface.GetBufferLookup<Patient>(ref __TypeHandle.__Game_Buildings_Patient_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DistrictModifiers = InternalCompilerInterface.GetBufferLookup<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_City = m_CitySystem.City,
				m_UpdateFrame = (int)updateFrame,
				m_RandomSeed = RandomSeed.Next(),
				m_CollectionRequestArchetype = m_CollectionRequestArchetype,
				m_GarbageParameters = singleton,
				m_GarbageEfficiencyPenalty = ((EntityQuery)(ref __query_2138252455_1)).GetSingleton<BuildingEfficiencyParameterData>().m_GarbagePenalty
			};
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			garbageAccumulationJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			garbageAccumulationJob.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
			garbageAccumulationJob.m_GarbageAccumulation = m_GarbageAccumulation;
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<GarbageAccumulationJob>(garbageAccumulationJob, m_GarbageProducerQuery, ((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(val2);
			m_IconCommandSystem.AddCommandBufferWriter(val2);
			((SystemBase)this).Dependency = val2;
			m_AccumulationDeps = val2;
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte num = (byte)m_GarbageAccumulation.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		for (int i = 0; i < m_GarbageAccumulation.Length; i++)
		{
			long num2 = m_GarbageAccumulation[i];
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		m_Accumulation = 0L;
		byte b = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
		long num = default(long);
		for (int i = 0; i < b; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			if (i < m_GarbageAccumulation.Length)
			{
				m_GarbageAccumulation[i] = num;
				m_Accumulation += num;
			}
		}
		for (int j = b; j < m_GarbageAccumulation.Length; j++)
		{
			m_GarbageAccumulation[j] = 0L;
		}
	}

	public void SetDefaults(Context context)
	{
		m_Accumulation = 0L;
		for (int i = 0; i < m_GarbageAccumulation.Length; i++)
		{
			m_GarbageAccumulation[i] = 0L;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<GarbageParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_2138252455_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<BuildingEfficiencyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_2138252455_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public GarbageAccumulationSystem()
	{
	}
}
