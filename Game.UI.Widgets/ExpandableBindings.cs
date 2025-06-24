using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using UnityEngine;

namespace Game.UI.Widgets;

public class ExpandableBindings : IWidgetBindingFactory
{
	public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
	{
		yield return (IBinding)(object)new TriggerBinding<IWidget, bool>(group, "setExpanded", (Action<IWidget, bool>)delegate(IWidget widget, bool expanded)
		{
			if (widget is IExpandable expandable)
			{
				expandable.expanded = expanded;
			}
			else
			{
				Debug.LogError((object)((widget != null) ? "Widget does not implement IExpandable" : "Invalid widget path"));
			}
		}, pathResolver, (IReader<bool>)null);
	}
}
