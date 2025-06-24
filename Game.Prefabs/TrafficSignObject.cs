using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[]
{
	typeof(StaticObjectPrefab),
	typeof(MarkerObjectPrefab)
})]
public class TrafficSignObject : ComponentBase
{
	public TrafficSignType[] m_SignTypes;

	public int m_SpeedLimit;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TrafficSignData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		TrafficSignData trafficSignData = default(TrafficSignData);
		trafficSignData.m_TypeMask = 0u;
		trafficSignData.m_SpeedLimit = m_SpeedLimit;
		if (m_SignTypes != null)
		{
			for (int i = 0; i < m_SignTypes.Length; i++)
			{
				trafficSignData.m_TypeMask |= TrafficSignData.GetTypeMask(m_SignTypes[i]);
			}
		}
		((EntityManager)(ref entityManager)).SetComponentData<TrafficSignData>(entity, trafficSignData);
	}
}
