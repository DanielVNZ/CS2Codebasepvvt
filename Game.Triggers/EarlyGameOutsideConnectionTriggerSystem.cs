using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Triggers;

[CompilerGenerated]
public class EarlyGameOutsideConnectionTriggerSystem : GameSystemBase
{
	[BurstCompile]
	private struct TriggerJob : IJob
	{
		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<Building> m_Buildings;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_AvailabilityDatas;

		public NativeQueue<TriggerAction> m_ActionBuffer;

		public void Execute()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Buildings.Length; i++)
			{
				Building building = m_Buildings[i];
				if (building.m_RoadEdge != Entity.Null && m_AvailabilityDatas.HasBuffer(building.m_RoadEdge) && NetUtils.GetAvailability(m_AvailabilityDatas[building.m_RoadEdge], AvailableResource.OutsideConnection, building.m_CurvePosition) <= 0f)
				{
					m_ActionBuffer.Enqueue(new TriggerAction(TriggerType.NoOutsideConnection, Entity.Null, 0f));
					break;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
		}
	}

	private const uint UPDATE_INTERVAL = 64u;

	private static readonly float kDelaySeconds = 10f;

	private EntityQuery m_BuildingQuery;

	private TriggerSystem m_TriggerSystem;

	private SimulationSystem m_SimulationSystem;

	private ResourceAvailabilitySystem m_ResourceAvailabilitySystem;

	private bool m_Started;

	private double m_StartTime;

	private bool m_Triggered;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<BuildingCondition>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ResourceAvailabilitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceAvailabilitySystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_BuildingQuery)).IsEmptyIgnoreFilter && !m_Triggered)
		{
			if (!m_Started)
			{
				m_StartTime = m_SimulationSystem.frameIndex;
				m_Started = true;
			}
			if (m_ResourceAvailabilitySystem.appliedResource == AvailableResource.OutsideConnection && (double)m_SimulationSystem.frameIndex - m_StartTime > (double)(kDelaySeconds * 60f))
			{
				TriggerJob triggerJob = new TriggerJob
				{
					m_Buildings = ((EntityQuery)(ref m_BuildingQuery)).ToComponentDataArray<Building>(AllocatorHandle.op_Implicit((Allocator)3)),
					m_AvailabilityDatas = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ActionBuffer = m_TriggerSystem.CreateActionBuffer()
				};
				((SystemBase)this).Dependency = IJobExtensions.Schedule<TriggerJob>(triggerJob, ((SystemBase)this).Dependency);
				m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
				m_Triggered = true;
			}
		}
		if (((EntityQuery)(ref m_BuildingQuery)).IsEmptyIgnoreFilter && m_Started && !m_Triggered)
		{
			m_Started = false;
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		base.OnGameLoaded(serializationContext);
		if ((int)((Context)(ref serializationContext)).purpose == 1)
		{
			m_Started = false;
			m_StartTime = 0.0;
			m_Triggered = false;
		}
		else
		{
			m_Triggered = true;
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
	public EarlyGameOutsideConnectionTriggerSystem()
	{
	}
}
