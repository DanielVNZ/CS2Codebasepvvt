using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Effects;
using Game.Net;
using Game.Prefabs;
using Game.Serialization;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Objects;

[CompilerGenerated]
public class ContainerClearSystem : GameSystemBase
{
	[BurstCompile]
	private struct ContainerClearJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<EnabledEffect> m_EffectOwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<SubObject> m_SubObjectType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> m_SubNetType;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.SubArea> m_SubAreaType;

		[ReadOnly]
		public BufferLookup<Effect> m_PrefabEffects;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> m_PrefabSubObjects;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> m_PrefabSubNets;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> m_PrefabSubAreas;

		[ReadOnly]
		public ComponentTypeSet m_SubTypes;

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
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<SubObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubObject>(ref m_SubObjectType);
			BufferAccessor<Game.Net.SubNet> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubNet>(ref m_SubNetType);
			BufferAccessor<Game.Areas.SubArea> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Areas.SubArea>(ref m_SubAreaType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<EnabledEffect>(ref m_EffectOwnerType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				PrefabRef prefabRef = nativeArray2[i];
				DynamicBuffer<SubObject> val2 = bufferAccessor[i];
				DynamicBuffer<Game.Net.SubNet> val3 = bufferAccessor2[i];
				DynamicBuffer<Game.Areas.SubArea> val4 = bufferAccessor3[i];
				bool3 val5 = bool3.op_Implicit(false);
				val5.x = val2.Length == 0 && !m_PrefabSubObjects.HasBuffer(prefabRef.m_Prefab);
				val5.y = val3.Length == 0 && !m_PrefabSubNets.HasBuffer(prefabRef.m_Prefab);
				val5.z = val4.Length == 0 && !m_PrefabSubAreas.HasBuffer(prefabRef.m_Prefab);
				if (math.all(val5))
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(unfilteredChunkIndex, val, ref m_SubTypes);
				}
				else
				{
					if (val5.x)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<SubObject>(unfilteredChunkIndex, val);
					}
					if (val5.y)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Net.SubNet>(unfilteredChunkIndex, val);
					}
					if (val5.z)
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Game.Areas.SubArea>(unfilteredChunkIndex, val);
					}
				}
				if (!flag && m_PrefabEffects.HasBuffer(prefabRef.m_Prefab))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<EnabledEffect>(unfilteredChunkIndex, val);
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
		public BufferTypeHandle<EnabledEffect> __Game_Effects_EnabledEffect_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<Effect> __Game_Prefabs_Effect_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> __Game_Prefabs_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Effects_EnabledEffect_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<EnabledEffect>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(true);
			__Game_Net_SubNet_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubNet>(true);
			__Game_Areas_SubArea_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Areas.SubArea>(true);
			__Game_Prefabs_Effect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Effect>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubObject>(true);
			__Game_Prefabs_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubNet>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubArea>(true);
		}
	}

	private LoadGameSystem m_LoadGameSystem;

	private EntityQuery m_EntityQuery;

	private ComponentTypeSet m_SubTypes;

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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_EntityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<SubObject>(),
			ComponentType.ReadOnly<Game.Net.SubNet>(),
			ComponentType.ReadOnly<Game.Areas.SubArea>(),
			ComponentType.ReadOnly<Object>(),
			ComponentType.Exclude<Building>()
		});
		m_SubTypes = new ComponentTypeSet(ComponentType.ReadWrite<SubObject>(), ComponentType.ReadWrite<Game.Net.SubNet>(), ComponentType.ReadWrite<Game.Areas.SubArea>());
		((ComponentSystemBase)this).RequireForUpdate(m_EntityQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		Context context = m_LoadGameSystem.context;
		if ((int)((Context)(ref context)).purpose == 1)
		{
			EntityCommandBuffer val = default(EntityCommandBuffer);
			((EntityCommandBuffer)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<ContainerClearJob>(new ContainerClearJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EffectOwnerType = InternalCompilerInterface.GetBufferTypeHandle<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubNetType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubAreaType = InternalCompilerInterface.GetBufferTypeHandle<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabEffects = InternalCompilerInterface.GetBufferLookup<Effect>(ref __TypeHandle.__Game_Prefabs_Effect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSubObjects = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubTypes = m_SubTypes,
				m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter()
			}, m_EntityQuery, ((SystemBase)this).Dependency);
			((JobHandle)(ref val2)).Complete();
			((EntityCommandBuffer)(ref val)).Playback(((ComponentSystemBase)this).EntityManager);
			((EntityCommandBuffer)(ref val)).Dispose();
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
	public ContainerClearSystem()
	{
	}
}
