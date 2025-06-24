using System;
using System.Collections.Generic;
using Game.Citizens;
using Game.Economy;
using Game.Events;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class HaveCoordinatedMeeting : ComponentBase
{
	public CoordinatedMeetingPhase[] m_Phases;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<HaveCoordinatedMeetingData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CoordinatedMeeting>());
		components.Add(ComponentType.ReadWrite<CoordinatedMeetingAttendee>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
		components.Add(ComponentType.ReadWrite<PrefabRef>());
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Phases.Length; i++)
		{
			if ((Object)(object)m_Phases[i].m_Notification != (Object)null)
			{
				prefabs.Add(m_Phases[i].m_Notification);
			}
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<HaveCoordinatedMeetingData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HaveCoordinatedMeetingData>(entity, false);
		if (m_Phases != null)
		{
			HaveCoordinatedMeetingData haveCoordinatedMeetingData = default(HaveCoordinatedMeetingData);
			for (int i = 0; i < m_Phases.Length; i++)
			{
				CoordinatedMeetingPhase coordinatedMeetingPhase = m_Phases[i];
				TravelPurpose travelPurpose = new TravelPurpose
				{
					m_Purpose = coordinatedMeetingPhase.m_Purpose.m_Purpose,
					m_Data = coordinatedMeetingPhase.m_Purpose.m_Data,
					m_Resource = EconomyUtils.GetResource(coordinatedMeetingPhase.m_Purpose.m_Resource)
				};
				haveCoordinatedMeetingData.m_TravelPurpose = travelPurpose;
				haveCoordinatedMeetingData.m_Delay = coordinatedMeetingPhase.m_Delay;
				haveCoordinatedMeetingData.m_Notification = (((Object)(object)coordinatedMeetingPhase.m_Notification != (Object)null) ? World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>().GetEntity(coordinatedMeetingPhase.m_Notification) : Entity.Null);
				buffer.Add(haveCoordinatedMeetingData);
			}
		}
	}
}
