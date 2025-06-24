using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Common;
using Game.Objects;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(StaticObjectPrefab) })]
public class CraneObject : ComponentBase
{
	public Bounds1 m_DistanceRange = new Bounds1(0f, float.MaxValue);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CraneData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Crane>());
		components.Add(ComponentType.ReadWrite<PointOfInterest>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		CraneData craneData = default(CraneData);
		craneData.m_DistanceRange = m_DistanceRange;
		((EntityManager)(ref entityManager)).SetComponentData<CraneData>(entity, craneData);
	}
}
