using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Routes;
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
public class RouteDebugSystem : BaseDebugSystem
{
	[BurstCompile]
	private struct RouteGizmoJob : IJobChunk
	{
		[ReadOnly]
		public bool m_RouteOption;

		[ReadOnly]
		public bool m_LaneConnectionOption;

		[ReadOnly]
		public ComponentTypeHandle<Route> m_RouteType;

		[ReadOnly]
		public ComponentTypeHandle<Position> m_PositionType;

		[ReadOnly]
		public ComponentTypeHandle<AccessLane> m_AccessLaneType;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> m_RouteLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> m_WaypointType;

		[ReadOnly]
		public ComponentTypeHandle<Error> m_ErrorType;

		[ReadOnly]
		public ComponentLookup<Position> m_PositionData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			Color val = (((ArchetypeChunk)(ref chunk)).Has<Error>(ref m_ErrorType) ? Color.red : ((!((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType)) ? Color.cyan : Color.blue));
			NativeArray<Route> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Route>(ref m_RouteType);
			if (nativeArray.Length != 0)
			{
				if (!m_RouteOption)
				{
					return;
				}
				BufferAccessor<RouteWaypoint> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteWaypoint>(ref m_WaypointType);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Route route = nativeArray[i];
					DynamicBuffer<RouteWaypoint> val2 = bufferAccessor[i];
					for (int j = 0; j < val2.Length; j++)
					{
						RouteWaypoint routeWaypoint = val2[j];
						float3 position = m_PositionData[routeWaypoint.m_Waypoint].m_Position;
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(position, 2f, val);
						if (j != val2.Length - 1 || (route.m_Flags & RouteFlags.Complete) != 0)
						{
							RouteWaypoint routeWaypoint2 = val2[math.select(j + 1, 0, j == val2.Length - 1)];
							float3 position2 = m_PositionData[routeWaypoint2.m_Waypoint].m_Position;
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawMiddleArrow(position, position2, val, 4f, 25f, 16);
						}
					}
				}
			}
			else
			{
				if (!m_LaneConnectionOption)
				{
					return;
				}
				NativeArray<AccessLane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AccessLane>(ref m_AccessLaneType);
				NativeArray<RouteLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RouteLane>(ref m_RouteLaneType);
				NativeArray<Position> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Position>(ref m_PositionType);
				NativeArray<Transform> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				if (nativeArray4.Length != 0)
				{
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						float3 position3 = nativeArray4[k].m_Position;
						AccessLane accessLane = nativeArray2[k];
						if (m_CurveData.HasComponent(accessLane.m_Lane))
						{
							float3 val3 = MathUtils.Position(m_CurveData[accessLane.m_Lane].m_Bezier, accessLane.m_CurvePos);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val3, 1f, Color.green);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position3, val3, Color.green);
						}
					}
					for (int l = 0; l < nativeArray3.Length; l++)
					{
						float3 position4 = nativeArray4[l].m_Position;
						RouteLane routeLane = nativeArray3[l];
						if (m_CurveData.HasComponent(routeLane.m_StartLane))
						{
							float3 val4 = MathUtils.Position(m_CurveData[routeLane.m_StartLane].m_Bezier, routeLane.m_StartCurvePos);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val4, 1f, Color.magenta);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position4, val4, Color.magenta);
						}
						if (m_CurveData.HasComponent(routeLane.m_EndLane))
						{
							float3 val5 = MathUtils.Position(m_CurveData[routeLane.m_EndLane].m_Bezier, routeLane.m_EndCurvePos);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val5, 1f, Color.magenta);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position4, val5, Color.magenta);
						}
					}
				}
				else
				{
					if (nativeArray5.Length == 0)
					{
						return;
					}
					for (int m = 0; m < nativeArray2.Length; m++)
					{
						float3 position5 = nativeArray5[m].m_Position;
						AccessLane accessLane2 = nativeArray2[m];
						if (m_CurveData.HasComponent(accessLane2.m_Lane))
						{
							float3 val6 = MathUtils.Position(m_CurveData[accessLane2.m_Lane].m_Bezier, accessLane2.m_CurvePos);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val6, 1f, Color.green);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position5, val6, Color.green);
						}
					}
					for (int n = 0; n < nativeArray3.Length; n++)
					{
						float3 position6 = nativeArray5[n].m_Position;
						RouteLane routeLane2 = nativeArray3[n];
						if (m_CurveData.HasComponent(routeLane2.m_StartLane))
						{
							float3 val7 = MathUtils.Position(m_CurveData[routeLane2.m_StartLane].m_Bezier, routeLane2.m_StartCurvePos);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val7, 1f, Color.magenta);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position6, val7, Color.magenta);
						}
						if (m_CurveData.HasComponent(routeLane2.m_EndLane))
						{
							float3 val8 = MathUtils.Position(m_CurveData[routeLane2.m_EndLane].m_Bezier, routeLane2.m_EndCurvePos);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireNode(val8, 1f, Color.magenta);
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(position6, val8, Color.magenta);
						}
					}
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
		public ComponentTypeHandle<Route> __Game_Routes_Route_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Position> __Game_Routes_Position_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AccessLane> __Game_Routes_AccessLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RouteLane> __Game_Routes_RouteLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Error> __Game_Tools_Error_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Position> __Game_Routes_Position_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			__Game_Routes_Route_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Route>(true);
			__Game_Routes_Position_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Position>(true);
			__Game_Routes_AccessLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AccessLane>(true);
			__Game_Routes_RouteLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteLane>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Routes_RouteWaypoint_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteWaypoint>(true);
			__Game_Tools_Error_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Error>(true);
			__Game_Routes_Position_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Position>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
		}
	}

	private EntityQuery m_RouteGroup;

	private GizmosSystem m_GizmosSystem;

	private Option m_RouteOption;

	private Option m_LaneConnectionOption;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadOnly<RouteWaypoint>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Hidden>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<AccessLane>(),
			ComponentType.ReadOnly<RouteLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Hidden>()
		};
		array[1] = val;
		m_RouteGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_RouteOption = AddOption("Routes", defaultEnabled: true);
		m_LaneConnectionOption = AddOption("Lane Connections", defaultEnabled: true);
		((ComponentSystemBase)this).RequireForUpdate(m_RouteGroup);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
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
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		JobHandle val = JobChunkExtensions.ScheduleParallel<RouteGizmoJob>(new RouteGizmoJob
		{
			m_RouteOption = m_RouteOption.enabled,
			m_LaneConnectionOption = m_LaneConnectionOption.enabled,
			m_RouteType = InternalCompilerInterface.GetComponentTypeHandle<Route>(ref __TypeHandle.__Game_Routes_Route_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PositionType = InternalCompilerInterface.GetComponentTypeHandle<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AccessLaneType = InternalCompilerInterface.GetComponentTypeHandle<AccessLane>(ref __TypeHandle.__Game_Routes_AccessLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteLaneType = InternalCompilerInterface.GetComponentTypeHandle<RouteLane>(ref __TypeHandle.__Game_Routes_RouteLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaypointType = InternalCompilerInterface.GetBufferTypeHandle<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ErrorType = InternalCompilerInterface.GetComponentTypeHandle<Error>(ref __TypeHandle.__Game_Tools_Error_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PositionData = InternalCompilerInterface.GetComponentLookup<Position>(ref __TypeHandle.__Game_Routes_Position_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2)
		}, m_RouteGroup, JobHandle.CombineDependencies(inputDeps, val2));
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
	public RouteDebugSystem()
	{
	}
}
