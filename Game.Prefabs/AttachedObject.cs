using System;
using System.Collections.Generic;
using Game.Objects;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[]
{
	typeof(StaticObjectPrefab),
	typeof(MarkerObjectPrefab)
})]
public class AttachedObject : ComponentBase
{
	public AttachedObjectType m_AttachType;

	public float m_AttachOffset;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PlaceableObjectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		PlaceableObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableObjectData>(entity);
		switch (m_AttachType)
		{
		case AttachedObjectType.Ground:
			componentData.m_Flags |= PlacementFlags.OnGround;
			componentData.m_PlacementOffset.y = m_AttachOffset;
			break;
		case AttachedObjectType.Wall:
			componentData.m_Flags &= ~PlacementFlags.OnGround;
			componentData.m_Flags |= PlacementFlags.Wall;
			componentData.m_PlacementOffset.z = m_AttachOffset;
			break;
		case AttachedObjectType.Hanging:
			componentData.m_Flags &= ~PlacementFlags.OnGround;
			componentData.m_Flags |= PlacementFlags.Hanging;
			componentData.m_PlacementOffset.y = m_AttachOffset;
			break;
		}
		((EntityManager)(ref entityManager)).SetComponentData<PlaceableObjectData>(entity, componentData);
	}
}
