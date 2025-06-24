using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Notifications;
using Game.Objects;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Serialization.DataMigration;

[CompilerGenerated]
public class PlaceholderCleanupSystem : GameSystemBase
{
	[BurstCompile]
	private struct PlaceholderCleanupJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<IconElement> m_IconElementType;

		[ReadOnly]
		public ComponentTypeSet m_ComponentSet;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<IconElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<IconElement>(ref m_IconElementType);
			DynamicBuffer<IconElement> val = default(DynamicBuffer<IconElement>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, nativeArray[i], ref m_ComponentSet);
				if (CollectionUtils.TryGet<IconElement>(bufferAccessor, i, ref val))
				{
					for (int j = 0; j < val.Length; j++)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val[j].m_Icon, default(Deleted));
					}
				}
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
		public BufferTypeHandle<IconElement> __Game_Notifications_IconElement_RO_BufferTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Notifications_IconElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<IconElement>(true);
		}
	}

	private LoadGameSystem m_LoadGameSystem;

	private DeserializationBarrier m_DeserializationBarrier;

	private EntityQuery m_Query;

	private ComponentTypeSet m_ComponentSet;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_DeserializationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DeserializationBarrier>();
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Placeholder>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Renter>()
		});
		m_ComponentSet = new ComponentTypeSet((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadWrite<Renter>(),
			ComponentType.ReadWrite<PropertyToBeOnMarket>(),
			ComponentType.ReadWrite<PropertyOnMarket>(),
			ComponentType.ReadWrite<ElectricityConsumer>(),
			ComponentType.ReadWrite<WaterConsumer>(),
			ComponentType.ReadWrite<GarbageProducer>(),
			ComponentType.ReadWrite<TelecomConsumer>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		Context context = m_LoadGameSystem.context;
		if (!(((Context)(ref context)).version >= Version.placeholderCleanup) && !((EntityQuery)(ref m_Query)).IsEmptyIgnoreFilter)
		{
			PlaceholderCleanupJob placeholderCleanupJob = new PlaceholderCleanupJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_IconElementType = InternalCompilerInterface.GetBufferTypeHandle<IconElement>(ref __TypeHandle.__Game_Notifications_IconElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ComponentSet = m_ComponentSet
			};
			EntityCommandBuffer val = m_DeserializationBarrier.CreateCommandBuffer();
			placeholderCleanupJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<PlaceholderCleanupJob>(placeholderCleanupJob, m_Query, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_DeserializationBarrier).AddJobHandleForProducer(val2);
			((SystemBase)this).Dependency = val2;
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
	public PlaceholderCleanupSystem()
	{
	}
}
