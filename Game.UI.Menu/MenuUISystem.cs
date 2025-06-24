using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Colossal;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI;
using Colossal.UI.Binding;
using Game.Assets;
using Game.City;
using Game.Modding;
using Game.Prefabs;
using Game.Prefabs.Modes;
using Game.PSI.PdxSdk;
using Game.SceneFlow;
using Game.Serialization;
using Game.Settings;
using Game.Simulation;
using Game.UI.InGame;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.UI.Menu;

[CompilerGenerated]
public class MenuUISystem : UISystemBase, IPreDeserialize
{
	private enum MapFilter
	{
		None = -1,
		Default,
		Custom
	}

	public enum MenuScreen
	{
		Menu,
		NewGame,
		LoadGame,
		Options,
		Credits
	}

	public class ThemeInfo : IJsonWritable
	{
		public string id { get; set; }

		public string icon { get; set; }

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin("menu.ThemeInfo");
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.TypeEnd();
		}
	}

	public struct NewGameArgs : IJsonReadable
	{
		public string mapId;

		public string cityName;

		public string theme;

		public Dictionary<string, bool> options;

		public string gameMode;

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("mapId");
			reader.Read(ref mapId);
			reader.ReadProperty("cityName");
			reader.Read(ref cityName);
			reader.ReadProperty("theme");
			reader.Read(ref theme);
			reader.ReadProperty("options");
			ulong num = reader.ReadMapBegin();
			options = new Dictionary<string, bool>((int)num);
			string key = default(string);
			bool value = default(bool);
			for (ulong num2 = 0uL; num2 < num; num2++)
			{
				reader.ReadMapKeyValue();
				reader.Read(ref key);
				reader.Read(ref value);
				options.Add(key, value);
			}
			reader.ReadMapEnd();
			reader.ReadProperty("gameMode");
			reader.Read(ref gameMode);
			reader.ReadMapEnd();
		}
	}

	public struct LoadGameArgs : IJsonReadable
	{
		public string saveId;

		public string cityName;

		public Dictionary<string, bool> options;

		public string gameMode;

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("saveId");
			reader.Read(ref saveId);
			reader.ReadProperty("cityName");
			reader.Read(ref cityName);
			reader.ReadProperty("options");
			ulong num = reader.ReadMapBegin();
			options = new Dictionary<string, bool>((int)num);
			string key = default(string);
			bool value = default(bool);
			for (ulong num2 = 0uL; num2 < num; num2++)
			{
				reader.ReadMapKeyValue();
				reader.Read(ref key);
				reader.Read(ref value);
				options.Add(key, value);
			}
			reader.ReadMapEnd();
			reader.ReadProperty("gameMode");
			reader.Read(ref gameMode);
			reader.ReadMapEnd();
		}
	}

	private class GameOptions : IJsonWritable
	{
		public bool unlockAll;

		public bool unlimitedMoney;

		public bool unlockMapTiles;

		public HashSet<string> usedMods;

		public GameOptions(CityConfigurationSystem cityConfigurationSystem)
		{
			unlockAll = cityConfigurationSystem.unlockAll;
			unlimitedMoney = cityConfigurationSystem.unlimitedMoney;
			unlockMapTiles = cityConfigurationSystem.unlockMapTiles;
			usedMods = cityConfigurationSystem.usedMods;
		}

		public void Write(IJsonWriter writer)
		{
			writer.MapBegin(4u);
			writer.Write("unlockAll");
			writer.Write(unlockAll);
			writer.Write("unlimitedMoney");
			writer.Write(unlimitedMoney);
			writer.Write("unlockMapTiles");
			writer.Write(unlockMapTiles);
			writer.Write("usedMods");
			JsonWriterExtensions.ArrayBegin(writer, usedMods.Count);
			foreach (string item in usedMods)
			{
				writer.Write(item);
			}
			writer.ArrayEnd();
			writer.MapEnd();
		}
	}

	private class DefaultGameOptions : IJsonWritable
	{
		public bool leftHandTraffic => SharedSettings.instance.userState.leftHandTraffic;

		public bool naturalDisasters => SharedSettings.instance.userState.naturalDisasters;

		public bool unlockAll => SharedSettings.instance.userState.unlockAll;

		public bool unlimitedMoney => SharedSettings.instance.userState.unlimitedMoney;

		public bool unlockMapTiles => SharedSettings.instance.userState.unlockMapTiles;

		public void Write(IJsonWriter writer)
		{
			writer.MapBegin(5u);
			writer.Write("leftHandTraffic");
			writer.Write(leftHandTraffic);
			writer.Write("naturalDisasters");
			writer.Write(naturalDisasters);
			writer.Write("unlockAll");
			writer.Write(unlockAll);
			writer.Write("unlimitedMoney");
			writer.Write(unlimitedMoney);
			writer.Write("unlockMapTiles");
			writer.Write(unlockMapTiles);
			writer.MapEnd();
		}

		public Dictionary<string, bool> MergeOptions(Dictionary<string, bool> options)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>
			{
				["leftHandTraffic"] = leftHandTraffic,
				["naturalDisasters"] = naturalDisasters,
				["unlockAll"] = unlockAll,
				["unlimitedMoney"] = unlimitedMoney,
				["unlockMapTiles"] = unlockMapTiles
			};
			if (options != null)
			{
				foreach (KeyValuePair<string, bool> option in options)
				{
					dictionary[option.Key] = option.Value;
				}
			}
			return dictionary;
		}
	}

	public struct SaveabilityStatus : IJsonWritable
	{
		public bool canSave;

		public string reasonHash;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin("SaveabilityStatus");
			writer.PropertyName("canSave");
			writer.Write(canSave);
			writer.PropertyName("reasonHash");
			if (canSave)
			{
				writer.WriteNull();
			}
			else
			{
				writer.Write(reasonHash);
			}
			writer.TypeEnd();
		}
	}

	private const string kPreviewName = "SaveGamePanel";

	private const int kPreviewWidth = 680;

	private const int kPreviewHeight = 383;

	private const string kGroup = "menu";

	private PrefabSystem m_PrefabSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private TimeSystem m_TimeSystem;

	private MapMetadataSystem m_MapMetadataSystem;

	private StandaloneAssetUploadPanelUISystem m_AssetUploadPanelUISystem;

	private GameScreenUISystem m_GameScreenUISystem;

	private GameModeSystem m_GameModeSystem;

	private ValueBinding<int> m_ActiveScreenBinding;

	private GetterValueBinding<List<ThemeInfo>> m_ThemesBinding;

	private ValueBinding<List<MapInfo>> m_MapsBinding;

	private ValueBinding<HashSet<int>> m_AvailableMapFilters;

	private ValueBinding<int> m_SelectedMapFilter;

	private GetterValueBinding<List<GameModeInfo>> m_GameModesBinding;

	private ValueBinding<string> m_CurrentGameModeBinding;

	private ValueBinding<List<SaveInfo>> m_SavesBinding;

	private ValueBinding<string> m_SavePreviewBinding;

	private ValueBinding<string> m_LastSaveNameBinding;

	private ValueBinding<int> m_SaveGameSlotsBinding;

	private GetterValueBinding<SaveabilityStatus> m_SaveabilityBinding;

	private ValueBinding<List<string>> m_AvailableCloudTargetsBinding;

	private GetterValueBinding<string> m_SelectedCloudTargetBinding;

	private DefaultGameOptions m_DefaultGameOptions;

	private MenuHelpers.SaveGamePreviewSettings m_PreviewSettings = new MenuHelpers.SaveGamePreviewSettings();

	private EntityQuery m_XPQuery;

	private bool m_IsLoading;

	private string m_LastSelectedCloudTarget;

	private PdxModsUI m_ModsUI;

	private static int s_PreviewId;

	public MenuScreen activeScreen
	{
		get
		{
			return (MenuScreen)m_ActiveScreenBinding.value;
		}
		set
		{
			m_ActiveScreenBinding.Update((int)value);
		}
	}

	private bool IsEditorEnabled()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.instance.configuration.disableModding)
		{
			return PlatformExt.IsPlatformSet((Platform)7, Application.platform, false);
		}
		return false;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Expected O, but got Unknown
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Expected O, but got Unknown
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Expected O, but got Unknown
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Expected O, but got Unknown
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Expected O, but got Unknown
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Expected O, but got Unknown
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_MapMetadataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapMetadataSystem>();
		m_AssetUploadPanelUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<StandaloneAssetUploadPanelUISystem>();
		m_GameScreenUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GameScreenUISystem>();
		m_GameModeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GameModeSystem>();
		m_DefaultGameOptions = new DefaultGameOptions();
		AssetDatabase.global.LoadSettings("Save Preview Settings", (object)m_PreviewSettings, (object)null);
		AddBinding((IBinding)(object)(m_ActiveScreenBinding = new ValueBinding<int>("menu", "activeScreen", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)new ValueBinding<bool>("menu", "canExitGame", !Application.isConsolePlatform, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		Version current = Version.current;
		AddBinding((IBinding)(object)new ValueBinding<string>("menu", "gameVersion", ((Version)(ref current)).fullVersion, (IWriter<string>)null, (EqualityComparer<string>)null));
		AddBinding((IBinding)(object)(m_SavePreviewBinding = new ValueBinding<string>("menu", "savePreview", (string)null, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<string>("menu", "mapName", (Func<string>)(() => m_MapMetadataSystem.mapName), (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null));
		AddBinding((IBinding)(object)(m_LastSaveNameBinding = new ValueBinding<string>("menu", "lastSaveName", (string)null, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		int num = -1;
		AddBinding((IBinding)(object)(m_SaveGameSlotsBinding = new ValueBinding<int>("menu", "saveGameSlots", num, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_AvailableCloudTargetsBinding = new ValueBinding<List<string>>("menu", "availableCloudTargets", MenuHelpers.GetAvailableCloudTargets(), (IWriter<List<string>>)(object)new ListWriter<string>((IWriter<string>)null), (EqualityComparer<List<string>>)null)));
		AddUpdateBinding((IUpdateBinding)(object)(m_SelectedCloudTargetBinding = new GetterValueBinding<string>("menu", "selectedCloudTarget", (Func<string>)(() => MenuHelpers.GetSanitizedCloudTarget(SharedSettings.instance.userState.lastCloudTarget).name), (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)(m_SaveabilityBinding = new GetterValueBinding<SaveabilityStatus>("menu", "saveabilityStatus", (Func<SaveabilityStatus>)GetSaveabilityStatus, (IWriter<SaveabilityStatus>)(object)new ValueWriter<SaveabilityStatus>(), (EqualityComparer<SaveabilityStatus>)null)));
		AddBinding((IBinding)(object)(m_ThemesBinding = new GetterValueBinding<List<ThemeInfo>>("menu", "themes", (Func<List<ThemeInfo>>)GetThemes, (IWriter<List<ThemeInfo>>)(object)new ListWriter<ThemeInfo>((IWriter<ThemeInfo>)(object)new ValueWriter<ThemeInfo>()), (EqualityComparer<List<ThemeInfo>>)null)));
		AddBinding((IBinding)(object)(m_MapsBinding = new ValueBinding<List<MapInfo>>("menu", "maps", new List<MapInfo>(), (IWriter<List<MapInfo>>)(object)new ListWriter<MapInfo>((IWriter<MapInfo>)(object)new ValueWriter<MapInfo>()), (EqualityComparer<List<MapInfo>>)null)));
		AddBinding((IBinding)(object)(m_AvailableMapFilters = new ValueBinding<HashSet<int>>("menu", "availableMapFilters", GetAvailableMapFilters(), (IWriter<HashSet<int>>)(object)new CollectionWriter<int>((IWriter<int>)null), (EqualityComparer<HashSet<int>>)null)));
		AddBinding((IBinding)(object)(m_SelectedMapFilter = new ValueBinding<int>("menu", "selectedMapFilter", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_SavesBinding = new ValueBinding<List<SaveInfo>>("menu", "saves", new List<SaveInfo>(), (IWriter<List<SaveInfo>>)(object)new ListWriter<SaveInfo>((IWriter<SaveInfo>)(object)new ValueWriter<SaveInfo>()), (EqualityComparer<List<SaveInfo>>)null)));
		AddBinding((IBinding)(object)(m_GameModesBinding = new GetterValueBinding<List<GameModeInfo>>("menu", "gameModes", (Func<List<GameModeInfo>>)m_GameModeSystem.GetGameModeInfo, (IWriter<List<GameModeInfo>>)(object)new ListWriter<GameModeInfo>((IWriter<GameModeInfo>)(object)new ValueWriter<GameModeInfo>()), (EqualityComparer<List<GameModeInfo>>)null)));
		AddBinding((IBinding)(object)(m_CurrentGameModeBinding = new ValueBinding<string>("menu", "gameMode", m_GameModeSystem.currentModeName, (IWriter<string>)null, (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)new GetterValueBinding<List<string>>("menu", "creditFiles", (Func<List<string>>)GetCreditFiles, (IWriter<List<string>>)(object)new ListWriter<string>((IWriter<string>)new StringWriter()), (EqualityComparer<List<string>>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<DefaultGameOptions>("menu", "defaultGameOptions", (Func<DefaultGameOptions>)(() => m_DefaultGameOptions), (IWriter<DefaultGameOptions>)(object)new ValueWriter<DefaultGameOptions>(), (EqualityComparer<DefaultGameOptions>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<GameOptions>("menu", "gameOptions", (Func<GameOptions>)GetGameOptions, (IWriter<GameOptions>)(object)new ValueWriter<GameOptions>(), (EqualityComparer<GameOptions>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("menu", "modsEnabled", (Func<bool>)ModManager.AreModsEnabled, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("menu", "pdxModsUIEnabled", (Func<bool>)IsPdxModsUIEnabled, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddBinding((IBinding)(object)new ValueBinding<bool>("menu", "hideModsUIButton", !IsModdingEnabled(), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddBinding((IBinding)(object)new ValueBinding<bool>("menu", "hideEditorButton", !IsEditorEnabled(), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddBinding((IBinding)(object)new ValueBinding<bool>("menu", "displayModdingBetaBanners", true, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("menu", "hasCompletedTutorials", (Func<bool>)(() => SharedSettings.instance.userState.shownTutorials.ContainsValue(value: true)), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("menu", "showTutorials", (Func<bool>)(() => SharedSettings.instance.gameplay.showTutorials), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("menu", "dismissLoadGameConfirmation", (Func<bool>)(() => SharedSettings.instance.userInterface.dismissedConfirmations.Contains("LoadGame")), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("menu", "isModsUIActive", (Func<bool>)IsModsUIActive, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int>("menu", "setActiveScreen", (Action<int>)m_ActiveScreenBinding.Update, (IReader<int>)null));
		AddBinding((IBinding)new TriggerBinding("menu", "continueGame", (Action)SafeContinueGame));
		AddBinding((IBinding)(object)new TriggerBinding<NewGameArgs>("menu", "newGame", (Action<NewGameArgs>)SafeNewGame, (IReader<NewGameArgs>)(object)new ValueReader<NewGameArgs>()));
		AddBinding((IBinding)(object)new TriggerBinding<LoadGameArgs, bool>("menu", "loadGame", (Action<LoadGameArgs, bool>)SafeLoadGame, (IReader<LoadGameArgs>)(object)new ValueReader<LoadGameArgs>(), (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("menu", "saveGame", (Action<string>)SafeSaveGame, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("menu", "deleteSave", (Action<string>)DeleteSave, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("menu", "shareSave", (Action<string>)ShareSave, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("menu", "shareMap", (Action<string>)ShareMap, (IReader<string>)null));
		AddBinding((IBinding)new TriggerBinding("menu", "quicksave", (Action)SafeQuickSave));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("menu", "quickload", (Action<bool>)SafeQuickLoad, (IReader<bool>)null));
		AddBinding((IBinding)new TriggerBinding("menu", "startEditor", (Action)StartEditor));
		AddBinding((IBinding)new TriggerBinding("menu", "showPdxModsUI", (Action)ShowModsUI));
		AddBinding((IBinding)new TriggerBinding("menu", "exitToMainMenu", (Action)ExitToMainMenu));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("menu", "onSaveGameScreenVisibilityChanged", (Action<bool>)OnSaveGameScreenVisibilityChanged, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<bool, bool>("menu", "applyTutorialSettings", (Action<bool, bool>)ApplyTutorialSettings, (IReader<bool>)null, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("menu", "selectCloudTarget", (Action<string>)SelectCloudTarget, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int>("menu", "selectMapFilter", (Action<int>)OnSelectMapFilter, (IReader<int>)null));
		m_XPQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<XP>() });
		m_LastSelectedCloudTarget = SharedSettings.instance.userState.lastCloudTarget;
		m_ModsUI = new PdxModsUI();
	}

	protected override void OnWorldReady()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		GameManager.instance.userInterface.appBindings.UpdateOwnedPrerequisiteBinding();
		m_PrefabSystem.onContentAvailabilityChanged += OnContentAvailabilityChanged;
		AssetDatabase.global.onAssetDatabaseChanged.Subscribe((EventDelegate<AssetChangedEventArgs>)UpdateClouds, (Predicate<AssetChangedEventArgs>)delegate(AssetChangedEventArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			ChangeType change = ((AssetChangedEventArgs)(ref args)).change;
			return (int)change == 0 || (int)change == 1;
		}, AssetChangedEventArgs.Default);
		EventExtensions.Subscribe<MapMetadata>(AssetDatabase.global.onAssetDatabaseChanged, (EventDelegate<AssetChangedEventArgs>)UpdateMaps, AssetChangedEventArgs.Default);
		EventExtensions.Subscribe<SaveGameMetadata>(AssetDatabase.global.onAssetDatabaseChanged, (EventDelegate<AssetChangedEventArgs>)UpdateSaves, AssetChangedEventArgs.Default);
	}

	private void OnContentAvailabilityChanged(ContentPrefab contentPrefab)
	{
		GameManager.instance.userInterface.appBindings.UpdateOwnedPrerequisiteBinding();
		UpdateMaps();
		UpdateSaves();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		string currentModeName = m_GameModeSystem.currentModeName;
		m_CurrentGameModeBinding.Update((currentModeName == string.Empty) ? "NormalMode" : currentModeName);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ModsUI.Dispose();
		AssetDatabase.global.onAssetDatabaseChanged.Unsubscribe((EventDelegate<AssetChangedEventArgs>)UpdateClouds);
		AssetDatabase.global.onAssetDatabaseChanged.Unsubscribe((EventDelegate<AssetChangedEventArgs>)UpdateMaps);
		AssetDatabase.global.onAssetDatabaseChanged.Unsubscribe((EventDelegate<AssetChangedEventArgs>)UpdateSaves);
		m_PrefabSystem.onContentAvailabilityChanged -= OnContentAvailabilityChanged;
		base.OnDestroy();
	}

	private bool IsModsUIActive()
	{
		return m_ModsUI.isActive;
	}

	private void UpdateClouds(AssetChangedEventArgs args)
	{
		GameManager.instance.RunOnMainThread(delegate
		{
			m_AvailableCloudTargetsBinding.Update(MenuHelpers.GetAvailableCloudTargets());
			m_SelectedCloudTargetBinding.Update();
		});
	}

	private void UpdateMaps(AssetChangedEventArgs args)
	{
		GameManager.instance.RunOnMainThread(UpdateMaps);
	}

	private void UpdateMaps()
	{
		m_AvailableMapFilters.Update(GetAvailableMapFilters());
		if (!m_AvailableMapFilters.value.Contains(m_SelectedMapFilter.value))
		{
			m_SelectedMapFilter.Update((m_AvailableMapFilters.value.Count > 0) ? m_AvailableMapFilters.value.First() : (-1));
		}
		MenuHelpers.UpdateMeta<MapInfo>(m_MapsBinding, FilterMaps);
	}

	private void UpdateSaves()
	{
		MenuHelpers.UpdateMeta<SaveInfo>(m_SavesBinding);
		GameManager.instance.userInterface.appBindings.UpdateCanContinueBinding();
	}

	private void UpdateSaves(AssetChangedEventArgs args)
	{
		GameManager.instance.RunOnMainThread(UpdateSaves);
	}

	private void ApplyTutorialSettings(bool showTutorials, bool resetTutorials)
	{
		SharedSettings.instance.gameplay.showTutorials = showTutorials;
		if (resetTutorials)
		{
			SharedSettings.instance.userState.ResetTutorials();
		}
	}

	public void PreDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)((Context)(ref context)).purpose == 6)
		{
			m_ActiveScreenBinding.Update(0);
		}
	}

	private void OnSaveGameScreenVisibilityChanged(bool visible)
	{
		if (visible)
		{
			m_SavePreviewBinding.Update(string.Format("{0}{1}/{2}?width={3}&height={4}&op={5}&{6}#{7}", "screencapture://", ((Component)Camera.main).tag.ToLowerInvariant(), "SaveGamePanel", 680, 383, "Screenshot", m_PreviewSettings.ToUri(), s_PreviewId++));
		}
		else
		{
			m_SavePreviewBinding.Update((string)null);
		}
	}

	private bool IsModdingEnabled()
	{
		return !GameManager.instance.configuration.disableModding;
	}

	private bool IsPdxModsUIEnabled()
	{
		if (!GameManager.instance.configuration.disablePDXSDK)
		{
			return !GameManager.instance.configuration.disableModding;
		}
		return false;
	}

	private void ShowModsUI()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (PlatformManager.instance.hasUgcPrivilege)
		{
			if (PlatformExt.IsPlatformSet((Platform)24, Application.platform, false))
			{
				GameManager.instance.userInterface.paradoxBindings.OnPSModsUIOpened(m_ModsUI.Show);
				m_ModsUI.platform.onModsUIClosed -= OnPSModsUIClosed;
				m_ModsUI.platform.onModsUIClosed += OnPSModsUIClosed;
			}
			else
			{
				m_ModsUI.Show();
			}
		}
	}

	private async void OnPSModsUIClosed()
	{
		HashSet<Mod> hashSet = await m_ModsUI.platform.GetModsInActivePlayset();
		if (hashSet != null && hashSet.Count > 0)
		{
			GameManager.instance.userInterface.paradoxBindings.OnPSModsUIClosed((Action)null, (Action)m_ModsUI.platform.DeactivateActivePlayset, (Action)m_ModsUI.Show);
		}
	}

	private List<string> GetCreditFiles()
	{
		return new List<string> { "Media/Menu/Credits.md", "Media/Menu/Licenses.md" };
	}

	private List<ThemeInfo> GetThemes()
	{
		return new List<ThemeInfo>
		{
			new ThemeInfo
			{
				id = "European",
				icon = "Media/Game/Themes/European.svg"
			},
			new ThemeInfo
			{
				id = "North American",
				icon = "Media/Game/Themes/North American.svg"
			}
		};
	}

	private void SafeContinueGame()
	{
		TaskManager.instance.EnqueueTask("SaveLoadGame", (Func<Task>)ContinueGame, 1);
	}

	private async Task ContinueGame()
	{
		try
		{
			SaveGameMetadata lastSave = GameManager.instance.settings.userState.lastSaveGameMetadata;
			if ((AssetData)(object)lastSave != (IAssetData)null && lastSave.isValidSaveGame)
			{
				m_MapMetadataSystem.mapName = ((Metadata<SaveInfo>)lastSave).target.mapName;
				PlatformManager.instance.achievementsEnabled = !((Metadata<SaveInfo>)lastSave).target.isReadonly;
				await GameManager.instance.Load(GameMode.Game, (Purpose)2, (IAssetData)(object)lastSave);
				SaveInfo target = ((Metadata<SaveInfo>)lastSave).target;
				m_LastSaveNameBinding.Update(target.autoSave ? null : target.displayName);
				if (!target.autoSave)
				{
					m_LastSelectedCloudTarget = ((IDataSourceAccessor)((AssetData)target.metaData).database).dataSource.remoteStorageSourceName;
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	private void SafeNewGame(NewGameArgs args)
	{
		TaskManager.instance.EnqueueTask("SaveLoadGame", (Func<Task>)(() => NewGame(args)), 1);
	}

	private async Task NewGame(NewGameArgs args)
	{
		try
		{
			MapInfo mapInfo = m_MapsBinding.value.Find((MapInfo x) => x.id == args.mapId);
			m_MapMetadataSystem.mapName = mapInfo.displayName;
			m_CityConfigurationSystem.overrideLoadedOptions = true;
			m_CityConfigurationSystem.overrideThemeName = args.theme;
			m_GameModeSystem.overrideMode = args.gameMode;
			ApplyOptions(args.cityName, args.options);
			PlatformManager.instance.achievementsEnabled = true;
			m_TimeSystem.startingYear = ((mapInfo.startingYear != -1) ? mapInfo.startingYear : DateTime.Now.Year);
			await GameManager.instance.Load(GameMode.Game, (Purpose)1, (IAssetData)(object)mapInfo.metaData);
			m_LastSaveNameBinding.Update((string)null);
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	private void SafeLoadGame(LoadGameArgs args, bool dismiss)
	{
		TaskManager.instance.EnqueueTask("SaveLoadGame", (Func<Task>)(() => LoadGame(args, dismiss)), 1);
	}

	private async Task LoadGame(LoadGameArgs args, bool dismiss)
	{
		try
		{
			if (dismiss)
			{
				SharedSettings.instance.userInterface.AddDismissedConfirmation("LoadGame");
			}
			SaveInfo saveInfo = m_SavesBinding.value.Find((SaveInfo x) => x.id == args.saveId);
			m_MapMetadataSystem.mapName = saveInfo.mapName;
			m_CityConfigurationSystem.overrideLoadedOptions = true;
			m_CityConfigurationSystem.overrideThemeName = null;
			m_GameModeSystem.overrideMode = args.gameMode;
			ApplyOptions(args.cityName, args.options);
			PlatformManager.instance.achievementsEnabled = !saveInfo.isReadonly;
			await GameManager.instance.Load(GameMode.Game, (Purpose)2, (IAssetData)(object)saveInfo.metaData);
			m_LastSaveNameBinding.Update(saveInfo.autoSave ? null : saveInfo.displayName);
			if (!saveInfo.autoSave)
			{
				m_LastSelectedCloudTarget = ((IDataSourceAccessor)((AssetData)saveInfo.metaData).database).dataSource.remoteStorageSourceName;
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	private void ApplyOptions(string cityName, Dictionary<string, bool> options)
	{
		m_CityConfigurationSystem.overrideCityName = cityName;
		if (options != null)
		{
			UserState userState = SharedSettings.instance.userState;
			if (options.TryGetValue("leftHandTraffic", out var value))
			{
				bool leftHandTraffic = (m_CityConfigurationSystem.overrideLeftHandTraffic = value);
				userState.leftHandTraffic = leftHandTraffic;
			}
			if (options.TryGetValue("naturalDisasters", out var value2))
			{
				bool leftHandTraffic = (m_CityConfigurationSystem.overrideNaturalDisasters = value2);
				userState.naturalDisasters = leftHandTraffic;
			}
			if (options.TryGetValue("unlockAll", out var value3))
			{
				bool leftHandTraffic = (m_CityConfigurationSystem.overrideUnlockAll = value3);
				userState.unlockAll = leftHandTraffic;
			}
			if (options.TryGetValue("unlimitedMoney", out var value4))
			{
				bool leftHandTraffic = (m_CityConfigurationSystem.overrideUnlimitedMoney = value4);
				userState.unlimitedMoney = leftHandTraffic;
			}
			if (options.TryGetValue("unlockMapTiles", out var value5))
			{
				bool leftHandTraffic = (m_CityConfigurationSystem.overrideUnlockMapTiles = value5);
				userState.unlockMapTiles = leftHandTraffic;
			}
			userState.ApplyAndSave();
		}
	}

	public SaveInfo GetSaveInfo(bool autoSave)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CitySystem existingSystemManaged = ((ComponentSystemBase)this).World.GetExistingSystemManaged<CitySystem>();
		DateTime currentDateTime = ((ComponentSystemBase)this).World.GetExistingSystemManaged<TimeSystem>().GetCurrentDateTime();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Population componentData = ((EntityManager)(ref entityManager)).GetComponentData<Population>(existingSystemManaged.City);
		((ComponentSystemBase)m_MapMetadataSystem).Update();
		return new SaveInfo
		{
			theme = m_MapMetadataSystem.theme,
			cityName = m_CityConfigurationSystem.cityName,
			population = componentData.m_Population,
			money = existingSystemManaged.moneyAmount,
			xp = existingSystemManaged.XP,
			simulationDate = new SimulationDateTime(currentDateTime.Year, currentDateTime.DayOfYear - 1, currentDateTime.Hour, currentDateTime.Minute),
			options = new Dictionary<string, bool>
			{
				{ "leftHandTraffic", m_CityConfigurationSystem.leftHandTraffic },
				{ "naturalDisasters", m_CityConfigurationSystem.naturalDisasters },
				{ "unlockAll", m_CityConfigurationSystem.unlockAll },
				{ "unlimitedMoney", m_CityConfigurationSystem.unlimitedMoney },
				{ "unlockMapTiles", m_CityConfigurationSystem.unlockMapTiles }
			},
			mapName = m_MapMetadataSystem.mapName,
			autoSave = autoSave,
			modsEnabled = m_CityConfigurationSystem.usedMods.ToArray(),
			gameMode = m_GameModeSystem.currentModeName
		};
	}

	private void SafeSaveGame(string saveName)
	{
		TaskManager.instance.EnqueueTask("SaveLoadGame", (Func<Task>)(() => SaveGame(saveName)), 1);
	}

	private async Task SaveGame(string saveName)
	{
		_ = 1;
		try
		{
			Texture savePreview = UIManager.defaultUISystem.userImagesManager.GetUserImageTarget("SaveGamePanel", 680, 383, default(Rect));
			ILocalAssetDatabase targetDatabase = MenuHelpers.GetSanitizedCloudTarget(SharedSettings.instance.userState.lastCloudTarget).db;
			SaveInfo saveInfo = GetSaveInfo(autoSave: false);
			if (await HandlesOverwrite(targetDatabase, saveName))
			{
				m_GameScreenUISystem.SetScreen(GameScreenUISystem.GameScreen.PauseMenu);
				await GameManager.instance.Save(saveName, saveInfo, targetDatabase, savePreview);
				m_LastSaveNameBinding.Update(saveName);
				m_LastSelectedCloudTarget = ((IDataSourceAccessor)((AssetData)saveInfo.metaData).database).dataSource.remoteStorageSourceName;
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	private bool SaveExists(ILocalAssetDatabase database, string name, out PackageAsset asset)
	{
		return database.Exists<PackageAsset>(SaveHelpers.GetAssetDataPath<SaveGameMetadata>(database, name), ref asset);
	}

	private Task<bool> HandlesOverwrite(ILocalAssetDatabase database, string saveName)
	{
		if (SaveExists(database, saveName, out var _) && !SharedSettings.instance.userInterface.dismissedConfirmations.Contains("SaveGame"))
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			GameManager.instance.RegisterCancellationOnQuit(tcs, stateOnCancel: false);
			GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(new DismissibleConfirmationDialog("Common.DIALOG_TITLE[Warning]", "Common.DIALOG_MESSAGE[Overwrite]", "Common.DIALOG_ACTION[Yes]", "Common.DIALOG_ACTION[No]"), delegate(int msg, bool dismiss)
			{
				if (msg == 0 && dismiss)
				{
					SharedSettings.instance.userInterface.AddDismissedConfirmation("SaveGame");
				}
				tcs.SetResult(msg == 0);
			});
			return tcs.Task;
		}
		return Task.FromResult(result: true);
	}

	private void SafeQuickSave()
	{
		TaskManager.instance.EnqueueTask("SaveLoadGame", (Func<Task>)QuickSave, 1);
	}

	private async Task QuickSave()
	{
		_ = 1;
		try
		{
			string saveName = m_LastSaveNameBinding.value;
			if (string.IsNullOrEmpty(saveName))
			{
				saveName = m_CityConfigurationSystem.cityName;
			}
			if (string.IsNullOrEmpty(saveName))
			{
				saveName = "SaveGame";
			}
			ILocalAssetDatabase targetDatabase = MenuHelpers.GetSanitizedCloudTarget(m_LastSelectedCloudTarget).db;
			if (((IAssetDatabase)targetDatabase).name != null)
			{
				RenderTexture savePreview = ScreenCaptureHelper.CreateRenderTarget("SaveGamePanel", 680, 383, (GraphicsFormat)8);
				ScreenCaptureHelper.CaptureScreenshot(Camera.main, savePreview, m_PreviewSettings);
				SaveInfo saveInfo = GetSaveInfo(autoSave: false);
				if (await HandlesOverwrite(targetDatabase, saveName))
				{
					await GameManager.instance.Save(saveName, saveInfo, targetDatabase, (Texture)(object)savePreview);
				}
				CoreUtils.Destroy((Object)(object)savePreview);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	private void SafeQuickLoad(bool dismiss)
	{
		TaskManager.instance.EnqueueTask("SaveLoadGame", (Func<Task>)(() => QuickLoad(dismiss)), 1);
	}

	private async Task QuickLoad(bool dismiss)
	{
		try
		{
			if (dismiss)
			{
				SharedSettings.instance.userInterface.AddDismissedConfirmation("LoadGame");
			}
			if (MenuHelpers.hasPreviouslySavedGame)
			{
				await ContinueGame();
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	public void DeleteSave(string guid)
	{
		try
		{
			SaveHelpers.DeleteSaveGame(m_SavesBinding.value.Find((SaveInfo x) => x.id == guid).metaData);
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	public void ShareSave(string id)
	{
		foreach (SaveInfo item in m_SavesBinding.value)
		{
			if (item.id == id)
			{
				m_AssetUploadPanelUISystem.Show((AssetData)(object)item.metaData);
				break;
			}
		}
	}

	public void ShareMap(string id)
	{
		foreach (MapInfo item in m_MapsBinding.value)
		{
			if (item.id == id)
			{
				m_AssetUploadPanelUISystem.Show((AssetData)(object)item.metaData);
				break;
			}
		}
	}

	private async void StartEditor()
	{
		try
		{
			m_CityConfigurationSystem.overrideLoadedOptions = false;
			m_CityConfigurationSystem.overrideThemeName = null;
			await GameManager.instance.Load(GameMode.Editor, (Purpose)4).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	private async void ExitToMainMenu()
	{
		try
		{
			m_CityConfigurationSystem.overrideLoadedOptions = false;
			m_CityConfigurationSystem.overrideThemeName = null;
			await GameManager.instance.MainMenu();
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			COSystemBase.baseLog.Error(ex2);
		}
	}

	private void SelectCloudTarget(string cloudTarget)
	{
		SharedSettings.instance.userState.lastCloudTarget = cloudTarget;
		_ = ((IDataSourceAccessor)MenuHelpers.GetSanitizedCloudTarget(cloudTarget).db).dataSource.maxSupportedFileLength;
	}

	private SaveabilityStatus GetSaveabilityStatus()
	{
		int count = MenuHelpers.GetAvailableCloudTargets().Count;
		return new SaveabilityStatus
		{
			canSave = (count > 0),
			reasonHash = ((count > 0) ? null : "NoLocations")
		};
	}

	private GameOptions GetGameOptions()
	{
		return new GameOptions(m_CityConfigurationSystem);
	}

	private static bool IsDefaultAsset(IAssetData asset)
	{
		return asset.database is AssetDatabase<Game>;
	}

	private HashSet<int> GetAvailableMapFilters()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		HashSet<int> hashSet = new HashSet<int>(2);
		foreach (Metadata<MapInfo> asset in AssetDatabase.global.GetAssets<Metadata<MapInfo>>(default(SearchFilter<Metadata<MapInfo>>)))
		{
			hashSet.Add((!IsDefaultAsset((IAssetData)(object)asset)) ? 1 : 0);
		}
		return hashSet;
	}

	private void OnSelectMapFilter(int tab)
	{
		m_SelectedMapFilter.Update(tab);
		UpdateMaps();
	}

	private bool FilterMaps(Metadata<MapInfo> meta)
	{
		if (m_AvailableMapFilters.value.Count > 1)
		{
			if (m_SelectedMapFilter.value < 0)
			{
				return true;
			}
			bool flag = IsDefaultAsset((IAssetData)(object)meta);
			if (m_SelectedMapFilter.value == 0 && flag)
			{
				return GameManager.instance.ArePrerequisitesMet<MapInfo>(meta);
			}
			if (m_SelectedMapFilter.value == 1 && !flag)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	[Preserve]
	public MenuUISystem()
	{
	}
}
