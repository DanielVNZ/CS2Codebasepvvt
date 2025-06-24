using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Colossal.Mathematics;
using Game.UI.InGame;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Rendering.CinematicCamera;

public static class PhotoModeUtils
{
	private static T ExtractMember<T>(Expression<Func<T>> expression, out string name)
	{
		MemberExpression memberExpression = (MemberExpression)expression.Body;
		if (!(memberExpression.Expression is MemberExpression memberExpression2))
		{
			ConstantExpression obj = (ConstantExpression)memberExpression.Expression;
			name = memberExpression.Expression.Type.Name + "." + memberExpression.Member.Name;
			object value = obj.Value;
			return (T)((FieldInfo)memberExpression.Member).GetValue(value);
		}
		if (!(memberExpression2.Expression is MemberExpression memberExpression3))
		{
			ConstantExpression obj2 = (ConstantExpression)memberExpression2.Expression;
			name = memberExpression2.Type.Name + "." + memberExpression.Member.Name;
			object value2 = obj2.Value;
			object value3 = ((FieldInfo)memberExpression2.Member).GetValue(value2);
			return (T)((FieldInfo)memberExpression.Member).GetValue(value3);
		}
		ConstantExpression constantExpression = (ConstantExpression)memberExpression3.Expression;
		name = memberExpression3.Type.Name + "." + memberExpression2.Member.Name + "." + memberExpression.Member.Name;
		object value4 = constantExpression.Value;
		object value5 = ((FieldInfo)memberExpression3.Member).GetValue(value4);
		object value6 = ((FieldInfo)memberExpression2.Member).GetValue(value5);
		return (T)((FieldInfo)memberExpression.Member).GetValue(value6);
	}

	private static T ExtractMember<T>(Expression<Func<T>> expression, out string name, out GameSystemBase systemBase)
	{
		MemberExpression memberExpression = (MemberExpression)expression.Body;
		MemberExpression memberExpression2 = (MemberExpression)memberExpression.Expression;
		ConstantExpression obj = (ConstantExpression)memberExpression2.Expression;
		name = memberExpression2.Type.Name + "." + memberExpression.Member.Name;
		object value = obj.Value;
		systemBase = (GameSystemBase)((FieldInfo)memberExpression2.Member).GetValue(value);
		return (T)((PropertyInfo)memberExpression.Member).GetValue(systemBase);
	}

	public static PhotoModeProperty BindPropertyW(string tab, Expression<Func<Vector4Parameter>> expression, float min, float max, Func<bool> isAvailable = null)
	{
		return BindProperty(tab, expression, (Vector4 v) => v.w, (Vector4 i, float o) => new Vector4(i.x, i.y, i.z, o), min, max, isAvailable);
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<FloatParameter>> expression, Func<bool> isAvailable = null)
	{
		string name;
		FloatParameter parameter = ExtractMember(expression, out name);
		float def = ((VolumeParameter<float>)(object)parameter).value;
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = ((VolumeParameter<float>)(object)parameter).Override,
			getValue = () => ((VolumeParameter<float>)(object)parameter).value,
			isEnabled = () => ((VolumeParameter)parameter).overrideState,
			setEnabled = delegate(bool enabled)
			{
				((VolumeParameter)parameter).overrideState = enabled;
			},
			reset = delegate
			{
				((VolumeParameter<float>)(object)parameter).value = def;
			},
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<FloatParameter>> expression, float min, float max, Func<bool> isAvailable = null)
	{
		string name;
		FloatParameter parameter = ExtractMember(expression, out name);
		float def = ((VolumeParameter<float>)(object)parameter).value;
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = ((VolumeParameter<float>)(object)parameter).Override,
			getValue = () => ((VolumeParameter<float>)(object)parameter).value,
			min = () => min,
			max = () => max,
			isEnabled = () => ((VolumeParameter)parameter).overrideState,
			setEnabled = delegate(bool enabled)
			{
				((VolumeParameter)parameter).overrideState = enabled;
			},
			reset = delegate
			{
				((VolumeParameter<float>)(object)parameter).value = def;
			},
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<MinFloatParameter>> expression, Func<bool> isAvailable = null)
	{
		string name;
		MinFloatParameter parameter = ExtractMember(expression, out name);
		float def = ((VolumeParameter<float>)(object)parameter).value;
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = ((VolumeParameter<float>)(object)parameter).Override,
			getValue = () => ((VolumeParameter<float>)(object)parameter).value,
			min = () => parameter.min,
			isEnabled = () => (isAvailable == null || isAvailable()) && ((VolumeParameter)parameter).overrideState,
			setEnabled = delegate(bool enabled)
			{
				((VolumeParameter)parameter).overrideState = enabled;
			},
			reset = delegate
			{
				((VolumeParameter<float>)(object)parameter).value = def;
			},
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<Vector4Parameter>> expression, Func<Vector4, float> getter, Func<Vector4, float, Vector4> setter, float min, float max, Func<bool> isAvailable = null)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		string name;
		Vector4Parameter parameter = ExtractMember(expression, out name);
		Vector4 def = ((VolumeParameter<Vector4>)(object)parameter).value;
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = delegate(float v)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				((VolumeParameter<Vector4>)(object)parameter).Override(setter(((VolumeParameter<Vector4>)(object)parameter).value, v));
			},
			getValue = () => getter(((VolumeParameter<Vector4>)(object)parameter).value),
			min = () => min,
			max = () => max,
			isEnabled = () => ((VolumeParameter)parameter).overrideState,
			setEnabled = delegate(bool enabled)
			{
				((VolumeParameter)parameter).overrideState = enabled;
			},
			reset = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				((VolumeParameter<Vector4>)(object)parameter).value = def;
			},
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<OverridableProperty<float>>> expression, Func<bool> isAvailable = null)
	{
		string name;
		GameSystemBase system;
		OverridableProperty<float> parameter = ExtractMember(expression, out name, out system);
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = delegate(float x)
			{
				parameter.overrideValue = x;
				((ComponentSystemBase)system).Update();
			},
			getValue = () => parameter.overrideValue,
			min = () => 0f,
			max = () => 1f,
			isEnabled = () => parameter.overrideState,
			setEnabled = delegate(bool enabled)
			{
				parameter.overrideState = enabled;
				((ComponentSystemBase)system).Update();
			},
			reset = delegate
			{
				parameter.overrideValue = parameter.value;
			},
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<ClampedFloatParameter>> expression, Func<bool> isAvailable = null)
	{
		string name;
		ClampedFloatParameter parameter = ExtractMember(expression, out name);
		float def = ((VolumeParameter<float>)(object)parameter).value;
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = ((VolumeParameter<float>)(object)parameter).Override,
			getValue = () => ((VolumeParameter<float>)(object)parameter).value,
			min = () => parameter.min,
			max = () => parameter.max,
			isEnabled = () => ((VolumeParameter)parameter).overrideState,
			setEnabled = delegate(bool enabled)
			{
				((VolumeParameter)parameter).overrideState = enabled;
			},
			reset = delegate
			{
				((VolumeParameter<float>)(object)parameter).value = def;
			},
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<ClampedIntParameter>> expression, Func<bool> isAvailable = null)
	{
		string name;
		ClampedIntParameter parameter = ExtractMember(expression, out name);
		int def = ((VolumeParameter<int>)(object)parameter).value;
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = delegate(float x)
			{
				((VolumeParameter<int>)(object)parameter).Override((int)x);
			},
			getValue = () => ((VolumeParameter<int>)(object)parameter).value,
			min = () => parameter.min,
			max = () => parameter.max,
			isEnabled = () => ((VolumeParameter)parameter).overrideState,
			setEnabled = delegate(bool enabled)
			{
				((VolumeParameter)parameter).overrideState = enabled;
			},
			reset = delegate
			{
				((VolumeParameter<int>)(object)parameter).value = def;
			},
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<OverridableLensProperty<float>>> expression, float min, float max, Func<float, float> from = null, Func<float, float> to = null, Func<bool> isAvailable = null)
	{
		string name;
		OverridableLensProperty<float> parameter = ExtractMember(expression, out name);
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = ((to != null) ? ((Action<float>)delegate(float x)
			{
				parameter.Override(to(x));
			}) : new Action<float>(parameter.Override)),
			getValue = ((from != null) ? ((Func<float>)(() => from(parameter.value))) : ((Func<float>)(() => parameter.value))),
			min = () => min,
			max = () => max,
			isEnabled = () => parameter.overrideState,
			setEnabled = delegate(bool enabled)
			{
				parameter.overrideState = enabled;
			},
			reset = parameter.Sync,
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty GroupTitle(string tab, string name)
	{
		return new PhotoModeProperty
		{
			id = name,
			group = tab
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<OverridableLensProperty<float>>> expression, Func<float> min, Func<float> max, Func<float, float> from = null, Func<float, float> to = null, Func<bool> isAvailable = null)
	{
		string name;
		OverridableLensProperty<float> parameter = ExtractMember(expression, out name);
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = ((to != null) ? ((Action<float>)delegate(float x)
			{
				parameter.Override(to(x));
			}) : new Action<float>(parameter.Override)),
			getValue = ((from != null) ? ((Func<float>)(() => from(parameter.value))) : ((Func<float>)(() => parameter.value))),
			min = min,
			max = max,
			isEnabled = () => parameter.overrideState,
			setEnabled = delegate(bool enabled)
			{
				parameter.overrideState = enabled;
			},
			reset = parameter.Sync,
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<OverridableLensProperty<int>>> expression, int min, int max, Func<bool> isAvailable = null)
	{
		string name;
		OverridableLensProperty<int> parameter = ExtractMember(expression, out name);
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = delegate(float x)
			{
				parameter.Override((int)x);
			},
			getValue = () => parameter.value,
			min = () => min,
			max = () => max,
			isEnabled = () => parameter.overrideState,
			setEnabled = delegate(bool enabled)
			{
				parameter.overrideState = enabled;
			},
			fractionDigits = 0,
			reset = parameter.Sync,
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<OverridableLensProperty<int>>> expression, int min, Func<bool> isAvailable = null)
	{
		string name;
		OverridableLensProperty<int> parameter = ExtractMember(expression, out name);
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = delegate(float x)
			{
				parameter.Override((int)x);
			},
			getValue = () => parameter.value,
			min = () => min,
			isEnabled = () => parameter.overrideState,
			setEnabled = delegate(bool enabled)
			{
				parameter.overrideState = enabled;
			},
			fractionDigits = 0,
			reset = parameter.Sync,
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty<T>(string tab, Expression<Func<VolumeParameter<T>>> expression, Func<bool> isAvailable = null) where T : Enum
	{
		string name;
		VolumeParameter<T> parameter = ExtractMember(expression, out name);
		T def = parameter.value;
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = delegate(float value)
			{
				parameter.Override(FindClosestEnumValue<T>(value));
			},
			getValue = () => Convert.ToInt32(parameter.value),
			isEnabled = () => ((VolumeParameter)parameter).overrideState,
			setEnabled = delegate(bool enabled)
			{
				((VolumeParameter)parameter).overrideState = enabled;
			},
			reset = delegate
			{
				parameter.value = def;
			},
			enumType = typeof(T),
			isAvailable = isAvailable
		};
	}

	public static PhotoModeProperty BindProperty<T>(string tab, Expression<Func<OverridableLensProperty<T>>> expression, Func<bool> isAvailable = null) where T : Enum
	{
		string name;
		OverridableLensProperty<T> parameter = ExtractMember(expression, out name);
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = delegate(float value)
			{
				parameter.Override(FindClosestEnumValue<T>(value));
			},
			getValue = () => Convert.ToInt32(parameter.value),
			isEnabled = () => parameter.overrideState,
			setEnabled = delegate(bool enabled)
			{
				parameter.overrideState = enabled;
			},
			reset = delegate
			{
				parameter.Sync();
			},
			enumType = typeof(T),
			isAvailable = isAvailable
		};
	}

	public static T FindClosestEnumValue<T>(float value) where T : Enum
	{
		T result = default(T);
		float num = 2.1474836E+09f;
		foreach (T value2 in Enum.GetValues(typeof(T)))
		{
			float num2 = Convert.ToInt32(value2);
			float num3 = Mathf.Abs(value - num2);
			if (num3 < num)
			{
				result = value2;
				num = num3;
			}
		}
		return result;
	}

	public static PhotoModeProperty BindProperty(string tab, Expression<Func<BoolParameter>> expression, Func<bool> isAvailable = null)
	{
		string name;
		BoolParameter parameter = ExtractMember(expression, out name);
		bool def = ((VolumeParameter<bool>)(object)parameter).value;
		return new PhotoModeProperty
		{
			id = name,
			group = tab,
			setValue = delegate(float value)
			{
				((VolumeParameter<bool>)(object)parameter).Override(FloatToBoolean(value));
			},
			getValue = () => BooleanToFloat(((VolumeParameter<bool>)(object)parameter).value),
			isEnabled = () => ((VolumeParameter)parameter).overrideState,
			setEnabled = delegate(bool enabled)
			{
				((VolumeParameter)parameter).overrideState = enabled;
			},
			reset = delegate
			{
				((VolumeParameter<bool>)(object)parameter).value = def;
			},
			overrideControl = PhotoModeProperty.OverrideControl.Checkbox,
			isAvailable = isAvailable
		};
	}

	public static bool FloatToBoolean(float value)
	{
		return Mathf.RoundToInt(value) != 0;
	}

	public static float BooleanToFloat(bool value)
	{
		if (!value)
		{
			return 0f;
		}
		return 1f;
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<ColorParameter>> expression)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		string name;
		ColorParameter parameter = ExtractMember(expression, out name);
		Color def = ((VolumeParameter<Color>)(object)parameter).value;
		List<PhotoModeProperty> list = new List<PhotoModeProperty>
		{
			new PhotoModeProperty
			{
				id = name + "/r",
				group = tab,
				setValue = delegate(float r)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Color value = ((VolumeParameter<Color>)(object)parameter).value;
					value.r = r;
					((VolumeParameter<Color>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Color>)(object)parameter).value.r,
				min = () => 0f,
				max = (parameter.hdr ? ((Func<float>)(() => float.PositiveInfinity)) : ((Func<float>)(() => 1f))),
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Color>)(object)parameter).value = def;
				},
				overrideControl = PhotoModeProperty.OverrideControl.ColorField
			},
			new PhotoModeProperty
			{
				id = name + "/g",
				group = tab,
				setValue = delegate(float g)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Color value = ((VolumeParameter<Color>)(object)parameter).value;
					value.g = g;
					((VolumeParameter<Color>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Color>)(object)parameter).value.g,
				min = () => 0f,
				max = (parameter.hdr ? ((Func<float>)(() => float.PositiveInfinity)) : ((Func<float>)(() => 1f))),
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Color>)(object)parameter).value = def;
				},
				overrideControl = PhotoModeProperty.OverrideControl.ColorField
			},
			new PhotoModeProperty
			{
				id = name + "/b",
				group = tab,
				setValue = delegate(float b)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Color value = ((VolumeParameter<Color>)(object)parameter).value;
					value.b = b;
					((VolumeParameter<Color>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Color>)(object)parameter).value.b,
				min = () => 0f,
				max = (parameter.hdr ? ((Func<float>)(() => float.PositiveInfinity)) : ((Func<float>)(() => 1f))),
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Color>)(object)parameter).value = def;
				},
				overrideControl = PhotoModeProperty.OverrideControl.ColorField
			}
		};
		if (parameter.showAlpha)
		{
			list.Add(new PhotoModeProperty
			{
				id = name + "/a",
				group = tab,
				setValue = delegate(float a)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Color value = ((VolumeParameter<Color>)(object)parameter).value;
					value.a = a;
					((VolumeParameter<Color>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Color>)(object)parameter).value.a,
				min = () => 0f,
				max = (parameter.hdr ? ((Func<float>)(() => float.PositiveInfinity)) : ((Func<float>)(() => 1f))),
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Color>)(object)parameter).value = def;
				},
				overrideControl = PhotoModeProperty.OverrideControl.ColorField
			});
		}
		return list.ToArray();
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<Vector2Parameter>> expression, float min, float max)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return BindProperty(tab, expression, new Bounds1(min, max));
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<Vector2Parameter>> expression, Bounds1 bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return BindProperty(tab, expression, bounds, bounds);
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<Vector2Parameter>> expression, Bounds1 xBounds, Bounds1 yBounds)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		string name;
		Vector2Parameter parameter = ExtractMember(expression, out name);
		Vector2 def = ((VolumeParameter<Vector2>)(object)parameter).value;
		return new PhotoModeProperty[2]
		{
			new PhotoModeProperty
			{
				id = name + "/x",
				group = tab,
				setValue = delegate(float x)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = ((VolumeParameter<Vector2>)(object)parameter).value;
					value.x = x;
					((VolumeParameter<Vector2>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector2>)(object)parameter).value.x,
				min = () => xBounds.min,
				max = () => xBounds.max,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector2>)(object)parameter).value = def;
				}
			},
			new PhotoModeProperty
			{
				id = name + "/y",
				group = tab,
				setValue = delegate(float y)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = ((VolumeParameter<Vector2>)(object)parameter).value;
					value.y = y;
					((VolumeParameter<Vector2>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector2>)(object)parameter).value.y,
				min = () => yBounds.min,
				max = () => yBounds.max,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector2>)(object)parameter).value = def;
				}
			}
		};
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<Vector2Parameter>> expression)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		string name;
		Vector2Parameter parameter = ExtractMember(expression, out name);
		Vector2 def = ((VolumeParameter<Vector2>)(object)parameter).value;
		return new PhotoModeProperty[2]
		{
			new PhotoModeProperty
			{
				id = name + "/x",
				group = tab,
				setValue = delegate(float x)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = ((VolumeParameter<Vector2>)(object)parameter).value;
					value.x = x;
					((VolumeParameter<Vector2>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector2>)(object)parameter).value.x,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector2>)(object)parameter).value = def;
				}
			},
			new PhotoModeProperty
			{
				id = name + "/y",
				group = tab,
				setValue = delegate(float y)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = ((VolumeParameter<Vector2>)(object)parameter).value;
					value.y = y;
					((VolumeParameter<Vector2>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector2>)(object)parameter).value.y,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector2>)(object)parameter).value = def;
				}
			}
		};
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<Vector3Parameter>> expression, float min, float max)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return BindProperty(tab, expression, new Bounds1(min, max));
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<Vector3Parameter>> expression, Bounds1 bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return BindProperty(tab, expression, bounds, bounds, bounds);
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<Vector3Parameter>> expression, Bounds1 xBounds, Bounds1 yBounds, Bounds1 zBounds)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		string name;
		Vector3Parameter parameter = ExtractMember(expression, out name);
		Vector3 def = ((VolumeParameter<Vector3>)(object)parameter).value;
		return new PhotoModeProperty[3]
		{
			new PhotoModeProperty
			{
				id = name + "/x",
				group = tab,
				setValue = delegate(float x)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector3 value = ((VolumeParameter<Vector3>)(object)parameter).value;
					value.x = x;
					((VolumeParameter<Vector3>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector3>)(object)parameter).value.x,
				min = () => xBounds.min,
				max = () => xBounds.max,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector3>)(object)parameter).value = def;
				}
			},
			new PhotoModeProperty
			{
				id = name + "/y",
				group = tab,
				setValue = delegate(float y)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector3 value = ((VolumeParameter<Vector3>)(object)parameter).value;
					value.y = y;
					((VolumeParameter<Vector3>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector3>)(object)parameter).value.y,
				min = () => yBounds.min,
				max = () => yBounds.max,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector3>)(object)parameter).value = def;
				}
			},
			new PhotoModeProperty
			{
				id = name + "/z",
				group = tab,
				setValue = delegate(float z)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector3 value = ((VolumeParameter<Vector3>)(object)parameter).value;
					value.z = z;
					((VolumeParameter<Vector3>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector3>)(object)parameter).value.z,
				min = () => zBounds.min,
				max = () => zBounds.max,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector3>)(object)parameter).value = def;
				}
			}
		};
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<Vector3Parameter>> expression)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		string name;
		Vector3Parameter parameter = ExtractMember(expression, out name);
		Vector3 def = ((VolumeParameter<Vector3>)(object)parameter).value;
		return new PhotoModeProperty[3]
		{
			new PhotoModeProperty
			{
				id = name + "/x",
				group = tab,
				setValue = delegate(float x)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector3 value = ((VolumeParameter<Vector3>)(object)parameter).value;
					value.x = x;
					((VolumeParameter<Vector3>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector3>)(object)parameter).value.x,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector3>)(object)parameter).value = def;
				}
			},
			new PhotoModeProperty
			{
				id = name + "/y",
				group = tab,
				setValue = delegate(float y)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector3 value = ((VolumeParameter<Vector3>)(object)parameter).value;
					value.y = y;
					((VolumeParameter<Vector3>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector3>)(object)parameter).value.y,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector3>)(object)parameter).value = def;
				}
			},
			new PhotoModeProperty
			{
				id = name + "/z",
				group = tab,
				setValue = delegate(float z)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector3 value = ((VolumeParameter<Vector3>)(object)parameter).value;
					value.z = z;
					((VolumeParameter<Vector3>)(object)parameter).Override(value);
				},
				getValue = () => ((VolumeParameter<Vector3>)(object)parameter).value.z,
				isEnabled = () => ((VolumeParameter)parameter).overrideState,
				setEnabled = delegate(bool enabled)
				{
					((VolumeParameter)parameter).overrideState = enabled;
				},
				reset = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Vector3>)(object)parameter).value = def;
				}
			}
		};
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<OverridableLensProperty<Vector2>>> expression)
	{
		string name;
		OverridableLensProperty<Vector2> parameter = ExtractMember(expression, out name);
		return new PhotoModeProperty[2]
		{
			new PhotoModeProperty
			{
				id = name + "/x",
				group = tab,
				setValue = delegate(float x)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = parameter.value;
					value.x = x;
					parameter.Override(value);
				},
				getValue = () => parameter.value.x,
				isEnabled = () => parameter.overrideState,
				setEnabled = delegate(bool enabled)
				{
					parameter.overrideState = enabled;
				},
				reset = delegate
				{
					parameter.Sync();
				}
			},
			new PhotoModeProperty
			{
				id = name + "/y",
				group = tab,
				setValue = delegate(float y)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = parameter.value;
					value.y = y;
					parameter.Override(value);
				},
				getValue = () => parameter.value.y,
				isEnabled = () => parameter.overrideState,
				setEnabled = delegate(bool enabled)
				{
					parameter.overrideState = enabled;
				},
				reset = delegate
				{
					parameter.Sync();
				}
			}
		};
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<OverridableLensProperty<Vector2>>> expression, float min = 0f, float max = 0f)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return BindProperty(tab, expression, new Bounds1(min, max));
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<OverridableLensProperty<Vector2>>> expression, Bounds1 bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return BindProperty(tab, expression, bounds, bounds);
	}

	public static PhotoModeProperty[] BindProperty(string tab, Expression<Func<OverridableLensProperty<Vector2>>> expression, Bounds1 xBounds, Bounds1 yBounds)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		string name;
		OverridableLensProperty<Vector2> parameter = ExtractMember(expression, out name);
		return new PhotoModeProperty[2]
		{
			new PhotoModeProperty
			{
				id = name + "/x",
				group = tab,
				setValue = delegate(float x)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = parameter.value;
					value.x = x;
					parameter.Override(value);
				},
				getValue = () => parameter.value.x,
				min = () => xBounds.min,
				max = () => xBounds.max,
				isEnabled = () => parameter.overrideState,
				setEnabled = delegate(bool enabled)
				{
					parameter.overrideState = enabled;
				},
				reset = delegate
				{
					parameter.Sync();
				}
			},
			new PhotoModeProperty
			{
				id = name + "/y",
				group = tab,
				setValue = delegate(float y)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					Vector2 value = parameter.value;
					value.y = y;
					parameter.Override(value);
				},
				getValue = () => parameter.value.y,
				min = () => yBounds.min,
				max = () => yBounds.max,
				isEnabled = () => parameter.overrideState,
				setEnabled = delegate(bool enabled)
				{
					parameter.overrideState = enabled;
				},
				reset = delegate
				{
					parameter.Sync();
				}
			}
		};
	}

	public static IEnumerable<PhotoModeProperty> ExtractMultiPropertyComponents(PhotoModeProperty property, IDictionary<string, PhotoModeProperty> allProperties)
	{
		int num = property.id.IndexOf("/");
		if (num < 0)
		{
			yield return property;
			yield break;
		}
		string name = property.id.Substring(0, num + 1);
		foreach (KeyValuePair<string, PhotoModeProperty> allProperty in allProperties)
		{
			if (allProperty.Key.StartsWith(name))
			{
				yield return allProperty.Value;
			}
		}
	}

	public static string ExtractPropertyID(PhotoModeProperty property)
	{
		int num = property.id.IndexOf("/");
		if (num < 0)
		{
			return property.id;
		}
		return property.id.Substring(0, num);
	}

	public static PhotoModeUIPreset CreatePreset(string name, PhotoModeProperty injectionProperty, PhotoModeProperty[] targetProperties, string[] options, Vector2[] values)
	{
		if (targetProperties.Length != 2)
		{
			throw new ArgumentException("targetProperties must be of length 2 with Vector2 values");
		}
		PresetDescriptor presetDescriptor = new PresetDescriptor();
		presetDescriptor.AddOptions(options);
		foreach (PhotoModeProperty photoModeProperty in targetProperties)
		{
			if (photoModeProperty.id.EndsWith("x"))
			{
				presetDescriptor.AddValues(photoModeProperty, values.Select((Vector2 v) => v.x).ToArray());
			}
			else if (photoModeProperty.id.EndsWith("y"))
			{
				presetDescriptor.AddValues(photoModeProperty, values.Select((Vector2 v) => v.y).ToArray());
			}
		}
		if (!presetDescriptor.Validate())
		{
			throw new ArgumentException("Preset descriptor is invalid");
		}
		return new PhotoModeUIPreset
		{
			id = name,
			injectionProperty = injectionProperty,
			descriptor = presetDescriptor
		};
	}
}
