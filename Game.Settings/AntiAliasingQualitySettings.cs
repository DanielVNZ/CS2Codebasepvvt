using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Settings;

[SettingsUIAdvanced]
[SettingsUISection("AntiAliasingQualitySettings")]
[SettingsUIDisableByCondition(typeof(AntiAliasingQualitySettings), "IsOptionsDisabled")]
public class AntiAliasingQualitySettings : QualitySetting<AntiAliasingQualitySettings>
{
	public enum AntialiasingMethod
	{
		None,
		FXAA,
		SMAA,
		TAA
	}

	private static HDAdditionalCameraData m_GameCamera;

	public AntialiasingMethod antiAliasingMethod { get; set; }

	public SMAAQualityLevel smaaQuality { get; set; }

	public MSAASamples outlinesMSAA { get; set; }

	private static AntiAliasingQualitySettings highQuality => new AntiAliasingQualitySettings
	{
		outlinesMSAA = (MSAASamples)8,
		antiAliasingMethod = AntialiasingMethod.SMAA,
		smaaQuality = (SMAAQualityLevel)2
	};

	private static AntiAliasingQualitySettings mediumQuality => new AntiAliasingQualitySettings
	{
		outlinesMSAA = (MSAASamples)4,
		antiAliasingMethod = AntialiasingMethod.SMAA,
		smaaQuality = (SMAAQualityLevel)0
	};

	private static AntiAliasingQualitySettings lowQuality => new AntiAliasingQualitySettings
	{
		outlinesMSAA = (MSAASamples)2,
		antiAliasingMethod = AntialiasingMethod.FXAA
	};

	private static AntiAliasingQualitySettings disabled => new AntiAliasingQualitySettings
	{
		outlinesMSAA = (MSAASamples)1,
		antiAliasingMethod = AntialiasingMethod.None
	};

	static AntiAliasingQualitySettings()
	{
		QualitySetting<AntiAliasingQualitySettings>.RegisterMockName(Level.Disabled, "None");
		QualitySetting<AntiAliasingQualitySettings>.RegisterMockName(Level.Low, "FXAA");
		QualitySetting<AntiAliasingQualitySettings>.RegisterMockName(Level.Medium, "LowSMAA");
		QualitySetting<AntiAliasingQualitySettings>.RegisterMockName(Level.High, "HighSMAA");
		QualitySetting<AntiAliasingQualitySettings>.RegisterSetting(Level.Disabled, disabled);
		QualitySetting<AntiAliasingQualitySettings>.RegisterSetting(Level.Low, lowQuality);
		QualitySetting<AntiAliasingQualitySettings>.RegisterSetting(Level.Medium, mediumQuality);
		QualitySetting<AntiAliasingQualitySettings>.RegisterSetting(Level.High, highQuality);
	}

	public AntiAliasingQualitySettings()
	{
	}

	public AntiAliasingQualitySettings(Level quality)
	{
		SetLevel(quality, apply: false);
	}

	private static AntialiasingMode ToAAMode(AntialiasingMethod method)
	{
		return (AntialiasingMode)(method switch
		{
			AntialiasingMethod.None => 0, 
			AntialiasingMethod.FXAA => 1, 
			AntialiasingMethod.SMAA => 3, 
			AntialiasingMethod.TAA => 2, 
			_ => 0, 
		});
	}

	public override void Apply()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.Apply();
		if (TryGetGameplayCamera(ref m_GameCamera))
		{
			if (!SharedSettings.instance.graphics.isDlssActive && !SharedSettings.instance.graphics.isFsr2Active)
			{
				m_GameCamera.antialiasing = ToAAMode(antiAliasingMethod);
				m_GameCamera.SMAAQuality = smaaQuality;
			}
			else
			{
				m_GameCamera.antialiasing = (AntialiasingMode)0;
			}
		}
	}

	public override bool IsOptionFullyDisabled()
	{
		if (!base.IsOptionFullyDisabled() && !SharedSettings.instance.graphics.isDlssActive)
		{
			return SharedSettings.instance.graphics.isFsr2Active;
		}
		return true;
	}
}
