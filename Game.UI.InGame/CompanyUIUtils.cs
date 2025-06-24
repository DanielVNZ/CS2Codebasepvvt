using Colossal.Entities;
using Game.Buildings;
using Game.Companies;
using Game.Prefabs;
using Game.Zones;
using Unity.Entities;

namespace Game.UI.InGame;

public static class CompanyUIUtils
{
	public static bool HasCompany(EntityManager entityManager, Entity entity, Entity prefab, out Entity company)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		company = Entity.Null;
		BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
		if (((EntityManager)(ref entityManager)).HasComponent<Renter>(entity) && EntitiesExtensions.TryGetComponent<BuildingPropertyData>(entityManager, prefab, ref buildingPropertyData) && buildingPropertyData.CountProperties(AreaType.Commercial) + buildingPropertyData.CountProperties(AreaType.Industrial) > 0)
		{
			DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
			if (EntitiesExtensions.TryGetBuffer<Renter>(entityManager, entity, true, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(val[i].m_Renter))
					{
						company = val[i].m_Renter;
						break;
					}
				}
			}
			return true;
		}
		return false;
	}

	public static bool HasCompany(Entity entity, Entity prefab, ref BufferLookup<Renter> renterFromEntity, ref ComponentLookup<BuildingPropertyData> buildingPropertyDataFromEntity, ref ComponentLookup<CompanyData> companyDataFromEntity, out Entity company)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		company = Entity.Null;
		BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
		if (renterFromEntity.HasBuffer(entity) && buildingPropertyDataFromEntity.TryGetComponent(prefab, ref buildingPropertyData) && buildingPropertyData.CountProperties(AreaType.Commercial) + buildingPropertyData.CountProperties(AreaType.Industrial) > 0)
		{
			DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
			if (renterFromEntity.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (companyDataFromEntity.HasComponent(val[i].m_Renter))
					{
						company = val[i].m_Renter;
						break;
					}
				}
			}
			return true;
		}
		return false;
	}

	public static CompanyProfitabilityKey GetProfitabilityKey(int profit)
	{
		if (profit > 128)
		{
			return CompanyProfitabilityKey.Profitable;
		}
		if (profit > 32)
		{
			return CompanyProfitabilityKey.GettingBy;
		}
		if (profit > -64)
		{
			return CompanyProfitabilityKey.BreakingEven;
		}
		if (profit > -182)
		{
			return CompanyProfitabilityKey.LosingMoney;
		}
		return CompanyProfitabilityKey.Bankrupt;
	}
}
