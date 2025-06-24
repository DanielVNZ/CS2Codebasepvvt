using System.Runtime.CompilerServices;
using Game.Citizens;
using Game.Common;
using Game.Events;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class EventTickSystem : GameSystemBase
{
	[BurstCompile]
	private struct EventTickJob : IJobChunk
	{
		[ReadOnly]
		public uint m_SimulationFrame;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Duration> m_DurationType;

		public BufferTypeHandle<TargetElement> m_TargetElementType;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidentData;

		[ReadOnly]
		public ComponentLookup<FacingWeather> m_FacingWeatherData;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<SpectatorSite> m_SpectatorSiteData;

		[ReadOnly]
		public ComponentLookup<Criminal> m_CriminalData;

		[ReadOnly]
		public ComponentLookup<Flooded> m_FloodedData;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> m_Attendings;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Duration> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Duration>(ref m_DurationType);
			BufferAccessor<TargetElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TargetElement>(ref m_TargetElementType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				DynamicBuffer<TargetElement> val2 = bufferAccessor[i];
				int num = 0;
				while (num < val2.Length)
				{
					TargetElement targetElement = val2[num++];
					if ((!m_OnFireData.HasComponent(targetElement.m_Entity) || !(m_OnFireData[targetElement.m_Entity].m_Event == val)) && (!m_AccidentSiteData.HasComponent(targetElement.m_Entity) || !(m_AccidentSiteData[targetElement.m_Entity].m_Event == val)) && (!m_InvolvedInAccidentData.HasComponent(targetElement.m_Entity) || !(m_InvolvedInAccidentData[targetElement.m_Entity].m_Event == val)) && (!m_FacingWeatherData.HasComponent(targetElement.m_Entity) || !(m_FacingWeatherData[targetElement.m_Entity].m_Event == val)) && (!m_HealthProblemData.HasComponent(targetElement.m_Entity) || !(m_HealthProblemData[targetElement.m_Entity].m_Event == val)) && (!m_DestroyedData.HasComponent(targetElement.m_Entity) || !(m_DestroyedData[targetElement.m_Entity].m_Event == val)) && (!m_SpectatorSiteData.HasComponent(targetElement.m_Entity) || !(m_SpectatorSiteData[targetElement.m_Entity].m_Event == val)) && (!m_CriminalData.HasComponent(targetElement.m_Entity) || !(m_CriminalData[targetElement.m_Entity].m_Event == val)) && (!m_FloodedData.HasComponent(targetElement.m_Entity) || !(m_FloodedData[targetElement.m_Entity].m_Event == val)) && (!m_Attendings.HasComponent(targetElement.m_Entity) || !(m_Attendings[targetElement.m_Entity].m_Meeting == val)))
					{
						val2[--num] = val2[val2.Length - 1];
						val2.RemoveAt(val2.Length - 1);
					}
				}
				if (val2.Length == 0 && (nativeArray2.Length == 0 || nativeArray2[i].m_EndFrame <= m_SimulationFrame))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
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
		public ComponentTypeHandle<Duration> __Game_Events_Duration_RO_ComponentTypeHandle;

		public BufferTypeHandle<TargetElement> __Game_Events_TargetElement_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<FacingWeather> __Game_Events_FacingWeather_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpectatorSite> __Game_Events_SpectatorSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Criminal> __Game_Citizens_Criminal_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Flooded> __Game_Events_Flooded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AttendingMeeting> __Game_Citizens_AttendingMeeting_RO_ComponentLookup;

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
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Events_Duration_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Duration>(true);
			__Game_Events_TargetElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TargetElement>(false);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Events_AccidentSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(true);
			__Game_Events_InvolvedInAccident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InvolvedInAccident>(true);
			__Game_Events_FacingWeather_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FacingWeather>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Events_SpectatorSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpectatorSite>(true);
			__Game_Citizens_Criminal_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Criminal>(true);
			__Game_Events_Flooded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Flooded>(true);
			__Game_Citizens_AttendingMeeting_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AttendingMeeting>(true);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_EventQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

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
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Events.Event>(),
			ComponentType.ReadWrite<TargetElement>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_EventQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		EventTickJob eventTickJob = new EventTickJob
		{
			m_SimulationFrame = m_SimulationSystem.frameIndex
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		eventTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		eventTickJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_DurationType = InternalCompilerInterface.GetComponentTypeHandle<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_TargetElementType = InternalCompilerInterface.GetBufferTypeHandle<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_AccidentSiteData = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_InvolvedInAccidentData = InternalCompilerInterface.GetComponentLookup<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_FacingWeatherData = InternalCompilerInterface.GetComponentLookup<FacingWeather>(ref __TypeHandle.__Game_Events_FacingWeather_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_HealthProblemData = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_SpectatorSiteData = InternalCompilerInterface.GetComponentLookup<SpectatorSite>(ref __TypeHandle.__Game_Events_SpectatorSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_CriminalData = InternalCompilerInterface.GetComponentLookup<Criminal>(ref __TypeHandle.__Game_Citizens_Criminal_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_FloodedData = InternalCompilerInterface.GetComponentLookup<Flooded>(ref __TypeHandle.__Game_Events_Flooded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		eventTickJob.m_Attendings = InternalCompilerInterface.GetComponentLookup<AttendingMeeting>(ref __TypeHandle.__Game_Citizens_AttendingMeeting_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		EventTickJob eventTickJob2 = eventTickJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<EventTickJob>(eventTickJob2, m_EventQuery, ((SystemBase)this).Dependency);
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
	public EventTickSystem()
	{
	}
}
