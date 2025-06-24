using System.Runtime.CompilerServices;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class GenerateAggregatesSystem : GameSystemBase
{
	[BurstCompile]
	private struct CreateAggregatesJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> m_CreationDefinitionType;

		[ReadOnly]
		public BufferTypeHandle<AggregateElement> m_AggregateElementType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AggregateNetData> m_AggregateData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DefinitionChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DeletedChunks;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelMultiHashMap<Entity, Entity> deletedAggregates = default(NativeParallelMultiHashMap<Entity, Entity>);
			deletedAggregates._002Ector(16, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_DeletedChunks.Length; i++)
			{
				FillDeletedAggregates(m_DeletedChunks[i], deletedAggregates);
			}
			for (int j = 0; j < m_DefinitionChunks.Length; j++)
			{
				CreateAggregates(m_DefinitionChunks[j], deletedAggregates);
			}
			deletedAggregates.Dispose();
		}

		private void FillDeletedAggregates(ArchetypeChunk chunk, NativeParallelMultiHashMap<Entity, Entity> deletedAggregates)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				deletedAggregates.Add(nativeArray2[i].m_Prefab, val);
			}
		}

		private void CreateAggregates(ArchetypeChunk chunk, NativeParallelMultiHashMap<Entity, Entity> deletedAggregates)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CreationDefinition> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CreationDefinition>(ref m_CreationDefinitionType);
			BufferAccessor<AggregateElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AggregateElement>(ref m_AggregateElementType);
			Entity val2 = default(Entity);
			NativeParallelMultiHashMapIterator<Entity> val3 = default(NativeParallelMultiHashMapIterator<Entity>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				CreationDefinition creationDefinition = nativeArray[i];
				DynamicBuffer<AggregateElement> val = bufferAccessor[i];
				TempFlags tempFlags = (TempFlags)0u;
				if (creationDefinition.m_Original != Entity.Null)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Hidden>(creationDefinition.m_Original, default(Hidden));
					creationDefinition.m_Prefab = m_PrefabRefData[creationDefinition.m_Original].m_Prefab;
					if ((creationDefinition.m_Flags & CreationFlags.Delete) != 0)
					{
						tempFlags |= TempFlags.Delete;
					}
					else if ((creationDefinition.m_Flags & CreationFlags.Select) != 0)
					{
						tempFlags |= TempFlags.Select;
					}
					else if ((creationDefinition.m_Flags & CreationFlags.Relocate) != 0)
					{
						tempFlags |= TempFlags.Modify;
					}
				}
				else
				{
					tempFlags |= TempFlags.Create;
				}
				tempFlags |= TempFlags.Essential;
				if (deletedAggregates.TryGetFirstValue(creationDefinition.m_Prefab, ref val2, ref val3))
				{
					deletedAggregates.Remove(val3);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Temp>(val2, new Temp(creationDefinition.m_Original, tempFlags));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val2);
				}
				else
				{
					AggregateNetData aggregateNetData = m_AggregateData[creationDefinition.m_Prefab];
					val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(aggregateNetData.m_Archetype);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val2, new PrefabRef(creationDefinition.m_Prefab));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val2, new Temp(creationDefinition.m_Original, tempFlags));
				}
				DynamicBuffer<AggregateElement> val4 = ((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<AggregateElement>(val2);
				val4.ResizeUninitialized(val.Length);
				for (int j = 0; j < val.Length; j++)
				{
					AggregateElement aggregateElement = val[j];
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Highlighted>(aggregateElement.m_Edge, default(Highlighted));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(aggregateElement.m_Edge, default(BatchesUpdated));
					val4[j] = aggregateElement;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreationDefinition> __Game_Tools_CreationDefinition_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<AggregateElement> __Game_Net_AggregateElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AggregateNetData> __Game_Prefabs_AggregateNetData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Tools_CreationDefinition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreationDefinition>(true);
			__Game_Net_AggregateElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AggregateElement>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_AggregateNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AggregateNetData>(true);
		}
	}

	private ModificationBarrier1 m_ModificationBarrier;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_DeletedQuery;

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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier1>();
		m_DefinitionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CreationDefinition>(),
			ComponentType.ReadOnly<AggregateElement>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_DeletedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<AggregateElement>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_DefinitionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> definitionChunks = ((EntityQuery)(ref m_DefinitionQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> deletedChunks = ((EntityQuery)(ref m_DeletedQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		JobHandle val3 = IJobExtensions.Schedule<CreateAggregatesJob>(new CreateAggregatesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreationDefinitionType = InternalCompilerInterface.GetComponentTypeHandle<CreationDefinition>(ref __TypeHandle.__Game_Tools_CreationDefinition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AggregateElementType = InternalCompilerInterface.GetBufferTypeHandle<AggregateElement>(ref __TypeHandle.__Game_Net_AggregateElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AggregateData = InternalCompilerInterface.GetComponentLookup<AggregateNetData>(ref __TypeHandle.__Game_Prefabs_AggregateNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DefinitionChunks = definitionChunks,
			m_DeletedChunks = deletedChunks,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, val2));
		definitionChunks.Dispose(val3);
		deletedChunks.Dispose(val3);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val3;
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
	public GenerateAggregatesSystem()
	{
	}
}
