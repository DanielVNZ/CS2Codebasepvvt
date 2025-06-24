using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.Objects;
using Game.UI.Editor;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { typeof(BuildingPrefab) })]
public class SignatureBuilding : ComponentBase
{
	public const int kStatLevel = 5;

	public ZonePrefab m_ZoneType;

	public int m_XPReward = 300;

	[CustomField(typeof(UIIconField))]
	public string m_UnlockEventImage;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_ZoneType);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<SignatureBuildingData>());
		components.Add(ComponentType.ReadWrite<SpawnableBuildingData>());
		components.Add(ComponentType.ReadWrite<PlaceableObjectData>());
		components.Add(ComponentType.ReadWrite<PlaceableInfoviewItem>());
		if ((Object)(object)m_ZoneType != (Object)null)
		{
			m_ZoneType.GetBuildingPrefabComponents(components, (BuildingPrefab)base.prefab, 5);
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<BuildingCondition>());
		components.Add(ComponentType.ReadWrite<Signature>());
		components.Add(ComponentType.ReadWrite<Game.Objects.UniqueObject>());
		if ((Object)(object)m_ZoneType != (Object)null)
		{
			m_ZoneType.GetBuildingArchetypeComponents(components, (BuildingPrefab)base.prefab, 5);
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		PlaceableObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableObjectData>(entity);
		componentData.m_XPReward = m_XPReward;
		if ((componentData.m_Flags & (PlacementFlags.Shoreline | PlacementFlags.Floating | PlacementFlags.Hovering)) == 0)
		{
			componentData.m_Flags |= PlacementFlags.OnGround;
		}
		componentData.m_Flags |= PlacementFlags.Unique;
		((EntityManager)(ref entityManager)).SetComponentData<PlaceableObjectData>(entity, componentData);
		if ((Object)(object)m_ZoneType != (Object)null)
		{
			m_ZoneType.InitializeBuilding(entityManager, entity, (BuildingPrefab)base.prefab, 5);
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		SpawnableBuildingData spawnableBuildingData = new SpawnableBuildingData
		{
			m_Level = 5
		};
		if ((Object)(object)m_ZoneType != (Object)null)
		{
			spawnableBuildingData.m_ZonePrefab = existingSystemManaged.GetEntity(m_ZoneType);
		}
		((EntityManager)(ref entityManager)).SetComponentData<SpawnableBuildingData>(entity, spawnableBuildingData);
	}
}
