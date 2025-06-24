using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.UI.Binding;
using UnityEngine;

namespace Game.UI.Widgets;

public class AnimationCurveField : Field<AnimationCurve>, IMutable<AnimationCurve>, IWidget, IJsonWritable
{
	public class Bindings : IWidgetBindingFactory
	{
		public IEnumerable<IBinding> CreateBindings(string group, IReader<IWidget> pathResolver, ValueChangedCallback onValueChanged)
		{
			yield return (IBinding)(object)new CallBinding<IWidget, int, Keyframe, bool, int, int>(group, "moveKeyframe", (Func<IWidget, int, Keyframe, bool, int, int>)delegate(IWidget widget, int index, Keyframe key, bool smooth, int curveIndex)
			{
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				bool flag = true;
				int result = index;
				if (widget is IMutable<AnimationCurve> mutable)
				{
					if (index > 0)
					{
						Keyframe val = mutable.GetValue()[index - 1];
						if (((Keyframe)(ref val)).time == ((Keyframe)(ref key)).time)
						{
							flag = false;
						}
					}
					if (index < mutable.GetValue().length - 1)
					{
						Keyframe val2 = mutable.GetValue()[index + 1];
						if (((Keyframe)(ref val2)).time == ((Keyframe)(ref key)).time)
						{
							flag = false;
						}
					}
					if (flag)
					{
						result = mutable.GetValue().MoveKey(index, key);
					}
					if (smooth)
					{
						mutable.GetValue().SmoothTangents(index, (((Keyframe)(ref key)).inWeight + ((Keyframe)(ref key)).outWeight) / 2f);
					}
					onValueChanged(widget);
					return result;
				}
				Debug.LogError((object)((widget != null) ? "Widget does not implement IMutable<AnimationCurve>" : "Invalid widget path"));
				return result;
			}, pathResolver, (IReader<int>)null, (IReader<Keyframe>)null, (IReader<bool>)null, (IReader<int>)null);
			yield return (IBinding)(object)new CallBinding<IWidget, float, float, int, int>(group, "addKeyframe", (Func<IWidget, float, float, int, int>)delegate(IWidget widget, float time, float value, int curveIndex)
			{
				int result = -1;
				if (widget is IMutable<AnimationCurve> mutable)
				{
					result = mutable.GetValue().AddKey(time, value);
					onValueChanged(widget);
					return result;
				}
				Debug.LogError((object)((widget != null) ? "Widget does not implement IMutable<AnimationCurve>" : "Invalid widget path"));
				return result;
			}, pathResolver, (IReader<float>)null, (IReader<float>)null, (IReader<int>)null);
			yield return (IBinding)(object)new TriggerBinding<IWidget, Keyframe[], int>(group, "setKeyframes", (Action<IWidget, Keyframe[], int>)delegate(IWidget widget, Keyframe[] keys, int curveIndex)
			{
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				if (widget is IMutable<AnimationCurve> mutable)
				{
					while (mutable.GetValue().length > 0)
					{
						mutable.GetValue().RemoveKey(0);
					}
					for (int i = 0; i < keys.Count(); i++)
					{
						mutable.GetValue().AddKey(((Keyframe)(ref keys[i])).time, ((Keyframe)(ref keys[i])).value);
						mutable.GetValue().MoveKey(i, keys[i]);
					}
					onValueChanged(widget);
				}
				else
				{
					Debug.LogError((object)((widget != null) ? "Widget does not implement IMutable<AnimationCurve>" : "Invalid widget path"));
				}
			}, pathResolver, (IReader<Keyframe[]>)null, (IReader<int>)null);
			yield return (IBinding)(object)new TriggerBinding<IWidget, int, int>(group, "removeKeyframe", (Action<IWidget, int, int>)delegate(IWidget widget, int index, int curveIndex)
			{
				if (widget is IMutable<AnimationCurve> mutable)
				{
					mutable.GetValue().RemoveKey(index);
					onValueChanged(widget);
				}
				else
				{
					Debug.LogError((object)((widget != null) ? "Widget does not implement IMutable<AnimationCurve>" : "Invalid widget path"));
				}
			}, pathResolver, (IReader<int>)null, (IReader<int>)null);
		}
	}

	private List<Keyframe> m_Keys = new List<Keyframe>();

	private WrapMode m_PreWrapMode;

	private WrapMode m_PostWrapMode;

	protected override WidgetChanges Update()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		WidgetChanges widgetChanges = base.Update();
		Keyframe[] keys = m_Value.keys;
		if (!m_Keys.SequenceEqual(keys))
		{
			widgetChanges |= WidgetChanges.Properties;
			m_Keys.Clear();
			m_Keys.AddRange(keys);
		}
		if (m_Value.preWrapMode != m_PreWrapMode)
		{
			widgetChanges |= WidgetChanges.Properties;
			m_PreWrapMode = m_Value.preWrapMode;
		}
		if (m_Value.postWrapMode != m_PostWrapMode)
		{
			widgetChanges |= WidgetChanges.Properties;
			m_PostWrapMode = m_Value.postWrapMode;
		}
		return widgetChanges;
	}
}
