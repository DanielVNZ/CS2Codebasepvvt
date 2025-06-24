using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.UI.Widgets;

namespace Game.UI.Editor;

public class ListField : Widget
{
	public struct Item : IJsonWritable
	{
		public string m_Label;

		public bool m_Removable;

		public object m_Data;

		public string[] m_SubItems;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().Name);
			writer.PropertyName("label");
			writer.Write(m_Label);
			writer.PropertyName("removable");
			writer.Write(m_Removable);
			writer.PropertyName("subItems");
			JsonWriterExtensions.Write(writer, m_SubItems);
			writer.TypeEnd();
		}
	}

	public class Bindings : IWidgetBindingFactory
	{
		public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
		{
			yield return (IBinding)(object)new TriggerBinding<IWidget, int>(group, "removeListItem", (Action<IWidget, int>)delegate(IWidget widget, int index)
			{
				if (widget is ListField listField)
				{
					listField.RemoveItem(index);
					onValueChanged(widget);
				}
			}, pathResolver, (IReader<int>)null);
		}
	}

	public List<Item> m_Items;

	public Action<int> onItemRemoved;

	protected override void WriteProperties(IJsonWriter writer)
	{
		base.WriteProperties(writer);
		writer.PropertyName("items");
		JsonWriterExtensions.Write<Item>(writer, (IList<Item>)m_Items);
	}

	protected void RemoveItem(int index)
	{
		onItemRemoved?.Invoke(index);
	}
}
