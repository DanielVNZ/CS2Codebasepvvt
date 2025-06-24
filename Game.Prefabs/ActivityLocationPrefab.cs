using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { })]
public class ActivityLocationPrefab : TransformPrefab
{
	public ActivityType[] m_Activities;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ActivityLocationData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		ActivityLocationData activityLocationData = default(ActivityLocationData);
		activityLocationData.m_ActivityMask = default(ActivityMask);
		if (m_Activities != null)
		{
			for (int i = 0; i < m_Activities.Length; i++)
			{
				activityLocationData.m_ActivityMask.m_Mask |= new ActivityMask(m_Activities[i]).m_Mask;
			}
		}
		((EntityManager)(ref entityManager)).SetComponentData<ActivityLocationData>(entity, activityLocationData);
	}
}
