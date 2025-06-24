using System.Runtime.CompilerServices;
using Game.Common;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CalendarEventLaunchSystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckEventLaunchJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CalendarEventData> m_CalendarEventType;

		[ReadOnly]
		public ComponentTypeHandle<EventData> m_EventType;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		public CalendarEventMonths m_Month;

		public CalendarEventTimes m_Time;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CalendarEventData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CalendarEventData>(ref m_CalendarEventType);
			NativeArray<EventData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EventData>(ref m_EventType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity prefab = nativeArray[i];
				CalendarEventData calendarEventData = nativeArray2[i];
				EventData eventData = nativeArray3[i];
				if ((m_Month & calendarEventData.m_AllowedMonths) != 0 && (m_Time & calendarEventData.m_AllowedTimes) != 0 && (float)((Random)(ref random)).NextInt(100) < calendarEventData.m_OccurenceProbability.min)
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, eventData.m_Archetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(unfilteredChunkIndex, val, new PrefabRef(prefab));
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
		public ComponentTypeHandle<EventData> __Game_Prefabs_EventData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CalendarEventData> __Game_Prefabs_CalendarEventData_RO_ComponentTypeHandle;

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
			__Game_Prefabs_EventData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EventData>(true);
			__Game_Prefabs_CalendarEventData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CalendarEventData>(true);
		}
	}

	private const int UPDATES_PER_DAY = 4;

	private EndFrameBarrier m_EndFrameBarrier;

	private TimeSystem m_TimeSystem;

	private EntityQuery m_CalendarEventQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 65536;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CalendarEventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CalendarEventData>() });
		((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_CalendarEventQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		CalendarEventMonths month = (CalendarEventMonths)(1 << Mathf.FloorToInt(m_TimeSystem.normalizedDate * 12f));
		CalendarEventTimes time = (CalendarEventTimes)(1 << Mathf.FloorToInt(m_TimeSystem.normalizedTime * 4f));
		CheckEventLaunchJob checkEventLaunchJob = new CheckEventLaunchJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EventType = InternalCompilerInterface.GetComponentTypeHandle<EventData>(ref __TypeHandle.__Game_Prefabs_EventData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CalendarEventType = InternalCompilerInterface.GetComponentTypeHandle<CalendarEventData>(ref __TypeHandle.__Game_Prefabs_CalendarEventData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Month = month,
			m_Time = time,
			m_RandomSeed = RandomSeed.Next()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		checkEventLaunchJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		CheckEventLaunchJob checkEventLaunchJob2 = checkEventLaunchJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckEventLaunchJob>(checkEventLaunchJob2, m_CalendarEventQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public CalendarEventLaunchSystem()
	{
	}
}
