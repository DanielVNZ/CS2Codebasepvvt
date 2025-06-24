using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs.Effects;

[ComponentMenu("Effects/", new Type[] { typeof(EffectPrefab) })]
public class RandomTransform : ComponentBase
{
	public float3 m_MinAngle = new float3(0f, 0f, 0f);

	public float3 m_MaxAngle = new float3(0f, 0f, 360f);

	public float3 m_MinPosition = new float3(0f, 0f, 0f);

	public float3 m_MaxPosition = new float3(0f, 0f, 0f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<RandomTransformData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		RandomTransformData randomTransformData = default(RandomTransformData);
		randomTransformData.m_AngleRange.min = math.radians(m_MinAngle);
		randomTransformData.m_AngleRange.max = math.radians(m_MaxAngle);
		randomTransformData.m_PositionRange.min = m_MinPosition;
		randomTransformData.m_PositionRange.max = m_MaxPosition;
		((EntityManager)(ref entityManager)).SetComponentData<RandomTransformData>(entity, randomTransformData);
	}
}
