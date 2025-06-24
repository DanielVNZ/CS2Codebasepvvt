using Game.Areas;
using Game.Buildings;
using Game.Companies;
using Game.Objects;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation;

public static class CompanyUtils
{
	public static int GetCompanyMoveAwayChance(Entity company, Entity companyPrefab, Entity property, ref ComponentLookup<ServiceAvailable> serviceAvailables, ref ComponentLookup<OfficeProperty> officeProperties, ref ComponentLookup<IndustrialProcessData> industrialProcessDatas, ref ComponentLookup<WorkProvider> workProviders, NativeArray<int> taxRates)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		bool num2 = serviceAvailables.HasComponent(company);
		bool flag = officeProperties.HasComponent(property);
		IndustrialProcessData industrialProcessData = industrialProcessDatas[companyPrefab];
		int num3 = (num2 ? TaxSystem.GetCommercialTaxRate(industrialProcessData.m_Output.m_Resource, taxRates) : ((!flag) ? TaxSystem.GetIndustrialTaxRate(industrialProcessData.m_Output.m_Resource, taxRates) : TaxSystem.GetOfficeTaxRate(industrialProcessData.m_Output.m_Resource, taxRates)));
		num += (num3 - 10) * 5 / 2;
		WorkProvider workProvider = workProviders[company];
		if (workProvider.m_UneducatedNotificationEntity != Entity.Null)
		{
			num += 5;
		}
		if (workProvider.m_EducatedNotificationEntity != Entity.Null)
		{
			num += 20;
		}
		return num;
	}

	public static int GetCommercialMaxFittingWorkers(BuildingData building, BuildingPropertyData properties, int level, ServiceCompanyData serviceData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.CeilToInt(serviceData.m_MaxWorkersPerCell * (float)building.m_LotSize.x * (float)building.m_LotSize.y * (1f + 0.5f * (float)level) * properties.m_SpaceMultiplier);
	}

	public static int GetIndustrialAndOfficeFittingWorkers(BuildingData building, BuildingPropertyData properties, int level, IndustrialProcessData processData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.CeilToInt(processData.m_MaxWorkersPerCell * (float)building.m_LotSize.x * (float)building.m_LotSize.y * (1f + 0.5f * (float)level) * properties.m_SpaceMultiplier);
	}

	public static int GetExtractorFittingWorkers(float area, float spaceMultiplier, IndustrialProcessData processData)
	{
		return Mathf.CeilToInt(processData.m_MaxWorkersPerCell * area * spaceMultiplier / 2f);
	}

	public static int GetCompanyMaxFittingWorkers(Entity companyEntity, Entity buildingEntity, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<ServiceCompanyData> serviceCompanyDatas, ref ComponentLookup<BuildingData> buildingDatas, ref ComponentLookup<BuildingPropertyData> buildingPropertyDatas, ref ComponentLookup<SpawnableBuildingData> spawnableBuildingDatas, ref ComponentLookup<IndustrialProcessData> industrialProcessDatas, ref ComponentLookup<ExtractorCompanyData> extractorCompanyDatas, ref ComponentLookup<Attached> attacheds, ref BufferLookup<Game.Areas.SubArea> subAreaBufs, ref BufferLookup<InstalledUpgrade> installedUpgrades, ref ComponentLookup<Game.Areas.Lot> lots, ref ComponentLookup<Geometry> geometries)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Entity val = prefabRefs[companyEntity];
		Entity val2 = prefabRefs[buildingEntity];
		int level = 1;
		if (spawnableBuildingDatas.HasComponent(val2))
		{
			level = spawnableBuildingDatas[val2].m_Level;
		}
		if (serviceCompanyDatas.HasComponent(val))
		{
			return GetCommercialMaxFittingWorkers(buildingDatas[val2], buildingPropertyDatas[val2], level, serviceCompanyDatas[val]);
		}
		if (extractorCompanyDatas.HasComponent(val))
		{
			float area = 0f;
			if (attacheds.HasComponent(buildingEntity))
			{
				area = ExtractorAISystem.GetArea(attacheds[buildingEntity].m_Parent, ref subAreaBufs, ref installedUpgrades, ref lots, ref geometries);
			}
			return math.max(1, GetExtractorFittingWorkers(area, 1f, industrialProcessDatas[val]));
		}
		if (industrialProcessDatas.HasComponent(val))
		{
			return GetIndustrialAndOfficeFittingWorkers(buildingDatas[val2], buildingPropertyDatas[val2], level, industrialProcessDatas[val]);
		}
		return 0;
	}
}
