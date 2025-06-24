using System;
using System.Collections.Generic;
using Game.Common;
using Game.Triggers;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Triggers/", new Type[] { typeof(TriggerPrefab) })]
public class LifePathEvent : ComponentBase
{
	public LifePathEventType m_EventType;

	public bool m_IsChirp;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<LifePathEventData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Triggers.LifePathEvent>());
		components.Add(ComponentType.ReadWrite<PrefabRef>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		hashSet.Add(ComponentType.ReadWrite<Game.Triggers.Chirp>());
		hashSet.Add(ComponentType.ReadWrite<ChirpEntity>());
		hashSet.Add(ComponentType.ReadWrite<PrefabRef>());
		LifePathEventData lifePathEventData = default(LifePathEventData);
		lifePathEventData.m_ChirpArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		lifePathEventData.m_IsChirp = m_IsChirp;
		lifePathEventData.m_EventType = m_EventType;
		((EntityManager)(ref entityManager)).SetComponentData<LifePathEventData>(entity, lifePathEventData);
	}
}
