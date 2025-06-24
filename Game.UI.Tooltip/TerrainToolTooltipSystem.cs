using System.Runtime.CompilerServices;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class TerrainToolTooltipSystem : TooltipSystemBase
{
	private ToolSystem m_ToolSystem;

	private TerrainToolSystem m_TerrainTool;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private EntityQuery m_ParameterQuery;

	private IntTooltip m_GroundwaterVolume;

	private NativeReference<TempWaterPumpingTooltipSystem.GroundWaterReservoirResult> m_ReservoirResult;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_TerrainTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainToolSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_ParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_ParameterQuery);
		m_GroundwaterVolume = new IntTooltip
		{
			path = "groundWaterCapacity",
			icon = "Media/Game/Icons/Water.svg",
			label = LocalizedString.Id("Tools.GROUNDWATER_VOLUME"),
			unit = "volume"
		};
		m_ReservoirResult = new NativeReference<TempWaterPumpingTooltipSystem.GroundWaterReservoirResult>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ReservoirResult.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool == m_TerrainTool && (Object)(object)m_TerrainTool.prefab != (Object)null && m_TerrainTool.prefab.m_Target == TerraformingTarget.GroundWater && m_ToolRaycastSystem.GetRaycastResult(out var result))
		{
			ProcessResults();
			m_ReservoirResult.Value = default(TempWaterPumpingTooltipSystem.GroundWaterReservoirResult);
			if (GroundWaterSystem.TryGetCell(result.m_Hit.m_HitPosition, out var cell))
			{
				JobHandle dependencies;
				NativeArray<GroundWater> map = m_GroundWaterSystem.GetMap(readOnly: true, out dependencies);
				NativeList<int2> tempGroundWaterPumpCells = default(NativeList<int2>);
				tempGroundWaterPumpCells._002Ector(1, AllocatorHandle.op_Implicit((Allocator)3));
				tempGroundWaterPumpCells.Add(ref cell);
				TempWaterPumpingTooltipSystem.GroundWaterReservoirJob groundWaterReservoirJob = new TempWaterPumpingTooltipSystem.GroundWaterReservoirJob
				{
					m_GroundWaterMap = map,
					m_PumpCapacityMap = new NativeParallelHashMap<int2, int>(0, AllocatorHandle.op_Implicit((Allocator)3)),
					m_TempGroundWaterPumpCells = tempGroundWaterPumpCells,
					m_Queue = new NativeQueue<int2>(AllocatorHandle.op_Implicit((Allocator)3)),
					m_Result = m_ReservoirResult
				};
				((SystemBase)this).Dependency = IJobExtensions.Schedule<TempWaterPumpingTooltipSystem.GroundWaterReservoirJob>(groundWaterReservoirJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
				groundWaterReservoirJob.m_Queue.Dispose(((SystemBase)this).Dependency);
				groundWaterReservoirJob.m_PumpCapacityMap.Dispose(((SystemBase)this).Dependency);
				tempGroundWaterPumpCells.Dispose(((SystemBase)this).Dependency);
				m_GroundWaterSystem.AddReader(((SystemBase)this).Dependency);
			}
		}
		else
		{
			m_ReservoirResult.Value = default(TempWaterPumpingTooltipSystem.GroundWaterReservoirResult);
		}
	}

	private void ProcessResults()
	{
		TempWaterPumpingTooltipSystem.GroundWaterReservoirResult value = m_ReservoirResult.Value;
		if (value.m_Volume > 0)
		{
			WaterPipeParameterData singleton = ((EntityQuery)(ref m_ParameterQuery)).GetSingleton<WaterPipeParameterData>();
			float num = singleton.m_GroundwaterReplenish / singleton.m_GroundwaterUsageMultiplier * (float)value.m_Volume;
			m_GroundwaterVolume.value = Mathf.RoundToInt(num);
			if (m_GroundwaterVolume.value > 0)
			{
				AddMouseTooltip(m_GroundwaterVolume);
			}
		}
	}

	[Preserve]
	public TerrainToolTooltipSystem()
	{
	}
}
