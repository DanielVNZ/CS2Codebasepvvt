using System.Linq;
using Game.UI.Widgets;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

public abstract class TooltipSystemBase : GameSystemBase
{
	private TooltipUISystem m_TooltipUISystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_TooltipUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TooltipUISystem>();
	}

	protected void AddGroup(TooltipGroup group)
	{
		if (group.path != PathSegment.Empty && m_TooltipUISystem.groups.Any((TooltipGroup g) => g.path == group.path))
		{
			Debug.LogError((object)$"Trying to add tooltip group with duplicate path '{group.path}'");
		}
		else
		{
			m_TooltipUISystem.groups.Add(group);
		}
	}

	protected void AddMouseTooltip(IWidget tooltip)
	{
		if (tooltip.path != PathSegment.Empty && m_TooltipUISystem.mouseGroup.children.Any((IWidget t) => t.path == tooltip.path))
		{
			Debug.LogError((object)$"Trying to add mouse tooltip with duplicate path '{tooltip.path}'");
		}
		else
		{
			m_TooltipUISystem.mouseGroup.children.Add(tooltip);
		}
	}

	protected static float2 WorldToTooltipPos(Vector3 worldPos, out bool onScreen)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		float3 val = float3.op_Implicit(Camera.main.WorldToScreenPoint(worldPos));
		float2 xy = ((float3)(ref val)).xy;
		xy.y = (float)Screen.height - xy.y;
		onScreen = xy.x >= 0f && xy.y >= 0f && xy.x <= (float)Screen.width && xy.y <= (float)Screen.height;
		return xy;
	}

	[Preserve]
	protected TooltipSystemBase()
	{
	}
}
