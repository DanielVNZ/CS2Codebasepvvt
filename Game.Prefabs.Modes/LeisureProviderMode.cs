using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class LeisureProviderMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public BuildingPrefab m_Prefab;

		public float m_EfficiencyMultifier;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			LeisureProvider component = m_ModeDatas[i].m_Prefab.GetComponent<LeisureProvider>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<LeisureProviderData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			LeisureProvider component = modeData.m_Prefab.GetComponent<LeisureProvider>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			LeisureProviderData componentData = ((EntityManager)(ref entityManager)).GetComponentData<LeisureProviderData>(entity);
			componentData.m_Efficiency = (int)((float)componentData.m_Efficiency * modeData.m_EfficiencyMultifier);
			((EntityManager)(ref entityManager)).SetComponentData<LeisureProviderData>(entity, componentData);
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
			LeisureProvider component = m_ModeDatas[i].m_Prefab.GetComponent<LeisureProvider>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			LeisureProviderData componentData = ((EntityManager)(ref entityManager)).GetComponentData<LeisureProviderData>(entity);
			componentData.m_Efficiency = component.m_Efficiency;
			((EntityManager)(ref entityManager)).SetComponentData<LeisureProviderData>(entity, componentData);
		}
	}
}
