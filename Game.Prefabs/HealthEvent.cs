using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Events;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class HealthEvent : ComponentBase
{
	public EventTargetType m_RandomTargetType = EventTargetType.Citizen;

	public HealthEventType m_HealthEventType;

	public Bounds1 m_OccurenceProbability = new Bounds1(0f, 50f);

	public Bounds1 m_TransportProbability = new Bounds1(0f, 100f);

	public bool m_RequireTracking = true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<HealthEventData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.HealthEvent>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		HealthEventData healthEventData = default(HealthEventData);
		healthEventData.m_RandomTargetType = m_RandomTargetType;
		healthEventData.m_HealthEventType = m_HealthEventType;
		healthEventData.m_OccurenceProbability = m_OccurenceProbability;
		healthEventData.m_TransportProbability = m_TransportProbability;
		healthEventData.m_RequireTracking = m_RequireTracking;
		((EntityManager)(ref entityManager)).SetComponentData<HealthEventData>(entity, healthEventData);
	}
}
