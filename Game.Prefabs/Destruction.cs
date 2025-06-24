using System;
using System.Collections.Generic;
using Game.Events;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Events/", new Type[] { typeof(EventPrefab) })]
public class Destruction : ComponentBase
{
	public EventTargetType m_RandomTargetType;

	public float m_OccurenceProbability = 0.01f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<DestructionData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Events.Destruction>());
		components.Add(ComponentType.ReadWrite<TargetElement>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		DestructionData destructionData = default(DestructionData);
		destructionData.m_RandomTargetType = m_RandomTargetType;
		destructionData.m_OccurenceProbability = m_OccurenceProbability;
		((EntityManager)(ref entityManager)).SetComponentData<DestructionData>(entity, destructionData);
	}
}
