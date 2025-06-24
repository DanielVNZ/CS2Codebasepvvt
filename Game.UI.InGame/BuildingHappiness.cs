using Colossal.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.UI.InGame;

public static class BuildingHappiness
{
	public static void GetResidentialBuildingHappinessFactors(Entity city, NativeArray<int> taxRates, Entity property, NativeArray<int2> factors, ref ComponentLookup<PrefabRef> prefabs, ref ComponentLookup<SpawnableBuildingData> spawnableBuildings, ref ComponentLookup<BuildingPropertyData> buildingPropertyDatas, ref BufferLookup<CityModifier> cityModifiers, ref ComponentLookup<Building> buildings, ref ComponentLookup<ElectricityConsumer> electricityConsumers, ref ComponentLookup<WaterConsumer> waterConsumers, ref BufferLookup<Game.Net.ServiceCoverage> serviceCoverages, ref ComponentLookup<Locked> locked, ref ComponentLookup<Transform> transforms, ref ComponentLookup<GarbageProducer> garbageProducers, ref ComponentLookup<CrimeProducer> crimeProducers, ref ComponentLookup<MailProducer> mailProducers, ref BufferLookup<Renter> renters, ref ComponentLookup<Citizen> citizenDatas, ref BufferLookup<HouseholdCitizen> householdCitizens, ref ComponentLookup<BuildingData> buildingDatas, CitizenHappinessParameterData citizenHappinessParameters, GarbageParameterData garbageParameters, HealthcareParameterData healthcareParameters, ParkParameterData parkParameters, EducationParameterData educationParameters, TelecomParameterData telecomParameters, DynamicBuffer<HappinessFactorParameterData> happinessFactorParameters, NativeArray<GroundPollution> pollutionMap, NativeArray<NoisePollution> noisePollutionMap, NativeArray<AirPollution> airPollutionMap, CellMapData<TelecomCoverage> telecomCoverage, float relativeElectricityFee, float relativeWaterFee)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0912: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_093f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0946: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6e: Unknown result type (might be due to invalid IL or missing references)
		if (!prefabs.HasComponent(property))
		{
			return;
		}
		Entity prefab = prefabs[property].m_Prefab;
		if (!spawnableBuildings.HasComponent(prefab) || !buildingDatas.HasComponent(prefab))
		{
			return;
		}
		BuildingPropertyData buildingPropertyData = buildingPropertyDatas[prefab];
		DynamicBuffer<CityModifier> cityModifiers2 = cityModifiers[city];
		BuildingData buildingData = buildingDatas[prefab];
		float num = buildingData.m_LotSize.x * buildingData.m_LotSize.y;
		Entity val = Entity.Null;
		float curvePosition = 0f;
		int level = spawnableBuildings[prefab].m_Level;
		if (buildings.HasComponent(property))
		{
			Building building = buildings[property];
			val = building.m_RoadEdge;
			curvePosition = building.m_CurvePosition;
		}
		if (buildingPropertyData.m_ResidentialProperties <= 0)
		{
			return;
		}
		num /= (float)buildingPropertyData.m_ResidentialProperties;
		float num2 = 1f;
		int currentHappiness = 50;
		int num3 = 128;
		float num4 = 0.3f;
		float num5 = 0.25f;
		float num6 = 0.25f;
		float num7 = 0.15f;
		float num8 = 0.05f;
		float num9 = 2f;
		if (renters.HasBuffer(property))
		{
			num4 = 0f;
			num5 = 0f;
			num6 = 0f;
			num7 = 0f;
			num8 = 0f;
			int2 val2 = default(int2);
			int2 val3 = default(int2);
			int num10 = 0;
			int num11 = 0;
			DynamicBuffer<Renter> val4 = renters[property];
			for (int i = 0; i < val4.Length; i++)
			{
				Entity renter = val4[i].m_Renter;
				if (!householdCitizens.HasBuffer(renter))
				{
					continue;
				}
				num11++;
				DynamicBuffer<HouseholdCitizen> val5 = householdCitizens[renter];
				for (int j = 0; j < val5.Length; j++)
				{
					Entity citizen = val5[j].m_Citizen;
					if (citizenDatas.HasComponent(citizen))
					{
						Citizen citizen2 = citizenDatas[citizen];
						val3.x += citizen2.Happiness;
						val3.y++;
						num10 += citizen2.m_LeisureCounter;
						switch (citizen2.GetEducationLevel())
						{
						case 0:
							num4 += 1f;
							break;
						case 1:
							num5 += 1f;
							break;
						case 2:
							num6 += 1f;
							break;
						case 3:
							num7 += 1f;
							break;
						case 4:
							num8 += 1f;
							break;
						}
						if (citizen2.GetAge() == CitizenAge.Child)
						{
							val2.x++;
						}
					}
				}
				val2.y++;
			}
			if (val2.y > 0)
			{
				num2 = (float)val2.x / (float)val2.y;
			}
			if (val3.y > 0)
			{
				currentHappiness = Mathf.RoundToInt((float)val3.x / (float)val3.y);
				num3 = Mathf.RoundToInt((float)num10 / (float)val3.y);
				num4 /= (float)val3.y;
				num5 /= (float)val3.y;
				num6 /= (float)val3.y;
				num7 /= (float)val3.y;
				num8 /= (float)val3.y;
				num9 = (float)val3.y / (float)num11;
			}
		}
		Entity healthcareServicePrefab = healthcareParameters.m_HealthcareServicePrefab;
		Entity parkServicePrefab = parkParameters.m_ParkServicePrefab;
		Entity educationServicePrefab = educationParameters.m_EducationServicePrefab;
		Entity telecomServicePrefab = telecomParameters.m_TelecomServicePrefab;
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[4].m_LockedEntity))
		{
			int2 electricitySupplyBonuses = CitizenHappinessSystem.GetElectricitySupplyBonuses(property, ref electricityConsumers, in citizenHappinessParameters);
			int2 val6 = factors[3];
			val6.x++;
			val6.y += (electricitySupplyBonuses.x + electricitySupplyBonuses.y) / 2 - happinessFactorParameters[4].m_BaseLevel;
			factors[3] = val6;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[23].m_LockedEntity))
		{
			int2 electricityFeeBonuses = CitizenHappinessSystem.GetElectricityFeeBonuses(property, ref electricityConsumers, relativeElectricityFee, in citizenHappinessParameters);
			int2 val7 = factors[26];
			val7.x++;
			val7.y += (electricityFeeBonuses.x + electricityFeeBonuses.y) / 2 - happinessFactorParameters[23].m_BaseLevel;
			factors[26] = val7;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[8].m_LockedEntity))
		{
			int2 waterSupplyBonuses = CitizenHappinessSystem.GetWaterSupplyBonuses(property, ref waterConsumers, in citizenHappinessParameters);
			int2 val8 = factors[7];
			val8.x++;
			val8.y += (waterSupplyBonuses.x + waterSupplyBonuses.y) / 2 - happinessFactorParameters[8].m_BaseLevel;
			factors[7] = val8;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[24].m_LockedEntity))
		{
			int2 waterFeeBonuses = CitizenHappinessSystem.GetWaterFeeBonuses(property, ref waterConsumers, relativeWaterFee, in citizenHappinessParameters);
			int2 val9 = factors[27];
			val9.x++;
			val9.y += (waterFeeBonuses.x + waterFeeBonuses.y) / 2 - happinessFactorParameters[24].m_BaseLevel;
			factors[27] = val9;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[9].m_LockedEntity))
		{
			int2 waterPollutionBonuses = CitizenHappinessSystem.GetWaterPollutionBonuses(property, ref waterConsumers, cityModifiers2, in citizenHappinessParameters);
			int2 val10 = factors[8];
			val10.x++;
			val10.y += (waterPollutionBonuses.x + waterPollutionBonuses.y) / 2 - happinessFactorParameters[9].m_BaseLevel;
			factors[8] = val10;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[10].m_LockedEntity))
		{
			int2 sewageBonuses = CitizenHappinessSystem.GetSewageBonuses(property, ref waterConsumers, in citizenHappinessParameters);
			int2 val11 = factors[9];
			val11.x++;
			val11.y += (sewageBonuses.x + sewageBonuses.y) / 2 - happinessFactorParameters[10].m_BaseLevel;
			factors[9] = val11;
		}
		if (serviceCoverages.HasBuffer(val))
		{
			DynamicBuffer<Game.Net.ServiceCoverage> serviceCoverage = serviceCoverages[val];
			if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[5].m_LockedEntity))
			{
				int2 healthcareBonuses = CitizenHappinessSystem.GetHealthcareBonuses(curvePosition, serviceCoverage, ref locked, healthcareServicePrefab, in citizenHappinessParameters);
				int2 val12 = factors[4];
				val12.x++;
				val12.y += (healthcareBonuses.x + healthcareBonuses.y) / 2 - happinessFactorParameters[5].m_BaseLevel;
				factors[4] = val12;
			}
			if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[12].m_LockedEntity))
			{
				int2 entertainmentBonuses = CitizenHappinessSystem.GetEntertainmentBonuses(curvePosition, serviceCoverage, cityModifiers2, ref locked, parkServicePrefab, in citizenHappinessParameters);
				int2 val13 = factors[11];
				val13.x++;
				val13.y += (entertainmentBonuses.x + entertainmentBonuses.y) / 2 - happinessFactorParameters[12].m_BaseLevel;
				factors[11] = val13;
			}
			if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[13].m_LockedEntity))
			{
				int2 educationBonuses = CitizenHappinessSystem.GetEducationBonuses(curvePosition, serviceCoverage, ref locked, educationServicePrefab, in citizenHappinessParameters, 1);
				int2 val14 = factors[12];
				val14.x++;
				val14.y += Mathf.RoundToInt(num2 * (float)(educationBonuses.x + educationBonuses.y) / 2f) - happinessFactorParameters[13].m_BaseLevel;
				factors[12] = val14;
			}
			if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[15].m_LockedEntity))
			{
				int2 wellfareBonuses = CitizenHappinessSystem.GetWellfareBonuses(curvePosition, serviceCoverage, in citizenHappinessParameters, currentHappiness);
				int2 val15 = factors[14];
				val15.x++;
				val15.y += (wellfareBonuses.x + wellfareBonuses.y) / 2 - happinessFactorParameters[15].m_BaseLevel;
				factors[14] = val15;
			}
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[6].m_LockedEntity))
		{
			int2 groundPollutionBonuses = CitizenHappinessSystem.GetGroundPollutionBonuses(property, ref transforms, pollutionMap, cityModifiers2, in citizenHappinessParameters);
			int2 val16 = factors[5];
			val16.x++;
			val16.y += (groundPollutionBonuses.x + groundPollutionBonuses.y) / 2 - happinessFactorParameters[6].m_BaseLevel;
			factors[5] = val16;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[2].m_LockedEntity))
		{
			int2 airPollutionBonuses = CitizenHappinessSystem.GetAirPollutionBonuses(property, ref transforms, airPollutionMap, cityModifiers2, in citizenHappinessParameters);
			int2 val17 = factors[2];
			val17.x++;
			val17.y += (airPollutionBonuses.x + airPollutionBonuses.y) / 2 - happinessFactorParameters[2].m_BaseLevel;
			factors[2] = val17;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[7].m_LockedEntity))
		{
			int2 noiseBonuses = CitizenHappinessSystem.GetNoiseBonuses(property, ref transforms, noisePollutionMap, in citizenHappinessParameters);
			int2 val18 = factors[6];
			val18.x++;
			val18.y += (noiseBonuses.x + noiseBonuses.y) / 2 - happinessFactorParameters[7].m_BaseLevel;
			factors[6] = val18;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[11].m_LockedEntity))
		{
			int2 garbageBonuses = CitizenHappinessSystem.GetGarbageBonuses(property, ref garbageProducers, ref locked, happinessFactorParameters[11].m_LockedEntity, in garbageParameters);
			int2 val19 = factors[10];
			val19.x++;
			val19.y += (garbageBonuses.x + garbageBonuses.y) / 2 - happinessFactorParameters[11].m_BaseLevel;
			factors[10] = val19;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[1].m_LockedEntity))
		{
			int2 crimeBonuses = CitizenHappinessSystem.GetCrimeBonuses(default(CrimeVictim), property, ref crimeProducers, ref locked, happinessFactorParameters[1].m_LockedEntity, in citizenHappinessParameters);
			int2 val20 = factors[1];
			val20.x++;
			val20.y += (crimeBonuses.x + crimeBonuses.y) / 2 - happinessFactorParameters[1].m_BaseLevel;
			factors[1] = val20;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[14].m_LockedEntity))
		{
			int2 mailBonuses = CitizenHappinessSystem.GetMailBonuses(property, ref mailProducers, ref locked, telecomServicePrefab, in citizenHappinessParameters);
			int2 val21 = factors[13];
			val21.x++;
			val21.y += (mailBonuses.x + mailBonuses.y) / 2 - happinessFactorParameters[14].m_BaseLevel;
			factors[13] = val21;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[0].m_LockedEntity))
		{
			int2 telecomBonuses = CitizenHappinessSystem.GetTelecomBonuses(property, ref transforms, telecomCoverage, ref locked, telecomServicePrefab, in citizenHappinessParameters);
			int2 val22 = factors[0];
			val22.x++;
			val22.y += (telecomBonuses.x + telecomBonuses.y) / 2 - happinessFactorParameters[0].m_BaseLevel;
			factors[0] = val22;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[16].m_LockedEntity))
		{
			int2 leisureBonuses = CitizenHappinessSystem.GetLeisureBonuses((byte)num3);
			int2 val23 = factors[15];
			val23.x++;
			val23.y += (leisureBonuses.x + leisureBonuses.y) / 2 - happinessFactorParameters[16].m_BaseLevel;
			factors[15] = val23;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[17].m_LockedEntity))
		{
			float2 val24 = new float2(num4, num4) * float2.op_Implicit(CitizenHappinessSystem.GetTaxBonuses(0, taxRates, in citizenHappinessParameters)) + new float2(num5, num5) * float2.op_Implicit(CitizenHappinessSystem.GetTaxBonuses(1, taxRates, in citizenHappinessParameters)) + new float2(num6, num6) * float2.op_Implicit(CitizenHappinessSystem.GetTaxBonuses(2, taxRates, in citizenHappinessParameters)) + new float2(num7, num7) * float2.op_Implicit(CitizenHappinessSystem.GetTaxBonuses(3, taxRates, in citizenHappinessParameters)) + new float2(num8, num8) * float2.op_Implicit(CitizenHappinessSystem.GetTaxBonuses(4, taxRates, in citizenHappinessParameters));
			int2 val25 = factors[16];
			val25.x++;
			val25.y += Mathf.RoundToInt(val24.x + val24.y) / 2 - happinessFactorParameters[17].m_BaseLevel;
			factors[16] = val25;
		}
		if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, happinessFactorParameters[3].m_LockedEntity))
		{
			float2 val26 = float2.op_Implicit(CitizenHappinessSystem.GetApartmentWellbeing(buildingPropertyData.m_SpaceMultiplier * num / num9, level));
			int2 val27 = factors[21];
			val27.x++;
			val27.y += Mathf.RoundToInt(val26.x + val26.y) / 2 - happinessFactorParameters[3].m_BaseLevel;
			factors[21] = val27;
		}
	}

	public static void GetCompanyHappinessFactors(Entity property, NativeArray<int2> factors, ref ComponentLookup<PrefabRef> prefabs, ref ComponentLookup<SpawnableBuildingData> spawnableBuildings, ref ComponentLookup<BuildingPropertyData> buildingPropertyDatas, ref ComponentLookup<Building> buildings, ref ComponentLookup<OfficeBuilding> officeBuildings, ref BufferLookup<Renter> renters, ref ComponentLookup<BuildingData> buildingDatas, ref ComponentLookup<CompanyData> companies, ref ComponentLookup<IndustrialProcessData> industrialProcessDatas, ref ComponentLookup<WorkProvider> workProviders, ref BufferLookup<Employee> employees, ref ComponentLookup<WorkplaceData> workplaceDatas, ref ComponentLookup<Citizen> citizens, ref ComponentLookup<HealthProblem> healthProblems, ref ComponentLookup<ServiceAvailable> serviceAvailables, ref ComponentLookup<ResourceData> resourceDatas, ref ComponentLookup<ZonePropertiesData> zonePropertiesDatas, ref BufferLookup<Efficiency> efficiencies, ref ComponentLookup<ServiceCompanyData> serviceCompanyDatas, ref BufferLookup<ResourceAvailability> availabilities, ref BufferLookup<TradeCost> tradeCosts, EconomyParameterData economyParameters, NativeArray<int> taxRates, NativeArray<Entity> processes, ResourcePrefabs resourcePrefabs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (!prefabs.HasComponent(property))
		{
			return;
		}
		Entity prefab = prefabs[property].m_Prefab;
		if (!spawnableBuildings.HasComponent(prefab) || !buildingDatas.HasComponent(prefab))
		{
			return;
		}
		BuildingPropertyData buildingPropertyData = buildingPropertyDatas[prefab];
		BuildingData buildingData = buildingDatas[prefab];
		SpawnableBuildingData spawnableData = spawnableBuildings[prefab];
		int level = spawnableData.m_Level;
		Building building = default(Building);
		if (buildings.HasComponent(property))
		{
			building = buildings[property];
		}
		bool flag = false;
		Entity val = default(Entity);
		Entity val2 = default(Entity);
		IndustrialProcessData processData = default(IndustrialProcessData);
		ServiceCompanyData serviceCompanyData = default(ServiceCompanyData);
		Resource resource = buildingPropertyData.m_AllowedManufactured | buildingPropertyData.m_AllowedSold;
		if (resource == Resource.NoResource)
		{
			return;
		}
		if (renters.HasBuffer(property))
		{
			DynamicBuffer<Renter> val3 = renters[property];
			for (int i = 0; i < val3.Length; i++)
			{
				val = val3[i].m_Renter;
				if (!companies.HasComponent(val) || !prefabs.HasComponent(val))
				{
					continue;
				}
				val2 = prefabs[val].m_Prefab;
				if (industrialProcessDatas.HasComponent(val2))
				{
					if (serviceCompanyDatas.HasComponent(val2))
					{
						serviceCompanyData = serviceCompanyDatas[val2];
					}
					processData = industrialProcessDatas[val2];
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			AddCompanyHappinessFactors(factors, property, prefab, val, val2, processData, serviceCompanyData, buildingPropertyData.m_AllowedSold != Resource.NoResource, level, ref officeBuildings, ref workProviders, ref employees, ref workplaceDatas, ref citizens, ref healthProblems, ref serviceAvailables, ref buildingPropertyDatas, ref resourceDatas, ref serviceCompanyDatas, ref efficiencies, ref availabilities, ref tradeCosts, taxRates, building, spawnableData, buildingData, resourcePrefabs, ref economyParameters);
			return;
		}
		for (int j = 0; j < processes.Length; j++)
		{
			processData = industrialProcessDatas[processes[j]];
			if (serviceCompanyDatas.HasComponent(processes[j]))
			{
				serviceCompanyData = serviceCompanyDatas[processes[j]];
			}
			if ((resource & processData.m_Output.m_Resource) != Resource.NoResource)
			{
				AddCompanyHappinessFactors(factors, property, prefab, val, val2, processData, serviceCompanyData, buildingPropertyData.m_AllowedSold != Resource.NoResource, level, ref officeBuildings, ref workProviders, ref employees, ref workplaceDatas, ref citizens, ref healthProblems, ref serviceAvailables, ref buildingPropertyDatas, ref resourceDatas, ref serviceCompanyDatas, ref efficiencies, ref availabilities, ref tradeCosts, taxRates, building, spawnableData, buildingData, resourcePrefabs, ref economyParameters);
			}
		}
	}

	private static void AddCompanyHappinessFactors(NativeArray<int2> factors, Entity property, Entity prefab, Entity renter, Entity renterPrefab, IndustrialProcessData processData, ServiceCompanyData serviceCompanyData, bool commercial, int level, ref ComponentLookup<OfficeBuilding> officeBuildings, ref ComponentLookup<WorkProvider> workProviders, ref BufferLookup<Employee> employees, ref ComponentLookup<WorkplaceData> workplaceDatas, ref ComponentLookup<Citizen> citizens, ref ComponentLookup<HealthProblem> healthProblems, ref ComponentLookup<ServiceAvailable> serviceAvailables, ref ComponentLookup<BuildingPropertyData> buildingPropertyDatas, ref ComponentLookup<ResourceData> resourceDatas, ref ComponentLookup<ServiceCompanyData> serviceCompanyDatas, ref BufferLookup<Efficiency> efficiencies, ref BufferLookup<ResourceAvailability> availabilities, ref BufferLookup<TradeCost> tradeCosts, NativeArray<int> taxRates, Building building, SpawnableBuildingData spawnableData, BuildingData buildingData, ResourcePrefabs resourcePrefabs, ref EconomyParameterData economyParameters)
	{
	}
}
