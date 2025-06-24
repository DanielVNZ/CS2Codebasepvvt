using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class TransportationOverviewUISystem : UISystemBase
{
	private const string kGroup = "transportationOverview";

	private NameSystem m_NameSystem;

	private UnlockSystem m_UnlockSystem;

	private PrefabSystem m_PrefabSystem;

	private PrefabUISystem m_PrefabUISystem;

	private PoliciesUISystem m_PoliciesUISystem;

	private SelectedInfoUISystem m_SelectedInfoUISystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private Entity m_OutOfServicePolicy;

	private Entity m_DayRoutePolicy;

	private Entity m_NightRoutePolicy;

	private EntityQuery m_ConfigQuery;

	private EntityQuery m_LineQuery;

	private EntityQuery m_ModifiedLineQuery;

	private EntityQuery m_UnlockQuery;

	private EntityArchetype m_ColorUpdateArchetype;

	private RawValueBinding m_TransportLines;

	private RawValueBinding m_PassengerTypes;

	private RawValueBinding m_CargoTypes;

	private ValueBinding<string> m_SelectedCargoType;

	private ValueBinding<string> m_SelectedPassengerType;

	private UITransportConfigurationPrefab m_Config;

	private UIUpdateState m_UpdateState;

	public override GameMode gameMode => GameMode.Game;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Expected O, but got Unknown
		//IL_022b: Expected O, but got Unknown
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_0255: Expected O, but got Unknown
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_027f: Expected O, but got Unknown
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Expected O, but got Unknown
		base.OnCreate();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_UnlockSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UnlockSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_PoliciesUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliciesUISystem>();
		m_SelectedInfoUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UITransportConfigurationData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadWrite<TransportLine>(),
			ComponentType.ReadOnly<RouteWaypoint>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_LineQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadWrite<TransportLine>(),
			ComponentType.ReadOnly<RouteWaypoint>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		m_ModifiedLineQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_UnlockQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ColorUpdateArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<ColorUpdated>()
		});
		RawValueBinding val2 = new RawValueBinding("transportationOverview", "lines", (Action<IJsonWriter>)BindLines);
		RawValueBinding binding = val2;
		m_TransportLines = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("transportationOverview", "passengerTypes", (Action<IJsonWriter>)BindPassengerTypes);
		binding = val3;
		m_PassengerTypes = val3;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val4 = new RawValueBinding("transportationOverview", "cargoTypes", (Action<IJsonWriter>)BindCargoTypes);
		binding = val4;
		m_CargoTypes = val4;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_SelectedPassengerType = new ValueBinding<string>("transportationOverview", "selectedPassengerType", Enum.GetName(typeof(TransportType), TransportType.Bus), (IWriter<string>)null, (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)(m_SelectedCargoType = new ValueBinding<string>("transportationOverview", "selectedCargoType", "None", (IWriter<string>)null, (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("transportationOverview", "delete", (Action<Entity>)DeleteLine, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("transportationOverview", "select", (Action<Entity>)SelectLine, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, Color32>("transportationOverview", "setColor", (Action<Entity, Color32>)SetLineColor, (IReader<Entity>)null, (IReader<Color32>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, string>("transportationOverview", "rename", (Action<Entity, string>)SetLineName, (IReader<Entity>)null, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, bool>("transportationOverview", "setActive", (Action<Entity, bool>)SetLineState, (IReader<Entity>)null, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, bool>("transportationOverview", "showLine", (Action<Entity, bool>)ShowLine, (IReader<Entity>)null, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, bool>("transportationOverview", "hideLine", (Action<Entity, bool>)HideLine, (IReader<Entity>)null, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, int>("transportationOverview", "setSchedule", (Action<Entity, int>)SetLineSchedule, (IReader<Entity>)null, (IReader<int>)null));
		AddBinding((IBinding)new TriggerBinding("transportationOverview", "resetVisibility", (Action)ResetLinesVisibility));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("transportationOverview", "toggleHighlight", (Action<Entity>)ToggleHighlight, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("transportationOverview", "setSelectedPassengerType", (Action<string>)SetSelectedPassengerType, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("transportationOverview", "setSelectedCargoType", (Action<string>)SetSelectedCargoType, (IReader<string>)null));
		m_UpdateState = UIUpdateState.Create(((ComponentSystemBase)this).World, 256);
	}

	private string GetInitialSelectedType()
	{
		UITransportItem[] cargoLineTypes = m_Config.m_CargoLineTypes;
		foreach (UITransportItem uITransportItem in cargoLineTypes)
		{
			if (!m_UnlockSystem.IsLocked(uITransportItem.m_Unlockable))
			{
				return Enum.GetName(typeof(TransportType), uITransportItem.m_Type);
			}
		}
		return Enum.GetName(typeof(TransportType), TransportType.None);
	}

	private void SetSelectedPassengerType(string type)
	{
		m_SelectedPassengerType.Update(type);
	}

	private void SetSelectedCargoType(string type)
	{
		m_SelectedCargoType.Update(type);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentSystemBase)this).Enabled)
		{
			m_Config = m_PrefabSystem.GetSingletonPrefab<UITransportConfigurationPrefab>(m_ConfigQuery);
			m_OutOfServicePolicy = m_PrefabSystem.GetEntity(m_Config.m_OutOfServicePolicy);
			m_DayRoutePolicy = m_PrefabSystem.GetEntity(m_Config.m_DayRoutePolicy);
			m_NightRoutePolicy = m_PrefabSystem.GetEntity(m_Config.m_NightRoutePolicy);
			m_CargoTypes.Update();
			m_PassengerTypes.Update();
			m_TransportLines.Update();
			m_SelectedCargoType.Update(GetInitialSelectedType());
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ModifiedLineQuery)).IsEmptyIgnoreFilter || m_UpdateState.Advance())
		{
			m_TransportLines.Update();
		}
		if (PrefabUtils.HasUnlockedPrefab<RouteData>(((ComponentSystemBase)this).EntityManager, m_UnlockQuery))
		{
			m_CargoTypes.Update();
			m_PassengerTypes.Update();
		}
	}

	public void RequestUpdate()
	{
		m_UpdateState.ForceUpdate();
	}

	private void DeleteLine(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			((EntityCommandBuffer)(ref val)).AddComponent<Deleted>(entity, default(Deleted));
		}
	}

	private void SelectLine(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			m_SelectedInfoUISystem.SetSelection(entity);
		}
	}

	private void SetLineColor(Entity entity, Color32 color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(entity))
		{
			return;
		}
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		((EntityCommandBuffer)(ref val)).SetComponent<Color>(entity, new Color(color));
		DynamicBuffer<RouteVehicle> val2 = default(DynamicBuffer<RouteVehicle>);
		if (EntitiesExtensions.TryGetBuffer<RouteVehicle>(((ComponentSystemBase)this).EntityManager, entity, true, ref val2))
		{
			for (int i = 0; i < val2.Length; i++)
			{
				((EntityCommandBuffer)(ref val)).AddComponent<Color>(val2[i].m_Vehicle, new Color(color));
			}
		}
		Entity val3 = ((EntityCommandBuffer)(ref val)).CreateEntity(m_ColorUpdateArchetype);
		((EntityCommandBuffer)(ref val)).SetComponent<ColorUpdated>(val3, new ColorUpdated(entity));
		RequestUpdate();
	}

	private void SetLineName(Entity entity, string name)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			m_NameSystem.SetCustomName(entity, name);
			RequestUpdate();
		}
	}

	public void SetLineState(Entity entity, bool state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			m_PoliciesUISystem.SetPolicy(entity, m_OutOfServicePolicy, !state);
			RequestUpdate();
		}
	}

	private void SetLineSchedule(Entity entity, int schedule)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			switch ((RouteSchedule)schedule)
			{
			case RouteSchedule.Day:
				m_PoliciesUISystem.SetPolicy(entity, m_NightRoutePolicy, active: false);
				m_PoliciesUISystem.SetPolicy(entity, m_DayRoutePolicy, active: true);
				break;
			case RouteSchedule.Night:
				m_PoliciesUISystem.SetPolicy(entity, m_NightRoutePolicy, active: true);
				m_PoliciesUISystem.SetPolicy(entity, m_DayRoutePolicy, active: false);
				break;
			default:
				m_PoliciesUISystem.SetPolicy(entity, m_NightRoutePolicy, active: false);
				m_PoliciesUISystem.SetPolicy(entity, m_DayRoutePolicy, active: false);
				break;
			}
			RequestUpdate();
		}
	}

	public void ShowLine(Entity entity, bool hideOthers)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			if (hideOthers)
			{
				((EntityCommandBuffer)(ref val)).AddComponent<HiddenRoute>(m_LineQuery, (EntityQueryCaptureMode)1);
			}
			((EntityCommandBuffer)(ref val)).RemoveComponent<HiddenRoute>(entity);
			RequestUpdate();
		}
	}

	public void HideLine(Entity entity, bool showOthers)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			if (showOthers)
			{
				((EntityCommandBuffer)(ref val)).RemoveComponent<HiddenRoute>(m_LineQuery, (EntityQueryCaptureMode)1);
			}
			((EntityCommandBuffer)(ref val)).AddComponent<HiddenRoute>(entity);
			RequestUpdate();
		}
	}

	public void ToggleHighlight(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Highlighted>(entity))
			{
				((EntityCommandBuffer)(ref val)).AddComponent<Highlighted>(entity);
			}
			else
			{
				((EntityCommandBuffer)(ref val)).RemoveComponent<Highlighted>(entity);
			}
		}
	}

	public void ResetLinesVisibility()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		((EntityCommandBuffer)(ref val)).RemoveComponent<HiddenRoute>(m_LineQuery, (EntityQueryCaptureMode)1);
		((EntityCommandBuffer)(ref val)).RemoveComponent<Highlighted>(m_LineQuery, (EntityQueryCaptureMode)1);
		RequestUpdate();
	}

	private void BindPassengerTypes(IJsonWriter writer)
	{
		BindTypes(writer, m_Config.m_PassengerLineTypes);
	}

	private void BindCargoTypes(IJsonWriter writer)
	{
		BindTypes(writer, m_Config.m_CargoLineTypes);
	}

	private void BindTypes(IJsonWriter writer, UITransportItem[] items)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		JsonWriterExtensions.ArrayBegin(writer, items.Length);
		foreach (UITransportItem uITransportItem in items)
		{
			new UITransportType(m_PrefabSystem.GetEntity(uITransportItem.m_Unlockable), Enum.GetName(typeof(TransportType), uITransportItem.m_Type), uITransportItem.m_Icon, m_UnlockSystem.IsLocked(uITransportItem.m_Unlockable)).Write(m_PrefabUISystem, writer);
		}
		writer.ArrayEnd();
	}

	private void BindLines(IJsonWriter binder)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<UITransportLineData> sortedLines = TransportUIUtils.GetSortedLines(m_LineQuery, ((ComponentSystemBase)this).EntityManager, m_PrefabSystem);
		JsonWriterExtensions.ArrayBegin(binder, sortedLines.Length);
		for (int i = 0; i < sortedLines.Length; i++)
		{
			BindLine(sortedLines[i], binder);
		}
		binder.ArrayEnd();
	}

	private void BindLine(UITransportLineData lineData, IJsonWriter binder)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		binder.TypeBegin("Game.UI.InGame.UITransportLine");
		binder.PropertyName("name");
		m_NameSystem.BindName(binder, lineData.entity);
		binder.PropertyName("vkName");
		m_NameSystem.BindNameForVirtualKeyboard(binder, lineData.entity);
		binder.PropertyName("lineData");
		JsonWriterExtensions.Write<UITransportLineData>(binder, lineData);
		binder.TypeEnd();
	}

	[Preserve]
	public TransportationOverviewUISystem()
	{
	}
}
