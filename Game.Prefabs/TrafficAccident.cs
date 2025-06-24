using System;
using System.Collections.Generic;
using Game.Events;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class TrafficAccident : ComponentBase
{
	public EventTargetType m_RandomSiteType = EventTargetType.Road;

	public EventTargetType m_SubjectType = EventTargetType.MovingCar;

	public TrafficAccidentType m_AccidentType;

	public float m_OccurrenceProbability = 0.01f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TrafficAccidentData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.TrafficAccident>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		TrafficAccidentData trafficAccidentData = default(TrafficAccidentData);
		trafficAccidentData.m_RandomSiteType = m_RandomSiteType;
		trafficAccidentData.m_SubjectType = m_SubjectType;
		trafficAccidentData.m_AccidentType = m_AccidentType;
		trafficAccidentData.m_OccurenceProbability = m_OccurrenceProbability;
		((EntityManager)(ref entityManager)).SetComponentData<TrafficAccidentData>(entity, trafficAccidentData);
	}
}
