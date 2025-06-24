using System.Runtime.CompilerServices;
using Game.Common;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ApplyZonesSystem : GameSystemBase
{
	[BurstCompile]
	private struct HandleTempEntitiesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<Cell> m_ZoneCellType;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public BufferLookup<Cell> m_Cells;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

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
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<Cell> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Cell>(ref m_ZoneCellType);
			if (bufferAccessor.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity entity = nativeArray[i];
					Temp temp = nativeArray2[i];
					if ((temp.m_Flags & TempFlags.Delete) != 0)
					{
						Delete(unfilteredChunkIndex, entity, temp);
					}
					else if (temp.m_Original != Entity.Null)
					{
						Update(unfilteredChunkIndex, entity, temp, bufferAccessor[i]);
					}
					else
					{
						Create(unfilteredChunkIndex, entity);
					}
				}
				return;
			}
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Entity entity2 = nativeArray[j];
				Temp temp2 = nativeArray2[j];
				if ((temp2.m_Flags & TempFlags.Delete) != 0)
				{
					Delete(unfilteredChunkIndex, entity2, temp2);
				}
				else if (temp2.m_Original != Entity.Null)
				{
					Update(unfilteredChunkIndex, entity2, temp2);
				}
				else
				{
					Create(unfilteredChunkIndex, entity2);
				}
			}
		}

		private void Delete(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (m_Cells.HasBuffer(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, temp.m_Original, default(Deleted));
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void Update(int chunkIndex, Entity entity, Temp temp, bool updateOriginal = true)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			if (m_HiddenData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(chunkIndex, temp.m_Original);
				if (!updateOriginal)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(chunkIndex, temp.m_Original, default(BatchesUpdated));
				}
			}
			if (updateOriginal && m_Cells.HasBuffer(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(chunkIndex, temp.m_Original, default(Updated));
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void Update(int chunkIndex, Entity entity, Temp temp, DynamicBuffer<Cell> cells)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (m_Cells.HasBuffer(temp.m_Original))
			{
				DynamicBuffer<Cell> val = m_Cells[temp.m_Original];
				DynamicBuffer<Cell> val2 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<Cell>(chunkIndex, temp.m_Original);
				val2.ResizeUninitialized(val.Length);
				for (int i = 0; i < val.Length; i++)
				{
					Cell cell = cells[i];
					Cell cell2 = val[i];
					if ((cell.m_State & CellFlags.Selected) != CellFlags.None)
					{
						if ((cell2.m_State & CellFlags.Overridden) != CellFlags.None)
						{
							if (cell2.m_Zone.Equals(cell.m_Zone))
							{
								cell2.m_State &= ~CellFlags.Overridden;
							}
						}
						else
						{
							cell2.m_Zone = cell.m_Zone;
						}
					}
					val2[i] = cell2;
				}
			}
			Update(chunkIndex, entity, temp);
		}

		private void Create(int chunkIndex, Entity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Temp>(chunkIndex, entity);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(chunkIndex, entity, ref m_AppliedTypes);
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
		public BufferTypeHandle<Cell> __Game_Zones_Cell_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Cell> __Game_Zones_Cell_RO_BufferLookup;

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
			__Game_Zones_Cell_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Cell>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Zones_Cell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Cell>(true);
		}
	}

	private ToolOutputBarrier m_ToolOutputBarrier;

	private EntityQuery m_TempQuery;

	private ComponentTypeSet m_AppliedTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Block>()
		});
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
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
		HandleTempEntitiesJob handleTempEntitiesJob = new HandleTempEntitiesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneCellType = InternalCompilerInterface.GetBufferTypeHandle<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AppliedTypes = m_AppliedTypes
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
	public ApplyZonesSystem()
	{
	}
}
