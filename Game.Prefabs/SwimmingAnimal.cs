using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Creatures/", new Type[] { typeof(AnimalPrefab) })]
public class SwimmingAnimal : ComponentBase
{
	public float m_SwimSpeed = 20f;

	public Bounds1 m_SwimDepth = new Bounds1(5f, 20f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		AnimalData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AnimalData>(entity);
		componentData.m_SwimSpeed = m_SwimSpeed / 3.6f;
		componentData.m_SwimDepth = m_SwimDepth;
		((EntityManager)(ref entityManager)).SetComponentData<AnimalData>(entity, componentData);
	}
}
