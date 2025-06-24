using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Vehicles/", new Type[] { typeof(VehiclePrefab) })]
public class VehicleSideEffects : ComponentBase
{
	public float2 m_RoadWear = new float2(0.5f, 1f);

	public float2 m_NoisePollution = new float2(0.5f, 1f);

	public float2 m_AirPollution = new float2(0.5f, 1f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<VehicleSideEffectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		VehicleSideEffectData vehicleSideEffectData = default(VehicleSideEffectData);
		vehicleSideEffectData.m_Min = new float3(m_RoadWear.x, m_NoisePollution.x, m_AirPollution.x);
		vehicleSideEffectData.m_Max = new float3(m_RoadWear.y, m_NoisePollution.y, m_AirPollution.y);
		((EntityManager)(ref entityManager)).SetComponentData<VehicleSideEffectData>(entity, vehicleSideEffectData);
	}
}
