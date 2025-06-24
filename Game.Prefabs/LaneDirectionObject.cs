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
public class LaneDirectionObject : ComponentBase
{
	public LaneDirectionType m_Left = LaneDirectionType.None;

	public LaneDirectionType m_Forward;

	public LaneDirectionType m_Right = LaneDirectionType.None;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<LaneDirectionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Objects.NetObject>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		LaneDirectionData laneDirectionData = default(LaneDirectionData);
		laneDirectionData.m_Left = m_Left;
		laneDirectionData.m_Forward = m_Forward;
		laneDirectionData.m_Right = m_Right;
		((EntityManager)(ref entityManager)).SetComponentData<LaneDirectionData>(entity, laneDirectionData);
	}
}
