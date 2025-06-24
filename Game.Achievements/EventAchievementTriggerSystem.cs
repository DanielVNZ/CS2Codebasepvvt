using System.Runtime.CompilerServices;
using Colossal.PSI.Common;
using Game.Common;
using Game.Events;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Achievements;

[CompilerGenerated]
public class EventAchievementTriggerSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<EventAchievementData> __Game_Prefabs_EventAchievementData_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Duration> __Game_Events_Duration_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_EventAchievementData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EventAchievementData>(true);
			__Game_Events_Duration_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Duration>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private ModificationEndBarrier m_ModifiactionEndBarrier;

	private EntityQuery m_TrackingQuery;

	private EntityQuery m_CreatedEventQuery;

	private EntityArchetype m_TrackingArchetype;

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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ModifiactionEndBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		m_CreatedEventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Events.Event>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<EventAchievement>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_TrackingArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<EventAchievementTrackingData>() });
		m_TrackingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<EventAchievementTrackingData>() });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_CreatedEventQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_CreatedEventQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			BufferLookup<EventAchievementData> bufferLookup = InternalCompilerInterface.GetBufferLookup<EventAchievementData>(ref __TypeHandle.__Game_Prefabs_EventAchievementData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Duration> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabRef> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			EntityCommandBuffer buffer = m_ModifiactionEndBarrier.CreateCommandBuffer();
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				if (((ArchetypeChunk)(ref val2)).Has<Duration>(ref componentTypeHandle))
				{
					val2 = val[i];
					NativeArray<Duration> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<Duration>(ref componentTypeHandle);
					val2 = val[i];
					NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref componentTypeHandle2);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						DynamicBuffer<EventAchievementData> val3 = bufferLookup[nativeArray2[j].m_Prefab];
						for (int k = 0; k < val3.Length; k++)
						{
							StartTracking(val3[k].m_ID, nativeArray[j].m_StartFrame + val3[k].m_FrameDelay, buffer);
						}
					}
					continue;
				}
				val2 = val[i];
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref componentTypeHandle2);
				for (int l = 0; l < nativeArray3.Length; l++)
				{
					DynamicBuffer<EventAchievementData> val4 = bufferLookup[nativeArray3[l].m_Prefab];
					for (int m = 0; m < val4.Length; m++)
					{
						StartTracking(val4[m].m_ID, m_SimulationSystem.frameIndex + val4[m].m_FrameDelay, buffer);
					}
				}
			}
			val.Dispose();
		}
		if (((EntityQuery)(ref m_TrackingQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<EventAchievementTrackingData> val5 = ((EntityQuery)(ref m_TrackingQuery)).ToComponentDataArray<EventAchievementTrackingData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Entity> val6 = ((EntityQuery)(ref m_TrackingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityCommandBuffer buffer2 = m_ModifiactionEndBarrier.CreateCommandBuffer();
		for (int n = 0; n < val5.Length; n++)
		{
			if (m_SimulationSystem.frameIndex > val5[n].m_StartFrame)
			{
				StopTracking(val5[n], val6[n], buffer2);
			}
		}
		val5.Dispose();
		val6.Dispose();
	}

	private void StartTracking(AchievementId id, uint startFrame, EntityCommandBuffer buffer)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Entity val = ((EntityCommandBuffer)(ref buffer)).CreateEntity(m_TrackingArchetype);
		((EntityCommandBuffer)(ref buffer)).SetComponent<EventAchievementTrackingData>(val, new EventAchievementTrackingData
		{
			m_ID = id,
			m_StartFrame = startFrame
		});
	}

	private void StopTracking(EventAchievementTrackingData data, Entity entity, EntityCommandBuffer buffer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		((EntityCommandBuffer)(ref buffer)).AddComponent<Deleted>(entity);
		PlatformManager.instance.UnlockAchievement(data.m_ID);
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
	public EventAchievementTriggerSystem()
	{
	}
}
