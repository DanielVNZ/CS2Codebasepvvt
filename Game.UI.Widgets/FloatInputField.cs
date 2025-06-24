using Unity.Mathematics;

namespace Game.UI.Widgets;

public class FloatInputField : FloatField<double>
{
	protected override double defaultMin => double.MinValue;

	protected override double defaultMax => double.MaxValue;

	public override double ToFieldType(double4 value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return value.x;
	}
}
