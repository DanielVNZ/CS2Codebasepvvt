using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[] { })]
public class MultipleUnitTrainFrontPrefab : TrainPrefab
{
	public int m_MinMultipleUnitCount = 1;

	public int m_MaxMultipleUnitCount = 1;

	public MultipleUnitTrainCarriageInfo[] m_Carriages;

	public bool m_AddReversedEndCarriage = true;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Carriages != null)
		{
			for (int i = 0; i < m_Carriages.Length; i++)
			{
				prefabs.Add(m_Carriages[i].m_Carriage);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TrainEngineData>());
		components.Add(ComponentType.ReadWrite<MultipleUnitTrainData>());
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
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<TrainEngineData>(entity, new TrainEngineData(m_MinMultipleUnitCount, m_MaxMultipleUnitCount));
		DynamicBuffer<VehicleCarriageElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<VehicleCarriageElement>(entity, false);
		if (m_Carriages != null)
		{
			PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
			for (int i = 0; i < m_Carriages.Length; i++)
			{
				MultipleUnitTrainCarriageInfo multipleUnitTrainCarriageInfo = m_Carriages[i];
				Entity entity2 = existingSystemManaged.GetEntity(multipleUnitTrainCarriageInfo.m_Carriage);
				buffer.Add(new VehicleCarriageElement(entity2, multipleUnitTrainCarriageInfo.m_MinCount, multipleUnitTrainCarriageInfo.m_MaxCount, multipleUnitTrainCarriageInfo.m_Direction));
			}
		}
		if (m_AddReversedEndCarriage)
		{
			buffer.Add(new VehicleCarriageElement(entity, 1, 1, VehicleCarriageDirection.Reversed));
		}
	}
}
