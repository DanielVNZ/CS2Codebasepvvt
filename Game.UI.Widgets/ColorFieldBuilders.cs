using System;
using Game.Reflection;
using UnityEngine;

namespace Game.UI.Widgets;

public class ColorFieldBuilders : IFieldBuilderFactory
{
	public FieldBuilder TryCreate(Type memberType, object[] attributes)
	{
		if (memberType == typeof(Color))
		{
			return CreateColorFieldBuilder(attributes, ToColor, FromColor);
		}
		if (memberType == typeof(Color32))
		{
			return CreateColorFieldBuilder(attributes, ToColor32, FromColor32);
		}
		return null;
		static object FromColor(Color value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return value;
		}
		static object FromColor32(Color value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Color32.op_Implicit(value);
		}
		static Color ToColor(object value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return (Color)value;
		}
		static Color ToColor32(object value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Color32.op_Implicit((Color32)value);
		}
	}

	private static FieldBuilder CreateColorFieldBuilder(object[] attributes, Converter<object, Color> fromObject, Converter<Color, object> toObject)
	{
		bool hdr = false;
		bool showAlpha = false;
		WidgetAttributeUtils.GetColorUsage(attributes, ref hdr, ref showAlpha);
		return (IValueAccessor accessor) => new ColorField
		{
			hdr = hdr,
			showAlpha = showAlpha,
			accessor = new CastAccessor<Color>(accessor, fromObject, toObject)
		};
	}
}
