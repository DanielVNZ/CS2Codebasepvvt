using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class Pollution : ComponentBase, IServiceUpgrade
{
	[Min(0f)]
	public int m_GroundPollution;

	[Min(0f)]
	public int m_AirPollution;

	[Min(0f)]
	public int m_NoisePollution;

	[Tooltip("Disable this if you don't want the pollution to be multiplied with renters(household members/employees)")]
	public bool m_ScaleWithRenters = true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PollutionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		if (!base.prefab.Has<ServiceUpgrade>() && !base.prefab.Has<PlaceholderBuilding>())
		{
			GetPollutionData().AddArchetypeComponents(components);
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		GetPollutionData().AddArchetypeComponents(components);
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<PollutionData>(entity, GetPollutionData());
	}

	private PollutionData GetPollutionData()
	{
		return new PollutionData
		{
			m_GroundPollution = m_GroundPollution,
			m_AirPollution = m_AirPollution,
			m_NoisePollution = m_NoisePollution,
			m_ScaleWithRenters = m_ScaleWithRenters
		};
	}
}
