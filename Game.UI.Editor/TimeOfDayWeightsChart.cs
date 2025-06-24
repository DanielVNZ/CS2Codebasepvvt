using Colossal.UI.Binding;
using Game.Reflection;
using Game.UI.Widgets;
using Unity.Mathematics;

namespace Game.UI.Editor;

public class TimeOfDayWeightsChart : Widget
{
	private float4 m_Value;

	public float min { get; set; }

	public float max { get; set; }

	public ITypedValueAccessor<float4> accessor { get; set; }

	protected override WidgetChanges Update()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		WidgetChanges widgetChanges = base.Update();
		float4 val = math.unlerp(float4.op_Implicit(min), float4.op_Implicit(max), accessor.GetTypedValue());
		if (!object.Equals(val, m_Value))
		{
			widgetChanges |= WidgetChanges.Properties;
			m_Value = val;
		}
		return widgetChanges;
	}

	protected override void WriteProperties(IJsonWriter writer)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.WriteProperties(writer);
		writer.PropertyName("value");
		MathematicsWriters.Write(writer, m_Value);
	}
}
