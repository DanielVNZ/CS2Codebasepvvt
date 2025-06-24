using System;
using System.Collections.Generic;
using Colossal.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class ServiceFeeParameterPrefab : PrefabBase
{
	public FeeParameters m_ElectricityFee;

	[Tooltip("Defines how the electricity consumption correlates to the electricity fee (0-200%)")]
	public AnimationCurve m_ElectricityFeeConsumptionMultiplier;

	public FeeParameters m_HealthcareFee;

	public FeeParameters m_BasicEducationFee;

	public FeeParameters m_SecondaryEducationFee;

	public FeeParameters m_HigherEducationFee;

	[Tooltip("This is used by budget UI, not in gameplay, use the other two garbage fee for gameplay garbage fee")]
	public FeeParameters m_GarbageFee;

	[Tooltip("Defines the garbage fee of RCIO zone tpye per building, x-residential,y-commercial,z-industrial,w-office ")]
	public int4 m_GarbageFeeRCIO;

	public FeeParameters m_WaterFee;

	[Tooltip("Defines how the water consumption correlates to the water fee (0-200%)")]
	public AnimationCurve m_WaterFeeConsumptionMultiplier;

	public FeeParameters m_FireResponseFee;

	public FeeParameters m_PoliceFee;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ServiceFeeParameterData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		ServiceFeeParameterData serviceFeeParameterData = new ServiceFeeParameterData
		{
			m_ElectricityFee = m_ElectricityFee,
			m_ElectricityFeeConsumptionMultiplier = new AnimationCurve1(m_ElectricityFeeConsumptionMultiplier),
			m_HealthcareFee = m_HealthcareFee,
			m_BasicEducationFee = m_BasicEducationFee,
			m_HigherEducationFee = m_HigherEducationFee,
			m_SecondaryEducationFee = m_SecondaryEducationFee,
			m_GarbageFee = m_GarbageFee,
			m_GarbageFeeRCIO = m_GarbageFeeRCIO,
			m_WaterFee = m_WaterFee,
			m_WaterFeeConsumptionMultiplier = new AnimationCurve1(m_WaterFeeConsumptionMultiplier),
			m_FireResponseFee = m_FireResponseFee,
			m_PoliceFee = m_PoliceFee
		};
		((EntityManager)(ref entityManager)).SetComponentData<ServiceFeeParameterData>(entity, serviceFeeParameterData);
	}
}
