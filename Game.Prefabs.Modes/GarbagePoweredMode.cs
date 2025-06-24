using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class GarbagePoweredMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_ProductionPerUnitMultiplier;

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
			GarbagePowered component = m_ModeDatas[i].m_Prefab.GetComponent<GarbagePowered>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<GarbagePoweredData>(entity);
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
			GarbagePowered component = modeData.m_Prefab.GetComponent<GarbagePowered>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			GarbagePoweredData componentData = ((EntityManager)(ref entityManager)).GetComponentData<GarbagePoweredData>(entity);
			componentData.m_ProductionPerUnit = (int)(componentData.m_ProductionPerUnit * modeData.m_ProductionPerUnitMultiplier);
			componentData.m_Capacity = (int)((float)componentData.m_Capacity * modeData.m_CapacityMultiplier);
			((EntityManager)(ref entityManager)).SetComponentData<GarbagePoweredData>(entity, componentData);
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
			GarbagePowered component = m_ModeDatas[i].m_Prefab.GetComponent<GarbagePowered>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			GarbagePoweredData componentData = ((EntityManager)(ref entityManager)).GetComponentData<GarbagePoweredData>(entity);
			componentData.m_ProductionPerUnit = component.m_ProductionPerUnit;
			componentData.m_Capacity = component.m_Capacity;
			((EntityManager)(ref entityManager)).SetComponentData<GarbagePoweredData>(entity, componentData);
		}
	}
}
