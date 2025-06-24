using System;
using System.Collections.Generic;
using System.Linq;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.PSI.Common;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Input;
using Game.Policies;
using Game.Prefabs;
using Game.Prefabs.Modes;
using Game.PSI.Internal;
using Game.SceneFlow;
using Game.Settings;
using Game.Simulation;
using Game.Tools;
using Game.Tutorials;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.PSI;

[Telemetry]
public static class Telemetry
{
	private struct HardwarePayload
	{
		public string os_version;

		public int ram;

		public int gfx_memory;

		public int cpucount;

		public string cpu_model;

		public string gpu_model;
	}

	private struct LanguagePayload
	{
		public string os_language;

		public string game_language;
	}

	private struct GraphicsSettingsPayload
	{
		public Helpers.json_displaymode display_mode;

		public string resolution;

		public string graphics_quality;
	}

	private struct AchievementPayload
	{
		public Guid playthrough_id;

		public string achievement_name;

		public int achievement_number;
	}

	private struct TutorialEventPayload
	{
		public Guid playthrough_id;

		public string advice_followed;
	}

	private struct MilestoneUnlockedPayload
	{
		public Guid playthrough_id;

		public int milestone_index;

		public int ingame_days;
	}

	private struct DevNodePurchasedPayload
	{
		public Guid playthrough_id;

		public string dev_node_name;

		public string node_type;

		public int tier_id;
	}

	private struct ControlInputPayload
	{
		public Guid playthrough_id;

		public string control_scheme;
	}

	private struct PanelClosedPayload
	{
		public Guid playthrough_id;

		public string panel_name;

		public double time_spent;
	}

	private struct CityStatsPayload
	{
		public Guid playthrough_id;

		public string map_id;

		public int n_buildings;

		public int population;

		public int happiness;

		public int ingame_days;

		public int map_tiles;

		public int resource_output;

		public int tagged_citizens;

		public int cash_balance;

		public int cash_income;
	}

	private struct ChirperPayload
	{
		public Guid playthrough_id;

		public string message_type;

		public uint likes;
	}

	private struct BuildingPlacedPayload
	{
		public Guid playthrough_id;

		public string map_id;

		public string building_id;

		public string type;

		public int building_level;

		public string coordinates;

		public string origin;
	}

	private struct PolicyPayload
	{
		public Guid playthrough_id;

		public string policy_id;

		public PolicyCategory policy_category;

		public ModifiedSystem.PolicyRange policy_range;
	}

	private struct InputIdleEndPayload
	{
		public Guid playthrough_id;

		public float simulation_speed_start;

		public float simulation_speed_end;

		public double duration;
	}

	public class GameplayData
	{
		private EntityQuery m_PopulationQuery;

		private EntityQuery m_BuildingsQuery;

		private EntityQuery m_OwnedTileQuery;

		private EntityQuery m_FollowedQuery;

		private readonly PrefabSystem m_PrefabSystem;

		private readonly TimeUISystem m_TimeSystem;

		private readonly SimulationSystem m_SimulationSystem;

		private readonly MapMetadataSystem m_MapMetadataSystem;

		private readonly CityStatisticsSystem m_CityStatisticsSystem;

		private readonly CityConfigurationSystem m_CityConfigurationSystem;

		private readonly TutorialSystem m_TutorialSystem;

		private readonly CitySystem m_CitySystem;

		private readonly CityServiceBudgetSystem m_CityServiceBudgetSystem;

		private readonly GameModeSystem m_GameModeSystem;

		public int buildingCount => ((EntityQuery)(ref m_BuildingsQuery)).CalculateEntityCount();

		public Population population
		{
			get
			{
				Population result = new Population
				{
					m_Population = 0,
					m_AverageHappiness = 50
				};
				if (!((EntityQuery)(ref m_PopulationQuery)).IsEmpty)
				{
					((EntityQuery)(ref m_PopulationQuery)).CompleteDependency();
					result = ((EntityQuery)(ref m_PopulationQuery)).GetSingleton<Population>();
				}
				return result;
			}
		}

		public int moneyAmount => m_CitySystem.moneyAmount;

		public int moneyDelta => m_CityServiceBudgetSystem.GetMoneyDelta();

		public int followedCitizens => ((EntityQuery)(ref m_FollowedQuery)).CalculateEntityCount();

		public int ownedMapTiles => ((EntityQuery)(ref m_OwnedTileQuery)).CalculateEntityCount();

		public string mapName => m_MapMetadataSystem.mapName;

		public float simulationSpeed
		{
			get
			{
				if (GameManager.instance.gameMode != GameMode.Game)
				{
					return 0f;
				}
				return m_SimulationSystem.selectedSpeed;
			}
		}

		public bool unlimitedMoney => m_CityConfigurationSystem.unlimitedMoney;

		public bool unlockAll => m_CityConfigurationSystem.unlockAll;

		public bool naturalDisasters => m_CityConfigurationSystem.naturalDisasters;

		public bool tutorialEnabled => m_TutorialSystem.tutorialEnabled;

		public string currentGameMode => m_GameModeSystem.currentModeName;

		private void InitializeStats(World world)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = world.EntityManager;
			m_PopulationQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<Game.City.City>(),
				ComponentType.ReadOnly<Population>()
			});
			entityManager = world.EntityManager;
			m_BuildingsQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[3]
			{
				ComponentType.ReadOnly<Building>(),
				ComponentType.Exclude<Temp>(),
				ComponentType.Exclude<Deleted>()
			});
			entityManager = world.EntityManager;
			m_OwnedTileQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<MapTile>(),
				ComponentType.Exclude<Native>()
			});
			entityManager = world.EntityManager;
			m_FollowedQuery = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[4]
			{
				ComponentType.ReadOnly<Citizen>(),
				ComponentType.ReadOnly<Followed>(),
				ComponentType.Exclude<Temp>(),
				ComponentType.Exclude<Deleted>()
			});
		}

		public int GetResourcesOutputCount()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			BufferLookup<CityStatistic> bufferLookup = ((SystemBase)m_PrefabSystem).GetBufferLookup<CityStatistic>(true);
			int num = 0;
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				num += m_CityStatisticsSystem.GetStatisticValue(bufferLookup, StatisticType.ProcessingCount, EconomyUtils.GetResourceIndex(iterator.resource));
			}
			return num;
		}

		public GameplayData(World world)
		{
			m_PrefabSystem = world.GetExistingSystemManaged<PrefabSystem>();
			m_TimeSystem = world.GetExistingSystemManaged<TimeUISystem>();
			m_SimulationSystem = world.GetExistingSystemManaged<SimulationSystem>();
			m_MapMetadataSystem = world.GetExistingSystemManaged<MapMetadataSystem>();
			m_CityStatisticsSystem = world.GetExistingSystemManaged<CityStatisticsSystem>();
			m_CityConfigurationSystem = world.GetExistingSystemManaged<CityConfigurationSystem>();
			m_TutorialSystem = world.GetExistingSystemManaged<TutorialSystem>();
			m_CitySystem = world.GetExistingSystemManaged<CitySystem>();
			m_CityServiceBudgetSystem = world.GetExistingSystemManaged<CityServiceBudgetSystem>();
			m_GameModeSystem = world.GetExistingSystemManaged<GameModeSystem>();
			InitializeStats(world);
		}

		public IEnumerable<IDlc> GetDLCsFromContent()
		{
			return m_PrefabSystem.GetAvailableContentPrefabs().Select(CreateDummyDLC);
			static IDlc CreateDummyDLC(ContentPrefab contentPrefab)
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Expected O, but got Unknown
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Expected O, but got Unknown
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Expected O, but got Unknown
				if (!contentPrefab.TryGet<DlcRequirement>(out var component))
				{
					if (!contentPrefab.TryGet<PdxLoginRequirement>(out var _))
					{
						return (IDlc)new CommonDlc(DlcId.Invalid, ((Object)contentPrefab).name, default(Version));
					}
					return (IDlc)new CommonDlc(DlcId.Virtual, ((Object)contentPrefab).name, default(Version));
				}
				return (IDlc)new CommonDlc(component.m_Dlc, ((Object)contentPrefab).name, default(Version));
			}
		}

		public int GetDay()
		{
			return m_TimeSystem.GetDay();
		}

		public T GetPrefab<T>(Entity entity) where T : PrefabBase
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return m_PrefabSystem.GetPrefab<T>(entity);
		}
	}

	private struct Session
	{
		public Guid guid;

		private DateTime m_InputIdleStart;

		private float m_StartSimulationSpeed;

		private DateTime m_TimeStarted;

		private Dictionary<string, DateTime> timeSpentInPanel;

		public bool active { get; private set; }

		public TimeSpan duration => DateTime.UtcNow - m_TimeStarted;

		public TimeSpan idleTime => DateTime.UtcNow - m_InputIdleStart;

		public float startSimulationSpeed => m_StartSimulationSpeed;

		public void Open(Guid guid)
		{
			if (guid == Guid.Empty)
			{
				guid = Guid.NewGuid();
			}
			this.guid = guid;
			timeSpentInPanel = new Dictionary<string, DateTime>();
			active = true;
			m_TimeStarted = DateTime.UtcNow;
			log.DebugFormat("Telemetry session {0} opened", (object)guid);
		}

		public void PanelOpened(string name)
		{
			timeSpentInPanel[name] = DateTime.UtcNow;
		}

		public bool PanelClosed(string name, out TimeSpan timeSpent)
		{
			timeSpent = TimeSpan.MinValue;
			if (timeSpentInPanel.TryGetValue(name, out var value))
			{
				timeSpent = DateTime.UtcNow - value;
			}
			return timeSpentInPanel.Remove(name);
		}

		public void Close()
		{
			log.DebugFormat("Telemetry session {0} closed", (object)guid);
			guid = default(Guid);
			active = false;
		}

		public void ReportInputIdle()
		{
			m_InputIdleStart = DateTime.UtcNow;
			m_StartSimulationSpeed = gameplayData.simulationSpeed;
		}
	}

	private struct OpenSessionPayload
	{
		public Guid playthrough_id;

		public string map_id;

		public Helpers.json_gameplay_mode gameplay_mode;

		public bool tutorial_messages;

		public bool unlock_all;

		public bool unlimited_money;

		public bool disasters;

		public string mode_settings;
	}

	private struct CloseSessionPayload
	{
		public Guid playthrough_id;

		public string map_id;

		public int ingame_days;

		public double time_passed;
	}

	private struct ModUsedPayload
	{
		public struct Mod
		{
			public string mod_name;

			public string mod_id;

			public string[] mod_tags;
		}

		public Mod[] mods;
	}

	private struct DlcPayload
	{
		public struct Dlc
		{
			public string dlc_name;

			public string dlc_platform_id;
		}

		public Dlc[] dlcs;
	}

	private static ILog log = LogManager.GetLogger("SceneFlow");

	private const string kHardwareEvent = "hardware";

	private const string kLanguageEvent = "language";

	private const string kGraphicsSettings = "graphics_settings";

	private const string kAchievementUnlocked = "achievement";

	private const string kTutorialEvent = "tutorial_event";

	private const string kMilestoneUnlocked = "milestone_unlocked";

	private const string kDevNodePurchased = "dev_node";

	private const string kControlInput = "control_input";

	private const string kPanelClosed = "panel_closed";

	private const string kCityStats = "city_stats";

	private const string kChirper = "chirper";

	private const string kBuildingPlaced = "building_placed";

	private const string kPolicy = "policy";

	private const string kInputIdleEnd = "idle_time_end";

	private const string kSessionOpen = "playsession_start";

	private const string kSessionClose = "playsession_close";

	private static Session s_Session = default(Session);

	private const string kModsUsed = "mod_used";

	private const string kDlc = "dlc";

	public static GameplayData gameplayData { get; set; }

	[TelemetryEvent("hardware", typeof(HardwarePayload))]
	private static void Hardware()
	{
		try
		{
			HardwarePayload hardwarePayload = new HardwarePayload
			{
				os_version = SystemInfo.operatingSystem,
				ram = Mathf.RoundToInt((float)SystemInfo.systemMemorySize / 1024f),
				gfx_memory = Mathf.RoundToInt((float)SystemInfo.graphicsMemorySize / 1024f),
				cpucount = SystemInfo.processorCount,
				cpu_model = SystemInfo.processorType,
				gpu_model = SystemInfo.graphicsDeviceName
			};
			PlatformManager.instance.SendTelemetry<HardwarePayload>("hardware", hardwarePayload);
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"hardware");
		}
	}

	[TelemetryEvent("language", typeof(LanguagePayload))]
	private static void Language()
	{
		try
		{
			LanguagePayload languagePayload = new LanguagePayload
			{
				os_language = Helpers.GetSystemLanguage(),
				game_language = GameManager.instance.localizationManager.activeLocaleId
			};
			PlatformManager.instance.SendTelemetry<LanguagePayload>("language", languagePayload);
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"language");
		}
	}

	[TelemetryEvent("graphics_settings", typeof(GraphicsSettingsPayload))]
	public static void GraphicsSettings()
	{
		try
		{
			GraphicsSettingsPayload graphicsSettingsPayload = new GraphicsSettingsPayload
			{
				display_mode = SharedSettings.instance.graphics.displayMode.ToTelemetry(),
				resolution = SharedSettings.instance.graphics.resolution.ToTelemetry(),
				graphics_quality = SharedSettings.instance.graphics.GetLevel().ToString().ToLowerInvariant()
			};
			PlatformManager.instance.SendTelemetry<GraphicsSettingsPayload>("graphics_settings", graphicsSettingsPayload);
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"graphics_settings");
		}
	}

	[TelemetryEvent("achievement", typeof(AchievementPayload))]
	public static void AchievementUnlocked(AchievementId id)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			IAchievement val = default(IAchievement);
			if (s_Session.active && PlatformManager.instance.GetAchievement(id, ref val) && val.achieved)
			{
				AchievementPayload achievementPayload = new AchievementPayload
				{
					playthrough_id = s_Session.guid,
					achievement_name = val.internalName,
					achievement_number = PlatformManager.instance.CountAchievements(true)
				};
				PlatformManager.instance.SendTelemetry<AchievementPayload>("achievement", achievementPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"achievement");
		}
	}

	[TelemetryEvent("tutorial_event", typeof(TutorialEventPayload))]
	public static void TutorialEvent(Entity tutorial)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (gameplayData != null && s_Session.active)
			{
				PrefabBase prefab = gameplayData.GetPrefab<PrefabBase>(tutorial);
				TutorialEventPayload tutorialEventPayload = new TutorialEventPayload
				{
					playthrough_id = s_Session.guid,
					advice_followed = (((Object)(object)prefab != (Object)null) ? ((Object)prefab).name : null)
				};
				PlatformManager.instance.SendTelemetry<TutorialEventPayload>("tutorial_event", tutorialEventPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"tutorial_event");
		}
	}

	[TelemetryEvent("milestone_unlocked", typeof(MilestoneUnlockedPayload))]
	public static void MilestoneUnlocked(int milestoneIndex)
	{
		try
		{
			if (gameplayData != null && s_Session.active)
			{
				MilestoneUnlockedPayload milestoneUnlockedPayload = new MilestoneUnlockedPayload
				{
					playthrough_id = s_Session.guid,
					milestone_index = milestoneIndex,
					ingame_days = gameplayData.GetDay()
				};
				PlatformManager.instance.SendTelemetry<MilestoneUnlockedPayload>("milestone_unlocked", milestoneUnlockedPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"milestone_unlocked");
		}
	}

	[TelemetryEvent("dev_node", typeof(DevNodePurchasedPayload))]
	public static void DevNodePurchased(DevTreeNodePrefab nodePrefab)
	{
		try
		{
			if (s_Session.active)
			{
				DevNodePurchasedPayload devNodePurchasedPayload = new DevNodePurchasedPayload
				{
					playthrough_id = s_Session.guid,
					dev_node_name = ((Object)nodePrefab).name,
					node_type = ((Object)nodePrefab.m_Service).name,
					tier_id = nodePrefab.m_HorizontalPosition
				};
				PlatformManager.instance.SendTelemetry<DevNodePurchasedPayload>("dev_node", devNodePurchasedPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"dev_node");
		}
	}

	[TelemetryEvent("control_input", typeof(ControlInputPayload))]
	public static void ControlSchemeChanged(InputManager.ControlScheme controlScheme)
	{
		try
		{
			if (s_Session.active)
			{
				ControlInputPayload controlInputPayload = new ControlInputPayload
				{
					playthrough_id = s_Session.guid,
					control_scheme = controlScheme.ToString()
				};
				PlatformManager.instance.SendTelemetry<ControlInputPayload>("control_input", controlInputPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"control_input");
		}
	}

	public static void PanelOpened(GamePanel panel)
	{
		if (s_Session.active)
		{
			s_Session.PanelOpened(panel.GetType().Name);
		}
	}

	[TelemetryEvent("panel_closed", typeof(PanelClosedPayload))]
	public static void PanelClosed(GamePanel panel)
	{
		try
		{
			if (s_Session.active && s_Session.PanelClosed(panel.GetType().Name, out var timeSpent))
			{
				string name = panel.GetType().Name;
				PanelClosedPayload panelClosedPayload = new PanelClosedPayload
				{
					playthrough_id = s_Session.guid,
					panel_name = name,
					time_spent = Math.Round(timeSpent.TotalSeconds, 2)
				};
				PlatformManager.instance.SendTelemetry<PanelClosedPayload>("panel_closed", panelClosedPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"panel_closed");
		}
	}

	[TelemetryEvent("city_stats", typeof(CityStatsPayload))]
	public static void CityStats()
	{
		try
		{
			if (gameplayData != null && s_Session.active)
			{
				Population population = gameplayData.population;
				CityStatsPayload cityStatsPayload = new CityStatsPayload
				{
					playthrough_id = s_Session.guid,
					map_id = gameplayData.mapName,
					n_buildings = gameplayData.buildingCount,
					population = population.m_Population,
					happiness = population.m_AverageHappiness,
					ingame_days = gameplayData.GetDay(),
					map_tiles = gameplayData.ownedMapTiles,
					resource_output = gameplayData.GetResourcesOutputCount(),
					cash_balance = gameplayData.moneyAmount,
					cash_income = gameplayData.moneyDelta,
					tagged_citizens = gameplayData.followedCitizens
				};
				PlatformManager.instance.SendTelemetry<CityStatsPayload>("city_stats", cityStatsPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"city_stats");
		}
	}

	[TelemetryEvent("chirper", typeof(ChirperPayload))]
	public static void Chirp(Entity chirpPrefab, uint likes)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (gameplayData != null && s_Session.active)
			{
				PrefabBase prefab = gameplayData.GetPrefab<PrefabBase>(chirpPrefab);
				ChirperPayload chirperPayload = new ChirperPayload
				{
					playthrough_id = s_Session.guid,
					message_type = ((Object)prefab).name,
					likes = likes
				};
				PlatformManager.instance.SendTelemetry<ChirperPayload>("chirper", chirperPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"chirper");
		}
	}

	[TelemetryEvent("building_placed", typeof(BuildingPlacedPayload))]
	public static void PlaceBuilding(Entity entity, PrefabBase building, float3 position)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (gameplayData != null && s_Session.active && (building.Has<BuildingPrefab>() || building.Has<BuildingExtensionPrefab>()))
			{
				string type = null;
				int building_level = 0;
				if (building.TryGet<UIObject>(out var component) && component.m_Group is UIAssetCategoryPrefab uIAssetCategoryPrefab)
				{
					type = ((Object)uIAssetCategoryPrefab).name;
				}
				string origin = "base_game";
				if (building.TryGet<ContentPrerequisite>(out var component2))
				{
					origin = ((Object)component2.m_ContentPrerequisite).name;
				}
				BuildingPlacedPayload buildingPlacedPayload = new BuildingPlacedPayload
				{
					playthrough_id = s_Session.guid,
					map_id = gameplayData.mapName,
					building_id = ((Object)building).name,
					type = type,
					building_level = building_level,
					coordinates = $"{position.x}|{position.z}",
					origin = origin
				};
				PlatformManager.instance.SendTelemetry<BuildingPlacedPayload>("building_placed", buildingPlacedPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"building_placed");
		}
	}

	[TelemetryEvent("policy", typeof(PolicyPayload))]
	public static void Policy(ModifiedSystem.PolicyEventInfo eventInfo)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (gameplayData != null && s_Session.active)
			{
				PolicyPrefab prefab = gameplayData.GetPrefab<PolicyPrefab>(eventInfo.m_Entity);
				if (prefab.m_Visibility != PolicyVisibility.HideFromPolicyList)
				{
					PolicyPayload policyPayload = new PolicyPayload
					{
						playthrough_id = s_Session.guid,
						policy_id = ((Object)prefab).name,
						policy_category = prefab.m_Category,
						policy_range = eventInfo.m_PolicyRange
					};
					PlatformManager.instance.SendTelemetry<PolicyPayload>("policy", policyPayload);
				}
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"policy");
		}
	}

	public static void InputIdleStart()
	{
		try
		{
			if (gameplayData != null && s_Session.active)
			{
				s_Session.ReportInputIdle();
			}
		}
		catch (Exception ex)
		{
			log.Warn(ex);
		}
	}

	[TelemetryEvent("idle_time_end", typeof(InputIdleEndPayload))]
	public static void InputIdleEnd()
	{
		try
		{
			if (gameplayData != null && s_Session.active && !GameManager.instance.isGameLoading)
			{
				InputIdleEndPayload inputIdleEndPayload = new InputIdleEndPayload
				{
					playthrough_id = s_Session.guid,
					simulation_speed_start = s_Session.startSimulationSpeed,
					simulation_speed_end = gameplayData.simulationSpeed,
					duration = Math.Round(s_Session.idleTime.TotalSeconds, 2)
				};
				PlatformManager.instance.SendTelemetry<InputIdleEndPayload>("idle_time_end", inputIdleEndPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"idle_time_end");
		}
	}

	public static void FireSessionStartEvents()
	{
		Hardware();
		Language();
		GraphicsSettings();
	}

	public static Guid GetCurrentSession()
	{
		return s_Session.guid;
	}

	[TelemetryEvent("playsession_start", typeof(OpenSessionPayload))]
	public static void OpenSession(Guid guid)
	{
		try
		{
			CloseSession();
			if (gameplayData != null && !s_Session.active)
			{
				s_Session.Open(guid);
				OpenSessionPayload openSessionPayload = new OpenSessionPayload
				{
					playthrough_id = s_Session.guid,
					map_id = gameplayData.mapName,
					gameplay_mode = GameManager.instance.gameMode.ToTelemetry(),
					tutorial_messages = gameplayData.tutorialEnabled,
					unlimited_money = gameplayData.unlimitedMoney,
					unlock_all = gameplayData.unlockAll,
					disasters = gameplayData.naturalDisasters,
					mode_settings = gameplayData.currentGameMode
				};
				PlatformManager.instance.SendTelemetry<OpenSessionPayload>("playsession_start", openSessionPayload);
				ModsUsed();
				DlcsInstalled(gameplayData);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"playsession_start");
		}
	}

	[TelemetryEvent("mod_used", typeof(ModUsedPayload))]
	private static void ModsUsed()
	{
		try
		{
			IDataSourceProvider obj = AssetDatabase<ParadoxMods>.instance?.dataSource;
			ParadoxModsDataSource val = (ParadoxModsDataSource)(object)((obj is ParadoxModsDataSource) ? obj : null);
			if (val != null)
			{
				ModUsedPayload modUsedPayload = new ModUsedPayload
				{
					mods = (from mod in val.GetActiveMods()
						select new ModUsedPayload.Mod
						{
							mod_name = mod.displayName,
							mod_id = mod.id.ToString(),
							mod_tags = mod.tags
						}).ToArray()
				};
				if (modUsedPayload.mods.Any())
				{
					PlatformManager.instance.SendTelemetry<ModUsedPayload>("mod_used", modUsedPayload);
				}
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"mod_used");
		}
	}

	[TelemetryEvent("dlc", typeof(DlcPayload))]
	private static void DlcsInstalled(GameplayData data)
	{
		try
		{
			DlcPayload dlcPayload = new DlcPayload
			{
				dlcs = (from dlc in PlatformManager.instance.EnumerateDLCs()
					where dlc.hasStoreBackend
					select new DlcPayload.Dlc
					{
						dlc_name = dlc.internalName,
						dlc_platform_id = dlc.backendId
					}).ToArray()
			};
			if (dlcPayload.dlcs.Any())
			{
				PlatformManager.instance.SendTelemetry<DlcPayload>("dlc", dlcPayload);
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"dlc");
		}
	}

	[TelemetryEvent("playsession_close", typeof(CloseSessionPayload))]
	public static void CloseSession()
	{
		try
		{
			if (gameplayData != null && s_Session.active)
			{
				CloseSessionPayload closeSessionPayload = new CloseSessionPayload
				{
					playthrough_id = s_Session.guid,
					map_id = gameplayData.mapName,
					ingame_days = gameplayData.GetDay(),
					time_passed = Math.Round(s_Session.duration.TotalHours, 2)
				};
				PlatformManager.instance.SendTelemetry<CloseSessionPayload>("playsession_close", closeSessionPayload);
				s_Session.Close();
			}
		}
		catch (Exception ex)
		{
			log.WarnFormat(ex, "{0} telemetry event payload generation failed", (object)"playsession_close");
		}
	}
}
