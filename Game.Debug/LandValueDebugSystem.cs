using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class LandValueDebugSystem : BaseDebugSystem
{
	private struct LandValueEdgeGizmoJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<LandValue> m_LandValues;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		public GizmoBatcher m_GizmoBatcher;

		public bool m_LandValueOption;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			if (!m_LandValueOption)
			{
				return;
			}
			NativeArray<Edge> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<LandValue> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LandValue>(ref m_LandValues);
			NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			if (nativeArray.Length == 0)
			{
				return;
			}
			if (nativeArray3.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Curve curve = nativeArray3[i];
					float landValue = nativeArray2[i].m_LandValue;
					Color color = GetColor(Color.gray, Color.blue, Color.magenta, landValue, 30f, 500f);
					float3 val = MathUtils.Position(curve.m_Bezier, 0.5f);
					val.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, val);
					val.y += heightScale * landValue / 2f;
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val, 5f, heightScale * landValue, color, 36);
				}
				return;
			}
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Edge edge = nativeArray[j];
				Node node = m_NodeData[edge.m_Start];
				Node node2 = m_NodeData[edge.m_End];
				float landValue2 = nativeArray2[j].m_LandValue;
				Color color2 = GetColor(Color.gray, Color.blue, Color.magenta, landValue2, 30f, 500f);
				float3 val2 = 0.5f * (node.m_Position + node2.m_Position);
				val2.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, val2);
				val2.y += heightScale * landValue2 / 2f;
				((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCylinder(val2, 5f, heightScale * landValue2, color2, 36);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct LandValueGizmoJob : IJob
	{
		[ReadOnly]
		public NativeArray<LandValueCell> m_LandValueMap;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public GizmoBatcher m_GizmoBatcher;

		public bool m_LandValueOption;

		[ReadOnly]
		public LandValueParameterData m_LandValueParameterData;

		public void Execute()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			if (!m_LandValueOption)
			{
				return;
			}
			for (int i = 0; i < m_LandValueMap.Length; i++)
			{
				float landValue = m_LandValueMap[i].m_LandValue;
				Color color = GetColor(Color.red, Color.yellow, Color.green, landValue, 30f, 500f);
				float3 cellCenter = LandValueSystem.GetCellCenter(i);
				cellCenter.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cellCenter);
				cellCenter.y += heightScale * landValue / 2f;
				if (landValue > m_LandValueParameterData.m_LandValueBaseline)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(cellCenter, new float3(15f, heightScale * landValue, 15f), color);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LandValue> __Game_Net_LandValue_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

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
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_LandValue_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LandValue>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
		}
	}

	private LandValueSystem m_LandValueSystem;

	private GizmosSystem m_GizmosSystem;

	private TerrainSystem m_TerrainSystem;

	private DefaultToolSystem m_DefaultToolSystem;

	private EntityQuery m_LandValueEdgeQuery;

	private EntityQuery m_LandValueParameterQuery;

	public Option m_LandValueCellOption;

	private Option m_EdgeLandValueOption;

	private static readonly float heightScale = 1f;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LandValueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LandValueSystem>();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_DefaultToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_LandValueParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LandValueParameterData>() });
		m_LandValueEdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<LandValue>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Hidden>()
		});
		m_LandValueCellOption = AddOption("Land value (Cell)", defaultEnabled: true);
		m_EdgeLandValueOption = AddOption("Land value (Edge)", defaultEnabled: true);
		((ComponentSystemBase)this).Enabled = false;
	}

	public override void OnEnabled(Container container)
	{
		base.OnEnabled(container);
		m_DefaultToolSystem.debugLandValue = true;
	}

	public override void OnDisabled(Container container)
	{
		base.OnDisabled(container);
		m_DefaultToolSystem.debugLandValue = false;
	}

	private static Color GetColor(Color a, Color b, Color c, float value, float maxValue1, float maxValue2)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (value < maxValue1)
		{
			return Color.Lerp(a, b, value / maxValue1);
		}
		return Color.Lerp(b, c, math.saturate((value - maxValue1) / (maxValue2 - maxValue1)));
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		if (m_LandValueCellOption.enabled)
		{
			JobHandle dependencies;
			JobHandle val = default(JobHandle);
			LandValueGizmoJob landValueGizmoJob = new LandValueGizmoJob
			{
				m_LandValueMap = m_LandValueSystem.GetMap(readOnly: true, out dependencies),
				m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val),
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_LandValueOption = m_LandValueCellOption.enabled,
				m_LandValueParameterData = ((EntityQuery)(ref m_LandValueParameterQuery)).GetSingleton<LandValueParameterData>()
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<LandValueGizmoJob>(landValueGizmoJob, JobHandle.CombineDependencies(inputDeps, val, dependencies));
			m_GizmosSystem.AddGizmosBatcherWriter(((SystemBase)this).Dependency);
		}
		if (m_EdgeLandValueOption.enabled)
		{
			JobHandle val2 = default(JobHandle);
			LandValueEdgeGizmoJob landValueEdgeGizmoJob = new LandValueEdgeGizmoJob
			{
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LandValues = InternalCompilerInterface.GetComponentTypeHandle<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2),
				m_LandValueOption = m_EdgeLandValueOption.enabled
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<LandValueEdgeGizmoJob>(landValueEdgeGizmoJob, m_LandValueEdgeQuery, JobHandle.CombineDependencies(inputDeps, val2));
			m_GizmosSystem.AddGizmosBatcherWriter(((SystemBase)this).Dependency);
		}
		m_TerrainSystem.AddCPUHeightReader(((SystemBase)this).Dependency);
		return ((SystemBase)this).Dependency;
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
	public LandValueDebugSystem()
	{
	}
}
