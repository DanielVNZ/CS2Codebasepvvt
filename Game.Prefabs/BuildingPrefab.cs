using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Effects;
using Game.Net;
using Game.Objects;
using Game.Policies;
using Game.Routes;
using Game.Simulation;
using Game.UI.Editor;
using Game.UI.Widgets;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { })]
public class BuildingPrefab : StaticObjectPrefab
{
	public BuildingAccessType m_AccessType;

	[CustomField(typeof(BuildingLotWidthField))]
	public int m_LotWidth = 4;

	[CustomField(typeof(BuildingLotDepthField))]
	public int m_LotDepth = 4;

	public int lotSize => m_LotWidth * m_LotDepth;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<BuildingData>());
		components.Add(ComponentType.ReadWrite<PlaceableObjectData>());
		components.Add(ComponentType.ReadWrite<BuildingTerraformData>());
		components.Add(ComponentType.ReadWrite<Effect>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Building>());
		components.Add(ComponentType.ReadWrite<CitizenPresence>());
		components.Add(ComponentType.ReadWrite<SpawnLocationElement>());
		components.Add(ComponentType.ReadWrite<CurrentDistrict>());
		components.Add(ComponentType.ReadWrite<UpdateFrame>());
		components.Add(ComponentType.ReadWrite<Game.Objects.Color>());
		components.Add(ComponentType.ReadWrite<Game.Objects.Surface>());
		components.Add(ComponentType.ReadWrite<BuildingModifier>());
		components.Add(ComponentType.ReadWrite<Policy>());
		components.Add(ComponentType.ReadWrite<Game.Net.SubLane>());
		components.Add(ComponentType.ReadWrite<Game.Objects.SubObject>());
		components.Add(ComponentType.ReadWrite<Game.Buildings.Lot>());
		components.Add(ComponentType.ReadWrite<EnabledEffect>());
	}

	protected override void RefreshArchetype(EntityManager entityManager, Entity entity)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		if (((EntityManager)(ref entityManager)).HasComponent<BuildingUpgradeElement>(entity))
		{
			hashSet.Add(ComponentType.ReadWrite<InstalledUpgrade>());
			hashSet.Add(ComponentType.ReadWrite<Game.Net.SubNet>());
			hashSet.Add(ComponentType.ReadWrite<SubRoute>());
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		((EntityManager)(ref entityManager)).SetComponentData<ObjectData>(entity, new ObjectData
		{
			m_Archetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet))
		});
	}

	public void AddUpgrade(EntityManager entityManager, ServiceUpgrade upgrade)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>().TryGetEntity(this, out var entity))
		{
			if (!((EntityManager)(ref entityManager)).HasComponent<BuildingUpgradeElement>(entity))
			{
				((EntityManager)(ref entityManager)).AddBuffer<BuildingUpgradeElement>(entity);
				ObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ObjectData>(entity);
				if (((EntityArchetype)(ref componentData.m_Archetype)).Valid)
				{
					RefreshArchetype(entityManager, entity);
				}
			}
			return;
		}
		throw new Exception("Building prefab entity not found for upgrade!");
	}
}
