using System;
using System.Collections.Generic;
using Game.Companies;
using Game.Economy;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Companies/", new Type[] { typeof(CompanyPrefab) })]
public class StorageCompany : ComponentBase
{
	public IndustrialProcess process;

	public int transports;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<StorageCompanyData>());
		components.Add(ComponentType.ReadWrite<IndustrialProcessData>());
		if (transports > 0)
		{
			components.Add(ComponentType.ReadWrite<TransportCompanyData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (transports > 0)
		{
			components.Add(ComponentType.ReadWrite<TransportCompany>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
		components.Add(ComponentType.ReadWrite<Game.Companies.StorageCompany>());
		components.Add(ComponentType.ReadWrite<TradeCost>());
		components.Add(ComponentType.ReadWrite<ResourceSeller>());
		components.Add(ComponentType.ReadWrite<StorageTransferRequest>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		((EntityManager)(ref entityManager)).SetComponentData<IndustrialProcessData>(entity, new IndustrialProcessData
		{
			m_Input1 = 
			{
				m_Amount = process.m_Input1.m_Amount,
				m_Resource = EconomyUtils.GetResource(process.m_Input1.m_Resource)
			},
			m_Input2 = 
			{
				m_Amount = process.m_Input2.m_Amount,
				m_Resource = EconomyUtils.GetResource(process.m_Input2.m_Resource)
			},
			m_Output = 
			{
				m_Amount = process.m_Output.m_Amount,
				m_Resource = EconomyUtils.GetResource(process.m_Output.m_Resource)
			},
			m_MaxWorkersPerCell = process.m_MaxWorkersPerCell
		});
		StorageCompanyData storageCompanyData = new StorageCompanyData
		{
			m_StoredResources = EconomyUtils.GetResource(process.m_Output.m_Resource)
		};
		((EntityManager)(ref entityManager)).SetComponentData<StorageCompanyData>(entity, storageCompanyData);
		if (transports > 0)
		{
			((EntityManager)(ref entityManager)).SetComponentData<TransportCompanyData>(entity, new TransportCompanyData
			{
				m_MaxTransports = transports
			});
		}
	}
}
