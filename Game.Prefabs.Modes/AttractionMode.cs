using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class AttractionMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public BuildingPrefab m_Prefab;

		public float m_AttractivenessMultifier;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			Attraction component = m_ModeDatas[i].m_Prefab.GetComponent<Attraction>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<AttractionData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			Attraction component = modeData.m_Prefab.GetComponent<Attraction>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			AttractionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AttractionData>(entity);
			componentData.m_Attractiveness *= (int)((float)componentData.m_Attractiveness * modeData.m_AttractivenessMultifier);
			((EntityManager)(ref entityManager)).SetComponentData<AttractionData>(entity, componentData);
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
			Attraction component = m_ModeDatas[i].m_Prefab.GetComponent<Attraction>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			AttractionData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AttractionData>(entity);
			componentData.m_Attractiveness = component.m_Attractiveness;
			((EntityManager)(ref entityManager)).SetComponentData<AttractionData>(entity, componentData);
		}
	}
}
