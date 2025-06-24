using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.UI.Binding;
using Game.UI.Widgets;
using Unity.Mathematics;

namespace Game.UI.Tooltip;

public class TooltipGroup : Widget
{
	public enum Alignment
	{
		Start,
		Center,
		End
	}

	[Flags]
	public enum Category
	{
		None = 0,
		Network = 1
	}

	private float2 m_Position;

	private Category m_Category;

	private Alignment m_HorizontalAlignment;

	private Alignment m_VerticalAlignment;

	private List<IWidget> m_LastChildren = new List<IWidget>();

	public float2 position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Position;
		}
		set
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (!((float2)(ref value)).Equals(m_Position))
			{
				m_Position = value;
				SetPropertiesChanged();
			}
		}
	}

	public Alignment horizontalAlignment
	{
		get
		{
			return m_HorizontalAlignment;
		}
		set
		{
			if (value != m_HorizontalAlignment)
			{
				m_HorizontalAlignment = value;
				SetPropertiesChanged();
			}
		}
	}

	public Alignment verticalAlignment
	{
		get
		{
			return m_VerticalAlignment;
		}
		set
		{
			if (value != m_VerticalAlignment)
			{
				m_VerticalAlignment = value;
				SetPropertiesChanged();
			}
		}
	}

	public Category category
	{
		get
		{
			return m_Category;
		}
		set
		{
			if (value != m_Category)
			{
				m_Category = value;
				SetPropertiesChanged();
			}
		}
	}

	public IList<IWidget> children { get; set; } = new List<IWidget>();

	public override IList<IWidget> visibleChildren => m_LastChildren;

	protected override WidgetChanges Update()
	{
		WidgetChanges widgetChanges = base.Update();
		if (!m_LastChildren.SequenceEqual(children))
		{
			widgetChanges |= WidgetChanges.Children;
			m_LastChildren.Clear();
			m_LastChildren.AddRange(children);
			ContainerExtensions.SetDefaults(m_LastChildren);
		}
		return widgetChanges;
	}

	protected override void WriteProperties(IJsonWriter writer)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.WriteProperties(writer);
		writer.PropertyName("position");
		MathematicsWriters.Write(writer, position);
		writer.PropertyName("horizontalAlignment");
		writer.Write((int)horizontalAlignment);
		writer.PropertyName("verticalAlignment");
		writer.Write((int)verticalAlignment);
		writer.PropertyName("category");
		writer.Write((int)category);
	}
}
