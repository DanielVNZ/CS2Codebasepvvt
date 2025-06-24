using UnityEngine;

namespace Game.Input;

public class SmoothVector2Processor : SmoothProcessor<Vector2>
{
	private const float kDelta = 1E-12f;

	protected override Vector2 Smooth(Vector2 value, ref Vector2 lastValue, float delta)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (m_Smoothing > 0f)
		{
			float num = Mathf.Pow(m_Smoothing, delta);
			value = Vector2.Lerp(value, lastValue, num);
			if (((Vector2)(ref value)).sqrMagnitude < 1E-12f)
			{
				value = Vector2.zero;
			}
		}
		lastValue = value;
		if (m_Time)
		{
			value *= Time.deltaTime;
		}
		return value;
	}
}
