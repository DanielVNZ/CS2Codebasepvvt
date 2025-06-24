using System.Runtime.CompilerServices;
using Game.Common;
using Game.Objects;
using Game.Tools;
using Game.Vehicles;
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
public class DamagedVehicleSystem : GameSystemBase
{
	[BurstCompile]
	private struct DamagedVehicleJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> m_DestroyedType;

		[ReadOnly]
		public ComponentTypeHandle<MaintenanceConsumer> m_MaintenanceConsumerType;

		[ReadOnly]
		public ComponentTypeHandle<Damaged> m_DamagedType;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> m_MaintenanceRequestData;

		[ReadOnly]
		public EntityArchetype m_MaintenanceRequestArchetype;

		public ParallelWriter m_CommandBuffer;

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
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Destroyed> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Destroyed>(ref m_DestroyedType);
			NativeArray<MaintenanceConsumer> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MaintenanceConsumer>(ref m_MaintenanceConsumerType);
			NativeArray<Damaged> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Damaged>(ref m_DamagedType);
			if (nativeArray2.Length != 0)
			{
				if (nativeArray3.Length != 0)
				{
					for (int i = 0; i < nativeArray2.Length; i++)
					{
						Destroyed destroyed = nativeArray2[i];
						Entity val = nativeArray[i];
						if (destroyed.m_Cleared < 1f)
						{
							MaintenanceConsumer maintenanceConsumer = nativeArray3[i];
							RequestMaintenanceIfNeeded(unfilteredChunkIndex, val, maintenanceConsumer);
						}
						else
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<MaintenanceConsumer>(unfilteredChunkIndex, val);
						}
					}
					return;
				}
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					if (nativeArray2[j].m_Cleared < 1f)
					{
						Entity val2 = nativeArray[j];
						MaintenanceConsumer maintenanceConsumer2 = default(MaintenanceConsumer);
						RequestMaintenanceIfNeeded(unfilteredChunkIndex, val2, maintenanceConsumer2);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<MaintenanceConsumer>(unfilteredChunkIndex, val2, maintenanceConsumer2);
					}
				}
				return;
			}
			if (nativeArray3.Length != 0)
			{
				for (int k = 0; k < nativeArray4.Length; k++)
				{
					Damaged damaged = nativeArray4[k];
					Entity val3 = nativeArray[k];
					if (math.any(damaged.m_Damage > 0f))
					{
						MaintenanceConsumer maintenanceConsumer3 = nativeArray3[k];
						RequestMaintenanceIfNeeded(unfilteredChunkIndex, val3, maintenanceConsumer3);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<MaintenanceConsumer>(unfilteredChunkIndex, val3);
					}
				}
				return;
			}
			for (int l = 0; l < nativeArray4.Length; l++)
			{
				if (math.any(nativeArray4[l].m_Damage > 0f))
				{
					Entity val4 = nativeArray[l];
					MaintenanceConsumer maintenanceConsumer4 = default(MaintenanceConsumer);
					RequestMaintenanceIfNeeded(unfilteredChunkIndex, val4, maintenanceConsumer4);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<MaintenanceConsumer>(unfilteredChunkIndex, val4, maintenanceConsumer4);
				}
			}
		}

		private void RequestMaintenanceIfNeeded(int jobIndex, Entity entity, MaintenanceConsumer maintenanceConsumer)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (!m_MaintenanceRequestData.HasComponent(maintenanceConsumer.m_Request))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_MaintenanceRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<MaintenanceRequest>(jobIndex, val, new MaintenanceRequest(entity, 100));
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
		public ComponentTypeHandle<Destroyed> __Game_Common_Destroyed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MaintenanceConsumer> __Game_Simulation_MaintenanceConsumer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Damaged> __Game_Objects_Damaged_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> __Game_Simulation_MaintenanceRequest_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Destroyed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(true);
			__Game_Simulation_MaintenanceConsumer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MaintenanceConsumer>(true);
			__Game_Objects_Damaged_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Damaged>(true);
			__Game_Simulation_MaintenanceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceRequest>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 512u;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_DamagedQuery;

	private EntityArchetype m_MaintenanceRequestArchetype;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 512;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_DamagedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Damaged>(),
			ComponentType.ReadOnly<Stopped>(),
			ComponentType.ReadOnly<Car>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_MaintenanceRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<MaintenanceRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_DamagedQuery);
		Assert.IsTrue(true);
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
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		DamagedVehicleJob damagedVehicleJob = new DamagedVehicleJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceConsumerType = InternalCompilerInterface.GetComponentTypeHandle<MaintenanceConsumer>(ref __TypeHandle.__Game_Simulation_MaintenanceConsumer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedType = InternalCompilerInterface.GetComponentTypeHandle<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequestData = InternalCompilerInterface.GetComponentLookup<MaintenanceRequest>(ref __TypeHandle.__Game_Simulation_MaintenanceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MaintenanceRequestArchetype = m_MaintenanceRequestArchetype
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		damagedVehicleJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<DamagedVehicleJob>(damagedVehicleJob, m_DamagedQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public DamagedVehicleSystem()
	{
	}
}
