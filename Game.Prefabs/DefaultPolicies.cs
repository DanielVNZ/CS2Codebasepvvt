using System;
using System.Collections.Generic;
using Game.Policies;
using Game.Routes;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Policies/", new Type[]
{
	typeof(DistrictPrefab),
	typeof(BuildingPrefab),
	typeof(RoutePrefab),
	typeof(ServiceFeeParameterPrefab)
})]
public class DefaultPolicies : ComponentBase
{
	public DefaultPolicyInfo[] m_Policies;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Policies != null)
		{
			for (int i = 0; i < m_Policies.Length; i++)
			{
				prefabs.Add(m_Policies[i].m_Policy);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<DefaultPolicyData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!components.Contains(ComponentType.ReadWrite<Waypoint>()) && !components.Contains(ComponentType.ReadWrite<Segment>()))
		{
			components.Add(ComponentType.ReadWrite<Policy>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (m_Policies != null)
		{
			PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
			DynamicBuffer<DefaultPolicyData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<DefaultPolicyData>(entity, false);
			for (int i = 0; i < m_Policies.Length; i++)
			{
				DefaultPolicyInfo defaultPolicyInfo = m_Policies[i];
				buffer.Add(new DefaultPolicyData(existingSystemManaged.GetEntity(defaultPolicyInfo.m_Policy)));
			}
		}
	}
}
