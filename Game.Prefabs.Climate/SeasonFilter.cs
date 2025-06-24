using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs.Climate;

[ComponentMenu("Themes/", new Type[] { typeof(WeatherPrefab) })]
public class SeasonFilter : ComponentBase
{
	public SeasonPrefab[] m_Seasons;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Seasons.Length; i++)
		{
			prefabs.Add(m_Seasons[i]);
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
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		DynamicBuffer<ObjectRequirementElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ObjectRequirementElement>(entity, false);
		int length = buffer.Length;
		for (int i = 0; i < m_Seasons.Length; i++)
		{
			SeasonPrefab seasonPrefab = m_Seasons[i];
			Entity entity2 = existingSystemManaged.GetEntity(seasonPrefab);
			buffer.Add(new ObjectRequirementElement(entity2, length));
		}
	}
}
