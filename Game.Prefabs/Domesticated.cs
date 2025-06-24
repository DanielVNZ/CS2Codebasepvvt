using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Creatures;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Creatures/", new Type[] { typeof(AnimalPrefab) })]
public class Domesticated : ComponentBase
{
	public Bounds1 m_IdleTime = new Bounds1(20f, 120f);

	public int m_MinGroupMemberCount = 1;

	public int m_MaxGroupMemberCount = 2;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<DomesticatedData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Creatures.Domesticated>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		DomesticatedData domesticatedData = default(DomesticatedData);
		domesticatedData.m_IdleTime = m_IdleTime;
		domesticatedData.m_GroupMemberCount.x = m_MinGroupMemberCount;
		domesticatedData.m_GroupMemberCount.y = m_MaxGroupMemberCount;
		((EntityManager)(ref entityManager)).SetComponentData<DomesticatedData>(entity, domesticatedData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(9));
	}
}
