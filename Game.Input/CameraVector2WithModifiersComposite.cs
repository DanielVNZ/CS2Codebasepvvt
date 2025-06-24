using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace Game.Input;

[DisplayStringFormat("{binding}")]
[DisplayName("CO Camera Vector 2D With Modifiers")]
public class CameraVector2WithModifiersComposite : AnalogValueInputBindingComposite<Vector2>
{
	public bool m_ModifierActuatesControl;

	[InputControl(layout = "Vector2")]
	public int vector;

	[InputControl(layout = "Button")]
	public int trigger;

	public override Vector2 ReadValue(ref InputBindingCompositeContext context)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsDummy)
		{
			return default(Vector2);
		}
		if (m_Mode == Mode.Analog)
		{
			return CompositeUtility.ReadValue<Vector2>(ref context, vector, allowModifiers: true, trigger, (IComparer<Vector2>)Vector2Comparer.instance);
		}
		if (!CompositeUtility.ReadValueAsButton(ref context, vector, allowModifiers: true, trigger))
		{
			return Vector2.zero;
		}
		return Vector2.one;
	}

	public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
	{
		if (CompositeUtility.CheckModifiers(ref context, allowModifiers: true, trigger))
		{
			if (m_ModifierActuatesControl && trigger != 0)
			{
				return Mathf.Abs(((InputBindingCompositeContext)(ref context)).ReadValue<float, ModifiersComparer>(trigger, default(ModifiersComparer)));
			}
			return ((InputBindingCompositeContext)(ref context)).EvaluateMagnitude(vector);
		}
		return 0f;
	}

	public static InputManager.CompositeData GetCompositeData()
	{
		return new InputManager.CompositeData(CompositeUtility.GetCompositeTypeName(typeof(CameraVector2WithModifiersComposite)), ActionType.Button, new InputManager.CompositeComponentData[1]
		{
			new InputManager.CompositeComponentData(ActionComponent.Press, "trigger", string.Empty)
		});
	}
}
