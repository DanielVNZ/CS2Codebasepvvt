using System;
using System.Collections.Generic;
using Game.Creatures;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Ambience/", new Type[] { })]
public class AmbienceEmitterPrefab : ComponentBase
{
	public GroupAmbienceType m_AmbienceType;

	public float m_Intensity = 1f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AmbienceEmitterData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AmbienceEmitter>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<AmbienceEmitterData>(entity, new AmbienceEmitterData
		{
			m_AmbienceType = m_AmbienceType,
			m_Intensity = m_Intensity
		});
	}
}
