using Unity.Mathematics;

namespace Game.UI.Widgets;

public class Float2InputField : FloatField<float2>
{
	protected override float2 defaultMin => new float2(float.MinValue);

	protected override float2 defaultMax => new float2(float.MaxValue);

	public override float2 ToFieldType(double4 value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new float2(((double4)(ref value)).xy);
	}
}
