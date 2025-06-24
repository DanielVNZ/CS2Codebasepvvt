using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
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
public class IgniteSystem : GameSystemBase
{
	[BurstCompile]
	private struct IgniteFireJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Ignite> m_IgniteType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		public ComponentLookup<OnFire> m_OnFireData;

		public BufferLookup<TargetElement> m_TargetElements;

		public EntityArchetype m_JournalDataArchetype;

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
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelHashMap<Entity, OnFire> val2 = default(NativeParallelHashMap<Entity, OnFire>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			OnFire onFire2 = default(OnFire);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val3 = m_Chunks[j];
				NativeArray<Ignite> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<Ignite>(ref m_IgniteType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Ignite ignite = nativeArray[k];
					if (!m_PrefabRefData.HasComponent(ignite.m_Target))
					{
						continue;
					}
					OnFire onFire = new OnFire(ignite.m_Event, ignite.m_Intensity, ignite.m_RequestFrame);
					if (val2.TryGetValue(ignite.m_Target, ref onFire2))
					{
						if (onFire.m_Intensity > onFire2.m_Intensity)
						{
							val2[ignite.m_Target] = onFire;
						}
					}
					else if (m_OnFireData.HasComponent(ignite.m_Target))
					{
						onFire2 = m_OnFireData[ignite.m_Target];
						if (onFire.m_Intensity > onFire2.m_Intensity)
						{
							val2.TryAdd(ignite.m_Target, onFire);
						}
					}
					else
					{
						val2.TryAdd(ignite.m_Target, onFire);
					}
				}
			}
			if (val2.Count() == 0)
			{
				return;
			}
			NativeArray<Entity> keyArray = val2.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
			DynamicBuffer<InstalledUpgrade> val5 = default(DynamicBuffer<InstalledUpgrade>);
			for (int l = 0; l < keyArray.Length; l++)
			{
				Entity val4 = keyArray[l];
				OnFire onFire3 = val2[val4];
				if (m_OnFireData.HasComponent(val4))
				{
					OnFire onFire4 = m_OnFireData[val4];
					if (onFire4.m_Event != onFire3.m_Event)
					{
						if (m_TargetElements.HasBuffer(onFire3.m_Event))
						{
							CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[onFire3.m_Event], new TargetElement(val4));
						}
						AddJournalData(val4, onFire3);
					}
					if (onFire4.m_RequestFrame < onFire3.m_RequestFrame)
					{
						onFire3.m_RequestFrame = onFire4.m_RequestFrame;
					}
					onFire3.m_RescueRequest = onFire4.m_RescueRequest;
					m_OnFireData[val4] = onFire3;
					continue;
				}
				if (m_TargetElements.HasBuffer(onFire3.m_Event))
				{
					CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[onFire3.m_Event], new TargetElement(val4));
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OnFire>(val4, onFire3);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(val4, default(BatchesUpdated));
				AddJournalData(val4, onFire3);
				if (!m_InstalledUpgrades.TryGetBuffer(val4, ref val5))
				{
					continue;
				}
				for (int m = 0; m < val5.Length; m++)
				{
					Entity upgrade = val5[m].m_Upgrade;
					if (!m_BuildingData.HasComponent(upgrade))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(upgrade);
					}
				}
			}
		}

		private void AddJournalData(Entity target, OnFire onFire)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (m_BuildingData.HasComponent(target))
			{
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(m_JournalDataArchetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<AddEventJournalData>(val, new AddEventJournalData(onFire.m_Event, EventDataTrackingType.Damages));
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Ignite> __Game_Events_Ignite_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		public ComponentLookup<OnFire> __Game_Events_OnFire_RW_ComponentLookup;

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
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_Ignite_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Ignite>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Events_OnFire_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(false);
			__Game_Events_TargetElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(false);
		}
	}

	private ModificationBarrier4 m_ModificationBarrier;

	private EntityQuery m_IgniteQuery;

	private EntityArchetype m_JournalDataArchetype;

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
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_IgniteQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Ignite>(),
			ComponentType.ReadOnly<Game.Common.Event>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_JournalDataArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<AddEventJournalData>(),
			ComponentType.ReadWrite<Game.Common.Event>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_IgniteQuery);
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
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_IgniteQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<IgniteFireJob>(new IgniteFireJob
		{
			m_Chunks = chunks,
			m_IgniteType = InternalCompilerInterface.GetComponentTypeHandle<Ignite>(ref __TypeHandle.__Game_Events_Ignite_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_JournalDataArchetype = m_JournalDataArchetype,
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
	public IgniteSystem()
	{
	}
}
