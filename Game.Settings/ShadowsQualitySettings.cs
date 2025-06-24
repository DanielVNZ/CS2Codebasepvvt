using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Settings;

[SettingsUIAdvanced]
[SettingsUISection("ShadowsQualitySettings")]
[SettingsUIDisableByCondition(typeof(ShadowsQualitySettings), "IsOptionsDisabled")]
public class ShadowsQualitySettings : QualitySetting<ShadowsQualitySettings>
{
	private static HDShadowSettings m_CascadeShadows;

	private static HDAdditionalLightData m_SunLightData;

	[SettingsUIHidden]
	public bool enabled { get; set; }

	public int directionalShadowResolution { get; set; }

	public bool terrainCastShadows { get; set; }

	public float shadowCullingThresholdHeight { get; set; }

	public float shadowCullingThresholdVolume { get; set; }

	private static ShadowsQualitySettings highQuality => new ShadowsQualitySettings
	{
		enabled = true,
		directionalShadowResolution = 2048,
		terrainCastShadows = true,
		shadowCullingThresholdHeight = 1f,
		shadowCullingThresholdVolume = 1f
	};

	private static ShadowsQualitySettings mediumQuality => new ShadowsQualitySettings
	{
		enabled = true,
		directionalShadowResolution = 1024,
		terrainCastShadows = true,
		shadowCullingThresholdHeight = 1.5f,
		shadowCullingThresholdVolume = 1.5f
	};

	private static ShadowsQualitySettings lowQuality => new ShadowsQualitySettings
	{
		enabled = true,
		directionalShadowResolution = 512,
		terrainCastShadows = false,
		shadowCullingThresholdHeight = 2f,
		shadowCullingThresholdVolume = 2f
	};

	private static ShadowsQualitySettings disabled => new ShadowsQualitySettings
	{
		enabled = false
	};

	static ShadowsQualitySettings()
	{
		QualitySetting<ShadowsQualitySettings>.RegisterSetting(Level.Disabled, disabled);
		QualitySetting<ShadowsQualitySettings>.RegisterSetting(Level.Low, lowQuality);
		QualitySetting<ShadowsQualitySettings>.RegisterSetting(Level.Medium, mediumQuality);
		QualitySetting<ShadowsQualitySettings>.RegisterSetting(Level.High, highQuality);
	}

	public ShadowsQualitySettings()
	{
	}

	public ShadowsQualitySettings(Level quality, VolumeProfile profile)
	{
		CreateVolumeComponent<HDShadowSettings>(profile, ref m_CascadeShadows);
		SetLevel(quality, apply: false);
	}

	public override void Apply()
	{
		base.Apply();
		if ((Object)(object)m_SunLightData == (Object)null)
		{
			TryGetSunLightData(ref m_SunLightData);
		}
		if ((Object)(object)m_SunLightData != (Object)null)
		{
			m_SunLightData.EnableShadows(enabled);
		}
		if ((Object)(object)m_CascadeShadows != (Object)null)
		{
			((VolumeComponent)m_CascadeShadows).active = enabled;
		}
		foreach (TerrainSurface instance in TerrainSurface.instances)
		{
			instance.castShadows = terrainCastShadows;
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
