using Unity.Mathematics;

namespace Game.UI.Widgets;

public class EulerAnglesField : FloatField<float3>
{
	protected override float3 defaultMin => new float3(float.MinValue);

	protected override float3 defaultMax => new float3(float.MaxValue);

	public override float3 ToFieldType(double4 value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new float3(((double4)(ref value)).xyz);
	}
}
