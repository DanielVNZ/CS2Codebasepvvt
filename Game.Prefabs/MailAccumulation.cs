using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Services/", new Type[]
{
	typeof(ServicePrefab),
	typeof(ZonePrefab)
})]
public class MailAccumulation : ComponentBase
{
	public bool m_RequireCollect;

	public float m_SendingRate = 1f;

	public float m_ReceivingRate = 1f;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<MailAccumulationData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		MailAccumulationData mailAccumulationData = default(MailAccumulationData);
		mailAccumulationData.m_RequireCollect = m_RequireCollect;
		mailAccumulationData.m_AccumulationRate.x = m_SendingRate;
		mailAccumulationData.m_AccumulationRate.y = m_ReceivingRate;
		((EntityManager)(ref entityManager)).SetComponentData<MailAccumulationData>(entity, mailAccumulationData);
	}
}
