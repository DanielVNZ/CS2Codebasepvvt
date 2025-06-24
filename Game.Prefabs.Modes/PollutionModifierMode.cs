using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class PollutionModifierMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		[Range(-1f, 1f)]
		public float m_GroundPollutionMultiplier;

		[Range(-1f, 1f)]
		public float m_AirPollutionMultiplier;

		[Range(-1f, 1f)]
		public float m_NoisePollutionMultiplier;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			PollutionModifier component = m_ModeDatas[i].m_Prefab.GetComponent<PollutionModifier>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<PollutionModifierData>(entity);
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
			PollutionModifier component = modeData.m_Prefab.GetComponent<PollutionModifier>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			PollutionModifierData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PollutionModifierData>(entity);
			componentData.m_GroundPollutionMultiplier = modeData.m_GroundPollutionMultiplier;
			componentData.m_AirPollutionMultiplier = modeData.m_AirPollutionMultiplier;
			componentData.m_NoisePollutionMultiplier = modeData.m_NoisePollutionMultiplier;
			((EntityManager)(ref entityManager)).SetComponentData<PollutionModifierData>(entity, componentData);
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
			PollutionModifier component = m_ModeDatas[i].m_Prefab.GetComponent<PollutionModifier>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			PollutionModifierData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PollutionModifierData>(entity);
			componentData.m_GroundPollutionMultiplier = component.m_GroundPollutionMultiplier;
			componentData.m_AirPollutionMultiplier = component.m_AirPollutionMultiplier;
			componentData.m_NoisePollutionMultiplier = component.m_NoisePollutionMultiplier;
			((EntityManager)(ref entityManager)).SetComponentData<PollutionModifierData>(entity, componentData);
		}
	}
}
