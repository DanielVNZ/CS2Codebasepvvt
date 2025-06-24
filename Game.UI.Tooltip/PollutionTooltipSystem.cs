using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class PollutionTooltipSystem : TooltipSystemBase
{
	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private RaycastSystem m_RaycastSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_ActiveInfomodeQuery;

	private IntTooltip m_Garbage;

	private IntTooltip m_AirPollution;

	private IntTooltip m_GroundPollution;

	private IntTooltip m_NoisePollution;

	private IntTooltip m_WaterPollution;

	private RaycastResult m_RaycastResult;

	private RaycastResult raycastResult
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (m_RaycastResult.m_Owner == Entity.Null && m_CameraUpdateSystem.TryGetViewer(out var viewer))
			{
				RaycastInput input = new RaycastInput
				{
					m_Line = ToolRaycastSystem.CalculateRaycastLine(viewer.camera),
					m_TypeMask = (TypeMask.Terrain | TypeMask.StaticObjects),
					m_CollisionMask = (CollisionMask.OnGround | CollisionMask.Overground)
				};
				m_RaycastSystem.AddInput(this, input);
				NativeArray<RaycastResult> result = m_RaycastSystem.GetResult(this);
				if (result.Length != 0)
				{
					m_RaycastResult = result[0];
				}
			}
			return m_RaycastResult;
		}
		set
		{
			m_RaycastResult = value;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_RaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RaycastSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<InfomodeData>(),
			ComponentType.ReadOnly<InfomodeActive>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfoviewHeatmapData>(),
			ComponentType.ReadOnly<InfoviewBuildingStatusData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfomodeGroup>() };
		array[0] = val;
		m_ActiveInfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_Garbage = new IntTooltip
		{
			path = "garbage",
			icon = "Media/Game/Icons/Garbage.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[Garbage]"),
			unit = "integer"
		};
		m_AirPollution = new IntTooltip
		{
			path = "airPollution",
			icon = "Media/Game/Icons/AirPollution.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[AirPollution]"),
			unit = "integer"
		};
		m_GroundPollution = new IntTooltip
		{
			path = "groundPollution",
			icon = "Media/Game/Icons/GroundPollution.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[GroundPollution]"),
			unit = "integer"
		};
		m_NoisePollution = new IntTooltip
		{
			path = "noisePollution",
			icon = "Media/Game/Icons/NoisePollution.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[NoisePollution]"),
			unit = "integer"
		};
		m_WaterPollution = new IntTooltip
		{
			path = "waterPollution",
			icon = "Media/Game/Icons/WaterPollution.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[WaterPollution]"),
			unit = "integer"
		};
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool != m_DefaultTool || (Object)(object)m_ToolSystem.activeInfoview == (Object)null || ((EntityQuery)(ref m_ActiveInfomodeQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		raycastResult = default(RaycastResult);
		Enumerator<Entity> enumerator = ((EntityQuery)(ref m_ActiveInfomodeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		try
		{
			InfoviewHeatmapData infoviewHeatmapData = default(InfoviewHeatmapData);
			InfoviewBuildingStatusData infoviewBuildingStatusData = default(InfoviewBuildingStatusData);
			GarbageProducer garbageProducer = default(GarbageProducer);
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				if (EntitiesExtensions.TryGetComponent<InfoviewHeatmapData>(((ComponentSystemBase)this).EntityManager, current, ref infoviewHeatmapData))
				{
					switch (infoviewHeatmapData.m_Type)
					{
					case HeatmapData.GroundPollution:
					{
						if (raycastResult.m_Owner == Entity.Null)
						{
							return;
						}
						JobHandle dependencies4;
						NativeArray<GroundPollution> map4 = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies4);
						((JobHandle)(ref dependencies4)).Complete();
						GroundPollution pollution3 = GroundPollutionSystem.GetPollution(raycastResult.m_Hit.m_HitPosition, map4);
						m_GroundPollution.value = pollution3.m_Pollution;
						AddMouseTooltip(m_GroundPollution);
						break;
					}
					case HeatmapData.AirPollution:
					{
						if (raycastResult.m_Owner == Entity.Null)
						{
							return;
						}
						JobHandle dependencies3;
						NativeArray<AirPollution> map3 = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies3);
						((JobHandle)(ref dependencies3)).Complete();
						AirPollution pollution2 = AirPollutionSystem.GetPollution(raycastResult.m_Hit.m_HitPosition, map3);
						m_AirPollution.value = pollution2.m_Pollution;
						AddMouseTooltip(m_AirPollution);
						break;
					}
					case HeatmapData.WaterPollution:
					{
						if (raycastResult.m_Owner == Entity.Null)
						{
							return;
						}
						JobHandle deps;
						WaterSurfaceData data = m_WaterSystem.GetSurfaceData(out deps);
						((JobHandle)(ref deps)).Complete();
						if (WaterUtils.SampleDepth(ref data, raycastResult.m_Hit.m_HitPosition) > 0f)
						{
							float num = WaterUtils.SamplePolluted(ref data, raycastResult.m_Hit.m_HitPosition);
							m_WaterPollution.value = (int)(num * 10000f);
							AddMouseTooltip(m_WaterPollution);
						}
						break;
					}
					case HeatmapData.GroundWaterPollution:
					{
						if (raycastResult.m_Owner == Entity.Null)
						{
							return;
						}
						JobHandle deps2;
						WaterSurfaceData data2 = m_WaterSystem.GetSurfaceData(out deps2);
						((JobHandle)(ref deps2)).Complete();
						if (WaterUtils.SampleDepth(ref data2, raycastResult.m_Hit.m_HitPosition) == 0f)
						{
							JobHandle dependencies2;
							NativeArray<GroundWater> map2 = m_GroundWaterSystem.GetMap(readOnly: true, out dependencies2);
							((JobHandle)(ref dependencies2)).Complete();
							GroundWater groundWater = GroundWaterSystem.GetGroundWater(raycastResult.m_Hit.m_HitPosition, map2);
							m_WaterPollution.value = groundWater.m_Polluted;
							AddMouseTooltip(m_WaterPollution);
						}
						break;
					}
					case HeatmapData.Noise:
					{
						if (raycastResult.m_Owner == Entity.Null)
						{
							return;
						}
						JobHandle dependencies;
						NativeArray<NoisePollution> map = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies);
						((JobHandle)(ref dependencies)).Complete();
						NoisePollution pollution = NoisePollutionSystem.GetPollution(raycastResult.m_Hit.m_HitPosition, map);
						m_NoisePollution.value = pollution.m_Pollution;
						AddMouseTooltip(m_NoisePollution);
						break;
					}
					}
				}
				else if (EntitiesExtensions.TryGetComponent<InfoviewBuildingStatusData>(((ComponentSystemBase)this).EntityManager, current, ref infoviewBuildingStatusData) && infoviewBuildingStatusData.m_Type == BuildingStatusType.GarbageAccumulation)
				{
					if (raycastResult.m_Owner == Entity.Null)
					{
						break;
					}
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Building>(raycastResult.m_Owner) && EntitiesExtensions.TryGetComponent<GarbageProducer>(((ComponentSystemBase)this).EntityManager, raycastResult.m_Owner, ref garbageProducer))
					{
						m_Garbage.value = garbageProducer.m_Garbage;
						AddMouseTooltip(m_Garbage);
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	[Preserve]
	public PollutionTooltipSystem()
	{
	}
}
