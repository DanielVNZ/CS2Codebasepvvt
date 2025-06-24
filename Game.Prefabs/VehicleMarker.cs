using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Notifications/", new Type[] { typeof(NotificationIconPrefab) })]
public class VehicleMarker : ComponentBase
{
	public VehicleType m_VehicleType;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<VehicleMarkerData>());
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
		VehicleMarkerData vehicleMarkerData = default(VehicleMarkerData);
		vehicleMarkerData.m_VehicleType = m_VehicleType;
		((EntityManager)(ref entityManager)).SetComponentData<VehicleMarkerData>(entity, vehicleMarkerData);
	}
}
