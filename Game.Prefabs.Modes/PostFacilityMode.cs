using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class PostFacilityMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_MailStorageCapacityMultifier;

		public float m_MailBoxCapacityMultifier;

		public float m_SortingRateMultifier;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			PostFacility component = m_ModeDatas[i].m_Prefab.GetComponent<PostFacility>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<PostFacilityData>(entity);
			if (((EntityManager)(ref entityManager)).HasComponent<MailBoxData>(entity))
			{
				((EntityManager)(ref entityManager)).GetComponentData<MailBoxData>(entity);
			}
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			PostFacility component = modeData.m_Prefab.GetComponent<PostFacility>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			PostFacilityData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PostFacilityData>(entity);
			componentData.m_MailCapacity = (int)((float)componentData.m_MailCapacity * modeData.m_MailStorageCapacityMultifier);
			componentData.m_SortingRate = (int)((float)componentData.m_SortingRate * modeData.m_SortingRateMultifier);
			((EntityManager)(ref entityManager)).SetComponentData<PostFacilityData>(entity, componentData);
			if (((EntityManager)(ref entityManager)).HasComponent<MailBoxData>(entity))
			{
				MailBoxData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<MailBoxData>(entity);
				componentData2.m_MailCapacity = (int)((float)componentData2.m_MailCapacity * modeData.m_MailBoxCapacityMultifier);
				((EntityManager)(ref entityManager)).SetComponentData<MailBoxData>(entity, componentData2);
			}
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			PostFacility component = m_ModeDatas[i].m_Prefab.GetComponent<PostFacility>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			PostFacilityData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PostFacilityData>(entity);
			componentData.m_MailCapacity = component.m_MailStorageCapacity;
			componentData.m_SortingRate = component.m_SortingRate;
			((EntityManager)(ref entityManager)).SetComponentData<PostFacilityData>(entity, componentData);
			if (((EntityManager)(ref entityManager)).HasComponent<MailBoxData>(entity))
			{
				MailBoxData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<MailBoxData>(entity);
				componentData2.m_MailCapacity = component.m_MailBoxCapacity;
				((EntityManager)(ref entityManager)).SetComponentData<MailBoxData>(entity, componentData2);
			}
		}
	}
}
