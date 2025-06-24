using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Game.Debug;
using Game.Reflection;
using Game.UI.Localization;
using Game.UI.Widgets;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.UI.Debug;

public static class DebugWidgetBuilders
{
	public static IEnumerable<IWidget> BuildWidgets(ObservableList<Widget> debugWidgets)
	{
		foreach (Widget debugWidget in debugWidgets)
		{
			if (debugWidget.isEditorOnly)
			{
				continue;
			}
			IWidget widget = TryBuildWidget(debugWidget);
			if (widget != null)
			{
				if (widget is INamed named)
				{
					named.displayName = LocalizedString.Value(debugWidget.displayName);
				}
				yield return widget;
			}
		}
	}

	[CanBeNull]
	private static IWidget TryBuildWidget(Widget debugWidget)
	{
		Foldout val = (Foldout)(object)((debugWidget is Foldout) ? debugWidget : null);
		if (val != null)
		{
			return BuildExpandableGroup(val);
		}
		Container val2 = (Container)(object)((debugWidget is Container) ? debugWidget : null);
		if (val2 != null)
		{
			return BuildGroup(val2);
		}
		ValueTuple val3 = (ValueTuple)(object)((debugWidget is ValueTuple) ? debugWidget : null);
		if (val3 != null)
		{
			return BuildGroup(val3);
		}
		Button val4 = (Button)(object)((debugWidget is Button) ? debugWidget : null);
		if (val4 != null)
		{
			return BuildButton(val4);
		}
		Value val5 = (Value)(object)((debugWidget is Value) ? debugWidget : null);
		if (val5 != null)
		{
			return BuildValueField(val5);
		}
		BoolField val6 = (BoolField)(object)((debugWidget is BoolField) ? debugWidget : null);
		if (val6 != null)
		{
			return BuildToggleField(val6);
		}
		IntField val7 = (IntField)(object)((debugWidget is IntField) ? debugWidget : null);
		if (val7 != null)
		{
			return BuildIntField(val7);
		}
		if (debugWidget is Game.Debug.IntInputField debugWidget2)
		{
			return BuildIntInputField(debugWidget2);
		}
		UIntField val8 = (UIntField)(object)((debugWidget is UIntField) ? debugWidget : null);
		if (val8 != null)
		{
			return BuildUIntField(val8);
		}
		FloatField val9 = (FloatField)(object)((debugWidget is FloatField) ? debugWidget : null);
		if (val9 != null)
		{
			return BuildFloatField(val9);
		}
		Vector2Field val10 = (Vector2Field)(object)((debugWidget is Vector2Field) ? debugWidget : null);
		if (val10 != null)
		{
			return BuildFloat2Field(val10);
		}
		Vector3Field val11 = (Vector3Field)(object)((debugWidget is Vector3Field) ? debugWidget : null);
		if (val11 != null)
		{
			return BuildFloat3Field(val11);
		}
		Vector4Field val12 = (Vector4Field)(object)((debugWidget is Vector4Field) ? debugWidget : null);
		if (val12 != null)
		{
			return BuildFloat4Field(val12);
		}
		EnumField val13 = (EnumField)(object)((debugWidget is EnumField) ? debugWidget : null);
		if (val13 != null)
		{
			return BuildEnumField(val13);
		}
		BitField val14 = (BitField)(object)((debugWidget is BitField) ? debugWidget : null);
		if (val14 != null)
		{
			return BuildFlagsField(val14);
		}
		ColorField val15 = (ColorField)(object)((debugWidget is ColorField) ? debugWidget : null);
		if (val15 != null)
		{
			return BuildColorField(val15);
		}
		TextField val16 = (TextField)(object)((debugWidget is TextField) ? debugWidget : null);
		if (val16 != null)
		{
			return BuildStringInputField(val16);
		}
		return null;
	}

	private static ExpandableGroup BuildExpandableGroup(Foldout debugWidget)
	{
		return new ExpandableGroup(new DelegateAccessor<bool>(() => debugWidget.opened, delegate(bool value)
		{
			debugWidget.opened = value;
		}))
		{
			children = new List<IWidget>(BuildWidgets(((Container)debugWidget).children))
		};
	}

	private static Group BuildGroup(Container debugWidget)
	{
		return new Group
		{
			children = new List<IWidget>(BuildWidgets(debugWidget.children))
		};
	}

	private static Group BuildGroup(ValueTuple debugWidget)
	{
		List<IWidget> list = new List<IWidget>(debugWidget.values.Length);
		IContainer parent = ((Widget)debugWidget).parent;
		IContainer obj = ((parent is Foldout) ? parent : null);
		string[] array = ((obj != null) ? ((Foldout)obj).columnLabels : null);
		for (int i = 0; i < debugWidget.values.Length; i++)
		{
			list.Add(new ValueField(debugWidget.values[i])
			{
				path = i,
				displayName = ((array != null && array.Length > i) ? array[i] : string.Empty)
			});
		}
		return new Group
		{
			children = list
		};
	}

	private static Button BuildButton(Button debugWidget)
	{
		return new Button
		{
			action = debugWidget.action
		};
	}

	private static ValueField BuildValueField(Value debugWidget)
	{
		return new ValueField(debugWidget);
	}

	private static ToggleField BuildToggleField(BoolField debugWidget)
	{
		return new ToggleField
		{
			accessor = new DebugFieldAccessor<bool>((Field<bool>)(object)debugWidget)
		};
	}

	private static IntField<int> BuildIntField(IntField debugWidget)
	{
		int num = Invoke(debugWidget.min, int.MinValue);
		int num2 = Invoke(debugWidget.max, int.MaxValue);
		DebugFieldAccessor<int> accessor = new DebugFieldAccessor<int>((Field<int>)(object)debugWidget);
		if (num > int.MinValue && num2 < int.MaxValue)
		{
			return new IntSliderField
			{
				min = num,
				max = num2,
				step = debugWidget.incStep,
				stepMultiplier = debugWidget.intStepMult,
				accessor = accessor
			};
		}
		return new IntArrowField
		{
			step = debugWidget.incStep,
			stepMultiplier = debugWidget.intStepMult,
			accessor = accessor
		};
	}

	private static UIntField BuildUIntField(UIntField debugWidget)
	{
		uint num = Invoke(debugWidget.min, 0u);
		uint num2 = Invoke(debugWidget.max, uint.MaxValue);
		DebugFieldAccessor<uint> accessor = new DebugFieldAccessor<uint>((Field<uint>)(object)debugWidget);
		if (num != 0 && num2 < uint.MaxValue)
		{
			return new UIntSliderField
			{
				min = num,
				max = num2,
				step = debugWidget.incStep,
				stepMultiplier = debugWidget.intStepMult,
				accessor = accessor
			};
		}
		return new UIntArrowField
		{
			step = debugWidget.incStep,
			stepMultiplier = debugWidget.intStepMult,
			accessor = accessor
		};
	}

	private static FloatField<double> BuildFloatField(FloatField debugWidget)
	{
		float num = Invoke(debugWidget.min, float.MinValue);
		float num2 = Invoke(debugWidget.max, float.MaxValue);
		DebugFieldCastAccessor<double, float> accessor = new DebugFieldCastAccessor<double, float>((Field<float>)(object)debugWidget, (float value) => value, (double value) => (float)value);
		if (num > float.MinValue && num2 < float.MaxValue)
		{
			return new FloatSliderField
			{
				min = num,
				max = num2,
				step = debugWidget.incStep,
				stepMultiplier = debugWidget.incStepMult,
				accessor = accessor
			};
		}
		return new FloatArrowField
		{
			min = num,
			max = num2,
			step = debugWidget.incStep,
			stepMultiplier = debugWidget.incStepMult,
			accessor = accessor
		};
	}

	private static Float2SliderField BuildFloat2Field(Vector2Field debugWidget)
	{
		return new Float2SliderField
		{
			step = debugWidget.incStep,
			stepMultiplier = debugWidget.incStepMult,
			fractionDigits = debugWidget.decimals,
			accessor = new DebugFieldCastAccessor<float2, Vector2>((Field<Vector2>)(object)debugWidget, ToFloat, FromFloat)
		};
		static Vector2 FromFloat(float2 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float2.op_Implicit(value);
		}
		static float2 ToFloat(Vector2 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float2.op_Implicit(value);
		}
	}

	private static Float3SliderField BuildFloat3Field(Vector3Field debugWidget)
	{
		return new Float3SliderField
		{
			step = debugWidget.incStep,
			stepMultiplier = debugWidget.incStepMult,
			fractionDigits = debugWidget.decimals,
			accessor = new DebugFieldCastAccessor<float3, Vector3>((Field<Vector3>)(object)debugWidget, ToFloat, FromFloat)
		};
		static Vector3 FromFloat(float3 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float3.op_Implicit(value);
		}
		static float3 ToFloat(Vector3 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float3.op_Implicit(value);
		}
	}

	private static Float4SliderField BuildFloat4Field(Vector4Field debugWidget)
	{
		return new Float4SliderField
		{
			step = debugWidget.incStep,
			stepMultiplier = debugWidget.incStepMult,
			fractionDigits = debugWidget.decimals,
			accessor = new DebugFieldCastAccessor<float4, Vector4>((Field<Vector4>)(object)debugWidget, ToFloat, FromFloat)
		};
		static Vector4 FromFloat(float4 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float4.op_Implicit(value);
		}
		static float4 ToFloat(Vector4 value)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return float4.op_Implicit(value);
		}
	}

	private static EnumField BuildEnumField(EnumField debugWidget)
	{
		return new EnumField
		{
			enumMembers = BuildEnumMembers<int>((EnumField<int>)(object)debugWidget),
			accessor = new DelegateAccessor<ulong>(() => (ulong)((Field<int>)(object)debugWidget).GetValue(), delegate(ulong value)
			{
				((Field<int>)(object)debugWidget).SetValue((int)value);
			})
		};
	}

	private static FlagsField BuildFlagsField(BitField debugWidget)
	{
		if (!EnumFieldBuilders.GetConverters(debugWidget.enumType, out var fromObject, out var toObject))
		{
			fromObject = (object value) => (ulong)(long)value;
			toObject = (ulong value) => (long)value;
		}
		return new FlagsField
		{
			enumMembers = BuildEnumMembers<Enum>((EnumField<Enum>)(object)debugWidget),
			accessor = new DelegateAccessor<ulong>(() => fromObject(((Field<Enum>)(object)debugWidget).GetValue()), delegate(ulong value)
			{
				((Field<Enum>)(object)debugWidget).SetValue(toObject(value));
			})
		};
	}

	private static EnumMember[] BuildEnumMembers<T>(EnumField<T> debugWidget)
	{
		EnumMember[] array = new EnumMember[debugWidget.enumValues.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new EnumMember((ulong)debugWidget.enumValues[i], debugWidget.enumNames[i].text);
		}
		return array;
	}

	private static ColorField BuildColorField(ColorField debugWidget)
	{
		return new ColorField
		{
			hdr = debugWidget.hdr,
			showAlpha = debugWidget.showAlpha,
			accessor = new DebugFieldAccessor<Color>((Field<Color>)(object)debugWidget)
		};
	}

	private static StringInputField BuildStringInputField(TextField debugWidget)
	{
		return new StringInputField
		{
			accessor = new DebugFieldAccessor<string>((Field<string>)(object)debugWidget)
		};
	}

	private static IntInputField BuildIntInputField(Game.Debug.IntInputField debugWidget)
	{
		return new IntInputField(debugWidget);
	}

	private static T Invoke<T>(Func<T> func, T fallback)
	{
		if (func == null)
		{
			return fallback;
		}
		return func();
	}
}
