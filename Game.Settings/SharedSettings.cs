using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.IO.AssetDatabase;
using Colossal.Localization;
using Colossal.PSI.Common;
using Game.Input;
using Game.PSI.PdxSdk;
using Game.SceneFlow;
using UnityEngine.InputSystem;

namespace Game.Settings;

public class SharedSettings
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static OnStatusChangedEventHandler _003C_003E9__48_0;

		internal void _003CRegisterInOptionsUI_003Eb__48_0(IPlatformServiceIntegration psi)
		{
			new About().RegisterInOptionsUI("About");
		}
	}

	private readonly List<Setting> m_Settings = new List<Setting>();

	public static SharedSettings instance => GameManager.instance?.settings;

	public GeneralSettings general { get; private set; }

	public AudioSettings audio { get; private set; }

	public GameplaySettings gameplay { get; private set; }

	public RadioSettings radio { get; private set; }

	public GraphicsSettings graphics { get; private set; }

	public EditorSettings editor { get; private set; }

	public InterfaceSettings userInterface { get; private set; }

	public InputSettings input { get; private set; }

	public KeybindingSettings keybinding { get; private set; }

	public ModdingSettings modding { get; private set; }

	public UserState userState { get; private set; }

	public SharedSettings(LocalizationManager localizationManager)
	{
		m_Settings.Add(general = new GeneralSettings());
		m_Settings.Add(audio = new AudioSettings());
		m_Settings.Add(gameplay = new GameplaySettings());
		m_Settings.Add(radio = new RadioSettings());
		m_Settings.Add(graphics = new GraphicsSettings());
		m_Settings.Add(editor = new EditorSettings());
		m_Settings.Add(userInterface = new InterfaceSettings());
		m_Settings.Add(input = new InputSettings());
		m_Settings.Add(this.userState = new UserState());
		m_Settings.Add(keybinding = new KeybindingSettings());
		m_Settings.Add(modding = new ModdingSettings());
		LoadSettings();
		LauncherSettings.LoadSettings(localizationManager, this);
		localizationManager.SetActiveLocale(userInterface.locale);
	}

	public void RegisterInOptionsUI()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		general.RegisterInOptionsUI("General");
		graphics.RegisterInOptionsUI("Graphics");
		gameplay.RegisterInOptionsUI("Gameplay");
		userInterface.RegisterInOptionsUI("Interface");
		audio.RegisterInOptionsUI("Audio");
		input.RegisterInOptionsUI("Input");
		modding.RegisterInOptionsUI("Modding");
		if (GameManager.instance.configuration.developerMode)
		{
			new About().RegisterInOptionsUI("About");
			PlatformManager obj = PlatformManager.instance;
			object obj2 = _003C_003Ec._003C_003E9__48_0;
			if (obj2 == null)
			{
				OnStatusChangedEventHandler val = delegate
				{
					new About().RegisterInOptionsUI("About");
				};
				_003C_003Ec._003C_003E9__48_0 = val;
				obj2 = (object)val;
			}
			obj.onStatusChanged += (OnStatusChangedEventHandler)obj2;
		}
		InputSystem.onDeviceChange += OnDeviceChange;
		InputManager.instance.EventControlSchemeChanged += OnControlSchemeChanged;
		void OnControlSchemeChanged(InputManager.ControlScheme controlScheme)
		{
			input.RegisterInOptionsUI("Input");
		}
		void OnDeviceChange(InputDevice changedDevice, InputDeviceChange change)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			if ((int)change == 0 || (int)change == 1)
			{
				input.RegisterInOptionsUI("Input");
			}
		}
	}

	public void LoadSettings()
	{
		AssetDatabase.global.LoadSettings("General Settings", (object)general, (object)new GeneralSettings());
		AssetDatabase.global.LoadSettings("Audio Settings", (object)audio, (object)new AudioSettings());
		AssetDatabase.global.LoadSettings("Gameplay Settings", (object)gameplay, (object)new GameplaySettings());
		AssetDatabase.global.LoadSettings("Radio Settings", (object)radio, (object)new RadioSettings());
		AssetDatabase.global.LoadSettings("Graphics Settings", (object)graphics, (object)new GraphicsSettings());
		AssetDatabase.global.LoadSettings("Editor Settings", (object)editor, (object)new EditorSettings());
		AssetDatabase.global.LoadSettings("Interface Settings", (object)userInterface, (object)new InterfaceSettings());
		AssetDatabase.global.LoadSettings("Input Settings", (object)input, (object)new InputSettings());
		AssetDatabase.global.LoadSettings("Keybinding Settings", (object)keybinding, (object)null);
		AssetDatabase.global.LoadSettings("Modding Settings", (object)modding, (object)new ModdingSettings());
	}

	public void LoadUserSettings()
	{
		AssetDatabase.global.LoadSettings("User Settings", (object)userState, (object)new UserState());
	}

	public void Reset()
	{
		Launcher.DeleteLastSaveMetadata();
		foreach (Setting setting in m_Settings)
		{
			setting.SetDefaults();
			setting.ApplyAndSave();
		}
	}

	public void Apply()
	{
		foreach (Setting setting in m_Settings)
		{
			setting.Apply();
		}
	}
}
