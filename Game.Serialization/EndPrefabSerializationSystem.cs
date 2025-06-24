using System.Runtime.CompilerServices;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class EndPrefabSerializationSystem : GameSystemBase
{
	[BurstCompile]
	private struct EndPrefabSerializationJob : IJobChunk
	{
		[ReadOnly]
		public BufferTypeHandle<LoadedIndex> m_LoadedIndexType;

		public ComponentTypeHandle<PrefabData> m_PrefabDataType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
			BufferAccessor<LoadedIndex> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LoadedIndex>(ref m_LoadedIndexType);
			EnabledMask enabledMask = ((ArchetypeChunk)(ref chunk)).GetEnabledMask<PrefabData>(ref m_PrefabDataType);
			PrefabData prefabData = default(PrefabData);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (((EnabledMask)(ref enabledMask))[i])
				{
					prefabData.m_Index = bufferAccessor[i][0].m_Index;
					nativeArray[i] = prefabData;
				}
				else
				{
					prefabData = nativeArray[i];
				}
				((EnabledMask)(ref enabledMask))[i] = prefabData.m_Index >= 0;
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
		public BufferTypeHandle<LoadedIndex> __Game_Prefabs_LoadedIndex_RO_BufferTypeHandle;

		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RW_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_LoadedIndex_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LoadedIndex>(true);
			__Game_Prefabs_PrefabData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(false);
		}
	}

	private SaveGameSystem m_SaveGameSystem;

	private EntityQuery m_LoadedPrefabsQuery;

	private EntityQuery m_ContentPrefabQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SaveGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SaveGameSystem>();
		m_LoadedPrefabsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LoadedIndex>() });
		m_ContentPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ContentData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (m_SaveGameSystem.referencedContent.IsCreated)
		{
			m_SaveGameSystem.referencedContent.Dispose();
		}
		m_SaveGameSystem.referencedContent = ((EntityQuery)(ref m_ContentPrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)4));
		JobHandle val = JobChunkExtensions.ScheduleParallel<EndPrefabSerializationJob>(new EndPrefabSerializationJob
		{
			m_LoadedIndexType = InternalCompilerInterface.GetBufferTypeHandle<LoadedIndex>(ref __TypeHandle.__Game_Prefabs_LoadedIndex_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDataType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, m_LoadedPrefabsQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
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
	public EndPrefabSerializationSystem()
	{
	}
}
