using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Policies/", new Type[] { })]
public class PolicySliderPrefab : PolicyPrefab
{
	public Bounds1 m_SliderRange = new Bounds1(0f, 1f);

	public float m_SliderDefault = 0.5f;

	public float m_SliderStep = 0.1f;

	public PolicySliderUnit m_Unit = PolicySliderUnit.integer;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<PolicySliderData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		PolicySliderData policySliderData = default(PolicySliderData);
		policySliderData.m_Range = m_SliderRange;
		policySliderData.m_Default = m_SliderDefault;
		policySliderData.m_Step = m_SliderStep;
		policySliderData.m_Unit = (int)m_Unit;
		((EntityManager)(ref entityManager)).SetComponentData<PolicySliderData>(entity, policySliderData);
	}
}
