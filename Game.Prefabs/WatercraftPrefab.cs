using System;
using System.Collections.Generic;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[] { })]
public class WatercraftPrefab : VehiclePrefab
{
	public SizeClass m_SizeClass = SizeClass.Large;

	public EnergyTypes m_EnergyType = EnergyTypes.Fuel;

	public float m_MaxSpeed = 150f;

	public float m_Acceleration = 1f;

	public float m_Braking = 2f;

	public float2 m_Turning = new float2(30f, 5f);

	public float m_AngularAcceleration = 5f;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			yield return "Ship";
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<WatercraftData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Watercraft>());
		if (components.Contains(ComponentType.ReadWrite<Moving>()))
		{
			components.Add(ComponentType.ReadWrite<WatercraftNavigation>());
			components.Add(ComponentType.ReadWrite<WatercraftNavigationLane>());
			components.Add(ComponentType.ReadWrite<WatercraftCurrentLane>());
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
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		WatercraftData watercraftData = default(WatercraftData);
		watercraftData.m_SizeClass = m_SizeClass;
		watercraftData.m_EnergyType = m_EnergyType;
		watercraftData.m_MaxSpeed = m_MaxSpeed / 3.6f;
		watercraftData.m_Acceleration = m_Acceleration;
		watercraftData.m_Braking = m_Braking;
		watercraftData.m_Turning = math.radians(m_Turning);
		watercraftData.m_AngularAcceleration = math.radians(m_AngularAcceleration);
		((EntityManager)(ref entityManager)).SetComponentData<WatercraftData>(entity, watercraftData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(8));
	}
}
