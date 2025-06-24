using Unity.Mathematics;

namespace Game.UI.Widgets;

public class Float4SliderField : FloatSliderField<float4>
{
	protected override float4 defaultMin => new float4(float.MinValue);

	protected override float4 defaultMax => new float4(float.MaxValue);

	public override float4 ToFieldType(double4 value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new float4(value);
	}
}
