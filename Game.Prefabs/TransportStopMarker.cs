using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Notifications/", new Type[] { typeof(NotificationIconPrefab) })]
public class TransportStopMarker : ComponentBase
{
	public TransportType m_TransportType;

	public bool m_PassengerTransport;

	public bool m_CargoTransport;

	public bool m_WorkStop;

	public bool m_WorkLocation;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TransportStopMarkerData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		TransportStopMarkerData transportStopMarkerData = default(TransportStopMarkerData);
		transportStopMarkerData.m_TransportType = m_TransportType;
		if (m_TransportType == TransportType.Work)
		{
			transportStopMarkerData.m_StopTypeA = m_WorkStop;
			transportStopMarkerData.m_StopTypeB = m_WorkLocation;
		}
		else
		{
			transportStopMarkerData.m_StopTypeA = m_PassengerTransport;
			transportStopMarkerData.m_StopTypeB = m_CargoTransport;
		}
		((EntityManager)(ref entityManager)).SetComponentData<TransportStopMarkerData>(entity, transportStopMarkerData);
	}
}
