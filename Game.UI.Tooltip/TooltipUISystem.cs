using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.Input;
using Game.UI.Widgets;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

public class TooltipUISystem : UISystemBase
{
	private const string kGroup = "tooltip";

	private static readonly float2 kTooltipPointerDistance = new float2(0f, 16f);

	private UpdateSystem m_UpdateSystem;

	private WidgetBindings m_WidgetBindings;

	public override GameMode gameMode => GameMode.GameOrEditor;

	public List<TooltipGroup> groups { get; private set; }

	public TooltipGroup mouseGroup { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		AddUpdateBinding((IUpdateBinding)(object)(m_WidgetBindings = new WidgetBindings("tooltip", "groups")));
		groups = new List<TooltipGroup>();
		mouseGroup = new TooltipGroup
		{
			path = "mouse",
			position = default(float2),
			horizontalAlignment = TooltipGroup.Alignment.Start,
			verticalAlignment = TooltipGroup.Alignment.Start
		};
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (m_WidgetBindings.active)
		{
			m_WidgetBindings.children.Clear();
			groups.Clear();
			mouseGroup.children.Clear();
			if (!InputManager.instance.mouseOverUI)
			{
				m_UpdateSystem.Update(SystemUpdatePhase.UITooltip);
				if (InputManager.instance.mouseOnScreen && mouseGroup.children.Count > 0)
				{
					Vector3 mousePosition = InputManager.instance.mousePosition;
					mouseGroup.position = math.round(new float2(mousePosition.x, (float)Screen.height - mousePosition.y) + kTooltipPointerDistance);
					m_WidgetBindings.children.Add(mouseGroup);
				}
				foreach (TooltipGroup group in groups)
				{
					m_WidgetBindings.children.Add(group);
				}
			}
		}
		base.OnUpdate();
	}

	[Preserve]
	public TooltipUISystem()
	{
	}
}
