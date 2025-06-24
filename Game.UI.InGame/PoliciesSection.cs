using System;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Policies;
using Game.Routes;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PoliciesSection : InfoSectionBase
{
	private enum PoliciesKey
	{
		Building,
		District
	}

	private PoliciesUISystem m_PoliciesUISystem;

	protected override string group => "PoliciesSection";

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_PoliciesUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliciesUISystem>();
		PoliciesUISystem policiesUISystem = m_PoliciesUISystem;
		policiesUISystem.EventPolicyUnlocked = (Action)Delegate.Combine(policiesUISystem.EventPolicyUnlocked, new Action(m_InfoUISystem.RequestUpdate));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		PoliciesUISystem policiesUISystem = m_PoliciesUISystem;
		policiesUISystem.EventPolicyUnlocked = (Action)Delegate.Remove(policiesUISystem.EventPolicyUnlocked, new Action(m_InfoUISystem.RequestUpdate));
	}

	protected override void Reset()
	{
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		base.visible = ((EntityManager)(ref entityManager)).HasComponent<Policy>(selectedEntity) && m_PoliciesUISystem.GatherSelectedInfoPolicies(selectedEntity);
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
		{
			base.tooltipKeys.Add(PoliciesKey.Building.ToString());
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
		{
			base.tooltipKeys.Add(PoliciesKey.District.ToString());
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			base.tooltipTags.Add(TooltipTags.CargoRoute.ToString());
			base.tooltipTags.Add(TooltipTags.TransportLine.ToString());
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("policies");
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
		{
			m_PoliciesUISystem.BindBuildingPolicies(writer);
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
		{
			m_PoliciesUISystem.BindDistrictPolicies(writer);
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			m_PoliciesUISystem.BindRoutePolicies(writer);
		}
		else
		{
			writer.WriteNull();
		}
	}

	[Preserve]
	public PoliciesSection()
	{
	}
}
