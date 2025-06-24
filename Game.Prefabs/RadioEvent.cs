using System;
using System.Collections.Generic;
using Game.Audio.Radio;
using Game.Common;
using Game.Triggers;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Triggers/", new Type[]
{
	typeof(TriggerPrefab),
	typeof(StatisticTriggerPrefab)
})]
public class RadioEvent : ComponentBase
{
	public Radio.SegmentType m_SegmentType = Radio.SegmentType.News;

	[Tooltip("Only for emergency events")]
	[ShowIf("m_SegmentType", 6, false)]
	public int m_EmergencyFrameDelay;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<RadioEventData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Triggers.RadioEvent>());
		components.Add(ComponentType.ReadWrite<PrefabRef>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		RadioEventData radioEventData = default(RadioEventData);
		radioEventData.m_Archetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		radioEventData.m_SegmentType = m_SegmentType;
		radioEventData.m_EmergencyFrameDelay = m_EmergencyFrameDelay;
		((EntityManager)(ref entityManager)).SetComponentData<RadioEventData>(entity, radioEventData);
	}
}
