using Colossal;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

public class GroundWaterDebugSystem : GameSystemBase
{
	private struct GroundWaterGizmoJob : IJob
	{
		[ReadOnly]
		public NativeArray<GroundWater> m_GroundWaterMap;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public GizmoBatcher m_GizmoBatcher;

		public void Execute()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_GroundWaterMap.Length; i++)
			{
				GroundWater groundWater = m_GroundWaterMap[i];
				if (groundWater.m_Max > 0)
				{
					float3 cellCenter = GroundWaterSystem.GetCellCenter(i);
					cellCenter.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cellCenter);
					float3 val = cellCenter;
					float3 val2 = cellCenter;
					cellCenter.y += (float)groundWater.m_Max / 400f;
					val.y += (float)groundWater.m_Amount / 400f;
					val2.y += (float)groundWater.m_Polluted / 400f;
					float num = (float)groundWater.m_Polluted / (float)groundWater.m_Amount;
					Color val3 = ((!(num < 0.1f)) ? Color.Lerp(Color.yellow, Color.red, (num - 0.1f) / 0.9f) : Color.Lerp(Color.green, Color.yellow, 10f * num));
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(cellCenter, new float3(10f, (float)groundWater.m_Max / 200f, 10f), Color.grey);
					((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val, new float3(10f, (float)groundWater.m_Amount / 200f, 10f), val3);
					if (groundWater.m_Polluted > 0)
					{
						((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(val2, new float3(10f, (float)groundWater.m_Polluted / 200f, 10f), Color.red);
					}
				}
			}
		}
	}

	private GroundWaterSystem m_GroundWaterSystem;

	private GizmosSystem m_GizmosSystem;

	private TerrainSystem m_TerrainSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val = default(JobHandle);
		GroundWaterGizmoJob groundWaterGizmoJob = new GroundWaterGizmoJob
		{
			m_GroundWaterMap = m_GroundWaterSystem.GetMap(readOnly: true, out dependencies),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val)
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<GroundWaterGizmoJob>(groundWaterGizmoJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, dependencies));
		m_GroundWaterSystem.AddReader(((SystemBase)this).Dependency);
		m_TerrainSystem.AddCPUHeightReader(((SystemBase)this).Dependency);
		m_GizmosSystem.AddGizmosBatcherWriter(((SystemBase)this).Dependency);
	}

	[Preserve]
	public GroundWaterDebugSystem()
	{
	}
}
