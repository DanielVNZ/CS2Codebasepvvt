using System;
using System.Collections.Generic;
using Game.Citizens;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Citizens/", new Type[] { })]
public class HouseholdPetPrefab : ArchetypePrefab
{
	public PetType m_Type;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<HouseholdPetData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<HouseholdPet>());
		components.Add(ComponentType.ReadWrite<UpdateFrame>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		HouseholdPetData householdPetData = default(HouseholdPetData);
		householdPetData.m_Type = m_Type;
		((EntityManager)(ref entityManager)).SetComponentData<HouseholdPetData>(entity, householdPetData);
	}
}
