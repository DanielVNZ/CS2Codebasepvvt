using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace Game.Input;

[DisplayStringFormat("{up}/{left}/{down}/{right}")]
[DisplayName("CO Up/Down/Left/Right Binding With Modifiers")]
public class Vector2SeparatedWithModifiersComposite : AnalogValueInputBindingComposite<Vector2>
{
	[InputControl(layout = "Button")]
	public int up;

	[InputControl(layout = "Button")]
	public int down;

	[InputControl(layout = "Button")]
	public int left;

	[InputControl(layout = "Button")]
	public int right;

	[InputControl(layout = "Button")]
	public int upModifier;

	[InputControl(layout = "Button")]
	public int downModifier;

	[InputControl(layout = "Button")]
	public int leftModifier;

	[InputControl(layout = "Button")]
	public int rightModifier;

	public override Vector2 ReadValue(ref InputBindingCompositeContext context)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsDummy)
		{
			return default(Vector2);
		}
		if (m_Mode == Mode.Analog)
		{
			float num = CompositeUtility.ReadValue(ref context, up, m_AllowModifiers, upModifier, DefaultComparer<float>.instance);
			float num2 = CompositeUtility.ReadValue(ref context, down, m_AllowModifiers, downModifier, DefaultComparer<float>.instance);
			float num3 = CompositeUtility.ReadValue(ref context, left, m_AllowModifiers, leftModifier, DefaultComparer<float>.instance);
			float num4 = CompositeUtility.ReadValue(ref context, right, m_AllowModifiers, rightModifier, DefaultComparer<float>.instance);
			return DpadControl.MakeDpadVector(num, num2, num3, num4);
		}
		bool num5 = CompositeUtility.ReadValueAsButton(ref context, up, m_AllowModifiers, upModifier);
		bool flag = CompositeUtility.ReadValueAsButton(ref context, down, m_AllowModifiers, downModifier);
		bool flag2 = CompositeUtility.ReadValueAsButton(ref context, left, m_AllowModifiers, leftModifier);
		bool flag3 = CompositeUtility.ReadValueAsButton(ref context, right, m_AllowModifiers, rightModifier);
		return DpadControl.MakeDpadVector(num5, flag, flag2, flag3, m_Mode == Mode.DigitalNormalized);
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
		return new InputManager.CompositeData(CompositeUtility.GetCompositeTypeName(typeof(Vector2SeparatedWithModifiersComposite)), ActionType.Vector2, new InputManager.CompositeComponentData[4]
		{
			new InputManager.CompositeComponentData(ActionComponent.Up, "up", "upModifier"),
			new InputManager.CompositeComponentData(ActionComponent.Down, "down", "downModifier"),
			new InputManager.CompositeComponentData(ActionComponent.Left, "left", "leftModifier"),
			new InputManager.CompositeComponentData(ActionComponent.Right, "right", "rightModifier")
		});
	}
}
