using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[] { })]
public class TrainEnginePrefab : TrainPrefab
{
	public int m_MinEngineCount = 2;

	public int m_MaxEngineCount = 2;

	public int m_MinCarriagesPerEngine = 5;

	public int m_MaxCarriagesPerEngine = 5;

	public TrainCarPrefab m_Tender;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TrainEngineData>());
		components.Add(ComponentType.ReadWrite<VehicleCarriageElement>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<TrainEngineData>(entity, new TrainEngineData(m_MinEngineCount, m_MaxEngineCount));
		DynamicBuffer<VehicleCarriageElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<VehicleCarriageElement>(entity, false);
		if ((Object)(object)m_Tender != (Object)null)
		{
			Entity entity2 = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>().GetEntity(m_Tender);
			buffer.Add(new VehicleCarriageElement(entity2, 1, 1, VehicleCarriageDirection.Default));
		}
		buffer.Add(new VehicleCarriageElement(Entity.Null, m_MinCarriagesPerEngine, m_MaxCarriagesPerEngine, VehicleCarriageDirection.Random));
	}
}
