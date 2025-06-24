using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Settings;

[SettingsUIAdvanced]
[SettingsUISection("WaterQualitySettings")]
[SettingsUIDisableByCondition(typeof(WaterQualitySettings), "IsOptionsDisabled")]
public class WaterQualitySettings : QualitySetting<WaterQualitySettings>
{
	private static WaterRendering m_WaterRenderingComponent;

	public bool waterflow { get; set; }

	[SettingsUISlider(min = 0f, max = 15f, step = 0.1f, unit = "floatSingleFraction")]
	public float maxTessellationFactor { get; set; }

	[SettingsUISlider(min = 0f, max = 4000f, step = 1f, unit = "floatSingleFraction")]
	public float tessellationFactorFadeStart { get; set; }

	[SettingsUISlider(min = 10f, max = 4000f, step = 1f, unit = "floatSingleFraction")]
	public float tessellationFactorFadeRange { get; set; }

	private static WaterQualitySettings highQuality => new WaterQualitySettings
	{
		maxTessellationFactor = 10f,
		tessellationFactorFadeStart = 150f,
		tessellationFactorFadeRange = 1850f,
		waterflow = true
	};

	private static WaterQualitySettings mediumQuality => new WaterQualitySettings
	{
		maxTessellationFactor = 6f,
		tessellationFactorFadeStart = 150f,
		tessellationFactorFadeRange = 1850f,
		waterflow = true
	};

	private static WaterQualitySettings lowQuality => new WaterQualitySettings
	{
		maxTessellationFactor = 2f,
		tessellationFactorFadeStart = 150f,
		tessellationFactorFadeRange = 1850f,
		waterflow = false
	};

	public WaterQualitySettings()
	{
	}

	static WaterQualitySettings()
	{
		QualitySetting<WaterQualitySettings>.RegisterSetting(Level.Low, lowQuality);
		QualitySetting<WaterQualitySettings>.RegisterSetting(Level.Medium, mediumQuality);
		QualitySetting<WaterQualitySettings>.RegisterSetting(Level.High, highQuality);
	}

	public WaterQualitySettings(Level quality, VolumeProfile profile)
	{
		CreateVolumeComponent<WaterRendering>(profile, ref m_WaterRenderingComponent);
		SetLevel(quality, apply: false);
	}

	public override void Apply()
	{
		base.Apply();
		if ((Object)(object)m_WaterRenderingComponent != (Object)null)
		{
			ApplyState((VolumeParameter<float>)(object)m_WaterRenderingComponent.maxTessellationFactor, maxTessellationFactor);
			ApplyState((VolumeParameter<float>)(object)m_WaterRenderingComponent.tessellationFactorFadeStart, tessellationFactorFadeStart);
			ApplyState((VolumeParameter<float>)(object)m_WaterRenderingComponent.tessellationFactorFadeRange, tessellationFactorFadeRange);
		}
		foreach (WaterSurface instance in WaterSurface.instances)
		{
			instance.waterFlow = waterflow;
		}
	}
}
