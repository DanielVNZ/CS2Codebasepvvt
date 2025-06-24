using System;
using Colossal.UI.Binding;
using Unity.Mathematics;

namespace Game.UI.Widgets;

public class GradientSliderField : FloatSliderField<float>, IIconProvider
{
	protected override float defaultMin => float.MinValue;

	protected override float defaultMax => float.MaxValue;

	public ColorGradient gradient { get; set; }

	public Func<string> iconSrc { get; set; }

	public override float ToFieldType(double4 value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return (float)value.x;
	}

	protected override void WriteProperties(IJsonWriter writer)
	{
		base.WriteProperties(writer);
		writer.PropertyName("gradient");
		JsonWriterExtensions.Write<ColorGradient>(writer, gradient);
		writer.PropertyName("iconSrc");
		writer.Write(iconSrc());
	}
}
