using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class PrisonMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_PrisonVanCapacityMultiplier;

		public sbyte m_PrisonerWellbeing;

		public sbyte m_PrisonerHealth;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			Prison component = m_ModeDatas[i].m_Prefab.GetComponent<Prison>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<PrisonData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			Prison component = modeData.m_Prefab.GetComponent<Prison>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			PrisonData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrisonData>(entity);
			componentData.m_PrisonVanCapacity = (int)((float)componentData.m_PrisonVanCapacity * modeData.m_PrisonVanCapacityMultiplier);
			componentData.m_PrisonerWellbeing = modeData.m_PrisonerWellbeing;
			componentData.m_PrisonerHealth = modeData.m_PrisonerHealth;
			((EntityManager)(ref entityManager)).SetComponentData<PrisonData>(entity, componentData);
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
			Prison component = m_ModeDatas[i].m_Prefab.GetComponent<Prison>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			PrisonData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrisonData>(entity);
			componentData.m_PrisonVanCapacity = component.m_PrisonVanCapacity;
			componentData.m_PrisonerWellbeing = component.m_PrisonerWellbeing;
			componentData.m_PrisonerHealth = component.m_PrisonerHealth;
			((EntityManager)(ref entityManager)).SetComponentData<PrisonData>(entity, componentData);
		}
	}
}
