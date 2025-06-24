using System;
using System.Collections.Generic;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[] { })]
public class AirplanePrefab : AircraftPrefab
{
	public float2 m_FlyingSpeed = new float2(200f, 1000f);

	public float m_FlyingAcceleration = 20f;

	public float m_FlyingBraking = 20f;

	public float m_FlyingTurning = 10f;

	public float m_FlyingAngularAcceleration = 20f;

	public float m_ClimbAngle = 20f;

	public float m_SlowPitchAngle = 20f;

	public float m_TurningRollFactor = 0.5f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AirplaneData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Airplane>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<AirplaneData>(entity, new AirplaneData
		{
			m_FlyingSpeed = m_FlyingSpeed / 3.6f,
			m_FlyingAcceleration = m_FlyingAcceleration,
			m_FlyingBraking = m_FlyingBraking,
			m_FlyingTurning = math.radians(m_FlyingTurning),
			m_FlyingAngularAcceleration = math.radians(m_FlyingAngularAcceleration),
			m_ClimbAngle = math.radians(m_ClimbAngle),
			m_SlowPitchAngle = math.radians(m_SlowPitchAngle),
			m_TurningRollFactor = m_TurningRollFactor
		});
	}
}
