using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using UnityEngine;

namespace Game.UI.Widgets;

public class SettableBindings : IWidgetBindingFactory
{
	public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
	{
		yield return (IBinding)new RawTriggerBinding(group, "setValue", (Action<IJsonReader>)delegate(IJsonReader reader)
		{
			IWidget widget = default(IWidget);
			pathResolver.Read(reader, ref widget);
			if (widget is ISettable settable)
			{
				settable.SetValue(reader);
				if (settable.shouldTriggerValueChangedEvent)
				{
					onValueChanged(widget);
				}
			}
			else
			{
				reader.SkipValue();
				Debug.LogError((object)((widget != null) ? "Widget does not implement ISettable" : "Invalid widget path"));
			}
		});
	}
}
