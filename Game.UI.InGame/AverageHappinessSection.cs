using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class AverageHappinessSection : InfoSectionBase
{
	public enum Result
	{
		Visible,
		ResidentCount,
		Happiness,
		ResultCount
	}

	[BurstCompile]
	private struct CountHappinessJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingFromEntity;

		[ReadOnly]
		public ComponentLookup<ResidentialProperty> m_ResidentialPropertyFromEntity;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdFromEntity;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenFromEntity;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemFromEntity;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenterFromEntity;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_AbandonedFromEntity;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenFromEntity;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterFromEntity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDataFromEntity;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDataFromEntity;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumerFromEntity;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumerFromEntity;

		[ReadOnly]
		public ComponentLookup<Locked> m_LockedFromEntity;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformFromEntity;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducersFromEntity;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> m_CrimeProducersFromEntity;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducerFromEntity;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDataFromEntity;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifierFromEntity;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverageFromEntity;

		public CitizenHappinessParameterData m_CitizenHappinessParameters;

		public GarbageParameterData m_GarbageParameters;

		public HealthcareParameterData m_HealthcareParameters;

		public ParkParameterData m_ParkParameters;

		public EducationParameterData m_EducationParameters;

		public TelecomParameterData m_TelecomParameters;

		[ReadOnly]
		public DynamicBuffer<HappinessFactorParameterData> m_HappinessFactorParameters;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverage;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoisePollutionMap;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		public NativeArray<int2> m_Factors;

		public NativeArray<int> m_Results;

		public Entity m_City;

		public float m_RelativeElectricityFee;

		public float m_RelativeWaterFee;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			int happiness = 0;
			int citizenCount = 0;
			if (m_BuildingFromEntity.HasComponent(m_SelectedEntity) && m_ResidentialPropertyFromEntity.HasComponent(m_SelectedEntity))
			{
				bool flag = m_AbandonedFromEntity.HasComponent(m_SelectedEntity);
				BuildingHappiness.GetResidentialBuildingHappinessFactors(m_City, m_TaxRates, m_SelectedEntity, m_Factors, ref m_PrefabRefFromEntity, ref m_SpawnableBuildingDataFromEntity, ref m_BuildingPropertyDataFromEntity, ref m_CityModifierFromEntity, ref m_BuildingFromEntity, ref m_ElectricityConsumerFromEntity, ref m_WaterConsumerFromEntity, ref m_ServiceCoverageFromEntity, ref m_LockedFromEntity, ref m_TransformFromEntity, ref m_GarbageProducersFromEntity, ref m_CrimeProducersFromEntity, ref m_MailProducerFromEntity, ref m_RenterFromEntity, ref m_CitizenFromEntity, ref m_HouseholdCitizenFromEntity, ref m_BuildingDataFromEntity, m_CitizenHappinessParameters, m_GarbageParameters, m_HealthcareParameters, m_ParkParameters, m_EducationParameters, m_TelecomParameters, m_HappinessFactorParameters, m_PollutionMap, m_NoisePollutionMap, m_AirPollutionMap, m_TelecomCoverage, m_RelativeElectricityFee, m_RelativeWaterFee);
				if (TryAddPropertyHappiness(ref happiness, ref citizenCount, m_SelectedEntity, m_HouseholdFromEntity, m_CitizenFromEntity, m_HealthProblemFromEntity, m_RenterFromEntity, m_HouseholdCitizenFromEntity))
				{
					m_Results[1] = citizenCount;
					m_Results[2] = happiness;
				}
				m_Results[0] = ((citizenCount > 0 || flag) ? 1 : 0);
			}
			else
			{
				DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
				if (!m_HouseholdCitizenFromEntity.TryGetBuffer(m_SelectedEntity, ref val))
				{
					return;
				}
				for (int i = 0; i < val.Length; i++)
				{
					Entity citizen = val[i].m_Citizen;
					if (m_CitizenFromEntity.HasComponent(citizen) && !CitizenUtils.IsDead(citizen, ref m_HealthProblemFromEntity))
					{
						happiness += m_CitizenFromEntity[citizen].Happiness;
						citizenCount++;
					}
				}
				m_Results[0] = 1;
				m_Results[1] = citizenCount;
				m_Results[2] = happiness;
				PropertyRenter propertyRenter = default(PropertyRenter);
				if (m_PropertyRenterFromEntity.TryGetComponent(m_SelectedEntity, ref propertyRenter))
				{
					BuildingHappiness.GetResidentialBuildingHappinessFactors(m_City, m_TaxRates, propertyRenter.m_Property, m_Factors, ref m_PrefabRefFromEntity, ref m_SpawnableBuildingDataFromEntity, ref m_BuildingPropertyDataFromEntity, ref m_CityModifierFromEntity, ref m_BuildingFromEntity, ref m_ElectricityConsumerFromEntity, ref m_WaterConsumerFromEntity, ref m_ServiceCoverageFromEntity, ref m_LockedFromEntity, ref m_TransformFromEntity, ref m_GarbageProducersFromEntity, ref m_CrimeProducersFromEntity, ref m_MailProducerFromEntity, ref m_RenterFromEntity, ref m_CitizenFromEntity, ref m_HouseholdCitizenFromEntity, ref m_BuildingDataFromEntity, m_CitizenHappinessParameters, m_GarbageParameters, m_HealthcareParameters, m_ParkParameters, m_EducationParameters, m_TelecomParameters, m_HappinessFactorParameters, m_PollutionMap, m_NoisePollutionMap, m_AirPollutionMap, m_TelecomCoverage, m_RelativeElectricityFee, m_RelativeWaterFee);
				}
			}
		}
	}

	[BurstCompile]
	public struct CountDistrictHappinessJob : IJobChunk
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictHandle;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_AbandonedFromEntity;

		[ReadOnly]
		public ComponentLookup<Household> m_HouseholdFromEntity;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenFromEntity;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemFromEntity;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizenFromEntity;

		[ReadOnly]
		public BufferLookup<Renter> m_RenterFromEntity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDataFromEntity;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingFromEntity;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> m_ElectricityConsumerFromEntity;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> m_WaterConsumerFromEntity;

		[ReadOnly]
		public ComponentLookup<Locked> m_LockedFromEntity;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformFromEntity;

		[ReadOnly]
		public ComponentLookup<GarbageProducer> m_GarbageProducersFromEntity;

		[ReadOnly]
		public ComponentLookup<CrimeProducer> m_CrimeProducersFromEntity;

		[ReadOnly]
		public ComponentLookup<MailProducer> m_MailProducerFromEntity;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDataFromEntity;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifierFromEntity;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> m_ServiceCoverageFromEntity;

		public CitizenHappinessParameterData m_CitizenHappinessParameters;

		public GarbageParameterData m_GarbageParameters;

		public HealthcareParameterData m_HealthcareParameters;

		public ParkParameterData m_ParkParameters;

		public EducationParameterData m_EducationParameters;

		public TelecomParameterData m_TelecomParameters;

		[ReadOnly]
		public DynamicBuffer<HappinessFactorParameterData> m_HappinessFactorParameters;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverage;

		[ReadOnly]
		public NativeArray<GroundPollution> m_PollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoisePollutionMap;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		public NativeArray<int2> m_Factors;

		public NativeArray<int> m_Results;

		public Entity m_City;

		public float m_RelativeElectricityFee;

		public float m_RelativeWaterFee;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			NativeArray<CurrentDistrict> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentDistrict>(ref m_CurrentDistrictHandle);
			int num = 0;
			int happiness = 0;
			int citizenCount = 0;
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				if (!(nativeArray2[i].m_District != m_SelectedEntity) && m_SpawnableBuildingDataFromEntity.HasComponent(m_PrefabRefFromEntity[val].m_Prefab) && (TryAddPropertyHappiness(ref happiness, ref citizenCount, val, m_HouseholdFromEntity, m_CitizenFromEntity, m_HealthProblemFromEntity, m_RenterFromEntity, m_HouseholdCitizenFromEntity) || m_AbandonedFromEntity.HasComponent(val)))
				{
					num = 1;
					BuildingHappiness.GetResidentialBuildingHappinessFactors(m_City, m_TaxRates, val, m_Factors, ref m_PrefabRefFromEntity, ref m_SpawnableBuildingDataFromEntity, ref m_BuildingPropertyDataFromEntity, ref m_CityModifierFromEntity, ref m_BuildingFromEntity, ref m_ElectricityConsumerFromEntity, ref m_WaterConsumerFromEntity, ref m_ServiceCoverageFromEntity, ref m_LockedFromEntity, ref m_TransformFromEntity, ref m_GarbageProducersFromEntity, ref m_CrimeProducersFromEntity, ref m_MailProducerFromEntity, ref m_RenterFromEntity, ref m_CitizenFromEntity, ref m_HouseholdCitizenFromEntity, ref m_BuildingDataFromEntity, m_CitizenHappinessParameters, m_GarbageParameters, m_HealthcareParameters, m_ParkParameters, m_EducationParameters, m_TelecomParameters, m_HappinessFactorParameters, m_PollutionMap, m_NoisePollutionMap, m_AirPollutionMap, m_TelecomCoverage, m_RelativeElectricityFee, m_RelativeWaterFee);
				}
			}
			ref NativeArray<int> reference = ref m_Results;
			reference[0] = reference[0] + num;
			reference = ref m_Results;
			reference[1] = reference[1] + citizenCount;
			reference = ref m_Results;
			reference[2] = reference[2] + happiness;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public BufferLookup<HappinessFactorParameterData> __Game_Prefabs_HappinessFactorParameterData_RW_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ElectricityConsumer> __Game_Buildings_ElectricityConsumer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterConsumer> __Game_Buildings_WaterConsumer_RO_ComponentLookup;

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
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResidentialProperty> __Game_Buildings_ResidentialProperty_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_HappinessFactorParameterData_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HappinessFactorParameterData>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_ElectricityConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ElectricityConsumer>(true);
			__Game_Buildings_WaterConsumer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterConsumer>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Buildings_GarbageProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageProducer>(true);
			__Game_Buildings_CrimeProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CrimeProducer>(true);
			__Game_Buildings_MailProducer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MailProducer>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Net_ServiceCoverage_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(true);
			__Game_Buildings_ResidentialProperty_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResidentialProperty>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
		}
	}

	private GroundPollutionSystem m_GroundPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private TaxSystem m_TaxSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_DistrictBuildingQuery;

	public NativeArray<int> m_Results;

	private NativeArray<int2> m_Factors;

	protected EntityQuery m_CitizenHappinessParameterQuery;

	protected EntityQuery m_GarbageParameterQuery;

	protected EntityQuery m_HappinessFactorParameterQuery;

	protected EntityQuery m_HealthcareParameterQuery;

	protected EntityQuery m_ParkParameterQuery;

	protected EntityQuery m_EducationParameterQuery;

	protected EntityQuery m_TelecomParameterQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1244325462_0;

	protected override string group => "AverageHappinessSection";

	private CitizenHappiness averageHappiness { get; set; }

	private NativeList<FactorInfo> happinessFactors { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_DistrictBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Renter>(),
			ComponentType.ReadOnly<CurrentDistrict>(),
			ComponentType.ReadOnly<ResidentialProperty>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CitizenHappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		m_GarbageParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		m_HappinessFactorParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HappinessFactorParameterData>() });
		m_HealthcareParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_ParkParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ParkParameterData>() });
		m_EducationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EducationParameterData>() });
		m_TelecomParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TelecomParameterData>() });
		m_Factors = new NativeArray<int2>(28, (Allocator)4, (NativeArrayOptions)1);
		happinessFactors = new NativeList<FactorInfo>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_Results = new NativeArray<int>(3, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		m_Results.Dispose();
		m_Factors.Dispose();
		happinessFactors.Dispose();
		base.OnDestroy();
	}

	protected override void Reset()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Factors.Length; i++)
		{
			m_Factors[i] = int2.op_Implicit(0);
		}
		averageHappiness = default(CitizenHappiness);
		happinessFactors.Clear();
		m_Results[0] = 0;
		m_Results[1] = 0;
		m_Results[2] = 0;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		CitizenHappinessParameterData singleton = ((EntityQuery)(ref m_CitizenHappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>();
		GarbageParameterData singleton2 = ((EntityQuery)(ref m_GarbageParameterQuery)).GetSingleton<GarbageParameterData>();
		DynamicBuffer<HappinessFactorParameterData> bufferAfterCompletingDependency = InternalCompilerInterface.GetBufferAfterCompletingDependency<HappinessFactorParameterData>(ref __TypeHandle.__Game_Prefabs_HappinessFactorParameterData_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef, ((EntityQuery)(ref m_HappinessFactorParameterQuery)).GetSingletonEntity());
		HealthcareParameterData singleton3 = ((EntityQuery)(ref m_HealthcareParameterQuery)).GetSingleton<HealthcareParameterData>();
		ParkParameterData singleton4 = ((EntityQuery)(ref m_ParkParameterQuery)).GetSingleton<ParkParameterData>();
		EducationParameterData singleton5 = ((EntityQuery)(ref m_EducationParameterQuery)).GetSingleton<EducationParameterData>();
		TelecomParameterData singleton6 = ((EntityQuery)(ref m_TelecomParameterQuery)).GetSingleton<TelecomParameterData>();
		ServiceFeeParameterData singleton7 = ((EntityQuery)(ref __query_1244325462_0)).GetSingleton<ServiceFeeParameterData>();
		JobHandle dependencies;
		NativeArray<GroundPollution> buffer = m_GroundPollutionSystem.GetData(readOnly: true, out dependencies).m_Buffer;
		JobHandle dependencies2;
		NativeArray<NoisePollution> buffer2 = m_NoisePollutionSystem.GetData(readOnly: true, out dependencies2).m_Buffer;
		JobHandle dependencies3;
		NativeArray<AirPollution> buffer3 = m_AirPollutionSystem.GetData(readOnly: true, out dependencies3).m_Buffer;
		JobHandle dependencies4;
		CellMapData<TelecomCoverage> data = m_TelecomCoverageSystem.GetData(readOnly: true, out dependencies4);
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		((JobHandle)(ref dependencies3)).Complete();
		((JobHandle)(ref dependencies4)).Complete();
		NativeArray<int> taxRates = m_TaxSystem.GetTaxRates();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<ServiceFee> buffer4 = ((EntityManager)(ref entityManager)).GetBuffer<ServiceFee>(m_CitySystem.City, false);
		float relativeElectricityFee = ServiceFeeSystem.GetFee(PlayerResource.Electricity, buffer4) / singleton7.m_ElectricityFee.m_Default;
		float relativeWaterFee = ServiceFeeSystem.GetFee(PlayerResource.Water, buffer4) / singleton7.m_WaterFee.m_Default;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		JobHandle val;
		if (((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Area>(selectedEntity))
			{
				val = JobChunkExtensions.Schedule<CountDistrictHappinessJob>(new CountDistrictHappinessJob
				{
					m_SelectedEntity = selectedEntity,
					m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CitizenFromEntity = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_HouseholdFromEntity = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CurrentDistrictHandle = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_AbandonedFromEntity = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_HealthProblemFromEntity = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_HouseholdCitizenFromEntity = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_RenterFromEntity = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_SpawnableBuildingDataFromEntity = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_BuildingPropertyDataFromEntity = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_BuildingFromEntity = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ElectricityConsumerFromEntity = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_WaterConsumerFromEntity = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_LockedFromEntity = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransformFromEntity = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_GarbageProducersFromEntity = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CrimeProducersFromEntity = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_MailProducerFromEntity = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_BuildingDataFromEntity = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CityModifierFromEntity = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ServiceCoverageFromEntity = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CitizenHappinessParameters = singleton,
					m_GarbageParameters = singleton2,
					m_HealthcareParameters = singleton3,
					m_ParkParameters = singleton4,
					m_EducationParameters = singleton5,
					m_TelecomParameters = singleton6,
					m_HappinessFactorParameters = bufferAfterCompletingDependency,
					m_TelecomCoverage = data,
					m_PollutionMap = buffer,
					m_NoisePollutionMap = buffer2,
					m_AirPollutionMap = buffer3,
					m_TaxRates = taxRates,
					m_Factors = m_Factors,
					m_Results = m_Results,
					m_City = m_CitySystem.City,
					m_RelativeElectricityFee = relativeElectricityFee,
					m_RelativeWaterFee = relativeWaterFee
				}, m_DistrictBuildingQuery, ((SystemBase)this).Dependency);
				((JobHandle)(ref val)).Complete();
				base.visible = m_Results[0] > 0;
				return;
			}
		}
		val = IJobExtensions.Schedule<CountHappinessJob>(new CountHappinessJob
		{
			m_SelectedEntity = selectedEntity,
			m_BuildingFromEntity = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentialPropertyFromEntity = InternalCompilerInterface.GetComponentLookup<ResidentialProperty>(ref __TypeHandle.__Game_Buildings_ResidentialProperty_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdFromEntity = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemFromEntity = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterFromEntity = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AbandonedFromEntity = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenFromEntity = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizenFromEntity = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RenterFromEntity = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingDataFromEntity = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyDataFromEntity = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElectricityConsumerFromEntity = InternalCompilerInterface.GetComponentLookup<ElectricityConsumer>(ref __TypeHandle.__Game_Buildings_ElectricityConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterConsumerFromEntity = InternalCompilerInterface.GetComponentLookup<WaterConsumer>(ref __TypeHandle.__Game_Buildings_WaterConsumer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LockedFromEntity = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFromEntity = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageProducersFromEntity = InternalCompilerInterface.GetComponentLookup<GarbageProducer>(ref __TypeHandle.__Game_Buildings_GarbageProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CrimeProducersFromEntity = InternalCompilerInterface.GetComponentLookup<CrimeProducer>(ref __TypeHandle.__Game_Buildings_CrimeProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MailProducerFromEntity = InternalCompilerInterface.GetComponentLookup<MailProducer>(ref __TypeHandle.__Game_Buildings_MailProducer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDataFromEntity = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifierFromEntity = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCoverageFromEntity = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenHappinessParameters = singleton,
			m_GarbageParameters = singleton2,
			m_HealthcareParameters = singleton3,
			m_ParkParameters = singleton4,
			m_EducationParameters = singleton5,
			m_TelecomParameters = singleton6,
			m_HappinessFactorParameters = bufferAfterCompletingDependency,
			m_TelecomCoverage = data,
			m_PollutionMap = buffer,
			m_NoisePollutionMap = buffer2,
			m_AirPollutionMap = buffer3,
			m_TaxRates = taxRates,
			m_Factors = m_Factors,
			m_Results = m_Results,
			m_City = m_CitySystem.City,
			m_RelativeElectricityFee = relativeElectricityFee,
			m_RelativeWaterFee = relativeWaterFee
		}, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		base.visible = m_Results[0] > 0;
	}

	protected override void OnProcess()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		int num = m_Results[1];
		int num2 = m_Results[2];
		averageHappiness = CitizenUIUtils.GetCitizenHappiness(num2 / math.select(num, 1, num == 0));
		for (int i = 0; i < m_Factors.Length; i++)
		{
			int x = m_Factors[i].x;
			if (x > 0)
			{
				float num3 = math.round((float)m_Factors[i].y / (float)x);
				if (num3 != 0f)
				{
					NativeList<FactorInfo> val = happinessFactors;
					FactorInfo factorInfo = new FactorInfo(i, (int)num3);
					val.Add(ref factorInfo);
				}
			}
		}
		NativeSortExtension.Sort<FactorInfo>(happinessFactors);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
		{
			base.tooltipKeys.Add("Building");
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Household>(selectedEntity))
		{
			base.tooltipKeys.Add("Household");
		}
		else
		{
			base.tooltipKeys.Add("District");
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("averageHappiness");
		JsonWriterExtensions.Write<CitizenHappiness>(writer, averageHappiness);
		int num = math.min(10, happinessFactors.Length);
		writer.PropertyName("happinessFactors");
		JsonWriterExtensions.ArrayBegin(writer, num);
		for (int i = 0; i < num; i++)
		{
			happinessFactors[i].WriteBuildingHappinessFactor(writer);
		}
		writer.ArrayEnd();
	}

	private static bool TryAddPropertyHappiness(ref int happiness, ref int citizenCount, Entity entity, ComponentLookup<Household> householdFromEntity, ComponentLookup<Citizen> citizenFromEntity, ComponentLookup<HealthProblem> healthProblemFromEntity, BufferLookup<Renter> renterFromEntity, BufferLookup<HouseholdCitizen> householdCitizenFromEntity)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (renterFromEntity.TryGetBuffer(entity, ref val))
		{
			DynamicBuffer<HouseholdCitizen> val2 = default(DynamicBuffer<HouseholdCitizen>);
			for (int i = 0; i < val.Length; i++)
			{
				Entity renter = val[i].m_Renter;
				if (!householdFromEntity.HasComponent(renter) || !householdCitizenFromEntity.TryGetBuffer(renter, ref val2))
				{
					continue;
				}
				result = true;
				for (int j = 0; j < val2.Length; j++)
				{
					Entity citizen = val2[j].m_Citizen;
					if (citizenFromEntity.HasComponent(citizen) && !CitizenUtils.IsDead(citizen, ref healthProblemFromEntity))
					{
						happiness += citizenFromEntity[citizen].Happiness;
						citizenCount++;
					}
				}
			}
		}
		return result;
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
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<ServiceFeeParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1244325462_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public AverageHappinessSection()
	{
	}
}
