using System;
using System.Collections.Generic;
using Game.Events;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class WaterLevelChangeComponent : ComponentBase
{
	public WaterLevelTargetType m_TargetType;

	public WaterLevelChangeType m_ChangeType;

	public float m_EscalationDelay = 1f;

	public bool m_Evacuate;

	public bool m_StayIndoors;

	[Tooltip("How dangerous the disaster is for the cims in the city. Determines how likely cims will leave shelter while the disaster is ongoing")]
	[Range(0f, 1f)]
	public float m_DangerLevel = 1f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WaterLevelChangeData>());
		if (m_ChangeType == WaterLevelChangeType.RainControlled)
		{
			components.Add(ComponentType.ReadWrite<FloodData>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WaterLevelChange>());
		components.Add(ComponentType.ReadWrite<Duration>());
		components.Add(ComponentType.ReadWrite<DangerLevel>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
		if (m_ChangeType == WaterLevelChangeType.RainControlled)
		{
			components.Add(ComponentType.ReadWrite<Flood>());
		}
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		WaterLevelChangeData waterLevelChangeData = default(WaterLevelChangeData);
		waterLevelChangeData.m_TargetType = m_TargetType;
		waterLevelChangeData.m_ChangeType = m_ChangeType;
		waterLevelChangeData.m_EscalationDelay = m_EscalationDelay;
		waterLevelChangeData.m_DangerFlags = (DangerFlags)0u;
		if (m_Evacuate)
		{
			waterLevelChangeData.m_DangerFlags = DangerFlags.Evacuate;
		}
		if (m_StayIndoors)
		{
			waterLevelChangeData.m_DangerFlags = DangerFlags.StayIndoors;
		}
		waterLevelChangeData.m_DangerLevel = m_DangerLevel;
		((EntityManager)(ref entityManager)).SetComponentData<WaterLevelChangeData>(entity, waterLevelChangeData);
	}
}
