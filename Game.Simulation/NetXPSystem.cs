using System.Runtime.CompilerServices;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class NetXPSystem : GameSystemBase
{
	[BurstCompile]
	private struct NetXPJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PlaceableNetDatas;

		[ReadOnly]
		public ComponentLookup<Elevation> m_Elevations;

		[ReadOnly]
		public ComponentLookup<RoadData> m_RoadDatas;

		[ReadOnly]
		public ComponentLookup<TrackData> m_TrackDatas;

		[ReadOnly]
		public ComponentLookup<WaterwayData> m_WaterwayDatas;

		[ReadOnly]
		public ComponentLookup<PipelineData> m_PipelineDatas;

		[ReadOnly]
		public ComponentLookup<PowerLineData> m_PowerLineDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<Edge> m_Edges;

		[ReadOnly]
		public ComponentLookup<Curve> m_Curves;

		[ReadOnly]
		public NativeArray<Entity> m_CreatedEntities;

		[ReadOnly]
		public NativeArray<Entity> m_DeletedEntities;

		public NativeQueue<XPGain> m_XPQueue;

		private float GetElevationBonus(Edge edge, ComponentLookup<Elevation> elevations, bool isRoad)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Elevation elevation = default(Elevation);
			if (!isRoad || ((!elevations.TryGetComponent(edge.m_Start, ref elevation) || !(elevation.m_Elevation.x > 0f) || !(elevation.m_Elevation.y > 0f)) && (!elevations.TryGetComponent(edge.m_End, ref elevation) || !(elevation.m_Elevation.x > 0f) || !(elevation.m_Elevation.y > 0f))))
			{
				return 0f;
			}
			return 1f;
		}

		private NetXPs CountXP(ref NativeArray<Entity> entities)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			NetXPs result = default(NetXPs);
			PlaceableNetData placeableNetData = default(PlaceableNetData);
			for (int i = 0; i < entities.Length; i++)
			{
				Entity val = entities[i];
				Entity prefab = m_PrefabRefs[val].m_Prefab;
				if (!m_PlaceableNetDatas.TryGetComponent(prefab, ref placeableNetData) || placeableNetData.m_XPReward <= 0)
				{
					continue;
				}
				float num = ((float)placeableNetData.m_XPReward + GetElevationBonus(m_Edges[val], m_Elevations, m_RoadDatas.HasComponent(prefab))) * m_Curves[val].m_Length / kXPRewardLength;
				if (m_RoadDatas.HasComponent(prefab))
				{
					result.m_Roads += num;
				}
				else if (m_TrackDatas.HasComponent(prefab))
				{
					TrackData trackData = m_TrackDatas[prefab];
					if (trackData.m_TrackType == TrackTypes.Train)
					{
						result.m_Trains += num;
					}
					else if (trackData.m_TrackType == TrackTypes.Tram)
					{
						result.m_Trams += num;
					}
					else if (trackData.m_TrackType == TrackTypes.Subway)
					{
						result.m_Subways += num;
					}
				}
				else if (m_WaterwayDatas.HasComponent(prefab))
				{
					result.m_Waterways += num;
				}
				else if (m_PipelineDatas.HasComponent(prefab))
				{
					result.m_Pipes += num;
				}
				else if (m_PowerLineDatas.HasComponent(prefab))
				{
					result.m_Powerlines += num;
				}
			}
			return result;
		}

		public void Execute()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			NetXPs netXPs = CountXP(ref m_CreatedEntities);
			if (m_DeletedEntities.Length > 0)
			{
				netXPs -= CountXP(ref m_DeletedEntities);
			}
			netXPs.GetMaxValue(out var max, out var reason);
			int num = Mathf.FloorToInt(max);
			if (num > 0)
			{
				m_XPQueue.Enqueue(new XPGain
				{
					amount = num,
					entity = Entity.Null,
					reason = reason
				});
			}
		}
	}

	private struct NetXPs
	{
		public float m_Roads;

		public float m_Trains;

		public float m_Trams;

		public float m_Subways;

		public float m_Waterways;

		public float m_Pipes;

		public float m_Powerlines;

		public void Clear()
		{
			m_Roads = 0f;
			m_Trains = 0f;
			m_Trams = 0f;
			m_Subways = 0f;
			m_Waterways = 0f;
			m_Pipes = 0f;
			m_Powerlines = 0f;
		}

		public static NetXPs operator +(NetXPs a, NetXPs b)
		{
			return new NetXPs
			{
				m_Roads = a.m_Roads + b.m_Roads,
				m_Trains = a.m_Trains + b.m_Trains,
				m_Trams = a.m_Trams + b.m_Trams,
				m_Subways = a.m_Subways + b.m_Subways,
				m_Waterways = a.m_Waterways + b.m_Waterways,
				m_Pipes = a.m_Pipes + b.m_Pipes,
				m_Powerlines = a.m_Powerlines + b.m_Powerlines
			};
		}

		public static NetXPs operator -(NetXPs a, NetXPs b)
		{
			return new NetXPs
			{
				m_Roads = a.m_Roads - b.m_Roads,
				m_Trains = a.m_Trains - b.m_Trains,
				m_Trams = a.m_Trams - b.m_Trams,
				m_Subways = a.m_Subways - b.m_Subways,
				m_Waterways = a.m_Waterways - b.m_Waterways,
				m_Pipes = a.m_Pipes - b.m_Pipes,
				m_Powerlines = a.m_Powerlines - b.m_Powerlines
			};
		}

		public void GetMaxValue(out float max, out XPReason reason)
		{
			reason = XPReason.Unknown;
			max = 0f;
			Check(m_Roads, XPReason.Road, ref max, ref reason);
			Check(m_Trains, XPReason.TrainTrack, ref max, ref reason);
			Check(m_Trams, XPReason.TramTrack, ref max, ref reason);
			Check(m_Subways, XPReason.SubwayTrack, ref max, ref reason);
			Check(m_Waterways, XPReason.Waterway, ref max, ref reason);
			Check(m_Pipes, XPReason.Pipe, ref max, ref reason);
			Check(m_Powerlines, XPReason.PowerLine, ref max, ref reason);
		}

		private void Check(float value, XPReason reason, ref float max, ref XPReason maxReason)
		{
			if (value > max)
			{
				max = value;
				maxReason = reason;
			}
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RW_ComponentLookup;

		public ComponentLookup<Elevation> __Game_Net_Elevation_RW_ComponentLookup;

		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RW_ComponentLookup;

		public ComponentLookup<TrackData> __Game_Prefabs_TrackData_RW_ComponentLookup;

		public ComponentLookup<WaterwayData> __Game_Prefabs_WaterwayData_RW_ComponentLookup;

		public ComponentLookup<PipelineData> __Game_Prefabs_PipelineData_RW_ComponentLookup;

		public ComponentLookup<PowerLineData> __Game_Prefabs_PowerLineData_RW_ComponentLookup;

		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RW_ComponentLookup;

		public ComponentLookup<Edge> __Game_Net_Edge_RW_ComponentLookup;

		public ComponentLookup<Curve> __Game_Net_Curve_RW_ComponentLookup;

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
			__Game_Prefabs_PlaceableNetData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(false);
			__Game_Net_Elevation_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(false);
			__Game_Prefabs_RoadData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(false);
			__Game_Prefabs_TrackData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackData>(false);
			__Game_Prefabs_WaterwayData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterwayData>(false);
			__Game_Prefabs_PipelineData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PipelineData>(false);
			__Game_Prefabs_PowerLineData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PowerLineData>(false);
			__Game_Prefabs_PrefabRef_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(false);
			__Game_Net_Edge_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(false);
			__Game_Net_Curve_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(false);
		}
	}

	private static readonly float kXPRewardLength = 112f;

	private XPSystem m_XPSystem;

	private EntityQuery m_CreatedNetQuery;

	private EntityQuery m_DeletedNetQuery;

	private TypeHandle __TypeHandle;

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
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CreatedNetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Curve>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_DeletedNetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Curve>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Created>(),
			ComponentType.Exclude<Temp>()
		});
		m_XPSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<XPSystem>();
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedNetQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val2 = default(JobHandle);
		NativeList<Entity> val = ((EntityQuery)(ref m_CreatedNetQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		JobHandle val4 = default(JobHandle);
		NativeList<Entity> val3 = ((EntityQuery)(ref m_DeletedNetQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
		JobHandle deps;
		NetXPJob netXPJob = new NetXPJob
		{
			m_PlaceableNetDatas = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Elevations = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadDatas = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackDatas = InternalCompilerInterface.GetComponentLookup<TrackData>(ref __TypeHandle.__Game_Prefabs_TrackData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterwayDatas = InternalCompilerInterface.GetComponentLookup<WaterwayData>(ref __TypeHandle.__Game_Prefabs_WaterwayData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PipelineDatas = InternalCompilerInterface.GetComponentLookup<PipelineData>(ref __TypeHandle.__Game_Prefabs_PipelineData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PowerLineDatas = InternalCompilerInterface.GetComponentLookup<PowerLineData>(ref __TypeHandle.__Game_Prefabs_PowerLineData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Edges = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Curves = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreatedEntities = val.AsDeferredJobArray(),
			m_DeletedEntities = val3.AsDeferredJobArray(),
			m_XPQueue = m_XPSystem.GetQueue(out deps)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<NetXPJob>(netXPJob, JobHandle.CombineDependencies(val2, val4, JobHandle.CombineDependencies(deps, ((SystemBase)this).Dependency)));
		m_XPSystem.AddQueueWriter(((SystemBase)this).Dependency);
		val.Dispose(((SystemBase)this).Dependency);
		val3.Dispose(((SystemBase)this).Dependency);
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
	public NetXPSystem()
	{
	}
}
