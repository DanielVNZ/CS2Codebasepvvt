using System;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.UI;
using Game.Reflection;
using Game.UI.Widgets;
using Unity.Entities;

namespace Game.UI.Editor;

public class UIIconField : IFieldBuilderFactory
{
	public FieldBuilder TryCreate(Type memberType, object[] attributes)
	{
		return delegate(IValueAccessor accessor)
		{
			CastAccessor<string> castAccessor = new CastAccessor<string>(accessor);
			StringInputField stringInputField = new StringInputField
			{
				displayName = "URI",
				accessor = castAccessor
			};
			IconButton iconPicker = new IconButton
			{
				icon = ((accessor.GetValue() as string) ?? string.Empty)
			};
			iconPicker.action = delegate
			{
				World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<InspectorPanelSystem>().ShowThumbnailPicker(delegate(Hash128 hash)
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					string text = string.Empty;
					ImageAsset val = default(ImageAsset);
					if (AssetDatabase.global.TryGetAsset<ImageAsset>(hash, ref val))
					{
						text = UIExtensions.ToGlobalUri((AssetData)(object)val);
					}
					castAccessor.SetValue(text);
					iconPicker.icon = text;
				});
			};
			return new Group
			{
				displayName = "Icon",
				children = new IWidget[2] { stringInputField, iconPicker }
			};
		};
	}
}
