using System;
using System.Collections.Generic;
using Game.Objects;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingPrefab) })]
public class PlaceholderBuilding : ComponentBase
{
	public const int kStatLevel = 1;

	public BuildingType m_BuildingType;

	public ZonePrefab m_ZoneType;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_ZoneType);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceholderBuildingData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Placeholder>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<PlaceholderBuildingData>(entity, new PlaceholderBuildingData
		{
			m_Type = m_BuildingType,
			m_ZonePrefab = (((Object)(object)m_ZoneType != (Object)null) ? existingSystemManaged.GetEntity(m_ZoneType) : Entity.Null)
		});
	}
}
