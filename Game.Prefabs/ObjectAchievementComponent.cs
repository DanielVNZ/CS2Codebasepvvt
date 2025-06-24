using System;
using System.Collections.Generic;
using Colossal.PSI.Common;
using Game.Achievements;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Achievements/", new Type[] { })]
public class ObjectAchievementComponent : ComponentBase
{
	[Serializable]
	public struct ObjectAchievementSetup
	{
		public AchievementId m_ID;

		public bool m_BypassCounter;

		public bool m_AbsoluteCounter;
	}

	public ObjectAchievementSetup[] m_Achievements;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ObjectAchievement>());
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ObjectAchievementData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		DynamicBuffer<ObjectAchievementData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ObjectAchievementData>(entity, false);
		ObjectAchievementSetup[] achievements = m_Achievements;
		for (int i = 0; i < achievements.Length; i++)
		{
			ObjectAchievementSetup objectAchievementSetup = achievements[i];
			buffer.Add(new ObjectAchievementData
			{
				m_ID = objectAchievementSetup.m_ID,
				m_BypassCounter = objectAchievementSetup.m_BypassCounter,
				m_AbsoluteCounter = objectAchievementSetup.m_AbsoluteCounter
			});
		}
	}
}
