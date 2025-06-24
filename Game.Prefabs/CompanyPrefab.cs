using System;
using System.Collections.Generic;
using Game.Agents;
using Game.Citizens;
using Game.Companies;
using Game.Economy;
using Game.Simulation;
using Game.Vehicles;
using Game.Zones;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Companies/", new Type[] { })]
public class CompanyPrefab : ArchetypePrefab
{
	public AreaType zone;

	public float profitability;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		if (zone == AreaType.Commercial)
		{
			components.Add(ComponentType.ReadWrite<CommercialCompanyData>());
		}
		else if (zone == AreaType.Industrial)
		{
			components.Add(ComponentType.ReadWrite<IndustrialCompanyData>());
		}
		components.Add(ComponentType.ReadWrite<CompanyBrandElement>());
		components.Add(ComponentType.ReadWrite<AffiliatedBrandElement>());
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
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<CompanyData>());
		components.Add(ComponentType.ReadWrite<UpdateFrame>());
		components.Add(ComponentType.ReadWrite<Resources>());
		components.Add(ComponentType.ReadWrite<PropertySeeker>());
		components.Add(ComponentType.ReadWrite<TripNeeded>());
		components.Add(ComponentType.ReadWrite<CompanyNotifications>());
		components.Add(ComponentType.ReadWrite<GuestVehicle>());
		if (zone == AreaType.Commercial)
		{
			components.Add(ComponentType.ReadWrite<CommercialCompany>());
		}
		else if (zone == AreaType.Industrial)
		{
			components.Add(ComponentType.ReadWrite<IndustrialCompany>());
		}
	}
}
