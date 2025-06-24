using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class TrafficConfigurationPrefab : PrefabBase
{
	public NotificationIconPrefab m_BottleneckNotification;

	public NotificationIconPrefab m_DeadEndNotification;

	public NotificationIconPrefab m_RoadConnectionNotification;

	public NotificationIconPrefab m_TrackConnectionNotification;

	public NotificationIconPrefab m_CarConnectionNotification;

	public NotificationIconPrefab m_ShipConnectionNotification;

	public NotificationIconPrefab m_TrainConnectionNotification;

	public NotificationIconPrefab m_PedestrianConnectionNotification;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_BottleneckNotification);
		prefabs.Add(m_DeadEndNotification);
		prefabs.Add(m_RoadConnectionNotification);
		prefabs.Add(m_TrackConnectionNotification);
		prefabs.Add(m_CarConnectionNotification);
		prefabs.Add(m_ShipConnectionNotification);
		prefabs.Add(m_TrainConnectionNotification);
		prefabs.Add(m_PedestrianConnectionNotification);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TrafficConfigurationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<TrafficConfigurationData>(entity, new TrafficConfigurationData
		{
			m_BottleneckNotification = orCreateSystemManaged.GetEntity(m_BottleneckNotification),
			m_DeadEndNotification = orCreateSystemManaged.GetEntity(m_DeadEndNotification),
			m_RoadConnectionNotification = orCreateSystemManaged.GetEntity(m_RoadConnectionNotification),
			m_TrackConnectionNotification = orCreateSystemManaged.GetEntity(m_TrackConnectionNotification),
			m_CarConnectionNotification = orCreateSystemManaged.GetEntity(m_CarConnectionNotification),
			m_ShipConnectionNotification = orCreateSystemManaged.GetEntity(m_ShipConnectionNotification),
			m_TrainConnectionNotification = orCreateSystemManaged.GetEntity(m_TrainConnectionNotification),
			m_PedestrianConnectionNotification = orCreateSystemManaged.GetEntity(m_PedestrianConnectionNotification)
		});
	}
}
