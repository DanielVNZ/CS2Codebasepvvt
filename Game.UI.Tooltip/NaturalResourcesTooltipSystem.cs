using System;
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

public class NaturalResourcesTooltipSystem : TooltipSystemBase
{
	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private RaycastSystem m_RaycastSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_ActiveInfomodeQuery;

	private IntTooltip m_Fertility;

	private IntTooltip m_Wood;

	private IntTooltip m_Oil;

	private IntTooltip m_Ore;

	private IntTooltip m_Fish;

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
					m_TypeMask = TypeMask.Terrain,
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
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_RaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RaycastSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<InfomodeData>(),
			ComponentType.ReadOnly<InfomodeActive>(),
			ComponentType.ReadOnly<InfoviewHeatmapData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<InfomodeGroup>() };
		array[0] = val;
		m_ActiveInfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_Fertility = new IntTooltip
		{
			path = "fertility",
			icon = "Media/Game/Icons/Fertility.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[Fertility]"),
			unit = "integer"
		};
		m_Wood = new IntTooltip
		{
			path = "wood",
			icon = "Media/Game/Resources/Wood.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[Wood]"),
			unit = "integer"
		};
		m_Oil = new IntTooltip
		{
			path = "oil",
			icon = "Media/Game/Resources/Oil.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[Oil]"),
			unit = "integer"
		};
		m_Ore = new IntTooltip
		{
			path = "ore",
			icon = "Media/Game/Resources/Ore.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[Ore]"),
			unit = "integer"
		};
		m_Fish = new IntTooltip
		{
			path = "fish",
			icon = "Media/Game/Resources/Fish.svg",
			label = LocalizedString.Id("DefaultTool.INFOMODE_TOOLTIP[Fish]"),
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
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool != m_DefaultTool || (Object)(object)m_ToolSystem.activeInfoview == (Object)null || ((EntityQuery)(ref m_ActiveInfomodeQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		raycastResult = default(RaycastResult);
		NativeArray<Entity> val = ((EntityQuery)(ref m_ActiveInfomodeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		Enumerator<Entity> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				switch (((EntityManager)(ref entityManager)).GetComponentData<InfoviewHeatmapData>(current).m_Type)
				{
				case HeatmapData.Fertility:
				{
					if (raycastResult.m_Owner == Entity.Null)
					{
						return;
					}
					JobHandle deps2;
					WaterSurfaceData data2 = m_WaterSystem.GetSurfaceData(out deps2);
					((JobHandle)(ref deps2)).Complete();
					if (!(WaterUtils.SampleDepth(ref data2, raycastResult.m_Hit.m_HitPosition) > 0f))
					{
						JobHandle dependencies4;
						NativeArray<NaturalResourceCell> map4 = m_NaturalResourceSystem.GetMap(readOnly: true, out dependencies4);
						((JobHandle)(ref dependencies4)).Complete();
						NaturalResourceAmount fertilityAmount = NaturalResourceSystem.GetFertilityAmount(raycastResult.m_Hit.m_HitPosition, map4);
						if (fertilityAmount.m_Base > fertilityAmount.m_Used)
						{
							m_Fertility.value = fertilityAmount.m_Base - fertilityAmount.m_Used;
							AddMouseTooltip(m_Fertility);
						}
					}
					break;
				}
				case HeatmapData.Oil:
				{
					if (raycastResult.m_Owner == Entity.Null)
					{
						return;
					}
					JobHandle dependencies3;
					NativeArray<NaturalResourceCell> map3 = m_NaturalResourceSystem.GetMap(readOnly: true, out dependencies3);
					((JobHandle)(ref dependencies3)).Complete();
					NaturalResourceAmount oilAmount = NaturalResourceSystem.GetOilAmount(raycastResult.m_Hit.m_HitPosition, map3);
					if (oilAmount.m_Base > oilAmount.m_Used)
					{
						m_Oil.value = oilAmount.m_Base - oilAmount.m_Used;
						AddMouseTooltip(m_Oil);
					}
					break;
				}
				case HeatmapData.Ore:
				{
					if (raycastResult.m_Owner == Entity.Null)
					{
						return;
					}
					JobHandle dependencies2;
					NativeArray<NaturalResourceCell> map2 = m_NaturalResourceSystem.GetMap(readOnly: true, out dependencies2);
					((JobHandle)(ref dependencies2)).Complete();
					NaturalResourceAmount oreAmount = NaturalResourceSystem.GetOreAmount(raycastResult.m_Hit.m_HitPosition, map2);
					if (oreAmount.m_Base > oreAmount.m_Used)
					{
						m_Ore.value = oreAmount.m_Base - oreAmount.m_Used;
						AddMouseTooltip(m_Ore);
					}
					break;
				}
				case HeatmapData.Fish:
				{
					if (raycastResult.m_Owner == Entity.Null)
					{
						return;
					}
					JobHandle deps;
					WaterSurfaceData data = m_WaterSystem.GetSurfaceData(out deps);
					((JobHandle)(ref deps)).Complete();
					if (!(WaterUtils.SampleDepth(ref data, raycastResult.m_Hit.m_HitPosition) <= 0f))
					{
						JobHandle dependencies;
						NativeArray<NaturalResourceCell> map = m_NaturalResourceSystem.GetMap(readOnly: true, out dependencies);
						((JobHandle)(ref dependencies)).Complete();
						NaturalResourceAmount fishAmount = NaturalResourceSystem.GetFishAmount(raycastResult.m_Hit.m_HitPosition, map);
						if (fishAmount.m_Base > fishAmount.m_Used)
						{
							m_Fish.value = fishAmount.m_Base - fishAmount.m_Used;
							AddMouseTooltip(m_Fish);
						}
					}
					break;
				}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
	}

	[Preserve]
	public NaturalResourcesTooltipSystem()
	{
	}
}
