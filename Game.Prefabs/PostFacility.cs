using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Buildings;
using Game.Economy;
using Game.Routes;
using Game.Simulation;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/CityServices/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class PostFacility : ComponentBase, IServiceUpgrade
{
	public int m_PostVanCapacity = 10;

	public int m_PostTruckCapacity;

	public int m_MailStorageCapacity = 100000;

	public int m_MailBoxCapacity = 10000;

	public int m_SortingRate;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PostFacilityData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
		if (m_MailBoxCapacity > 0)
		{
			components.Add(ComponentType.ReadWrite<MailBoxData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.PostFacility>());
		if ((Object)(object)GetComponent<ServiceUpgrade>() == (Object)null)
		{
			components.Add(ComponentType.ReadWrite<Resources>());
			if ((Object)(object)GetComponent<CityServiceBuilding>() != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<GuestVehicle>());
				components.Add(ComponentType.ReadWrite<Efficiency>());
			}
			if (m_PostTruckCapacity > 0)
			{
				components.Add(ComponentType.ReadWrite<ServiceDispatch>());
				components.Add(ComponentType.ReadWrite<OwnedVehicle>());
			}
			if (m_PostVanCapacity > 0)
			{
				components.Add(ComponentType.ReadWrite<ServiceDispatch>());
				components.Add(ComponentType.ReadWrite<ServiceDistrict>());
				components.Add(ComponentType.ReadWrite<OwnedVehicle>());
			}
			if (m_MailBoxCapacity > 0)
			{
				components.Add(ComponentType.ReadWrite<Game.Routes.MailBox>());
			}
		}
	}

	public void GetUpgradeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Buildings.PostFacility>());
		components.Add(ComponentType.ReadWrite<Resources>());
		if (m_PostTruckCapacity > 0)
		{
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
		if (m_PostVanCapacity > 0)
		{
			components.Add(ComponentType.ReadWrite<ServiceDispatch>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
		if (m_MailBoxCapacity > 0)
		{
			components.Add(ComponentType.ReadWrite<Game.Routes.MailBox>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		PostFacilityData postFacilityData = default(PostFacilityData);
		postFacilityData.m_PostVanCapacity = m_PostVanCapacity;
		postFacilityData.m_PostTruckCapacity = m_PostTruckCapacity;
		postFacilityData.m_MailCapacity = m_MailStorageCapacity;
		postFacilityData.m_SortingRate = m_SortingRate;
		((EntityManager)(ref entityManager)).SetComponentData<PostFacilityData>(entity, postFacilityData);
		if (m_MailBoxCapacity > 0)
		{
			MailBoxData mailBoxData = default(MailBoxData);
			mailBoxData.m_MailCapacity = m_MailBoxCapacity;
			((EntityManager)(ref entityManager)).SetComponentData<MailBoxData>(entity, mailBoxData);
		}
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(11));
	}
}
