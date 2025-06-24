using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { typeof(LotPrefab) })]
public class ExtractorArea : ComponentBase
{
	public MapFeature m_MapFeature;

	[Tooltip("Spawned object surface area per extracted resource amount")]
	public float m_ObjectSpawnFactor = 2f;

	[Tooltip("Maximum object surface area proportion of total extractor area")]
	public float m_MaxObjectArea = 0.25f;

	[Tooltip("Determines whether or not producing this resource requires a set Natural Resource; Fertile Land, Forest, Ore, Oil")]
	public bool m_RequireNaturalResource = true;

	[Tooltip("The work vehicle spawn factor that multiply to extractor company's work amount")]
	public float m_WorkAmountFactor = 1f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ExtractorAreaData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Extractor>());
		components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		if (m_MapFeature == MapFeature.Forest)
		{
			components.Add(ComponentType.ReadWrite<WoodResource>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<ExtractorAreaData>(entity, new ExtractorAreaData
		{
			m_MapFeature = m_MapFeature,
			m_ObjectSpawnFactor = m_ObjectSpawnFactor,
			m_MaxObjectArea = m_MaxObjectArea,
			m_RequireNaturalResource = m_RequireNaturalResource,
			m_WorkAmountFactor = m_WorkAmountFactor
		});
	}
}
