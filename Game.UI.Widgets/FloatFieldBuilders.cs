using System;
using Game.Reflection;
using Unity.Mathematics;
using UnityEngine;

namespace Game.UI.Widgets;

public class FloatFieldBuilders : IFieldBuilderFactory
{
	private const double kGlobalValueRange = 10000000.0;

	public FieldBuilder TryCreate(Type memberType, object[] attributes)
	{
		if (memberType == typeof(float))
		{
			return CreateFloatFieldBuilder(attributes, -3.4028234663852886E+38, 3.4028234663852886E+38, ToFloat, FromFloat);
		}
		if (memberType == typeof(double))
		{
			return CreateFloatFieldBuilder(attributes, double.MinValue, double.MaxValue, ToDouble, FromDouble);
		}
		if (memberType == typeof(float2))
		{
			return CreateFloatFieldBuilder<Float2InputField, float2>(attributes);
		}
		if (memberType == typeof(Vector2))
		{
			return CreateFloatFieldBuilder<Float2InputField, float2>(attributes, ToVector, FromVector);
		}
		if (memberType == typeof(float3))
		{
			return CreateFloatFieldBuilder<Float3InputField, float3>(attributes);
		}
		if (memberType == typeof(Vector3))
		{
			return CreateFloatFieldBuilder<Float3InputField, float3>(attributes, ToVector3, FromVector3);
		}
		if (memberType == typeof(quaternion))
		{
			return CreateFloatFieldBuilder<EulerAnglesField, float3>(attributes, ToEulerAngles, FromEulerAngles);
		}
		if (memberType == typeof(Quaternion))
		{
			return CreateFloatFieldBuilder<EulerAnglesField, float3>(attributes, ToEulerAngles2, FromEulerAngles2);
		}
		if (memberType == typeof(float4))
		{
			return CreateFloatFieldBuilder<Float4InputField, float4>(attributes);
		}
		if (memberType == typeof(Vector4))
		{
			return CreateFloatFieldBuilder<Float4InputField, float4>(attributes, ToVector4, FromVector4);
		}
		return null;
		static object FromDouble(double value)
		{
			return value;
		}
		static object FromEulerAngles(float3 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return quaternion.op_Implicit(Quaternion.Euler(float3.op_Implicit(value)));
		}
		static object FromEulerAngles2(float3 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Quaternion.Euler(float3.op_Implicit(value));
		}
		static object FromFloat(double value)
		{
			return (float)value;
		}
		static object FromVector(float2 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float2.op_Implicit(value);
		}
		static object FromVector3(float3 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float3.op_Implicit(value);
		}
		static object FromVector4(float4 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float4.op_Implicit(value);
		}
		static double ToDouble(object value)
		{
			return (double)value;
		}
		static float3 ToEulerAngles(object value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = quaternion.op_Implicit((quaternion)value);
			return float3.op_Implicit(((Quaternion)(ref val)).eulerAngles);
		}
		static float3 ToEulerAngles2(object value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = (Quaternion)value;
			return float3.op_Implicit(((Quaternion)(ref val)).eulerAngles);
		}
		static double ToFloat(object value)
		{
			return (float)value;
		}
		static float2 ToVector(object value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return float2.op_Implicit((Vector2)value);
		}
		static float3 ToVector3(object value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return float3.op_Implicit((Vector3)value);
		}
		static float4 ToVector4(object value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return float4.op_Implicit((Vector4)value);
		}
	}

	private static FieldBuilder CreateFloatFieldBuilder(object[] attributes, double min, double max, Converter<object, double> fromObject, Converter<double, object> toObject)
	{
		if (!EditorGenerator.sBypassValueLimits)
		{
			min = math.max(min, -10000000.0);
			max = math.min(max, 10000000.0);
		}
		double step = WidgetAttributeUtils.GetNumberStep(attributes, 0.01);
		if (!EditorGenerator.sBypassValueLimits && WidgetAttributeUtils.GetNumberRange(attributes, ref min, ref max) && !WidgetAttributeUtils.RequiresInputField(attributes))
		{
			string unit = WidgetAttributeUtils.GetNumberUnit(attributes);
			return (IValueAccessor accessor) => new FloatSliderField
			{
				min = min,
				max = max,
				step = step,
				unit = unit,
				accessor = new CastAccessor<double>(accessor, fromObject, toObject)
			};
		}
		return (IValueAccessor accessor) => new FloatInputField
		{
			min = min,
			max = max,
			step = step,
			accessor = new CastAccessor<double>(accessor, fromObject, toObject)
		};
	}

	private static FieldBuilder CreateFloatFieldBuilder<TWidget, TValue>(object[] attributes, Converter<object, TValue> fromObject = null, Converter<TValue, object> toObject = null) where TWidget : FloatField<TValue>, new()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		float4 min = new float4(-10000000.0);
		float4 max = new float4(10000000.0);
		double step = WidgetAttributeUtils.GetNumberStep(attributes, 0.01);
		WidgetAttributeUtils.GetNumberRange(attributes, ref min, ref max);
		return delegate(IValueAccessor accessor)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			TWidget val = new TWidget
			{
				step = step,
				accessor = ((fromObject != null && toObject != null) ? new CastAccessor<TValue>(accessor, fromObject, toObject) : new CastAccessor<TValue>(accessor))
			};
			if (!EditorGenerator.sBypassValueLimits)
			{
				val.min = val.ToFieldType(double4.op_Implicit(min));
				val.max = val.ToFieldType(double4.op_Implicit(max));
			}
			return val;
		};
	}
}
