using System;
using System.Collections.Generic;
using Colossal.PSI.Common;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Achievements/", new Type[] { typeof(BuildingPrefab) })]
public class AchievementFilter : ComponentBase
{
	public AchievementId[] m_ValidFor;

	public AchievementId[] m_NotValidFor;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AchievementFilterData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		DynamicBuffer<AchievementFilterData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AchievementFilterData>(entity, false);
		if (m_ValidFor != null)
		{
			for (int i = 0; i < m_ValidFor.Length; i++)
			{
				buffer.Add(new AchievementFilterData
				{
					m_AchievementID = m_ValidFor[i],
					m_Allow = true
				});
			}
		}
		if (m_NotValidFor != null)
		{
			for (int j = 0; j < m_NotValidFor.Length; j++)
			{
				buffer.Add(new AchievementFilterData
				{
					m_AchievementID = m_NotValidFor[j],
					m_Allow = false
				});
			}
		}
	}
}
