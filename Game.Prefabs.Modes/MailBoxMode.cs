using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class MailBoxMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public ObjectPrefab m_Prefab;

		public float m_MailCapacityMultifier;

		public float m_ComfortFactor;
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
			MailBox component = m_ModeDatas[i].m_Prefab.GetComponent<MailBox>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<MailBoxData>(entity);
			if (((EntityManager)(ref entityManager)).HasComponent<TransportStopData>(entity))
			{
				((EntityManager)(ref entityManager)).GetComponentData<TransportStopData>(entity);
			}
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			MailBox component = modeData.m_Prefab.GetComponent<MailBox>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			MailBoxData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MailBoxData>(entity);
			componentData.m_MailCapacity = (int)((float)componentData.m_MailCapacity * modeData.m_MailCapacityMultifier);
			((EntityManager)(ref entityManager)).SetComponentData<MailBoxData>(entity, componentData);
			if (((EntityManager)(ref entityManager)).HasComponent<TransportStopData>(entity))
			{
				TransportStopData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<TransportStopData>(entity);
				componentData2.m_ComfortFactor = modeData.m_ComfortFactor;
				((EntityManager)(ref entityManager)).SetComponentData<TransportStopData>(entity, componentData2);
			}
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			MailBox component = m_ModeDatas[i].m_Prefab.GetComponent<MailBox>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			MailBoxData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MailBoxData>(entity);
			componentData.m_MailCapacity = component.m_MailCapacity;
			((EntityManager)(ref entityManager)).SetComponentData<MailBoxData>(entity, componentData);
			if (((EntityManager)(ref entityManager)).HasComponent<TransportStopData>(entity))
			{
				TransportStopData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<TransportStopData>(entity);
				componentData2.m_ComfortFactor = component.m_ComfortFactor;
				((EntityManager)(ref entityManager)).SetComponentData<TransportStopData>(entity, componentData2);
			}
		}
	}
}
