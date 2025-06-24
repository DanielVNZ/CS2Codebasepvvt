using System.Runtime.CompilerServices;
using Colossal.Collections;
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
public class AddAccidentSiteSystem : GameSystemBase
{
	[BurstCompile]
	private struct AddAccidentSiteJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<AddAccidentSite> m_AddAccidentSiteType;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		public ComponentLookup<CrimeProducer> m_CrimeProducerData;

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
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelHashMap<Entity, AccidentSite> val2 = default(NativeParallelHashMap<Entity, AccidentSite>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			AccidentSite accidentSite2 = default(AccidentSite);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val3 = m_Chunks[j];
				NativeArray<AddAccidentSite> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<AddAccidentSite>(ref m_AddAccidentSiteType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					AddAccidentSite addAccidentSite = nativeArray[k];
					if (m_PrefabRefData.HasComponent(addAccidentSite.m_Target))
					{
						AccidentSite accidentSite = new AccidentSite(addAccidentSite.m_Event, addAccidentSite.m_Flags, m_SimulationFrame);
						if (val2.TryGetValue(addAccidentSite.m_Target, ref accidentSite2))
						{
							val2[addAccidentSite.m_Target] = MergeAccidentSites(accidentSite2, accidentSite);
						}
						else if (m_AccidentSiteData.HasComponent(addAccidentSite.m_Target))
						{
							accidentSite2 = m_AccidentSiteData[addAccidentSite.m_Target];
							val2.TryAdd(addAccidentSite.m_Target, MergeAccidentSites(accidentSite2, accidentSite));
						}
						else
						{
							val2.TryAdd(addAccidentSite.m_Target, accidentSite);
						}
						if ((accidentSite.m_Flags & AccidentSiteFlags.CrimeScene) != 0 && m_CrimeProducerData.HasComponent(addAccidentSite.m_Target))
						{
							CrimeProducer crimeProducer = m_CrimeProducerData[addAccidentSite.m_Target];
							crimeProducer.m_Crime *= 0.3f;
							m_CrimeProducerData[addAccidentSite.m_Target] = crimeProducer;
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
				AccidentSite accidentSite3 = val2[val4];
				if (m_AccidentSiteData.HasComponent(val4))
				{
					if (m_AccidentSiteData[val4].m_Event != accidentSite3.m_Event && m_TargetElements.HasBuffer(accidentSite3.m_Event))
					{
						CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[accidentSite3.m_Event], new TargetElement(val4));
					}
					m_AccidentSiteData[val4] = accidentSite3;
				}
				else
				{
					if (m_TargetElements.HasBuffer(accidentSite3.m_Event))
					{
						CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[accidentSite3.m_Event], new TargetElement(val4));
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<AccidentSite>(val4, accidentSite3);
				}
			}
		}

		private AccidentSite MergeAccidentSites(AccidentSite accidentSite1, AccidentSite accidentSite2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			AccidentSite result;
			if (accidentSite1.m_Event != Entity.Null != (accidentSite2.m_Event != Entity.Null))
			{
				result = ((accidentSite1.m_Event != Entity.Null) ? accidentSite1 : accidentSite2);
				result.m_Flags |= ((accidentSite1.m_Event != Entity.Null) ? accidentSite2.m_Flags : accidentSite1.m_Flags);
			}
			else
			{
				result = accidentSite1;
				result.m_Flags |= accidentSite2.m_Flags;
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<AddAccidentSite> __Game_Events_AddAccidentSite_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RW_ComponentLookup;

		public ComponentLookup<CrimeProducer> __Game_Buildings_CrimeProducer_RW_ComponentLookup;

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
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_AddAccidentSite_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AddAccidentSite>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Events_AccidentSite_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(false);
			__Game_Buildings_CrimeProducer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeProducer>(false);
			__Game_Events_TargetElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_ImpactQuery;

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
		m_ImpactQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<AddAccidentSite>(),
			ComponentType.ReadOnly<Game.Common.Event>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ImpactQuery);
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
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_ImpactQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<AddAccidentSiteJob>(new AddAccidentSiteJob
		{
			m_Chunks = chunks,
			m_AddAccidentSiteType = InternalCompilerInterface.GetComponentTypeHandle<AddAccidentSite>(ref __TypeHandle.__Game_Events_AddAccidentSite_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteData = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeProducerData = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
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
	public AddAccidentSiteSystem()
	{
	}
}
