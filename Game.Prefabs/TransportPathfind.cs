using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Pathfind/", new Type[] { typeof(PathfindPrefab) })]
public class TransportPathfind : ComponentBase
{
	public PathfindCostInfo m_OrderingCost = new PathfindCostInfo(5f, 0f, 0f, 5f);

	public PathfindCostInfo m_StartingCost = new PathfindCostInfo(5f, 0f, 10f, 5f);

	public PathfindCostInfo m_TravelCost = new PathfindCostInfo(0f, 0f, 0.02f, 0f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PathfindTransportData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<PathfindTransportData>(entity, new PathfindTransportData
		{
			m_OrderingCost = m_OrderingCost.ToPathfindCosts(),
			m_StartingCost = m_StartingCost.ToPathfindCosts(),
			m_TravelCost = m_TravelCost.ToPathfindCosts()
		});
	}
}
