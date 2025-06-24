using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

[CompilerGenerated]
public class BuildableAreaDebugSystem : BaseDebugSystem
{
	private struct BuildableAreaGizmoJob : IJobParallelFor
	{
		[ReadOnly]
		public CellMapData<NaturalResourceCell> m_NaturalResourceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public GizmoBatcher m_GizmoBatcher;

		public ParallelWriter<AverageFloat> m_Average;

		public Bounds1 m_BuildableLandMaxSlope;

		public void Execute(int index)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			float3 cellCenter = CellMapSystem<NaturalResourceCell>.GetCellCenter(index, NaturalResourceSystem.kTextureSize);
			float num = AreaResourceSystem.CalculateBuildable(cellCenter, m_NaturalResourceData.m_CellSize, m_WaterSurfaceData, m_TerrainHeightData, m_BuildableLandMaxSlope);
			m_Average.Accumulate(new AverageFloat
			{
				m_Total = num,
				m_Count = 1
			});
			if (num > 0f)
			{
				Color color = Color.Lerp(Color.red, Color.green, num);
				float2 val = 0.5f * math.sqrt(num) * m_NaturalResourceData.m_CellSize;
				DrawLine(cellCenter + new float3(0f - val.x, 0f, 0f - val.y), cellCenter + new float3(val.x, 0f, 0f - val.y), color);
				DrawLine(cellCenter + new float3(val.x, 0f, 0f - val.y), cellCenter + new float3(val.x, 0f, val.y), color);
				DrawLine(cellCenter + new float3(val.x, 0f, val.y), cellCenter + new float3(0f - val.x, 0f, val.y), color);
				DrawLine(cellCenter + new float3(0f - val.x, 0f, val.y), cellCenter + new float3(0f - val.x, 0f, 0f - val.y), color);
			}
		}

		private void DrawLine(float3 a, float3 b, Color color)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			a.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, a);
			b.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, b);
			((GizmoBatcher)(ref m_GizmoBatcher)).DrawLine(a, b, color);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct TypeHandle
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
		}
	}

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private GizmosSystem m_GizmosSystem;

	private Option m_StrictOption;

	private NativeAccumulator<AverageFloat> m_BuildableArea;

	private float m_LastBuildableArea;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1438325908_0;

	public float buildableArea => m_LastBuildableArea;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		((ComponentSystemBase)this).RequireForUpdate<AreasConfigurationData>();
		m_BuildableArea = new NativeAccumulator<AverageFloat>(AllocatorHandle.op_Implicit((Allocator)4));
		m_StrictOption = AddOption("Strict", defaultEnabled: false);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_BuildableArea.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		AverageFloat result = m_BuildableArea.GetResult(0);
		m_LastBuildableArea = ((AverageFloat)(ref result)).average;
		m_BuildableArea.Clear();
		JobHandle val2 = default(JobHandle);
		JobHandle dependencies;
		JobHandle deps;
		JobHandle val = IJobParallelForExtensions.Schedule<BuildableAreaGizmoJob>(new BuildableAreaGizmoJob
		{
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2),
			m_NaturalResourceData = m_NaturalResourceSystem.GetData(readOnly: true, out dependencies),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_BuildableLandMaxSlope = (Bounds1)(m_StrictOption.enabled ? new Bounds1(0f, 0.3f) : ((EntityQuery)(ref __query_1438325908_0)).GetSingleton<AreasConfigurationData>().m_BuildableLandMaxSlope),
			m_Average = m_BuildableArea.AsParallelWriter()
		}, NaturalResourceSystem.kTextureSize * NaturalResourceSystem.kTextureSize, NaturalResourceSystem.kTextureSize, JobUtils.CombineDependencies(inputDeps, val2, dependencies, deps));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		m_TerrainSystem.AddCPUHeightReader(val);
		m_WaterSystem.AddSurfaceReader(val);
		m_NaturalResourceSystem.AddReader(val);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<AreasConfigurationData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1438325908_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public BuildableAreaDebugSystem()
	{
	}
}
