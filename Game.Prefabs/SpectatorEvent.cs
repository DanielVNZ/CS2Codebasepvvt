using System;
using System.Collections.Generic;
using Game.Events;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class SpectatorEvent : ComponentBase
{
	public EventTargetType m_RandomSiteType = EventTargetType.TransportDepot;

	public float m_PreparationDuration = 0.1f;

	public float m_ActiveDuration = 0.1f;

	public float m_TerminationDuration = 0.1f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<SpectatorEventData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.SpectatorEvent>());
		components.Add(ComponentType.ReadWrite<Duration>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		SpectatorEventData spectatorEventData = default(SpectatorEventData);
		spectatorEventData.m_RandomSiteType = m_RandomSiteType;
		spectatorEventData.m_PreparationDuration = m_PreparationDuration;
		spectatorEventData.m_ActiveDuration = m_ActiveDuration;
		spectatorEventData.m_TerminationDuration = m_TerminationDuration;
		((EntityManager)(ref entityManager)).SetComponentData<SpectatorEventData>(entity, spectatorEventData);
	}
}
