using Game.Rendering;
using Unity.Entities;
using UnityEngine.Rendering.VirtualTexturing;

namespace Game.Settings;

[SettingsUIAdvanced]
[SettingsUISection("TextureQualitySettings")]
[SettingsUIDisableByCondition(typeof(TextureQualitySettings), "IsOptionsDisabled")]
public class TextureQualitySettings : QualitySetting<TextureQualitySettings>
{
	[SettingsUISlider(min = 0f, max = 3f, step = 1f, unit = "integer")]
	public int mipbias { get; set; }

	public FilterMode filterMode { get; set; }

	private static TextureQualitySettings highQuality => new TextureQualitySettings
	{
		mipbias = 0,
		filterMode = (FilterMode)2
	};

	private static TextureQualitySettings mediumQuality => new TextureQualitySettings
	{
		mipbias = 1,
		filterMode = (FilterMode)2
	};

	private static TextureQualitySettings lowQuality => new TextureQualitySettings
	{
		mipbias = 2,
		filterMode = (FilterMode)1
	};

	private static TextureQualitySettings veryLowQuality => new TextureQualitySettings
	{
		mipbias = 3,
		filterMode = (FilterMode)1
	};

	static TextureQualitySettings()
	{
		QualitySetting<TextureQualitySettings>.RegisterSetting(Level.VeryLow, veryLowQuality);
		QualitySetting<TextureQualitySettings>.RegisterSetting(Level.Low, lowQuality);
		QualitySetting<TextureQualitySettings>.RegisterSetting(Level.Medium, mediumQuality);
		QualitySetting<TextureQualitySettings>.RegisterSetting(Level.High, highQuality);
	}

	public TextureQualitySettings()
	{
	}

	public TextureQualitySettings(Level quality)
	{
		SetLevel(quality, apply: false);
	}

	public override void Apply()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		base.Apply();
		World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
		((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetExistingSystemManaged<ManagedBatchSystem>() : null)?.ResetVT(mipbias, filterMode);
	}
}
