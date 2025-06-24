using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class SchoolMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public PrefabBase m_Prefab;

		public float m_StudentCapacityMultifier;

		public float m_GraduationModifier;

		public sbyte m_StudentWellbeing;

		public sbyte m_StudentHealth;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			School component = m_ModeDatas[i].m_Prefab.GetComponent<School>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			((EntityManager)(ref entityManager)).GetComponentData<SchoolData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			School component = modeData.m_Prefab.GetComponent<School>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			SchoolData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SchoolData>(entity);
			componentData.m_StudentCapacity = (int)((float)componentData.m_StudentCapacity * modeData.m_StudentCapacityMultifier);
			componentData.m_GraduationModifier = modeData.m_GraduationModifier;
			componentData.m_StudentWellbeing = modeData.m_StudentWellbeing;
			componentData.m_StudentHealth = modeData.m_StudentHealth;
			((EntityManager)(ref entityManager)).SetComponentData<SchoolData>(entity, componentData);
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			School component = m_ModeDatas[i].m_Prefab.GetComponent<School>();
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(component.prefab);
			SchoolData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SchoolData>(entity);
			componentData.m_StudentCapacity = component.m_StudentCapacity;
			componentData.m_GraduationModifier = component.m_GraduationModifier;
			componentData.m_StudentWellbeing = component.m_StudentWellbeing;
			componentData.m_StudentHealth = component.m_StudentHealth;
			((EntityManager)(ref entityManager)).SetComponentData<SchoolData>(entity, componentData);
		}
	}
}
