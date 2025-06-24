using System.Runtime.CompilerServices;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ApplyWaterSourcesSystem : GameSystemBase
{
	[BurstCompile]
	private struct HandleTempEntitiesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Simulation.WaterSourceData> m_WaterSourceData;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				Temp temp = nativeArray2[i];
				if ((temp.m_Flags & TempFlags.Cancel) != 0)
				{
					Cancel(unfilteredChunkIndex, entity, temp);
				}
				else if ((temp.m_Flags & TempFlags.Delete) != 0)
				{
					Delete(unfilteredChunkIndex, entity, temp);
				}
				else if (m_WaterSourceData.HasComponent(temp.m_Original))
				{
					UpdateComponent<Game.Simulation.WaterSourceData>(unfilteredChunkIndex, entity, temp.m_Original, m_WaterSourceData, updateValue: true);
					UpdateComponent<Transform>(unfilteredChunkIndex, entity, temp.m_Original, m_TransformData, updateValue: true);
					Update(unfilteredChunkIndex, entity, temp);
				}
				else
				{
					Create(unfilteredChunkIndex, entity, temp);
				}
			}
		}

		private void Cancel(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (m_HiddenData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(chunkIndex, temp.m_Original, default(BatchesUpdated));
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(chunkIndex, temp.m_Original);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void Delete(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (m_WaterSourceData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, temp.m_Original, default(Deleted));
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void UpdateComponent<T>(int chunkIndex, Entity entity, Entity original, ComponentLookup<T> data, bool updateValue) where T : unmanaged, IComponentData
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (data.HasComponent(entity))
			{
				if (data.HasComponent(original))
				{
					if (updateValue)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<T>(chunkIndex, original, data[entity]);
					}
				}
				else if (updateValue)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<T>(chunkIndex, original, data[entity]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<T>(chunkIndex, original, default(T));
				}
			}
			else if (data.HasComponent(original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<T>(chunkIndex, original);
			}
		}

		private void UpdateComponent<T>(int chunkIndex, Entity entity, Entity original, BufferLookup<T> data, bool updateValue) where T : unmanaged, IBufferElementData
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (data.HasBuffer(entity))
			{
				if (data.HasBuffer(original))
				{
					if (updateValue)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<T>(chunkIndex, original).CopyFrom(data[entity]);
					}
				}
				else if (updateValue)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<T>(chunkIndex, original).CopyFrom(data[entity]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<T>(chunkIndex, original);
				}
			}
			else if (data.HasBuffer(original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<T>(chunkIndex, original);
			}
		}

		private void Update(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (m_HiddenData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(chunkIndex, temp.m_Original, default(BatchesUpdated));
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(chunkIndex, temp.m_Original);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(chunkIndex, temp.m_Original, default(Updated));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void Create(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Temp>(chunkIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(chunkIndex, entity, default(Updated));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Created>(chunkIndex, entity, default(Created));
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
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Simulation.WaterSourceData> __Game_Simulation_WaterSourceData_RO_ComponentLookup;

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
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Simulation_WaterSourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Simulation.WaterSourceData>(true);
		}
	}

	private ToolOutputBarrier m_ToolOutputBarrier;

	private EntityQuery m_TempQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Game.Simulation.WaterSourceData>(),
			ComponentType.Exclude<PrefabRef>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		HandleTempEntitiesJob handleTempEntitiesJob = new HandleTempEntitiesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterSourceData = InternalCompilerInterface.GetComponentLookup<Game.Simulation.WaterSourceData>(ref __TypeHandle.__Game_Simulation_WaterSourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ToolOutputBarrier.CreateCommandBuffer();
		handleTempEntitiesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<HandleTempEntitiesJob>(handleTempEntitiesJob, m_TempQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
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
	public ApplyWaterSourcesSystem()
	{
	}
}
