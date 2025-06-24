using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(ObjectPrefab) })]
public class DestructibleObject : ComponentBase
{
	public float m_FireHazard = 100f;

	public float m_StructuralIntegrity = 15000f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<DestructibleObjectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		DestructibleObjectData destructibleObjectData = default(DestructibleObjectData);
		destructibleObjectData.m_FireHazard = m_FireHazard;
		destructibleObjectData.m_StructuralIntegrity = m_StructuralIntegrity;
		((EntityManager)(ref entityManager)).SetComponentData<DestructibleObjectData>(entity, destructibleObjectData);
	}
}
