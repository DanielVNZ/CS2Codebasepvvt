using System;
using System.Linq;
using Colossal.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace Game.UI.Widgets;

public static class WidgetAttributeUtils
{
	public static bool RequiresInputField(object[] attributes)
	{
		return attributes.OfType<InputFieldAttribute>().Any();
	}

	public static bool IsTimeField(object[] attributes)
	{
		return attributes.OfType<TimeFieldAttribute>().Any();
	}

	public static bool AllowsMinGreaterMax(object[] attributes)
	{
		return attributes.OfType<AllowMinGreaterMaxAttribute>().Any();
	}

	public static void GetColorUsage(object[] attributes, ref bool hdr, ref bool showAlpha)
	{
		ColorUsageAttribute val = attributes.OfType<ColorUsageAttribute>().FirstOrDefault();
		if (val != null)
		{
			hdr = val.hdr;
			showAlpha = val.showAlpha;
		}
	}

	public static bool GetNumberRange(object[] attributes, ref int min, ref int max)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		RangeAttribute val = attributes.OfType<RangeAttribute>().FirstOrDefault();
		if (val != null)
		{
			min = (int)val.min;
			max = (int)val.max;
			return true;
		}
		RangeNAttribute rangeNAttribute = attributes.OfType<RangeNAttribute>().FirstOrDefault();
		if (rangeNAttribute != null)
		{
			min = (int)rangeNAttribute.min.x;
			max = (int)rangeNAttribute.max.x;
			return true;
		}
		return false;
	}

	public static bool GetNumberRange(object[] attributes, ref uint min, ref uint max)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		RangeAttribute val = attributes.OfType<RangeAttribute>().FirstOrDefault();
		if (val != null)
		{
			min = (uint)val.min;
			max = (uint)val.max;
			return true;
		}
		RangeNAttribute rangeNAttribute = attributes.OfType<RangeNAttribute>().FirstOrDefault();
		if (rangeNAttribute != null)
		{
			min = (uint)rangeNAttribute.min.x;
			max = (uint)rangeNAttribute.max.x;
			return true;
		}
		return false;
	}

	public static bool GetNumberRange(object[] attributes, ref float min, ref float max)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		RangeAttribute val = attributes.OfType<RangeAttribute>().FirstOrDefault();
		if (val != null)
		{
			min = val.min;
			max = val.max;
			return true;
		}
		RangeNAttribute rangeNAttribute = attributes.OfType<RangeNAttribute>().FirstOrDefault();
		if (rangeNAttribute != null)
		{
			min = rangeNAttribute.min.x;
			max = rangeNAttribute.max.x;
			return true;
		}
		return false;
	}

	public static bool GetNumberRange(object[] attributes, ref float4 min, ref float4 max)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		RangeAttribute val = attributes.OfType<RangeAttribute>().FirstOrDefault();
		if (val != null)
		{
			min = float4.op_Implicit(val.min);
			max = float4.op_Implicit(val.max);
			return true;
		}
		RangeNAttribute rangeNAttribute = attributes.OfType<RangeNAttribute>().FirstOrDefault();
		if (rangeNAttribute != null)
		{
			min = rangeNAttribute.min;
			max = rangeNAttribute.max;
			return true;
		}
		return false;
	}

	public static bool GetNumberRange(object[] attributes, ref double min, ref double max)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		RangeAttribute val = attributes.OfType<RangeAttribute>().FirstOrDefault();
		if (val != null)
		{
			min = val.min;
			max = val.max;
			return true;
		}
		RangeNAttribute rangeNAttribute = attributes.OfType<RangeNAttribute>().FirstOrDefault();
		if (rangeNAttribute != null)
		{
			min = rangeNAttribute.min.x;
			max = rangeNAttribute.max.x;
			return true;
		}
		return false;
	}

	public static int GetNumberStep(object[] attributes, int defaultStep = 1)
	{
		NumberStepAttribute numberStepAttribute = attributes.OfType<NumberStepAttribute>().FirstOrDefault();
		if (numberStepAttribute != null)
		{
			int num = (int)numberStepAttribute.Step;
			if (num > 0)
			{
				return num;
			}
		}
		return defaultStep;
	}

	public static uint GetNumberStep(object[] attributes, uint defaultStep = 1u)
	{
		NumberStepAttribute numberStepAttribute = attributes.OfType<NumberStepAttribute>().FirstOrDefault();
		if (numberStepAttribute != null)
		{
			uint num = (uint)numberStepAttribute.Step;
			if (num != 0)
			{
				return num;
			}
		}
		return defaultStep;
	}

	public static float GetNumberStep(object[] attributes, float defaultStep = 0.01f)
	{
		NumberStepAttribute numberStepAttribute = attributes.OfType<NumberStepAttribute>().FirstOrDefault();
		if (numberStepAttribute == null || !(numberStepAttribute.Step > 0f))
		{
			return defaultStep;
		}
		return numberStepAttribute.Step;
	}

	public static double GetNumberStep(object[] attributes, double defaultStep = 0.01)
	{
		NumberStepAttribute numberStepAttribute = attributes.OfType<NumberStepAttribute>().FirstOrDefault();
		if (numberStepAttribute == null || !(numberStepAttribute.Step > 0f))
		{
			return defaultStep;
		}
		return numberStepAttribute.Step;
	}

	[CanBeNull]
	public static string GetNumberUnit(object[] attributes)
	{
		return attributes.OfType<NumberUnitAttribute>().FirstOrDefault()?.Unit;
	}

	public static Type GetCustomFieldFactory(object[] attributes)
	{
		return attributes.OfType<CustomFieldAttribute>().FirstOrDefault()?.Factory;
	}
}
