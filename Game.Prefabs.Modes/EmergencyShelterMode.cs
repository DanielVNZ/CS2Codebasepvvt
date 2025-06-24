using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class EmergencyShelterMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_ShelterCapacityMultiplier;

		public int m_VehicleCapacity;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			GroundWaterPowered component = m_ModeDatas[i].m_Prefab.GetComponent<GroundWaterPowered>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<EmergencyShelterData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			GroundWaterPowered component = modeData.m_Prefab.GetComponent<GroundWaterPowered>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			EmergencyShelterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<EmergencyShelterData>(entity);
			componentData.m_ShelterCapacity = (int)((float)componentData.m_ShelterCapacity * modeData.m_ShelterCapacityMultiplier);
			componentData.m_VehicleCapacity = modeData.m_VehicleCapacity;
			((EntityManager)(ref entityManager)).SetComponentData<EmergencyShelterData>(entity, componentData);
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
			EmergencyShelter component = m_ModeDatas[i].m_Prefab.GetComponent<EmergencyShelter>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			EmergencyShelterData componentData = ((EntityManager)(ref entityManager)).GetComponentData<EmergencyShelterData>(entity);
			componentData.m_ShelterCapacity = component.m_ShelterCapacity;
			componentData.m_VehicleCapacity = component.m_VehicleCapacity;
			((EntityManager)(ref entityManager)).SetComponentData<EmergencyShelterData>(entity, componentData);
		}
	}
}
