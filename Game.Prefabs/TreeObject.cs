using System;
using System.Collections.Generic;
using Colossal.Entities;
using Game.Net;
using Game.Objects;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(StaticObjectPrefab) })]
public class TreeObject : ComponentBase
{
	public float m_WoodAmount = 3000f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlantData>());
		components.Add(ComponentType.ReadWrite<TreeData>());
		components.Add(ComponentType.ReadWrite<GrowthScaleData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Plant>());
		components.Add(ComponentType.ReadWrite<Tree>());
		components.Add(ComponentType.ReadWrite<Color>());
		components.Add(ComponentType.ReadWrite<UpdateFrame>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		TreeData treeData = default(TreeData);
		treeData.m_WoodAmount = m_WoodAmount;
		((EntityManager)(ref entityManager)).SetComponentData<TreeData>(entity, treeData);
		PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
		if (EntitiesExtensions.TryGetComponent<PlaceableObjectData>(entityManager, entity, ref placeableObjectData) && (placeableObjectData.m_Flags & (Game.Objects.PlacementFlags.RoadNode | Game.Objects.PlacementFlags.RoadEdge)) == 0)
		{
			placeableObjectData.m_SubReplacementType = SubReplacementType.Tree;
			((EntityManager)(ref entityManager)).SetComponentData<PlaceableObjectData>(entity, placeableObjectData);
		}
	}
}
