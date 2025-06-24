using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(ObjectPrefab) })]
public class CompanyObject : ComponentBase
{
	public bool m_SelectCompany;

	public CompanyPrefab[] m_Companies;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Companies.Length; i++)
		{
			prefabs.Add(m_Companies[i]);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ObjectRequirementElement>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<ObjectRequirementElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ObjectRequirementElement>(entity, false);
		int length = buffer.Length;
		ObjectRequirementType type = (m_SelectCompany ? ObjectRequirementType.SelectOnly : ((ObjectRequirementType)0));
		for (int i = 0; i < m_Companies.Length; i++)
		{
			buffer.Add(new ObjectRequirementElement(existingSystemManaged.GetEntity(m_Companies[i]), length, type));
		}
	}
}
