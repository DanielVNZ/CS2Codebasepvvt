using Game.Buildings;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Effects;

public struct EffectControlData
{
	[ReadOnly]
	public ComponentLookup<Owner> m_Owners;

	[ReadOnly]
	public ComponentLookup<Transform> m_Transforms;

	[ReadOnly]
	public ComponentLookup<Hidden> m_Hidden;

	[ReadOnly]
	public ComponentLookup<EffectData> m_EffectDatas;

	[ReadOnly]
	public ComponentLookup<LightEffectData> m_LightEffectDatas;

	[ReadOnly]
	public ComponentLookup<Building> m_Buildings;

	[ReadOnly]
	public ComponentLookup<Signature> m_SignatureBuildings;

	[ReadOnly]
	public ComponentLookup<Vehicle> m_Vehicles;

	[ReadOnly]
	public ComponentLookup<Car> m_Cars;

	[ReadOnly]
	public ComponentLookup<Aircraft> m_Aircraft;

	[ReadOnly]
	public ComponentLookup<Watercraft> m_Watercraft;

	[ReadOnly]
	public ComponentLookup<ParkedCar> m_ParkedCars;

	[ReadOnly]
	public ComponentLookup<ParkedTrain> m_ParkedTrains;

	[ReadOnly]
	public ComponentLookup<PrefabRef> m_Prefabs;

	[ReadOnly]
	public ComponentLookup<OnFire> m_OnFires;

	[ReadOnly]
	public ComponentLookup<Game.Vehicles.FireEngine> m_FireEngines;

	[ReadOnly]
	public ComponentLookup<Temp> m_Temps;

	[ReadOnly]
	public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransports;

	[ReadOnly]
	public ComponentLookup<Game.Vehicles.Taxi> m_Taxis;

	[ReadOnly]
	public ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransports;

	[ReadOnly]
	public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCars;

	[ReadOnly]
	public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgrades;

	[ReadOnly]
	public ComponentLookup<Extension> m_Extensions;

	[ReadOnly]
	public ComponentLookup<Game.Events.WeatherPhenomenon> m_WeatherPhenomenonData;

	[ReadOnly]
	public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeeds;

	[ReadOnly]
	public ComponentLookup<Destroyed> m_Destroyeds;

	[ReadOnly]
	public ComponentLookup<EarlyDisasterWarningDuration> m_EarlyDisasterWarningDurations;

	[ReadOnly]
	public ComponentLookup<Game.Buildings.WaterPumpingStation> m_WaterPumpingStations;

	[ReadOnly]
	public ComponentLookup<Game.Buildings.SewageOutlet> m_SewageOutlets;

	[ReadOnly]
	public ComponentLookup<Game.Buildings.WaterTower> m_WaterTowers;

	[ReadOnly]
	public ComponentLookup<StreetLight> m_StreetLights;

	[ReadOnly]
	public ComponentLookup<Stopped> m_Stoppeds;

	[ReadOnly]
	public ComponentLookup<Composition> m_Composition;

	[ReadOnly]
	public ComponentLookup<NetCompositionData> m_NetCompositionData;

	[ReadOnly]
	public ComponentLookup<Game.Buildings.ExtractorFacility> m_ExtractorData;

	[ReadOnly]
	public ComponentLookup<Attachment> m_AttachmentData;

	[ReadOnly]
	public BufferLookup<TransformFrame> m_TransformFrames;

	[ReadOnly]
	public BufferLookup<Renter> m_Renter;

	[ReadOnly]
	public ComponentLookup<TrafficLights> m_TrafficLights;

	[ReadOnly]
	public EffectFlagSystem.EffectFlagData m_EffectFlagData;

	[ReadOnly]
	public uint m_SimulationFrame;

	[ReadOnly]
	public Entity m_Selected;

	public EffectControlData(SystemBase system)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		m_Owners = system.GetComponentLookup<Owner>(true);
		m_Temps = system.GetComponentLookup<Temp>(true);
		m_Buildings = system.GetComponentLookup<Building>(true);
		m_EffectDatas = system.GetComponentLookup<EffectData>(true);
		m_LightEffectDatas = system.GetComponentLookup<LightEffectData>(true);
		m_Hidden = system.GetComponentLookup<Hidden>(true);
		m_ParkedCars = system.GetComponentLookup<ParkedCar>(true);
		m_ParkedTrains = system.GetComponentLookup<ParkedTrain>(true);
		m_Transforms = system.GetComponentLookup<Transform>(true);
		m_Vehicles = system.GetComponentLookup<Vehicle>(true);
		m_Cars = system.GetComponentLookup<Car>(true);
		m_Aircraft = system.GetComponentLookup<Aircraft>(true);
		m_Watercraft = system.GetComponentLookup<Watercraft>(true);
		m_Prefabs = system.GetComponentLookup<PrefabRef>(true);
		m_OnFires = system.GetComponentLookup<OnFire>(true);
		m_FireEngines = system.GetComponentLookup<Game.Vehicles.FireEngine>(true);
		m_PublicTransports = system.GetComponentLookup<Game.Vehicles.PublicTransport>(true);
		m_ServiceUpgrades = system.GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
		m_Extensions = system.GetComponentLookup<Extension>(true);
		m_WeatherPhenomenonData = system.GetComponentLookup<Game.Events.WeatherPhenomenon>(true);
		m_PseudoRandomSeeds = system.GetComponentLookup<PseudoRandomSeed>(true);
		m_Destroyeds = system.GetComponentLookup<Destroyed>(true);
		m_CargoTransports = system.GetComponentLookup<Game.Vehicles.CargoTransport>(true);
		m_PersonalCars = system.GetComponentLookup<Game.Vehicles.PersonalCar>(true);
		m_Taxis = system.GetComponentLookup<Game.Vehicles.Taxi>(true);
		m_EarlyDisasterWarningDurations = system.GetComponentLookup<EarlyDisasterWarningDuration>(true);
		m_WaterPumpingStations = system.GetComponentLookup<Game.Buildings.WaterPumpingStation>(true);
		m_SewageOutlets = system.GetComponentLookup<Game.Buildings.SewageOutlet>(true);
		m_WaterTowers = system.GetComponentLookup<Game.Buildings.WaterTower>(true);
		m_StreetLights = system.GetComponentLookup<StreetLight>(true);
		m_Stoppeds = system.GetComponentLookup<Stopped>(true);
		m_Composition = system.GetComponentLookup<Composition>(true);
		m_NetCompositionData = system.GetComponentLookup<NetCompositionData>(true);
		m_TransformFrames = system.GetBufferLookup<TransformFrame>(true);
		m_Renter = system.GetBufferLookup<Renter>(true);
		m_SignatureBuildings = system.GetComponentLookup<Signature>(true);
		m_ExtractorData = system.GetComponentLookup<Game.Buildings.ExtractorFacility>(true);
		m_AttachmentData = system.GetComponentLookup<Attachment>(true);
		m_TrafficLights = system.GetComponentLookup<TrafficLights>(true);
		m_EffectFlagData = default(EffectFlagSystem.EffectFlagData);
		m_SimulationFrame = 0u;
		m_Selected = default(Entity);
	}

	public void Update(SystemBase system, EffectFlagSystem.EffectFlagData effectFlagData, uint simulationFrame, Entity selected)
	{
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		m_Owners.Update(system);
		m_Temps.Update(system);
		m_Buildings.Update(system);
		m_EffectDatas.Update(system);
		m_LightEffectDatas.Update(system);
		m_Hidden.Update(system);
		m_ParkedCars.Update(system);
		m_ParkedTrains.Update(system);
		m_Transforms.Update(system);
		m_Vehicles.Update(system);
		m_Cars.Update(system);
		m_Aircraft.Update(system);
		m_Watercraft.Update(system);
		m_Prefabs.Update(system);
		m_OnFires.Update(system);
		m_FireEngines.Update(system);
		m_PublicTransports.Update(system);
		m_ServiceUpgrades.Update(system);
		m_Extensions.Update(system);
		m_WeatherPhenomenonData.Update(system);
		m_PseudoRandomSeeds.Update(system);
		m_Destroyeds.Update(system);
		m_CargoTransports.Update(system);
		m_PersonalCars.Update(system);
		m_Taxis.Update(system);
		m_EarlyDisasterWarningDurations.Update(system);
		m_WaterPumpingStations.Update(system);
		m_SewageOutlets.Update(system);
		m_WaterTowers.Update(system);
		m_StreetLights.Update(system);
		m_Stoppeds.Update(system);
		m_Composition.Update(system);
		m_NetCompositionData.Update(system);
		m_TransformFrames.Update(system);
		m_Renter.Update(system);
		m_SignatureBuildings.Update(system);
		m_ExtractorData.Update(system);
		m_AttachmentData.Update(system);
		m_TrafficLights.Update(system);
		m_EffectFlagData = effectFlagData;
		m_SimulationFrame = simulationFrame;
		m_Selected = selected;
	}

	private bool CheckTrigger(Entity owner, Entity buildingOwner, Entity topOwner, EffectConditionFlags flag, bool forbidden)
	{
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		switch (flag)
		{
		case EffectConditionFlags.Operational:
		{
			Building building = default(Building);
			if (m_Buildings.TryGetComponent(buildingOwner, ref building))
			{
				if (BuildingUtils.CheckOption(building, BuildingOption.Inactive))
				{
					return false;
				}
				if (m_OnFires.HasComponent(buildingOwner))
				{
					return false;
				}
			}
			Extension extension = default(Extension);
			if (m_Extensions.TryGetComponent(owner, ref extension) && (extension.m_Flags & ExtensionFlags.Disabled) != ExtensionFlags.None)
			{
				return false;
			}
			if (m_Destroyeds.HasComponent(owner))
			{
				return false;
			}
			if (m_SignatureBuildings.HasComponent(buildingOwner))
			{
				DynamicBuffer<Renter> val8 = default(DynamicBuffer<Renter>);
				if (m_Renter.TryGetBuffer(buildingOwner, ref val8) && val8.Length > 0)
				{
					return true;
				}
				return false;
			}
			Building building2 = default(Building);
			if (m_Buildings.TryGetComponent(topOwner, ref building2))
			{
				return (building2.m_Flags & Game.Buildings.BuildingFlags.LowEfficiency) == 0;
			}
			return true;
		}
		case EffectConditionFlags.Parked:
			if (m_Vehicles.HasComponent(topOwner))
			{
				if (!m_ParkedCars.HasComponent(topOwner))
				{
					return m_ParkedTrains.HasComponent(topOwner);
				}
				return true;
			}
			return true;
		case EffectConditionFlags.OnFire:
			return m_OnFires.HasComponent(buildingOwner);
		case EffectConditionFlags.Emergency:
		{
			Car car3 = default(Car);
			if (m_Cars.TryGetComponent(topOwner, ref car3))
			{
				return (car3.m_Flags & CarFlags.Emergency) != 0;
			}
			Aircraft aircraft = default(Aircraft);
			if (m_Aircraft.TryGetComponent(topOwner, ref aircraft))
			{
				return (aircraft.m_Flags & AircraftFlags.Emergency) != 0;
			}
			return false;
		}
		case EffectConditionFlags.Extinguishing:
			if (m_FireEngines.HasComponent(topOwner))
			{
				return (m_FireEngines[topOwner].m_State & FireEngineFlags.Extinguishing) != 0;
			}
			return false;
		case EffectConditionFlags.TakingOff:
		{
			DynamicBuffer<TransformFrame> val3 = default(DynamicBuffer<TransformFrame>);
			if (m_TransformFrames.TryGetBuffer(topOwner, ref val3))
			{
				if (forbidden)
				{
					for (int m = 0; m < val3.Length; m++)
					{
						if ((val3[m].m_Flags & TransformFlags.TakingOff) == 0)
						{
							return false;
						}
					}
					return true;
				}
				for (int n = 0; n < val3.Length; n++)
				{
					if ((val3[n].m_Flags & TransformFlags.TakingOff) != 0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
		case EffectConditionFlags.Landing:
		{
			DynamicBuffer<TransformFrame> val5 = default(DynamicBuffer<TransformFrame>);
			if (m_TransformFrames.TryGetBuffer(topOwner, ref val5))
			{
				if (forbidden)
				{
					for (int num3 = 0; num3 < val5.Length; num3++)
					{
						if ((val5[num3].m_Flags & TransformFlags.Landing) == 0)
						{
							return false;
						}
					}
					return true;
				}
				for (int num4 = 0; num4 < val5.Length; num4++)
				{
					if ((val5[num4].m_Flags & TransformFlags.Landing) != 0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
		case EffectConditionFlags.Flying:
		{
			DynamicBuffer<TransformFrame> val7 = default(DynamicBuffer<TransformFrame>);
			if (m_TransformFrames.TryGetBuffer(topOwner, ref val7))
			{
				if (forbidden)
				{
					for (int num7 = 0; num7 < val7.Length; num7++)
					{
						if ((val7[num7].m_Flags & TransformFlags.Flying) == 0)
						{
							return false;
						}
					}
					return true;
				}
				for (int num8 = 0; num8 < val7.Length; num8++)
				{
					if ((val7[num8].m_Flags & TransformFlags.Flying) != 0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
		case EffectConditionFlags.Stopped:
			return m_Stoppeds.HasComponent(topOwner);
		case EffectConditionFlags.Processing:
		{
			bool flag2 = false;
			if (m_WaterPumpingStations.HasComponent(topOwner))
			{
				flag2 = m_WaterPumpingStations[topOwner].m_LastProduction != 0;
			}
			if (m_SewageOutlets.HasComponent(topOwner))
			{
				flag2 = flag2 || m_SewageOutlets[topOwner].m_LastProcessed != 0;
			}
			if (m_WaterTowers.HasComponent(topOwner))
			{
				flag2 = flag2 || m_WaterTowers[topOwner].m_LastStoredWater != m_WaterTowers[topOwner].m_StoredWater;
			}
			return flag2;
		}
		case EffectConditionFlags.Boarding:
			if (m_PublicTransports.HasComponent(topOwner))
			{
				return (m_PublicTransports[topOwner].m_State & PublicTransportFlags.Boarding) != 0;
			}
			if (m_Taxis.HasComponent(topOwner))
			{
				return (m_Taxis[topOwner].m_State & TaxiFlags.Boarding) != 0;
			}
			if (m_CargoTransports.HasComponent(topOwner))
			{
				return (m_CargoTransports[topOwner].m_State & CargoTransportFlags.Boarding) != 0;
			}
			if (m_PersonalCars.HasComponent(topOwner))
			{
				return (m_PersonalCars[topOwner].m_State & PersonalCarFlags.Boarding) != 0;
			}
			return false;
		case EffectConditionFlags.Disaster:
			if (m_EarlyDisasterWarningDurations.HasComponent(topOwner))
			{
				return m_SimulationFrame < m_EarlyDisasterWarningDurations[topOwner].m_EndFrame;
			}
			return false;
		case EffectConditionFlags.Occurring:
			if (m_WeatherPhenomenonData.HasComponent(topOwner))
			{
				return m_WeatherPhenomenonData[topOwner].m_Intensity != 0f;
			}
			return false;
		case EffectConditionFlags.Night:
		case EffectConditionFlags.Cold:
		{
			Random random = GetRandom(topOwner);
			return EffectFlagSystem.IsEnabled(flag, random, m_EffectFlagData, m_SimulationFrame);
		}
		case EffectConditionFlags.LightsOff:
		{
			StreetLight streetLight = default(StreetLight);
			if (m_StreetLights.TryGetComponent(topOwner, ref streetLight))
			{
				return (streetLight.m_State & StreetLightState.TurnedOff) != 0;
			}
			Watercraft watercraft = default(Watercraft);
			if (m_Watercraft.TryGetComponent(topOwner, ref watercraft))
			{
				return (watercraft.m_Flags & WatercraftFlags.LightsOff) != 0;
			}
			return false;
		}
		case EffectConditionFlags.MainLights:
		{
			DynamicBuffer<TransformFrame> val = default(DynamicBuffer<TransformFrame>);
			if (m_TransformFrames.TryGetBuffer(topOwner, ref val))
			{
				if (forbidden)
				{
					for (int i = 0; i < val.Length; i++)
					{
						if ((val[i].m_Flags & TransformFlags.MainLights) == 0)
						{
							return false;
						}
					}
					return true;
				}
				for (int j = 0; j < val.Length; j++)
				{
					if ((val[j].m_Flags & TransformFlags.MainLights) != 0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
		case EffectConditionFlags.ExtraLights:
		{
			DynamicBuffer<TransformFrame> val6 = default(DynamicBuffer<TransformFrame>);
			if (m_TransformFrames.TryGetBuffer(topOwner, ref val6))
			{
				if (forbidden)
				{
					for (int num5 = 0; num5 < val6.Length; num5++)
					{
						if ((val6[num5].m_Flags & TransformFlags.ExtraLights) == 0)
						{
							return false;
						}
					}
					return true;
				}
				for (int num6 = 0; num6 < val6.Length; num6++)
				{
					if ((val6[num6].m_Flags & TransformFlags.ExtraLights) != 0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
		case EffectConditionFlags.WarningLights:
		{
			DynamicBuffer<TransformFrame> val2 = default(DynamicBuffer<TransformFrame>);
			if (m_TransformFrames.TryGetBuffer(topOwner, ref val2))
			{
				if (forbidden)
				{
					for (int k = 0; k < val2.Length; k++)
					{
						if ((val2[k].m_Flags & TransformFlags.WarningLights) == 0)
						{
							return false;
						}
					}
					return true;
				}
				for (int l = 0; l < val2.Length; l++)
				{
					if ((val2[l].m_Flags & TransformFlags.WarningLights) != 0)
					{
						return true;
					}
				}
				return false;
			}
			Car car = default(Car);
			if (m_Cars.TryGetComponent(topOwner, ref car))
			{
				return (car.m_Flags & (CarFlags.Emergency | CarFlags.Warning)) != 0;
			}
			return false;
		}
		case EffectConditionFlags.WorkLights:
		{
			DynamicBuffer<TransformFrame> val4 = default(DynamicBuffer<TransformFrame>);
			if (m_TransformFrames.TryGetBuffer(topOwner, ref val4))
			{
				if (forbidden)
				{
					for (int num = 0; num < val4.Length; num++)
					{
						if ((val4[num].m_Flags & TransformFlags.WorkLights) == 0)
						{
							return false;
						}
					}
					return true;
				}
				for (int num2 = 0; num2 < val4.Length; num2++)
				{
					if ((val4[num2].m_Flags & TransformFlags.WorkLights) != 0)
					{
						return true;
					}
				}
				return false;
			}
			Car car2 = default(Car);
			if (m_Cars.TryGetComponent(topOwner, ref car2))
			{
				return (car2.m_Flags & (CarFlags.Sign | CarFlags.Working)) != 0;
			}
			return false;
		}
		case EffectConditionFlags.Spillway:
		{
			Composition composition = default(Composition);
			NetCompositionData netCompositionData = default(NetCompositionData);
			if (m_Composition.TryGetComponent(topOwner, ref composition) && m_NetCompositionData.TryGetComponent(composition.m_Edge, ref netCompositionData))
			{
				return (netCompositionData.m_Flags.m_General & CompositionFlags.General.Spillway) != 0;
			}
			return false;
		}
		case EffectConditionFlags.Collapsing:
		{
			Destroyed destroyed = default(Destroyed);
			if (m_Destroyeds.TryGetComponent(owner, ref destroyed))
			{
				return destroyed.m_Cleared < 0f;
			}
			return false;
		}
		case EffectConditionFlags.MoveableBridgeWorking:
		{
			TrafficLights trafficLights = default(TrafficLights);
			if (m_TrafficLights.TryGetComponent(owner, ref trafficLights) && (trafficLights.m_Flags & TrafficLightFlags.MoveableBridge) != 0)
			{
				return trafficLights.m_State != Game.Net.TrafficLightState.Ongoing;
			}
			return false;
		}
		default:
			return false;
		}
	}

	private bool CheckTriggers(Entity owner, Entity buildingOwner, Entity topOwner, EffectCondition condition)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EffectConditionFlags effectConditionFlags = EffectConditionFlags.Emergency;
		while (true)
		{
			bool flag = (condition.m_RequiredFlags & effectConditionFlags) != 0;
			bool flag2 = (condition.m_ForbiddenFlags & effectConditionFlags) != 0;
			if (flag || flag2)
			{
				bool flag3 = CheckTrigger(owner, buildingOwner, topOwner, effectConditionFlags, flag2);
				if ((flag && !flag3) || (flag2 && flag3))
				{
					return true;
				}
			}
			if (effectConditionFlags == EffectConditionFlags.MoveableBridgeWorking)
			{
				break;
			}
			effectConditionFlags = (EffectConditionFlags)((int)effectConditionFlags << 1);
		}
		return false;
	}

	private bool CheckConditions(Entity owner, Entity buildingOwner, Entity topOwner, Entity effect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EffectData effectData = default(EffectData);
		if (m_EffectDatas.TryGetComponent(effect, ref effectData))
		{
			return !CheckTriggers(owner, buildingOwner, topOwner, effectData.m_Flags);
		}
		return true;
	}

	private Random GetRandom(Entity owner)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
		if (m_PseudoRandomSeeds.TryGetComponent(owner, ref pseudoRandomSeed))
		{
			return pseudoRandomSeed.GetRandom(PseudoRandomSeed.kEffectCondition);
		}
		Transform transform = default(Transform);
		if (m_Transforms.TryGetComponent(owner, ref transform))
		{
			return Random.CreateFromIndex((uint)math.dot(new float3(67f, 83f, 97f), transform.m_Position));
		}
		return Random.CreateFromIndex((uint)owner.Index);
	}

	public bool ShouldBeEnabled(Entity owner, Entity prefab, bool checkEnabled, bool isEditorContainer)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (isEditorContainer)
		{
			if (m_Hidden.HasComponent(owner))
			{
				return false;
			}
			if (!m_LightEffectDatas.HasComponent(prefab))
			{
				Temp temp = default(Temp);
				if (m_Temps.TryGetComponent(owner, ref temp))
				{
					if (m_Selected == Entity.Null || temp.m_Original != m_Selected)
					{
						if ((temp.m_Flags & TempFlags.Essential) == 0)
						{
							return false;
						}
						Owner owner2 = default(Owner);
						Temp temp2 = default(Temp);
						if (m_Owners.TryGetComponent(owner, ref owner2) && m_Temps.TryGetComponent(owner2.m_Owner, ref temp2) && (temp2.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Duplicate)) != 0)
						{
							return false;
						}
					}
				}
				else if (owner != m_Selected)
				{
					return false;
				}
			}
		}
		else
		{
			if (m_LightEffectDatas.HasComponent(prefab))
			{
				if (m_Hidden.HasComponent(owner))
				{
					return false;
				}
			}
			else if (m_Temps.HasComponent(owner))
			{
				return false;
			}
			if (checkEnabled)
			{
				Entity buildingOwner;
				Entity realOwner = GetRealOwner(owner, out buildingOwner);
				if (!CheckConditions(owner, buildingOwner, realOwner, prefab))
				{
					return false;
				}
			}
		}
		return true;
	}

	private Entity GetRealOwner(Entity owner, out Entity buildingOwner)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		Temp temp = default(Temp);
		if (m_Temps.TryGetComponent(owner, ref temp))
		{
			buildingOwner = ((temp.m_Original != Entity.Null) ? temp.m_Original : owner);
			return buildingOwner;
		}
		Owner owner2 = default(Owner);
		if (m_ServiceUpgrades.HasComponent(owner) && m_Owners.TryGetComponent(owner, ref owner2))
		{
			buildingOwner = (m_Buildings.HasComponent(owner) ? owner : owner2.m_Owner);
			return owner2.m_Owner;
		}
		Owner owner3 = default(Owner);
		if (m_ExtractorData.HasComponent(owner) && m_Owners.TryGetComponent(owner, ref owner3))
		{
			buildingOwner = owner;
			Entity val = owner3.m_Owner;
			Owner owner4 = default(Owner);
			while (m_Owners.TryGetComponent(val, ref owner4))
			{
				val = owner4.m_Owner;
			}
			Attachment attachment = default(Attachment);
			if (m_AttachmentData.TryGetComponent(val, ref attachment))
			{
				val = attachment.m_Attached;
			}
			return val;
		}
		buildingOwner = owner;
		return owner;
	}
}
