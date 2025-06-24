using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Prefab/", new Type[] { })]
public class MilestonesMode : LocalModePrefab
{
	[Serializable]
	public class ModeData
	{
		public MilestonePrefab m_Prefab;

		public int m_Reward;

		public int m_DevTreePoints;

		public int m_MapTiles;

		public int m_LoanLimit;

		public int m_XpRequried;
	}

	public ModeData[] m_ModeDatas;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			MilestonePrefab milestonePrefab = m_ModeDatas[i].m_Prefab;
			if ((Object)(object)milestonePrefab == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(milestonePrefab);
			((EntityManager)(ref entityManager)).GetComponentData<MilestoneData>(entity);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			ModeData modeData = m_ModeDatas[i];
			MilestonePrefab milestonePrefab = modeData.m_Prefab;
			if ((Object)(object)milestonePrefab == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(milestonePrefab);
			MilestoneData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MilestoneData>(entity);
			componentData.m_Reward = modeData.m_Reward;
			componentData.m_DevTreePoints = modeData.m_DevTreePoints;
			componentData.m_MapTiles = modeData.m_MapTiles;
			componentData.m_LoanLimit = modeData.m_LoanLimit;
			componentData.m_XpRequried = modeData.m_XpRequried;
			((EntityManager)(ref entityManager)).SetComponentData<MilestoneData>(entity, componentData);
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_ModeDatas.Length; i++)
		{
			MilestonePrefab milestonePrefab = m_ModeDatas[i].m_Prefab;
			if ((Object)(object)milestonePrefab == (Object)null)
			{
				ComponentBase.baseLog.Critical((object)$"Target not found {this}");
				continue;
			}
			Entity entity = prefabSystem.GetEntity(milestonePrefab);
			MilestoneData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MilestoneData>(entity);
			componentData.m_Reward = milestonePrefab.m_Reward;
			componentData.m_DevTreePoints = milestonePrefab.m_DevTreePoints;
			componentData.m_MapTiles = milestonePrefab.m_MapTiles;
			componentData.m_LoanLimit = milestonePrefab.m_LoanLimit;
			componentData.m_XpRequried = milestonePrefab.m_XpRequried;
			((EntityManager)(ref entityManager)).SetComponentData<MilestoneData>(entity, componentData);
		}
	}
}
