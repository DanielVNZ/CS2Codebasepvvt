using System;
using System.Collections.Generic;
using Game.Events;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class JournalEventComponent : ComponentBase
{
	public string m_Icon;

	public EventDataTrackingType[] m_TrackedData;

	public EventCityEffectTrackingType[] m_TrackedCityEffects;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<JournalEvent>());
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<JournalEventPrefabData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<JournalEventPrefabData>(entity, new JournalEventPrefabData
		{
			m_DataFlags = GetDataFlags(),
			m_EffectFlags = GetEffectFlags()
		});
	}

	public int GetDataFlags()
	{
		int num = 0;
		for (int i = 0; i < m_TrackedData.Length; i++)
		{
			if (EventJournalUtils.IsValid(m_TrackedData[i]))
			{
				num |= 1 << (int)m_TrackedData[i];
			}
		}
		return num;
	}

	public int GetEffectFlags()
	{
		int num = 0;
		for (int i = 0; i < m_TrackedCityEffects.Length; i++)
		{
			if (EventJournalUtils.IsValid(m_TrackedCityEffects[i]))
			{
				num |= 1 << (int)m_TrackedCityEffects[i];
			}
		}
		return num;
	}
}
