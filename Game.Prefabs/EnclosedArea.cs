using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[]
{
	typeof(LotPrefab),
	typeof(SpacePrefab),
	typeof(SurfacePrefab)
})]
public class EnclosedArea : ComponentBase
{
	public NetLanePrefab m_BorderLaneType;

	public bool m_CounterClockWise;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_BorderLaneType);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<EnclosedAreaData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Net.SubLane>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		EnclosedAreaData enclosedAreaData = default(EnclosedAreaData);
		enclosedAreaData.m_BorderLanePrefab = existingSystemManaged.GetEntity(m_BorderLaneType);
		enclosedAreaData.m_CounterClockWise = m_CounterClockWise;
		((EntityManager)(ref entityManager)).SetComponentData<EnclosedAreaData>(entity, enclosedAreaData);
	}
}
