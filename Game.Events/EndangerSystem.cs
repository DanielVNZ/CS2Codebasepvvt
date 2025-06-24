using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Events;

[CompilerGenerated]
public class EndangerSystem : GameSystemBase
{
	[BurstCompile]
	private struct EndangerJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public ComponentTypeHandle<Endanger> m_EndangerType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.School> m_SchoolData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> m_HospitalData;

		public ComponentLookup<InDanger> m_InDangerData;

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
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelHashMap<Entity, InDanger> val2 = default(NativeParallelHashMap<Entity, InDanger>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			InDanger inDanger2 = default(InDanger);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val3 = m_Chunks[j];
				NativeArray<Endanger> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<Endanger>(ref m_EndangerType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Endanger endanger = nativeArray[k];
					if (!m_PrefabRefData.HasComponent(endanger.m_Target))
					{
						continue;
					}
					if ((endanger.m_Flags & DangerFlags.Evacuate) != 0 && (m_SchoolData.HasComponent(endanger.m_Target) || m_HospitalData.HasComponent(endanger.m_Target)))
					{
						endanger.m_Flags |= DangerFlags.UseTransport;
					}
					InDanger inDanger = new InDanger(endanger.m_Event, Entity.Null, endanger.m_Flags, endanger.m_EndFrame);
					if (val2.TryGetValue(endanger.m_Target, ref inDanger2))
					{
						if (EventUtils.IsWorse(inDanger.m_Flags, inDanger2.m_Flags) || inDanger2.m_EndFrame < m_SimulationFrame + 64)
						{
							val2[endanger.m_Target] = inDanger;
						}
					}
					else if (m_InDangerData.HasComponent(endanger.m_Target))
					{
						inDanger2 = m_InDangerData[endanger.m_Target];
						if (EventUtils.IsWorse(inDanger.m_Flags, inDanger2.m_Flags) || inDanger2.m_EndFrame < m_SimulationFrame + 64)
						{
							val2.TryAdd(endanger.m_Target, inDanger);
						}
					}
					else
					{
						val2.TryAdd(endanger.m_Target, inDanger);
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
				InDanger inDanger3 = val2[val4];
				if (m_InDangerData.HasComponent(val4))
				{
					InDanger inDanger4 = m_InDangerData[val4];
					m_InDangerData[val4] = inDanger3;
					if (inDanger3.m_Flags != inDanger4.m_Flags)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(val4, default(EffectsUpdated));
					}
				}
				else
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InDanger>(val4, inDanger3);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<EffectsUpdated>(val4, default(EffectsUpdated));
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Endanger> __Game_Events_Endanger_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.School> __Game_Buildings_School_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> __Game_Buildings_Hospital_RO_ComponentLookup;

		public ComponentLookup<InDanger> __Game_Events_InDanger_RW_ComponentLookup;

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
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_Endanger_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Endanger>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Buildings_School_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.School>(true);
			__Game_Buildings_Hospital_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Hospital>(true);
			__Game_Events_InDanger_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InDanger>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_EndangerQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_EndangerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Endanger>(),
			ComponentType.ReadOnly<Game.Common.Event>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_EndangerQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_EndangerQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<EndangerJob>(new EndangerJob
		{
			m_Chunks = chunks,
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_EndangerType = InternalCompilerInterface.GetComponentTypeHandle<Endanger>(ref __TypeHandle.__Game_Events_Endanger_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SchoolData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.School>(ref __TypeHandle.__Game_Buildings_School_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HospitalData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Hospital>(ref __TypeHandle.__Game_Buildings_Hospital_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InDangerData = InternalCompilerInterface.GetComponentLookup<InDanger>(ref __TypeHandle.__Game_Events_InDanger_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
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
	public EndangerSystem()
	{
	}
}
