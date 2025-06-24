using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Objects;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[]
{
	typeof(StaticObjectPrefab),
	typeof(MarkerObjectPrefab)
})]
public class PillarObject : ComponentBase
{
	public PillarType m_Type;

	public float m_AnchorOffset;

	public Bounds1 m_VerticalPillarOffsetRange = new Bounds1(-1f, 1f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PillarData>());
		if (m_AnchorOffset != 0f)
		{
			components.Add(ComponentType.ReadWrite<PlaceableObjectData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Pillar>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		PillarData pillarData = default(PillarData);
		pillarData.m_Type = m_Type;
		pillarData.m_OffsetRange = m_VerticalPillarOffsetRange;
		((EntityManager)(ref entityManager)).SetComponentData<PillarData>(entity, pillarData);
		if (m_AnchorOffset != 0f)
		{
			PlaceableObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableObjectData>(entity);
			componentData.m_PlacementOffset.y = m_AnchorOffset;
			((EntityManager)(ref entityManager)).SetComponentData<PlaceableObjectData>(entity, componentData);
		}
	}
}
