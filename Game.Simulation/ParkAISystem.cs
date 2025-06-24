using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ParkAISystem : GameSystemBase
{
	[BurstCompile]
	private struct ParkTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<Game.Buildings.Park> m_ParkType;

		[ReadOnly]
		public ComponentTypeHandle<MaintenanceConsumer> m_MaintenanceConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<ModifiedServiceCoverage> m_ModifiedServiceCoverageType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public ComponentLookup<CoverageData> m_CoverageDatas;

		[ReadOnly]
		public ComponentLookup<ParkData> m_ParkDatas;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> m_MaintenanceRequestData;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public EntityArchetype m_MaintenanceRequestArchetype;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.Park> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.Park>(ref m_ParkType);
			NativeArray<MaintenanceConsumer> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MaintenanceConsumer>(ref m_MaintenanceConsumerType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<ModifiedServiceCoverage> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ModifiedServiceCoverage>(ref m_ModifiedServiceCoverageType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<CurrentDistrict>(ref m_CurrentDistrictType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = nativeArray4[i].m_Prefab;
				if (m_ParkDatas.HasComponent(prefab))
				{
					ParkData prefabParkData = m_ParkDatas[prefab];
					Game.Buildings.Park park = nativeArray2[i];
					park.m_Maintenance = (short)math.max(0, park.m_Maintenance - (400 + 50 * bufferAccessor[i].Length) / kUpdatesPerDay);
					nativeArray2[i] = park;
					if (nativeArray3.Length != 0)
					{
						MaintenanceConsumer maintenanceConsumer = nativeArray3[i];
						RequestMaintenanceIfNeeded(unfilteredChunkIndex, val, park, maintenanceConsumer, prefabParkData);
					}
					if (m_CoverageDatas.HasComponent(prefab))
					{
						CoverageData prefabCoverageData = m_CoverageDatas[prefab];
						nativeArray5[i] = GetModifiedServiceCoverage(park, prefabParkData, prefabCoverageData, cityModifiers);
					}
					if (!flag)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentDistrict>(unfilteredChunkIndex, val, default(CurrentDistrict));
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, val, default(Updated));
					}
				}
			}
		}

		private void RequestMaintenanceIfNeeded(int jobIndex, Entity entity, Game.Buildings.Park park, MaintenanceConsumer maintenanceConsumer, ParkData prefabParkData)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			int maintenancePriority = GetMaintenancePriority(park, prefabParkData);
			if (maintenancePriority > 0 && !m_MaintenanceRequestData.HasComponent(maintenanceConsumer.m_Request))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MaintenanceRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MaintenanceRequest>(jobIndex, val, new MaintenanceRequest(entity, maintenancePriority));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(32u));
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Buildings.Park> __Game_Buildings_Park_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MaintenanceConsumer> __Game_Simulation_MaintenanceConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentTypeHandle;

		public ComponentTypeHandle<ModifiedServiceCoverage> __Game_Buildings_ModifiedServiceCoverage_RW_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<CoverageData> __Game_Prefabs_CoverageData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkData> __Game_Prefabs_ParkData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> __Game_Simulation_MaintenanceRequest_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Park_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Park>(false);
			__Game_Simulation_MaintenanceConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MaintenanceConsumer>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(true);
			__Game_Buildings_ModifiedServiceCoverage_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ModifiedServiceCoverage>(false);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Prefabs_CoverageData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CoverageData>(true);
			__Game_Prefabs_ParkData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkData>(true);
			__Game_Simulation_MaintenanceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceRequest>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 256;

	private CitySystem m_CitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_ParkQuery;

	private EntityArchetype m_MaintenanceRequestArchetype;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ParkQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Buildings.Park>(),
			ComponentType.ReadWrite<ModifiedServiceCoverage>(),
			ComponentType.ReadOnly<Renter>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_MaintenanceRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<MaintenanceRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ParkQuery);
		Assert.IsTrue((long)(262144 / kUpdatesPerDay) >= 512L);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		ParkTickJob parkTickJob = new ParkTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceConsumerType = InternalCompilerInterface.GetComponentTypeHandle<MaintenanceConsumer>(ref __TypeHandle.__Game_Simulation_MaintenanceConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictType = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ModifiedServiceCoverageType = InternalCompilerInterface.GetComponentTypeHandle<ModifiedServiceCoverage>(ref __TypeHandle.__Game_Buildings_ModifiedServiceCoverage_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CoverageDatas = InternalCompilerInterface.GetComponentLookup<CoverageData>(ref __TypeHandle.__Game_Prefabs_CoverageData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkDatas = InternalCompilerInterface.GetComponentLookup<ParkData>(ref __TypeHandle.__Game_Prefabs_ParkData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequestData = InternalCompilerInterface.GetComponentLookup<MaintenanceRequest>(ref __TypeHandle.__Game_Simulation_MaintenanceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_MaintenanceRequestArchetype = m_MaintenanceRequestArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		parkTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<ParkTickJob>(parkTickJob, m_ParkQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
	}

	public static int GetMaintenancePriority(Game.Buildings.Park park, ParkData prefabParkData)
	{
		return prefabParkData.m_MaintenancePool - park.m_Maintenance - prefabParkData.m_MaintenancePool / 10;
	}

	public static ModifiedServiceCoverage GetModifiedServiceCoverage(Game.Buildings.Park park, ParkData prefabParkData, CoverageData prefabCoverageData, DynamicBuffer<CityModifier> cityModifiers)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)park.m_Maintenance / (float)math.max(1, (int)prefabParkData.m_MaintenancePool);
		ModifiedServiceCoverage result = new ModifiedServiceCoverage(prefabCoverageData);
		int num2 = Mathf.FloorToInt(num / 0.3f);
		result.m_Magnitude *= 0.95f + 0.05f * (float)math.min(1, num2) + 0.1f * (float)math.max(0, num2 - 1);
		result.m_Range *= 0.95f + 0.05f * (float)num2;
		if (cityModifiers.IsCreated)
		{
			CityUtils.ApplyModifier(ref result.m_Magnitude, cityModifiers, CityModifierType.ParkEntertainment);
		}
		return result;
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
	public ParkAISystem()
	{
	}
}
