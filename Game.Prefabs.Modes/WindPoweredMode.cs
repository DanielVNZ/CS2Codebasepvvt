using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class WindPoweredMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_MaximumWindMultiplier;

		public float m_ProductionMultiplier;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			WindPowered component = m_ModeDatas[i].m_Prefab.GetComponent<WindPowered>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<WindPoweredData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			WindPowered component = modeData.m_Prefab.GetComponent<WindPowered>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			WindPoweredData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WindPoweredData>(entity);
			componentData.m_MaximumWind *= modeData.m_MaximumWindMultiplier;
			componentData.m_Production = (int)((float)componentData.m_Production * modeData.m_ProductionMultiplier);
			((EntityManager)(ref entityManager)).SetComponentData<WindPoweredData>(entity, componentData);
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
			WindPowered component = m_ModeDatas[i].m_Prefab.GetComponent<WindPowered>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			WindPoweredData componentData = ((EntityManager)(ref entityManager)).GetComponentData<WindPoweredData>(entity);
			componentData.m_MaximumWind = component.m_MaximumWind;
			componentData.m_Production = component.m_Production;
			((EntityManager)(ref entityManager)).SetComponentData<WindPoweredData>(entity, componentData);
		}
	}
}
