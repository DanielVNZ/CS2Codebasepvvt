using System;
using System.Collections.Generic;
using Colossal.Entities;
using Game.Net;
using Game.Objects;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[]
{
	typeof(StaticObjectPrefab),
	typeof(NetLaneGeometryPrefab)
})]
public class PlantObject : ComponentBase
{
	public float m_PotCoverage;

	public bool m_TreeReplacement;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlantData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Plant>());
		components.Add(ComponentType.ReadWrite<UpdateFrame>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
		if (m_TreeReplacement && EntitiesExtensions.TryGetComponent<PlaceableObjectData>(entityManager, entity, ref placeableObjectData) && (placeableObjectData.m_Flags & (Game.Objects.PlacementFlags.RoadNode | Game.Objects.PlacementFlags.RoadEdge)) == 0)
		{
			placeableObjectData.m_SubReplacementType = SubReplacementType.Tree;
			((EntityManager)(ref entityManager)).SetComponentData<PlaceableObjectData>(entity, placeableObjectData);
		}
	}
}
