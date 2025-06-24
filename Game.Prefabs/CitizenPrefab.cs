using System;
using System.Collections.Generic;
using Game.Agents;
using Game.Citizens;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Citizens/", new Type[] { })]
public class CitizenPrefab : ArchetypePrefab
{
	public bool m_Male;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<CitizenData>());
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
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Citizen>());
		components.Add(ComponentType.ReadWrite<TripNeeded>());
		components.Add(ComponentType.ReadWrite<CrimeVictim>());
		components.Add(ComponentType.ReadWrite<MailSender>());
		components.Add(ComponentType.ReadWrite<Arrived>());
		components.Add(ComponentType.ReadWrite<CarKeeper>());
		components.Add(ComponentType.ReadWrite<HasJobSeeker>());
		components.Add(ComponentType.ReadWrite<UpdateFrame>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<CitizenData>(entity, new CitizenData
		{
			m_Male = m_Male
		});
	}
}
