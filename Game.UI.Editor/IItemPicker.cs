using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.UI.Binding;
using Game.UI.Localization;
using Game.UI.Widgets;

namespace Game.UI.Editor;

public interface IItemPicker
{
	public class Item : IJsonWritable
	{
		public LocalizedString displayName { get; set; }

		public LocalizedString? tooltip { get; set; }

		[CanBeNull]
		public string image { get; set; }

		[CanBeNull]
		public string badge { get; set; }

		public bool directory { get; set; }

		public bool favorite { get; set; }

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("displayName");
			JsonWriterExtensions.Write<LocalizedString>(writer, displayName);
			writer.PropertyName("tooltip");
			JsonWriterExtensions.Write<LocalizedString>(writer, tooltip);
			writer.PropertyName("image");
			writer.Write(image);
			writer.PropertyName("directory");
			writer.Write(directory);
			writer.PropertyName("favorite");
			writer.Write(favorite);
			writer.PropertyName("badge");
			writer.Write(badge);
			writer.TypeEnd();
		}
	}

	public class Bindings : IWidgetBindingFactory
	{
		public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
		{
			yield return (IBinding)(object)new TriggerBinding<IWidget, int, int>(group, "setVisibleItemRange", (Action<IWidget, int, int>)delegate(IWidget widget, int startIndex, int endIndex)
			{
				if (widget is IItemPicker itemPicker)
				{
					itemPicker.visibleStartIndex = startIndex;
					itemPicker.visibleEndIndex = endIndex;
				}
			}, pathResolver, (IReader<int>)null, (IReader<int>)null);
			yield return (IBinding)(object)new TriggerBinding<IWidget, int>(group, "setItemSelected", (Action<IWidget, int>)delegate(IWidget widget, int index)
			{
				if (widget is IItemPicker itemPicker)
				{
					itemPicker.selectedIndex = index;
					onValueChanged(widget);
				}
			}, pathResolver, (IReader<int>)null);
			yield return (IBinding)(object)new TriggerBinding<IWidget, int, bool>(group, "setItemFavorite", (Action<IWidget, int, bool>)delegate(IWidget widget, int index, bool favorite)
			{
				if (widget is IItemPicker itemPicker)
				{
					itemPicker.SetFavorite(index, favorite);
				}
			}, pathResolver, (IReader<int>)null, (IReader<bool>)null);
		}
	}

	int selectedIndex { get; set; }

	int visibleStartIndex { get; set; }

	int visibleEndIndex { get; set; }

	void SetFavorite(int index, bool favorite);
}
