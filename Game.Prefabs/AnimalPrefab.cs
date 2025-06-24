using System;
using System.Collections.Generic;
using Game.Common;
using Game.Creatures;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Creatures/", new Type[] { })]
public class AnimalPrefab : CreaturePrefab
{
	public float m_MoveSpeed = 20f;

	public float m_Acceleration = 10f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AnimalData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Animal>());
		components.Add(ComponentType.ReadWrite<AnimalNavigation>());
		components.Add(ComponentType.ReadWrite<Target>());
		components.Add(ComponentType.ReadWrite<Blocker>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		AnimalData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AnimalData>(entity);
		componentData.m_MoveSpeed = m_MoveSpeed / 3.6f;
		componentData.m_Acceleration = m_Acceleration;
		((EntityManager)(ref entityManager)).SetComponentData<AnimalData>(entity, componentData);
	}
}
