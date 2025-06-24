using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class BatteryMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_PowerOutputMultiplier;

		public float m_CapacityMultiplier;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			Battery component = m_ModeDatas[i].m_Prefab.GetComponent<Battery>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<BatteryData>(entity);
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
			Battery component = modeData.m_Prefab.GetComponent<Battery>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			BatteryData componentData = ((EntityManager)(ref entityManager)).GetComponentData<BatteryData>(entity);
			componentData.m_PowerOutput = (int)((float)componentData.m_PowerOutput * modeData.m_PowerOutputMultiplier);
			componentData.m_Capacity = (int)((float)componentData.m_Capacity * modeData.m_CapacityMultiplier);
			((EntityManager)(ref entityManager)).SetComponentData<BatteryData>(entity, componentData);
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
			Battery component = m_ModeDatas[i].m_Prefab.GetComponent<Battery>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			BatteryData componentData = ((EntityManager)(ref entityManager)).GetComponentData<BatteryData>(entity);
			componentData.m_PowerOutput = component.m_PowerOutput;
			componentData.m_Capacity = component.m_Capacity;
			((EntityManager)(ref entityManager)).SetComponentData<BatteryData>(entity, componentData);
		}
	}
}
