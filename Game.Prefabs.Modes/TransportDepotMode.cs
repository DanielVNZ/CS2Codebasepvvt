using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class TransportDepotMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public int m_VehicleCapacity = 10;

		public float m_ProductionDuration;

		public float m_MaintenanceDuration;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			TransportDepot component = m_ModeDatas[i].m_Prefab.GetComponent<TransportDepot>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<TransportDepotData>(entity);
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
			TransportDepot component = modeData.m_Prefab.GetComponent<TransportDepot>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			TransportDepotData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TransportDepotData>(entity);
			componentData.m_VehicleCapacity = modeData.m_VehicleCapacity;
			componentData.m_ProductionDuration = modeData.m_ProductionDuration;
			componentData.m_MaintenanceDuration = modeData.m_MaintenanceDuration;
			((EntityManager)(ref entityManager)).SetComponentData<TransportDepotData>(entity, componentData);
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
			TransportDepot component = m_ModeDatas[i].m_Prefab.GetComponent<TransportDepot>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			TransportDepotData componentData = ((EntityManager)(ref entityManager)).GetComponentData<TransportDepotData>(entity);
			componentData.m_VehicleCapacity = component.m_VehicleCapacity;
			componentData.m_ProductionDuration = component.m_ProductionDuration;
			componentData.m_MaintenanceDuration = component.m_MaintenanceDuration;
			((EntityManager)(ref entityManager)).SetComponentData<TransportDepotData>(entity, componentData);
		}
	}
}
