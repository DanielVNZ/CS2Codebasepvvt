using System.Runtime.CompilerServices;
using Colossal;
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
public class TerrainAttractivenessDebugSystem : BaseDebugSystem
{
	private struct TerrainAttractivenessGizmoJob : IJob
	{
		[ReadOnly]
		public CellMapData<TerrainAttractiveness> m_Map;

		[ReadOnly]
		public TerrainHeightData m_HeightData;

		public AttractivenessParameterData m_Parameters;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Map.m_Buffer.Length; i++)
			{
				float3 cellCenter = TerrainAttractivenessSystem.GetCellCenter(i);
				cellCenter.y = TerrainUtils.SampleHeight(ref m_HeightData, cellCenter);
				float num = TerrainAttractivenessSystem.EvaluateAttractiveness(cellCenter, m_Map, m_HeightData, m_Parameters, default(NativeArray<int>));
				if (num > 0f)
				{
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(cellCenter, new float3(10f, num, 10f), Color.white);
				}
			}
		}
	}

	private TerrainAttractivenessSystem m_TerrainAttractivenessSystem;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_ParameterQuery;

	private GizmosSystem m_GizmosSystem;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_TerrainAttractivenessSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainAttractivenessSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_ParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AttractivenessParameterData>() });
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<TerrainAttractivenessGizmoJob>(new TerrainAttractivenessGizmoJob
		{
			m_Map = m_TerrainAttractivenessSystem.GetData(readOnly: true, out dependencies),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2),
			m_HeightData = m_TerrainSystem.GetHeightData(),
			m_Parameters = ((EntityQuery)(ref m_ParameterQuery)).GetSingleton<AttractivenessParameterData>()
		}, JobHandle.CombineDependencies(inputDeps, val2, dependencies));
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		m_TerrainAttractivenessSystem.AddReader(val);
		m_TerrainSystem.AddCPUHeightReader(val);
		return val;
	}

	[Preserve]
	public TerrainAttractivenessDebugSystem()
	{
	}
}
