using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Events;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class CalendarEvent : ComponentBase
{
	public EventTargetType m_RandomTargetType = EventTargetType.Couple;

	public Bounds1 m_AffectedProbability = new Bounds1(25f, 25f);

	public Bounds1 m_OccurenceProbability = new Bounds1(100f, 100f);

	[EnumFlag]
	public CalendarEventMonths m_AllowedMonths;

	[EnumFlag]
	public CalendarEventTimes m_AllowedTimes;

	[Tooltip("In fourths of a day")]
	public int m_Duration;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CalendarEventData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.CalendarEvent>());
		components.Add(ComponentType.ReadWrite<Duration>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		CalendarEventData calendarEventData = default(CalendarEventData);
		calendarEventData.m_RandomTargetType = m_RandomTargetType;
		calendarEventData.m_AffectedProbability = m_AffectedProbability;
		calendarEventData.m_OccurenceProbability = m_OccurenceProbability;
		calendarEventData.m_AllowedMonths = m_AllowedMonths;
		calendarEventData.m_AllowedTimes = m_AllowedTimes;
		calendarEventData.m_Duration = m_Duration;
		((EntityManager)(ref entityManager)).SetComponentData<CalendarEventData>(entity, calendarEventData);
	}
}
