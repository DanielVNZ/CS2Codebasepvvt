using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class MailAccumulationSystem : GameSystemBase
{
	[BurstCompile]
	private struct MailAccumulationJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		public ComponentTypeHandle<MailProducer> m_MailProducerType;

		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentLookup<PostVanRequest> m_PostVanRequestData;

		[ReadOnly]
		public ComponentLookup<Quantity> m_QuantityData;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingData;

		[ReadOnly]
		public ComponentLookup<MailAccumulationData> m_MailAccumulationData;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> m_ServiceObjectData;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public BufferLookup<Employee> m_Employees;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_PostVanRequestArchetype;

		[ReadOnly]
		public PostConfigurationData m_PostConfigurationData;

		[ReadOnly]
		public BuildingEfficiencyParameterData m_EfficiencyParameters;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			float num = 0.28444445f;
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			NativeArray<MailProducer> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MailProducer>(ref m_MailProducerType);
			BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			int2 val2 = default(int2);
			int2 val3 = default(int2);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				float2 baseAccumulationRate = GetBaseAccumulationRate(nativeArray2[i].m_Prefab, out var requireCollect);
				if (!math.all(baseAccumulationRate == 0f))
				{
					if (bufferAccessor.Length != 0)
					{
						GetCitizenCounts(bufferAccessor[i], out var residentCount, out var workerCount);
						baseAccumulationRate *= (float)(residentCount + workerCount);
					}
					else
					{
						GetCitizenCounts(val, out var residentCount2, out var workerCount2);
						baseAccumulationRate *= (float)(residentCount2 + workerCount2);
					}
					val2.x = MathUtils.RoundToIntRandom(ref random, baseAccumulationRate.x * num);
					val2.y = MathUtils.RoundToIntRandom(ref random, baseAccumulationRate.y * num);
					ref MailProducer reference = ref CollectionUtils.ElementAt<MailProducer>(nativeArray3, i);
					((int2)(ref val3))._002Ector((int)reference.m_SendingMail, reference.receivingMail);
					int2 val4 = val3;
					val3 = math.min(val3 + val2, int2.op_Implicit(m_PostConfigurationData.m_MaxMailAccumulation));
					reference.m_SendingMail = (ushort)val3.x;
					reference.receivingMail = val3.y;
					RequestPostVanIfNeeded(unfilteredChunkIndex, val, ref reference, requireCollect);
					if (val4.y >= m_PostConfigurationData.m_MailAccumulationTolerance != val3.y >= m_PostConfigurationData.m_MailAccumulationTolerance)
					{
						QuantityUpdated(unfilteredChunkIndex, val);
					}
					if (bufferAccessor2.Length != 0)
					{
						BuildingUtils.SetEfficiencyFactor(bufferAccessor2[i], EfficiencyFactor.Mail, GetMailEfficiencyFactor(reference));
					}
				}
			}
		}

		private void QuantityUpdated(int jobIndex, Entity buildingEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubObjects.HasBuffer(buildingEntity))
			{
				return;
			}
			DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[buildingEntity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subObject = val[i].m_SubObject;
				if (m_QuantityData.HasComponent(subObject))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, subObject, default(BatchesUpdated));
				}
			}
		}

		private void GetCitizenCounts(DynamicBuffer<Renter> renters, out int residentCount, out int workerCount)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			residentCount = 0;
			workerCount = 0;
			for (int i = 0; i < renters.Length; i++)
			{
				GetCitizenCounts(renters[i].m_Renter, out var residentCount2, out var workerCount2);
				residentCount += residentCount2;
				workerCount += workerCount2;
			}
		}

		private void GetCitizenCounts(Entity entity, out int residentCount, out int workerCount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (m_HouseholdCitizens.HasBuffer(entity))
			{
				residentCount = m_HouseholdCitizens[entity].Length;
			}
			else
			{
				residentCount = 0;
			}
			if (m_Employees.HasBuffer(entity))
			{
				workerCount = m_Employees[entity].Length;
			}
			else
			{
				workerCount = 0;
			}
		}

		private float2 GetBaseAccumulationRate(Entity prefab, out bool requireCollect)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			if (m_SpawnableBuildingData.HasComponent(prefab))
			{
				SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildingData[prefab];
				if (m_MailAccumulationData.HasComponent(spawnableBuildingData.m_ZonePrefab))
				{
					MailAccumulationData mailAccumulationData = m_MailAccumulationData[spawnableBuildingData.m_ZonePrefab];
					requireCollect = mailAccumulationData.m_RequireCollect;
					return mailAccumulationData.m_AccumulationRate;
				}
			}
			else if (m_ServiceObjectData.HasComponent(prefab))
			{
				ServiceObjectData serviceObjectData = m_ServiceObjectData[prefab];
				if (m_MailAccumulationData.HasComponent(serviceObjectData.m_Service))
				{
					MailAccumulationData mailAccumulationData2 = m_MailAccumulationData[serviceObjectData.m_Service];
					requireCollect = mailAccumulationData2.m_RequireCollect;
					return mailAccumulationData2.m_AccumulationRate;
				}
			}
			requireCollect = false;
			return default(float2);
		}

		private void RequestPostVanIfNeeded(int jobIndex, Entity entity, ref MailProducer producer, bool requireCollect)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			int num = ((!requireCollect) ? producer.receivingMail : math.max((int)producer.m_SendingMail, producer.receivingMail));
			PostVanRequest postVanRequest = default(PostVanRequest);
			if (num >= m_PostConfigurationData.m_MailAccumulationTolerance && (!m_PostVanRequestData.TryGetComponent(producer.m_MailRequest, ref postVanRequest) || (!(postVanRequest.m_Target == entity) && postVanRequest.m_DispatchIndex != producer.m_DispatchIndex)))
			{
				producer.m_MailRequest = Entity.Null;
				producer.m_DispatchIndex = 0;
				PostVanRequestFlags postVanRequestFlags = PostVanRequestFlags.Deliver | PostVanRequestFlags.BuildingTarget;
				if (requireCollect)
				{
					postVanRequestFlags |= PostVanRequestFlags.Collect;
				}
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_PostVanRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PostVanRequest>(jobIndex, val, new PostVanRequest(entity, postVanRequestFlags, (ushort)num));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
			}
		}

		private float GetMailEfficiencyFactor(MailProducer producer)
		{
			int num = math.max(0, math.max((int)producer.m_SendingMail, producer.receivingMail) - m_EfficiencyParameters.m_NegligibleMail);
			float num2 = 0f;
			if (num > 25)
			{
				int num3 = math.min(50, num - 25);
				num2 = ((float)(num3 * num3) + 125f) / 2625f;
			}
			return 1f - m_EfficiencyParameters.m_MailEfficiencyPenalty * num2;
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

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		public ComponentTypeHandle<MailProducer> __Game_Buildings_MailProducer_RW_ComponentTypeHandle;

		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PostVanRequest> __Game_Simulation_PostVanRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Quantity> __Game_Objects_Quantity_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailAccumulationData> __Game_Prefabs_MailAccumulationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> __Game_Prefabs_ServiceObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Buildings_MailProducer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MailProducer>(false);
			__Game_Buildings_Efficiency_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(false);
			__Game_Simulation_PostVanRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PostVanRequest>(true);
			__Game_Objects_Quantity_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Quantity>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_MailAccumulationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailAccumulationData>(true);
			__Game_Prefabs_ServiceObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceObjectData>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 64u;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_MailProducerQuery;

	private EntityArchetype m_PostVanRequestArchetype;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_890676534_0;

	private EntityQuery __query_890676534_1;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MailProducer>() };
		val.Any = (ComponentType[])(object)new ComponentType[0];
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_MailProducerQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PostVanRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<PostVanRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_MailProducerQuery);
		((ComponentSystemBase)this).RequireForUpdate<PostConfigurationData>();
		((ComponentSystemBase)this).RequireForUpdate<BuildingEfficiencyParameterData>();
		Assert.IsTrue(true);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		PostConfigurationData singleton = ((EntityQuery)(ref __query_890676534_0)).GetSingleton<PostConfigurationData>();
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, singleton.m_PostServicePrefab))
		{
			uint updateFrameIndex = (m_SimulationSystem.frameIndex / 64) & 0xF;
			MailAccumulationJob mailAccumulationJob = new MailAccumulationJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MailProducerType = InternalCompilerInterface.GetComponentTypeHandle<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PostVanRequestData = InternalCompilerInterface.GetComponentLookup<PostVanRequest>(ref __TypeHandle.__Game_Simulation_PostVanRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_QuantityData = InternalCompilerInterface.GetComponentLookup<Quantity>(ref __TypeHandle.__Game_Objects_Quantity_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableBuildingData = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MailAccumulationData = InternalCompilerInterface.GetComponentLookup<MailAccumulationData>(ref __TypeHandle.__Game_Prefabs_MailAccumulationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceObjectData = InternalCompilerInterface.GetComponentLookup<ServiceObjectData>(ref __TypeHandle.__Game_Prefabs_ServiceObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateFrameIndex = updateFrameIndex,
				m_RandomSeed = RandomSeed.Next(),
				m_PostVanRequestArchetype = m_PostVanRequestArchetype,
				m_PostConfigurationData = singleton,
				m_EfficiencyParameters = ((EntityQuery)(ref __query_890676534_1)).GetSingleton<BuildingEfficiencyParameterData>()
			};
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			mailAccumulationJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<MailAccumulationJob>(mailAccumulationJob, m_MailProducerQuery, ((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(val2);
			((SystemBase)this).Dependency = val2;
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<PostConfigurationData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_890676534_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<BuildingEfficiencyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_890676534_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public MailAccumulationSystem()
	{
	}
}
