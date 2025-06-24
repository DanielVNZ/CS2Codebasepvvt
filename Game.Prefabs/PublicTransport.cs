using System;
using System.Collections.Generic;
using Game.Objects;
using Game.Pathfind;
using Game.PSI;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ExcludeGeneratedModTag]
[ComponentMenu("Vehicles/", new Type[] { typeof(VehiclePrefab) })]
public class PublicTransport : ComponentBase
{
	public TransportType m_TransportType;

	public int m_PassengerCapacity = 30;

	[EnumFlag]
	public PublicTransportPurpose m_Purposes = PublicTransportPurpose.TransportLine;

	public float m_MaintenanceRange;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			if ((m_Purposes & PublicTransportPurpose.TransportLine) != 0)
			{
				yield return "PublicTransport";
				yield return "PublicTransport" + m_TransportType;
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PublicTransportVehicleData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Vehicles.PublicTransport>());
		components.Add(ComponentType.ReadWrite<Passenger>());
		components.Add(ComponentType.ReadWrite<Odometer>());
		if (components.Contains(ComponentType.ReadWrite<Moving>()) && (!components.Contains(ComponentType.ReadWrite<Controller>()) || components.Contains(ComponentType.ReadWrite<LayoutElement>())))
		{
			components.Add(ComponentType.ReadWrite<PathInformation>());
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<PublicTransportVehicleData>(entity, new PublicTransportVehicleData(m_TransportType, m_PassengerCapacity, m_Purposes, m_MaintenanceRange * 1000f));
		if (((EntityManager)(ref entityManager)).HasComponent<CarData>(entity))
		{
			((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(1));
		}
	}
}
