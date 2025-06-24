using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class PowerPlantMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_ElectricityProductionMultiplier;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			PowerPlant component = m_ModeDatas[i].m_Prefab.GetComponent<PowerPlant>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<PowerPlantData>(entity);
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
			PowerPlant component = modeData.m_Prefab.GetComponent<PowerPlant>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			PowerPlantData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PowerPlantData>(entity);
			componentData.m_ElectricityProduction = (int)((float)componentData.m_ElectricityProduction * modeData.m_ElectricityProductionMultiplier);
			((EntityManager)(ref entityManager)).SetComponentData<PowerPlantData>(entity, componentData);
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
			PowerPlant component = m_ModeDatas[i].m_Prefab.GetComponent<PowerPlant>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			PowerPlantData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PowerPlantData>(entity);
			componentData.m_ElectricityProduction = component.m_ElectricityProduction;
			((EntityManager)(ref entityManager)).SetComponentData<PowerPlantData>(entity, componentData);
		}
	}
}
