using System.Collections.Generic;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public abstract class AircraftPrefab : VehiclePrefab
{
	public SizeClass m_SizeClass;

	public float m_GroundMaxSpeed = 100f;

	public float m_GroundAcceleration = 3f;

	public float m_GroundBraking = 5f;

	public float2 m_GroundTurning = new float2(90f, 15f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AircraftData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Aircraft>());
		if (components.Contains(ComponentType.ReadWrite<Stopped>()))
		{
			components.Add(ComponentType.ReadWrite<ParkedCar>());
		}
		if (components.Contains(ComponentType.ReadWrite<Moving>()))
		{
			components.Add(ComponentType.ReadWrite<AircraftNavigation>());
			components.Add(ComponentType.ReadWrite<AircraftNavigationLane>());
			components.Add(ComponentType.ReadWrite<AircraftCurrentLane>());
			components.Add(ComponentType.ReadWrite<PathOwner>());
			components.Add(ComponentType.ReadWrite<PathElement>());
			components.Add(ComponentType.ReadWrite<Target>());
			components.Add(ComponentType.ReadWrite<Blocker>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<AircraftData>(entity, new AircraftData
		{
			m_SizeClass = m_SizeClass,
			m_GroundMaxSpeed = m_GroundMaxSpeed / 3.6f,
			m_GroundAcceleration = m_GroundAcceleration,
			m_GroundBraking = m_GroundBraking,
			m_GroundTurning = math.radians(m_GroundTurning)
		});
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(10));
	}
}
