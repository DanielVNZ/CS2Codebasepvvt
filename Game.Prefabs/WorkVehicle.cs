using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Economy;
using Game.Pathfind;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[]
{
	typeof(CarPrefab),
	typeof(CarTrailerPrefab),
	typeof(WatercraftPrefab)
})]
public class WorkVehicle : ComponentBase
{
	public VehicleWorkType m_WorkType;

	public MapFeature m_MapFeature = MapFeature.None;

	public ResourceInEditor[] m_Resources;

	public float m_MaxWorkAmount = 30000f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WorkVehicleData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Vehicles.WorkVehicle>());
		components.Add(ComponentType.ReadWrite<PathInformation>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		Resource resource = Resource.NoResource;
		if (m_Resources != null)
		{
			for (int i = 0; i < m_Resources.Length; i++)
			{
				resource |= EconomyUtils.GetResource(m_Resources[i]);
			}
		}
		WorkVehicleData workVehicleData = default(WorkVehicleData);
		workVehicleData.m_WorkType = m_WorkType;
		workVehicleData.m_MapFeature = m_MapFeature;
		workVehicleData.m_MaxWorkAmount = m_MaxWorkAmount;
		workVehicleData.m_Resources = resource;
		((EntityManager)(ref entityManager)).SetComponentData<WorkVehicleData>(entity, workVehicleData);
		if (((EntityManager)(ref entityManager)).HasComponent<CarData>(entity))
		{
			((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(12));
		}
	}
}
