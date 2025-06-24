using System;
using System.Collections.Generic;
using Game.Events;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class Fire : ComponentBase
{
	public EventTargetType m_RandomTargetType;

	public float m_StartProbability = 0.01f;

	public float m_StartIntensity = 1f;

	public float m_EscalationRate = 1f / 60f;

	public float m_SpreadProbability = 1f;

	public float m_SpreadRange = 20f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<FireData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.Fire>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		FireData fireData = default(FireData);
		fireData.m_RandomTargetType = m_RandomTargetType;
		fireData.m_StartProbability = m_StartProbability;
		fireData.m_StartIntensity = m_StartIntensity;
		fireData.m_EscalationRate = m_EscalationRate;
		fireData.m_SpreadProbability = m_SpreadProbability;
		fireData.m_SpreadRange = m_SpreadRange;
		((EntityManager)(ref entityManager)).SetComponentData<FireData>(entity, fireData);
	}
}
