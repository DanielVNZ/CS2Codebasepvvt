using Game.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input;

public class CameraMoveProcessor : PlatformProcessor<Vector2>
{
	public float m_ScaleX = 1f;

	public float m_ScaleY = 1f;

	public override Vector2 Process(Vector2 value, InputControl control)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!base.needProcess)
		{
			return value;
		}
		InputSettings input = SharedSettings.instance.input;
		value.x *= m_ScaleX;
		value.y *= m_ScaleY;
		Vector2 val = value;
		value = val * m_DeviceType switch
		{
			ProcessorDeviceType.Mouse => input.mouseMoveSensitivity, 
			ProcessorDeviceType.Keyboard => input.keyboardMoveSensitivity, 
			ProcessorDeviceType.Gamepad => input.gamepadMoveSensitivity, 
			_ => 1f, 
		};
		return value;
	}
}
