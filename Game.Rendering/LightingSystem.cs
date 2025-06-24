using System;
using Game.Prefabs;
using Game.Settings;
using Game.Simulation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Rendering;

public class LightingSystem : GameSystemBase
{
	private static class ShaderID
	{
		public static readonly int _TargetLUT = Shader.PropertyToID("_ResultLUT");

		public static readonly int _SourceLUT = Shader.PropertyToID("_SourceLUT");

		public static readonly int _DestinationLUT = Shader.PropertyToID("_DestinationLUT");

		public static readonly int _BlendLUT = Shader.PropertyToID("_LUTBlend");
	}

	public enum State
	{
		Dawn,
		Sunrise,
		Day,
		Sunset,
		Dusk,
		Night,
		Invalid
	}

	private PlanetarySystem m_PlanetarySystem;

	protected EntityQuery m_TimeSettingGroup;

	private Exposure m_Exposure;

	private PhysicallyBasedSky m_PhysicallyBasedSky;

	private ColorAdjustments m_ColorAdjustments;

	private IndirectLightingController m_Indirect;

	private Tonemapping m_Tonemap;

	private bool m_PostProcessingSetup;

	private DayNightCycleData m_NightDayCycleData;

	private Volume m_Volume;

	private VolumeProfile m_Profile;

	private RenderTexture m_BlendResult;

	private ComputeShader m_LUTBlend;

	private int m_KernalBlend = -1;

	private State m_LastState = State.Invalid;

	private float m_LastDelta = -1f;

	private bool shadowDisabled { get; set; }

	public float dayLightBrightness { get; private set; }

	public State state
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			PlanetarySystem.LightData sunLight = m_PlanetarySystem.SunLight;
			if (!sunLight.isValid)
			{
				return State.Invalid;
			}
			float delta;
			return CalculateState(float3.op_Implicit(sunLight.transform.position), float3.op_Implicit(sunLight.transform.forward), out delta);
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		base.OnCreate();
		m_PlanetarySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PlanetarySystem>();
		m_NightDayCycleData = Resources.Load<DayNightCycleData>("DayNight/Default");
		SetupPostprocessing();
		m_LUTBlend = Resources.Load<ComputeShader>("DayNight/LUTBlend");
		if (Object.op_Implicit((Object)(object)m_LUTBlend))
		{
			m_KernalBlend = m_LUTBlend.FindKernel("CSBlend");
			RenderTextureDescriptor val = default(RenderTextureDescriptor);
			((RenderTextureDescriptor)(ref val)).autoGenerateMips = false;
			((RenderTextureDescriptor)(ref val)).bindMS = false;
			((RenderTextureDescriptor)(ref val)).depthBufferBits = 0;
			((RenderTextureDescriptor)(ref val)).dimension = (TextureDimension)3;
			((RenderTextureDescriptor)(ref val)).enableRandomWrite = true;
			((RenderTextureDescriptor)(ref val)).graphicsFormat = (GraphicsFormat)48;
			((RenderTextureDescriptor)(ref val)).memoryless = (RenderTextureMemoryless)0;
			((RenderTextureDescriptor)(ref val)).height = 32;
			((RenderTextureDescriptor)(ref val)).width = 32;
			((RenderTextureDescriptor)(ref val)).volumeDepth = 32;
			((RenderTextureDescriptor)(ref val)).mipCount = 1;
			((RenderTextureDescriptor)(ref val)).msaaSamples = 1;
			((RenderTextureDescriptor)(ref val)).sRGB = false;
			((RenderTextureDescriptor)(ref val)).useDynamicScale = false;
			((RenderTextureDescriptor)(ref val)).useMipMap = false;
			RenderTextureDescriptor val2 = val;
			m_BlendResult = new RenderTexture(val2);
			m_BlendResult.Create();
		}
		m_TimeSettingGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_TimeSettingGroup);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ((Object)(object)m_Volume != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)m_Volume).gameObject);
		}
		if ((Object)(object)m_BlendResult != (Object)null)
		{
			Object.Destroy((Object)(object)m_BlendResult);
		}
	}

	private float CalcObscured(PlanetarySystem.LightData moon, PlanetarySystem.LightData night, float range = 0.3f)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		float y = moon.transform.position.y;
		float y2 = night.transform.position.y;
		if (y != y2)
		{
			return math.clamp((y2 - y) / range, 0f, 1f);
		}
		return 0f;
	}

	private void EnableShadows(PlanetarySystem.LightData lightData, bool enabled)
	{
		lightData.additionalData.EnableShadows(enabled && !shadowDisabled);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
		ShadowsQualitySettings shadowsQualitySettings = SharedSettings.instance?.graphics?.GetQualitySetting<ShadowsQualitySettings>();
		if (shadowsQualitySettings != null)
		{
			shadowDisabled = !shadowsQualitySettings.enabled;
		}
		PlanetarySystem.LightData moonLight = m_PlanetarySystem.MoonLight;
		PlanetarySystem.LightData sunLight = m_PlanetarySystem.SunLight;
		PlanetarySystem.LightData nightLight = m_PlanetarySystem.NightLight;
		if (!sunLight.isValid || !moonLight.isValid || !nightLight.isValid || (Object)(object)m_NightDayCycleData == (Object)null)
		{
			return;
		}
		dayLightBrightness = math.saturate(sunLight.additionalData.intensity / 110000f);
		float delta;
		State state = CalculateState(float3.op_Implicit(sunLight.transform.position), float3.op_Implicit(sunLight.transform.forward), out delta);
		if (m_PlanetarySystem.overrideTime)
		{
			m_LastState = State.Invalid;
		}
		else
		{
			if ((state == m_LastState && delta < m_LastDelta) || NextState(state) == m_LastState)
			{
				state = m_LastState;
				delta = m_LastDelta;
			}
			m_LastState = state;
			m_LastDelta = delta;
		}
		float num = CalcObscured(moonLight, nightLight, m_NightDayCycleData.NightLightObscuredRange);
		float num2 = math.lerp(m_NightDayCycleData.NightLightIntensity, m_NightDayCycleData.NightLightObscuredIntensity, num);
		float num3 = m_NightDayCycleData.MoonIntensity - (num2 - m_NightDayCycleData.NightLightIntensity);
		float num4 = math.lerp(m_NightDayCycleData.NightIndirectReflectiveMultiplier, m_NightDayCycleData.NightObscuredIndirectReflectiveMultiplier, num);
		float num5 = math.lerp(m_NightDayCycleData.NightIndirectDiffuseMultiplier, m_NightDayCycleData.NightObscuredIndirectDiffuseMultiplier, num);
		if (!m_NightDayCycleData.UseLUT)
		{
			((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = null;
			((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = m_NightDayCycleData.LutContribution;
		}
		((VolumeParameter)m_Tonemap.mode).overrideState = m_NightDayCycleData.UseLUT;
		((VolumeParameter)m_ColorAdjustments.colorFilter).overrideState = m_NightDayCycleData.UseFilters;
		switch (state)
		{
		case State.Dawn:
		{
			((VolumeParameter<float>)(object)m_Exposure.limitMax).value = m_NightDayCycleData.NightExposureMax;
			((VolumeParameter<float>)(object)m_Exposure.limitMin).value = math.lerp(m_NightDayCycleData.NightExposureLowMin, m_NightDayCycleData.DayExposureMin, delta);
			moonLight.additionalData.intensity = num3;
			moonLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			bool flag2 = sunLight.additionalData.intensity > num3;
			EnableShadows(moonLight, !flag2);
			EnableShadows(sunLight, flag2);
			nightLight.additionalData.intensity = num2;
			nightLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(nightLight, enabled: false);
			((VolumeParameter<float>)(object)((SkySettings)m_PhysicallyBasedSky).exposure).value = math.lerp(m_NightDayCycleData.NightSkyExposure, m_NightDayCycleData.DaySkyExposure, delta);
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.zenithTint).value = Color.Lerp(m_NightDayCycleData.NightZenithTint, m_NightDayCycleData.DayZenithTint, delta);
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.horizonTint).value = Color.Lerp(m_NightDayCycleData.NightHorizonTint, m_NightDayCycleData.DayHorizonTint, delta);
			((VolumeParameter<Color>)(object)m_ColorAdjustments.colorFilter).value = Color.Lerp(m_NightDayCycleData.NightColorFilter, m_NightDayCycleData.SunriseColorFilter, delta);
			((VolumeParameter<float>)(object)m_ColorAdjustments.contrast).value = math.lerp(m_NightDayCycleData.NightContrast, m_NightDayCycleData.SunriseAndSunsetContrast, delta);
			((VolumeParameter<float>)(object)m_Indirect.reflectionLightingMultiplier).value = math.lerp(num4, 1f, delta);
			((VolumeParameter<float>)(object)m_Indirect.indirectDiffuseLightingMultiplier).value = math.lerp(num5, 1f, delta);
			if (m_NightDayCycleData.UseLUT)
			{
				BlendLUT(m_NightDayCycleData.NightLUT, m_NightDayCycleData.SunriseAndSunsetLUT, delta, m_NightDayCycleData.LutContribution);
			}
			break;
		}
		case State.Sunrise:
			((VolumeParameter<float>)(object)m_Exposure.limitMax).value = m_NightDayCycleData.DayExposureMax;
			((VolumeParameter<float>)(object)m_Exposure.limitMin).value = m_NightDayCycleData.DayExposureMin;
			moonLight.additionalData.intensity = math.lerp(num3, 0.5f, delta);
			moonLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(moonLight, enabled: false);
			nightLight.additionalData.intensity = math.lerp(num2, 0.5f, delta);
			nightLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(nightLight, enabled: false);
			EnableShadows(sunLight, enabled: true);
			((VolumeParameter<float>)(object)((SkySettings)m_PhysicallyBasedSky).exposure).value = m_NightDayCycleData.DaySkyExposure;
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.zenithTint).value = m_NightDayCycleData.DayZenithTint;
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.horizonTint).value = m_NightDayCycleData.DayHorizonTint;
			((VolumeParameter<Color>)(object)m_ColorAdjustments.colorFilter).value = Color.Lerp(m_NightDayCycleData.SunriseColorFilter, m_NightDayCycleData.DayColorFilter, delta);
			((VolumeParameter<float>)(object)m_ColorAdjustments.contrast).value = math.lerp(m_NightDayCycleData.SunriseAndSunsetContrast, m_NightDayCycleData.DayContrast, delta);
			((VolumeParameter<float>)(object)m_Indirect.reflectionLightingMultiplier).value = 1f;
			((VolumeParameter<float>)(object)m_Indirect.indirectDiffuseLightingMultiplier).value = 1f;
			if (m_NightDayCycleData.UseLUT)
			{
				BlendLUT(m_NightDayCycleData.SunriseAndSunsetLUT, m_NightDayCycleData.DayLUT, delta, m_NightDayCycleData.LutContribution);
			}
			break;
		case State.Day:
			((VolumeParameter<float>)(object)m_Exposure.limitMax).value = m_NightDayCycleData.DayExposureMax;
			((VolumeParameter<float>)(object)m_Exposure.limitMin).value = m_NightDayCycleData.DayExposureMin;
			((VolumeParameter<float>)(object)((SkySettings)m_PhysicallyBasedSky).exposure).value = m_NightDayCycleData.DaySkyExposure;
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.zenithTint).value = m_NightDayCycleData.DayZenithTint;
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.horizonTint).value = m_NightDayCycleData.DayHorizonTint;
			moonLight.additionalData.intensity = 0f;
			moonLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(moonLight, enabled: false);
			nightLight.additionalData.intensity = 0f;
			nightLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(nightLight, enabled: false);
			EnableShadows(sunLight, enabled: true);
			((VolumeParameter<Color>)(object)m_ColorAdjustments.colorFilter).value = m_NightDayCycleData.DayColorFilter;
			((VolumeParameter<float>)(object)m_ColorAdjustments.contrast).value = m_NightDayCycleData.DayContrast;
			((VolumeParameter<float>)(object)m_Indirect.reflectionLightingMultiplier).value = 1f;
			((VolumeParameter<float>)(object)m_Indirect.indirectDiffuseLightingMultiplier).value = 1f;
			if (m_NightDayCycleData.UseLUT)
			{
				((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = (Texture)(object)m_NightDayCycleData.DayLUT;
				((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = m_NightDayCycleData.LutContribution;
			}
			break;
		case State.Sunset:
			((VolumeParameter<float>)(object)m_Exposure.limitMax).value = m_NightDayCycleData.DayExposureMax;
			((VolumeParameter<float>)(object)m_Exposure.limitMin).value = m_NightDayCycleData.DayExposureMin;
			moonLight.additionalData.intensity = math.lerp(0.5f, num3, delta);
			moonLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(moonLight, enabled: false);
			nightLight.additionalData.intensity = math.lerp(0f, num2, delta);
			nightLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(nightLight, enabled: false);
			EnableShadows(sunLight, enabled: true);
			((VolumeParameter<float>)(object)((SkySettings)m_PhysicallyBasedSky).exposure).value = m_NightDayCycleData.DaySkyExposure;
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.zenithTint).value = m_NightDayCycleData.DayZenithTint;
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.horizonTint).value = m_NightDayCycleData.DayHorizonTint;
			((VolumeParameter<Color>)(object)m_ColorAdjustments.colorFilter).value = Color.Lerp(m_NightDayCycleData.DayColorFilter, m_NightDayCycleData.SunsetColorFilter, delta);
			((VolumeParameter<float>)(object)m_ColorAdjustments.contrast).value = math.lerp(m_NightDayCycleData.DayContrast, m_NightDayCycleData.SunriseAndSunsetContrast, delta);
			((VolumeParameter<float>)(object)m_Indirect.reflectionLightingMultiplier).value = 1f;
			((VolumeParameter<float>)(object)m_Indirect.indirectDiffuseLightingMultiplier).value = 1f;
			if (m_NightDayCycleData.UseLUT)
			{
				BlendLUT(m_NightDayCycleData.DayLUT, m_NightDayCycleData.SunriseAndSunsetLUT, delta, m_NightDayCycleData.LutContribution);
			}
			break;
		case State.Dusk:
		{
			((VolumeParameter<float>)(object)m_Exposure.limitMax).value = m_NightDayCycleData.NightExposureMax;
			((VolumeParameter<float>)(object)m_Exposure.limitMin).value = m_NightDayCycleData.NightExposureLowMin;
			moonLight.additionalData.intensity = num3;
			moonLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			bool flag = sunLight.additionalData.intensity > num3;
			EnableShadows(moonLight, !flag);
			EnableShadows(sunLight, flag);
			nightLight.additionalData.intensity = num2;
			nightLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(nightLight, enabled: false);
			((VolumeParameter<float>)(object)((SkySettings)m_PhysicallyBasedSky).exposure).value = math.lerp(m_NightDayCycleData.DaySkyExposure, m_NightDayCycleData.NightSkyExposure, delta);
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.zenithTint).value = Color.Lerp(m_NightDayCycleData.DayZenithTint, m_NightDayCycleData.NightZenithTint, delta);
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.horizonTint).value = Color.Lerp(m_NightDayCycleData.DayHorizonTint, m_NightDayCycleData.NightHorizonTint, delta);
			((VolumeParameter<Color>)(object)m_ColorAdjustments.colorFilter).value = Color.Lerp(m_NightDayCycleData.SunsetColorFilter, m_NightDayCycleData.NightColorFilter, delta);
			((VolumeParameter<float>)(object)m_ColorAdjustments.contrast).value = math.lerp(m_NightDayCycleData.SunriseAndSunsetContrast, m_NightDayCycleData.NightContrast, delta);
			((VolumeParameter<float>)(object)m_Indirect.reflectionLightingMultiplier).value = math.lerp(1f, num4, delta);
			((VolumeParameter<float>)(object)m_Indirect.indirectDiffuseLightingMultiplier).value = math.lerp(1f, num5, delta);
			if (m_NightDayCycleData.UseLUT)
			{
				BlendLUT(m_NightDayCycleData.SunriseAndSunsetLUT, m_NightDayCycleData.NightLUT, delta, m_NightDayCycleData.LutContribution);
			}
			break;
		}
		case State.Night:
			((VolumeParameter<float>)(object)m_Exposure.limitMax).value = m_NightDayCycleData.NightExposureMax;
			((VolumeParameter<float>)(object)m_Exposure.limitMin).value = m_NightDayCycleData.NightExposureLowMin;
			((VolumeParameter<float>)(object)((SkySettings)m_PhysicallyBasedSky).exposure).value = m_NightDayCycleData.NightSkyExposure;
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.zenithTint).value = m_NightDayCycleData.NightZenithTint;
			((VolumeParameter<Color>)(object)m_PhysicallyBasedSky.horizonTint).value = m_NightDayCycleData.NightHorizonTint;
			moonLight.additionalData.intensity = num3;
			moonLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(moonLight, enabled: true);
			EnableShadows(sunLight, enabled: false);
			moonLight.additionalData.shadowTint = m_NightDayCycleData.MoonShadowTint;
			nightLight.additionalData.intensity = num2;
			nightLight.additionalData.color = m_NightDayCycleData.MoonLightColor;
			EnableShadows(nightLight, enabled: false);
			((VolumeParameter<Color>)(object)m_ColorAdjustments.colorFilter).value = m_NightDayCycleData.NightColorFilter;
			((VolumeParameter<float>)(object)m_ColorAdjustments.contrast).value = m_NightDayCycleData.NightContrast;
			((VolumeParameter<float>)(object)m_Indirect.reflectionLightingMultiplier).value = num4;
			((VolumeParameter<float>)(object)m_Indirect.indirectDiffuseLightingMultiplier).value = num5;
			if (m_NightDayCycleData.UseLUT)
			{
				((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = (Texture)(object)m_NightDayCycleData.NightLUT;
				((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = m_NightDayCycleData.LutContribution;
			}
			break;
		}
		m_Profile.Reset();
	}

	private State CalculateState(float3 sunPosition, float3 sunDirection, out float delta)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_NightDayCycleData == (Object)null)
		{
			delta = 1f;
			return State.Day;
		}
		float3 val = default(float3);
		((float3)(ref val))._002Ector(sunPosition.x, 0f, sunPosition.z);
		if (math.dot(val, val) < 0.0001f)
		{
			((float3)(ref val))._002Ector(-1f, 0f, 0f);
		}
		else
		{
			val = math.normalize(val);
		}
		float num = math.acos(math.dot(-sunDirection, val)) * (180f / (float)Math.PI);
		bool num2 = val.x > 0f;
		if (sunDirection.y > 0f)
		{
			num = 0f - num;
		}
		if (num2)
		{
			delta = 1f;
			if (num < m_NightDayCycleData.DawnStartAngle)
			{
				return State.Night;
			}
			if (num >= m_NightDayCycleData.DawnStartAngle && num < m_NightDayCycleData.SunriseMidpointAngle)
			{
				delta = (num - m_NightDayCycleData.DawnStartAngle) / (m_NightDayCycleData.SunriseMidpointAngle - m_NightDayCycleData.DawnStartAngle);
				return State.Dawn;
			}
			if (num >= m_NightDayCycleData.SunriseMidpointAngle && num < m_NightDayCycleData.SunriseEndAngle)
			{
				delta = (num - m_NightDayCycleData.SunriseMidpointAngle) / (m_NightDayCycleData.SunriseEndAngle - m_NightDayCycleData.SunriseMidpointAngle);
				return State.Sunrise;
			}
			return State.Day;
		}
		delta = 1f;
		if (num > m_NightDayCycleData.SunsetStartAngle)
		{
			return State.Day;
		}
		if (num <= m_NightDayCycleData.SunsetStartAngle && num > m_NightDayCycleData.SunsetMidpointAngle)
		{
			delta = 1f - (num - m_NightDayCycleData.SunsetMidpointAngle) / (m_NightDayCycleData.SunsetStartAngle - m_NightDayCycleData.SunsetMidpointAngle);
			return State.Sunset;
		}
		if (num <= m_NightDayCycleData.SunsetMidpointAngle && num > m_NightDayCycleData.DuskEndAngle)
		{
			delta = 1f - (num - m_NightDayCycleData.DuskEndAngle) / (m_NightDayCycleData.SunsetMidpointAngle - m_NightDayCycleData.DuskEndAngle);
			return State.Dusk;
		}
		return State.Night;
	}

	private State NextState(State value)
	{
		return value switch
		{
			State.Dawn => State.Sunrise, 
			State.Sunrise => State.Day, 
			State.Day => State.Sunset, 
			State.Sunset => State.Dusk, 
			State.Dusk => State.Night, 
			State.Night => State.Dawn, 
			_ => State.Invalid, 
		};
	}

	private void SetupPostprocessing()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (!m_PostProcessingSetup)
		{
			GameObject val = new GameObject("LightingPostProcessVolume");
			Object.DontDestroyOnLoad((Object)(object)val);
			m_Volume = val.AddComponent<Volume>();
			m_Volume.priority = 1000f;
			m_Profile = m_Volume.profile;
			m_Exposure = m_Profile.Add<Exposure>(false);
			((VolumeComponent)m_Exposure).active = true;
			((VolumeParameter<ExposureMode>)(object)m_Exposure.mode).value = (ExposureMode)1;
			((VolumeParameter)m_Exposure.limitMin).overrideState = true;
			((VolumeParameter<float>)(object)m_Exposure.limitMin).value = -5f;
			((VolumeParameter)m_Exposure.limitMax).overrideState = true;
			((VolumeParameter<float>)(object)m_Exposure.limitMax).value = 14f;
			m_PhysicallyBasedSky = m_Profile.Add<PhysicallyBasedSky>(false);
			((VolumeParameter)m_PhysicallyBasedSky.zenithTint).overrideState = true;
			((VolumeParameter)m_PhysicallyBasedSky.horizonTint).overrideState = true;
			((VolumeParameter)((SkySettings)m_PhysicallyBasedSky).exposure).overrideState = true;
			((VolumeParameter<float>)(object)((SkySettings)m_PhysicallyBasedSky).exposure).value = 0f;
			m_ColorAdjustments = m_Profile.Add<ColorAdjustments>(false);
			((VolumeParameter)m_ColorAdjustments.colorFilter).overrideState = true;
			((VolumeParameter<Color>)(object)m_ColorAdjustments.colorFilter).value = new Color(1f, 1f, 1f);
			((VolumeParameter)m_ColorAdjustments.contrast).overrideState = true;
			((VolumeParameter<float>)(object)m_ColorAdjustments.contrast).value = 0f;
			m_Indirect = m_Profile.Add<IndirectLightingController>(false);
			((VolumeParameter)m_Indirect.reflectionLightingMultiplier).overrideState = true;
			((VolumeParameter)m_Indirect.indirectDiffuseLightingMultiplier).overrideState = true;
			((VolumeParameter<float>)(object)m_Indirect.reflectionLightingMultiplier).value = 1f;
			((VolumeParameter<float>)(object)m_Indirect.indirectDiffuseLightingMultiplier).value = 1f;
			m_Tonemap = m_Profile.Add<Tonemapping>(false);
			((VolumeParameter)m_Tonemap.mode).overrideState = true;
			((VolumeParameter<TonemappingMode>)(object)m_Tonemap.mode).value = (TonemappingMode)4;
			((VolumeParameter)m_Tonemap.lutContribution).overrideState = true;
			((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = 0.5f;
			((VolumeParameter)m_Tonemap.lutTexture).overrideState = true;
			((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = null;
			m_PostProcessingSetup = true;
		}
	}

	private void BlendLUT(Texture3D source, Texture3D destination, float delta, float lutContribution)
	{
		if ((Object)(object)source != (Object)null && (Object)(object)destination != (Object)null)
		{
			if ((Object)(object)m_LUTBlend != (Object)null)
			{
				m_LUTBlend.SetTexture(m_KernalBlend, ShaderID._TargetLUT, (Texture)(object)m_BlendResult);
				m_LUTBlend.SetTexture(m_KernalBlend, ShaderID._SourceLUT, (Texture)(object)source);
				m_LUTBlend.SetTexture(m_KernalBlend, ShaderID._DestinationLUT, (Texture)(object)destination);
				m_LUTBlend.SetFloat(ShaderID._BlendLUT, math.clamp(delta, 0f, 1f));
				m_LUTBlend.Dispatch(m_KernalBlend, 32, 32, 32);
				((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = (Texture)(object)m_BlendResult;
				((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = lutContribution;
			}
			else
			{
				((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = (Texture)(object)((delta < 0.5f) ? source : destination);
				((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = lutContribution;
			}
		}
		else if ((Object)(object)source != (Object)null && (Object)(object)destination == (Object)null)
		{
			((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = (Texture)(object)source;
			((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = math.lerp(lutContribution, 0f, delta);
		}
		else if ((Object)(object)destination != (Object)null && (Object)(object)source == (Object)null)
		{
			((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = (Texture)(object)destination;
			((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = math.lerp(0f, lutContribution, delta);
		}
		else
		{
			((VolumeParameter<Texture>)(object)m_Tonemap.lutTexture).value = null;
			((VolumeParameter<float>)(object)m_Tonemap.lutContribution).value = lutContribution;
		}
	}

	[Preserve]
	public LightingSystem()
	{
	}
}
