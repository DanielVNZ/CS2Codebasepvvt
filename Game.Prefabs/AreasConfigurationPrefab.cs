using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.Mathematics;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class AreasConfigurationPrefab : PrefabBase
{
	[NotNull]
	public AreaPrefab m_DefaultDistrictPrefab;

	[Tooltip("Maximum slope that is considered buildable land, for display in the map selection screen.\nMin and below: Fully buildable, Max and above: Not buildable")]
	public Bounds1 m_BuildableLandMaxSlope = new Bounds1(0.1f, 0.3f);

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_DefaultDistrictPrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AreasConfigurationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<AreasConfigurationData>(entity, new AreasConfigurationData
		{
			m_DefaultDistrictPrefab = existingSystemManaged.GetEntity(m_DefaultDistrictPrefab),
			m_BuildableLandMaxSlope = m_BuildableLandMaxSlope
		});
	}
}
