using Colossal;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Debug;

public class WindDebugSystem : BaseDebugSystem
{
	private struct WindGizmoJob : IJob
	{
		[ReadOnly]
		public NativeArray<WindSimulationSystem.WindCell> m_WindMap;

		public GizmoBatcher m_GizmoBatcher;

		public float2 m_TerrainRange;

		public void Execute()
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < WindSimulationSystem.kResolution.x; i++)
			{
				for (int j = 0; j < WindSimulationSystem.kResolution.y; j++)
				{
					for (int k = 0; k < WindSimulationSystem.kResolution.z; k++)
					{
						if (i == 0 || j == 0 || k == 0 || i == WindSimulationSystem.kResolution.x - 1 || j == WindSimulationSystem.kResolution.y - 1)
						{
							int num = i + j * WindSimulationSystem.kResolution.x + k * WindSimulationSystem.kResolution.x * WindSimulationSystem.kResolution.y;
							WindSimulationSystem.WindCell windCell = m_WindMap[num];
							float3 cellCenter = WindSimulationSystem.GetCellCenter(num);
							cellCenter.y = math.lerp(m_TerrainRange.x, m_TerrainRange.y, ((float)k + 0.5f) / (float)WindSimulationSystem.kResolution.z);
							Color white = Color.white;
							if (math.abs(windCell.m_Velocities.x) > 0.001f)
							{
								float3 val = cellCenter + new float3(0.5f * (float)CellMapSystem<Wind>.kMapSize / (float)WindSimulationSystem.kResolution.x, 0f, 0f);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(val, val + 50f * new float3(windCell.m_Velocities.x, 0f, 0f), white, 1f, 25f, 16);
							}
							if (math.abs(windCell.m_Velocities.y) > 0.001f)
							{
								float3 val = cellCenter + new float3(0f, 0f, 0.5f * (float)CellMapSystem<Wind>.kMapSize / (float)WindSimulationSystem.kResolution.y);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(val, val + 50f * new float3(0f, 0f, windCell.m_Velocities.y), white, 1f, 25f, 16);
							}
							if (math.abs(windCell.m_Velocities.z) > 0.001f)
							{
								float3 val = cellCenter + new float3(0f, 0.5f * (float)CellMapSystem<Wind>.kMapSize / (float)WindSimulationSystem.kResolution.x, 0f);
								((GizmoBatcher)(ref m_GizmoBatcher)).DrawArrow(val, val + 50f * new float3(0f, windCell.m_Velocities.z, 0f), white, 1f, 25f, 16);
							}
							if (windCell.m_Pressure < 0f)
							{
								((Color)(ref white))._002Ector(math.lerp(0f, 1f, math.saturate(-10f * windCell.m_Pressure)), 0f, 0f);
							}
							else
							{
								((Color)(ref white))._002Ector(0f, math.lerp(0f, 1f, math.saturate(10f * windCell.m_Pressure)), 0f);
							}
							((GizmoBatcher)(ref m_GizmoBatcher)).DrawWireCube(cellCenter, new float3(10f, 10f * windCell.m_Pressure, 10f), white);
						}
					}
				}
			}
		}
	}

	private WindSimulationSystem m_WindSimulationSystem;

	private TerrainSystem m_TerrainSystem;

	private GizmosSystem m_GizmosSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_WindSimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSimulationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightData data = m_TerrainSystem.GetHeightData();
		float2 terrainRange = default(float2);
		((float2)(ref terrainRange))._002Ector(TerrainUtils.ToWorldSpace(ref data, 0f), TerrainUtils.ToWorldSpace(ref data, 65535f));
		JobHandle deps;
		JobHandle val2 = default(JobHandle);
		JobHandle val = IJobExtensions.Schedule<WindGizmoJob>(new WindGizmoJob
		{
			m_WindMap = m_WindSimulationSystem.GetCells(out deps),
			m_GizmoBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2),
			m_TerrainRange = terrainRange
		}, JobHandle.CombineDependencies(inputDeps, val2, deps));
		m_WindSimulationSystem.AddReader(val);
		m_GizmosSystem.AddGizmosBatcherWriter(val);
		return val;
	}

	[Preserve]
	public WindDebugSystem()
	{
	}
}
