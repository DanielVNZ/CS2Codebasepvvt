using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class ServiceConsumption : ComponentBase, IServiceUpgrade
{
	public int m_Upkeep;

	public int m_ElectricityConsumption;

	public int m_WaterConsumption;

	public int m_GarbageAccumulation;

	public float m_TelecomNeed;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ConsumptionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		if (!base.prefab.Has<ServiceUpgrade>())
		{
			GetConsumptionData().AddArchetypeComponents(components);
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		GetConsumptionData().AddArchetypeComponents(components);
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<ConsumptionData>(entity, GetConsumptionData());
	}

	private ConsumptionData GetConsumptionData()
	{
		return new ConsumptionData
		{
			m_Upkeep = m_Upkeep,
			m_ElectricityConsumption = m_ElectricityConsumption,
			m_WaterConsumption = m_WaterConsumption,
			m_GarbageAccumulation = m_GarbageAccumulation,
			m_TelecomNeed = m_TelecomNeed
		};
	}
}
