using System;
using System.Collections.Generic;
using Game.Simulation;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(ObjectPrefab) })]
public class WaterSource : ComponentBase
{
	public float m_Radius = 50f;

	public float m_Amount = 1f;

	public float m_Polluted;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WaterSourceData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Simulation.WaterSourceData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<WaterSourceData>(entity, new WaterSourceData
		{
			m_Radius = m_Radius,
			m_Amount = m_Amount,
			m_InitialPolluted = m_Polluted
		});
	}
}
