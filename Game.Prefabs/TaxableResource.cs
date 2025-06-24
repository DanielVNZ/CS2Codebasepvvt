using System;
using System.Collections.Generic;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Resources/", new Type[] { typeof(ResourcePrefab) })]
public class TaxableResource : ComponentBase
{
	public TaxAreaType[] m_TaxAreas;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TaxableResourceData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		if (m_TaxAreas != null)
		{
			((EntityManager)(ref entityManager)).SetComponentData<TaxableResourceData>(entity, new TaxableResourceData(m_TaxAreas));
		}
	}
}
