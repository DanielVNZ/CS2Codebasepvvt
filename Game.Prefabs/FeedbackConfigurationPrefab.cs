using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.City;
using Game.Common;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class FeedbackConfigurationPrefab : PrefabBase
{
	public NotificationIconPrefab m_HappyFaceNotification;

	public NotificationIconPrefab m_SadFaceNotification;

	public float m_GarbageProducerGarbageFactor = 0.001f;

	public float m_GarbageVehicleFactor = 0.1f;

	public float m_HospitalAmbulanceFactor = 0.01f;

	public float m_HospitalHelicopterFactor = 0.1f;

	public float m_HospitalCapacityFactor = 0.001f;

	public float m_DeathcareHearseFactor = 0.1f;

	public float m_DeathcareCapacityFactor = 0.0005f;

	public float m_DeathcareProcessingFactor = 0.05f;

	public float m_ElectricityConsumptionFactor = 0.01f;

	public float m_ElectricityProductionFactor = 0.0001f;

	public float m_TransformerRadius = 500f;

	public float m_WaterConsumptionFactor = 0.01f;

	public float m_WaterCapacityFactor = 0.0001f;

	public float m_WaterConsumerSewageFactor = 0.002f;

	public float m_SewageCapacityFactor = 0.0001f;

	public float m_TransportVehicleCapacityFactor = 0.01f;

	public float m_TransportDispatchCenterFactor = 0.005f;

	public float m_TransportStationRange = 1000f;

	public float m_TransportStopRange = 250f;

	public float m_MailProducerMailFactor = 0.01f;

	public float m_PostFacilityVanFactor = 0.01f;

	public float m_PostFacilityTruckFactor = 0.1f;

	public float m_PostFacilityCapacityFactor = 0.0001f;

	public float m_PostFacilityProcessingFactor = 0.001f;

	public float m_TelecomCapacityFactor = 0.0001f;

	public float m_ElementarySchoolCapacityFactor = 0.002f;

	public float m_HighSchoolCapacityFactor = 0.001f;

	public float m_CollegeCapacityFactor = 0.0005f;

	public float m_UniversityCapacityFactor = 0.0002f;

	public float m_ParkingFacilityRange = 400f;

	public float m_MaintenanceVehicleFactor = 0.02f;

	public float m_FireStationEngineFactor = 0.02f;

	public float m_FireStationHelicopterFactor = 0.02f;

	public float m_CrimeProducerCrimeFactor = 0.0002f;

	public float m_PoliceStationCarFactor = 0.01f;

	public float m_PoliceStationHelicopterFactor = 0.1f;

	public float m_PoliceStationCapacityFactor = 0.005f;

	public float m_PrisonVehicleFactor = 0.02f;

	public float m_PrisonCapacityFactor = 0.0001f;

	public float m_GroundPollutionFactor = 0.01f;

	public float m_AirPollutionFactor = 0.01f;

	public float m_NoisePollutionFactor = 0.01f;

	public float m_GroundPollutionRadius = 150f;

	public float m_AirPollutionRadius = 1000f;

	public float m_NoisePollutionRadius = 200f;

	public float m_AttractivenessFactor = 0.005f;

	[EnumValue(typeof(LocalModifierType))]
	public float[] m_LocalModifierFactors;

	[EnumValue(typeof(CityModifierType))]
	public float[] m_CityModifierFactors;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_HappyFaceNotification);
		prefabs.Add(m_SadFaceNotification);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<FeedbackConfigurationData>());
		components.Add(ComponentType.ReadWrite<FeedbackLocalEffectFactor>());
		components.Add(ComponentType.ReadWrite<FeedbackCityEffectFactor>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<FeedbackConfigurationData>(entity, new FeedbackConfigurationData
		{
			m_HappyFaceNotification = orCreateSystemManaged.GetEntity(m_HappyFaceNotification),
			m_SadFaceNotification = orCreateSystemManaged.GetEntity(m_SadFaceNotification),
			m_GarbageProducerGarbageFactor = m_GarbageProducerGarbageFactor,
			m_GarbageVehicleFactor = m_GarbageVehicleFactor,
			m_HospitalAmbulanceFactor = m_HospitalAmbulanceFactor,
			m_HospitalHelicopterFactor = m_HospitalHelicopterFactor,
			m_HospitalCapacityFactor = m_HospitalCapacityFactor,
			m_DeathcareHearseFactor = m_DeathcareHearseFactor,
			m_DeathcareCapacityFactor = m_DeathcareCapacityFactor,
			m_DeathcareProcessingFactor = m_DeathcareProcessingFactor,
			m_ElectricityConsumptionFactor = m_ElectricityConsumptionFactor,
			m_ElectricityProductionFactor = m_ElectricityProductionFactor,
			m_TransformerRadius = m_TransformerRadius,
			m_WaterConsumptionFactor = m_WaterConsumptionFactor,
			m_WaterCapacityFactor = m_WaterCapacityFactor,
			m_WaterConsumerSewageFactor = m_WaterConsumerSewageFactor,
			m_SewageCapacityFactor = m_SewageCapacityFactor,
			m_TransportVehicleCapacityFactor = m_TransportVehicleCapacityFactor,
			m_TransportDispatchCenterFactor = m_TransportDispatchCenterFactor,
			m_TransportStationRange = m_TransportStationRange,
			m_TransportStopRange = m_TransportStopRange,
			m_MailProducerMailFactor = m_MailProducerMailFactor,
			m_PostFacilityVanFactor = m_PostFacilityVanFactor,
			m_PostFacilityTruckFactor = m_PostFacilityTruckFactor,
			m_PostFacilityCapacityFactor = m_PostFacilityCapacityFactor,
			m_PostFacilityProcessingFactor = m_PostFacilityProcessingFactor,
			m_TelecomCapacityFactor = m_TelecomCapacityFactor,
			m_ElementarySchoolCapacityFactor = m_ElementarySchoolCapacityFactor,
			m_HighSchoolCapacityFactor = m_HighSchoolCapacityFactor,
			m_CollegeCapacityFactor = m_CollegeCapacityFactor,
			m_UniversityCapacityFactor = m_UniversityCapacityFactor,
			m_ParkingFacilityRange = m_ParkingFacilityRange,
			m_MaintenanceVehicleFactor = m_MaintenanceVehicleFactor,
			m_FireStationEngineFactor = m_FireStationEngineFactor,
			m_FireStationHelicopterFactor = m_FireStationHelicopterFactor,
			m_CrimeProducerCrimeFactor = m_CrimeProducerCrimeFactor,
			m_PoliceStationCarFactor = m_PoliceStationCarFactor,
			m_PoliceStationHelicopterFactor = m_PoliceStationHelicopterFactor,
			m_PoliceStationCapacityFactor = m_PoliceStationCapacityFactor,
			m_PrisonVehicleFactor = m_PrisonVehicleFactor,
			m_PrisonCapacityFactor = m_PrisonCapacityFactor,
			m_GroundPollutionFactor = m_GroundPollutionFactor,
			m_AirPollutionFactor = m_AirPollutionFactor,
			m_NoisePollutionFactor = m_NoisePollutionFactor,
			m_GroundPollutionRadius = m_GroundPollutionRadius,
			m_AirPollutionRadius = m_AirPollutionRadius,
			m_NoisePollutionRadius = m_NoisePollutionRadius,
			m_AttractivenessFactor = m_AttractivenessFactor
		});
		if (m_LocalModifierFactors != null)
		{
			DynamicBuffer<FeedbackLocalEffectFactor> buffer = ((EntityManager)(ref entityManager)).GetBuffer<FeedbackLocalEffectFactor>(entity, false);
			buffer.ResizeUninitialized(m_LocalModifierFactors.Length);
			for (int i = 0; i < m_LocalModifierFactors.Length; i++)
			{
				buffer[i] = new FeedbackLocalEffectFactor
				{
					m_Factor = m_LocalModifierFactors[i]
				};
			}
		}
		if (m_CityModifierFactors != null)
		{
			DynamicBuffer<FeedbackCityEffectFactor> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<FeedbackCityEffectFactor>(entity, false);
			buffer2.ResizeUninitialized(m_CityModifierFactors.Length);
			for (int j = 0; j < m_CityModifierFactors.Length; j++)
			{
				buffer2[j] = new FeedbackCityEffectFactor
				{
					m_Factor = m_CityModifierFactors[j]
				};
			}
		}
	}
}
