using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Notifications/", new Type[] { typeof(NotificationIconPrefab) })]
public class MarkerMarker : ComponentBase
{
	public MarkerType m_MarkerType;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<MarkerMarkerData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		MarkerMarkerData markerMarkerData = default(MarkerMarkerData);
		markerMarkerData.m_MarkerType = m_MarkerType;
		((EntityManager)(ref entityManager)).SetComponentData<MarkerMarkerData>(entity, markerMarkerData);
	}
}
