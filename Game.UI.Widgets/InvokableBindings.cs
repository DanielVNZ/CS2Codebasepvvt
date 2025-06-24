using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using UnityEngine;

namespace Game.UI.Widgets;

public class InvokableBindings : IWidgetBindingFactory
{
	public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
	{
		yield return (IBinding)(object)new TriggerBinding<IWidget>(group, "invoke", (Action<IWidget>)delegate(IWidget widget)
		{
			if (widget is IInvokable invokable)
			{
				invokable.Invoke();
			}
			else
			{
				Debug.LogError((object)((widget != null) ? "Widget does not implement IInvokable" : "Invalid widget path"));
			}
		}, pathResolver);
	}
}
