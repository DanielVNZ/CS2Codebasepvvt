using System.Collections.Generic;
using Game.Rendering;
using Game.UI.Widgets;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

public class GuideLineTooltipSystem : TooltipSystemBase
{
	private GuideLinesSystem m_GuideLinesSystem;

	private List<TooltipGroup> m_Groups;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_GuideLinesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GuideLinesSystem>();
		m_Groups = new List<TooltipGroup>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeList<GuideLinesSystem.TooltipInfo> tooltips = m_GuideLinesSystem.GetTooltips(out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		for (int i = 0; i < tooltips.Length; i++)
		{
			GuideLinesSystem.TooltipInfo tooltipInfo = tooltips[i];
			if (m_Groups.Count <= i)
			{
				m_Groups.Add(new TooltipGroup
				{
					path = $"guideLineTooltip{i}",
					horizontalAlignment = TooltipGroup.Alignment.Center,
					verticalAlignment = TooltipGroup.Alignment.Center,
					category = TooltipGroup.Category.Network,
					children = { (IWidget)new FloatTooltip() }
				});
			}
			TooltipGroup tooltipGroup = m_Groups[i];
			bool onScreen;
			float2 val = TooltipSystemBase.WorldToTooltipPos(float3.op_Implicit(tooltipInfo.m_Position), out onScreen);
			float2 position = tooltipGroup.position;
			if (!((float2)(ref position)).Equals(val))
			{
				tooltipGroup.position = val;
				tooltipGroup.SetChildrenChanged();
			}
			FloatTooltip floatTooltip = tooltipGroup.children[0] as FloatTooltip;
			switch (tooltipInfo.m_Type)
			{
			case GuideLinesSystem.TooltipType.Angle:
				floatTooltip.icon = "Media/Glyphs/Angle.svg";
				floatTooltip.value = tooltipInfo.m_Value;
				floatTooltip.unit = "angle";
				break;
			case GuideLinesSystem.TooltipType.Length:
				floatTooltip.icon = "Media/Glyphs/Length.svg";
				floatTooltip.value = tooltipInfo.m_Value;
				floatTooltip.unit = "length";
				break;
			}
			AddGroup(tooltipGroup);
		}
	}

	[Preserve]
	public GuideLineTooltipSystem()
	{
	}
}
