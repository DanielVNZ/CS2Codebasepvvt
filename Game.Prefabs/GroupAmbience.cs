using System;
using System.Collections.Generic;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Zones/", new Type[] { typeof(ZonePrefab) })]
public class GroupAmbience : ComponentBase, IZoneBuildingComponent
{
	public GroupAmbienceType m_AmbienceType;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<GroupAmbienceData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<GroupAmbienceData>(entity, new GroupAmbienceData
		{
			m_AmbienceType = m_AmbienceType
		});
	}

	public void GetBuildingPrefabComponents(HashSet<ComponentType> components, BuildingPrefab buildingPrefab, byte level)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<GroupAmbienceData>());
	}

	public void GetBuildingArchetypeComponents(HashSet<ComponentType> components, BuildingPrefab buildingPrefab, byte level)
	{
	}

	public void InitializeBuilding(EntityManager entityManager, Entity entity, BuildingPrefab buildingPrefab, byte level)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!buildingPrefab.Has<GroupAmbience>())
		{
			((EntityManager)(ref entityManager)).SetComponentData<GroupAmbienceData>(entity, new GroupAmbienceData
			{
				m_AmbienceType = m_AmbienceType
			});
		}
	}
}
