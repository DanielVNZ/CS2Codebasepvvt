using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class ServiceCoverageMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_Range = 1000f;

		public float m_Capacity = 3000f;

		public float m_Magnitude = 1f;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ServiceCoverage component = m_ModeDatas[i].m_Prefab.GetComponent<ServiceCoverage>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<CoverageData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			ServiceCoverage component = modeData.m_Prefab.GetComponent<ServiceCoverage>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			CoverageData componentData = ((EntityManager)(ref entityManager)).GetComponentData<CoverageData>(entity);
			componentData.m_Capacity = modeData.m_Capacity;
			componentData.m_Range = modeData.m_Range;
			componentData.m_Magnitude = modeData.m_Magnitude;
			((EntityManager)(ref entityManager)).SetComponentData<CoverageData>(entity, componentData);
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ServiceCoverage component = m_ModeDatas[i].m_Prefab.GetComponent<ServiceCoverage>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			CoverageData componentData = ((EntityManager)(ref entityManager)).GetComponentData<CoverageData>(entity);
			componentData.m_Range = component.m_Range;
			componentData.m_Capacity = component.m_Capacity;
			componentData.m_Magnitude = component.m_Magnitude;
			((EntityManager)(ref entityManager)).SetComponentData<CoverageData>(entity, componentData);
		}
	}
}
