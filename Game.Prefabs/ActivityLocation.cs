using System;
using System.Collections.Generic;
using Game.Net;
using Game.Objects;
using Game.Rendering;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[]
{
	typeof(StaticObjectPrefab),
	typeof(MarkerObjectPrefab),
	typeof(VehiclePrefab)
})]
public class ActivityLocation : ComponentBase
{
	[Serializable]
	public class LocationInfo
	{
		public ActivityLocationPrefab m_Activity;

		public float3 m_Position = float3.zero;

		public quaternion m_Rotation = quaternion.identity;
	}

	public LocationInfo[] m_Locations;

	public NetInvertMode m_InvertWhen;

	public string m_AnimatedPropName;

	public bool m_RequireAuthorization;

	public override bool ignoreUnlockDependencies => true;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Locations != null)
		{
			for (int i = 0; i < m_Locations.Length; i++)
			{
				prefabs.Add(m_Locations[i].m_Activity);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!(base.prefab is VehiclePrefab) && !(base.prefab is BuildingPrefab))
		{
			components.Add(ComponentType.ReadWrite<SpawnLocationData>());
		}
		components.Add(ComponentType.ReadWrite<ActivityLocationElement>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!(base.prefab is VehiclePrefab) && !(base.prefab is BuildingPrefab))
		{
			components.Add(ComponentType.ReadWrite<Game.Objects.SpawnLocation>());
			components.Add(ComponentType.ReadWrite<Game.Objects.ActivityLocation>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		SpawnLocationData spawnLocationData = default(SpawnLocationData);
		spawnLocationData.m_ConnectionType = RouteConnectionType.Pedestrian;
		spawnLocationData.m_ActivityMask = default(ActivityMask);
		spawnLocationData.m_RoadTypes = RoadTypes.None;
		spawnLocationData.m_TrackTypes = TrackTypes.None;
		spawnLocationData.m_RequireAuthorization = m_RequireAuthorization;
		spawnLocationData.m_HangaroundOnLane = false;
		if (m_Locations != null && m_Locations.Length != 0)
		{
			DynamicBuffer<ActivityLocationElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ActivityLocationElement>(entity, false);
			buffer.ResizeUninitialized(m_Locations.Length);
			PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
			AnimatedPropID propID = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<AnimatedSystem>().GetPropID(m_AnimatedPropName);
			for (int i = 0; i < m_Locations.Length; i++)
			{
				LocationInfo locationInfo = m_Locations[i];
				ActivityLocationElement activityLocationElement = new ActivityLocationElement
				{
					m_Prefab = existingSystemManaged.GetEntity(locationInfo.m_Activity),
					m_Position = locationInfo.m_Position,
					m_Rotation = locationInfo.m_Rotation,
					m_PropID = propID
				};
				switch (m_InvertWhen)
				{
				case NetInvertMode.LefthandTraffic:
					activityLocationElement.m_ActivityFlags |= ActivityFlags.InvertLefthandTraffic;
					break;
				case NetInvertMode.RighthandTraffic:
					activityLocationElement.m_ActivityFlags |= ActivityFlags.InvertRighthandTraffic;
					break;
				case NetInvertMode.Always:
					activityLocationElement.m_ActivityFlags |= ActivityFlags.InvertLefthandTraffic | ActivityFlags.InvertRighthandTraffic;
					break;
				}
				ActivityLocationData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ActivityLocationData>(activityLocationElement.m_Prefab);
				activityLocationElement.m_ActivityMask = componentData.m_ActivityMask;
				spawnLocationData.m_ActivityMask.m_Mask |= componentData.m_ActivityMask.m_Mask;
				buffer[i] = activityLocationElement;
			}
		}
		else
		{
			ComponentBase.baseLog.ErrorFormat((Object)(object)base.prefab, "Empty activity location array: {0}", (object)((Object)base.prefab).name);
		}
		if (!(base.prefab is VehiclePrefab) && !(base.prefab is BuildingPrefab))
		{
			((EntityManager)(ref entityManager)).SetComponentData<SpawnLocationData>(entity, spawnLocationData);
		}
	}
}
