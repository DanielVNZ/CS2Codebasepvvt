using System;
using Game.Input;

namespace Game.Settings;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SettingsUIMouseActionAttribute : SettingsUIInputActionAttribute
{
	public SettingsUIMouseActionAttribute(string name, ActionType type = ActionType.Button, bool allowModifiers = true, bool developerOnly = false, string[] usages = null, string[] interactions = null, string[] processors = null)
		: base(name, InputManager.DeviceType.Mouse, type, allowModifiers, developerOnly, Mode.DigitalNormalized, usages, interactions, processors)
	{
	}

	public SettingsUIMouseActionAttribute(string name, ActionType type, params string[] customUsages)
		: base(name, InputManager.DeviceType.Mouse, type, Mode.DigitalNormalized, customUsages)
	{
	}

	public SettingsUIMouseActionAttribute(string name, params string[] customUsages)
		: base(name, InputManager.DeviceType.Mouse, ActionType.Button, Mode.DigitalNormalized, customUsages)
	{
	}
}
