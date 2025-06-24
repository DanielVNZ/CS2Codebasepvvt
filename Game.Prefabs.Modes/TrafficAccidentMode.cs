using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class TrafficAccidentMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public EventPrefab m_Prefab;

		public float m_OccurrenceProbability;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			TrafficAccident component = m_ModeDatas[i].m_Prefab.GetComponent<TrafficAccident>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<TrafficAccidentData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			TrafficAccident component = modeData.m_Prefab.GetComponent<TrafficAccident>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			TrafficAccidentData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TrafficAccidentData>(entity);
			componentData.m_OccurenceProbability = modeData.m_OccurrenceProbability;
			((EntityManager)(ref entityManager)).SetComponentData<TrafficAccidentData>(entity, componentData);
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			TrafficAccident component = m_ModeDatas[i].m_Prefab.GetComponent<TrafficAccident>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			TrafficAccidentData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TrafficAccidentData>(entity);
			componentData.m_OccurenceProbability = component.m_OccurrenceProbability;
			((EntityManager)(ref entityManager)).SetComponentData<TrafficAccidentData>(entity, componentData);
		}
	}
}
