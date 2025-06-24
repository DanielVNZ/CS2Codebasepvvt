using System;
using Game.Input;

namespace Game.Settings;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SettingsUIGamepadActionAttribute : SettingsUIInputActionAttribute
{
	public SettingsUIGamepadActionAttribute(string name, ActionType type = ActionType.Button, bool allowModifiers = true, bool developerOnly = false, Mode mode = Mode.Analog, string[] usages = null, string[] interactions = null, string[] processors = null)
		: base(name, InputManager.DeviceType.Gamepad, type, allowModifiers, developerOnly, mode, usages, interactions, processors)
	{
	}

	public SettingsUIGamepadActionAttribute(string name, ActionType type, Mode mode, params string[] customUsages)
		: base(name, InputManager.DeviceType.Gamepad, type, mode, customUsages)
	{
	}

	public SettingsUIGamepadActionAttribute(string name, ActionType type, params string[] customUsages)
		: base(name, InputManager.DeviceType.Gamepad, type, Mode.Analog, customUsages)
	{
	}

	public SettingsUIGamepadActionAttribute(string name, Mode mode, params string[] customUsages)
		: base(name, InputManager.DeviceType.Gamepad, ActionType.Button, mode, customUsages)
	{
	}

	public SettingsUIGamepadActionAttribute(string name, params string[] customUsages)
		: base(name, InputManager.DeviceType.Gamepad, ActionType.Button, Mode.Analog, customUsages)
	{
	}
}
