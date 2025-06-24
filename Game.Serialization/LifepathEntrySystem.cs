using System.Runtime.CompilerServices;
using Game.Common;
using Game.Triggers;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class LifepathEntrySystem : GameSystemBase
{
	public struct FixLifepathChirpReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Chirp> m_ChirpType;

		[ReadOnly]
		public BufferLookup<LifePathEntry> m_EntryDatas;

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
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityTypeHandle);
			NativeArray<Chirp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Chirp>(ref m_ChirpType);
			DynamicBuffer<LifePathEntry> entries = default(DynamicBuffer<LifePathEntry>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Entity sender = nativeArray2[i].m_Sender;
				if (m_EntryDatas.TryGetBuffer(sender, ref entries))
				{
					if (!Contains(entries, val))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AppendToBuffer<LifePathEntry>(unfilteredChunkIndex, sender, new LifePathEntry(val));
					}
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val);
				}
			}
		}

		private bool Contains(DynamicBuffer<LifePathEntry> entries, Entity chirp)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < entries.Length; i++)
			{
				if (entries[i].m_Entity == chirp)
				{
					return true;
				}
			}
			return false;
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
		public ComponentTypeHandle<Chirp> __Game_Triggers_Chirp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<LifePathEntry> __Game_Triggers_LifePathEntry_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Triggers_Chirp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Chirp>(true);
			__Game_Triggers_LifePathEntry_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LifePathEntry>(true);
		}
	}

	private EntityQuery m_ChirpQuery;

	private DeserializationBarrier m_DeserializationBarrier;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ChirpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Chirp>(),
			ComponentType.ReadOnly<LifePathEvent>()
		});
		m_DeserializationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DeserializationBarrier>();
		((ComponentSystemBase)this).RequireForUpdate(m_ChirpQuery);
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
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		FixLifepathChirpReferencesJob fixLifepathChirpReferencesJob = new FixLifepathChirpReferencesJob
		{
			m_EntityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ChirpType = InternalCompilerInterface.GetComponentTypeHandle<Chirp>(ref __TypeHandle.__Game_Triggers_Chirp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntryDatas = InternalCompilerInterface.GetBufferLookup<LifePathEntry>(ref __TypeHandle.__Game_Triggers_LifePathEntry_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_DeserializationBarrier.CreateCommandBuffer();
		fixLifepathChirpReferencesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		FixLifepathChirpReferencesJob fixLifepathChirpReferencesJob2 = fixLifepathChirpReferencesJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<FixLifepathChirpReferencesJob>(fixLifepathChirpReferencesJob2, m_ChirpQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_DeserializationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public LifepathEntrySystem()
	{
	}
}
