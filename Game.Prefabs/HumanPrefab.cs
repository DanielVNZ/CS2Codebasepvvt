using System;
using System.Collections.Generic;
using Game.Common;
using Game.Creatures;
using Game.Pathfind;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Creatures/", new Type[] { })]
public class HumanPrefab : CreaturePrefab
{
	public float m_WalkSpeed = 6f;

	public float m_RunSpeed = 12f;

	public float m_Acceleration = 8f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<HumanData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Human>());
		components.Add(ComponentType.ReadWrite<HumanNavigation>());
		components.Add(ComponentType.ReadWrite<Queue>());
		components.Add(ComponentType.ReadWrite<PathOwner>());
		components.Add(ComponentType.ReadWrite<PathElement>());
		components.Add(ComponentType.ReadWrite<Target>());
		components.Add(ComponentType.ReadWrite<Blocker>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<HumanData>(entity, new HumanData
		{
			m_WalkSpeed = m_WalkSpeed / 3.6f,
			m_RunSpeed = m_RunSpeed / 3.6f,
			m_Acceleration = m_Acceleration
		});
	}
}
