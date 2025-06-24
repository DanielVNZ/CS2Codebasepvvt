using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Citizens;
using Game.Input;
using Game.Prefabs;
using Game.PSI;
using Game.Serialization;
using Game.Settings;
using Game.Tools;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class GamePanelUISystem : UISystemBase, IPreDeserialize
{
	private const string kGroup = "game";

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private SelectedInfoUISystem m_SelectedInfoUISystem;

	private ToolbarUISystem m_ToolbarUISystem;

	private PhotoModeUISystem m_PhotoModeUISystem;

	private EntityQuery m_TransportConfigQuery;

	private InputBarrier m_ToolBarrier;

	private ValueBinding<GamePanel> m_ActivePanelBinding;

	private Dictionary<string, GamePanel> m_defaultArgs;

	public Action<GamePanel> eventPanelOpened;

	public Action<GamePanel> eventPanelClosed;

	private Entity m_PreviousSelectedEntity;

	private InfoviewPrefab m_PreviousInfoview;

	[CanBeNull]
	public GamePanel activePanel => m_ActivePanelBinding.value;

	private bool NeedsClear
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			if (m_SelectedInfoUISystem.selectedEntity != Entity.Null)
			{
				return true;
			}
			if (!(activePanel is InfoviewMenu))
			{
				if (m_ToolSystem.activeTool == m_DefaultTool)
				{
					return m_ToolbarUISystem.hasActiveSelection;
				}
				return true;
			}
			return false;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_SelectedInfoUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		m_ToolbarUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolbarUISystem>();
		m_PhotoModeUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PhotoModeUISystem>();
		m_TransportConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UITransportConfigurationData>() });
		m_ToolBarrier = InputManager.instance.CreateMapBarrier("Tool", "GamePanelUISystem");
		AddBinding((IBinding)(object)(m_ActivePanelBinding = new ValueBinding<GamePanel>("game", "activePanel", (GamePanel)null, (IWriter<GamePanel>)(object)ValueWriters.Nullable<GamePanel>((IWriter<GamePanel>)(object)new ValueWriter<GamePanel>()), (EqualityComparer<GamePanel>)null)));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("game", "blockingPanelActive", (Func<bool>)(() => activePanel?.blocking ?? false), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("game", "activePanelPosition", (Func<int>)(() => (int)((activePanel != null) ? activePanel.position : GamePanel.LayoutPosition.Undefined)), (IWriter<int>)null, (EqualityComparer<int>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("game", "togglePanel", (Action<string>)TogglePanel, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("game", "showPanel", (Action<string>)ShowPanel, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("game", "closePanel", (Action<string>)ClosePanel, (IReader<string>)null));
		AddBinding((IBinding)new TriggerBinding("game", "closeActivePanel", (Action)CloseActivePanel));
		m_defaultArgs = new Dictionary<string, GamePanel>();
		InitializeDefaults();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (activePanel != null && (NeedsClear || !IsPanelAllowed(activePanel)))
		{
			CloseActivePanel();
		}
		m_ToolBarrier.blocked = activePanel?.blocking ?? false;
	}

	public void PreDeserialize(Context context)
	{
		if (m_ActivePanelBinding.value is PhotoModePanel)
		{
			m_PhotoModeUISystem.Activate(enabled: false);
		}
		m_ActivePanelBinding.Update((GamePanel)null);
	}

	public void SetDefaultArgs(GamePanel defaultArgs)
	{
		m_defaultArgs[defaultArgs.GetType().FullName] = defaultArgs;
	}

	public void TogglePanel([CanBeNull] string panelType)
	{
		GamePanel value = m_ActivePanelBinding.value;
		if (value != null && value.GetType().FullName == panelType)
		{
			m_ActivePanelBinding.Update((GamePanel)null);
			OnPanelChanged(value, null);
		}
		else
		{
			ShowPanel(panelType);
		}
	}

	public void ShowPanel(string panelType)
	{
		if (m_defaultArgs.TryGetValue(panelType, out var value))
		{
			ShowPanel(value);
		}
	}

	public void ShowPanel(GamePanel panel)
	{
		if (IsPanelAllowed(panel))
		{
			GamePanel value = m_ActivePanelBinding.value;
			m_ActivePanelBinding.Update(panel);
			OnPanelChanged(value, panel);
		}
	}

	public void ClosePanel(string panelType)
	{
		GamePanel value = m_ActivePanelBinding.value;
		if (value != null && value.GetType().FullName == panelType)
		{
			m_ActivePanelBinding.Update((GamePanel)null);
			OnPanelChanged(value, null);
		}
	}

	private void CloseActivePanel()
	{
		GamePanel value = m_ActivePanelBinding.value;
		if (value != null)
		{
			m_ActivePanelBinding.Update((GamePanel)null);
			OnPanelChanged(value, null);
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		InitializeDefaults();
	}

	private void InitializeDefaults()
	{
		SetDefaultArgs(new InfoviewMenu());
		SetDefaultArgs(new ProgressionPanel());
		AddBinding((IBinding)(object)new TriggerBinding<int>("game", "showProgressionPanel", (Action<int>)ShowPanel<ProgressionPanel>, (IReader<int>)null));
		SetDefaultArgs(new EconomyPanel());
		AddBinding((IBinding)(object)new TriggerBinding<int>("game", "showEconomyPanel", (Action<int>)ShowPanel<EconomyPanel>, (IReader<int>)null));
		SetDefaultArgs(new CityInfoPanel());
		AddBinding((IBinding)(object)new TriggerBinding<int>("game", "showCityInfoPanel", (Action<int>)ShowPanel<CityInfoPanel>, (IReader<int>)null));
		SetDefaultArgs(new StatisticsPanel());
		SetDefaultArgs(new TransportationOverviewPanel());
		AddBinding((IBinding)(object)new TriggerBinding<int>("game", "showTransportationOverviewPanel", (Action<int>)ShowPanel<TransportationOverviewPanel>, (IReader<int>)null));
		SetDefaultArgs(new ChirperPanel());
		SetDefaultArgs(new LifePathPanel());
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("game", "showLifePathDetail", (Action<Entity>)ShowPanel<LifePathPanel>, (IReader<Entity>)null));
		SetDefaultArgs(new JournalPanel());
		SetDefaultArgs(new RadioPanel());
		SetDefaultArgs(new PhotoModePanel());
		SetDefaultArgs(new CinematicCameraPanel());
		SetDefaultArgs(new NotificationsPanel());
	}

	public void ShowPanel<T>(int tab) where T : TabbedGamePanel, new()
	{
		ShowPanel(new T
		{
			selectedTab = tab
		});
	}

	public void ShowPanel<T>(Entity selectedEntity) where T : EntityGamePanel, new()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ShowPanel(new T
		{
			selectedEntity = selectedEntity
		});
	}

	private bool IsPanelAllowed(GamePanel panel)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (panel is RadioPanel)
		{
			return SharedSettings.instance.audio.radioActive;
		}
		if (panel is LifePathPanel lifePathPanel && lifePathPanel.selectedEntity != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Followed>(lifePathPanel.selectedEntity);
		}
		return true;
	}

	private void OnPanelChanged([CanBeNull] GamePanel previous, [CanBeNull] GamePanel next)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (previous != null && (next == null || next.GetType() != previous.GetType()))
		{
			eventPanelClosed?.Invoke(previous);
			OnPanelClosed(previous);
			if (next == null)
			{
				if (!(previous is InfoviewMenu))
				{
					m_ToolSystem.infoview = (((Object)(object)m_PreviousInfoview != (Object)null) ? m_PreviousInfoview : m_ToolSystem.infoview);
					m_PreviousInfoview = null;
				}
				if (m_SelectedInfoUISystem.selectedEntity == Entity.Null)
				{
					m_SelectedInfoUISystem.SetSelection(m_PreviousSelectedEntity);
					m_PreviousSelectedEntity = Entity.Null;
				}
			}
		}
		if (next != null && (previous == null || next.GetType() != previous.GetType()))
		{
			OnPanelOpened(next);
			eventPanelOpened?.Invoke(next);
		}
	}

	private void OnPanelOpened(GamePanel panel)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		m_PreviousInfoview = m_ToolSystem.activeInfoview;
		if (panel is PhotoModePanel)
		{
			m_PhotoModeUISystem.Activate(enabled: true);
		}
		if (!(panel is InfoviewMenu))
		{
			m_ToolSystem.activeTool = m_DefaultTool;
			m_ToolbarUISystem.ClearAssetSelection();
		}
		if (panel is TransportationOverviewPanel && TryGetTransportConfig(out var config))
		{
			m_ToolSystem.infoview = config.m_TransportInfoview;
		}
		Telemetry.PanelOpened(panel);
		if (panel.retainSelection)
		{
			m_PreviousSelectedEntity = m_SelectedInfoUISystem.selectedEntity;
		}
		m_SelectedInfoUISystem.SetSelection(Entity.Null);
	}

	private void OnPanelClosed(GamePanel panel)
	{
		if (panel is PhotoModePanel)
		{
			m_PhotoModeUISystem.Activate(enabled: false);
		}
		Telemetry.PanelClosed(panel);
		if (panel.retainProperties)
		{
			SetDefaultArgs(panel);
		}
	}

	private bool TryGetTransportConfig(out UITransportConfigurationPrefab config)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return m_PrefabSystem.TryGetSingletonPrefab<UITransportConfigurationPrefab>(m_TransportConfigQuery, out config);
	}

	[Preserve]
	public GamePanelUISystem()
	{
	}
}
