using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Zones/", new Type[] { typeof(ZonePrefab) })]
public class ZoneServiceConsumption : ComponentBase, IZoneBuildingComponent
{
	public float m_Upkeep;

	public float m_ElectricityConsumption;

	public float m_WaterConsumption;

	public float m_GarbageAccumulation;

	public float m_TelecomNeed;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ZoneServiceConsumptionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<ZoneServiceConsumptionData>(entity, new ZoneServiceConsumptionData
		{
			m_Upkeep = m_Upkeep,
			m_ElectricityConsumption = m_ElectricityConsumption,
			m_WaterConsumption = m_WaterConsumption,
			m_GarbageAccumulation = m_GarbageAccumulation,
			m_TelecomNeed = m_TelecomNeed
		});
	}

	public void GetBuildingPrefabComponents(HashSet<ComponentType> components, BuildingPrefab buildingPrefab, byte level)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ConsumptionData>());
	}

	public void GetBuildingArchetypeComponents(HashSet<ComponentType> components, BuildingPrefab buildingPrefab, byte level)
	{
		if (!buildingPrefab.Has<ServiceConsumption>())
		{
			GetBuildingConsumptionData().AddArchetypeComponents(components);
		}
	}

	public void InitializeBuilding(EntityManager entityManager, Entity entity, BuildingPrefab buildingPrefab, byte level)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!buildingPrefab.Has<ServiceConsumption>())
		{
			((EntityManager)(ref entityManager)).SetComponentData<ConsumptionData>(entity, GetBuildingConsumptionData());
		}
	}

	private ConsumptionData GetBuildingConsumptionData()
	{
		return new ConsumptionData
		{
			m_Upkeep = 0,
			m_ElectricityConsumption = m_ElectricityConsumption,
			m_WaterConsumption = m_WaterConsumption,
			m_GarbageAccumulation = m_GarbageAccumulation,
			m_TelecomNeed = m_TelecomNeed
		};
	}
}
