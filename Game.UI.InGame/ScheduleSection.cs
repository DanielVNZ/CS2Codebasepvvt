using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Routes;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ScheduleSection : InfoSectionBase
{
	private PoliciesUISystem m_PoliciesUISystem;

	private Entity m_NightRoutePolicy;

	private Entity m_DayRoutePolicy;

	private EntityQuery m_ConfigQuery;

	protected override string group => "ScheduleSection";

	private RouteSchedule schedule { get; set; }

	protected override void Reset()
	{
		schedule = RouteSchedule.DayAndNight;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PoliciesUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliciesUISystem>();
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UITransportConfigurationData>() });
		AddBinding((IBinding)(object)new TriggerBinding<int>(group, "setSchedule", (Action<int>)OnSetSchedule, (IReader<int>)null));
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ConfigQuery)).IsEmptyIgnoreFilter)
		{
			UITransportConfigurationPrefab singletonPrefab = m_PrefabSystem.GetSingletonPrefab<UITransportConfigurationPrefab>(m_ConfigQuery);
			m_DayRoutePolicy = m_PrefabSystem.GetEntity(singletonPrefab.m_DayRoutePolicy);
			m_NightRoutePolicy = m_PrefabSystem.GetEntity(singletonPrefab.m_NightRoutePolicy);
		}
	}

	private void OnSetSchedule(int newSchedule)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		switch ((RouteSchedule)newSchedule)
		{
		case RouteSchedule.Day:
			m_PoliciesUISystem.SetPolicy(selectedEntity, m_NightRoutePolicy, active: false);
			m_PoliciesUISystem.SetPolicy(selectedEntity, m_DayRoutePolicy, active: true);
			break;
		case RouteSchedule.Night:
			m_PoliciesUISystem.SetPolicy(selectedEntity, m_NightRoutePolicy, active: true);
			m_PoliciesUISystem.SetPolicy(selectedEntity, m_DayRoutePolicy, active: false);
			break;
		default:
			m_PoliciesUISystem.SetPolicy(selectedEntity, m_NightRoutePolicy, active: false);
			m_PoliciesUISystem.SetPolicy(selectedEntity, m_DayRoutePolicy, active: false);
			break;
		}
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(selectedEntity);
			}
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Route componentData = ((EntityManager)(ref entityManager)).GetComponentData<Route>(selectedEntity);
		schedule = ((!RouteUtils.CheckOption(componentData, RouteOption.Day)) ? (RouteUtils.CheckOption(componentData, RouteOption.Night) ? RouteSchedule.Night : RouteSchedule.DayAndNight) : RouteSchedule.Day);
		base.tooltipTags.Add("TransportLine");
		base.tooltipTags.Add("CargoRoute");
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("schedule");
		writer.Write((int)schedule);
	}

	[Preserve]
	public ScheduleSection()
	{
	}
}
