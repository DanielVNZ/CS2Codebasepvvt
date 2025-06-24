using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class TaxParameterPrefab : PrefabBase
{
	public int2 m_TotalTaxLimits;

	public int2 m_ResidentialTaxLimits;

	public int2 m_CommercialTaxLimits;

	public int2 m_IndustrialTaxLimits;

	public int2 m_OfficeTaxLimits;

	public int2 m_JobLevelTaxLimits;

	public int2 m_ResourceTaxLimits;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TaxParameterData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		((EntityManager)(ref entityManager)).SetComponentData<TaxParameterData>(entity, new TaxParameterData
		{
			m_TotalTaxLimits = m_TotalTaxLimits,
			m_ResidentialTaxLimits = m_ResidentialTaxLimits,
			m_CommercialTaxLimits = m_CommercialTaxLimits,
			m_IndustrialTaxLimits = m_IndustrialTaxLimits,
			m_OfficeTaxLimits = m_OfficeTaxLimits,
			m_JobLevelTaxLimits = m_JobLevelTaxLimits,
			m_ResourceTaxLimits = m_ResourceTaxLimits
		});
	}
}
