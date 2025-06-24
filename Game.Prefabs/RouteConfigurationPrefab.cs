using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class RouteConfigurationPrefab : PrefabBase
{
	public NotificationIconPrefab m_PathfindNotification;

	public RoutePrefab m_CarPathVisualization;

	public RoutePrefab m_WatercraftPathVisualization;

	public RoutePrefab m_AircraftPathVisualization;

	public RoutePrefab m_TrainPathVisualization;

	public RoutePrefab m_HumanPathVisualization;

	public RoutePrefab m_MissingRoutePrefab;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_PathfindNotification);
		prefabs.Add(m_CarPathVisualization);
		prefabs.Add(m_WatercraftPathVisualization);
		prefabs.Add(m_AircraftPathVisualization);
		prefabs.Add(m_TrainPathVisualization);
		prefabs.Add(m_HumanPathVisualization);
		prefabs.Add(m_MissingRoutePrefab);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<RouteConfigurationData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<RouteConfigurationData>(entity, new RouteConfigurationData
		{
			m_PathfindNotification = orCreateSystemManaged.GetEntity(m_PathfindNotification),
			m_CarPathVisualization = orCreateSystemManaged.GetEntity(m_CarPathVisualization),
			m_WatercraftPathVisualization = orCreateSystemManaged.GetEntity(m_WatercraftPathVisualization),
			m_AircraftPathVisualization = orCreateSystemManaged.GetEntity(m_AircraftPathVisualization),
			m_TrainPathVisualization = orCreateSystemManaged.GetEntity(m_TrainPathVisualization),
			m_HumanPathVisualization = orCreateSystemManaged.GetEntity(m_HumanPathVisualization),
			m_MissingRoutePrefab = orCreateSystemManaged.GetEntity(m_MissingRoutePrefab)
		});
	}
}
