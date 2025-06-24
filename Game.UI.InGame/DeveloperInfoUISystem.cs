using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.Entities;
using Colossal.Rendering;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Events;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Simulation;
using Game.Vehicles;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class DeveloperInfoUISystem : UISystemBase
{
	private struct BuildingHappinessFactorValue : IComparable<BuildingHappinessFactorValue>
	{
		public BuildingHappinessFactor m_Factor;

		public int m_Value;

		public int CompareTo(BuildingHappinessFactorValue other)
		{
			return -math.abs(m_Value).CompareTo(math.abs(other.m_Value));
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ConsumptionData> __Game_Prefabs_ConsumptionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> __Game_Buildings_GarbageProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> __Game_Buildings_CrimeProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MailProducer> __Game_Buildings_MailProducer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OfficeBuilding> __Game_Prefabs_OfficeBuilding_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CompanyData> __Game_Companies_CompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceAvailable> __Game_Companies_ServiceAvailable_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> __Game_Prefabs_ZonePropertiesData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> __Game_Companies_ServiceCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TradeCost> __Game_Companies_TradeCost_RO_BufferLookup;

		public BufferLookup<HappinessFactorParameterData> __Game_Prefabs_HappinessFactorParameterData_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PollutionData> __Game_Prefabs_PollutionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PollutionModifierData> __Game_Prefabs_PollutionModifierData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Extractor> __Game_Areas_Extractor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorCompanyData> __Game_Prefabs_ExtractorCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
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
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_ConsumptionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ConsumptionData>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Net_ServiceCoverage_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(true);
			__Game_Buildings_CrimeProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeProducer>(true);
			__Game_Buildings_MailProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailProducer>(true);
			__Game_Prefabs_OfficeBuilding_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OfficeBuilding>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Companies_CompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CompanyData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Companies_WorkProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkProvider>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Companies_ServiceAvailable_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceAvailable>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZonePropertiesData>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Companies_ServiceCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceCompanyData>(true);
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
			__Game_Companies_TradeCost_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TradeCost>(true);
			__Game_Prefabs_HappinessFactorParameterData_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HappinessFactorParameterData>(false);
			__Game_Prefabs_PollutionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionData>(true);
			__Game_Prefabs_PollutionModifierData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionModifierData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Areas_Extractor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extractor>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorAreaData>(true);
			__Game_Prefabs_ExtractorCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorCompanyData>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TouristHousehold>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
		}
	}

	protected CitySystem m_CitySystem;

	protected NameSystem m_NameSystem;

	protected PrefabSystem m_PrefabSystem;

	protected ResourceSystem m_ResourceSystem;

	protected SimulationSystem m_SimulationSystem;

	protected SelectedInfoUISystem m_InfoUISystem;

	protected GroundPollutionSystem m_GroundPollutionSystem;

	protected AirPollutionSystem m_AirPollutionSystem;

	protected NoisePollutionSystem m_NoisePollutionSystem;

	protected TelecomCoverageSystem m_TelecomCoverageSystem;

	protected TaxSystem m_TaxSystem;

	protected BatchManagerSystem m_BatchManagerSystem;

	protected EntityQuery m_CitizenHappinessParameterQuery;

	protected EntityQuery m_HealthcareParameterQuery;

	protected EntityQuery m_ParkParameterQuery;

	protected EntityQuery m_EducationParameterQuery;

	protected EntityQuery m_TelecomParameterQuery;

	protected EntityQuery m_HappinessFactorParameterQuery;

	protected EntityQuery m_EconomyParameterQuery;

	protected EntityQuery m_ProcessQuery;

	protected EntityQuery m_TimeDataQuery;

	protected EntityQuery m_GarbageParameterQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_746694604_0;

	private EntityQuery __query_746694604_1;

	private EntityQuery __query_746694604_2;

	private EntityQuery __query_746694604_3;

	private EntityQuery __query_746694604_4;

	private EntityQuery __query_746694604_5;

	private EntityQuery __query_746694604_6;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitizenHappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		m_HealthcareParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_ParkParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ParkParameterData>() });
		m_EducationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EducationParameterData>() });
		m_TelecomParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TelecomParameterData>() });
		m_HappinessFactorParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HappinessFactorParameterData>() });
		m_GarbageParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_ProcessQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IndustrialProcessData>() });
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_InfoUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasEntityInfo, UpdateEntityInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasContentPrerequisite, UpdateContentPrerequisite));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasMeshGroupInfo, UpdateMeshGroupInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasBatchInfo, UpdateBatchInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasAddressInfo, UpdateAddressInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasCrimeInfo, UpdateCrimeInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasZoneInfo, UpdateZoneInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasZoneInfo, UpdateZoneHappinessInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasTelecomRangeInfo, UpdateTelecomRangeInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasPollutionInfo, UpdatePollutionInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasGarbageInfo, UpdateGarbageInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasWaterConsumeInfo, UpdateWaterConsumeInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasSpectatorSiteInfo, UpdateSpectatorSiteInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasInDangerInfo, UpdateInDangerInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasParkInfo, UpdateParkInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasCompanyInfo, UpdateCompanyInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasEfficiencyInfo, UpdateEfficiencyInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasElectricityProductionInfo, UpdateElectricityProductionInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasBatteriesInfo, UpdateBatteriesInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasGarbageProcessingInfo, UpdateGarbageProcessingInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasMailProcessingSpeedInfo, UpdateMailProcessingSpeedInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasMailInfo, UpdateMailInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasRentInfo, UpdateRentInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasTradeCostInfo, UpdateTradeCostInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasTradePartnerInfo, UpdateTradePartnerInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasWarehouseInfo, UpdateWarehouseInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasCompanyEconomyInfo, UpdateCompanyEconomyInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasCompanyProfitInfo, UpdateCompanyProfitInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasLandValueInfo, UpdateLandValueInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasElectricityConsumeInfo, UpdateElectricityConsumeInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasSendReceiveMailInfo, UpdateSendReceiveMailInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasNetworkCapacityInfo, UpdateNetworkCapacityInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasShelterInfo, UpdateShelterInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasPoliceInfo, UpdatePoliceInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasDeathcareInfo, UpdateDeathcareInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasStoredGarbageInfo, UpdateStoredGarbageInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasStoredMailInfo, UpdateStoredMailInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasZoneLevelInfo, UpdateZoneLevelInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasPrisonInfo, UpdatePrisonInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasParkingInfo, UpdateParkingInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasVehicleInfo, UpdateVehicleInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasResourceProductionInfo, UpdateResourceProductionInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasHouseholdsInfo, UpdateHouseholdsInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasHouseholdInfo, UpdateHouseholdInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasHomelessInfo, UpdateHomelessInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasResidentsInfo, UpdateResidentInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasEmployeesInfo, UpdateEmployeesInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasPatientsInfo, UpdatePatientsInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasStudentsInfo, UpdateStudentsInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasStoredResourcesInfo, UpdateStoredResourcesInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasHouseholdPetsInfo, UpdateHouseholdPetsInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasServiceCompanyInfo, UpdateServiceCompanyInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasExtractorCompanyInfo, UpdateExtractorCompanyInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasProcessingCompanyInfo, UpdateProcessingCompanyInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasStorageInfo, UpdateStorageInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasTransferRequestInfo, UpdateTransferRequestInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasOwnerInfo, UpdateOwnerInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasKeeperInfo, UpdateKeeperInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasControllerInfo, UpdateControllerInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasPassengerInfo, UpdatePassengerInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasPersonalCarInfo, UpdatePersonalCarInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasDeliveryTruckInfo, UpdateDeliveryTruckInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasAmbulanceInfo, UpdateAmbulanceInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasHearseInfo, UpdateHearseInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasGarbageTruckInfo, UpdateGarbageTruckInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasPublicTransportInfo, UpdatePublicTransportInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasCargoTransportInfo, UpdateCargoTransportInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasMaintenanceVehicleInfo, UpdateMaintenanceVehicleInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasPostVanInfo, UpdatePostVanInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasFireEngineInfo, UpdateFireEngineInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasPoliceCarInfo, UpdatePoliceCarInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasCitizenInfo, UpdateCitizenInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasMailSenderInfo, UpdateMailSenderInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasAnimalInfo, UpdateAnimalInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasCreatureInfo, UpdateCreatureInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasGroupLeaderInfo, UpdateGroupLeaderInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasGroupMemberInfo, UpdateGroupMemberInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasAreaInfo, UpdateAreaInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasTreeInfo, UpdateTreeInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasMailBoxInfo, UpdateMailBoxInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasBoardingVehicleInfo, UpdateBoardingVehicleInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasWaitingPassengerInfo, UpdateWaitingPassengerInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasMovingInfo, UpdateMovingInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasDamagedInfo, UpdateDamagedInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasDestroyedInfo, UpdateDestroyedInfo));
		m_InfoUISystem.AddDeveloperInfo(new CapacityInfo(HasDestroyedBuildingInfo, UpdateDestroyedBuildingInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasOnFireInfo, UpdateOnFireInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasFacingWeatherInfo, UpdateFacingWeatherInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasAccidentSiteInfo, UpdateAccidentSiteInfo));
		m_InfoUISystem.AddDeveloperInfo(new GenericInfo(HasInvolvedInAccidentInfo, UpdateInvolvedInAccidentInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasFloodedInfo, UpdateFloodedInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasEventInfo, UpdateEventInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasNotificationInfo, UpdateNotificationInfo));
		m_InfoUISystem.AddDeveloperInfo(new InfoList(HasVehicleModelInfo, UpdateVehicleModelInfo));
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	protected void AddUpgradeData<T>(Entity entity, ref T data) where T : unmanaged, IComponentData, ICombineData<T>
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<InstalledUpgrade> upgrades = default(DynamicBuffer<InstalledUpgrade>);
		if (EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, entity, true, ref upgrades))
		{
			UpgradeUtils.CombineStats(((ComponentSystemBase)this).EntityManager, ref data, upgrades);
		}
	}

	private bool HasContentPrerequisite(Entity entity, Entity prefab)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (prefab != InfoList.Item.kNullEntity)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<ContentPrerequisiteData>(prefab);
		}
		return false;
	}

	private void UpdateContentPrerequisite(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		info.label = StringUtils.Nicify("ContentPrerequisite");
		ContentPrerequisiteData contentPrerequisiteData = default(ContentPrerequisiteData);
		if (EntitiesExtensions.TryGetComponent<ContentPrerequisiteData>(((ComponentSystemBase)this).EntityManager, prefab, ref contentPrerequisiteData))
		{
			if (m_PrefabSystem.TryGetPrefab<ContentPrefab>(contentPrerequisiteData.m_ContentPrerequisite, out var prefab2))
			{
				foreach (ComponentBase component in prefab2.components)
				{
					info.Add(new InfoList.Item(component.GetDebugString(), InfoList.Item.kNullEntity));
				}
				return;
			}
			info.Add(new InfoList.Item(m_PrefabSystem.GetPrefabName(contentPrerequisiteData.m_ContentPrerequisite), InfoList.Item.kNullEntity));
		}
		else
		{
			info.label = "Missing or invalid ContentPrefab";
		}
	}

	private bool HasEntityInfo(Entity entity, Entity prefab)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return entity != InfoList.Item.kNullEntity;
	}

	private void UpdateEntityInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		info.label = "Entity";
		info.value = m_NameSystem.GetDebugName(entity);
	}

	private bool HasMeshPrefabInfo(Entity entity, Entity prefab)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (prefab != InfoList.Item.kNullEntity)
		{
			ObjectGeometryPrefab prefab2 = m_PrefabSystem.GetPrefab<ObjectGeometryPrefab>(prefab);
			if ((Object)(object)prefab2 != (Object)null)
			{
				return prefab2.m_Meshes.Length != 0;
			}
		}
		return false;
	}

	private void UpdateMeshPrefabInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		info.label = "MeshPrefabs";
		SubMeshFlags subMeshFlags = (SubMeshFlags)0u;
		Tree tree = default(Tree);
		if (EntitiesExtensions.TryGetComponent<Tree>(((ComponentSystemBase)this).EntityManager, entity, ref tree))
		{
			GrowthScaleData growthScaleData = default(GrowthScaleData);
			subMeshFlags = ((!EntitiesExtensions.TryGetComponent<GrowthScaleData>(((ComponentSystemBase)this).EntityManager, prefab, ref growthScaleData)) ? (subMeshFlags | SubMeshFlags.RequireAdult) : (subMeshFlags | BatchDataHelpers.CalculateTreeSubMeshData(tree, growthScaleData, out var _)));
		}
		ObjectGeometryPrefab prefab2 = m_PrefabSystem.GetPrefab<ObjectGeometryPrefab>(prefab);
		for (int i = 0; i < prefab2.m_Meshes.Length; i++)
		{
			ObjectState requireState = prefab2.m_Meshes[i].m_RequireState;
			string text = ((Object)prefab2.m_Meshes[i].m_Mesh).name;
			if ((requireState == ObjectState.Child && (subMeshFlags & SubMeshFlags.RequireChild) == SubMeshFlags.RequireChild) || (requireState == ObjectState.Teen && (subMeshFlags & SubMeshFlags.RequireTeen) == SubMeshFlags.RequireTeen) || (requireState == ObjectState.Adult && (subMeshFlags & SubMeshFlags.RequireAdult) == SubMeshFlags.RequireAdult) || (requireState == ObjectState.Elderly && (subMeshFlags & SubMeshFlags.RequireElderly) == SubMeshFlags.RequireElderly) || (requireState == ObjectState.Dead && (subMeshFlags & SubMeshFlags.RequireDead) == SubMeshFlags.RequireDead) || (requireState == ObjectState.Stump && (subMeshFlags & SubMeshFlags.RequireStump) == SubMeshFlags.RequireStump))
			{
				text += " [X]";
			}
			info.Add(new InfoList.Item(text, InfoList.Item.kNullEntity));
		}
	}

	private bool HasMeshGroupInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<MeshGroup>(entity))
		{
			CurrentTransport currentTransport = default(CurrentTransport);
			if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<MeshGroup>(currentTransport.m_CurrentTransport);
			}
			return false;
		}
		return true;
	}

	private void UpdateMeshGroupInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
			PrefabRef prefabRef = default(PrefabRef);
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref prefabRef))
			{
				prefab = prefabRef.m_Prefab;
			}
		}
		DynamicBuffer<MeshGroup> val = default(DynamicBuffer<MeshGroup>);
		if (!EntitiesExtensions.TryGetBuffer<MeshGroup>(((ComponentSystemBase)this).EntityManager, entity, true, ref val) || !m_PrefabSystem.TryGetPrefab<ObjectGeometryPrefab>(prefab, out var prefab2) || prefab2.m_Meshes == null)
		{
			return;
		}
		CreatureData creatureData = default(CreatureData);
		EntitiesExtensions.TryGetComponent<CreatureData>(((ComponentSystemBase)this).EntityManager, prefab, ref creatureData);
		info.label = $"Mesh groups ({val.Length})";
		for (int i = 0; i < val.Length; i++)
		{
			int subMeshGroup = val[i].m_SubMeshGroup;
			int num = 0;
			while (true)
			{
				int num2;
				CharacterGroup.OverrideInfo overrideInfo;
				int num3;
				if (num < prefab2.m_Meshes.Length)
				{
					if (prefab2.m_Meshes[num].m_Mesh is CharacterGroup { m_Characters: not null } characterGroup)
					{
						num2 = 0;
						while (num2 < characterGroup.m_Characters.Length)
						{
							if ((characterGroup.m_Characters[num2].m_Style.m_Gender & creatureData.m_Gender) != creatureData.m_Gender || subMeshGroup-- != 0)
							{
								num2++;
								continue;
							}
							goto IL_0108;
						}
						if (characterGroup.m_Overrides != null)
						{
							for (int j = 0; j < characterGroup.m_Overrides.Length; j++)
							{
								overrideInfo = characterGroup.m_Overrides[j];
								num3 = 0;
								while (num3 < characterGroup.m_Characters.Length)
								{
									if ((characterGroup.m_Characters[num3].m_Style.m_Gender & creatureData.m_Gender) != creatureData.m_Gender || subMeshGroup-- != 0)
									{
										num3++;
										continue;
									}
									goto IL_0196;
								}
							}
						}
					}
					num++;
					continue;
				}
				info.Add(new InfoList.Item($"Unknown group #{val[i].m_SubMeshGroup}"));
				break;
				IL_0196:
				info.Add(new InfoList.Item($"{((Object)characterGroup).name} #{num3} ({((Object)overrideInfo.m_Group).name})"));
				break;
				IL_0108:
				info.Add(new InfoList.Item($"{((Object)characterGroup).name} #{num2}"));
				break;
			}
		}
	}

	private bool HasBatchInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<MeshBatch>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<CurrentTransport>(entity);
		}
		return true;
	}

	private void UpdateBatchInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
		}
		DynamicBuffer<MeshBatch> val = default(DynamicBuffer<MeshBatch>);
		if (!EntitiesExtensions.TryGetBuffer<MeshBatch>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			return;
		}
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: true, out dependencies2);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		int num = 0;
		for (int i = 0; i < val.Length; i++)
		{
			num += nativeBatchGroups.GetBatchCount(val[i].m_GroupIndex);
		}
		info.label = $"Batches ({num})";
		for (int j = 0; j < val.Length; j++)
		{
			MeshBatch meshBatch = val[j];
			GroupData groupData = nativeBatchGroups.GetGroupData(meshBatch.m_GroupIndex);
			num = nativeBatchGroups.GetBatchCount(meshBatch.m_GroupIndex);
			int mergedInstanceGroupIndex = nativeBatchInstances.GetMergedInstanceGroupIndex(meshBatch.m_GroupIndex, meshBatch.m_InstanceIndex);
			int num2 = -1;
			for (int k = 0; k < num; k++)
			{
				BatchData batchData = nativeBatchGroups.GetBatchData(meshBatch.m_GroupIndex, k);
				int managedBatchIndex = nativeBatchGroups.GetManagedBatchIndex(meshBatch.m_GroupIndex, k);
				int num3 = -1;
				if (mergedInstanceGroupIndex >= 0)
				{
					num3 = nativeBatchGroups.GetManagedBatchIndex(mergedInstanceGroupIndex, k);
				}
				if (batchData.m_LodIndex != num2)
				{
					num2 = batchData.m_LodIndex;
					info.Add(new InfoList.Item($"--- Mesh {meshBatch.m_MeshIndex}, Tile {meshBatch.m_TileIndex}, Layer {groupData.m_Layer}, Lod {batchData.m_LodIndex} ---"));
				}
				if (managedBatchIndex < 0)
				{
					continue;
				}
				CustomBatch customBatch = (CustomBatch)(object)managedBatches.GetBatch(managedBatchIndex);
				RenderPrefab prefab4;
				if (num3 >= 0)
				{
					CustomBatch customBatch2 = (CustomBatch)(object)managedBatches.GetBatch(num3);
					if (m_PrefabSystem.TryGetPrefab<RenderPrefab>(customBatch.sourceMeshEntity, out var prefab2) && m_PrefabSystem.TryGetPrefab<RenderPrefab>(customBatch2.sourceMeshEntity, out var prefab3))
					{
						if (customBatch.generatedType != GeneratedType.None)
						{
							info.Add(new InfoList.Item($"{((Object)prefab3).name} {customBatch2.generatedType} -> {((Object)prefab2).name} {customBatch.generatedType}"));
						}
						else
						{
							info.Add(new InfoList.Item($"{((Object)prefab3).name}[{customBatch2.sourceSubMeshIndex}] -> {((Object)prefab2).name}[{customBatch.sourceSubMeshIndex}]"));
						}
					}
					else
					{
						info.Add(new InfoList.Item(((Object)((ManagedBatch)customBatch2).mesh).name + " -> " + ((Object)((ManagedBatch)customBatch).mesh).name));
					}
				}
				else if (m_PrefabSystem.TryGetPrefab<RenderPrefab>(customBatch.sourceMeshEntity, out prefab4))
				{
					if (customBatch.generatedType != GeneratedType.None)
					{
						info.Add(new InfoList.Item($"{((Object)prefab4).name} {customBatch.generatedType}"));
					}
					else
					{
						info.Add(new InfoList.Item($"{((Object)prefab4).name}[{customBatch.sourceSubMeshIndex}]"));
					}
				}
				else
				{
					info.Add(new InfoList.Item(((Object)((ManagedBatch)customBatch).mesh).name));
				}
			}
		}
	}

	private bool HasAddressInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Attached>(entity);
		}
		return true;
	}

	private void UpdateAddressInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		info.label = "Address";
		if (BuildingUtils.GetAddress(((ComponentSystemBase)this).EntityManager, entity, out var road, out var number))
		{
			info.value = m_NameSystem.GetDebugName(road) + " " + number;
			info.target = road;
		}
		else
		{
			info.value = "Unknown";
			info.target = InfoList.Item.kNullEntity;
		}
	}

	private bool HasCrimeInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<CrimeProducer>(entity);
		}
		return false;
	}

	private void UpdateCrimeInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		info.label = "Crime";
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		info.value = $"Accumulate: ({((EntityManager)(ref entityManager)).GetComponentData<CrimeProducer>(entity).m_Crime})";
	}

	private bool HasHomelessInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Renter>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(entity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					return ((EntityManager)(ref entityManager)).HasComponent<Abandoned>(entity);
				}
				return true;
			}
		}
		return false;
	}

	private void UpdateHomelessInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		info.label = "Homeless";
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Renter> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Renter>(entity, true);
		ComponentLookup<BuildingData> buildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<BuildingPropertyData> buildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		bool flag = false;
		DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
		if (EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			PrefabRef prefabRef = default(PrefabRef);
			DynamicBuffer<ObjectRequirementElement> val2 = default(DynamicBuffer<ObjectRequirementElement>);
			for (int i = 0; i < val.Length; i++)
			{
				if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val[i].m_SubObject, ref prefabRef) || !EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val2))
				{
					continue;
				}
				for (int j = 0; j < val2.Length; j++)
				{
					if ((val2[j].m_RequireFlags & ObjectRequirementFlags.Homeless) != 0)
					{
						flag = true;
						break;
					}
				}
			}
		}
		info.label = $"Homeless Count: ({buffer.Length}) MaxCapacity:{BuildingUtils.GetShelterHomelessCapacity(prefab, ref buildingDatas, ref buildingPropertyDatas)}";
		info.Add(new InfoList.Item($"HaveTent:{flag} "));
		for (int k = 0; k < buffer.Length; k++)
		{
			Entity renter = buffer[k].m_Renter;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Household>(renter))
			{
				info.Add(new InfoList.Item(m_NameSystem.GetDebugName(renter), renter));
			}
		}
	}

	private bool HasTelecomRangeInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			TelecomFacilityData data = default(TelecomFacilityData);
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.TelecomFacility>(entity) && EntitiesExtensions.TryGetComponent<TelecomFacilityData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
			{
				AddUpgradeData<TelecomFacilityData>(entity, ref data);
				return data.m_Range >= 1f;
			}
		}
		return false;
	}

	private void UpdateTelecomRangeInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		TelecomFacilityData data = ((EntityManager)(ref entityManager)).GetComponentData<TelecomFacilityData>(prefab);
		AddUpgradeData<TelecomFacilityData>(entity, ref data);
		float num = 1f;
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		if (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, entity, true, ref buffer))
		{
			num = BuildingUtils.GetEfficiency(buffer);
		}
		float num2 = data.m_Range * math.sqrt(num);
		info.label = "Range";
		info.value = Mathf.RoundToInt(num2) + "/" + Mathf.RoundToInt(data.m_Range);
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasNetworkCapacityInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			TelecomFacilityData data = default(TelecomFacilityData);
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.TelecomFacility>(entity) && EntitiesExtensions.TryGetComponent<TelecomFacilityData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
			{
				AddUpgradeData<TelecomFacilityData>(entity, ref data);
				return data.m_NetworkCapacity >= 1f;
			}
		}
		return false;
	}

	private void UpdateNetworkCapacityInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		TelecomFacilityData data = ((EntityManager)(ref entityManager)).GetComponentData<TelecomFacilityData>(prefab);
		AddUpgradeData<TelecomFacilityData>(entity, ref data);
		float num = 1f;
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		if (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, entity, true, ref buffer))
		{
			num = BuildingUtils.GetEfficiency(buffer);
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<CityModifier> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true);
		CityUtils.ApplyModifier(ref data.m_NetworkCapacity, buffer2, CityModifierType.TelecomCapacity);
		float num2 = data.m_NetworkCapacity * num;
		info.label = "Network Capacity";
		info.value = Mathf.RoundToInt(num2);
		info.max = info.value;
	}

	private bool HasZoneInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Renter>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(prefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					return ((EntityManager)(ref entityManager)).HasComponent<BuildingData>(prefab);
				}
				return false;
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Household>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<CompanyData>(entity))
			{
				return false;
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<PropertyRenter>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
			EntityManager entityManager3 = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(((EntityManager)(ref entityManager2)).GetComponentData<PrefabRef>(((EntityManager)(ref entityManager3)).GetComponentData<PropertyRenter>(entity).m_Property).m_Prefab);
		}
		return false;
	}

	private void UpdateZoneInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			entity = ((EntityManager)(ref entityManager)).GetComponentData<PropertyRenter>(entity).m_Property;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity).m_Prefab;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		SpawnableBuildingData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SpawnableBuildingData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		BuildingData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<BuildingData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		ZoneData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<ZoneData>(componentData.m_ZonePrefab);
		ZoneDensity zoneDensity = (ZoneDensity)255;
		ZonePropertiesData zonePropertiesData = default(ZonePropertiesData);
		if (EntitiesExtensions.TryGetComponent<ZonePropertiesData>(((ComponentSystemBase)this).EntityManager, componentData.m_ZonePrefab, ref zonePropertiesData))
		{
			zoneDensity = PropertyUtils.GetZoneDensity(componentData3, zonePropertiesData);
		}
		string prefabName = m_PrefabSystem.GetPrefabName(componentData.m_ZonePrefab);
		info.label = "Zone Info";
		info.value = prefabName + " " + componentData2.m_LotSize.x + "x" + componentData2.m_LotSize.y + " Density:" + ((zoneDensity == (ZoneDensity)255) ? "N/A" : Enum.GetName(typeof(ZoneDensity), zoneDensity));
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasZoneHappinessInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(prefab))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<BuildingData>(prefab);
			}
			return false;
		}
		return false;
	}

	private void UpdateZoneHappinessInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> processes = ((EntityQuery)(ref m_ProcessQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentLookup<PrefabRef> prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<SpawnableBuildingData> spawnableBuildings = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<BuildingPropertyData> buildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ConsumptionData> consumptionDatas = InternalCompilerInterface.GetComponentLookup<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<CityModifier> cityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Building> buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ElectricityConsumer> electricityConsumers = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<WaterConsumer> waterConsumers = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Game.Net.ServiceCoverage> serviceCoverages = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Locked> locked = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Transform> transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<GarbageProducer> garbageProducers = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<CrimeProducer> crimeProducers = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<MailProducer> mailProducers = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<OfficeBuilding> officeBuildings = InternalCompilerInterface.GetComponentLookup<OfficeBuilding>(ref __TypeHandle.__Game_Prefabs_OfficeBuilding_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Renter> renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Citizen> citizenDatas = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<HouseholdCitizen> householdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<BuildingData> buildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<CompanyData> companies = InternalCompilerInterface.GetComponentLookup<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<IndustrialProcessData> industrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<WorkProvider> workProviders = InternalCompilerInterface.GetComponentLookup<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Employee> employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<WorkplaceData> workplaceDatas = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Citizen> citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<HealthProblem> healthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ServiceAvailable> serviceAvailables = InternalCompilerInterface.GetComponentLookup<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ResourceData> resourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ZonePropertiesData> zonePropertiesDatas = InternalCompilerInterface.GetComponentLookup<ZonePropertiesData>(ref __TypeHandle.__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Efficiency> efficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ServiceCompanyData> serviceCompanyDatas = InternalCompilerInterface.GetComponentLookup<ServiceCompanyData>(ref __TypeHandle.__Game_Companies_ServiceCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<ResourceAvailability> availabilities = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<TradeCost> tradeCosts = InternalCompilerInterface.GetBufferLookup<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		CitizenHappinessParameterData singleton = ((EntityQuery)(ref m_CitizenHappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>();
		HealthcareParameterData singleton2 = ((EntityQuery)(ref m_HealthcareParameterQuery)).GetSingleton<HealthcareParameterData>();
		ParkParameterData singleton3 = ((EntityQuery)(ref m_ParkParameterQuery)).GetSingleton<ParkParameterData>();
		EducationParameterData singleton4 = ((EntityQuery)(ref m_EducationParameterQuery)).GetSingleton<EducationParameterData>();
		TelecomParameterData singleton5 = ((EntityQuery)(ref m_TelecomParameterQuery)).GetSingleton<TelecomParameterData>();
		DynamicBuffer<HappinessFactorParameterData> bufferAfterCompletingDependency = InternalCompilerInterface.GetBufferAfterCompletingDependency<HappinessFactorParameterData>(ref __TypeHandle.__Game_Prefabs_HappinessFactorParameterData_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef, ((EntityQuery)(ref m_HappinessFactorParameterQuery)).GetSingletonEntity());
		GarbageParameterData singleton6 = ((EntityQuery)(ref m_GarbageParameterQuery)).GetSingleton<GarbageParameterData>();
		EconomyParameterData economyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>();
		ServiceFeeParameterData singleton7 = ((EntityQuery)(ref __query_746694604_0)).GetSingleton<ServiceFeeParameterData>();
		JobHandle dependencies;
		NativeArray<GroundPollution> buffer = m_GroundPollutionSystem.GetData(readOnly: true, out dependencies).m_Buffer;
		JobHandle dependencies2;
		NativeArray<NoisePollution> buffer2 = m_NoisePollutionSystem.GetData(readOnly: true, out dependencies2).m_Buffer;
		JobHandle dependencies3;
		NativeArray<AirPollution> buffer3 = m_AirPollutionSystem.GetData(readOnly: true, out dependencies3).m_Buffer;
		JobHandle dependencies4;
		CellMapData<TelecomCoverage> data = m_TelecomCoverageSystem.GetData(readOnly: true, out dependencies4);
		NativeArray<int> taxRates = m_TaxSystem.GetTaxRates();
		ResourcePrefabs prefabs2 = m_ResourceSystem.GetPrefabs();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<ServiceFee> buffer4 = ((EntityManager)(ref entityManager)).GetBuffer<ServiceFee>(m_CitySystem.City, false);
		float relativeElectricityFee = ServiceFeeSystem.GetFee(PlayerResource.Electricity, buffer4) / singleton7.m_ElectricityFee.m_Default;
		float relativeWaterFee = ServiceFeeSystem.GetFee(PlayerResource.Water, buffer4) / singleton7.m_WaterFee.m_Default;
		NativeArray<int> factors = default(NativeArray<int>);
		factors._002Ector(28, (Allocator)2, (NativeArrayOptions)1);
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		((JobHandle)(ref dependencies3)).Complete();
		((JobHandle)(ref dependencies4)).Complete();
		CitizenHappinessSystem.GetBuildingHappinessFactors(entity, factors, ref prefabs, ref spawnableBuildings, ref buildingPropertyDatas, ref consumptionDatas, ref cityModifiers, ref buildings, ref electricityConsumers, ref waterConsumers, ref serviceCoverages, ref locked, ref transforms, ref garbageProducers, ref crimeProducers, ref mailProducers, ref officeBuildings, ref renters, ref citizenDatas, ref householdCitizens, ref buildingDatas, ref companies, ref industrialProcessDatas, ref workProviders, ref employees, ref workplaceDatas, ref citizens, ref healthProblems, ref serviceAvailables, ref resourceDatas, ref zonePropertiesDatas, ref efficiencies, ref serviceCompanyDatas, ref availabilities, ref tradeCosts, singleton, singleton6, singleton2, singleton3, singleton4, singleton5, ref economyParameters, bufferAfterCompletingDependency, buffer, buffer2, buffer3, data, m_CitySystem.City, taxRates, processes, prefabs2, relativeElectricityFee, relativeWaterFee);
		processes.Dispose();
		NativeList<BuildingHappinessFactorValue> val = default(NativeList<BuildingHappinessFactorValue>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < factors.Length; i++)
		{
			if (factors[i] != 0)
			{
				BuildingHappinessFactorValue buildingHappinessFactorValue = new BuildingHappinessFactorValue
				{
					m_Factor = (BuildingHappinessFactor)i,
					m_Value = factors[i]
				};
				val.Add(ref buildingHappinessFactorValue);
			}
		}
		NativeSortExtension.Sort<BuildingHappinessFactorValue>(val);
		string text = "";
		for (int j = 0; j < math.min(10, val.Length); j++)
		{
			text = text + val[j].m_Factor.ToString() + ": " + val[j].m_Value + "  ";
		}
		info.label = "Building happiness factors";
		info.value = text;
		info.target = InfoList.Item.kNullEntity;
		factors.Dispose();
		val.Dispose();
	}

	private bool HasZoneLevelInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
		if (EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefab, ref spawnableBuildingData))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ZoneData>(spawnableBuildingData.m_ZonePrefab))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<BuildingPropertyData>(prefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					return ((EntityManager)(ref entityManager)).HasComponent<BuildingCondition>(entity);
				}
			}
		}
		return false;
	}

	private void UpdateZoneLevelInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		SpawnableBuildingData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SpawnableBuildingData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		ZoneData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<ZoneData>(componentData.m_ZonePrefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		BuildingPropertyData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<BuildingPropertyData>(prefab);
		info.label = $"Level Progression (level {componentData.m_Level})";
		entityManager = ((ComponentSystemBase)this).EntityManager;
		info.value = ((EntityManager)(ref entityManager)).GetComponentData<BuildingCondition>(entity).m_Condition;
		Game.Zones.AreaType areaType = componentData2.m_AreaType;
		byte level = componentData.m_Level;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		info.max = BuildingUtils.GetLevelingCost(areaType, componentData3, level, ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true));
	}

	private bool HasPollutionInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<PollutionData>(prefab))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Abandoned>(entity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Destroyed>(entity))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(entity);
					}
				}
			}
			return true;
		}
		return false;
	}

	private void UpdatePollutionInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).CompleteDependency();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		bool destroyed = ((EntityManager)(ref entityManager)).HasComponent<Destroyed>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		bool abandoned = ((EntityManager)(ref entityManager)).HasComponent<Abandoned>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		bool isPark = ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(entity);
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		float efficiency = (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, entity, true, ref buffer) ? BuildingUtils.GetEfficiency(buffer) : 1f);
		DynamicBuffer<Renter> renters = default(DynamicBuffer<Renter>);
		EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, entity, true, ref renters);
		DynamicBuffer<InstalledUpgrade> installedUpgrades = default(DynamicBuffer<InstalledUpgrade>);
		EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, entity, true, ref installedUpgrades);
		PollutionParameterData singleton = ((EntityQuery)(ref __query_746694604_1)).GetSingleton<PollutionParameterData>();
		DynamicBuffer<CityModifier> singletonBuffer = ((EntityQuery)(ref __query_746694604_2)).GetSingletonBuffer<CityModifier>(true);
		ComponentLookup<PrefabRef> prefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<BuildingData> buildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<SpawnableBuildingData> spawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<PollutionData> pollutionDatas = InternalCompilerInterface.GetComponentLookup<PollutionData>(ref __TypeHandle.__Game_Prefabs_PollutionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<PollutionModifierData> pollutionModifierDatas = InternalCompilerInterface.GetComponentLookup<PollutionModifierData>(ref __TypeHandle.__Game_Prefabs_PollutionModifierData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ZoneData> zoneDatas = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Employee> employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<HouseholdCitizen> householdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Citizen> citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		PollutionData buildingPollution = BuildingPollutionAddSystem.GetBuildingPollution(prefab, destroyed, abandoned, isPark, efficiency, renters, installedUpgrades, singleton, singletonBuffer, ref prefabRefs, ref buildingDatas, ref spawnableDatas, ref pollutionDatas, ref pollutionModifierDatas, ref zoneDatas, ref employees, ref householdCitizens, ref citizens);
		info.label = "Pollution";
		info.value = "Ground: " + buildingPollution.m_GroundPollution + ". " + "Air: " + buildingPollution.m_AirPollution + ". " + "Noise: " + buildingPollution.m_NoisePollution + ".";
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasElectricityConsumeInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<ElectricityConsumer>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<ConsumptionData>(prefab);
		}
		return false;
	}

	private void UpdateElectricityConsumeInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		ConsumptionData data = ((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(prefab);
		AddUpgradeData<ConsumptionData>(entity, ref data);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		ElectricityConsumer componentData = ((EntityManager)(ref entityManager)).GetComponentData<ElectricityConsumer>(entity);
		info.label = "Electricity Consuming";
		info.value = $"consuming: {componentData.m_WantedConsumption}  fill: {componentData.m_FulfilledConsumption}";
	}

	private bool HasStorageInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<StorageLimitData>(prefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Resources>(entity);
		}
		return false;
	}

	private void UpdateStorageInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		info.label = "Resource Storage";
		StorageLimitData data = default(StorageLimitData);
		DynamicBuffer<Resources> val = default(DynamicBuffer<Resources>);
		if (EntitiesExtensions.TryGetComponent<StorageLimitData>(((ComponentSystemBase)this).EntityManager, prefab, ref data) && EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			AddUpgradeData<StorageLimitData>(entity, ref data);
			info.Add(new InfoList.Item($"Storage Limit: {data.m_Limit}"));
			for (int i = 0; i < val.Length; i++)
			{
				info.Add(new InfoList.Item($"{EconomyUtils.GetName(val[i].m_Resource)}({val[i].m_Amount})"));
			}
		}
	}

	private bool HasWaterConsumeInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<WaterConsumer>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<ConsumptionData>(prefab);
		}
		return false;
	}

	private void UpdateWaterConsumeInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		ConsumptionData data = ((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(prefab);
		AddUpgradeData<ConsumptionData>(entity, ref data);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		WaterConsumer componentData = ((EntityManager)(ref entityManager)).GetComponentData<WaterConsumer>(entity);
		info.label = "Water Consuming";
		info.value = $"consuming: {componentData.m_WantedConsumption}  fill: {componentData.m_FulfilledFresh}";
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasGarbageInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<GarbageProducer>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<ConsumptionData>(prefab);
		}
		return false;
	}

	private void UpdateGarbageInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		ConsumptionData data = ((EntityManager)(ref entityManager)).GetComponentData<ConsumptionData>(prefab);
		AddUpgradeData<ConsumptionData>(entity, ref data);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		GarbageProducer componentData = ((EntityManager)(ref entityManager)).GetComponentData<GarbageProducer>(entity);
		GarbageParameterData garbageParameter = ((EntityQuery)(ref __query_746694604_3)).GetSingleton<GarbageParameterData>();
		GarbageAccumulationSystem.GetGarbage(ref data, entity, prefab, ((SystemBase)this).GetBufferLookup<Renter>(true), ((SystemBase)this).GetBufferLookup<Game.Buildings.Student>(true), ((SystemBase)this).GetBufferLookup<Occupant>(true), ((SystemBase)this).GetComponentLookup<HomelessHousehold>(true), ((SystemBase)this).GetBufferLookup<HouseholdCitizen>(true), ((SystemBase)this).GetComponentLookup<Citizen>(true), ((SystemBase)this).GetBufferLookup<Employee>(true), ((SystemBase)this).GetBufferLookup<Patient>(true), ((SystemBase)this).GetComponentLookup<SpawnableBuildingData>(true), ((SystemBase)this).GetComponentLookup<CurrentDistrict>(true), ((SystemBase)this).GetBufferLookup<DistrictModifier>(true), ((SystemBase)this).GetComponentLookup<ZoneData>(true), ((SystemBase)this).GetBufferLookup<CityModifier>(true)[m_CitySystem.City], ref garbageParameter);
		int num = 0;
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			DynamicBuffer<HouseholdCitizen> val2 = default(DynamicBuffer<HouseholdCitizen>);
			for (int i = 0; i < val.Length; i++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<HomelessHousehold>(val[i].m_Renter) && EntitiesExtensions.TryGetBuffer<HouseholdCitizen>(((ComponentSystemBase)this).EntityManager, val[i].m_Renter, true, ref val2))
				{
					num += val2.Length;
				}
			}
		}
		string garbageStatus = GetGarbageStatus(Mathf.RoundToInt(data.m_GarbageAccumulation), componentData.m_Garbage, num, garbageParameter.m_HomelessGarbageProduce);
		info.label = "Garbage";
		info.value = garbageStatus;
		info.target = InfoList.Item.kNullEntity;
	}

	private string GetGarbageStatus(int accumulation, int garbage, int homeless, int homelessProduce)
	{
		return garbage + " (+" + accumulation + " / day) homeless(" + homeless + " * " + homelessProduce + "=" + homeless * homelessProduce + ")";
	}

	private bool HasSpectatorSiteInfo(Entity entity, Entity _)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		SpectatorSite spectatorSite = default(SpectatorSite);
		if (EntitiesExtensions.TryGetComponent<SpectatorSite>(((ComponentSystemBase)this).EntityManager, entity, ref spectatorSite))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Duration>(spectatorSite.m_Event);
		}
		return false;
	}

	private void UpdateSpectatorSiteInfo(Entity entity, Entity _, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		SpectatorSite componentData = ((EntityManager)(ref entityManager)).GetComponentData<SpectatorSite>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Duration componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Duration>(componentData.m_Event);
		if (m_SimulationSystem.frameIndex < componentData2.m_StartFrame)
		{
			info.label = "Preparing";
		}
		else if (m_SimulationSystem.frameIndex < componentData2.m_EndFrame)
		{
			info.label = "Event";
		}
		info.value = m_NameSystem.GetDebugName(componentData.m_Event);
		info.target = componentData.m_Event;
	}

	private bool HasInDangerInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		InDanger inDanger = default(InDanger);
		if (EntitiesExtensions.TryGetComponent<InDanger>(((ComponentSystemBase)this).EntityManager, entity, ref inDanger))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).Exists(inDanger.m_Event);
		}
		return false;
	}

	private void UpdateInDangerInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		InDanger componentData = ((EntityManager)(ref entityManager)).GetComponentData<InDanger>(entity);
		if ((componentData.m_Flags & DangerFlags.Evacuate) != 0)
		{
			info.label = "Evacuating";
		}
		else if ((componentData.m_Flags & DangerFlags.StayIndoors) != 0)
		{
			info.label = "In danger";
		}
		info.value = m_NameSystem.GetDebugName(componentData.m_Event);
		info.target = componentData.m_Event;
	}

	private bool HasVehicleInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<CompanyData>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Household>(entity))
				{
					return false;
				}
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<OwnedVehicle>(entity);
	}

	private void UpdateVehicleInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<OwnedVehicle> buffer = ((EntityManager)(ref entityManager)).GetBuffer<OwnedVehicle>(entity, true);
		int availableVehicles = VehicleUIUtils.GetAvailableVehicles(entity, ((ComponentSystemBase)this).EntityManager);
		info.label = $"Vehicles availableVehicles:{availableVehicles}";
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity vehicle = buffer[i].m_Vehicle;
			info.Add(new InfoList.Item(m_NameSystem.GetDebugName(vehicle), vehicle));
		}
	}

	private bool HasPoliceInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.PoliceStation>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			PoliceStationData data = default(PoliceStationData);
			if (((EntityManager)(ref entityManager)).HasComponent<Occupant>(entity) && EntitiesExtensions.TryGetComponent<PoliceStationData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
			{
				AddUpgradeData<PoliceStationData>(entity, ref data);
				return data.m_JailCapacity > 0;
			}
		}
		return false;
	}

	private void UpdatePoliceInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PoliceStationData data = ((EntityManager)(ref entityManager)).GetComponentData<PoliceStationData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Occupant> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Occupant>(entity, true);
		AddUpgradeData<PoliceStationData>(entity, ref data);
		info.label = "Arrested criminals";
		info.value = buffer.Length;
		info.max = data.m_JailCapacity;
	}

	private bool HasPrisonInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrisonData data = default(PrisonData);
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Prison>(entity) && EntitiesExtensions.TryGetComponent<PrisonData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Occupant>(entity))
			{
				AddUpgradeData<PrisonData>(entity, ref data);
				return data.m_PrisonerCapacity > 0;
			}
		}
		return false;
	}

	private void UpdatePrisonInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrisonData data = ((EntityManager)(ref entityManager)).GetComponentData<PrisonData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Occupant> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Occupant>(entity, true);
		AddUpgradeData<PrisonData>(entity, ref data);
		info.label = $"Prisoners ({buffer.Length})";
		info.Add(new InfoList.Item(buffer.Length + "/" + data.m_PrisonerCapacity));
		for (int i = 0; i < buffer.Length; i++)
		{
			info.Add(new InfoList.Item(m_NameSystem.GetDebugName(buffer[i].m_Occupant), buffer[i].m_Occupant));
		}
	}

	private bool HasResourceProductionInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Resources>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ResourceProducer>(entity);
		}
		return false;
	}

	private void UpdateResourceProductionInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		NativeList<ResourceProductionData> resources = default(NativeList<ResourceProductionData>);
		DynamicBuffer<ResourceProductionData> val = default(DynamicBuffer<ResourceProductionData>);
		if (EntitiesExtensions.TryGetBuffer<ResourceProductionData>(((ComponentSystemBase)this).EntityManager, prefab, true, ref val))
		{
			resources._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
			resources.AddRange(val.AsNativeArray());
		}
		DynamicBuffer<InstalledUpgrade> val2 = default(DynamicBuffer<InstalledUpgrade>);
		if (EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, entity, true, ref val2))
		{
			DynamicBuffer<ResourceProductionData> others = default(DynamicBuffer<ResourceProductionData>);
			for (int i = 0; i < val2.Length; i++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity prefab2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val2[i].m_Upgrade).m_Prefab;
				if (EntitiesExtensions.TryGetBuffer<ResourceProductionData>(((ComponentSystemBase)this).EntityManager, prefab2, true, ref others))
				{
					if (!resources.IsCreated)
					{
						resources._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
					}
					ResourceProductionData.Combine(resources, others);
				}
			}
		}
		info.label = "Resource Production";
		if (resources.IsCreated)
		{
			for (int j = 0; j < resources.Length; j++)
			{
				ResourceProductionData resourceProductionData = resources[j];
				int resources2 = EconomyUtils.GetResources(resourceProductionData.m_Type, buffer);
				info.Add(new InfoList.Item(string.Concat((EconomyUtils.GetName(resourceProductionData.m_Type), " ", resources2, "/", resourceProductionData.m_StorageCapacity))));
			}
			resources.Dispose();
		}
	}

	private bool HasShelterInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EmergencyShelterData data = default(EmergencyShelterData);
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.EmergencyShelter>(entity) && EntitiesExtensions.TryGetComponent<EmergencyShelterData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Occupant>(entity))
			{
				AddUpgradeData<EmergencyShelterData>(entity, ref data);
				return data.m_ShelterCapacity > 0;
			}
		}
		return false;
	}

	private void UpdateShelterInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EmergencyShelterData data = ((EntityManager)(ref entityManager)).GetComponentData<EmergencyShelterData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Occupant> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Occupant>(entity, true);
		AddUpgradeData<EmergencyShelterData>(entity, ref data);
		info.label = "Occupants";
		info.value = buffer.Length;
		info.max = data.m_ShelterCapacity;
	}

	private bool HasParkInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<ParkData>(prefab);
		}
		return false;
	}

	private void UpdateParkInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Buildings.Park componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.Park>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		ParkData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<ParkData>(prefab);
		info.label = "Maintenance";
		info.value = componentData.m_Maintenance + "/" + componentData2.m_MaintenancePool;
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasHouseholdInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Household>(entity);
	}

	private void UpdateHouseholdInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		info.label = "Household info";
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<HouseholdCitizen> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(entity, true);
		ComponentLookup<Worker> workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Citizen> citizenDatas = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<HealthProblem> healthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ResourceData> resourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		EconomyParameterData economyParameters = ((EntityQuery)(ref __query_746694604_4)).GetSingleton<EconomyParameterData>();
		int carCount = 0;
		DynamicBuffer<OwnedVehicle> val = default(DynamicBuffer<OwnedVehicle>);
		if (EntitiesExtensions.TryGetBuffer<OwnedVehicle>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			carCount = val.Length;
		}
		NativeArray<int> taxRates = m_TaxSystem.GetTaxRates();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Household componentData = ((EntityManager)(ref entityManager)).GetComponentData<Household>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		int resources = EconomyUtils.GetResources(Resource.Money, buffer2);
		int householdTotalWealth = EconomyUtils.GetHouseholdTotalWealth(componentData, buffer2);
		info.Add(new InfoList.Item("Wealth: " + householdTotalWealth));
		info.Add(new InfoList.Item("Income: " + EconomyUtils.GetHouseholdIncome(buffer, ref workers, ref citizenDatas, ref healthProblems, ref economyParameters, taxRates)));
		for (int i = 0; i < EconomyUtils.ResourceCount; i++)
		{
			info.Add(new InfoList.Item(string.Concat($"Shop Resource:{EconomyUtils.GetNameFixed(EconomyUtils.GetResource(i))} weight:{HouseholdBehaviorSystem.GetResourceShopWeightWithAge(resources, EconomyUtils.GetResource(i), m_ResourceSystem.GetPrefabs(), ref resourceDatas, carCount, leisureIncluded: false, buffer, ref citizenDatas)}")));
		}
	}

	private bool HasHouseholdsInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (((EntityManager)(ref entityManager)).HasComponent<Renter>(entity) && EntitiesExtensions.TryGetComponent<BuildingPropertyData>(((ComponentSystemBase)this).EntityManager, prefab, ref buildingPropertyData) && buildingPropertyData.m_ResidentialProperties > 0 && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			return val.Length > 0;
		}
		return false;
	}

	private void UpdateHouseholdsInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Renter> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Renter>(entity, true);
		info.label = $"Households ({buffer.Length})";
		Household householdData = default(Household);
		PropertyRenter propertyRenter = default(PropertyRenter);
		DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity renter = buffer[i].m_Renter;
			if (EntitiesExtensions.TryGetComponent<Household>(((ComponentSystemBase)this).EntityManager, renter, ref householdData) && EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, renter, ref propertyRenter) && EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, renter, true, ref resources))
			{
				info.Add(new InfoList.Item($"Name:{m_NameSystem.GetDebugName(renter)} Rent:{propertyRenter.m_Rent} Wealth:{EconomyUtils.GetHouseholdTotalWealth(householdData, resources)}", renter));
			}
		}
	}

	private bool HasResidentsInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
		if (((EntityManager)(ref entityManager)).HasComponent<Household>(entity) && EntitiesExtensions.TryGetBuffer<HouseholdCitizen>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			return val.Length > 0;
		}
		return false;
	}

	private void UpdateResidentInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<HouseholdCitizen> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(entity, true);
		info.label = $"Residents ({buffer.Length})";
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity citizen = buffer[i].m_Citizen;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(citizen))
			{
				info.Add(new InfoList.Item(m_NameSystem.GetDebugName(citizen), citizen));
			}
		}
	}

	private bool HasCompanyInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Entity company;
		return HasCompany(entity, prefab, out company);
	}

	private bool HasCompany(Entity entity, Entity prefab, out Entity company)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Renter>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
			if (((EntityManager)(ref entityManager)).HasComponent<BuildingPropertyData>(prefab) && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, entity, true, ref val) && val.Length > 0)
			{
				for (int i = 0; i < val.Length; i++)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(val[i].m_Renter))
					{
						company = val[i].m_Renter;
						return true;
					}
				}
			}
		}
		company = InfoList.Item.kNullEntity;
		return false;
	}

	private void UpdateCompanyInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Renter> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Renter>(entity, true);
		info.label = "Company";
		PropertyRenter propertyRenter = default(PropertyRenter);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity renter = buffer[i].m_Renter;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(renter) && EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, renter, ref propertyRenter))
			{
				info.value = $"Name:{m_NameSystem.GetDebugName(renter)} Rent:{propertyRenter.m_Rent}";
				info.target = renter;
			}
		}
	}

	private bool HasEmployeesInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Employee>(entity))
		{
			if (HasCompany(entity, prefab, out var company))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<Employee>(company);
			}
			return false;
		}
		return true;
	}

	private void UpdateEmployeesInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Employee> val = default(DynamicBuffer<Employee>);
		if (EntitiesExtensions.TryGetBuffer<Employee>(((ComponentSystemBase)this).EntityManager, entity, true, ref val) || (HasCompany(entity, prefab, out var company) && EntitiesExtensions.TryGetBuffer<Employee>(((ComponentSystemBase)this).EntityManager, company, true, ref val)))
		{
			info.label = $"Employees ({val.Length})";
			for (int i = 0; i < val.Length; i++)
			{
				info.Add(new InfoList.Item(m_NameSystem.GetDebugName(val[i].m_Worker), val[i].m_Worker));
			}
		}
	}

	private bool HasPatientsInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		HospitalData data = default(HospitalData);
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Hospital>(entity) || !EntitiesExtensions.TryGetComponent<HospitalData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
		{
			return false;
		}
		AddUpgradeData<HospitalData>(entity, ref data);
		return data.m_PatientCapacity > 0;
	}

	private void UpdatePatientsInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Patient> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Patient>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		HospitalData data = ((EntityManager)(ref entityManager)).GetComponentData<HospitalData>(prefab);
		AddUpgradeData<HospitalData>(entity, ref data);
		info.label = $"Patients ({buffer.Length})";
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity patient = buffer[i].m_Patient;
			info.Add(new InfoList.Item(m_NameSystem.GetDebugName(patient), patient));
		}
	}

	private bool HasStudentsInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.School>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SchoolData data = default(SchoolData);
			if (((EntityManager)(ref entityManager)).HasBuffer<Game.Buildings.Student>(entity) && EntitiesExtensions.TryGetComponent<SchoolData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
			{
				AddUpgradeData<SchoolData>(entity, ref data);
				return data.m_StudentCapacity > 0;
			}
		}
		return false;
	}

	private void UpdateStudentsInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Game.Buildings.Student> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Game.Buildings.Student>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		SchoolData data = ((EntityManager)(ref entityManager)).GetComponentData<SchoolData>(prefab);
		AddUpgradeData<SchoolData>(entity, ref data);
		info.label = $"Students ({buffer.Length})";
		DynamicBuffer<Efficiency> buffer2 = default(DynamicBuffer<Efficiency>);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity student = buffer[i].m_Student;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Citizen componentData = ((EntityManager)(ref entityManager)).GetComponentData<Citizen>(student);
			Random pseudoRandom = componentData.GetPseudoRandom(CitizenPseudoRandom.StudyWillingness);
			float studyWillingness = ((Random)(ref pseudoRandom)).NextFloat();
			float efficiency = (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, entity, true, ref buffer2) ? BuildingUtils.GetEfficiency(buffer2) : 1f);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<CityModifier> buffer3 = ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true);
			info.Add(new InfoList.Item(m_NameSystem.GetDebugName(student) + $"Graduation: {GraduationSystem.GetGraduationProbability(data.m_EducationLevel, componentData.m_WellBeing, data, buffer3, studyWillingness, efficiency)}", student));
		}
	}

	private bool HasDeathcareInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.DeathcareFacility>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DeathcareFacilityData data = default(DeathcareFacilityData);
			if (((EntityManager)(ref entityManager)).HasComponent<Patient>(entity) && EntitiesExtensions.TryGetComponent<DeathcareFacilityData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
			{
				AddUpgradeData<DeathcareFacilityData>(entity, ref data);
				return data.m_StorageCapacity > 0;
			}
		}
		return false;
	}

	private void UpdateDeathcareInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Patient> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Patient>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Buildings.DeathcareFacility componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.DeathcareFacility>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DeathcareFacilityData data = ((EntityManager)(ref entityManager)).GetComponentData<DeathcareFacilityData>(prefab);
		AddUpgradeData<DeathcareFacilityData>(entity, ref data);
		int value = componentData.m_LongTermStoredCount + buffer.Length;
		info.label = "Bodies";
		info.value = value;
		info.max = data.m_StorageCapacity;
	}

	private bool HasEfficiencyInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Efficiency>(entity);
		}
		PropertyRenter propertyRenter = default(PropertyRenter);
		if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, entity, ref propertyRenter))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Efficiency>(propertyRenter.m_Property);
		}
		return false;
	}

	private void UpdateEfficiencyInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		PropertyRenter propertyRenter = default(PropertyRenter);
		Entity val = ((!EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, entity, ref propertyRenter)) ? entity : propertyRenter.m_Property);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		float efficiency = BuildingUtils.GetEfficiency(((EntityManager)(ref entityManager)).GetBuffer<Efficiency>(val, true));
		info.label = "Efficiency";
		info.value = Mathf.RoundToInt(100f * efficiency) + " %";
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasStoredResourcesInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> val = default(DynamicBuffer<Resources>);
		if (((EntityManager)(ref entityManager)).HasComponent<StorageCompanyData>(prefab) && EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			return val.Length > 0;
		}
		return false;
	}

	private void UpdateStoredResourcesInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		StorageCompanyData componentData = ((EntityManager)(ref entityManager)).GetComponentData<StorageCompanyData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		ResourceIterator iterator = ResourceIterator.GetIterator();
		info.label = $"Stored Resources ({buffer.Length})";
		while (iterator.Next())
		{
			if ((componentData.m_StoredResources & iterator.resource) != Resource.NoResource)
			{
				int resources = EconomyUtils.GetResources(iterator.resource, buffer);
				info.Add(new InfoList.Item(EconomyUtils.GetName(iterator.resource) + resources));
			}
		}
	}

	private bool HasElectricityProductionInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<ElectricityProducer>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<PowerPlantData>(prefab);
		}
		return false;
	}

	private void UpdateElectricityProductionInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PowerPlantData data = ((EntityManager)(ref entityManager)).GetComponentData<PowerPlantData>(prefab);
		AddUpgradeData<PowerPlantData>(entity, ref data);
		int electricityProduction = data.m_ElectricityProduction;
		info.label = "Electricity Production";
		info.value = electricityProduction.ToString();
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasBatteriesInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Battery>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			BatteryData data = default(BatteryData);
			if (((EntityManager)(ref entityManager)).HasComponent<PowerPlantData>(prefab) && EntitiesExtensions.TryGetComponent<BatteryData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
			{
				AddUpgradeData<BatteryData>(entity, ref data);
				return data.m_Capacity > 0;
			}
		}
		return false;
	}

	private void UpdateBatteriesInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Buildings.Battery componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.Battery>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		BatteryData data = ((EntityManager)(ref entityManager)).GetComponentData<BatteryData>(prefab);
		AddUpgradeData<BatteryData>(entity, ref data);
		info.label = "Batteries";
		info.value = Mathf.RoundToInt(100f * (float)(componentData.m_StoredEnergy / data.capacityTicks)) + "%";
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasStoredGarbageInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		GarbageFacilityData data = default(GarbageFacilityData);
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.GarbageFacility>(entity) || !EntitiesExtensions.TryGetComponent<GarbageFacilityData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
		{
			return false;
		}
		AddUpgradeData<GarbageFacilityData>(entity, ref data);
		return data.m_GarbageCapacity > 0;
	}

	private void UpdateStoredGarbageInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		GarbageFacilityData data = ((EntityManager)(ref entityManager)).GetComponentData<GarbageFacilityData>(prefab);
		AddUpgradeData<GarbageFacilityData>(entity, ref data);
		PowerPlantData data2 = default(PowerPlantData);
		if (EntitiesExtensions.TryGetComponent<PowerPlantData>(((ComponentSystemBase)this).EntityManager, prefab, ref data2))
		{
			AddUpgradeData<PowerPlantData>(entity, ref data2);
		}
		int num = 0;
		DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
		if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, entity, true, ref resources))
		{
			num = EconomyUtils.GetResources(Resource.Garbage, resources);
		}
		DynamicBuffer<Game.Areas.SubArea> val = default(DynamicBuffer<Game.Areas.SubArea>);
		if (EntitiesExtensions.TryGetBuffer<Game.Areas.SubArea>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			Storage storage = default(Storage);
			StorageAreaData prefabStorageData = default(StorageAreaData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity area = val[i].m_Area;
				if (EntitiesExtensions.TryGetComponent<Storage>(((ComponentSystemBase)this).EntityManager, area, ref storage))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(area);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					Geometry componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Geometry>(area);
					if (EntitiesExtensions.TryGetComponent<StorageAreaData>(((ComponentSystemBase)this).EntityManager, componentData.m_Prefab, ref prefabStorageData))
					{
						data.m_GarbageCapacity += AreaUtils.CalculateStorageCapacity(componentData2, prefabStorageData);
						num += storage.m_Amount;
					}
				}
			}
		}
		info.label = "Stored Garbage";
		info.value = num;
		info.max = data.m_GarbageCapacity;
	}

	private bool HasGarbageProcessingInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		GarbageFacilityData data = default(GarbageFacilityData);
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.GarbageFacility>(entity) || !EntitiesExtensions.TryGetComponent<GarbageFacilityData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
		{
			return false;
		}
		AddUpgradeData<GarbageFacilityData>(entity, ref data);
		return data.m_ProcessingSpeed > 0;
	}

	private void UpdateGarbageProcessingInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Buildings.GarbageFacility componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.GarbageFacility>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		GarbageFacilityData data = ((EntityManager)(ref entityManager)).GetComponentData<GarbageFacilityData>(prefab);
		AddUpgradeData<GarbageFacilityData>(entity, ref data);
		info.label = "Garbage Processing Speed";
		info.value = componentData.m_ProcessingRate + "/" + data.m_ProcessingSpeed;
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasMailProcessingSpeedInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PostFacilityData data = default(PostFacilityData);
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.PostFacility>(entity) || !EntitiesExtensions.TryGetComponent<PostFacilityData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
		{
			return false;
		}
		AddUpgradeData<PostFacilityData>(entity, ref data);
		return data.m_SortingRate > 0;
	}

	private void UpdateMailProcessingSpeedInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Buildings.PostFacility componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Buildings.PostFacility>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PostFacilityData data = ((EntityManager)(ref entityManager)).GetComponentData<PostFacilityData>(prefab);
		AddUpgradeData<PostFacilityData>(entity, ref data);
		int num = (data.m_SortingRate * componentData.m_ProcessingFactor + 50) / 100;
		info.label = "Mail Processing Speed";
		info.value = num + "/" + data.m_SortingRate;
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasMailInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PostFacilityData data = default(PostFacilityData);
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.PostFacility>(entity) || !EntitiesExtensions.TryGetComponent<PostFacilityData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
		{
			return false;
		}
		AddUpgradeData<PostFacilityData>(entity, ref data);
		return data.m_MailCapacity > 0;
	}

	private void UpdateMailInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PostFacilityData data = ((EntityManager)(ref entityManager)).GetComponentData<PostFacilityData>(prefab);
		AddUpgradeData<PostFacilityData>(entity, ref data);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		int resources = EconomyUtils.GetResources(Resource.UnsortedMail, buffer);
		int resources2 = EconomyUtils.GetResources(Resource.LocalMail, buffer);
		int resources3 = EconomyUtils.GetResources(Resource.OutgoingMail, buffer);
		string text = ((data.m_PostVanCapacity > 0) ? ("Mail to deliver: " + resources2 + ". Collected mail: " + resources + ".") : ("Unsorted mail: " + resources + ". Local mail: " + resources2 + "."));
		if (data.m_SortingRate > 0 || resources3 > 0)
		{
			text = text + " Outgoing mail: " + resources3;
		}
		info.label = "Post Facility";
		info.value = text;
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasStoredMailInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PostFacilityData data = default(PostFacilityData);
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.PostFacility>(entity) && EntitiesExtensions.TryGetComponent<PostFacilityData>(((ComponentSystemBase)this).EntityManager, prefab, ref data))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Resources>(entity))
			{
				AddUpgradeData<PostFacilityData>(entity, ref data);
				return data.m_MailCapacity > 0;
			}
		}
		return false;
	}

	private void UpdateStoredMailInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PostFacilityData data = ((EntityManager)(ref entityManager)).GetComponentData<PostFacilityData>(prefab);
		AddUpgradeData<PostFacilityData>(entity, ref data);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		int resources = EconomyUtils.GetResources(Resource.UnsortedMail, buffer);
		int resources2 = EconomyUtils.GetResources(Resource.LocalMail, buffer);
		int resources3 = EconomyUtils.GetResources(Resource.OutgoingMail, buffer);
		int value = resources + resources2 + resources3;
		info.label = "Stored Mail";
		info.value = value;
		info.max = data.m_MailCapacity;
	}

	private bool HasSendReceiveMailInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<MailProducer>(entity);
	}

	private void UpdateSendReceiveMailInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		MailProducer componentData = ((EntityManager)(ref entityManager)).GetComponentData<MailProducer>(entity);
		info.label = "Send Receive Mail";
		info.value = $"Send: {componentData.m_SendingMail} Receive: {componentData.receivingMail}";
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasParkingInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Building>(entity);
	}

	private void UpdateParkingInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		int parkedCars = 0;
		int slotCapacity = 0;
		int parkingFee = 0;
		int laneCount = 0;
		string empty = string.Empty;
		NativeList<Entity> parkedCarList = default(NativeList<Entity>);
		parkedCarList._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, entity, true, ref subLanes))
		{
			CheckParkingLanes(subLanes, ref slotCapacity, ref parkedCars, ref parkingFee, ref laneCount, ref parkedCarList);
		}
		DynamicBuffer<Game.Net.SubNet> subNets = default(DynamicBuffer<Game.Net.SubNet>);
		if (EntitiesExtensions.TryGetBuffer<Game.Net.SubNet>(((ComponentSystemBase)this).EntityManager, entity, true, ref subNets))
		{
			CheckParkingLanes(subNets, ref slotCapacity, ref parkedCars, ref parkingFee, ref laneCount, ref parkedCarList);
		}
		DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
		if (EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(((ComponentSystemBase)this).EntityManager, entity, true, ref subObjects))
		{
			CheckParkingLanes(subObjects, ref slotCapacity, ref parkedCars, ref parkingFee, ref laneCount, ref parkedCarList);
		}
		info.label = $"Parking ({parkedCarList.Length})";
		BuildingData buildingData = default(BuildingData);
		if (laneCount != 0 && EntitiesExtensions.TryGetComponent<BuildingData>(((ComponentSystemBase)this).EntityManager, prefab, ref buildingData) && (buildingData.m_Flags & (Game.Prefabs.BuildingFlags.RestrictedPedestrian | Game.Prefabs.BuildingFlags.RestrictedCar)) == 0)
		{
			parkingFee /= laneCount;
			info.Add(new InfoList.Item("Parking Fee: " + parkingFee));
		}
		info.Add(new InfoList.Item(empty + " Parked Cars: " + parkedCars + "/" + slotCapacity + "."));
		for (int i = 0; i < parkedCarList.Length; i++)
		{
			info.Add(new InfoList.Item(m_NameSystem.GetDebugName(parkedCarList[i]), parkedCarList[i]));
		}
		parkedCarList.Dispose();
	}

	private void CheckParkingLanes(DynamicBuffer<Game.Objects.SubObject> subObjects, ref int slotCapacity, ref int parkedCars, ref int parkingFee, ref int laneCount, ref NativeList<Entity> parkedCarList)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		DynamicBuffer<Game.Objects.SubObject> subObjects2 = default(DynamicBuffer<Game.Objects.SubObject>);
		for (int i = 0; i < subObjects.Length; i++)
		{
			Entity subObject = subObjects[i].m_SubObject;
			if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, subObject, true, ref subLanes))
			{
				CheckParkingLanes(subLanes, ref slotCapacity, ref parkedCars, ref parkingFee, ref laneCount, ref parkedCarList);
			}
			if (EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(((ComponentSystemBase)this).EntityManager, subObject, true, ref subObjects2))
			{
				CheckParkingLanes(subObjects2, ref slotCapacity, ref parkedCars, ref parkingFee, ref laneCount, ref parkedCarList);
			}
		}
	}

	private void CheckParkingLanes(DynamicBuffer<Game.Net.SubNet> subNets, ref int slotCapacity, ref int parkedCars, ref int parkingFee, ref int laneCount, ref NativeList<Entity> parkedCarList)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		for (int i = 0; i < subNets.Length; i++)
		{
			Entity subNet = subNets[i].m_SubNet;
			if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, subNet, true, ref subLanes))
			{
				CheckParkingLanes(subLanes, ref slotCapacity, ref parkedCars, ref parkingFee, ref laneCount, ref parkedCarList);
			}
		}
	}

	private void CheckParkingLanes(DynamicBuffer<Game.Net.SubLane> subLanes, ref int slotCapacity, ref int parkedCars, ref int parkingFee, ref int laneCount, ref NativeList<Entity> parkedCarList)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		Game.Net.ParkingLane parkingLane = default(Game.Net.ParkingLane);
		GarageLane garageLane = default(GarageLane);
		for (int i = 0; i < subLanes.Length; i++)
		{
			Entity subLane = subLanes[i].m_SubLane;
			if (EntitiesExtensions.TryGetComponent<Game.Net.ParkingLane>(((ComponentSystemBase)this).EntityManager, subLane, ref parkingLane))
			{
				if ((parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
				{
					continue;
				}
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(subLane).m_Prefab;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Curve componentData = ((EntityManager)(ref entityManager)).GetComponentData<Curve>(subLane);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<LaneObject> buffer = ((EntityManager)(ref entityManager)).GetBuffer<LaneObject>(subLane, true);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				ParkingLaneData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<ParkingLaneData>(prefab);
				if (componentData2.m_SlotInterval != 0f)
				{
					int parkingSlotCount = NetUtils.GetParkingSlotCount(componentData, parkingLane, componentData2);
					slotCapacity += parkingSlotCount;
				}
				else
				{
					slotCapacity = -1000000;
				}
				for (int j = 0; j < buffer.Length; j++)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(buffer[j].m_LaneObject))
					{
						LaneObject laneObject = buffer[j];
						parkedCarList.Add(ref laneObject.m_LaneObject);
						parkedCars++;
					}
				}
				parkingFee += parkingLane.m_ParkingFee;
				laneCount++;
			}
			else if (EntitiesExtensions.TryGetComponent<GarageLane>(((ComponentSystemBase)this).EntityManager, subLane, ref garageLane))
			{
				slotCapacity += garageLane.m_VehicleCapacity;
				parkedCars += garageLane.m_VehicleCount;
				parkingFee += garageLane.m_ParkingFee;
				laneCount++;
			}
		}
	}

	private bool HasHouseholdPetsInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Household>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<HouseholdAnimal> val = default(DynamicBuffer<HouseholdAnimal>);
			if (((EntityManager)(ref entityManager)).HasComponent<HouseholdData>(prefab) && EntitiesExtensions.TryGetBuffer<HouseholdAnimal>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
			{
				return val.Length > 0;
			}
		}
		return false;
	}

	private void UpdateHouseholdPetsInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<HouseholdAnimal> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HouseholdAnimal>(entity, true);
		info.label = "Household Pets";
		for (int i = 0; i < buffer.Length; i++)
		{
			info.Add(new InfoList.Item(m_NameSystem.GetDebugName(buffer[i].m_HouseholdPet), buffer[i].m_HouseholdPet));
		}
	}

	private bool HasRentInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		PropertyRenter propertyRenter = default(PropertyRenter);
		if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, entity, ref propertyRenter))
		{
			return propertyRenter.m_Property != InfoList.Item.kNullEntity;
		}
		return false;
	}

	private void UpdateRentInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PropertyRenter componentData = ((EntityManager)(ref entityManager)).GetComponentData<PropertyRenter>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, false);
		info.label = "Rent";
		info.value = $"Rent: {componentData.m_Rent} Money:{EconomyUtils.GetResources(Resource.Money, buffer)}";
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasLandValueInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Renter>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Building>(entity);
		}
		return false;
	}

	private void UpdateLandValueInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Building componentData = ((EntityManager)(ref entityManager)).GetComponentData<Building>(entity);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		float num5 = 0f;
		EconomyParameterData economyParameterData = ((EntityQuery)(ref __query_746694604_4)).GetSingleton<EconomyParameterData>();
		BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
		if (EntitiesExtensions.TryGetComponent<BuildingPropertyData>(((ComponentSystemBase)this).EntityManager, prefab, ref buildingPropertyData))
		{
			LandValue landValue = default(LandValue);
			if (EntitiesExtensions.TryGetComponent<LandValue>(((ComponentSystemBase)this).EntityManager, componentData.m_RoadEdge, ref landValue))
			{
				num5 = landValue.m_LandValue;
			}
			ConsumptionData consumptionData = default(ConsumptionData);
			if (EntitiesExtensions.TryGetComponent<ConsumptionData>(((ComponentSystemBase)this).EntityManager, prefab, ref consumptionData))
			{
				num2 = consumptionData.m_Upkeep;
			}
			int lotSize = 0;
			BuildingData buildingData = default(BuildingData);
			if (EntitiesExtensions.TryGetComponent<BuildingData>(((ComponentSystemBase)this).EntityManager, prefab, ref buildingData))
			{
				lotSize = buildingData.m_LotSize.x * buildingData.m_LotSize.y;
			}
			Game.Zones.AreaType areaType = Game.Zones.AreaType.None;
			int buildingLevel = 1;
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			if (EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefab, ref spawnableBuildingData))
			{
				buildingLevel = spawnableBuildingData.m_Level;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				areaType = ((EntityManager)(ref entityManager)).GetComponentData<ZoneData>(spawnableBuildingData.m_ZonePrefab).m_AreaType;
				BuildingCondition buildingCondition = default(BuildingCondition);
				if (EntitiesExtensions.TryGetComponent<BuildingCondition>(((ComponentSystemBase)this).EntityManager, entity, ref buildingCondition))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					DynamicBuffer<CityModifier> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true);
					num4 = BuildingUtils.GetLevelingCost(areaType, buildingPropertyData, spawnableBuildingData.m_Level, buffer);
					num3 = buildingCondition.m_Condition;
				}
			}
			num = PropertyUtils.GetRentPricePerRenter(buildingPropertyData, buildingLevel, lotSize, num5, areaType, ref economyParameterData);
		}
		info.label = "Rent/Upkeep/Land value/Leveling";
		info.value = $"Rent per renter: {num} Upkeep: {num2} LV: {num5} Leveling {num3} / {num4}";
	}

	private bool HasTradeCostInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<TradeCost> val = default(DynamicBuffer<TradeCost>);
		return EntitiesExtensions.TryGetBuffer<TradeCost>(((ComponentSystemBase)this).EntityManager, entity, true, ref val);
	}

	private void UpdateTradeCostInfo(Entity entity, Entity prefab, InfoList infos)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		infos.label = "Trade Costs";
		DynamicBuffer<TradeCost> val = default(DynamicBuffer<TradeCost>);
		if (EntitiesExtensions.TryGetBuffer<TradeCost>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				TradeCost tradeCost = val[i];
				infos.Add(new InfoList.Item($"{EconomyUtils.GetName(tradeCost.m_Resource)} buy {tradeCost.m_BuyCost} sell {tradeCost.m_SellCost}"));
			}
		}
	}

	private bool HasTradePartnerInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Game.Companies.StorageCompany storageCompany = default(Game.Companies.StorageCompany);
		if (!EntitiesExtensions.TryGetComponent<Game.Companies.StorageCompany>(((ComponentSystemBase)this).EntityManager, entity, ref storageCompany) || !(storageCompany.m_LastTradePartner != InfoList.Item.kNullEntity))
		{
			BuyingCompany buyingCompany = default(BuyingCompany);
			if (EntitiesExtensions.TryGetComponent<BuyingCompany>(((ComponentSystemBase)this).EntityManager, entity, ref buyingCompany))
			{
				return buyingCompany.m_LastTradePartner != InfoList.Item.kNullEntity;
			}
			return false;
		}
		return true;
	}

	private void UpdateTradePartnerInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Game.Companies.StorageCompany storageCompany = default(Game.Companies.StorageCompany);
		string debugName;
		Entity lastTradePartner;
		if (EntitiesExtensions.TryGetComponent<Game.Companies.StorageCompany>(((ComponentSystemBase)this).EntityManager, entity, ref storageCompany) && storageCompany.m_LastTradePartner != InfoList.Item.kNullEntity)
		{
			debugName = m_NameSystem.GetDebugName(storageCompany.m_LastTradePartner);
			lastTradePartner = storageCompany.m_LastTradePartner;
		}
		else
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			BuyingCompany componentData = ((EntityManager)(ref entityManager)).GetComponentData<BuyingCompany>(entity);
			debugName = m_NameSystem.GetDebugName(componentData.m_LastTradePartner);
			lastTradePartner = componentData.m_LastTradePartner;
		}
		info.label = "Trade Partner";
		info.value = debugName;
		info.target = lastTradePartner;
	}

	private bool HasWarehouseInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Companies.StorageCompany>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<StorageCompanyData>(prefab);
		}
		return false;
	}

	private void UpdateWarehouseInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		StorageCompanyData componentData = ((EntityManager)(ref entityManager)).GetComponentData<StorageCompanyData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TradeCost> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TradeCost>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		TradeCost tradeCost = EconomyUtils.GetTradeCost(componentData.m_StoredResources, buffer);
		int resources = EconomyUtils.GetResources(componentData.m_StoredResources, buffer2);
		info.label = "Warehouse - ";
		info.value = "Stores: " + EconomyUtils.GetName(componentData.m_StoredResources) + " (" + resources + "). Buy Cost: " + tradeCost.m_BuyCost.ToString("F1") + ". Sell Cost: " + tradeCost.m_SellCost.ToString("F1");
		info.target = InfoList.Item.kNullEntity;
	}

	private bool HasCompanyEconomyInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<CompanyData>(entity);
	}

	private void UpdateCompanyEconomyInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).HasComponent<ServiceAvailable>(entity);
		((EntityQuery)(ref __query_746694604_4)).GetSingleton<EconomyParameterData>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(prefab);
		ComponentLookup<ResourceData> resourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Game.Vehicles.DeliveryTruck> deliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<LayoutElement> layouts = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		DynamicBuffer<OwnedVehicle> vehicles = default(DynamicBuffer<OwnedVehicle>);
		int num = ((!EntitiesExtensions.TryGetBuffer<OwnedVehicle>(((ComponentSystemBase)this).EntityManager, entity, true, ref vehicles)) ? EconomyUtils.GetCompanyTotalWorth(buffer, prefabs, ref resourceDatas) : EconomyUtils.GetCompanyTotalWorth(buffer, vehicles, ref layouts, ref deliveryTrucks, prefabs, ref resourceDatas));
		info.label = "Company Economy - ";
		info.value = WorthToString(num) + " (" + num + ").";
		info.target = InfoList.Item.kNullEntity;
	}

	private string WorthToString(int worth)
	{
		if (worth < -7500)
		{
			return "Bankrupt ";
		}
		if (worth < -1000)
		{
			return "Poor ";
		}
		if (worth < 1000)
		{
			return "Stable ";
		}
		return "Wealthy ";
	}

	private bool HasCompanyProfitInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Profitability>(entity);
	}

	private void UpdateCompanyProfitInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		Profitability profitability = default(Profitability);
		if (!EntitiesExtensions.TryGetComponent<Profitability>(((ComponentSystemBase)this).EntityManager, entity, ref profitability))
		{
			return;
		}
		info.label = "Company Profit";
		info.Add(new InfoList.Item($"profitability: {profitability.m_Profitability}"));
		info.Add(new InfoList.Item($"Last Day Total Worth: {profitability.m_LastTotalWorth}"));
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		EconomyParameterData econParams = ((EntityQuery)(ref __query_746694604_4)).GetSingleton<EconomyParameterData>();
		ComponentLookup<ResourceData> resourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Citizen> citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		int num = 0;
		int num2 = 0;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		bool isIndustrial = !((EntityManager)(ref entityManager)).HasComponent<ServiceAvailable>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		bool flag = ((EntityManager)(ref entityManager)).HasComponent<Game.Companies.ExtractorCompany>(entity);
		IndustrialProcessData processData = default(IndustrialProcessData);
		if (!EntitiesExtensions.TryGetComponent<IndustrialProcessData>(((ComponentSystemBase)this).EntityManager, prefab, ref processData))
		{
			return;
		}
		float num3 = 0f;
		float concentration = 0f;
		int buildingLevel = 1;
		BufferLookup<Game.Areas.SubArea> subAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<InstalledUpgrade> installedUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Extractor> extractors = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Attached> attacheds = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Geometry> geometries = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Game.Areas.Lot> lots = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ExtractorAreaData> extractorDatas = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<PrefabRef> prefabs2 = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ServiceCompanyData> serviceCompanyDatas = InternalCompilerInterface.GetComponentLookup<ServiceCompanyData>(ref __TypeHandle.__Game_Companies_ServiceCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<BuildingData> buildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<BuildingPropertyData> buildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<SpawnableBuildingData> spawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<IndustrialProcessData> industrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ExtractorCompanyData> extractorCompanyDatas = InternalCompilerInterface.GetComponentLookup<ExtractorCompanyData>(ref __TypeHandle.__Game_Prefabs_ExtractorCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		PropertyRenter propertyRenter = default(PropertyRenter);
		if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, entity, ref propertyRenter))
		{
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			num3 = (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, true, ref buffer) ? BuildingUtils.GetEfficiency(buffer) : 1f);
			PrefabRef prefabRef = default(PrefabRef);
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref prefabRef) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref spawnableBuildingData))
			{
				buildingLevel = spawnableBuildingData.m_Level;
			}
			Attached attached = default(Attached);
			if (EntitiesExtensions.TryGetComponent<Attached>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref attached))
			{
				ExtractorCompanySystem.GetBestConcentration(processData.m_Output.m_Resource, attached.m_Parent, ref subAreas, ref installedUpgrades, ref extractors, ref geometries, ref prefabs2, ref extractorDatas, ((EntityQuery)(ref __query_746694604_5)).GetSingleton<ExtractorParameterData>(), m_ResourceSystem.GetPrefabs(), ref resourceDatas, out concentration, out var _);
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Employee> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<Employee>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		WorkProvider componentData = ((EntityManager)(ref entityManager)).GetComponentData<WorkProvider>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		WorkplaceData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<WorkplaceData>(prefab);
		num2 = EconomyUtils.CalculateTotalWage(buffer2, ref econParams);
		int num4 = EconomyUtils.CalculateTotalWage(componentData.m_MaxWorkers, componentData2.m_Complexity, buildingLevel, econParams);
		num = EconomyUtils.GetCompanyProductionPerDay(num3, isIndustrial, buffer2, processData, prefabs, ref resourceDatas, ref citizens, ref econParams);
		info.Add(flag ? new InfoList.Item($"efficiency:{num3 * 100f}% concentration:{concentration}") : new InfoList.Item($"efficiency:{num3 * 100f}%"));
		info.Add(new InfoList.Item("Wages: " + num2));
		info.Add(new InfoList.Item($"maxWages:{num4}"));
		int companyMaxFittingWorkers = CompanyUtils.GetCompanyMaxFittingWorkers(entity, propertyRenter.m_Property, ref prefabs2, ref serviceCompanyDatas, ref buildingDatas, ref buildingPropertyDatas, ref spawnableBuildingDatas, ref industrialProcessDatas, ref extractorCompanyDatas, ref attacheds, ref subAreas, ref installedUpgrades, ref lots, ref geometries);
		info.Add(new InfoList.Item($"maxFittingWorkers(not current max worker):{companyMaxFittingWorkers}"));
		info.Add(new InfoList.Item($"Production Per Day: {num} * {EconomyUtils.GetNameFixed(processData.m_Output.m_Resource)}"));
		if (processData.m_Input1.m_Resource != Resource.NoResource)
		{
			info.Add(new InfoList.Item($"Input1: {processData.m_Input1.m_Amount}*{EconomyUtils.GetNameFixed(processData.m_Input1.m_Resource)}({EconomyUtils.GetIndustrialPrice(processData.m_Input1.m_Resource, prefabs, ref resourceDatas)})"));
		}
		if (processData.m_Input2.m_Resource != Resource.NoResource)
		{
			info.Add(new InfoList.Item($"Input2: {processData.m_Input2.m_Amount}*{EconomyUtils.GetNameFixed(processData.m_Input2.m_Resource)}({EconomyUtils.GetIndustrialPrice(processData.m_Input2.m_Resource, prefabs, ref resourceDatas)})"));
		}
		if (processData.m_Output.m_Resource != Resource.NoResource)
		{
			info.Add(new InfoList.Item($"Output: {processData.m_Output.m_Amount}*{EconomyUtils.GetNameFixed(processData.m_Output.m_Resource)}({EconomyUtils.GetIndustrialPrice(processData.m_Output.m_Resource, prefabs, ref resourceDatas)})"));
		}
	}

	private bool HasServiceCompanyInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ServiceAvailable>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<Resources>(entity);
			}
		}
		return false;
	}

	private void UpdateServiceCompanyInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		ServiceAvailable componentData = ((EntityManager)(ref entityManager)).GetComponentData<ServiceAvailable>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		ServiceCompanyData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<ServiceCompanyData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		info.label = "Service Company";
		entityManager = ((ComponentSystemBase)this).EntityManager;
		IndustrialProcessData industrialProcessData = default(IndustrialProcessData);
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Companies.ProcessingCompany>(entity) && EntitiesExtensions.TryGetComponent<IndustrialProcessData>(((ComponentSystemBase)this).EntityManager, prefab, ref industrialProcessData))
		{
			Resource resource = industrialProcessData.m_Output.m_Resource;
			info.Add(new InfoList.Item(string.Concat("Service: ", componentData.m_ServiceAvailable + "/" + componentData2.m_MaxService + "(" + ServicesToString(componentData.m_ServiceAvailable, componentData2.m_MaxService) + ")")));
			info.Add(new InfoList.Item("Provide Resource Storage: " + EconomyUtils.GetName(resource) + " (" + EconomyUtils.GetResources(resource, buffer) + ")"));
			PropertyRenter propertyRenter = default(PropertyRenter);
			LodgingProvider lodgingProvider = default(LodgingProvider);
			if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, entity, ref propertyRenter) && propertyRenter.m_Property != InfoList.Item.kNullEntity && EntitiesExtensions.TryGetComponent<LodgingProvider>(((ComponentSystemBase)this).EntityManager, entity, ref lodgingProvider) && resource == Resource.Lodging)
			{
				Entity property = propertyRenter.m_Property;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity prefab2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(property).m_Prefab;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				SpawnableBuildingData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<SpawnableBuildingData>(prefab2);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				BuildingPropertyData componentData4 = ((EntityManager)(ref entityManager)).GetComponentData<BuildingPropertyData>(prefab2);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				int roomCount = LodgingProviderSystem.GetRoomCount(((EntityManager)(ref entityManager)).GetComponentData<BuildingData>(prefab2).m_LotSize, componentData3.m_Level, componentData4);
				info.Add(new InfoList.Item("Lodging rooms free: " + lodgingProvider.m_FreeRooms + "/" + roomCount));
			}
		}
		BuyingCompany buyingCompany = default(BuyingCompany);
		if (EntitiesExtensions.TryGetComponent<BuyingCompany>(((ComponentSystemBase)this).EntityManager, entity, ref buyingCompany))
		{
			info.Add(new InfoList.Item("Trip Length: " + buyingCompany.m_MeanInputTripLength));
		}
	}

	private string ServicesToString(int services, int maxServices)
	{
		float num = (float)services / (float)maxServices;
		if (services <= 0)
		{
			return "Overworked";
		}
		if (num < 0.2f)
		{
			return "Busy";
		}
		if (num < 0.8f)
		{
			return "Operational";
		}
		return "Low on customers";
	}

	private bool HasExtractorCompanyInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Companies.ExtractorCompany>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<IndustrialProcessData>(prefab))
				{
					return true;
				}
			}
		}
		Owner owner = default(Owner);
		Attachment attachment = default(Attachment);
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, entity, ref owner) && EntitiesExtensions.TryGetComponent<Attachment>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref attachment) && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, attachment.m_Attached, true, ref val))
		{
			return val.Length > 0;
		}
		return false;
	}

	private void UpdateExtractorCompanyInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		Owner owner = default(Owner);
		Attachment attachment = default(Attachment);
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		EntityManager entityManager;
		if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, entity, ref owner) && EntitiesExtensions.TryGetComponent<Attachment>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref attachment) && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, attachment.m_Attached, true, ref val) && val.Length > 0)
		{
			entity = val[0].m_Renter;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity).m_Prefab;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		IndustrialProcessData componentData = ((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(prefab);
		info.label = "Extractor Company";
		Resource resource = componentData.m_Output.m_Resource;
		info.Add(new InfoList.Item("Produces: " + EconomyUtils.GetName(resource) + " (" + EconomyUtils.GetResources(resource, buffer) + ")"));
		PropertyRenter propertyRenter = default(PropertyRenter);
		Attached attached = default(Attached);
		WorkplaceData workplaceData = default(WorkplaceData);
		PrefabRef prefabRef = default(PrefabRef);
		IndustrialProcessData industrialProcessData = default(IndustrialProcessData);
		if (!EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, entity, ref propertyRenter) || !(propertyRenter.m_Property != InfoList.Item.kNullEntity) || !EntitiesExtensions.TryGetComponent<Attached>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref attached) || !EntitiesExtensions.TryGetComponent<WorkplaceData>(((ComponentSystemBase)this).EntityManager, prefab, ref workplaceData) || !EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref prefabRef) || !EntitiesExtensions.TryGetComponent<IndustrialProcessData>(((ComponentSystemBase)this).EntityManager, prefab, ref industrialProcessData))
		{
			return;
		}
		((EntityQuery)(ref __query_746694604_5)).GetSingleton<ExtractorParameterData>();
		((EntityQuery)(ref __query_746694604_4)).GetSingleton<EconomyParameterData>();
		InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>().GetPrefabs();
		InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		DynamicBuffer<Game.Areas.SubArea> val2 = default(DynamicBuffer<Game.Areas.SubArea>);
		if (EntitiesExtensions.TryGetBuffer<Game.Areas.SubArea>(((ComponentSystemBase)this).EntityManager, attached.m_Parent, true, ref val2))
		{
			Enumerator<Game.Areas.SubArea> enumerator = val2.GetEnumerator();
			try
			{
				Extractor extractor = default(Extractor);
				while (enumerator.MoveNext())
				{
					Game.Areas.SubArea current = enumerator.Current;
					info.Add(new InfoList.Item("Area:" + m_NameSystem.GetDebugName(attached.m_Parent), current.m_Area));
					if (EntitiesExtensions.TryGetComponent<Extractor>(((ComponentSystemBase)this).EntityManager, current.m_Area, ref extractor))
					{
						info.Add(new InfoList.Item($"ResourceAmount: {extractor.m_ResourceAmount}"));
						info.Add(new InfoList.Item($"WorkAmount(Harvest Work): {extractor.m_WorkAmount}"));
						info.Add(new InfoList.Item($"HarvestedAmount(Collect Work): {extractor.m_HarvestedAmount}"));
						info.Add(new InfoList.Item($"ExtractedAmount: {extractor.m_ExtractedAmount}"));
						info.Add(new InfoList.Item($"TotalExtracted: {extractor.m_TotalExtracted}"));
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		DynamicBuffer<InstalledUpgrade> val3 = default(DynamicBuffer<InstalledUpgrade>);
		if (!EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, attached.m_Parent, true, ref val3))
		{
			return;
		}
		Enumerator<InstalledUpgrade> enumerator2 = val3.GetEnumerator();
		try
		{
			Extractor extractor2 = default(Extractor);
			while (enumerator2.MoveNext())
			{
				InstalledUpgrade current2 = enumerator2.Current;
				info.Add(new InfoList.Item("InstalledUpgrade:" + m_NameSystem.GetDebugName(current2), current2));
				if (!EntitiesExtensions.TryGetBuffer<Game.Areas.SubArea>(((ComponentSystemBase)this).EntityManager, (Entity)current2, true, ref val2))
				{
					continue;
				}
				Enumerator<Game.Areas.SubArea> enumerator = val2.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Game.Areas.SubArea current3 = enumerator.Current;
						info.Add(new InfoList.Item("Area:" + m_NameSystem.GetDebugName(attached.m_Parent), current3.m_Area));
						if (EntitiesExtensions.TryGetComponent<Extractor>(((ComponentSystemBase)this).EntityManager, current3.m_Area, ref extractor2))
						{
							info.Add(new InfoList.Item($"ResourceAmount: {extractor2.m_ResourceAmount}"));
							info.Add(new InfoList.Item($"WorkAmount(Harvest Work): {extractor2.m_WorkAmount}"));
							info.Add(new InfoList.Item($"HarvestedAmount(Collect Work): {extractor2.m_HarvestedAmount}"));
							info.Add(new InfoList.Item($"ExtractedAmount: {extractor2.m_ExtractedAmount}"));
							info.Add(new InfoList.Item($"TotalExtracted: {extractor2.m_TotalExtracted}"));
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private bool HasProcessingCompanyInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Companies.ProcessingCompany>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<IndustrialProcessData>(prefab);
			}
		}
		return false;
	}

	private void UpdateProcessingCompanyInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		IndustrialProcessData componentData = ((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(prefab);
		ComponentLookup<Citizen> citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		info.label = "Processing Company";
		Resource resource = componentData.m_Input1.m_Resource;
		Resource resource2 = componentData.m_Input2.m_Resource;
		Resource resource3 = componentData.m_Output.m_Resource;
		info.Add(new InfoList.Item("In: " + EconomyUtils.GetName(resource) + " (" + EconomyUtils.GetResources(resource, buffer) + ")"));
		if (resource2 != Resource.NoResource)
		{
			info.Add(new InfoList.Item("In: " + EconomyUtils.GetName(resource2) + " (" + EconomyUtils.GetResources(resource2, buffer) + ")"));
		}
		info.Add(new InfoList.Item("Out: " + EconomyUtils.GetName(resource3) + " (" + EconomyUtils.GetResources(resource3, buffer) + ")"));
		EconomyParameterData singleton = ((EntityQuery)(ref __query_746694604_4)).GetSingleton<EconomyParameterData>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<ServiceAvailable>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).HasComponent<Game.Companies.ExtractorCompany>(entity);
		}
		DynamicBuffer<Employee> employees = default(DynamicBuffer<Employee>);
		PropertyRenter propertyRenter = default(PropertyRenter);
		PrefabRef prefabRef = default(PrefabRef);
		WorkplaceData workplaceData = default(WorkplaceData);
		SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
		if (EntitiesExtensions.TryGetBuffer<Employee>(((ComponentSystemBase)this).EntityManager, entity, true, ref employees) && EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, entity, ref propertyRenter) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref prefabRef) && EntitiesExtensions.TryGetComponent<WorkplaceData>(((ComponentSystemBase)this).EntityManager, prefab, ref workplaceData) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, (Entity)prefabRef, ref spawnableBuildingData))
		{
			float workforce = EconomyUtils.GetWorkforce(employees, ref citizens);
			info.Add(new InfoList.Item($"Workforce Per Tick:{workforce}"));
			DynamicBuffer<Efficiency> buffer2 = default(DynamicBuffer<Efficiency>);
			float num = (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, true, ref buffer2) ? BuildingUtils.GetEfficiency(buffer2) : 1f);
			info.Add(new InfoList.Item($"Building Efficiency:{num}"));
		}
		BuyingCompany buyingCompany = default(BuyingCompany);
		if (EntitiesExtensions.TryGetComponent<BuyingCompany>(((ComponentSystemBase)this).EntityManager, entity, ref buyingCompany))
		{
			info.Add(new InfoList.Item("Trip Length: " + buyingCompany.m_MeanInputTripLength));
		}
	}

	private bool HasOwnerInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Owner>(entity);
		}
		return false;
	}

	private void UpdateOwnerInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Owner componentData = ((EntityManager)(ref entityManager)).GetComponentData<Owner>(entity);
		info.label = "Owner";
		info.value = m_NameSystem.GetDebugName(componentData.m_Owner);
		info.target = componentData.m_Owner;
	}

	private bool HasKeeperInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.PersonalCar personalCar = default(Game.Vehicles.PersonalCar);
		if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity) && EntitiesExtensions.TryGetComponent<Game.Vehicles.PersonalCar>(((ComponentSystemBase)this).EntityManager, entity, ref personalCar))
		{
			return personalCar.m_Keeper != InfoList.Item.kNullEntity;
		}
		return false;
	}

	private void UpdateKeeperInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.PersonalCar componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.PersonalCar>(entity);
		info.label = "Keeper";
		info.value = m_NameSystem.GetDebugName(componentData.m_Keeper);
		info.target = componentData.m_Keeper;
	}

	private bool HasControllerInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Controller controller = default(Controller);
		if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity) && EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, entity, ref controller))
		{
			return controller.m_Controller != InfoList.Item.kNullEntity;
		}
		return false;
	}

	private void UpdateControllerInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Controller componentData = ((EntityManager)(ref entityManager)).GetComponentData<Controller>(entity);
		info.label = "Controller";
		info.value = m_NameSystem.GetDebugName(componentData.m_Controller);
		info.target = componentData.m_Controller;
	}

	private bool HasTransferRequestInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<StorageTransferRequest>(entity);
	}

	private void UpdateTransferRequestInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<StorageTransferRequest> buffer = ((EntityManager)(ref entityManager)).GetBuffer<StorageTransferRequest>(entity, true);
		info.label = "Transfer requests";
		for (int i = 0; i < buffer.Length; i++)
		{
			StorageTransferRequest storageTransferRequest = buffer[i];
			info.Add(new InfoList.Item(string.Format("{0} {1} {2} {3} {4}", storageTransferRequest.m_Amount, EconomyUtils.GetName(storageTransferRequest.m_Resource), ((storageTransferRequest.m_Flags & StorageTransferFlags.Incoming) != 0) ? " from " : " to ", storageTransferRequest.m_Target.Index, ((storageTransferRequest.m_Flags & StorageTransferFlags.Car) != 0) ? "(C)" : "")));
		}
	}

	private bool HasPassengerInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		int num = 0;
		PersonalCarData personalCarData = default(PersonalCarData);
		PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
		AmbulanceData ambulanceData = default(AmbulanceData);
		HearseData hearseData = default(HearseData);
		PoliceCarData policeCarData = default(PoliceCarData);
		TaxiData taxiData = default(TaxiData);
		if (EntitiesExtensions.TryGetComponent<PersonalCarData>(((ComponentSystemBase)this).EntityManager, prefab, ref personalCarData))
		{
			num = personalCarData.m_PassengerCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(((ComponentSystemBase)this).EntityManager, prefab, ref publicTransportVehicleData))
		{
			num = publicTransportVehicleData.m_PassengerCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<AmbulanceData>(((ComponentSystemBase)this).EntityManager, prefab, ref ambulanceData))
		{
			num = ambulanceData.m_PatientCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<HearseData>(((ComponentSystemBase)this).EntityManager, prefab, ref hearseData))
		{
			num = hearseData.m_CorpseCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<PoliceCarData>(((ComponentSystemBase)this).EntityManager, prefab, ref policeCarData))
		{
			num = policeCarData.m_CriminalCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<TaxiData>(((ComponentSystemBase)this).EntityManager, prefab, ref taxiData))
		{
			num = taxiData.m_PassengerCapacity;
		}
		return num > 0;
	}

	private void UpdatePassengerInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		int max = 0;
		int value = 0;
		PersonalCarData personalCarData = default(PersonalCarData);
		PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
		AmbulanceData ambulanceData = default(AmbulanceData);
		HearseData hearseData = default(HearseData);
		PoliceCarData policeCarData = default(PoliceCarData);
		TaxiData taxiData = default(TaxiData);
		if (EntitiesExtensions.TryGetComponent<PersonalCarData>(((ComponentSystemBase)this).EntityManager, prefab, ref personalCarData))
		{
			max = personalCarData.m_PassengerCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(((ComponentSystemBase)this).EntityManager, prefab, ref publicTransportVehicleData))
		{
			max = publicTransportVehicleData.m_PassengerCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<AmbulanceData>(((ComponentSystemBase)this).EntityManager, prefab, ref ambulanceData))
		{
			max = ambulanceData.m_PatientCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<HearseData>(((ComponentSystemBase)this).EntityManager, prefab, ref hearseData))
		{
			max = hearseData.m_CorpseCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<PoliceCarData>(((ComponentSystemBase)this).EntityManager, prefab, ref policeCarData))
		{
			max = policeCarData.m_CriminalCapacity;
		}
		else if (EntitiesExtensions.TryGetComponent<TaxiData>(((ComponentSystemBase)this).EntityManager, prefab, ref taxiData))
		{
			max = taxiData.m_PassengerCapacity;
		}
		DynamicBuffer<Passenger> val = default(DynamicBuffer<Passenger>);
		if (EntitiesExtensions.TryGetBuffer<Passenger>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			value = val.Length;
		}
		info.label = "Passengers";
		info.value = value;
		info.max = max;
	}

	private bool HasPersonalCarInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PersonalCar>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<PersonalCarData>(prefab);
		}
		return false;
	}

	private void UpdatePersonalCarInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.PersonalCar componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.PersonalCar>(entity);
		info.label = "Personal Car";
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(entity))
		{
			info.Add(new InfoList.Item("Parked"));
		}
		if ((componentData.m_State & PersonalCarFlags.Boarding) != 0)
		{
			info.Add(new InfoList.Item("Boarding"));
		}
		else if ((componentData.m_State & PersonalCarFlags.Disembarking) != 0)
		{
			info.Add(new InfoList.Item("Disembarking"));
		}
		else if ((componentData.m_State & PersonalCarFlags.Transporting) != 0)
		{
			info.Add(new InfoList.Item("Transporting"));
		}
		if ((componentData.m_State & PersonalCarFlags.DummyTraffic) != 0)
		{
			info.Add(new InfoList.Item("Dummy Traffic"));
		}
		if ((componentData.m_State & PersonalCarFlags.HomeTarget) != 0)
		{
			info.Add(new InfoList.Item("Home Target"));
		}
	}

	private bool HasDeliveryTruckInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.DeliveryTruck>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<DeliveryTruckData>(prefab);
		}
		return false;
	}

	private void UpdateDeliveryTruckInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.DeliveryTruck componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.DeliveryTruck>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DeliveryTruckData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<DeliveryTruckData>(prefab);
		Resource resource = Resource.NoResource;
		int num = 0;
		int num2 = 0;
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val) && val.Length != 0)
		{
			Game.Vehicles.DeliveryTruck deliveryTruck = default(Game.Vehicles.DeliveryTruck);
			DeliveryTruckData deliveryTruckData = default(DeliveryTruckData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				if (EntitiesExtensions.TryGetComponent<Game.Vehicles.DeliveryTruck>(((ComponentSystemBase)this).EntityManager, vehicle, ref deliveryTruck))
				{
					resource |= deliveryTruck.m_Resource;
					if ((deliveryTruck.m_State & DeliveryTruckFlags.Loaded) != 0)
					{
						num += deliveryTruck.m_Amount;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					Entity prefab2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(vehicle).m_Prefab;
					if (EntitiesExtensions.TryGetComponent<DeliveryTruckData>(((ComponentSystemBase)this).EntityManager, prefab2, ref deliveryTruckData))
					{
						num2 += deliveryTruckData.m_CargoCapacity;
					}
				}
			}
		}
		else
		{
			resource = componentData.m_Resource;
			if ((componentData.m_State & DeliveryTruckFlags.Loaded) != 0)
			{
				num = componentData.m_Amount;
			}
			num2 = componentData2.m_CargoCapacity;
		}
		bool flag = (componentData.m_State & DeliveryTruckFlags.StorageTransfer) != 0;
		bool flag2 = (componentData.m_State & DeliveryTruckFlags.Buying) != 0;
		bool flag3 = (componentData.m_State & DeliveryTruckFlags.Returning) != 0;
		bool flag4 = (componentData.m_State & DeliveryTruckFlags.Delivering) != 0;
		info.label = "Delivery Truck";
		if ((componentData.m_State & DeliveryTruckFlags.DummyTraffic) != 0)
		{
			info.Add(new InfoList.Item("Dummy Traffic"));
		}
		info.Add(new InfoList.Item("Cargo: " + num + "/" + num2));
		Target target = default(Target);
		if (EntitiesExtensions.TryGetComponent<Target>(((ComponentSystemBase)this).EntityManager, entity, ref target))
		{
			if (flag2)
			{
				string text = (flag3 ? "Bought " : "Buying ");
				string text2 = (flag3 ? string.Empty : ("from " + m_NameSystem.GetDebugName(target.m_Target)));
				Entity entity2 = (flag3 ? InfoList.Item.kNullEntity : target.m_Target);
				info.Add(new InfoList.Item(string.Concat(text, resource, text2), entity2));
			}
			else if (flag)
			{
				string text3 = (flag3 ? "Exported " : "Exporting ");
				string text4 = (flag3 ? string.Empty : ("to " + m_NameSystem.GetDebugName(target.m_Target)));
				Entity entity3 = (flag3 ? InfoList.Item.kNullEntity : target.m_Target);
				info.Add(new InfoList.Item(string.Concat(text3, resource, text4), entity3));
			}
			else if (flag4)
			{
				string text5 = (flag3 ? "Delivered " : "Delivering ");
				string text6 = (flag3 ? string.Empty : ("to " + m_NameSystem.GetDebugName(target.m_Target)));
				Entity entity4 = (flag3 ? InfoList.Item.kNullEntity : target.m_Target);
				info.Add(new InfoList.Item(string.Concat(text5, resource, text6), entity4));
			}
			else
			{
				string text7 = (flag3 ? "Transported " : "Transporting ");
				string text8 = (flag3 ? string.Empty : ("to " + m_NameSystem.GetDebugName(target.m_Target)));
				Entity entity5 = (flag3 ? InfoList.Item.kNullEntity : target.m_Target);
				info.Add(new InfoList.Item(string.Concat(text7, resource, text8), entity5));
			}
		}
		if (flag3)
		{
			info.Add(new InfoList.Item("Returning"));
		}
	}

	private bool HasAmbulanceInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.Ambulance>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<AmbulanceData>(prefab);
		}
		return false;
	}

	private void UpdateAmbulanceInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.Ambulance componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.Ambulance>(entity);
		info.label = "Ambulance";
		Target target = default(Target);
		if (EntitiesExtensions.TryGetComponent<Target>(((ComponentSystemBase)this).EntityManager, entity, ref target))
		{
			if (componentData.m_TargetPatient != InfoList.Item.kNullEntity)
			{
				info.Add(new InfoList.Item("Patient" + m_NameSystem.GetDebugName(componentData.m_TargetPatient), componentData.m_TargetPatient));
			}
			if ((componentData.m_State & AmbulanceFlags.Returning) != 0)
			{
				info.Add(new InfoList.Item("Returning"));
			}
			else if ((componentData.m_State & AmbulanceFlags.Transporting) != 0)
			{
				info.Add(new InfoList.Item("Transporting to: " + m_NameSystem.GetDebugName(target.m_Target), target.m_Target));
			}
			else
			{
				info.Add(new InfoList.Item("Picking up from: " + m_NameSystem.GetDebugName(target.m_Target), target.m_Target));
			}
		}
	}

	private bool HasHearseInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.Hearse>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<HearseData>(prefab);
		}
		return false;
	}

	private void UpdateHearseInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.Hearse componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.Hearse>(entity);
		info.label = "Hearse";
		Target target = default(Target);
		if (EntitiesExtensions.TryGetComponent<Target>(((ComponentSystemBase)this).EntityManager, entity, ref target))
		{
			if (componentData.m_TargetCorpse != InfoList.Item.kNullEntity)
			{
				info.Add(new InfoList.Item("Body" + m_NameSystem.GetDebugName(componentData.m_TargetCorpse), componentData.m_TargetCorpse));
			}
			if ((componentData.m_State & HearseFlags.Returning) != 0)
			{
				info.Add(new InfoList.Item("Returning"));
			}
			else if ((componentData.m_State & HearseFlags.Transporting) != 0)
			{
				info.Add(new InfoList.Item("Transporting to" + m_NameSystem.GetDebugName(target.m_Target), target.m_Target));
			}
			else
			{
				info.Add(new InfoList.Item("Picking up from" + m_NameSystem.GetDebugName(target.m_Target), target.m_Target));
			}
		}
	}

	private bool HasGarbageTruckInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.GarbageTruck>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<GarbageTruckData>(prefab);
		}
		return false;
	}

	private void UpdateGarbageTruckInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.GarbageTruck componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.GarbageTruck>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		GarbageTruckData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<GarbageTruckData>(prefab);
		info.label = "Garbage Truck";
		info.Add(new InfoList.Item("Capacity: " + componentData.m_Garbage + "/" + componentData2.m_GarbageCapacity));
		if ((componentData.m_State & GarbageTruckFlags.Unloading) != 0)
		{
			info.Add(new InfoList.Item("Unloading"));
		}
		else if ((componentData.m_State & GarbageTruckFlags.Returning) != 0)
		{
			info.Add(new InfoList.Item("Returning"));
		}
	}

	private bool HasPublicTransportInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PublicTransport>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<PublicTransportVehicleData>(prefab);
		}
		return false;
	}

	private void UpdatePublicTransportInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.PublicTransport componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.PublicTransport>(entity);
		info.label = "Public Transport";
		if ((componentData.m_State & PublicTransportFlags.DummyTraffic) != 0)
		{
			info.Add(new InfoList.Item("Dummy Traffic"));
		}
		CurrentRoute currentRoute = default(CurrentRoute);
		if (EntitiesExtensions.TryGetComponent<CurrentRoute>(((ComponentSystemBase)this).EntityManager, entity, ref currentRoute))
		{
			info.Add(new InfoList.Item("Line: " + m_NameSystem.GetDebugName(currentRoute.m_Route), currentRoute.m_Route));
		}
		if ((componentData.m_State & PublicTransportFlags.Returning) != 0)
		{
			info.Add(new InfoList.Item("Returning"));
		}
		else if ((componentData.m_State & PublicTransportFlags.Boarding) != 0)
		{
			info.Add(new InfoList.Item("Boarding"));
			if (m_SimulationSystem.frameIndex < componentData.m_DepartureFrame)
			{
				int num = Mathf.CeilToInt((float)(componentData.m_DepartureFrame - m_SimulationSystem.frameIndex) / 60f);
				info.Add(new InfoList.Item("Departure: " + num + "s"));
			}
			else
			{
				Entity passengerWaiting = GetPassengerWaiting(entity);
				if (passengerWaiting != Entity.Null)
				{
					info.Add(new InfoList.Item("Waiting for: " + m_NameSystem.GetDebugName(passengerWaiting), passengerWaiting));
				}
			}
		}
		else if ((componentData.m_State & PublicTransportFlags.EnRoute) != 0)
		{
			info.Add(new InfoList.Item("En route"));
		}
		PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
		if (EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(((ComponentSystemBase)this).EntityManager, prefab, ref publicTransportVehicleData))
		{
			Odometer odometer = default(Odometer);
			if ((componentData.m_State & PublicTransportFlags.RequiresMaintenance) != 0)
			{
				info.Add(new InfoList.Item("Maintenance scheduled"));
			}
			else if (publicTransportVehicleData.m_MaintenanceRange > 0.1f && EntitiesExtensions.TryGetComponent<Odometer>(((ComponentSystemBase)this).EntityManager, entity, ref odometer))
			{
				int num2 = Mathf.RoundToInt(publicTransportVehicleData.m_MaintenanceRange * 0.001f);
				int num3 = math.max(0, Mathf.RoundToInt((publicTransportVehicleData.m_MaintenanceRange - odometer.m_Distance) * 0.001f));
				info.Add(new InfoList.Item("Remaining range: " + num3 + "/" + num2));
			}
		}
		if (GetRoutePosition(entity, out var nextWaypointIndex, out var segmentPosition))
		{
			info.Add(new InfoList.Item("Route waypoint index: " + nextWaypointIndex));
			info.Add(new InfoList.Item("Route segment position: " + Mathf.RoundToInt(segmentPosition * 100f) + "%"));
		}
	}

	private Entity GetPassengerWaiting(Entity vehicleEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, vehicleEntity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity passengerWaiting = GetPassengerWaiting2(val[i].m_Vehicle);
				if (passengerWaiting != Entity.Null)
				{
					return passengerWaiting;
				}
			}
			return Entity.Null;
		}
		return GetPassengerWaiting2(vehicleEntity);
	}

	private Entity GetPassengerWaiting2(Entity vehicleEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Passenger> val = default(DynamicBuffer<Passenger>);
		if (EntitiesExtensions.TryGetBuffer<Passenger>(((ComponentSystemBase)this).EntityManager, vehicleEntity, true, ref val))
		{
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			for (int i = 0; i < val.Length; i++)
			{
				Entity passenger = val[i].m_Passenger;
				if (EntitiesExtensions.TryGetComponent<CurrentVehicle>(((ComponentSystemBase)this).EntityManager, passenger, ref currentVehicle) && (currentVehicle.m_Flags & CreatureVehicleFlags.Ready) == 0)
				{
					return passenger;
				}
			}
		}
		return Entity.Null;
	}

	private bool GetRoutePosition(Entity transportVehicle, out int nextWaypointIndex, out float segmentPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		CurrentRoute currentRoute = default(CurrentRoute);
		if (EntitiesExtensions.TryGetComponent<CurrentRoute>(((ComponentSystemBase)this).EntityManager, transportVehicle, ref currentRoute))
		{
			PathInformation pathInformation = default(PathInformation);
			Waypoint waypoint = default(Waypoint);
			DynamicBuffer<RouteSegment> val = default(DynamicBuffer<RouteSegment>);
			if (EntitiesExtensions.TryGetComponent<PathInformation>(((ComponentSystemBase)this).EntityManager, transportVehicle, ref pathInformation) && EntitiesExtensions.TryGetComponent<Waypoint>(((ComponentSystemBase)this).EntityManager, pathInformation.m_Destination, ref waypoint) && EntitiesExtensions.TryGetBuffer<RouteSegment>(((ComponentSystemBase)this).EntityManager, currentRoute.m_Route, true, ref val))
			{
				nextWaypointIndex = waypoint.m_Index;
				int num = math.select(nextWaypointIndex - 1, val.Length - 1, nextWaypointIndex == 0);
				RouteSegment routeSegment = val[num];
				DynamicBuffer<PathElement> val2 = default(DynamicBuffer<PathElement>);
				if (EntitiesExtensions.TryGetBuffer<PathElement>(((ComponentSystemBase)this).EntityManager, routeSegment.m_Segment, true, ref val2) && val2.Length != 0)
				{
					int num2 = 0;
					PathOwner pathOwner = default(PathOwner);
					DynamicBuffer<PathElement> val3 = default(DynamicBuffer<PathElement>);
					if (EntitiesExtensions.TryGetComponent<PathOwner>(((ComponentSystemBase)this).EntityManager, transportVehicle, ref pathOwner) && EntitiesExtensions.TryGetBuffer<PathElement>(((ComponentSystemBase)this).EntityManager, transportVehicle, true, ref val3))
					{
						num2 += math.max(0, val3.Length - pathOwner.m_ElementIndex);
					}
					DynamicBuffer<CarNavigationLane> val4 = default(DynamicBuffer<CarNavigationLane>);
					DynamicBuffer<TrainNavigationLane> val5 = default(DynamicBuffer<TrainNavigationLane>);
					DynamicBuffer<WatercraftNavigationLane> val6 = default(DynamicBuffer<WatercraftNavigationLane>);
					DynamicBuffer<AircraftNavigationLane> val7 = default(DynamicBuffer<AircraftNavigationLane>);
					if (EntitiesExtensions.TryGetBuffer<CarNavigationLane>(((ComponentSystemBase)this).EntityManager, transportVehicle, true, ref val4))
					{
						num2 += val4.Length;
					}
					else if (EntitiesExtensions.TryGetBuffer<TrainNavigationLane>(((ComponentSystemBase)this).EntityManager, transportVehicle, true, ref val5))
					{
						num2 += val5.Length;
					}
					else if (EntitiesExtensions.TryGetBuffer<WatercraftNavigationLane>(((ComponentSystemBase)this).EntityManager, transportVehicle, true, ref val6))
					{
						num2 += val6.Length;
					}
					else if (EntitiesExtensions.TryGetBuffer<AircraftNavigationLane>(((ComponentSystemBase)this).EntityManager, transportVehicle, true, ref val7))
					{
						num2 += val7.Length;
					}
					segmentPosition = math.saturate((float)(val2.Length - num2) / (float)val2.Length);
					return true;
				}
			}
			Target target = default(Target);
			DynamicBuffer<RouteWaypoint> val8 = default(DynamicBuffer<RouteWaypoint>);
			if (EntitiesExtensions.TryGetComponent<Target>(((ComponentSystemBase)this).EntityManager, transportVehicle, ref target) && EntitiesExtensions.TryGetComponent<Waypoint>(((ComponentSystemBase)this).EntityManager, target.m_Target, ref waypoint) && EntitiesExtensions.TryGetBuffer<RouteWaypoint>(((ComponentSystemBase)this).EntityManager, currentRoute.m_Route, true, ref val8))
			{
				nextWaypointIndex = waypoint.m_Index;
				int num3 = math.select(nextWaypointIndex - 1, val8.Length - 1, nextWaypointIndex == 0);
				RouteWaypoint routeWaypoint = val8[num3];
				Transform transform = default(Transform);
				Position position = default(Position);
				Position position2 = default(Position);
				if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, transportVehicle, ref transform) && EntitiesExtensions.TryGetComponent<Position>(((ComponentSystemBase)this).EntityManager, routeWaypoint.m_Waypoint, ref position) && EntitiesExtensions.TryGetComponent<Position>(((ComponentSystemBase)this).EntityManager, target.m_Target, ref position2))
				{
					float num4 = math.distance(transform.m_Position, position2.m_Position);
					float num5 = math.max(1f, math.distance(position.m_Position, position2.m_Position));
					segmentPosition = math.saturate((num5 - num4) / num5);
					return true;
				}
			}
		}
		nextWaypointIndex = 0;
		segmentPosition = 0f;
		return false;
	}

	private bool HasCargoTransportInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.CargoTransport>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<CargoTransportVehicleData>(prefab);
		}
		return false;
	}

	private void UpdateCargoTransportInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.CargoTransport componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.CargoTransport>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		CargoTransportVehicleData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<CargoTransportVehicleData>(prefab);
		info.label = "Cargo Transport";
		if ((componentData.m_State & CargoTransportFlags.DummyTraffic) != 0)
		{
			info.Add(new InfoList.Item("Dummy Traffic"));
		}
		CurrentRoute currentRoute = default(CurrentRoute);
		if (EntitiesExtensions.TryGetComponent<CurrentRoute>(((ComponentSystemBase)this).EntityManager, entity, ref currentRoute))
		{
			info.Add(new InfoList.Item("Route: " + m_NameSystem.GetDebugName(currentRoute.m_Route), currentRoute.m_Route));
		}
		if ((componentData.m_State & CargoTransportFlags.Returning) != 0)
		{
			info.Add(new InfoList.Item("Returning"));
		}
		else if ((componentData.m_State & CargoTransportFlags.Boarding) != 0)
		{
			info.Add(new InfoList.Item("Loading"));
			if (m_SimulationSystem.frameIndex < componentData.m_DepartureFrame)
			{
				int num = Mathf.CeilToInt((float)(componentData.m_DepartureFrame - m_SimulationSystem.frameIndex) / 60f);
				info.Add(new InfoList.Item("Departure: " + num + "s"));
			}
		}
		else if ((componentData.m_State & CargoTransportFlags.EnRoute) != 0)
		{
			info.Add(new InfoList.Item("En route"));
		}
		CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
		if (EntitiesExtensions.TryGetComponent<CargoTransportVehicleData>(((ComponentSystemBase)this).EntityManager, prefab, ref cargoTransportVehicleData))
		{
			Odometer odometer = default(Odometer);
			if ((componentData.m_State & CargoTransportFlags.RequiresMaintenance) != 0)
			{
				info.Add(new InfoList.Item("Maintenance scheduled"));
			}
			else if (cargoTransportVehicleData.m_MaintenanceRange > 0.1f && EntitiesExtensions.TryGetComponent<Odometer>(((ComponentSystemBase)this).EntityManager, entity, ref odometer))
			{
				int num2 = Mathf.RoundToInt(cargoTransportVehicleData.m_MaintenanceRange * 0.001f);
				int num3 = math.max(0, Mathf.RoundToInt((cargoTransportVehicleData.m_MaintenanceRange - odometer.m_Distance) * 0.001f));
				info.Add(new InfoList.Item("Remaining range: " + num3 + "/" + num2));
			}
		}
		NativeList<Resources> target = default(NativeList<Resources>);
		target._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
		int num4 = 0;
		DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
		if (EntitiesExtensions.TryGetBuffer<LayoutElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			DynamicBuffer<Resources> source = default(DynamicBuffer<Resources>);
			PrefabRef prefabRef = default(PrefabRef);
			CargoTransportVehicleData cargoTransportVehicleData2 = default(CargoTransportVehicleData);
			for (int i = 0; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, vehicle, true, ref source))
				{
					AddResources(source, target);
				}
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, vehicle, ref prefabRef) && EntitiesExtensions.TryGetComponent<CargoTransportVehicleData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref cargoTransportVehicleData2))
				{
					num4 += cargoTransportVehicleData2.m_CargoCapacity;
				}
			}
		}
		else
		{
			DynamicBuffer<Resources> source2 = default(DynamicBuffer<Resources>);
			if (EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, entity, true, ref source2))
			{
				AddResources(source2, target);
			}
			num4 += componentData2.m_CargoCapacity;
		}
		info.Add(new InfoList.Item("Cargo: "));
		int num5 = 0;
		for (int j = 0; j < target.Length; j++)
		{
			Resources resources = target[j];
			info.Add(new InfoList.Item(string.Concat(resources.m_Resource, " ", resources.m_Amount)));
			num5 += resources.m_Amount;
		}
		info.Add(new InfoList.Item("Capacity " + num5 + "/" + num4));
		target.Dispose();
	}

	private void AddResources(DynamicBuffer<Resources> source, NativeList<Resources> target)
	{
		for (int i = 0; i < source.Length; i++)
		{
			Resources resources = source[i];
			if (resources.m_Amount == 0)
			{
				continue;
			}
			int num = 0;
			while (true)
			{
				if (num < target.Length)
				{
					Resources resources2 = target[num];
					if (resources2.m_Resource == resources.m_Resource)
					{
						resources2.m_Amount += resources.m_Amount;
						target[num] = resources2;
						break;
					}
					num++;
					continue;
				}
				target.Add(ref resources);
				break;
			}
		}
	}

	private bool HasMaintenanceVehicleInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.MaintenanceVehicle>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<MaintenanceVehicleData>(prefab);
		}
		return false;
	}

	private void UpdateMaintenanceVehicleInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.MaintenanceVehicle componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.MaintenanceVehicle>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		MaintenanceVehicleData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<MaintenanceVehicleData>(prefab);
		componentData2.m_MaintenanceCapacity = Mathf.CeilToInt((float)componentData2.m_MaintenanceCapacity * componentData.m_Efficiency);
		info.label = "Maintenance Vehicle";
		info.Add(new InfoList.Item("Work shift: " + $"{Mathf.CeilToInt(math.select((float)componentData.m_Maintained / (float)componentData2.m_MaintenanceCapacity, 0f, componentData2.m_MaintenanceCapacity == 0) * 100f)}%"));
		if ((componentData.m_State & MaintenanceVehicleFlags.ClearingDebris) != 0)
		{
			info.Add(new InfoList.Item("Clearing debris"));
		}
		else if ((componentData.m_State & MaintenanceVehicleFlags.Returning) != 0)
		{
			info.Add(new InfoList.Item("Returning"));
		}
	}

	private bool HasPostVanInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PostVan>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<PostVanData>(prefab);
		}
		return false;
	}

	private void UpdatePostVanInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.PostVan componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.PostVan>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PostVanData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PostVanData>(prefab);
		info.label = "Post Van";
		info.Add(new InfoList.Item("Mail to deliver: " + componentData.m_DeliveringMail + "/" + componentData2.m_MailCapacity));
		info.Add(new InfoList.Item("Collected mail: " + componentData.m_CollectedMail + "/" + componentData2.m_MailCapacity));
		if ((componentData.m_State & PostVanFlags.Returning) != 0)
		{
			info.Add(new InfoList.Item("Returning"));
		}
	}

	private bool HasFireEngineInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.FireEngine>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<FireEngineData>(prefab))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<ServiceDispatch>(entity);
			}
		}
		return false;
	}

	private void UpdateFireEngineInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.FireEngine componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.FireEngine>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		FireEngineData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<FireEngineData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<ServiceDispatch> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceDispatch>(entity, true);
		info.label = "Fire Engine";
		int num = Mathf.CeilToInt(componentData.m_ExtinguishingAmount);
		int num2 = Mathf.CeilToInt(componentData2.m_ExtinguishingCapacity);
		if (num2 > 0)
		{
			info.Add(new InfoList.Item("Load: " + num + "/" + num2));
		}
		if ((componentData.m_State & FireEngineFlags.Extinguishing) != 0)
		{
			info.Add(new InfoList.Item("Extinguishing"));
		}
		else if ((componentData.m_State & FireEngineFlags.Rescueing) != 0)
		{
			info.Add(new InfoList.Item("Searching for survivors"));
		}
		else if ((componentData.m_State & FireEngineFlags.Returning) != 0)
		{
			info.Add(new InfoList.Item("Returning"));
		}
		else
		{
			if (componentData.m_RequestCount <= 0 || buffer.Length <= 0)
			{
				return;
			}
			ServiceDispatch serviceDispatch = buffer[0];
			FireRescueRequest fireRescueRequest = default(FireRescueRequest);
			if (EntitiesExtensions.TryGetComponent<FireRescueRequest>(((ComponentSystemBase)this).EntityManager, serviceDispatch.m_Request, ref fireRescueRequest))
			{
				OnFire onFire = default(OnFire);
				Destroyed destroyed = default(Destroyed);
				if (EntitiesExtensions.TryGetComponent<OnFire>(((ComponentSystemBase)this).EntityManager, fireRescueRequest.m_Target, ref onFire) && onFire.m_Event != InfoList.Item.kNullEntity)
				{
					info.Add(new InfoList.Item("Dispatched" + m_NameSystem.GetDebugName(onFire.m_Event), onFire.m_Event));
				}
				else if (EntitiesExtensions.TryGetComponent<Destroyed>(((ComponentSystemBase)this).EntityManager, fireRescueRequest.m_Target, ref destroyed) && destroyed.m_Event != InfoList.Item.kNullEntity)
				{
					info.Add(new InfoList.Item("Dispatched" + m_NameSystem.GetDebugName(destroyed.m_Event), destroyed.m_Event));
				}
				else if (fireRescueRequest.m_Target != InfoList.Item.kNullEntity)
				{
					info.Add(new InfoList.Item("Dispatched" + m_NameSystem.GetDebugName(fireRescueRequest.m_Target), fireRescueRequest.m_Target));
				}
			}
		}
	}

	private bool HasPoliceCarInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return false;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Vehicles.PoliceCar>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<PoliceCarData>(prefab))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<ServiceDispatch>(entity);
			}
		}
		return false;
	}

	private void UpdatePoliceCarInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Vehicles.PoliceCar componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Vehicles.PoliceCar>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PoliceCarData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PoliceCarData>(prefab);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<ServiceDispatch> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceDispatch>(entity, true);
		info.label = "Police Car";
		if (componentData2.m_ShiftDuration != 0)
		{
			uint num = math.min(componentData.m_ShiftTime, componentData2.m_ShiftDuration);
			info.Add(new InfoList.Item("Work shift: " + num + "/" + componentData2.m_ShiftDuration));
		}
		DynamicBuffer<Passenger> val = default(DynamicBuffer<Passenger>);
		if (EntitiesExtensions.TryGetBuffer<Passenger>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			Game.Creatures.Resident resident = default(Game.Creatures.Resident);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val2 = val[i].m_Passenger;
				if (EntitiesExtensions.TryGetComponent<Game.Creatures.Resident>(((ComponentSystemBase)this).EntityManager, val2, ref resident))
				{
					val2 = resident.m_Citizen;
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(val2))
				{
					info.Add(new InfoList.Item("Arrested criminal" + m_NameSystem.GetDebugName(val2), val2));
				}
			}
		}
		if ((componentData.m_State & PoliceCarFlags.Returning) != 0)
		{
			info.Add(new InfoList.Item("Returning"));
		}
		else if ((componentData.m_State & PoliceCarFlags.AccidentTarget) != 0)
		{
			if ((componentData.m_State & PoliceCarFlags.AtTarget) != 0)
			{
				if (componentData.m_RequestCount <= 0 || buffer.Length <= 0)
				{
					return;
				}
				ServiceDispatch serviceDispatch = buffer[0];
				PoliceEmergencyRequest policeEmergencyRequest = default(PoliceEmergencyRequest);
				AccidentSite accidentSite = default(AccidentSite);
				if (EntitiesExtensions.TryGetComponent<PoliceEmergencyRequest>(((ComponentSystemBase)this).EntityManager, serviceDispatch.m_Request, ref policeEmergencyRequest) && EntitiesExtensions.TryGetComponent<AccidentSite>(((ComponentSystemBase)this).EntityManager, policeEmergencyRequest.m_Site, ref accidentSite))
				{
					if ((accidentSite.m_Flags & AccidentSiteFlags.TrafficAccident) != 0)
					{
						info.Add(new InfoList.Item("Securing accident site"));
					}
					else if ((accidentSite.m_Flags & AccidentSiteFlags.CrimeScene) != 0)
					{
						info.Add(new InfoList.Item("Securing crime scene"));
					}
				}
			}
			else
			{
				if (componentData.m_RequestCount <= 0 || buffer.Length <= 0)
				{
					return;
				}
				ServiceDispatch serviceDispatch2 = buffer[0];
				PoliceEmergencyRequest policeEmergencyRequest2 = default(PoliceEmergencyRequest);
				if (EntitiesExtensions.TryGetComponent<PoliceEmergencyRequest>(((ComponentSystemBase)this).EntityManager, serviceDispatch2.m_Request, ref policeEmergencyRequest2))
				{
					AccidentSite accidentSite2 = default(AccidentSite);
					if (EntitiesExtensions.TryGetComponent<AccidentSite>(((ComponentSystemBase)this).EntityManager, policeEmergencyRequest2.m_Site, ref accidentSite2) && accidentSite2.m_Event != InfoList.Item.kNullEntity)
					{
						info.Add(new InfoList.Item("Dispatched" + m_NameSystem.GetDebugName(accidentSite2.m_Event), accidentSite2.m_Event));
					}
					else
					{
						info.Add(new InfoList.Item("Dispatched" + m_NameSystem.GetDebugName(policeEmergencyRequest2.m_Site), policeEmergencyRequest2.m_Site));
					}
				}
			}
		}
		else
		{
			info.Add(new InfoList.Item("Patrolling"));
		}
	}

	private bool HasCitizenInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<HouseholdMember>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Citizen>(entity);
		}
		return false;
	}

	private unsafe void UpdateCitizenInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_099f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_093f: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Citizen componentData = ((EntityManager)(ref entityManager)).GetComponentData<Citizen>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Entity household = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(entity).m_Household;
		Household household2 = default(Household);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Household>(household))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			household2 = ((EntityManager)(ref entityManager)).GetComponentData<Household>(household);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(household, true);
		}
		EconomyParameterData economyParameters = ((EntityQuery)(ref __query_746694604_4)).GetSingleton<EconomyParameterData>();
		((EntityQuery)(ref __query_746694604_6)).GetSingleton<CitizenHappinessParameterData>();
		bool flag = (componentData.m_State & CitizenFlags.Tourist) != 0;
		bool flag2 = (componentData.m_State & CitizenFlags.Commuter) != 0;
		info.label = "Citizen";
		if (!flag2)
		{
			Entity val = InfoList.Item.kNullEntity;
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			if (EntitiesExtensions.TryGetComponent<CurrentBuilding>(((ComponentSystemBase)this).EntityManager, entity, ref currentBuilding))
			{
				val = currentBuilding.m_CurrentBuilding;
			}
			info.Add(new InfoList.Item("Current Building: " + val, val));
			info.Add(new InfoList.Item("Household: " + m_NameSystem.GetDebugName(household), household));
			info.Add(new InfoList.Item("Wellbeing: " + WellbeingToString(componentData.m_WellBeing) + "(" + componentData.m_WellBeing + ")"));
			info.Add(new InfoList.Item("Health: " + HealthToString(componentData.m_Health) + "(" + componentData.m_Health + ")"));
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Resources>(household))
			{
				Household householdData = household2;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				int householdTotalWealth = EconomyUtils.GetHouseholdTotalWealth(householdData, ((EntityManager)(ref entityManager)).GetBuffer<Resources>(household, true));
				long resource = 1L;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				int resources = EconomyUtils.GetResources((Resource)resource, ((EntityManager)(ref entityManager)).GetBuffer<Resources>(household, true));
				info.Add(new InfoList.Item("Household total Wealth Value: " + householdTotalWealth));
				info.Add(new InfoList.Item("Household Money: " + resources));
				PropertyRenter propertyRenter = default(PropertyRenter);
				if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, household, ref propertyRenter))
				{
					BufferLookup<Renter> m_RenterBufs = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
					ComponentLookup<ConsumptionData> consumptionDatas = InternalCompilerInterface.GetComponentLookup<ConsumptionData>(ref __TypeHandle.__Game_Prefabs_ConsumptionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
					ComponentLookup<PrefabRef> prefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
					Household householdData2 = household2;
					entityManager = ((ComponentSystemBase)this).EntityManager;
					info.Add(new InfoList.Item("Household spendable Money: " + EconomyUtils.GetHouseholdSpendableMoney(householdData2, ((EntityManager)(ref entityManager)).GetBuffer<Resources>(household, true), ref m_RenterBufs, ref consumptionDatas, ref prefabRefs, propertyRenter)));
				}
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		CarKeeper carKeeper = default(CarKeeper);
		if (((EntityManager)(ref entityManager)).IsComponentEnabled<CarKeeper>(entity) && EntitiesExtensions.TryGetComponent<CarKeeper>(((ComponentSystemBase)this).EntityManager, entity, ref carKeeper))
		{
			info.Add(new InfoList.Item("Car: " + m_NameSystem.GetDebugName(carKeeper.m_Car), carKeeper.m_Car));
		}
		if (!flag2)
		{
			info.Add(new InfoList.Item("Household total Resources: " + household2.m_Resources));
			PropertyRenter propertyRenter2 = default(PropertyRenter);
			if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, household, ref propertyRenter2))
			{
				info.Add(new InfoList.Item("Property: " + ((object)(*(Entity*)(&propertyRenter2.m_Property))/*cast due to .constrained prefix*/).ToString(), propertyRenter2.m_Property));
				info.Add(new InfoList.Item("Rent: " + propertyRenter2.m_Rent));
			}
			else if (!flag)
			{
				info.Add(new InfoList.Item("Homeless"));
			}
		}
		else
		{
			info.Add(new InfoList.Item("From outside the city"));
		}
		Criminal criminal = default(Criminal);
		bool flag3 = EntitiesExtensions.TryGetComponent<Criminal>(((ComponentSystemBase)this).EntityManager, entity, ref criminal);
		TravelPurpose purpose = default(TravelPurpose);
		if (EntitiesExtensions.TryGetComponent<TravelPurpose>(((ComponentSystemBase)this).EntityManager, entity, ref purpose))
		{
			Entity entity2 = InfoList.Item.kNullEntity;
			string purposeText = GetPurposeText(purpose, flag, criminal, ref entity2);
			info.Add(new InfoList.Item(purposeText + " " + m_NameSystem.GetDebugName(entity2), entity2));
		}
		string text = " female";
		if ((componentData.m_State & CitizenFlags.Male) != CitizenFlags.None)
		{
			text = " male";
		}
		info.Add(new InfoList.Item(GetAgeString(entity) + "(" + componentData.GetAgeInDays(m_SimulationSystem.frameIndex, TimeData.GetSingleton(m_TimeDataQuery)).ToString(CultureInfo.InvariantCulture) + ")" + text));
		info.Add(new InfoList.Item("Leisure: " + componentData.m_LeisureCounter));
		HealthProblem healthProblem = default(HealthProblem);
		if (EntitiesExtensions.TryGetComponent<HealthProblem>(((ComponentSystemBase)this).EntityManager, entity, ref healthProblem))
		{
			if ((healthProblem.m_Flags & HealthProblemFlags.Sick) != HealthProblemFlags.None)
			{
				info.Add(new InfoList.Item("Sick " + m_NameSystem.GetDebugName(healthProblem.m_Event), healthProblem.m_Event));
			}
			else if ((healthProblem.m_Flags & HealthProblemFlags.Injured) != HealthProblemFlags.None)
			{
				info.Add(new InfoList.Item("Injured " + m_NameSystem.GetDebugName(healthProblem.m_Event), healthProblem.m_Event));
			}
			else if ((healthProblem.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
			{
				info.Add(new InfoList.Item("Dead " + m_NameSystem.GetDebugName(healthProblem.m_Event), healthProblem.m_Event));
			}
			else if ((healthProblem.m_Flags & HealthProblemFlags.Trapped) != HealthProblemFlags.None)
			{
				info.Add(new InfoList.Item("Trapped " + m_NameSystem.GetDebugName(healthProblem.m_Event), healthProblem.m_Event));
			}
			else if ((healthProblem.m_Flags & HealthProblemFlags.InDanger) != HealthProblemFlags.None)
			{
				info.Add(new InfoList.Item("In danger " + m_NameSystem.GetDebugName(healthProblem.m_Event), healthProblem.m_Event));
			}
		}
		if (flag3)
		{
			string text2 = "Criminal";
			if ((criminal.m_Flags & CriminalFlags.Robber) != 0)
			{
				text2 += " Robber ";
			}
			if ((criminal.m_Flags & CriminalFlags.Prisoner) != 0)
			{
				text2 = text2 + "Jail Time: " + (uint)(criminal.m_JailTime * 16 * 16) / 262144u;
			}
			info.Add(new InfoList.Item(text2));
		}
		if (flag)
		{
			info.Add(new InfoList.Item("Tourist"));
			ComponentLookup<TouristHousehold> componentLookup = InternalCompilerInterface.GetComponentLookup<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			if (componentLookup.HasComponent(household) && componentLookup.HasComponent(household))
			{
				TouristHousehold touristHousehold = componentLookup[household];
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).Exists(touristHousehold.m_Hotel))
				{
					info.Add(new InfoList.Item("Staying at: " + m_NameSystem.GetDebugName(touristHousehold.m_Hotel), touristHousehold.m_Hotel));
				}
			}
		}
		if (!flag)
		{
			info.Add(new InfoList.Item(GetEducationString(componentData.GetEducationLevel())));
			Citizen citizen = default(Citizen);
			if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, entity, ref citizen))
			{
				CitizenAge age = citizen.GetAge();
				Worker worker = default(Worker);
				Game.Citizens.Student student = default(Game.Citizens.Student);
				if (EntitiesExtensions.TryGetComponent<Worker>(((ComponentSystemBase)this).EntityManager, entity, ref worker))
				{
					Entity workplace = worker.m_Workplace;
					info.Add(new InfoList.Item("Works at: " + m_NameSystem.GetDebugName(workplace), workplace));
					float2 timeToWork = WorkerSystem.GetTimeToWork(componentData, worker, ref economyParameters, includeCommute: false);
					info.Add(new InfoList.Item(string.Concat("Work Shift: ", GetTimeString(timeToWork.x) + " to " + GetTimeString(timeToWork.y))));
					timeToWork = WorkerSystem.GetTimeToWork(componentData, worker, ref economyParameters, includeCommute: true);
					info.Add(new InfoList.Item(string.Concat("Work Shift(Commute): ", GetTimeString(timeToWork.x) + " to " + GetTimeString(timeToWork.y))));
				}
				else if (EntitiesExtensions.TryGetComponent<Game.Citizens.Student>(((ComponentSystemBase)this).EntityManager, entity, ref student))
				{
					Entity school = student.m_School;
					info.Add(new InfoList.Item("Studies at: " + m_NameSystem.GetDebugName(school), school));
					float2 timeToStudy = StudentSystem.GetTimeToStudy(componentData, student, ref economyParameters);
					info.Add(new InfoList.Item(string.Concat("Study Time: ", GetTimeString(timeToStudy.x) + " to " + GetTimeString(timeToStudy.y))));
				}
				else
				{
					switch (age)
					{
					case CitizenAge.Adult:
						info.Add(new InfoList.Item("Unemployed"));
						break;
					case CitizenAge.Child:
					case CitizenAge.Teen:
						info.Add(new InfoList.Item("Not in school!"));
						break;
					}
				}
				ComponentLookup<Worker> workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				ComponentLookup<Game.Citizens.Student> students = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				float2 sleepTime = CitizenBehaviorSystem.GetSleepTime(entity, componentData, ref economyParameters, ref workers, ref students);
				info.Add(new InfoList.Item(string.Concat("Sleep Time: ", GetTimeString(sleepTime.x) + " to " + GetTimeString(sleepTime.y))));
			}
		}
		AttendingMeeting attendingMeeting = default(AttendingMeeting);
		CoordinatedMeeting coordinatedMeeting = default(CoordinatedMeeting);
		if (EntitiesExtensions.TryGetComponent<AttendingMeeting>(((ComponentSystemBase)this).EntityManager, entity, ref attendingMeeting) && EntitiesExtensions.TryGetComponent<CoordinatedMeeting>(((ComponentSystemBase)this).EntityManager, attendingMeeting.m_Meeting, ref coordinatedMeeting))
		{
			InfoList.Item item = ((coordinatedMeeting.m_Target != InfoList.Item.kNullEntity) ? new InfoList.Item("Meeting at: " + m_NameSystem.GetDebugName(coordinatedMeeting.m_Target), coordinatedMeeting.m_Target) : new InfoList.Item("Planning a meeting"));
			info.Add(item);
		}
		AttendingEvent attendingEvent = default(AttendingEvent);
		if (EntitiesExtensions.TryGetComponent<AttendingEvent>(((ComponentSystemBase)this).EntityManager, household, ref attendingEvent))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Events.CalendarEvent>(attendingEvent.m_Event))
			{
				info.Add(new InfoList.Item("Participating in " + m_NameSystem.GetDebugName(attendingEvent.m_Event)));
			}
		}
	}

	private string GetPurposeText(TravelPurpose purpose, bool tourist, Criminal criminal, ref Entity entity)
	{
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		string result;
		switch (purpose.m_Purpose)
		{
		case Purpose.GoingHome:
			result = (tourist ? "Going to hotel" : "Going home");
			break;
		case Purpose.GoingToWork:
			result = "Going to work";
			break;
		case Purpose.GoingToSchool:
			result = "Going to school";
			break;
		case Purpose.Studying:
			result = "Studying";
			break;
		case Purpose.Shopping:
			result = "Buying " + EconomyUtils.GetName(purpose.m_Resource);
			break;
		case Purpose.Working:
			result = "Working";
			break;
		case Purpose.Sleeping:
			result = "Sleeping";
			break;
		case Purpose.MovingAway:
			result = "Moving away";
			break;
		case Purpose.Leisure:
			result = (tourist ? "Sightseeing" : "Spending free time");
			break;
		case Purpose.Hospital:
			result = "Seeking medical care";
			break;
		case Purpose.Safety:
			result = "Getting safe";
			break;
		case Purpose.Escape:
			result = "Escaping";
			break;
		case Purpose.EmergencyShelter:
			result = "Evacuating";
			break;
		case Purpose.Crime:
			result = "Committing crime";
			entity = criminal.m_Event;
			break;
		case Purpose.GoingToJail:
			result = "Going to jail";
			entity = criminal.m_Event;
			break;
		case Purpose.GoingToPrison:
			result = "Going to prison";
			break;
		case Purpose.InJail:
			if ((criminal.m_Flags & CriminalFlags.Sentenced) != 0)
			{
				result = "Sentenced to prison";
				break;
			}
			result = "In jail";
			entity = criminal.m_Event;
			break;
		case Purpose.InPrison:
			result = "In prison";
			break;
		case Purpose.InHospital:
			result = "Getting medical care";
			break;
		case Purpose.Deathcare:
			result = "Transferring to death care";
			break;
		case Purpose.InDeathcare:
			result = "Waiting for processing";
			break;
		case Purpose.SendMail:
			result = "Sending mail";
			break;
		case Purpose.Disappear:
			result = "Disappearing";
			break;
		case Purpose.WaitingHome:
			result = "Waiting for new home";
			break;
		case Purpose.PathFailed:
			result = "Can't reach destination";
			break;
		default:
			result = "Idling";
			break;
		}
		return result;
	}

	private static string WellbeingToString(int wellbeing)
	{
		if (wellbeing < 25)
		{
			return "Depressed";
		}
		if (wellbeing < 40)
		{
			return "Sad";
		}
		if (wellbeing < 60)
		{
			return "Neutral";
		}
		if (wellbeing < 80)
		{
			return "Content";
		}
		return "Happy";
	}

	private static string HealthToString(int wellbeing)
	{
		if (wellbeing < 25)
		{
			return "Weak";
		}
		if (wellbeing < 40)
		{
			return "Poor";
		}
		if (wellbeing < 60)
		{
			return "OK";
		}
		if (wellbeing < 80)
		{
			return "Healthy";
		}
		return "Vigorous";
	}

	private static string ConsumptionToString(int dailyConsumption, int citizens, CitizenHappinessParameterData happinessParameters)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		int2 consumptionBonuses = CitizenHappinessSystem.GetConsumptionBonuses(dailyConsumption, citizens, in happinessParameters);
		int num = consumptionBonuses.x + consumptionBonuses.y;
		if (num < -15)
		{
			return "Wretched";
		}
		if (num < -5)
		{
			return "Poor";
		}
		if (num < 5)
		{
			return "Modest";
		}
		if (num < 15)
		{
			return "Comfortable";
		}
		return "Wealthy";
	}

	private static string GetLevelupTime(int condition, int levelup, int changePerDay)
	{
		if (changePerDay <= 0)
		{
			return "Decaying";
		}
		return "In " + Mathf.CeilToInt((float)(levelup - condition) / (float)changePerDay) + " days";
	}

	private string GetAgeString(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Citizen citizen = default(Citizen);
		if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, entity, ref citizen))
		{
			return citizen.GetAge() switch
			{
				CitizenAge.Child => "Child", 
				CitizenAge.Teen => "Teenager", 
				CitizenAge.Adult => "Adult", 
				_ => "Elderly", 
			};
		}
		return "Unknown";
	}

	private string GetEducationString(int education)
	{
		return education switch
		{
			0 => "Uneducated", 
			1 => "Poorly educated", 
			2 => "Educated", 
			3 => "Well educated", 
			4 => "Highly educated", 
			_ => "Unknown education", 
		};
	}

	private string GetTimeString(float time)
	{
		return Mathf.RoundToInt(time * 24f) + ":00";
	}

	private bool HasMailSenderInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return EntitiesExtensions.HasEnabledComponent<MailSender>(((ComponentSystemBase)this).EntityManager, entity);
	}

	private void UpdateMailSenderInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		MailSender componentData = ((EntityManager)(ref entityManager)).GetComponentData<MailSender>(entity);
		info.label = "Mail sender";
		info.value = componentData.m_Amount;
		info.max = 100;
	}

	private bool HasAnimalInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<HouseholdPet>(entity);
	}

	private void UpdateAnimalInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		HouseholdPet componentData = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdPet>(entity);
		info.label = "Household";
		info.value = m_NameSystem.GetDebugName(componentData.m_Household);
		info.target = componentData.m_Household;
	}

	private bool HasCreatureInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Creature>(entity);
	}

	private void UpdateCreatureInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		info.label = "Creature";
		Citizen citizen = default(Citizen);
		CurrentTransport currentTransport = default(CurrentTransport);
		if (!EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, entity, ref citizen) || !EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			return;
		}
		info.Add(new InfoList.Item("Entity: " + m_NameSystem.GetDebugName(currentTransport.m_CurrentTransport)));
		bool tourist = (citizen.m_State & CitizenFlags.Tourist) != 0;
		Criminal criminal = default(Criminal);
		EntitiesExtensions.TryGetComponent<Criminal>(((ComponentSystemBase)this).EntityManager, entity, ref criminal);
		Divert divert = default(Divert);
		if (EntitiesExtensions.TryGetComponent<Divert>(((ComponentSystemBase)this).EntityManager, currentTransport.m_CurrentTransport, ref divert) && divert.m_Purpose != Purpose.None)
		{
			Entity entity2 = InfoList.Item.kNullEntity;
			string purposeText = GetPurposeText(new TravelPurpose
			{
				m_Purpose = divert.m_Purpose,
				m_Resource = divert.m_Resource
			}, tourist, criminal, ref entity2);
			info.Add(new InfoList.Item(purposeText + " " + m_NameSystem.GetDebugName(entity2), entity2));
		}
		RideNeeder rideNeeder = default(RideNeeder);
		if (EntitiesExtensions.TryGetComponent<RideNeeder>(((ComponentSystemBase)this).EntityManager, currentTransport.m_CurrentTransport, ref rideNeeder))
		{
			Dispatched dispatched = default(Dispatched);
			DynamicBuffer<ServiceDispatch> val = default(DynamicBuffer<ServiceDispatch>);
			if (EntitiesExtensions.TryGetComponent<Dispatched>(((ComponentSystemBase)this).EntityManager, rideNeeder.m_RideRequest, ref dispatched) && EntitiesExtensions.TryGetBuffer<ServiceDispatch>(((ComponentSystemBase)this).EntityManager, dispatched.m_Handler, true, ref val) && val.Length > 0 && val[0].m_Request == rideNeeder.m_RideRequest)
			{
				info.Add(new InfoList.Item("Waiting for ride: " + m_NameSystem.GetDebugName(dispatched.m_Handler), dispatched.m_Handler));
			}
			else
			{
				info.Add(new InfoList.Item("Taking a taxi"));
			}
		}
		HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
		if (!EntitiesExtensions.TryGetComponent<HumanCurrentLane>(((ComponentSystemBase)this).EntityManager, currentTransport.m_CurrentTransport, ref humanCurrentLane))
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Creature componentData = ((EntityManager)(ref entityManager)).GetComponentData<Creature>(currentTransport.m_CurrentTransport);
		if ((humanCurrentLane.m_Flags & CreatureLaneFlags.EndReached) != 0)
		{
			if ((humanCurrentLane.m_Flags & CreatureLaneFlags.Transport) == 0)
			{
				return;
			}
			PathOwner pathOwner = default(PathOwner);
			DynamicBuffer<PathElement> val2 = default(DynamicBuffer<PathElement>);
			if (EntitiesExtensions.TryGetComponent<PathOwner>(((ComponentSystemBase)this).EntityManager, currentTransport.m_CurrentTransport, ref pathOwner) && EntitiesExtensions.TryGetBuffer<PathElement>(((ComponentSystemBase)this).EntityManager, currentTransport.m_CurrentTransport, true, ref val2) && val2.Length > pathOwner.m_ElementIndex)
			{
				Entity val3 = val2[pathOwner.m_ElementIndex].m_Target;
				Owner owner = default(Owner);
				if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val3, ref owner))
				{
					val3 = owner.m_Owner;
				}
				info.Add(new InfoList.Item("Waiting for transport: " + m_NameSystem.GetDebugName(val3), val3));
			}
			else
			{
				info.Add(new InfoList.Item("Waiting for transport"));
			}
		}
		else
		{
			if (!(componentData.m_QueueArea.radius > 0f))
			{
				return;
			}
			if (componentData.m_QueueEntity != Entity.Null)
			{
				Entity val4 = componentData.m_QueueEntity;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Owner owner2 = default(Owner);
				if (((EntityManager)(ref entityManager)).HasComponent<Waypoint>(val4) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val4, ref owner2))
				{
					val4 = owner2.m_Owner;
				}
				info.Add(new InfoList.Item("Queueing for: " + m_NameSystem.GetDebugName(val4), val4));
			}
			else
			{
				info.Add(new InfoList.Item("Queueing"));
			}
		}
	}

	private bool HasGroupLeaderInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<GroupCreature>(entity);
	}

	private bool HasGroupMemberInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<GroupMember>(entity);
	}

	private void UpdateGroupLeaderInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<GroupCreature> buffer = ((EntityManager)(ref entityManager)).GetBuffer<GroupCreature>(entity, true);
		info.label = $"Group members ({buffer.Length})";
		for (int i = 0; i < buffer.Length; i++)
		{
			info.Add(new InfoList.Item(m_NameSystem.GetDebugName(buffer[i].m_Creature), buffer[i].m_Creature));
		}
	}

	private void UpdateGroupMemberInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		GroupMember componentData = ((EntityManager)(ref entityManager)).GetComponentData<GroupMember>(entity);
		info.label = "Group leader";
		info.value = m_NameSystem.GetDebugName(componentData.m_Leader);
		info.target = componentData.m_Leader;
	}

	private bool HasVehicleModelInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<VehicleModel>(entity);
	}

	private void UpdateVehicleModelInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		VehicleModel componentData = ((EntityManager)(ref entityManager)).GetComponentData<VehicleModel>(entity);
		info.label = "Vehicle Model";
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<PrefabData>(componentData.m_PrimaryPrefab))
		{
			info.Add(new InfoList.Item(m_PrefabSystem.GetPrefabName(componentData.m_PrimaryPrefab)));
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<PrefabData>(componentData.m_SecondaryPrefab))
		{
			info.Add(new InfoList.Item(m_PrefabSystem.GetPrefabName(componentData.m_SecondaryPrefab)));
		}
	}

	private bool HasAreaInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Area>(entity);
	}

	private void UpdateAreaInfo(Entity entity, Entity prefab, InfoList info)
	{
		info.label = "Area Info";
		info.Add(new InfoList.Item("Nothing to see here, move along! (TBD)"));
	}

	private bool HasTreeInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Tree>(entity);
		}
		return false;
	}

	private void UpdateTreeInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Tree componentData = ((EntityManager)(ref entityManager)).GetComponentData<Tree>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Plant componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Plant>(entity);
		Damaged damaged = default(Damaged);
		EntitiesExtensions.TryGetComponent<Damaged>(((ComponentSystemBase)this).EntityManager, entity, ref damaged);
		int num = 0;
		TreeData treeData = default(TreeData);
		if (EntitiesExtensions.TryGetComponent<TreeData>(((ComponentSystemBase)this).EntityManager, prefab, ref treeData))
		{
			num = Mathf.RoundToInt(ObjectUtils.CalculateWoodAmount(componentData, componentData2, damaged, treeData));
		}
		info.label = $"Wood: {num}";
		info.value = num;
		info.max = Mathf.RoundToInt(treeData.m_WoodAmount);
	}

	private bool HasMailBoxInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Routes.MailBox>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<MailBoxData>(prefab);
			}
		}
		return false;
	}

	private void UpdateMailBoxInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Game.Routes.MailBox componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Routes.MailBox>(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		MailBoxData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<MailBoxData>(prefab);
		info.label = "Stored Mail in Mailbox";
		info.value = componentData.m_MailAmount;
		info.max = componentData2.m_MailCapacity;
	}

	private bool HasBoardingVehicleInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		BoardingVehicle boardingVehicle = default(BoardingVehicle);
		if (EntitiesExtensions.TryGetComponent<BoardingVehicle>(((ComponentSystemBase)this).EntityManager, entity, ref boardingVehicle))
		{
			return boardingVehicle.m_Vehicle != InfoList.Item.kNullEntity;
		}
		return false;
	}

	private void UpdateBoardingVehicleInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		BoardingVehicle componentData = ((EntityManager)(ref entityManager)).GetComponentData<BoardingVehicle>(entity);
		info.label = "Boarding";
		info.value = m_NameSystem.GetDebugName(componentData.m_Vehicle);
		info.target = componentData.m_Vehicle;
	}

	private bool HasWaitingPassengerInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<WaitingPassengers>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasBuffer<ConnectedRoute>(entity);
		}
		return true;
	}

	private void UpdateWaitingPassengerInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		WaitingPassengers waitingPassengers = default(WaitingPassengers);
		EntitiesExtensions.TryGetComponent<WaitingPassengers>(((ComponentSystemBase)this).EntityManager, entity, ref waitingPassengers);
		DynamicBuffer<ConnectedRoute> val = default(DynamicBuffer<ConnectedRoute>);
		if (EntitiesExtensions.TryGetBuffer<ConnectedRoute>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			int num = 0;
			WaitingPassengers waitingPassengers2 = default(WaitingPassengers);
			for (int i = 0; i < val.Length; i++)
			{
				if (EntitiesExtensions.TryGetComponent<WaitingPassengers>(((ComponentSystemBase)this).EntityManager, val[i].m_Waypoint, ref waitingPassengers2))
				{
					waitingPassengers.m_Count += waitingPassengers2.m_Count;
					num += waitingPassengers2.m_AverageWaitingTime;
				}
			}
			num /= math.max(1, val.Length);
			num -= num % 5;
			waitingPassengers.m_AverageWaitingTime = (ushort)num;
		}
		info.label = "Waiting passengers";
		info.Add(new InfoList.Item("Passenger count: " + waitingPassengers.m_Count));
		info.Add(new InfoList.Item(string.Concat("Waiting time: ", waitingPassengers.m_AverageWaitingTime + "s")));
	}

	private bool HasMovingInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Moving>(entity);
	}

	private void UpdateMovingInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		CurrentTransport currentTransport = default(CurrentTransport);
		EntityManager entityManager;
		if (EntitiesExtensions.TryGetComponent<CurrentTransport>(((ComponentSystemBase)this).EntityManager, entity, ref currentTransport))
		{
			entity = currentTransport.m_CurrentTransport;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity).m_Prefab;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		int num = Mathf.RoundToInt(math.length(((EntityManager)(ref entityManager)).GetComponentData<Moving>(entity).m_Velocity) * 3.6f);
		int max = Mathf.RoundToInt(999.99994f);
		CarData carData = default(CarData);
		WatercraftData watercraftData = default(WatercraftData);
		AirplaneData airplaneData = default(AirplaneData);
		HelicopterData helicopterData = default(HelicopterData);
		TrainData trainData = default(TrainData);
		HumanData humanData = default(HumanData);
		AnimalData animalData = default(AnimalData);
		if (EntitiesExtensions.TryGetComponent<CarData>(((ComponentSystemBase)this).EntityManager, prefab, ref carData))
		{
			max = Mathf.RoundToInt(carData.m_MaxSpeed * 3.6f);
		}
		else if (EntitiesExtensions.TryGetComponent<WatercraftData>(((ComponentSystemBase)this).EntityManager, prefab, ref watercraftData))
		{
			max = Mathf.RoundToInt(watercraftData.m_MaxSpeed * 3.6f);
		}
		else if (EntitiesExtensions.TryGetComponent<AirplaneData>(((ComponentSystemBase)this).EntityManager, prefab, ref airplaneData))
		{
			max = Mathf.RoundToInt(airplaneData.m_FlyingSpeed.y * 3.6f);
		}
		else if (EntitiesExtensions.TryGetComponent<HelicopterData>(((ComponentSystemBase)this).EntityManager, prefab, ref helicopterData))
		{
			max = Mathf.RoundToInt(helicopterData.m_FlyingMaxSpeed * 3.6f);
		}
		else if (EntitiesExtensions.TryGetComponent<TrainData>(((ComponentSystemBase)this).EntityManager, prefab, ref trainData))
		{
			max = Mathf.RoundToInt(trainData.m_MaxSpeed * 3.6f);
		}
		else if (EntitiesExtensions.TryGetComponent<HumanData>(((ComponentSystemBase)this).EntityManager, prefab, ref humanData))
		{
			max = Mathf.RoundToInt(humanData.m_RunSpeed * 3.6f);
		}
		else if (EntitiesExtensions.TryGetComponent<AnimalData>(((ComponentSystemBase)this).EntityManager, prefab, ref animalData))
		{
			max = Mathf.RoundToInt(animalData.m_MoveSpeed * 3.6f);
		}
		info.label = $"Moving: {num} km/h";
		info.value = num;
		info.max = max;
	}

	private bool HasDamagedInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Damaged>(entity);
	}

	private void UpdateDamagedInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Damaged componentData = ((EntityManager)(ref entityManager)).GetComponentData<Damaged>(entity);
		float4 val = default(float4);
		((float4)(ref val))._002Ector(componentData.m_Damage, ObjectUtils.GetTotalDamage(componentData));
		val = math.clamp(val * 100f, float4.op_Implicit(0f), float4.op_Implicit(100f));
		val = math.select(val, float4.op_Implicit(1f), (val > 0f) & (val < 1f));
		val = math.select(val, float4.op_Implicit(99f), (val > 99f) & (val < 100f));
		info.label = "Damaged";
		info.Add(new InfoList.Item("Physical: " + Mathf.RoundToInt(val.x) + "%"));
		info.Add(new InfoList.Item("Fire: " + Mathf.RoundToInt(val.y) + "%"));
		info.Add(new InfoList.Item("Water: " + Mathf.RoundToInt(val.z) + "%"));
		info.Add(new InfoList.Item("Total: " + Mathf.RoundToInt(val.w) + "%"));
	}

	private bool HasDestroyedInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Destroyed>(entity);
	}

	private void UpdateDestroyedInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Destroyed componentData = ((EntityManager)(ref entityManager)).GetComponentData<Destroyed>(entity);
		info.label = ((componentData.m_Event == InfoList.Item.kNullEntity) ? "Destroyed" : "Destroyed By");
		info.value = ((componentData.m_Event == InfoList.Item.kNullEntity) ? string.Empty : m_NameSystem.GetDebugName(componentData.m_Event));
		info.target = componentData.m_Event;
	}

	private bool HasDestroyedBuildingInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Destroyed>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Building>(entity);
		}
		return false;
	}

	private void UpdateDestroyedBuildingInfo(Entity entity, Entity prefab, CapacityInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Destroyed componentData = ((EntityManager)(ref entityManager)).GetComponentData<Destroyed>(entity);
		info.label = $"Searching for survivors: {Mathf.RoundToInt(componentData.m_Cleared * 100f)}%)";
		info.value = Mathf.RoundToInt(componentData.m_Cleared * 100f);
		info.max = 100;
	}

	private bool HasOnFireInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<OnFire>(entity);
	}

	private void UpdateOnFireInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		OnFire componentData = ((EntityManager)(ref entityManager)).GetComponentData<OnFire>(entity);
		info.label = "On fire";
		if (componentData.m_Event != InfoList.Item.kNullEntity)
		{
			info.Add(new InfoList.Item("Ignited by: " + m_NameSystem.GetDebugName(componentData.m_Event), componentData.m_Event));
		}
		info.Add(new InfoList.Item("Intensity: " + Mathf.RoundToInt(componentData.m_Intensity) + "%"));
	}

	private bool HasFacingWeatherInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<FacingWeather>(entity);
	}

	private void UpdateFacingWeatherInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		FacingWeather componentData = ((EntityManager)(ref entityManager)).GetComponentData<FacingWeather>(entity);
		info.label = ((componentData.m_Event == InfoList.Item.kNullEntity) ? "Suffering from weather" : "Weather phenomenon");
		info.value = ((componentData.m_Event == InfoList.Item.kNullEntity) ? string.Empty : m_NameSystem.GetDebugName(componentData.m_Event));
		info.target = componentData.m_Event;
	}

	private bool HasAccidentSiteInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<AccidentSite>(entity);
	}

	private void UpdateAccidentSiteInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		AccidentSite componentData = ((EntityManager)(ref entityManager)).GetComponentData<AccidentSite>(entity);
		info.label = ((componentData.m_Event == InfoList.Item.kNullEntity) ? "Accident site" : "Incident");
		info.value = ((componentData.m_Event == InfoList.Item.kNullEntity) ? string.Empty : m_NameSystem.GetDebugName(componentData.m_Event));
		info.target = componentData.m_Event;
	}

	private bool HasInvolvedInAccidentInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<InvolvedInAccident>(entity);
	}

	private void UpdateInvolvedInAccidentInfo(Entity entity, Entity prefab, GenericInfo info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		InvolvedInAccident componentData = ((EntityManager)(ref entityManager)).GetComponentData<InvolvedInAccident>(entity);
		info.label = ((componentData.m_Event == InfoList.Item.kNullEntity) ? "Involved in accident" : "Involved in");
		info.value = ((componentData.m_Event == InfoList.Item.kNullEntity) ? string.Empty : m_NameSystem.GetDebugName(componentData.m_Event));
		info.target = componentData.m_Event;
	}

	private bool HasFloodedInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Flooded>(entity);
	}

	private void UpdateFloodedInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Flooded componentData = ((EntityManager)(ref entityManager)).GetComponentData<Flooded>(entity);
		info.label = "Flooded";
		if (componentData.m_Event != InfoList.Item.kNullEntity)
		{
			info.Add(new InfoList.Item("Caused by: " + m_NameSystem.GetDebugName(componentData.m_Event), componentData.m_Event));
		}
		info.Add(new InfoList.Item("Depth: " + Mathf.RoundToInt(componentData.m_Depth) + "m"));
	}

	private bool HasEventInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Events.Event>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<TargetElement>(entity);
		}
		return false;
	}

	private void UpdateEventInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<TargetElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TargetElement>(entity, true);
		info.label = $"Affected Objects: {buffer.Length})";
		for (int i = 0; i < buffer.Length; i++)
		{
			info.Add(new InfoList.Item(m_NameSystem.GetDebugName(buffer[i].m_Entity), buffer[i].m_Entity));
		}
	}

	private bool HasNotificationInfo(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Icon>(entity);
	}

	private void UpdateNotificationInfo(Entity entity, Entity prefab, InfoList info)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		NotificationIconPrefab prefab2 = m_PrefabSystem.GetPrefab<NotificationIconPrefab>(prefab);
		info.label = "Notification Info";
		Owner owner = default(Owner);
		info.Add(EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, entity, ref owner) ? new InfoList.Item(prefab2.m_Description + m_NameSystem.GetDebugName(owner.m_Owner), owner.m_Owner) : new InfoList.Item(prefab2.m_Description));
		Target target = default(Target);
		if (EntitiesExtensions.TryGetComponent<Target>(((ComponentSystemBase)this).EntityManager, entity, ref target))
		{
			info.Add(new InfoList.Item(prefab2.m_TargetDescription + m_NameSystem.GetDebugName(target.m_Target), target.m_Target));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<ServiceFeeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_746694604_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<PollutionParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_746694604_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAllRW<CityModifier>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_746694604_2 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<GarbageParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_746694604_3 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<EconomyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_746694604_4 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<ExtractorParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_746694604_5 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAll<CitizenHappinessParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_746694604_6 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public DeveloperInfoUISystem()
	{
	}
}
