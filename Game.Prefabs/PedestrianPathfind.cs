using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Pathfind/", new Type[] { typeof(PathfindPrefab) })]
public class PedestrianPathfind : ComponentBase
{
	public PathfindCostInfo m_WalkingCost = new PathfindCostInfo(0f, 0f, 0f, 0.01f);

	public PathfindCostInfo m_CrosswalkCost = new PathfindCostInfo(0f, 0f, 0f, 5f);

	public PathfindCostInfo m_UnsafeCrosswalkCost = new PathfindCostInfo(0f, 100f, 0f, 5f);

	public PathfindCostInfo m_SpawnCost = new PathfindCostInfo(5f, 0f, 0f, 0f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PathfindPedestrianData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<PathfindPedestrianData>(entity, new PathfindPedestrianData
		{
			m_WalkingCost = m_WalkingCost.ToPathfindCosts(),
			m_CrosswalkCost = m_CrosswalkCost.ToPathfindCosts(),
			m_UnsafeCrosswalkCost = m_UnsafeCrosswalkCost.ToPathfindCosts(),
			m_SpawnCost = m_SpawnCost.ToPathfindCosts()
		});
	}
}
