using System;
using Colossal;
using UnityEngine.InputSystem.Utilities;

namespace Game.Input;

public interface ICustomComposite
{
	const bool kDefaultIsRebindable = true;

	const bool kDefaultIsModifiersRebindable = true;

	const bool kDefaultAllowModifiers = true;

	const bool kDefaultCanBeEmpty = true;

	const bool kDefaultDeveloperOnly = false;

	const bool kDefaultBuiltIn = true;

	const bool kDefaultIsDummy = false;

	const bool kDefaultIsHidden = false;

	const Mode kDefaultMode = Mode.DigitalNormalized;

	const OptionGroupOverride kDefaultOptionGroupOverride = OptionGroupOverride.None;

	const Platform kDefaultPlatform = (Platform)255;

	bool isRebindable { get; }

	bool isModifiersRebindable { get; }

	bool allowModifiers { get; }

	bool canBeEmpty { get; }

	bool developerOnly { get; }

	Platform platform { get; }

	bool builtIn { get; }

	bool isDummy { get; }

	bool isHidden { get; }

	Usages usages { get; }

	NameAndParameters parameters { get; }

	OptionGroupOverride optionGroupOverride { get; }

	Guid linkedGuid { get; }

	static Usages defaultUsages => Usages.defaultUsages.Copy(readOnly: false);
}
