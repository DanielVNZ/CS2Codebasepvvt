using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct TaxParameterData : IComponentData, IQueryTypeParameter, IEquatable<TaxParameterData>
{
	public int2 m_TotalTaxLimits;

	public int2 m_ResidentialTaxLimits;

	public int2 m_CommercialTaxLimits;

	public int2 m_IndustrialTaxLimits;

	public int2 m_OfficeTaxLimits;

	public int2 m_JobLevelTaxLimits;

	public int2 m_ResourceTaxLimits;

	public bool Equals(TaxParameterData other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (((int2)(ref m_CommercialTaxLimits)).Equals(other.m_CommercialTaxLimits) && ((int2)(ref m_IndustrialTaxLimits)).Equals(other.m_IndustrialTaxLimits) && ((int2)(ref m_JobLevelTaxLimits)).Equals(other.m_JobLevelTaxLimits) && ((int2)(ref m_OfficeTaxLimits)).Equals(other.m_OfficeTaxLimits) && ((int2)(ref m_ResidentialTaxLimits)).Equals(other.m_ResidentialTaxLimits) && ((int2)(ref m_ResourceTaxLimits)).Equals(other.m_ResourceTaxLimits))
		{
			return ((int2)(ref m_TotalTaxLimits)).Equals(other.m_TotalTaxLimits);
		}
		return false;
	}
}
