using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Creatures;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Creatures/", new Type[] { typeof(AnimalPrefab) })]
public class Wildlife : ComponentBase
{
	public Bounds1 m_TripLength = new Bounds1(20f, 200f);

	public Bounds1 m_IdleTime = new Bounds1(10f, 60f);

	public int m_MinGroupMemberCount = 1;

	public int m_MaxGroupMemberCount = 4;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<WildlifeData>());
		components.Add(ComponentType.ReadWrite<UpdateFrameData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Creatures.Wildlife>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		WildlifeData wildlifeData = default(WildlifeData);
		wildlifeData.m_TripLength = m_TripLength;
		wildlifeData.m_IdleTime = m_IdleTime;
		wildlifeData.m_GroupMemberCount.x = m_MinGroupMemberCount;
		wildlifeData.m_GroupMemberCount.y = m_MaxGroupMemberCount;
		((EntityManager)(ref entityManager)).SetComponentData<WildlifeData>(entity, wildlifeData);
		((EntityManager)(ref entityManager)).SetComponentData<UpdateFrameData>(entity, new UpdateFrameData(13));
	}
}
