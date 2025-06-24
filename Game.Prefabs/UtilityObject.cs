using System;
using System.Collections.Generic;
using Game.Net;
using Game.Objects;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[]
{
	typeof(StaticObjectPrefab),
	typeof(MarkerObjectPrefab)
})]
public class UtilityObject : ComponentBase
{
	public UtilityTypes m_UtilityType = UtilityTypes.WaterPipe;

	public float3 m_UtilityPosition;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<UtilityObjectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Objects.UtilityObject>());
		components.Add(ComponentType.ReadWrite<Color>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		UtilityObjectData utilityObjectData = default(UtilityObjectData);
		utilityObjectData.m_UtilityTypes = m_UtilityType;
		utilityObjectData.m_UtilityPosition = m_UtilityPosition;
		((EntityManager)(ref entityManager)).SetComponentData<UtilityObjectData>(entity, utilityObjectData);
	}
}
