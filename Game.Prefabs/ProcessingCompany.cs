using System;
using System.Collections.Generic;
using Game.Agents;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Companies/", new Type[] { typeof(CompanyPrefab) })]
public class ProcessingCompany : ComponentBase
{
	public IndustrialProcess process;

	public int transports;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<IndustrialProcessData>());
		if (transports > 0)
		{
			components.Add(ComponentType.ReadWrite<TransportCompanyData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Companies.ProcessingCompany>());
		components.Add(ComponentType.ReadWrite<TaxPayer>());
		if (process.m_Input1.m_Resource != ResourceInEditor.NoResource || process.m_Input2.m_Resource != ResourceInEditor.NoResource)
		{
			components.Add(ComponentType.ReadWrite<BuyingCompany>());
		}
		if (process.m_Output.m_Resource == ResourceInEditor.Lodging)
		{
			components.Add(ComponentType.ReadWrite<LodgingProvider>());
			components.Add(ComponentType.ReadWrite<Renter>());
		}
		components.Add(ComponentType.ReadWrite<ResourceSeller>());
		components.Add(ComponentType.ReadWrite<Profitability>());
		components.Add(ComponentType.ReadWrite<TradeCost>());
		if (transports > 0)
		{
			components.Add(ComponentType.ReadWrite<TransportCompany>());
			components.Add(ComponentType.ReadWrite<OwnedVehicle>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
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
		if (transports > 0)
		{
			((EntityManager)(ref entityManager)).SetComponentData<TransportCompanyData>(entity, new TransportCompanyData
			{
				m_MaxTransports = transports
			});
		}
	}
}
