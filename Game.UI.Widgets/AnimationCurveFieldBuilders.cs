using System;
using Game.Reflection;
using UnityEngine;

namespace Game.UI.Widgets;

public class AnimationCurveFieldBuilders : IFieldBuilderFactory
{
	public FieldBuilder TryCreate(Type memberType, object[] attributes)
	{
		if (memberType == typeof(AnimationCurve))
		{
			return delegate(IValueAccessor accessor)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Expected O, but got Unknown
				if (accessor.GetValue() == null)
				{
					accessor.SetValue((object)new AnimationCurve());
				}
				return new AnimationCurveField
				{
					accessor = new CastAccessor<AnimationCurve>(accessor)
				};
			};
		}
		return null;
	}
}
