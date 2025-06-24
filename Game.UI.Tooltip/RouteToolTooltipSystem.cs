using Game.Common;
using Game.Routes;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

public class RouteToolTooltipSystem : TooltipSystemBase
{
	private ToolSystem m_ToolSystem;

	private RouteToolSystem m_RouteTool;

	private ImageSystem m_ImageSystem;

	private NameSystem m_NameSystem;

	private EntityQuery m_TempRouteQuery;

	private EntityQuery m_TempStopQuery;

	private NameTooltip m_StopName;

	private NameTooltip m_RouteName;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_RouteTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RouteToolSystem>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Route>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array[0] = val;
		m_TempRouteQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<TransportStop>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array2[0] = val;
		m_TempStopQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_StopName = new NameTooltip
		{
			path = "routeToolStopName",
			nameBinder = m_NameSystem
		};
		m_RouteName = new NameTooltip
		{
			path = "routeToolRouteName",
			nameBinder = m_NameSystem
		};
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (m_ToolSystem.activeTool == m_RouteTool && m_RouteTool.tooltip != RouteToolSystem.Tooltip.None)
		{
			switch (m_RouteTool.tooltip)
			{
			case RouteToolSystem.Tooltip.CreateRoute:
			case RouteToolSystem.Tooltip.AddWaypoint:
			case RouteToolSystem.Tooltip.CompleteRoute:
				TryAddStopName();
				break;
			case RouteToolSystem.Tooltip.CreateOrModify:
				TryAddStopName();
				TryAddRouteName();
				break;
			case RouteToolSystem.Tooltip.InsertWaypoint:
			case RouteToolSystem.Tooltip.MoveWaypoint:
			case RouteToolSystem.Tooltip.MergeWaypoints:
			case RouteToolSystem.Tooltip.RemoveWaypoint:
				TryAddStopName();
				TryAddRouteName();
				break;
			default:
				TryAddRouteName();
				break;
			}
		}
	}

	public void TryAddStopName()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_TempStopQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<Temp> val = ((EntityQuery)(ref m_TempStopQuery)).ToComponentDataArray<Temp>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				Temp temp = val[i];
				if (temp.m_Original != Entity.Null)
				{
					AddMouseTooltip(m_StopName);
					m_StopName.icon = m_ImageSystem.GetInstanceIcon(temp.m_Original);
					m_StopName.entity = temp.m_Original;
					break;
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	public void TryAddRouteName()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_TempRouteQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<Temp> val = ((EntityQuery)(ref m_TempRouteQuery)).ToComponentDataArray<Temp>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				Temp temp = val[i];
				if (temp.m_Original != Entity.Null)
				{
					AddMouseTooltip(m_RouteName);
					m_RouteName.icon = m_ImageSystem.GetInstanceIcon(temp.m_Original);
					m_RouteName.entity = temp.m_Original;
					break;
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	[Preserve]
	public RouteToolTooltipSystem()
	{
	}
}
