using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs.Effects;

[ComponentMenu("Effects/", new Type[] { typeof(EffectPrefab) })]
public class VehicleSFX : ComponentBase
{
	public float2 m_SpeedLimits;

	public float2 m_SpeedPitches;

	public float2 m_SpeedVolumes;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<VehicleAudioEffectData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		VehicleAudioEffectData vehicleAudioEffectData = new VehicleAudioEffectData
		{
			m_SpeedLimits = m_SpeedLimits,
			m_SpeedPitches = m_SpeedPitches,
			m_SpeedVolumes = m_SpeedVolumes
		};
		((EntityManager)(ref entityManager)).SetComponentData<VehicleAudioEffectData>(entity, vehicleAudioEffectData);
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}
