using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Services/", new Type[]
{
	typeof(ServicePrefab),
	typeof(ZonePrefab)
})]
public class CrimeAccumulation : ComponentBase
{
	public float m_CrimeRate = 7f;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<CrimeAccumulationData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		CrimeAccumulationData crimeAccumulationData = default(CrimeAccumulationData);
		crimeAccumulationData.m_CrimeRate = m_CrimeRate;
		((EntityManager)(ref entityManager)).SetComponentData<CrimeAccumulationData>(entity, crimeAccumulationData);
	}
}
