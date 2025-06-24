using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[]
{
	typeof(BuildingPrefab),
	typeof(BuildingExtensionPrefab)
})]
public class PollutionModifier : ComponentBase
{
	[Tooltip("Factor to increase (+) or decrease (-) ground pollution")]
	[Range(-1f, 1f)]
	public float m_GroundPollutionMultiplier;

	[Tooltip("Factor to increase (+) or decrease (-) air pollution")]
	[Range(-1f, 1f)]
	public float m_AirPollutionMultiplier;

	[Tooltip("Factor to increase (+) or decrease (-) noise pollution")]
	[Range(-1f, 1f)]
	public float m_NoisePollutionMultiplier;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PollutionModifierData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!base.prefab.Has<ServiceUpgrade>())
		{
			ComponentBase.baseLog.ErrorFormat((Object)(object)base.prefab, "PollutionModifier should only be added to service upgrades: {0}", (object)((Object)base.prefab).name);
		}
		((EntityManager)(ref entityManager)).SetComponentData<PollutionModifierData>(entity, new PollutionModifierData
		{
			m_GroundPollutionMultiplier = m_GroundPollutionMultiplier,
			m_AirPollutionMultiplier = m_AirPollutionMultiplier,
			m_NoisePollutionMultiplier = m_NoisePollutionMultiplier
		});
	}
}
