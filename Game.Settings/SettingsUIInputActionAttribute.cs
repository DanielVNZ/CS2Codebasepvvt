using System;
using System.Collections.ObjectModel;
using Game.Input;

namespace Game.Settings;

public abstract class SettingsUIInputActionAttribute : Attribute
{
	public readonly string name;

	public readonly InputManager.DeviceType device;

	public readonly ActionType type;

	public readonly bool allowModifiers;

	public readonly bool developerOnly;

	public readonly Mode mode;

	public readonly ReadOnlyCollection<string> interactions;

	public readonly ReadOnlyCollection<string> processors;

	private readonly string[] customUsages;

	public Usages usages
	{
		get
		{
			if (customUsages != null && customUsages.Length != 0)
			{
				return new Usages(readOnly: true, customUsages);
			}
			return Usages.defaultUsages;
		}
	}

	protected SettingsUIInputActionAttribute(string name, InputManager.DeviceType device, ActionType type, bool allowModifiers, bool developerOnly, Mode mode, string[] customUsages, string[] interactions, string[] processors)
	{
		this.name = name;
		this.device = device;
		this.type = type;
		this.allowModifiers = allowModifiers;
		this.developerOnly = developerOnly;
		this.mode = mode;
		this.interactions = new ReadOnlyCollection<string>(interactions ?? Array.Empty<string>());
		this.processors = new ReadOnlyCollection<string>(processors ?? Array.Empty<string>());
		this.customUsages = customUsages ?? Array.Empty<string>();
	}

	protected SettingsUIInputActionAttribute(string name, InputManager.DeviceType device, ActionType type, Mode mode, string[] customUsages)
		: this(name, device, type, allowModifiers: true, developerOnly: false, mode, customUsages, Array.Empty<string>(), Array.Empty<string>())
	{
	}
}
