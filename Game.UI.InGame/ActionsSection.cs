using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Audio;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Policies;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.Triggers;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ActionsSection : InfoSectionBase
{
	private ToolSystem m_ToolSystem;

	private AreaToolSystem m_AreaToolSystem;

	private DefaultToolSystem m_DefaultToolSystem;

	private ObjectToolSystem m_ObjectToolSystem;

	private UIInitializeSystem m_UIInitializeSystem;

	private PoliciesUISystem m_PoliciesUISystem;

	private LifePathEventSystem m_LifePathEventSystem;

	private GamePanelUISystem m_GamePanelUISystem;

	private TrafficRoutesSystem m_TrafficRoutesSystem;

	private AudioManager m_AudioManager;

	private EntityQuery m_SoundQuery;

	private EntityQuery m_RouteConfigQuery;

	private PolicyPrefab m_RouteOutOfServicePolicy;

	private PolicyPrefab m_BuildingOutOfServicePolicy;

	private PolicyPrefab m_EmptyingPolicy;

	private AreaPrefab m_LotPrefab;

	private bool m_EditingLot;

	private Color32[] m_TrafficRouteColors;

	private ValueBinding<bool> m_MovingBinding;

	private ValueBinding<bool> m_EditingLotBinding;

	private ValueBinding<bool> m_TrafficRoutesVisibleBinding;

	private ValueBinding<Color32[]> m_TrafficRouteColorsBinding;

	private RawValueBinding m_MoveableObjectName;

	protected override string group => "ActionsSection";

	public bool editingLot => m_EditingLot;

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForOutsideConnections => true;

	protected override bool displayForUnderConstruction => true;

	protected override bool displayForUpgrades => true;

	private bool focusable { get; set; }

	private bool focusing { get; set; }

	private bool following { get; set; }

	private bool followable { get; set; }

	private bool moveable { get; set; }

	private bool deletable { get; set; }

	private bool disabled { get; set; }

	private bool disableable { get; set; }

	private bool hasTutorial { get; set; }

	private bool emptying { get; set; }

	private bool emptiable { get; set; }

	private bool hasLotTool { get; set; }

	private bool hasTrafficRoutes { get; set; }

	protected override void Reset()
	{
		focusable = false;
		focusing = false;
		following = false;
		followable = false;
		moveable = false;
		deletable = false;
		disabled = false;
		disableable = false;
		hasTutorial = false;
		emptying = false;
		emptiable = false;
		hasLotTool = false;
		hasTrafficRoutes = false;
		m_LotPrefab = null;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Expected O, but got Unknown
		//IL_028c: Expected O, but got Unknown
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_AreaToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaToolSystem>();
		m_DefaultToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_UIInitializeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UIInitializeSystem>();
		m_PoliciesUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliciesUISystem>();
		m_LifePathEventSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LifePathEventSystem>();
		m_GamePanelUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GamePanelUISystem>();
		m_TrafficRoutesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TrafficRoutesSystem>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_RouteConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RouteConfigurationData>() });
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Combine(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
		AddBinding((IBinding)new TriggerBinding(group, "focus", (Action)OnFocus));
		AddBinding((IBinding)new TriggerBinding(group, "toggleMove", (Action)OnToggleMove));
		AddBinding((IBinding)new TriggerBinding(group, "follow", (Action)OnFollow));
		AddBinding((IBinding)new TriggerBinding(group, "delete", (Action)OnDelete));
		AddBinding((IBinding)new TriggerBinding(group, "toggle", (Action)OnToggle));
		AddBinding((IBinding)new TriggerBinding(group, "toggleEmptying", (Action)OnToggleEmptying));
		AddBinding((IBinding)new TriggerBinding(group, "toggleLotTool", (Action)OnToggleLotTool));
		AddBinding((IBinding)new TriggerBinding(group, "toggleTrafficRoutes", (Action)OnToggleTrafficRoutes));
		AddBinding((IBinding)(object)(m_MovingBinding = new ValueBinding<bool>(group, "moving", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_EditingLotBinding = new ValueBinding<bool>(group, "editingLot", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		RawValueBinding val = new RawValueBinding(group, "moveableObjectName", (Action<IJsonWriter>)BindObjectName);
		RawValueBinding binding = val;
		m_MoveableObjectName = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_TrafficRouteColorsBinding = new ValueBinding<Color32[]>(group, "trafficRouteColors", m_TrafficRouteColors, (IWriter<Color32[]>)(object)new ArrayWriter<Color32>((IWriter<Color32>)null, false), (EqualityComparer<Color32[]>)null)));
		AddBinding((IBinding)(object)(m_TrafficRoutesVisibleBinding = new ValueBinding<bool>(group, "trafficRoutesVisible", m_TrafficRoutesSystem.routesVisible, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
	}

	private void OnToggleTrafficRoutes()
	{
		m_TrafficRoutesSystem.routesVisible = !m_TrafficRoutesSystem.routesVisible;
		m_InfoUISystem.SetDirty();
	}

	private void BindObjectName(IJsonWriter binder)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.activeTool == m_ObjectToolSystem && m_ObjectToolSystem.mode == ObjectToolSystem.Mode.Move)
		{
			m_NameSystem.BindName(binder, m_InfoUISystem.selectedEntity);
		}
		else
		{
			binder.WriteNull();
		}
	}

	private void OnToolChanged(ToolBaseSystem tool)
	{
		if (tool != m_AreaToolSystem)
		{
			m_EditingLot = false;
		}
		m_MoveableObjectName.Update();
		m_MovingBinding.Update(tool == m_ObjectToolSystem && m_ObjectToolSystem.mode == ObjectToolSystem.Mode.Move);
		m_EditingLotBinding.Update(tool == m_AreaToolSystem && hasLotTool && m_EditingLot);
	}

	private void OnToggleLotTool()
	{
		if (m_EditingLot)
		{
			m_ToolSystem.activeTool = m_DefaultToolSystem;
		}
		else if ((Object)(object)m_LotPrefab != (Object)null)
		{
			m_EditingLot = true;
			m_AreaToolSystem.prefab = m_LotPrefab;
			m_ToolSystem.activeTool = m_AreaToolSystem;
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		foreach (PolicyPrefab policy in m_UIInitializeSystem.policies)
		{
			switch (((Object)policy).name)
			{
			case "Route Out of Service":
				m_RouteOutOfServicePolicy = policy;
				break;
			case "Out of Service":
				m_BuildingOutOfServicePolicy = policy;
				break;
			case "Empty":
				m_EmptyingPolicy = policy;
				break;
			}
		}
	}

	private void OnToggle()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(selectedEntity))
				{
					m_PoliciesUISystem.SetSelectedInfoPolicy(m_PrefabSystem.GetEntity(m_RouteOutOfServicePolicy), !disabled);
					return;
				}
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Policy>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<CityServiceUpkeep>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Efficiency>(selectedEntity))
					{
						goto IL_00db;
					}
				}
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(selectedEntity))
		{
			return;
		}
		goto IL_00db;
		IL_00db:
		m_PoliciesUISystem.SetSelectedInfoPolicy(m_PrefabSystem.GetEntity(m_BuildingOutOfServicePolicy), !disabled);
	}

	private void OnToggleEmptying()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_PoliciesUISystem.SetSelectedInfoPolicy(m_PrefabSystem.GetEntity(m_EmptyingPolicy), !emptying);
	}

	private void OnDelete()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_BulldozeSound);
			}
			else
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_DeletetEntitySound);
			}
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			((EntityCommandBuffer)(ref val)).AddComponent<Deleted>(selectedEntity);
		}
	}

	private void OnFocus()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		m_InfoUISystem.Focus((!focusing) ? selectedEntity : Entity.Null);
	}

	private void OnToggleMove()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (moveable)
		{
			if (m_ToolSystem.activeTool == m_ObjectToolSystem)
			{
				m_ToolSystem.activeTool = m_DefaultToolSystem;
				return;
			}
			m_ObjectToolSystem.StartMoving(selectedEntity);
			m_ToolSystem.activeTool = m_ObjectToolSystem;
		}
	}

	private void OnFollow()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Followed>(selectedEntity))
		{
			m_LifePathEventSystem.FollowCitizen(selectedEntity);
			m_GamePanelUISystem.ShowPanel<LifePathPanel>(selectedEntity);
		}
		else
		{
			m_LifePathEventSystem.UnfollowCitizen(selectedEntity);
		}
		m_InfoUISystem.SetDirty();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.visible = selectedEntity != Entity.Null;
	}

	protected override void OnProcess()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		int num;
		if (!((EntityManager)(ref entityManager)).HasComponent<BuildingExtensionData>(selectedPrefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Household>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				num = (((EntityManager)(ref entityManager)).HasComponent<PropertyRenter>(selectedEntity) ? 1 : 0);
			}
			else
			{
				num = 1;
			}
		}
		else
		{
			num = 0;
		}
		focusable = (byte)num != 0;
		focusing = (Object)(object)SelectedInfoUISystem.s_CameraController != (Object)null && SelectedInfoUISystem.s_CameraController.controllerEnabled && SelectedInfoUISystem.s_CameraController.followedEntity == selectedEntity;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int num2;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Static>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Native>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Owner>(selectedEntity))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							num2 = ((!((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(selectedEntity)) ? 1 : 0);
						}
						else
						{
							num2 = 0;
						}
					}
					else
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.WaterPowered>(selectedEntity))
						{
							goto IL_0196;
						}
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Owner>(selectedEntity))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(selectedEntity))
							{
								goto IL_0196;
							}
						}
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(selectedPrefab))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							num2 = (((EntityManager)(ref entityManager)).HasComponent<SignatureBuildingData>(selectedPrefab) ? 1 : 0);
						}
						else
						{
							num2 = 1;
						}
					}
					goto IL_019a;
				}
			}
		}
		num2 = 0;
		goto IL_019a;
		IL_0730:
		int num3;
		hasTrafficRoutes = (byte)num3 != 0;
		m_TrafficRoutesVisibleBinding.Update(m_TrafficRoutesSystem.routesVisible);
		return;
		IL_0236:
		int num4;
		deletable = (byte)num4 != 0;
		Building building = default(Building);
		Extension extension = default(Extension);
		Route route = default(Route);
		disabled = (EntitiesExtensions.TryGetComponent<Building>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref building) && BuildingUtils.CheckOption(building, BuildingOption.Inactive)) || (EntitiesExtensions.TryGetComponent<Extension>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref extension) && (extension.m_Flags & ExtensionFlags.Disabled) != ExtensionFlags.None) || (EntitiesExtensions.TryGetComponent<Route>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref route) && RouteUtils.CheckOption(route, RouteOption.Inactive));
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(selectedEntity))
		{
			disableable = true;
			base.tooltipKeys.Add("Building");
		}
		else
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Policy>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<CityServiceUpkeep>(selectedEntity))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Efficiency>(selectedEntity))
						{
							disableable = true;
							base.tooltipKeys.Add("Building");
						}
					}
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(selectedEntity))
						{
							disableable = true;
						}
					}
				}
			}
		}
		emptying = EntitiesExtensions.TryGetComponent<Building>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref building) && BuildingUtils.CheckOption(building, BuildingOption.Empty);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int num5;
		if (((EntityManager)(ref entityManager)).HasComponent<Policy>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			GarbageFacilityData garbageFacilityData = default(GarbageFacilityData);
			if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity) && EntitiesExtensions.TryGetComponent<GarbageFacilityData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref garbageFacilityData))
			{
				num5 = (garbageFacilityData.m_LongTermStorage ? 1 : 0);
				goto IL_0407;
			}
		}
		num5 = 0;
		goto IL_0407;
		IL_0407:
		emptiable = (byte)num5 != 0;
		DynamicBuffer<Game.Prefabs.SubArea> val = default(DynamicBuffer<Game.Prefabs.SubArea>);
		if (EntitiesExtensions.TryGetBuffer<Game.Prefabs.SubArea>(((ComponentSystemBase)this).EntityManager, selectedPrefab, true, ref val) && val.Length > 0)
		{
			for (int i = 0; i < val.Length; i++)
			{
				AreaPrefab prefab = m_PrefabSystem.GetPrefab<AreaPrefab>(val[i].m_Prefab);
				if ((Object)(object)prefab != (Object)null && prefab.Has<LotPrefab>())
				{
					m_LotPrefab = prefab;
					hasLotTool = ((LotPrefab)prefab).m_AllowEditing;
					break;
				}
			}
		}
		if (!hasLotTool)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Attached attached = default(Attached);
			DynamicBuffer<Game.Areas.SubArea> val2 = default(DynamicBuffer<Game.Areas.SubArea>);
			if (((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(selectedPrefab) && EntitiesExtensions.TryGetComponent<Attached>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref attached) && EntitiesExtensions.TryGetBuffer<Game.Areas.SubArea>(((ComponentSystemBase)this).EntityManager, attached.m_Parent, true, ref val2) && val2.Length > 0)
			{
				PrefabRef prefabRef = default(PrefabRef);
				for (int j = 0; j < val2.Length; j++)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Areas.Lot>(val2[j].m_Area) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val2[j].m_Area, ref prefabRef))
					{
						AreaPrefab prefab2 = m_PrefabSystem.GetPrefab<AreaPrefab>(prefabRef.m_Prefab);
						if ((Object)(object)prefab2 != (Object)null)
						{
							m_LotPrefab = prefab2;
							hasLotTool = true;
							break;
						}
					}
				}
			}
		}
		if (m_TrafficRouteColors == null)
		{
			RouteConfigurationData singleton = ((EntityQuery)(ref m_RouteConfigQuery)).GetSingleton<RouteConfigurationData>();
			m_TrafficRouteColors = (Color32[])(object)new Color32[5]
			{
				m_PrefabSystem.GetPrefab<LivePathPrefab>(singleton.m_CarPathVisualization).color,
				m_PrefabSystem.GetPrefab<LivePathPrefab>(singleton.m_WatercraftPathVisualization).color,
				m_PrefabSystem.GetPrefab<LivePathPrefab>(singleton.m_AircraftPathVisualization).color,
				m_PrefabSystem.GetPrefab<LivePathPrefab>(singleton.m_TrainPathVisualization).color,
				m_PrefabSystem.GetPrefab<LivePathPrefab>(singleton.m_HumanPathVisualization).color
			};
			m_TrafficRouteColorsBinding.Update(m_TrafficRouteColors);
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Aggregate>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Game.Net.Node>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Edge>(selectedEntity))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Game.Routes.TransportStop>(selectedEntity))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(selectedEntity))
							{
								entityManager = ((ComponentSystemBase)this).EntityManager;
								if (!((EntityManager)(ref entityManager)).HasComponent<Human>(selectedEntity))
								{
									entityManager = ((ComponentSystemBase)this).EntityManager;
									if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(selectedEntity))
									{
										entityManager = ((ComponentSystemBase)this).EntityManager;
										if (!((EntityManager)(ref entityManager)).HasComponent<Citizen>(selectedEntity))
										{
											entityManager = ((ComponentSystemBase)this).EntityManager;
											num3 = (((EntityManager)(ref entityManager)).HasComponent<Household>(selectedEntity) ? 1 : 0);
											goto IL_0730;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		num3 = 1;
		goto IL_0730;
		IL_0196:
		num2 = 0;
		goto IL_019a;
		IL_019a:
		moveable = (byte)num2 != 0;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		followable = ((EntityManager)(ref entityManager)).HasComponent<Citizen>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		following = ((EntityManager)(ref entityManager)).HasComponent<Followed>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					num4 = (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity) ? 1 : 0);
				}
				else
				{
					num4 = 0;
				}
				goto IL_0236;
			}
		}
		num4 = 1;
		goto IL_0236;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("focusable");
		writer.Write(focusable);
		writer.PropertyName("focusing");
		writer.Write(focusing);
		writer.PropertyName("following");
		writer.Write(following);
		writer.PropertyName("followable");
		writer.Write(followable);
		writer.PropertyName("moveable");
		writer.Write(moveable);
		writer.PropertyName("deletable");
		writer.Write(deletable);
		writer.PropertyName("disabled");
		writer.Write(disabled);
		writer.PropertyName("disableable");
		writer.Write(disableable);
		writer.PropertyName("emptying");
		writer.Write(emptying);
		writer.PropertyName("emptiable");
		writer.Write(emptiable);
		writer.PropertyName("hasLotTool");
		writer.Write(hasLotTool);
		writer.PropertyName("hasTrafficRoutes");
		writer.Write(hasTrafficRoutes);
	}

	[Preserve]
	public ActionsSection()
	{
	}
}
