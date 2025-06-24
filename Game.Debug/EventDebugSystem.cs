using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class EventDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct EventGizmoJob : IJobChunk
	{
		[ReadOnly]
		public uint m_SimulationFrame;

		public GizmoBatcher m_GizmoBatcher;

		[ReadOnly]
		public ComponentTypeHandle<Game.Events.Event> m_EventType;

		[ReadOnly]
		public ComponentTypeHandle<Duration> m_DurationType;

		[ReadOnly]
		public ComponentTypeHandle<WeatherPhenomenon> m_WeatherPhenomenonType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Events.Event>(ref m_EventType))
			{
				NativeArray<Duration> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Duration>(ref m_DurationType);
				NativeArray<WeatherPhenomenon> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WeatherPhenomenon>(ref m_WeatherPhenomenonType);
				NativeArray<InterpolatedTransform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<InterpolatedTransform>(ref m_InterpolatedTransformType);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Duration duration = nativeArray[i];
					WeatherPhenomenon weatherPhenomenon = nativeArray2[i];
					if (duration.m_EndFrame < m_SimulationFrame && weatherPhenomenon.m_Intensity == 0f)
					{
						continue;
					}
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireArc(weatherPhenomenon.m_PhenomenonPosition, new float3(0f, 1f, 0f), new float3(1f, 0f, 0f), 360f, weatherPhenomenon.m_PhenomenonRadius, Color.cyan, 72);
					if (duration.m_StartFrame <= m_SimulationFrame || weatherPhenomenon.m_Intensity != 0f)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(weatherPhenomenon.m_HotspotPosition, 10f, Color.yellow);
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireArc(weatherPhenomenon.m_HotspotPosition, new float3(0f, 1f, 0f), new float3(1f, 0f, 0f), 360f, weatherPhenomenon.m_HotspotRadius, Color.yellow, 72);
						if (nativeArray3.Length != 0)
						{
							InterpolatedTransform interpolatedTransform = nativeArray3[i];
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(interpolatedTransform.m_Position, 10f, Color.green);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireArc(interpolatedTransform.m_Position, new float3(0f, 1f, 0f), new float3(1f, 0f, 0f), 360f, weatherPhenomenon.m_HotspotRadius, Color.green, 72);
						}
					}
				}
				return;
			}
			NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			if (nativeArray4.Length != 0)
			{
				for (int j = 0; j < nativeArray4.Length; j++)
				{
					Transform transform = nativeArray4[j];
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(transform.m_Position + new float3(0f, 20f, 0f), transform.m_Position, Color.red, 5f, 25f, 16);
				}
				return;
			}
			NativeArray<Curve> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			if (nativeArray5.Length != 0)
			{
				for (int k = 0; k < nativeArray5.Length; k++)
				{
					float3 val = MathUtils.Position(nativeArray5[k].m_Bezier, 0.5f);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(val + new float3(0f, 20f, 0f), val, Color.red, 5f, 25f, 16);
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
		public ComponentTypeHandle<Game.Events.Event> __Game_Events_Event_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Duration> __Game_Events_Duration_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WeatherPhenomenon> __Game_Events_WeatherPhenomenon_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle;

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
			__Game_Events_Event_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Events.Event>(true);
			__Game_Events_Duration_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Duration>(true);
			__Game_Events_WeatherPhenomenon_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WeatherPhenomenon>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(true);
		}
	}

	private EntityQuery m_EventQuery;

	private GizmosSystem m_GizmosSystem;

	private SimulationSystem m_SimulationSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<WeatherPhenomenon>(),
			ComponentType.ReadOnly<OnFire>(),
			ComponentType.ReadOnly<AccidentSite>(),
			ComponentType.ReadOnly<InvolvedInAccident>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<FacingWeather>(),
			ComponentType.ReadOnly<SpectatorSite>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_EventQuery);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<EventGizmoJob>(new EventGizmoJob
		{
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2),
			m_EventType = InternalCompilerInterface.GetComponentTypeHandle<Game.Events.Event>(ref __TypeHandle.__Game_Events_Event_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DurationType = InternalCompilerInterface.GetComponentTypeHandle<Duration>(ref __TypeHandle.__Game_Events_Duration_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WeatherPhenomenonType = InternalCompilerInterface.GetComponentTypeHandle<WeatherPhenomenon>(ref __TypeHandle.__Game_Events_WeatherPhenomenon_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, m_EventQuery, JobHandle.CombineDependencies(inputDeps, val2));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
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
	public EventDebugSystem()
	{
	}
}
