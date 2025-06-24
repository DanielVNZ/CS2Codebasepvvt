using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Game.Rendering;
using Game.SceneFlow;
using Game.UI.Localization;
using Game.UI.Menu;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine;
using UnityEngine.NVIDIA;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Settings;

[FileLocation("Settings")]
[SettingsUIGroupOrder(new string[]
{
	"Main", "DepthOfField", "UpscalersGroup", "Quality", "DynamicResolutionScaleSettings", "AntiAliasingQualitySettings", "CloudsQualitySettings", "FogQualitySettings", "VolumetricsQualitySettings", "SSAOQualitySettings",
	"SSGIQualitySettings", "SSRQualitySettings", "DepthOfFieldQualitySettings", "MotionBlurQualitySettings", "ShadowsQualitySettings", "TerrainQualitySettings", "WaterQualitySettings", "LevelOfDetailQualitySettings", "AnimationQualitySettings", "TextureQualitySettings"
})]
[SettingsUIShowGroupName]
public class GraphicsSettings : GlobalQualitySettings
{
	public enum DepthOfFieldMode
	{
		Disabled,
		Physical,
		TiltShift
	}

	public enum CursorMode
	{
		Free,
		ConfinedToWindow
	}

	public enum DlssQuality
	{
		Off,
		Auto,
		MaximumQuality,
		Balanced,
		MaximumPerformance,
		UltraPerformance
	}

	public const string kName = "Graphics";

	private int m_resolutionItemsVersion;

	public const string kMainGroup = "Main";

	public const string kDepthOfFieldGroup = "DepthOfField";

	public const string kQualityGroup = "Quality";

	public const string kUpscalersGroup = "UpscalersGroup";

	private const int kDisplayIndexNotSelected = -1;

	private bool m_ShowAllResolutions;

	private ScreenResolution m_Resolution;

	private int m_DlssQuality;

	private static Camera m_Camera;

	private static Volume m_VolumeOverride;

	[Exclude]
	[SettingsUIPlatform(/*Could not decode attribute arguments.*/)]
	[SettingsUISection("General", "Main")]
	[SettingsUIDropdown(typeof(GraphicsSettings), "GetDisplayIndexValues")]
	[SettingsUIHideByCondition(typeof(ScreenHelper), "HasMultipleDisplay", true)]
	[SettingsUISetter(typeof(GraphicsSettings), "OnSetDisplayIndex")]
	public int currentDisplayIndex
	{
		get
		{
			if (displayIndex != -1)
			{
				return displayIndex;
			}
			IReadOnlyList<DisplayInfo> displayInfos;
			return GetActiveDisplayIndex(out displayInfos);
		}
		set
		{
			displayIndex = value;
		}
	}

	[SettingsUIHidden]
	public int displayIndex { get; set; }

	[SettingsUIPlatform(/*Could not decode attribute arguments.*/)]
	[SettingsUISection("General", "Main")]
	[SettingsUIHideByCondition(typeof(ScreenHelper), "HideAdditionalResolutionOption")]
	[SettingsUIValueVersion(typeof(GraphicsSettings), "GetResolutionItemsVersion")]
	[SettingsUISetter(typeof(GraphicsSettings), "OnResolutionItemsNeedRebuild")]
	public bool showAllResolutions
	{
		get
		{
			return m_ShowAllResolutions;
		}
		set
		{
			if (value != m_ShowAllResolutions)
			{
				m_ShowAllResolutions = value;
				resolution = resolution;
			}
		}
	}

	[SettingsUIPlatform(/*Could not decode attribute arguments.*/)]
	[SettingsUISection("General", "Main")]
	[SettingsUIDropdown(typeof(GraphicsSettings), "GetScreenResolutionValues")]
	[SettingsUIValueVersion(typeof(GraphicsSettings), "GetResolutionItemsVersion")]
	[SettingsUISetter(typeof(GraphicsSettings), "OnSetResolution")]
	public ScreenResolution resolution
	{
		get
		{
			return m_Resolution;
		}
		set
		{
			m_Resolution = ScreenHelper.GetClosestAvailable(value, showAllResolutions);
		}
	}

	[SettingsUIPlatform(/*Could not decode attribute arguments.*/)]
	[SettingsUISection("General", "Main")]
	[SettingsUISetter(typeof(GraphicsSettings), "OnSetDisplayMode")]
	public DisplayMode displayMode { get; set; }

	[SettingsUIPlatform(/*Could not decode attribute arguments.*/)]
	[SettingsUISection("General", "Main")]
	public bool vSync { get; set; }

	[SettingsUISection("General", "Main")]
	[SettingsUISlider(min = 1f, max = 3f, step = 1f, unit = "integer", updateOnDragEnd = true)]
	public int maxFrameLatency { get; set; }

	[SettingsUIPlatform(/*Could not decode attribute arguments.*/)]
	[SettingsUISection("General", "Main")]
	public CursorMode cursorMode { get; set; }

	[SettingsUISection("General", "Main", "DepthOfField")]
	public DepthOfFieldMode depthOfFieldMode { get; set; }

	[SettingsUIAdvanced]
	[SettingsUISection("General", "DepthOfField")]
	[SettingsUIDisableByCondition(typeof(GraphicsSettings), "IsTiltShiftDisabled")]
	[SettingsUISlider(min = 0f, max = 100f, step = 0.1f, unit = "percentageSingleFraction", scalarMultiplier = 100f, updateOnDragEnd = true)]
	public float tiltShiftNearStart { get; set; }

	[SettingsUIAdvanced]
	[SettingsUISection("General", "DepthOfField")]
	[SettingsUIDisableByCondition(typeof(GraphicsSettings), "IsTiltShiftDisabled")]
	[SettingsUISlider(min = 0f, max = 100f, step = 0.1f, unit = "percentageSingleFraction", scalarMultiplier = 100f, updateOnDragEnd = true)]
	public float tiltShiftNearEnd { get; set; }

	[SettingsUIAdvanced]
	[SettingsUISection("General", "DepthOfField")]
	[SettingsUIDisableByCondition(typeof(GraphicsSettings), "IsTiltShiftDisabled")]
	[SettingsUISlider(min = 0f, max = 100f, step = 0.1f, unit = "percentageSingleFraction", scalarMultiplier = 100f, updateOnDragEnd = true)]
	public float tiltShiftFarStart { get; set; }

	[SettingsUIAdvanced]
	[SettingsUISection("General", "DepthOfField")]
	[SettingsUIDisableByCondition(typeof(GraphicsSettings), "IsTiltShiftDisabled")]
	[SettingsUISlider(min = 0f, max = 100f, step = 0.1f, unit = "percentageSingleFraction", scalarMultiplier = 100f, updateOnDragEnd = true)]
	public float tiltShiftFarEnd { get; set; }

	[SettingsUIPlatform(/*Could not decode attribute arguments.*/)]
	[SettingsUISection("General", "UpscalersGroup")]
	[SettingsUIDisableByCondition(typeof(GraphicsSettings), "isDLSSDisabled")]
	public DlssQuality dlssQuality { get; set; }

	public bool isDlssActive
	{
		get
		{
			if (IsDLSSDectected())
			{
				return m_DlssQuality >= 0;
			}
			return false;
		}
	}

	public bool isFsr2Active => false;

	private bool isDLSSDisabled
	{
		get
		{
			if (IsDLSSDectected())
			{
				return isFsr2Active;
			}
			return true;
		}
	}

	private bool isFSRDisabled => isDlssActive;

	private bool IsDLSSDectected()
	{
		return HDDynamicResolutionPlatformCapabilities.DLSSDetected;
	}

	static GraphicsSettings()
	{
		QualitySetting<GlobalQualitySettings>.RegisterMockName(Level.Disabled, "VeryLow");
		QualitySetting<GlobalQualitySettings>.RegisterSetting(Level.High, new GlobalQualitySettings());
		QualitySetting<GlobalQualitySettings>.RegisterSetting(Level.Medium, new GlobalQualitySettings());
		QualitySetting<GlobalQualitySettings>.RegisterSetting(Level.Low, new GlobalQualitySettings());
		QualitySetting<GlobalQualitySettings>.RegisterSetting(Level.Disabled, new GlobalQualitySettings());
	}

	public override AutomaticSettings.SettingPageData GetPageData(string id, bool addPrefix)
	{
		AutomaticSettings.SettingPageData settingPageData = AutomaticSettings.FillSettingsPage(this, id, addPrefix);
		AutomaticSettings.ManualProperty property = new AutomaticSettings.ManualProperty(GetType(), typeof(Level), "Level")
		{
			canRead = true,
			canWrite = true,
			getter = (object settings) => ((GraphicsSettings)settings).GetLevel(),
			setter = delegate(object settings, object value)
			{
				((GraphicsSettings)settings).SetLevel((Level)value);
			},
			attributes = 
			{
				(Attribute)new SettingsUIDropdownAttribute(typeof(QualitySetting), "GetQualityValues"),
				(Attribute)new SettingsUIPathAttribute("GraphicsSettings.globalQuality"),
				(Attribute)new SettingsUIDisplayNameAttribute("GraphicsSettings.globalQuality")
			}
		};
		AutomaticSettings.SettingItemData item = new AutomaticSettings.SettingItemData(AutomaticSettings.WidgetType.AdvancedEnumDropdown, this, property, settingPageData.prefix)
		{
			isAdvanced = false,
			simpleGroup = "Quality",
			advancedGroup = "Quality"
		};
		settingPageData["General"].AddItem(item);
		foreach (QualitySetting qualitySetting in base.qualitySettings)
		{
			qualitySetting.AddToPageData(settingPageData);
		}
		return settingPageData;
	}

	internal override void AddToPageData(AutomaticSettings.SettingPageData pageData)
	{
	}

	public override void Apply()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		base.Apply();
		ApplyResolution();
		QualitySettings.vSyncCount = (vSync ? 1 : 0);
		QualitySettings.maxQueuedFrames = maxFrameLatency;
		Cursor.lockState = cursorMode.ToUnityCursorMode();
		if (TryGetGameplayCamera(ref m_Camera))
		{
			ApplyDLSSAutoSettings(m_Camera);
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (QualitySetting item in EnumerateQualitySettings())
		{
			stringBuilder.AppendFormat("{0}: {1}", item.GetType().Name, item.GetLevel());
			if (item != base.lastSetting)
			{
				stringBuilder.Append(" - ");
			}
			item.Apply();
		}
		Setting.log.InfoFormat("Current resolution: {1} {2} - Current quality settings: {0}", (object)stringBuilder.ToString(), (object)resolution, (object)displayMode);
	}

	private int GetActiveDisplayIndex(out IReadOnlyList<DisplayInfo> displayInfos)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<DisplayInfo> list = new List<DisplayInfo>();
		Screen.GetDisplayLayout(list);
		displayInfos = list;
		for (int i = 0; i < list.Count; i++)
		{
			DisplayInfo val = list[i];
			if (((DisplayInfo)(ref val)).Equals(Screen.mainWindowDisplayInfo))
			{
				return i;
			}
		}
		return 0;
	}

	private IEnumerator MoveToDisplay(DisplayInfo display)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		yield return Screen.MoveMainWindowTo(ref display, Screen.mainWindowPosition);
		ScreenHelper.RebuildResolutions();
		OnResolutionItemsNeedRebuild(value: false);
		resolution = resolution;
	}

	public void ApplyResolution()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<DisplayInfo> displayInfos;
		int activeDisplayIndex = GetActiveDisplayIndex(out displayInfos);
		if (displayIndex < 0 || displayIndex >= displayInfos.Count)
		{
			displayIndex = 0;
		}
		if (activeDisplayIndex != displayIndex)
		{
			Setting.log.InfoFormat("Switching from display {0} to {1}", (object)activeDisplayIndex, (object)displayIndex);
			((MonoBehaviour)GameManager.instance).StartCoroutine(MoveToDisplay(displayInfos[displayIndex]));
		}
		ScreenResolution currentResolution = ScreenHelper.currentResolution;
		DisplayMode currentDisplayMode = ScreenHelper.currentDisplayMode;
		if (currentResolution != resolution || currentDisplayMode != displayMode)
		{
			Setting.log.InfoFormat("Applying resolution: {0} {1}", (object)resolution, (object)displayMode);
			if (resolution.isValid)
			{
				Screen.SetResolution(resolution.width, resolution.height, ScreenHelper.GetFullscreenMode(displayMode), resolution.refreshRate);
			}
			else
			{
				Setting.log.ErrorFormat("Resolution {0} {1} is invalid", (object)resolution, (object)displayMode);
			}
		}
	}

	private DLSSQuality ToDlssQuality(DlssQuality dlssQuality)
	{
		return (DLSSQuality)(dlssQuality switch
		{
			DlssQuality.MaximumQuality => 2, 
			DlssQuality.Balanced => 1, 
			DlssQuality.MaximumPerformance => 0, 
			DlssQuality.UltraPerformance => 3, 
			_ => throw new Exception($"Unsupported upscaler quality conversion {dlssQuality}"), 
		});
	}

	private void ApplyDLSSAutoSettings(Camera camera)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected I4, but got Unknown
		m_DlssQuality = -1;
		if (IsDLSSDectected())
		{
			DlssQuality dlssQuality = this.dlssQuality;
			if (this.dlssQuality == DlssQuality.Auto)
			{
				ScreenResolution currentResolution = ScreenHelper.currentResolution;
				long num = currentResolution.width * currentResolution.height;
				dlssQuality = ((num >= 2073600) ? ((num <= 3686400) ? DlssQuality.MaximumQuality : ((num > 8294400) ? DlssQuality.UltraPerformance : DlssQuality.MaximumPerformance)) : DlssQuality.Off);
			}
			if (dlssQuality != DlssQuality.Off)
			{
				m_DlssQuality = (int)ToDlssQuality(dlssQuality);
			}
		}
		bool flag = m_DlssQuality >= 0;
		HDAdditionalCameraData component = ((Component)camera).GetComponent<HDAdditionalCameraData>();
		component.allowDeepLearningSuperSampling = flag;
		component.deepLearningSuperSamplingUseCustomQualitySettings = true;
		if (flag)
		{
			component.deepLearningSuperSamplingQuality = (uint)m_DlssQuality;
		}
	}

	private void CreateVolumeOverride()
	{
		if ((Object)(object)m_VolumeOverride == (Object)null)
		{
			m_VolumeOverride = VolumeHelper.CreateVolume("VolumeQualitySettingsOverride", 100);
		}
	}

	private void CleanupVolumeOverride()
	{
		VolumeHelper.DestroyVolume(m_VolumeOverride);
	}

	public T GetVolumeOverride<T>() where T : VolumeComponent
	{
		T result = default(T);
		if ((Object)(object)m_VolumeOverride != (Object)null && m_VolumeOverride.profileRef.TryGet<T>(ref result))
		{
			return result;
		}
		return default(T);
	}

	public GraphicsSettings()
	{
		CreateVolumeOverride();
		AddQualitySetting(new DynamicResolutionScaleSettings(Level.High));
		AddQualitySetting(new AntiAliasingQualitySettings(Level.High));
		AddQualitySetting(new CloudsQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new FogQualitySettings(Level.Low, m_VolumeOverride.profileRef));
		AddQualitySetting(new VolumetricsQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new SSAOQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new SSGIQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new SSRQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new DepthOfFieldQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new MotionBlurQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new ShadowsQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new TerrainQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new WaterQualitySettings(Level.High, m_VolumeOverride.profileRef));
		AddQualitySetting(new LevelOfDetailQualitySettings(Level.High));
		AddQualitySetting(new AnimationQualitySettings(Level.High));
		AddQualitySetting(new TextureQualitySettings(Level.High));
		SetDefaults();
	}

	public override void SetDefaults()
	{
		showAllResolutions = false;
		displayIndex = -1;
		resolution = ScreenHelper.currentResolution;
		displayMode = ScreenHelper.currentDisplayMode;
		depthOfFieldMode = DepthOfFieldMode.Physical;
		vSync = false;
		tiltShiftNearStart = 0.5f;
		tiltShiftNearEnd = 0.25f;
		tiltShiftFarStart = 0.25f;
		tiltShiftFarEnd = 0.5f;
		maxFrameLatency = QualitySettings.maxQueuedFrames;
		cursorMode = CursorMode.ConfinedToWindow;
		dlssQuality = (IsDLSSDectected() ? DlssQuality.Auto : DlssQuality.Off);
		base.SetDefaults();
	}

	public void OnSetResolution(ScreenResolution resolution)
	{
		if (displayMode == DisplayMode.Fullscreen)
		{
			World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<OptionsUISystem>().ShowDisplayConfirmation();
		}
	}

	public void OnSetDisplayMode(DisplayMode mode)
	{
		if (displayMode != DisplayMode.Fullscreen && mode == DisplayMode.Fullscreen)
		{
			World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<OptionsUISystem>().ShowDisplayConfirmation();
		}
	}

	public void OnSetDisplayIndex(int index)
	{
		if (displayMode == DisplayMode.Fullscreen)
		{
			World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<OptionsUISystem>().ShowDisplayConfirmation();
		}
	}

	public bool IsTiltShiftDisabled()
	{
		return depthOfFieldMode != DepthOfFieldMode.TiltShift;
	}

	public void OnResolutionItemsNeedRebuild(bool value)
	{
		m_resolutionItemsVersion++;
	}

	public int GetResolutionItemsVersion()
	{
		return m_resolutionItemsVersion;
	}

	[Preserve]
	public static DropdownItem<int>[] GetDisplayIndexValues()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		List<DisplayInfo> list = new List<DisplayInfo>();
		Screen.GetDisplayLayout(list);
		List<DropdownItem<int>> list2 = new List<DropdownItem<int>>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(new DropdownItem<int>
			{
				value = i,
				displayName = ((!string.IsNullOrEmpty(list[i].name)) ? LocalizedString.Value($"{i}: {list[i].name}") : new LocalizedString("Options.DISPLAY_INDEX_FORMAT", null, new Dictionary<string, ILocElement> { 
				{
					"INDEX",
					LocalizedString.Value(i.ToString())
				} }))
			});
		}
		return list2.ToArray();
	}

	[Preserve]
	public static DropdownItem<ScreenResolution>[] GetScreenResolutionValues()
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		bool num = SharedSettings.instance.graphics.showAllResolutions;
		ScreenResolution[] availableResolutions = ScreenHelper.GetAvailableResolutions(num);
		List<DropdownItem<ScreenResolution>> list = new List<DropdownItem<ScreenResolution>>(availableResolutions.Length);
		string unit = (num ? "screenFrequency" : "integer");
		ScreenResolution[] array = availableResolutions;
		for (int i = 0; i < array.Length; i++)
		{
			ScreenResolution value = array[i];
			DropdownItem<ScreenResolution> obj = new DropdownItem<ScreenResolution>
			{
				value = value
			};
			Dictionary<string, ILocElement> dictionary = new Dictionary<string, ILocElement>();
			int width = value.width;
			dictionary.Add("WIDTH", LocalizedString.Value(width.ToString("D")));
			width = value.height;
			dictionary.Add("HEIGHT", LocalizedString.Value(width.ToString("D")));
			RefreshRate refreshRate = value.refreshRate;
			dictionary.Add("REFRESH_RATE", new LocalizedNumber<double>(((RefreshRate)(ref refreshRate)).value, unit));
			obj.displayName = new LocalizedString("Options.SCREEN_RESOLUTION_FORMAT", null, dictionary);
			list.Add(obj);
		}
		return list.ToArray();
	}
}
