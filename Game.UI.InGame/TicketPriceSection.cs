using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Policies;
using Game.Prefabs;
using Game.Routes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class TicketPriceSection : InfoSectionBase
{
	private PoliciesUISystem m_PoliciesUISystem;

	private Entity m_TicketPricePolicy;

	private EntityQuery m_ConfigQuery;

	protected override string group => "TicketPriceSection";

	private UIPolicySlider sliderData { get; set; }

	protected override void Reset()
	{
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
		AddBinding((IBinding)(object)new TriggerBinding<int>(group, "setTicketPrice", (Action<int>)OnSetTicketPrice, (IReader<int>)null));
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ConfigQuery)).IsEmptyIgnoreFilter)
		{
			UITransportConfigurationPrefab singletonPrefab = m_PrefabSystem.GetSingletonPrefab<UITransportConfigurationPrefab>(m_ConfigQuery);
			m_TicketPricePolicy = m_PrefabSystem.GetEntity(singletonPrefab.m_TicketPricePolicy);
		}
	}

	private void OnSetTicketPrice(int newPrice)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_PoliciesUISystem.SetPolicy(selectedEntity, m_TicketPricePolicy, newPrice > 0, Mathf.Clamp((float)newPrice, sliderData.range.min, sliderData.range.max));
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					TransportLineData transportLineData = default(TransportLineData);
					if (((EntityManager)(ref entityManager)).HasComponent<Policy>(selectedEntity) && EntitiesExtensions.TryGetComponent<TransportLineData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref transportLineData))
					{
						return !transportLineData.m_CargoTransport;
					}
				}
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
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Policy> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Policy>(selectedEntity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PolicySliderData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PolicySliderData>(m_TicketPricePolicy);
		for (int i = 0; i < buffer.Length; i++)
		{
			if (!(buffer[i].m_Policy != m_TicketPricePolicy))
			{
				sliderData = new UIPolicySlider(((buffer[i].m_Flags & PolicyFlags.Active) != 0) ? buffer[i].m_Adjustment : 0f, componentData);
				return;
			}
		}
		sliderData = new UIPolicySlider(0f, componentData);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("sliderData");
		JsonWriterExtensions.Write<UIPolicySlider>(writer, sliderData);
	}

	[Preserve]
	public TicketPriceSection()
	{
	}
}
