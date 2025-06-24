using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Citizens;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Events;

[CompilerGenerated]
public class AddCriminalSystem : GameSystemBase
{
	[BurstCompile]
	private struct AddCriminalJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<AddCriminal> m_AddCriminalType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<Criminal> m_Criminals;

		public BufferLookup<TargetElement> m_TargetElements;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelHashMap<Entity, Criminal> val2 = default(NativeParallelHashMap<Entity, Criminal>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			Criminal criminal2 = default(Criminal);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val3 = m_Chunks[j];
				NativeArray<AddCriminal> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<AddCriminal>(ref m_AddCriminalType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					AddCriminal addCriminal = nativeArray[k];
					if (m_PrefabRefData.HasComponent(addCriminal.m_Target))
					{
						Criminal criminal = new Criminal(addCriminal.m_Event, addCriminal.m_Flags);
						if (val2.TryGetValue(addCriminal.m_Target, ref criminal2))
						{
							val2[addCriminal.m_Target] = MergeCriminals(criminal2, criminal);
						}
						else if (m_Criminals.HasComponent(addCriminal.m_Target))
						{
							criminal2 = m_Criminals[addCriminal.m_Target];
							val2.TryAdd(addCriminal.m_Target, MergeCriminals(criminal2, criminal));
						}
						else
						{
							val2.TryAdd(addCriminal.m_Target, criminal);
						}
					}
				}
			}
			if (val2.Count() == 0)
			{
				return;
			}
			NativeArray<Entity> keyArray = val2.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
			for (int l = 0; l < keyArray.Length; l++)
			{
				Entity val4 = keyArray[l];
				Criminal criminal3 = val2[val4];
				if (m_Criminals.HasComponent(val4))
				{
					if (m_Criminals[val4].m_Event != criminal3.m_Event && m_TargetElements.HasBuffer(criminal3.m_Event))
					{
						CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[criminal3.m_Event], new TargetElement(val4));
					}
					m_Criminals[val4] = criminal3;
				}
				else
				{
					if (m_TargetElements.HasBuffer(criminal3.m_Event))
					{
						CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[criminal3.m_Event], new TargetElement(val4));
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Criminal>(val4, criminal3);
				}
			}
		}

		private Criminal MergeCriminals(Criminal criminal1, Criminal criminal2)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			if (((criminal1.m_Flags ^ criminal2.m_Flags) & CriminalFlags.Prisoner) != 0)
			{
				if ((criminal1.m_Flags & CriminalFlags.Prisoner) == 0)
				{
					return criminal2;
				}
				return criminal1;
			}
			Criminal result;
			if (criminal1.m_Event != Entity.Null != (criminal2.m_Event != Entity.Null))
			{
				result = ((criminal1.m_Event != Entity.Null) ? criminal1 : criminal2);
				result.m_Flags |= ((criminal1.m_Event != Entity.Null) ? criminal2.m_Flags : criminal1.m_Flags);
			}
			else
			{
				result = criminal1;
				result.m_Flags |= criminal2.m_Flags;
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<AddCriminal> __Game_Events_AddCriminal_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		public ComponentLookup<Criminal> __Game_Citizens_Criminal_RW_ComponentLookup;

		public BufferLookup<TargetElement> __Game_Events_TargetElement_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_AddCriminal_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AddCriminal>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Citizens_Criminal_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Criminal>(false);
			__Game_Events_TargetElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(false);
		}
	}

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_AddCriminalQuery;

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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_AddCriminalQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Common.Event>(),
			ComponentType.ReadOnly<AddCriminal>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_AddCriminalQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_AddCriminalQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<AddCriminalJob>(new AddCriminalJob
		{
			m_Chunks = chunks,
			m_AddCriminalType = InternalCompilerInterface.GetComponentTypeHandle<AddCriminal>(ref __TypeHandle.__Game_Events_AddCriminal_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Criminals = InternalCompilerInterface.GetComponentLookup<Criminal>(ref __TypeHandle.__Game_Citizens_Criminal_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		chunks.Dispose(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
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
	public AddCriminalSystem()
	{
	}
}
