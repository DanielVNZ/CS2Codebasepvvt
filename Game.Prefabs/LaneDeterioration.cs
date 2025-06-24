using System;
using System.Collections.Generic;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetLanePrefab) })]
public class LaneDeterioration : ComponentBase
{
	public float m_TrafficDeterioration = 0.01f;

	public float m_TimeDeterioration = 0.5f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<LaneDeteriorationData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!components.Contains(ComponentType.ReadWrite<MasterLane>()))
		{
			components.Add(ComponentType.ReadWrite<LaneCondition>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		LaneDeteriorationData laneDeteriorationData = default(LaneDeteriorationData);
		laneDeteriorationData.m_TrafficFactor = m_TrafficDeterioration;
		laneDeteriorationData.m_TimeFactor = m_TimeDeterioration;
		((EntityManager)(ref entityManager)).SetComponentData<LaneDeteriorationData>(entity, laneDeteriorationData);
	}
}
