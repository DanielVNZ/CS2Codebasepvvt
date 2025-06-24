using Colossal;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Debug;

public class WaterCullingDebugSystem : BaseDebugSystem
{
	private GizmosSystem m_GizmosSystem;

	private WaterSystem m_WaterSystem;

	private TerrainSystem m_TerrainSystem;

	private Option m_ActiveWaterCellOption;

	private Option m_FixedHeight;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_GizmosSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GizmosSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_ActiveWaterCellOption = AddOption("Show active water cell", defaultEnabled: false);
		m_FixedHeight = AddOption("Fixed Height", defaultEnabled: false);
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		int num = 25;
		int num2 = 49;
		RenderPipeline currentPipeline = RenderPipelineManager.currentPipeline;
		HDRenderPipeline val = (HDRenderPipeline)(object)((currentPipeline is HDRenderPipeline) ? currentPipeline : null);
		if (val == null)
		{
			return;
		}
		JobHandle val2 = default(JobHandle);
		GizmoBatcher gizmosBatcher = m_GizmosSystem.GetGizmosBatcher(ref val2);
		((JobHandle)(ref val2)).Complete();
		if (!m_ActiveWaterCellOption.enabled)
		{
			float3[] waterPatchesPositions = val.GetWaterPatchesPositions();
			float2 val5 = default(float2);
			for (int i = 0; i < num; i++)
			{
				float3 val3 = waterPatchesPositions[i];
				float3 val4 = waterPatchesPositions[i + num2];
				((float2)(ref val5))._002Ector(val4.x, val4.y);
				Color val6 = ((val4.z > 0f) ? Color.blue : Color.red);
				if (m_FixedHeight.enabled)
				{
					val3.y = 400f;
				}
				((GizmoBatcher)(ref gizmosBatcher)).DrawWireNode(val3, 1f, val6);
				((GizmoBatcher)(ref gizmosBatcher)).DrawWireRect(val3, val5, val6);
				if (val4.z > 0f)
				{
					((GizmoBatcher)(ref gizmosBatcher)).DrawWireCapsule(val3, 64f, val4.z, Color.blue, 36);
				}
			}
			return;
		}
		float num3 = (float)m_WaterSystem.GridSize * m_WaterSystem.CellSize;
		float3 val7 = m_TerrainSystem.positionOffset + new float3(num3 * 0.5f, 0f, num3 * 0.5f);
		NativeArray<int> active = m_WaterSystem.GetActive();
		int2 activeGridSize = m_WaterSystem.m_ActiveGridSize;
		float2 val8 = default(float2);
		((float2)(ref val8))._002Ector(num3, num3);
		float2 val9 = default(float2);
		for (int j = 0; j < activeGridSize.x; j++)
		{
			for (int k = 0; k < activeGridSize.y; k++)
			{
				((float2)(ref val9))._002Ector((float)j * num3, (float)k * num3);
				float3 val10 = val7 + new float3(val9.x, 400f, val9.y);
				Color val11 = ((active[j + k * activeGridSize.x] > 0) ? Color.green : Color.red);
				((GizmoBatcher)(ref gizmosBatcher)).DrawWireRect(val10, val8 * 0.49f, val11);
			}
		}
	}

	[Preserve]
	public WaterCullingDebugSystem()
	{
	}
}
