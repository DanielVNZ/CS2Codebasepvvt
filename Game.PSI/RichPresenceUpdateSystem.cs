using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.PSI.Common;
using Colossal.PSI.Discord;
using Colossal.PSI.Steamworks;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Rendering;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using Game.UI.InGame;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.PSI;

[CompilerGenerated]
public class RichPresenceUpdateSystem : GameSystemBase
{
	private const int kUpdateRate = 5;

	private const int kStateCycleInterval = 10;

	private DateTime m_StartTime;

	private DateTime m_LastRichUpdate;

	private DateTime m_LastCycleUpdate;

	private PhotoModeRenderSystem m_PhotoModeRenderSystem;

	private ToolSystem m_ToolSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private CitySystem m_CitySystem;

	private ClimateSystem m_ClimateSystem;

	private TimeUISystem m_TimeUISystem;

	private ClimateUISystem m_ClimateUISystem;

	private EntityQuery m_MilestoneLevelQuery;

	private int m_StateIndex;

	private DiscordRichPresence m_DiscordRichPresence;

	private string[] m_DiscordState;

	private SteamworksPlatform m_SteamworksPlatform;

	private string[] m_SteamState;

	private static readonly string[] kHappinessEmoji = new string[5] { "\ud83d\ude1e", "\ud83d\ude41", "\ud83d\ude10", "\ud83d\ude42", "\ud83d\ude04" };

	private static readonly string[] kHealthEmoji = new string[3] { "❤\ufe0f", "❤\ufe0f❤\ufe0f", "❤\ufe0f❤\ufe0f❤\ufe0f" };

	private static readonly string[] kAltHealthEmoji = new string[3] { "❤\ufe0f\u200d\ud83e\ude79", "❤\ufe0f", "\ud83d\udc96" };

	private static readonly string[] kAltHealth2Emoji = new string[3] { "\ud83e\udd2e", "\ud83e\udd12\ufe0f", "\ud83d\udc96" };

	private static readonly string[] kRomanNumbers = new string[20]
	{
		"I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X",
		"XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX"
	};

	private DiscordRichPresence discordRichPresence => m_DiscordRichPresence ?? (m_DiscordRichPresence = PlatformManager.instance.GetPSI<DiscordRichPresence>("Discord"));

	private SteamworksPlatform steamworksPlatform => m_SteamworksPlatform ?? (m_SteamworksPlatform = PlatformManager.instance.GetPSI<SteamworksPlatform>("Steamworks"));

	private static int GetHappinessIndex(float happiness)
	{
		if (happiness > 70f)
		{
			return 4;
		}
		if (happiness > 55f)
		{
			return 3;
		}
		if (happiness > 40f)
		{
			return 2;
		}
		if (happiness > 25f)
		{
			return 1;
		}
		return 0;
	}

	private static int GetHealthIndex(float health)
	{
		if (health > 75f)
		{
			return 2;
		}
		if (health > 35f)
		{
			return 1;
		}
		return 0;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_StartTime = DateTime.Now;
		if (GameManager.instance.gameMode.IsGame())
		{
			m_DiscordState = new string[5] { "#StatusInGame_Population", "#StatusInGame_Money", "#StatusInGame_Happiness", "#StatusInGame_Health", "#StatusInGame_Milestone" };
			m_SteamState = new string[5] { "Population", "Money", "Happiness", "Health", "Milestone" };
		}
		else
		{
			GameManager.instance.gameMode.IsEditor();
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_ClimateUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateUISystem>();
		m_TimeUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeUISystem>();
		m_PhotoModeRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PhotoModeRenderSystem>();
		m_MilestoneLevelQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MilestoneLevel>() });
		RegisterKeys();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		try
		{
			DateTime now = DateTime.Now;
			if ((now - m_LastRichUpdate).TotalSeconds < 5.0)
			{
				return;
			}
			GameMode gameMode = GameManager.instance.gameMode;
			if (gameMode.IsGameOrEditor())
			{
				if ((now - m_LastCycleUpdate).TotalSeconds > 10.0)
				{
					m_StateIndex++;
					m_LastCycleUpdate = now;
				}
				if (gameMode.IsGame())
				{
					UpdateGameRichPresence();
				}
				else if (gameMode.IsEditor())
				{
					UpdateEditorRichPresence();
				}
			}
			m_LastRichUpdate = now;
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.Warn(ex);
		}
	}

	private int GetAverageHealth()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
		int result = 0;
		EntityManager entityManager = defaultGameObjectInjectionWorld.EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Population>(m_CitySystem.City))
		{
			entityManager = defaultGameObjectInjectionWorld.EntityManager;
			result = ((EntityManager)(ref entityManager)).GetComponentData<Population>(m_CitySystem.City).m_AverageHealth;
		}
		return result;
	}

	private string GetMilestoneNumber(int i)
	{
		if (i == 0)
		{
			return null;
		}
		return kRomanNumbers[i - 1];
	}

	private string GetMilestoneKey(int i)
	{
		return $"milestone{i}";
	}

	private int GetAchievedMilestone()
	{
		if (((EntityQuery)(ref m_MilestoneLevelQuery)).IsEmptyIgnoreFilter)
		{
			return 0;
		}
		return ((EntityQuery)(ref m_MilestoneLevelQuery)).GetSingleton<MilestoneLevel>().m_AchievedMilestone;
	}

	private int GetAverageHappiness()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
		int result = 0;
		EntityManager entityManager = defaultGameObjectInjectionWorld.EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Population>(m_CitySystem.City))
		{
			entityManager = defaultGameObjectInjectionWorld.EntityManager;
			result = ((EntityManager)(ref entityManager)).GetComponentData<Population>(m_CitySystem.City).m_AverageHappiness;
		}
		return result;
	}

	private int GetPopulationCount()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
		int result = 0;
		EntityManager entityManager = defaultGameObjectInjectionWorld.EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Population>(m_CitySystem.City))
		{
			entityManager = defaultGameObjectInjectionWorld.EntityManager;
			result = ((EntityManager)(ref entityManager)).GetComponentData<Population>(m_CitySystem.City).m_Population;
		}
		return result;
	}

	private string GetWeatherIconKey()
	{
		string text = m_ClimateSystem.classification.ToString().ToLowerInvariant();
		if ((float)m_ClimateSystem.precipitation > 0.3f)
		{
			if (m_ClimateSystem.isRaining)
			{
				text = "rain";
			}
			else if (m_ClimateSystem.isSnowing)
			{
				text = "snow";
			}
		}
		if (m_ClimateSystem.classification == ClimateSystem.WeatherClassification.Stormy && (float)m_ClimateSystem.precipitation > 0.9f)
		{
			text = ((!m_ClimateSystem.isRaining) ? "hail" : "stormy");
		}
		string text2 = GetLightingState().ToString().ToLowerInvariant();
		return text + text2;
	}

	private LightingSystem.State GetLightingState()
	{
		LightingSystem.State lightingState = m_TimeUISystem.GetLightingState();
		switch (lightingState)
		{
		case LightingSystem.State.Sunset:
		case LightingSystem.State.Dusk:
			return LightingSystem.State.Night;
		case LightingSystem.State.Dawn:
		case LightingSystem.State.Sunrise:
			return LightingSystem.State.Day;
		default:
			return lightingState;
		}
	}

	private void RegisterKeys()
	{
		PlatformManager instance = PlatformManager.instance;
		instance.RegisterRichPresenceKey("#StatusInGame_Building", (Func<string>)(() => "Building " + m_CityConfigurationSystem.cityName));
		instance.RegisterRichPresenceKey("#StatusInGame_Bulldozing", (Func<string>)(() => "Bulldozing " + m_CityConfigurationSystem.cityName));
		instance.RegisterRichPresenceKey("#StatusInGame_Inspecting", (Func<string>)(() => "Inspecting " + m_CityConfigurationSystem.cityName));
		instance.RegisterRichPresenceKey("#StatusInGame_CapturingMemories", (Func<string>)(() => "Capturing memories in " + m_CityConfigurationSystem.cityName));
		instance.RegisterRichPresenceKey("#StatusInGame_Population", (Func<string>)(() => $"Population: {GetPopulationCount()}"));
		instance.RegisterRichPresenceKey("#StatusInGame_Money", (Func<string>)(() => "Money: " + m_CitySystem.moneyAmount.ToString(CultureInfo.InvariantCulture) + "¢"));
		instance.RegisterRichPresenceKey("#StatusInGame_Happiness", (Func<string>)(() => "Happiness: " + kHappinessEmoji[GetHappinessIndex(GetAverageHappiness())]));
		instance.RegisterRichPresenceKey("#StatusInGame_Health", (Func<string>)(() => "Health: " + kHealthEmoji[GetHealthIndex(GetAverageHealth())]));
		instance.RegisterRichPresenceKey("#StatusInGame_Milestone", (Func<string>)(() => "Milestone " + GetMilestoneNumber(GetAchievedMilestone())));
	}

	private string GetActionKey()
	{
		if (((ComponentSystemBase)m_PhotoModeRenderSystem).Enabled)
		{
			return "#StatusInGame_CapturingMemories";
		}
		if ((Object)(object)m_ToolSystem.activeInfoview != (Object)null)
		{
			return "#StatusInGame_Inspecting";
		}
		if (m_ToolSystem.activeTool is BulldozeToolSystem)
		{
			return "#StatusInGame_Bulldozing";
		}
		return "#StatusInGame_Building";
	}

	private void UpdateEditorRichPresence()
	{
		discordRichPresence.SetRichPresence("In Editor", "Authoring UGC...", m_StartTime, "editor", "In-Game Editor", (string)null, (string)null);
	}

	private void UpdateGameRichPresence()
	{
		int achievedMilestone = GetAchievedMilestone();
		string milestoneNumber = GetMilestoneNumber(achievedMilestone);
		steamworksPlatform.SetRichPresence("cityname", m_CityConfigurationSystem.cityName);
		if (m_SteamState != null)
		{
			steamworksPlatform.SetRichPresence("stat", m_SteamState[m_StateIndex % ((achievedMilestone == 0) ? (m_SteamState.Length - 1) : m_SteamState.Length)]);
		}
		steamworksPlatform.SetRichPresence("population", GetPopulationCount().ToString());
		steamworksPlatform.SetRichPresence("money", m_CitySystem.moneyAmount.ToString(CultureInfo.InvariantCulture));
		steamworksPlatform.SetRichPresence("health", kHealthEmoji[GetHealthIndex(GetAverageHealth())]);
		steamworksPlatform.SetRichPresence("happiness", kHappinessEmoji[GetHappinessIndex(GetAverageHappiness())]);
		steamworksPlatform.SetRichPresence("milestone", milestoneNumber);
		steamworksPlatform.SetRichPresence(GetActionKey());
		if (m_DiscordState != null)
		{
			discordRichPresence.SetRichPresence(GetActionKey(), m_DiscordState[m_StateIndex % ((achievedMilestone == 0) ? (m_DiscordState.Length - 1) : m_DiscordState.Length)], m_StartTime, GetMilestoneKey(achievedMilestone), (milestoneNumber != null) ? ("Milestone " + milestoneNumber) : null, GetWeatherIconKey(), $"{math.round((float)m_ClimateSystem.temperature)}°C");
		}
	}

	[Preserve]
	public RichPresenceUpdateSystem()
	{
	}
}
