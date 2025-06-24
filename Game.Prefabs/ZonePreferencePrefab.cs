using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Zones/", new Type[] { })]
public class ZonePreferencePrefab : PrefabBase
{
	public float m_ResidentialSignificanceServices = 100f;

	public float m_ResidentialSignificanceWorkplaces = 50f;

	public float m_ResidentialSignificanceLandValue = -1f;

	public float m_ResidentialSignificancePollution = -100f;

	public float m_ResidentialNeutralLandValue = 10f;

	public float m_CommercialSignificanceConsumers = 100f;

	public float m_CommercialSignificanceCompetitors = 200f;

	public float m_CommercialSignificanceWorkplaces = 70f;

	public float m_CommercialSignificanceLandValue = -0.5f;

	public float m_CommercialNeutralLandValue = 20f;

	public float m_IndustrialSignificanceInput = 100f;

	public float m_IndustrialSignificanceOutside = 100f;

	public float m_IndustrialSignificanceLandValue = -1f;

	public float m_IndustrialNeutralLandValue = 5f;

	public float m_OfficeSignificanceEmployees = 100f;

	public float m_OfficeSignificanceServices = 100f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ZonePreferenceData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<ZonePreferenceData>(entity, new ZonePreferenceData
		{
			m_ResidentialSignificanceServices = m_ResidentialSignificanceServices,
			m_ResidentialSignificanceWorkplaces = m_ResidentialSignificanceWorkplaces,
			m_ResidentialSignificanceLandValue = m_ResidentialSignificanceLandValue,
			m_ResidentialSignificancePollution = m_ResidentialSignificancePollution,
			m_ResidentialNeutralLandValue = m_ResidentialNeutralLandValue,
			m_CommercialSignificanceCompetitors = m_CommercialSignificanceCompetitors,
			m_CommercialSignificanceConsumers = m_CommercialSignificanceConsumers,
			m_CommercialSignificanceWorkplaces = m_CommercialSignificanceWorkplaces,
			m_CommercialSignificanceLandValue = m_CommercialSignificanceLandValue,
			m_CommercialNeutralLandValue = m_CommercialNeutralLandValue,
			m_IndustrialSignificanceInput = m_IndustrialSignificanceInput,
			m_IndustrialSignificanceLandValue = m_IndustrialSignificanceLandValue,
			m_IndustrialSignificanceOutside = m_IndustrialSignificanceOutside,
			m_IndustrialNeutralLandValue = m_IndustrialNeutralLandValue,
			m_OfficeSignificanceEmployees = m_OfficeSignificanceEmployees,
			m_OfficeSignificanceServices = m_OfficeSignificanceServices
		});
	}
}
