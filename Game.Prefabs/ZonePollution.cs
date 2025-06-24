using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Zones/", new Type[] { typeof(ZonePrefab) })]
public class ZonePollution : ComponentBase, IZoneBuildingComponent
{
	[Min(0f)]
	public float m_GroundPollution;

	[Min(0f)]
	public float m_AirPollution;

	[Min(0f)]
	public float m_NoisePollution;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ZonePollutionData>());
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
		((EntityManager)(ref entityManager)).SetComponentData<ZonePollutionData>(entity, new ZonePollutionData
		{
			m_GroundPollution = m_GroundPollution,
			m_AirPollution = m_AirPollution,
			m_NoisePollution = m_NoisePollution
		});
	}

	public void GetBuildingPrefabComponents(HashSet<ComponentType> components, BuildingPrefab buildingPrefab, byte level)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PollutionData>());
	}

	public void GetBuildingArchetypeComponents(HashSet<ComponentType> components, BuildingPrefab buildingPrefab, byte level)
	{
		GetBuildingPollutionData(buildingPrefab).AddArchetypeComponents(components);
	}

	public void InitializeBuilding(EntityManager entityManager, Entity entity, BuildingPrefab buildingPrefab, byte level)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!buildingPrefab.Has<Pollution>())
		{
			((EntityManager)(ref entityManager)).SetComponentData<PollutionData>(entity, GetBuildingPollutionData(buildingPrefab));
		}
	}

	private PollutionData GetBuildingPollutionData(BuildingPrefab buildingPrefab)
	{
		int lotSize = buildingPrefab.lotSize;
		return new PollutionData
		{
			m_GroundPollution = m_GroundPollution * (float)lotSize,
			m_AirPollution = m_AirPollution * (float)lotSize,
			m_NoisePollution = m_NoisePollution * (float)lotSize
		};
	}
}
