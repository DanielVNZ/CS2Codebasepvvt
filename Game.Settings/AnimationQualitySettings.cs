using UnityEngine;

namespace Game.Settings;

[SettingsUIAdvanced]
[SettingsUISection("AnimationQualitySettings")]
[SettingsUIDisableByCondition(typeof(AnimationQualitySettings), "IsOptionsDisabled")]
public class AnimationQualitySettings : QualitySetting<AnimationQualitySettings>
{
	public enum Skinning
	{
		TwoBones,
		FourBones
	}

	public Skinning maxBoneInfuence { get; set; }

	private static AnimationQualitySettings highQuality => new AnimationQualitySettings
	{
		maxBoneInfuence = Skinning.FourBones
	};

	private static AnimationQualitySettings mediumQuality => new AnimationQualitySettings
	{
		maxBoneInfuence = Skinning.TwoBones
	};

	static AnimationQualitySettings()
	{
		QualitySetting<AnimationQualitySettings>.RegisterSetting(Level.Medium, mediumQuality);
		QualitySetting<AnimationQualitySettings>.RegisterSetting(Level.High, highQuality);
	}

	public AnimationQualitySettings()
	{
	}

	public AnimationQualitySettings(Level quality)
	{
		SetLevel(quality, apply: false);
	}

	public override void Apply()
	{
		base.Apply();
		if (maxBoneInfuence == Skinning.FourBones)
		{
			Shader.DisableKeyword("TWO_BONES_INFLUENCE");
		}
		else if (maxBoneInfuence == Skinning.TwoBones)
		{
			Shader.EnableKeyword("TWO_BONES_INFLUENCE");
		}
	}
}
