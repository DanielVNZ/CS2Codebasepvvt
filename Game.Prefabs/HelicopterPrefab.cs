using System;
using System.Collections.Generic;
using Game.PSI;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ExcludeGeneratedModTag]
[ComponentMenu("Vehicles/", new Type[] { })]
public class HelicopterPrefab : AircraftPrefab
{
	public float m_FlyingMaxSpeed = 250f;

	public float m_FlyingAcceleration = 10f;

	public float m_FlyingAngularAcceleration = 10f;

	public float m_AccelerationSwayFactor = 0.5f;

	public float m_VelocitySwayFactor = 0.7f;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			yield return GetHelicopterType().ToString();
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<HelicopterData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Helicopter>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		HelicopterData helicopterData = default(HelicopterData);
		helicopterData.m_HelicopterType = GetHelicopterType();
		helicopterData.m_FlyingMaxSpeed = m_FlyingMaxSpeed / 3.6f;
		helicopterData.m_FlyingAcceleration = m_FlyingAcceleration;
		helicopterData.m_FlyingAngularAcceleration = math.radians(m_FlyingAngularAcceleration);
		helicopterData.m_AccelerationSwayFactor = m_AccelerationSwayFactor;
		helicopterData.m_VelocitySwayFactor = m_VelocitySwayFactor / helicopterData.m_FlyingMaxSpeed;
		((EntityManager)(ref entityManager)).SetComponentData<HelicopterData>(entity, helicopterData);
	}

	protected virtual HelicopterType GetHelicopterType()
	{
		return HelicopterType.Helicopter;
	}
}
