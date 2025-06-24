using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.PSI.Common;
using Colossal.PSI.PdxSdk;
using Game.SceneFlow;
using Game.Simulation;
using Game.UI;
using Unity.Entities;

namespace Game.Settings;

[FileLocation("Settings")]
public class GeneralSettings : Setting
{
	public enum FPSMode
	{
		Off,
		Simple,
		Advanced,
		Precise
	}

	public enum AutoSaveCount
	{
		One = 1,
		Three = 3,
		Ten = 10,
		Fifty = 50,
		Hundred = 100,
		Unlimited = 0
	}

	public enum AutoSaveInterval
	{
		OneMinute = 60,
		TwoMinutes = 120,
		FiveMinutes = 300,
		TenMinutes = 600,
		ThirtyMinutes = 1800,
		OneHour = 3600
	}

	public const string kName = "General";

	private AutoReloadMode m_AssetDatabaseAutoReloadMode;

	private SimulationSystem.PerformancePreference m_PerformancePreference;

	private PdxSdkPlatform m_Manager;

	private bool m_OptionalTelemetryConsentFaulted;

	[SettingsUIPlatform(/*Could not decode attribute arguments.*/)]
	public AutoReloadMode assetDatabaseAutoReloadMode
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_AssetDatabaseAutoReloadMode;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			m_AssetDatabaseAutoReloadMode = value;
			AssetDatabase.global.autoReloadMode = m_AssetDatabaseAutoReloadMode;
		}
	}

	public SimulationSystem.PerformancePreference performancePreference
	{
		get
		{
			return m_PerformancePreference;
		}
		set
		{
			if (m_PerformancePreference != value)
			{
				m_PerformancePreference = value;
				World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
				SimulationSystem simulationSystem = ((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetExistingSystemManaged<SimulationSystem>() : null);
				if (simulationSystem != null)
				{
					simulationSystem.performancePreference = value;
				}
			}
		}
	}

	[SettingsUIDeveloper]
	public FPSMode fpsMode { get; set; }

	public bool autoSave { get; set; }

	[SettingsUIDisableByCondition(typeof(GeneralSettings), "AutoSaveEnabled")]
	public AutoSaveInterval autoSaveInterval { get; set; }

	[SettingsUIDisableByCondition(typeof(GeneralSettings), "AutoSaveEnabled")]
	public AutoSaveCount autoSaveCount { get; set; }

	[SettingsUIDeveloper]
	[SettingsUIButton]
	[SettingsUIDisableByCondition(typeof(GeneralSettings), "CanSave")]
	public bool autoSaveNow
	{
		get
		{
			return true;
		}
		set
		{
			if (GameManager.instance.gameMode.IsGameOrEditor())
			{
				World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<AutoSaveSystem>()?.PerformAutoSave(this);
			}
		}
	}

	[Exclude]
	[SettingsUIHideByCondition(typeof(GeneralSettings), "HideTelemetryConsentChoice")]
	[SettingsUIWarning(typeof(GeneralSettings), "TelemetryConsentFaulted")]
	public bool allowOptionalTelemetry
	{
		get
		{
			PdxSdkPlatform manager = m_Manager;
			if (manager == null)
			{
				return false;
			}
			return manager.GetTelemetryConsentChoice();
		}
		set
		{
			if (m_Manager != null)
			{
				SetTelemetryConsentChoice(value);
			}
		}
	}

	[SettingsUIButton]
	[SettingsUIConfirmation(null, null)]
	public bool resetSettings
	{
		set
		{
			GameManager.instance.settings.Reset();
		}
	}

	private async void SetTelemetryConsentChoice(bool allow)
	{
		bool flag = await m_Manager.SetTelemetryConsentChoice(allow);
		m_OptionalTelemetryConsentFaulted = !flag;
		if (!flag)
		{
			GameManager.instance.userInterface.appBindings.ShowMessageDialog(new MessageDialog("Paradox.TELEMETRY_CONSENT_ERROR_TITLE", "Paradox.TELEMETRY_CONSENT_ERROR_DESCRIPTION", "Common.OK"), delegate
			{
			});
		}
	}

	private bool HideTelemetryConsentChoice()
	{
		if (m_Manager != null)
		{
			return !m_Manager.IsTelemetryConsentPresentable();
		}
		return true;
	}

	private bool TelemetryConsentFaulted()
	{
		return m_OptionalTelemetryConsentFaulted;
	}

	private void InitializePlatform()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		m_Manager = PlatformManager.instance.GetPSI<PdxSdkPlatform>("PdxSdk");
		PlatformManager.instance.onPlatformRegistered += (PlatformRegisteredHandler)delegate(IPlatformServiceIntegration psi)
		{
			PdxSdkPlatform val = (PdxSdkPlatform)(object)((psi is PdxSdkPlatform) ? psi : null);
			if (val != null)
			{
				m_Manager = val;
			}
		};
	}

	public GeneralSettings()
	{
		SetDefaults();
		InitializePlatform();
	}

	public override void SetDefaults()
	{
		autoSave = false;
		autoSaveInterval = AutoSaveInterval.FiveMinutes;
		autoSaveCount = AutoSaveCount.Three;
		fpsMode = FPSMode.Off;
		assetDatabaseAutoReloadMode = (AutoReloadMode)0;
		performancePreference = SimulationSystem.PerformancePreference.Balanced;
	}

	public static bool CanSave()
	{
		return !GameManager.instance.gameMode.IsGameOrEditor();
	}

	public static bool AutoSaveEnabled()
	{
		return !SharedSettings.instance.general.autoSave;
	}
}
