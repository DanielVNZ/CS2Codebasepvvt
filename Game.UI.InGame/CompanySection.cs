using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Zones;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class CompanySection : InfoSectionBase
{
	private enum ExtractedKey
	{
		Harvested,
		Extracted
	}

	private Entity companyEntity;

	protected override string group => "CompanySection";

	private Resource input1 { get; set; }

	private Resource input2 { get; set; }

	private Resource output { get; set; }

	private Resource sells { get; set; }

	private Resource stores { get; set; }

	private int2 customers { get; set; }

	private float price { get; set; }

	private bool isRentable { get; set; }

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		companyEntity = Entity.Null;
		input1 = Resource.NoResource;
		input2 = Resource.NoResource;
		output = Resource.NoResource;
		sells = Resource.NoResource;
		stores = Resource.NoResource;
		customers = int2.zero;
		isRentable = false;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return CompanyUIUtils.HasCompany(((ComponentSystemBase)this).EntityManager, selectedEntity, selectedPrefab, out companyEntity);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager;
		if (companyEntity == Entity.Null)
		{
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			ZoneData zoneData = default(ZoneData);
			if (EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref spawnableBuildingData) && EntitiesExtensions.TryGetComponent<ZoneData>(((ComponentSystemBase)this).EntityManager, spawnableBuildingData.m_ZonePrefab, ref zoneData))
			{
				switch (zoneData.m_AreaType)
				{
				case AreaType.Commercial:
					base.tooltipKeys.Add("VacantCommercial");
					break;
				case AreaType.Industrial:
					base.tooltipKeys.Add(zoneData.IsOffice() ? "VacantOffice" : "VacantIndustrial");
					break;
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<PropertyOnMarket>(selectedEntity))
			{
				isRentable = true;
			}
		}
		DynamicBuffer<Resources> val = default(DynamicBuffer<Resources>);
		PrefabRef prefabRef = default(PrefabRef);
		IndustrialProcessData industrialProcessData = default(IndustrialProcessData);
		if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, companyEntity, true, ref val) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, companyEntity, ref prefabRef) && EntitiesExtensions.TryGetComponent<IndustrialProcessData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref industrialProcessData))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ServiceAvailable>(companyEntity))
			{
				Resource resource = industrialProcessData.m_Input1.m_Resource;
				Resource resource2 = industrialProcessData.m_Input2.m_Resource;
				Resource resource3 = industrialProcessData.m_Output.m_Resource;
				if (resource != Resource.NoResource && resource != resource3)
				{
					input1 = resource;
				}
				if (resource2 != Resource.NoResource && resource2 != resource3 && resource2 != resource)
				{
					input2 = resource2;
					base.tooltipKeys.Add("Requires");
				}
				sells = resource3;
				base.tooltipKeys.Add("Sells");
			}
			else
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Game.Companies.ProcessingCompany>(companyEntity))
				{
					input1 = industrialProcessData.m_Input1.m_Resource;
					input2 = industrialProcessData.m_Input2.m_Resource;
					output = industrialProcessData.m_Output.m_Resource;
					base.tooltipKeys.Add("Requires");
					base.tooltipKeys.Add("Produces");
				}
				else
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Companies.ExtractorCompany>(companyEntity))
					{
						output = industrialProcessData.m_Output.m_Resource;
						base.tooltipKeys.Add("Produces");
					}
					else
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Game.Companies.StorageCompany>(companyEntity))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							stores = ((EntityManager)(ref entityManager)).GetComponentData<StorageCompanyData>(prefabRef.m_Prefab).m_StoredResources;
							base.tooltipKeys.Add("Stores");
						}
					}
				}
			}
		}
		LodgingProvider lodgingProvider = default(LodgingProvider);
		DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
		if (EntitiesExtensions.TryGetComponent<LodgingProvider>(((ComponentSystemBase)this).EntityManager, companyEntity, ref lodgingProvider) && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, companyEntity, true, ref val2))
		{
			customers = new int2(val2.Length, val2.Length + lodgingProvider.m_FreeRooms);
			price = lodgingProvider.m_Price;
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("companyName");
		if (companyEntity == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			m_NameSystem.BindName(writer, companyEntity);
		}
		writer.PropertyName("isRentable");
		writer.Write(isRentable);
		writer.PropertyName("input1");
		if (input1 == Resource.NoResource)
		{
			writer.WriteNull();
		}
		else
		{
			writer.Write(Enum.GetName(typeof(Resource), input1));
		}
		writer.PropertyName("input2");
		if (input2 == Resource.NoResource)
		{
			writer.WriteNull();
		}
		else
		{
			writer.Write(Enum.GetName(typeof(Resource), input2));
		}
		writer.PropertyName("output");
		if (output == Resource.NoResource)
		{
			writer.WriteNull();
		}
		else
		{
			writer.Write(Enum.GetName(typeof(Resource), output));
		}
		writer.PropertyName("sells");
		if (sells == Resource.NoResource)
		{
			writer.WriteNull();
		}
		else
		{
			writer.Write(Enum.GetName(typeof(Resource), sells));
		}
		writer.PropertyName("stores");
		if (stores == Resource.NoResource)
		{
			writer.WriteNull();
		}
		else
		{
			writer.Write(Enum.GetName(typeof(Resource), stores));
		}
		writer.PropertyName("customers");
		if (customers.y == 0)
		{
			writer.WriteNull();
		}
		else
		{
			MathematicsWriters.Write(writer, customers);
		}
	}

	[Preserve]
	public CompanySection()
	{
	}
}
