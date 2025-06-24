using System;
using System.Collections.Generic;
using Game.Events;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class VehicleLaunch : ComponentBase
{
	public TransportType m_TransportType;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<VehicleLaunchData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.VehicleLaunch>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		VehicleLaunchData vehicleLaunchData = default(VehicleLaunchData);
		vehicleLaunchData.m_TransportType = m_TransportType;
		((EntityManager)(ref entityManager)).SetComponentData<VehicleLaunchData>(entity, vehicleLaunchData);
	}
}
