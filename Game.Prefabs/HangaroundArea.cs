using System;
using System.Collections.Generic;
using Game.Areas;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[]
{
	typeof(LotPrefab),
	typeof(SpacePrefab)
})]
public class HangaroundArea : ComponentBase
{
	public ActivityType[] m_Activities;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<SpawnLocationData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<HangaroundLocation>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		NavigationArea component = GetComponent<NavigationArea>();
		SpawnLocationData spawnLocationData = default(SpawnLocationData);
		spawnLocationData.m_ConnectionType = component.m_ConnectionType;
		spawnLocationData.m_TrackTypes = component.m_TrackTypes;
		spawnLocationData.m_RoadTypes = component.m_RoadTypes;
		spawnLocationData.m_RequireAuthorization = false;
		spawnLocationData.m_HangaroundOnLane = false;
		if (m_Activities != null && m_Activities.Length != 0)
		{
			spawnLocationData.m_ActivityMask = default(ActivityMask);
			for (int i = 0; i < m_Activities.Length; i++)
			{
				spawnLocationData.m_ActivityMask.m_Mask |= new ActivityMask(m_Activities[i]).m_Mask;
			}
		}
		else
		{
			spawnLocationData.m_ActivityMask = new ActivityMask(ActivityType.Standing);
		}
		((EntityManager)(ref entityManager)).SetComponentData<SpawnLocationData>(entity, spawnLocationData);
	}
}
