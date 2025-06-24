using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class TelecomFacilityMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_RangeMultiplier;

		public float m_NetworkCapacityMultiplier;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			TelecomFacility component = m_ModeDatas[i].m_Prefab.GetComponent<TelecomFacility>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<TelecomFacilityData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			TelecomFacility component = modeData.m_Prefab.GetComponent<TelecomFacility>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			TelecomFacilityData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TelecomFacilityData>(entity);
			componentData.m_Range = (int)(componentData.m_Range * modeData.m_RangeMultiplier);
			componentData.m_NetworkCapacity = (int)(componentData.m_NetworkCapacity * modeData.m_NetworkCapacityMultiplier);
			((EntityManager)(ref entityManager)).SetComponentData<TelecomFacilityData>(entity, componentData);
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			TelecomFacility component = m_ModeDatas[i].m_Prefab.GetComponent<TelecomFacility>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			TelecomFacilityData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TelecomFacilityData>(entity);
			componentData.m_Range = component.m_Range;
			componentData.m_NetworkCapacity = component.m_NetworkCapacity;
			((EntityManager)(ref entityManager)).SetComponentData<TelecomFacilityData>(entity, componentData);
		}
	}
}
