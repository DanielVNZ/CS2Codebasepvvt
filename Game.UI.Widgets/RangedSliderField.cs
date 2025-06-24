using System;
using Colossal.UI.Binding;
using Unity.Mathematics;

namespace Game.UI.Widgets;

public class RangedSliderField : FloatSliderField<float>, IIconProvider
{
	protected override float defaultMin => float.MinValue;

	protected override float defaultMax => float.MaxValue;

	public float[] ranges { get; set; }

	public Func<string> iconSrc { get; set; }

	public override float ToFieldType(double4 value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return (float)value.x;
	}

	protected override void WriteProperties(IJsonWriter writer)
	{
		base.WriteProperties(writer);
		writer.PropertyName("ranges");
		int num = ((ranges != null) ? ranges.Length : 0);
		JsonWriterExtensions.ArrayBegin(writer, num);
		for (int i = 0; i < num; i++)
		{
			writer.Write(ranges[i]);
		}
		writer.ArrayEnd();
		writer.PropertyName("iconSrc");
		writer.Write(iconSrc());
	}
}
