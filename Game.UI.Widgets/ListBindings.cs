using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using UnityEngine;

namespace Game.UI.Widgets;

public class ListBindings : IWidgetBindingFactory
{
	public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
	{
		yield return (IBinding)(object)new TriggerBinding<IWidget>(group, "addListElement", (Action<IWidget>)delegate(IWidget widget)
		{
			if (widget is IListWidget listWidget)
			{
				listWidget.AddElement();
				onValueChanged(widget);
			}
			else
			{
				Debug.LogError((object)((widget != null) ? "Widget does not implement IListContainer" : "Invalid widget path"));
			}
		}, pathResolver);
		yield return (IBinding)(object)new TriggerBinding<IWidget, int>(group, "duplicateListElement", (Action<IWidget, int>)delegate(IWidget widget, int index)
		{
			if (widget is IListWidget listWidget)
			{
				listWidget.DuplicateElement(index);
				onValueChanged(widget);
			}
			else
			{
				Debug.LogError((object)((widget != null) ? "Widget does not implement IListContainer" : "Invalid widget path"));
			}
		}, pathResolver, (IReader<int>)null);
		yield return (IBinding)(object)new TriggerBinding<IWidget, int, int>(group, "moveListElement", (Action<IWidget, int, int>)delegate(IWidget widget, int fromIndex, int toIndex)
		{
			if (widget is IListWidget listWidget)
			{
				listWidget.MoveElement(fromIndex, toIndex);
				onValueChanged(widget);
			}
			else
			{
				Debug.LogError((object)((widget != null) ? "Widget does not implement IListContainer" : "Invalid widget path"));
			}
		}, pathResolver, (IReader<int>)null, (IReader<int>)null);
		yield return (IBinding)(object)new TriggerBinding<IWidget, int>(group, "deleteListElement", (Action<IWidget, int>)delegate(IWidget widget, int index)
		{
			if (widget is IListWidget listWidget)
			{
				listWidget.DeleteElement(index);
				onValueChanged(widget);
			}
			else
			{
				Debug.LogError((object)((widget != null) ? "Widget does not implement IListContainer" : "Invalid widget path"));
			}
		}, pathResolver, (IReader<int>)null);
		yield return (IBinding)(object)new TriggerBinding<IWidget>(group, "clearList", (Action<IWidget>)delegate(IWidget widget)
		{
			if (widget is IListWidget listWidget)
			{
				listWidget.Clear();
				onValueChanged(widget);
			}
			else
			{
				Debug.LogError((object)((widget != null) ? "Widget does not implement IListContainer" : "Invalid widget path"));
			}
		}, pathResolver);
	}
}
