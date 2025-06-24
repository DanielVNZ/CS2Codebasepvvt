using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class XPParametersPrefab : PrefabBase
{
	public float m_XPPerPopulation;

	public float m_XPPerHappiness;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<XPParameterData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<XPParameterData>(entity, new XPParameterData
		{
			m_XPPerHappiness = m_XPPerHappiness,
			m_XPPerPopulation = m_XPPerPopulation
		});
	}
}
