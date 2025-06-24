using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Settings;

[SettingsUIAdvanced]
[SettingsUISection("VolumetricsQualitySettings")]
[SettingsUIDisableByCondition(typeof(VolumetricsQualitySettings), "IsOptionsDisabled")]
public class VolumetricsQualitySettings : QualitySetting<VolumetricsQualitySettings>
{
	private static Fog m_FogComponent;

	[SettingsUIHidden]
	public bool enabled { get; set; }

	[SettingsUISlider(min = 0.001f, max = 1f, step = 0.1f, unit = "floatSingleFraction")]
	public float budget { get; set; }

	[SettingsUISlider(min = 0.001f, max = 1f, step = 0.1f, unit = "floatSingleFraction")]
	public float resolutionDepthRatio { get; set; }

	private static VolumetricsQualitySettings highQuality => new VolumetricsQualitySettings
	{
		budget = 0.666f,
		resolutionDepthRatio = 0.5f,
		enabled = true
	};

	private static VolumetricsQualitySettings mediumQuality => new VolumetricsQualitySettings
	{
		budget = 0.33f,
		resolutionDepthRatio = 0.666f,
		enabled = true
	};

	private static VolumetricsQualitySettings lowQuality => new VolumetricsQualitySettings
	{
		budget = 0.166f,
		resolutionDepthRatio = 0.666f,
		enabled = true
	};

	private static VolumetricsQualitySettings disabled => new VolumetricsQualitySettings
	{
		enabled = false
	};

	static VolumetricsQualitySettings()
	{
		QualitySetting<VolumetricsQualitySettings>.RegisterSetting(Level.Disabled, disabled);
		QualitySetting<VolumetricsQualitySettings>.RegisterSetting(Level.Low, lowQuality);
		QualitySetting<VolumetricsQualitySettings>.RegisterSetting(Level.Medium, mediumQuality);
		QualitySetting<VolumetricsQualitySettings>.RegisterSetting(Level.High, highQuality);
	}

	public VolumetricsQualitySettings()
	{
	}

	public VolumetricsQualitySettings(Level quality, VolumeProfile profile)
	{
		CreateVolumeComponent<Fog>(profile, ref m_FogComponent);
		SetLevel(quality, apply: false);
	}

	public override void Apply()
	{
		base.Apply();
		if ((Object)(object)m_FogComponent != (Object)null)
		{
			ApplyState((VolumeParameter<bool>)(object)m_FogComponent.enableVolumetricFog, enabled);
			ApplyState((VolumeParameter<float>)(object)m_FogComponent.m_VolumetricFogBudget, budget);
			ApplyState((VolumeParameter<float>)(object)m_FogComponent.m_ResolutionDepthRatio, resolutionDepthRatio);
		}
	}

	public override bool IsOptionsDisabled()
	{
		if (!IsOptionFullyDisabled())
		{
			return !enabled;
		}
		return true;
	}
}
