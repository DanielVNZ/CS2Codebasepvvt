using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Settings;

[SettingsUIAdvanced]
[SettingsUISection("DepthOfFieldQualitySettings")]
[SettingsUIDisableByCondition(typeof(DepthOfFieldQualitySettings), "IsOptionsDisabled")]
public class DepthOfFieldQualitySettings : QualitySetting<DepthOfFieldQualitySettings>
{
	private static DepthOfField m_DOFComponent;

	[SettingsUIHidden]
	public bool enabled { get; set; }

	[SettingsUISlider(min = 3f, max = 8f, step = 1f, unit = "integer")]
	public int nearSampleCount { get; set; }

	[SettingsUISlider(min = 0f, max = 8f, step = 0.1f, unit = "floatSingleFraction")]
	public float nearMaxRadius { get; set; }

	[SettingsUISlider(min = 3f, max = 16f, step = 1f, unit = "integer")]
	public int farSampleCount { get; set; }

	[SettingsUISlider(min = 0f, max = 16f, step = 0.1f, unit = "floatSingleFraction")]
	public float farMaxRadius { get; set; }

	public DepthOfFieldResolution resolution { get; set; }

	public bool highQualityFiltering { get; set; }

	private static DepthOfFieldQualitySettings highQuality => new DepthOfFieldQualitySettings
	{
		enabled = true,
		nearSampleCount = 8,
		nearMaxRadius = 7f,
		farSampleCount = 14,
		farMaxRadius = 13f,
		resolution = (DepthOfFieldResolution)2,
		highQualityFiltering = true
	};

	private static DepthOfFieldQualitySettings mediumQuality => new DepthOfFieldQualitySettings
	{
		enabled = true,
		nearSampleCount = 5,
		nearMaxRadius = 4f,
		farSampleCount = 7,
		farMaxRadius = 8f,
		resolution = (DepthOfFieldResolution)2,
		highQualityFiltering = true
	};

	private static DepthOfFieldQualitySettings lowQuality => new DepthOfFieldQualitySettings
	{
		enabled = true,
		nearSampleCount = 3,
		nearMaxRadius = 2f,
		farSampleCount = 4,
		farMaxRadius = 5f,
		resolution = (DepthOfFieldResolution)4,
		highQualityFiltering = false
	};

	private static DepthOfFieldQualitySettings disabled => new DepthOfFieldQualitySettings
	{
		enabled = false
	};

	static DepthOfFieldQualitySettings()
	{
		QualitySetting<DepthOfFieldQualitySettings>.RegisterSetting(Level.Disabled, disabled);
		QualitySetting<DepthOfFieldQualitySettings>.RegisterSetting(Level.Low, lowQuality);
		QualitySetting<DepthOfFieldQualitySettings>.RegisterSetting(Level.Medium, mediumQuality);
		QualitySetting<DepthOfFieldQualitySettings>.RegisterSetting(Level.High, highQuality);
	}

	public DepthOfFieldQualitySettings()
	{
	}

	public DepthOfFieldQualitySettings(Level quality, VolumeProfile profile)
	{
		CreateVolumeComponent<DepthOfField>(profile, ref m_DOFComponent);
		SetLevel(quality, apply: false);
	}

	public override void Apply()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.Apply();
		if ((Object)(object)m_DOFComponent != (Object)null)
		{
			ApplyState<DepthOfFieldMode>((VolumeParameter<DepthOfFieldMode>)(object)m_DOFComponent.focusMode, (DepthOfFieldMode)0, !enabled);
			ApplyState((VolumeParameter<int>)(object)m_DOFComponent.m_NearSampleCount, nearSampleCount);
			ApplyState((VolumeParameter<float>)(object)m_DOFComponent.m_NearMaxBlur, nearMaxRadius);
			ApplyState((VolumeParameter<int>)(object)m_DOFComponent.m_FarSampleCount, farSampleCount);
			ApplyState((VolumeParameter<float>)(object)m_DOFComponent.m_FarMaxBlur, farMaxRadius);
			ApplyState<DepthOfFieldResolution>((VolumeParameter<DepthOfFieldResolution>)(object)m_DOFComponent.m_Resolution, resolution);
			ApplyState((VolumeParameter<bool>)(object)m_DOFComponent.m_HighQualityFiltering, highQualityFiltering);
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
