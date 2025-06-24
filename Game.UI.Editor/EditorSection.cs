using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.UI.Binding;
using Game.Reflection;
using Game.UI.Widgets;
using UnityEngine;

namespace Game.UI.Editor;

public class EditorSection : ExpandableGroup, IDisableCallback
{
	public class Bindings : IWidgetBindingFactory
	{
		public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
		{
			yield return (IBinding)(object)new TriggerBinding<IWidget>(group, "deleteEditorSection", (Action<IWidget>)delegate(IWidget widget)
			{
				if (widget is EditorSection editorSection)
				{
					editorSection.onDelete?.Invoke();
					onValueChanged(widget);
				}
			}, pathResolver);
			yield return (IBinding)(object)new TriggerBinding<IWidget, bool>(group, "setEditorSectionActive", (Action<IWidget, bool>)delegate(IWidget widget, bool active)
			{
				if (widget is EditorSection editorSection)
				{
					editorSection.active?.SetTypedValue(active);
					onValueChanged(widget);
				}
			}, pathResolver, (IReader<bool>)null);
		}
	}

	public static readonly Color kPrefabColor = new Color(27f / 85f, 0.2509804f, 0.20784314f);

	private bool m_Active;

	private Color? m_Color;

	[CanBeNull]
	public Action onDelete { get; set; }

	[CanBeNull]
	public ITypedValueAccessor<bool> active { get; set; }

	public bool primary { get; set; }

	[CanBeNull]
	public Color? color
	{
		get
		{
			return m_Color;
		}
		set
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Color? val = value;
			Color? val2 = m_Color;
			if (val.HasValue != val2.HasValue || (val.HasValue && !(val.GetValueOrDefault() == val2.GetValueOrDefault())))
			{
				m_Color = value;
				SetPropertiesChanged();
			}
		}
	}

	protected override WidgetChanges Update()
	{
		WidgetChanges widgetChanges = base.Update();
		bool flag = active?.GetTypedValue() ?? true;
		if (flag != m_Active)
		{
			widgetChanges |= WidgetChanges.Properties;
			m_Active = flag;
		}
		return widgetChanges;
	}

	protected override void WriteProperties(IJsonWriter writer)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		base.WriteProperties(writer);
		writer.PropertyName("expandable");
		writer.Write(base.children.Count != 0);
		writer.PropertyName("deletable");
		writer.Write(onDelete != null);
		writer.PropertyName("activatable");
		writer.Write(active != null);
		writer.PropertyName("active");
		writer.Write(m_Active);
		writer.PropertyName("primary");
		writer.Write(primary);
		writer.PropertyName("color");
		if (color.HasValue)
		{
			UnityWriters.Write(writer, color.Value);
		}
		else
		{
			writer.WriteNull();
		}
	}
}
