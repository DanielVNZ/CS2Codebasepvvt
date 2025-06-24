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
public class TrafficLightObject : ComponentBase
{
	public bool m_VehicleLeft;

	public bool m_VehicleRight;

	public bool m_CrossingLeft;

	public bool m_CrossingRight;

	public bool m_AllowFlipped;

	public Bounds1 m_ReachOffset;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TrafficLightData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TrafficLight>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		TrafficLightData trafficLightData = default(TrafficLightData);
		trafficLightData.m_Type = (TrafficLightType)0;
		trafficLightData.m_ReachOffset = m_ReachOffset;
		if (m_VehicleLeft)
		{
			trafficLightData.m_Type |= TrafficLightType.VehicleLeft;
		}
		if (m_VehicleRight)
		{
			trafficLightData.m_Type |= TrafficLightType.VehicleRight;
		}
		if (m_CrossingLeft)
		{
			trafficLightData.m_Type |= TrafficLightType.CrossingLeft;
		}
		if (m_CrossingRight)
		{
			trafficLightData.m_Type |= TrafficLightType.CrossingRight;
		}
		if (m_AllowFlipped)
		{
			trafficLightData.m_Type |= TrafficLightType.AllowFlipped;
		}
		((EntityManager)(ref entityManager)).SetComponentData<TrafficLightData>(entity, trafficLightData);
	}
}
