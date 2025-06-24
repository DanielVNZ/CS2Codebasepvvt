using System;
using System.Collections.Generic;
using Game.Common;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(StaticObjectPrefab) })]
public class MoveableBridge : ComponentBase
{
	public float3 m_LiftOffsets;

	public float m_MovingTime;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<MoveableBridgeData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PointOfInterest>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		MoveableBridgeData moveableBridgeData = default(MoveableBridgeData);
		moveableBridgeData.m_LiftOffsets = m_LiftOffsets;
		moveableBridgeData.m_MovingTime = m_MovingTime;
		((EntityManager)(ref entityManager)).SetComponentData<MoveableBridgeData>(entity, moveableBridgeData);
	}
}
