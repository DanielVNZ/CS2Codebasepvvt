using System.Collections.Generic;
using Colossal.Collections;
using Game.City;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct ServiceFeeParameterData : IComponentData, IQueryTypeParameter
{
	public FeeParameters m_ElectricityFee;

	public AnimationCurve1 m_ElectricityFeeConsumptionMultiplier;

	public FeeParameters m_HealthcareFee;

	public FeeParameters m_BasicEducationFee;

	public FeeParameters m_SecondaryEducationFee;

	public FeeParameters m_HigherEducationFee;

	public FeeParameters m_GarbageFee;

	public int4 m_GarbageFeeRCIO;

	public FeeParameters m_WaterFee;

	public AnimationCurve1 m_WaterFeeConsumptionMultiplier;

	public FeeParameters m_FireResponseFee;

	public FeeParameters m_PoliceFee;

	public FeeParameters GetFeeParameters(PlayerResource resource)
	{
		return resource switch
		{
			PlayerResource.Healthcare => m_HealthcareFee, 
			PlayerResource.Electricity => m_ElectricityFee, 
			PlayerResource.BasicEducation => m_BasicEducationFee, 
			PlayerResource.HigherEducation => m_HigherEducationFee, 
			PlayerResource.SecondaryEducation => m_SecondaryEducationFee, 
			PlayerResource.Garbage => m_GarbageFee, 
			PlayerResource.Water => m_WaterFee, 
			PlayerResource.FireResponse => m_FireResponseFee, 
			PlayerResource.Police => m_PoliceFee, 
			_ => default(FeeParameters), 
		};
	}

	public IEnumerable<ServiceFee> GetDefaultFees()
	{
		yield return GetDefaultServiceFee(PlayerResource.Healthcare);
		yield return GetDefaultServiceFee(PlayerResource.Electricity);
		yield return GetDefaultServiceFee(PlayerResource.BasicEducation);
		yield return GetDefaultServiceFee(PlayerResource.HigherEducation);
		yield return GetDefaultServiceFee(PlayerResource.SecondaryEducation);
		yield return GetDefaultServiceFee(PlayerResource.Garbage);
		yield return GetDefaultServiceFee(PlayerResource.Water);
		yield return GetDefaultServiceFee(PlayerResource.FireResponse);
		yield return GetDefaultServiceFee(PlayerResource.Police);
	}

	private ServiceFee GetDefaultServiceFee(PlayerResource resource)
	{
		return new ServiceFee
		{
			m_Resource = resource,
			m_Fee = GetFeeParameters(resource).m_Default
		};
	}
}
