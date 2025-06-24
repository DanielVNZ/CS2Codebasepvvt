using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace Game.Input;

[DisplayStringFormat("{binding}")]
[DisplayName("CO Vector 2D With Modifiers")]
public class Vector2WithModifiersComposite : AnalogValueInputBindingComposite<Vector2>
{
	[InputControl(layout = "Vector2")]
	public int binding;

	[InputControl(layout = "Button")]
	public int modifier;

	public override Vector2 ReadValue(ref InputBindingCompositeContext context)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsDummy)
		{
			return default(Vector2);
		}
		if (m_Mode == Mode.Analog)
		{
			return CompositeUtility.ReadValue<Vector2>(ref context, binding, base.allowModifiers, modifier, (IComparer<Vector2>)Vector2Comparer.instance);
		}
		if (!CompositeUtility.ReadValueAsButton(ref context, binding, base.allowModifiers, modifier))
		{
			return Vector2.zero;
		}
		return Vector2.one;
	}

	public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = ((InputBindingComposite<Vector2>)this).ReadValue(ref context);
		return ((Vector2)(ref val)).magnitude;
	}

	public static InputManager.CompositeData GetCompositeData()
	{
		return new InputManager.CompositeData(CompositeUtility.GetCompositeTypeName(typeof(Vector2WithModifiersComposite)), ActionType.Button, new InputManager.CompositeComponentData[1]
		{
			new InputManager.CompositeComponentData(ActionComponent.Press, "binding", "modifier")
		});
	}
}
