using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Settings;

[SettingsUIAdvanced]
[SettingsUISection("FogQualitySettings")]
[SettingsUIDisableByCondition(typeof(FogQualitySettings), "IsOptionsDisabled")]
public class FogQualitySettings : QualitySetting<FogQualitySettings>
{
	private static Fog m_FogComponent;

	[SettingsUIHidden]
	public bool enabled { get; set; }

	private static FogQualitySettings lowQuality => new FogQualitySettings
	{
		enabled = true
	};

	private static FogQualitySettings disabled => new FogQualitySettings
	{
		enabled = false
	};

	static FogQualitySettings()
	{
		QualitySetting<FogQualitySettings>.RegisterMockName(Level.Low, "Enabled");
		QualitySetting<FogQualitySettings>.RegisterSetting(Level.Disabled, disabled);
		QualitySetting<FogQualitySettings>.RegisterSetting(Level.Low, lowQuality);
	}

	public FogQualitySettings()
	{
	}

	public FogQualitySettings(Level quality, VolumeProfile profile)
	{
		CreateVolumeComponent<Fog>(profile, ref m_FogComponent);
		SetLevel(quality, apply: false);
	}

	public override void Apply()
	{
		base.Apply();
		if ((Object)(object)m_FogComponent != (Object)null)
		{
			ApplyState((VolumeParameter<bool>)(object)m_FogComponent.enabled, enabled);
			VolumetricsQualitySettings qualitySetting = SharedSettings.instance.graphics.GetQualitySetting<VolumetricsQualitySettings>();
			qualitySetting.disableSetting = !enabled;
			if (!enabled)
			{
				qualitySetting.enabled = enabled;
			}
		}
	}
}
