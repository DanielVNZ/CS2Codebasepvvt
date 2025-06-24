using System;
using System.Collections.Generic;
using Game.Objects;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(StaticObjectPrefab) })]
public class StandingObject : ComponentBase
{
	public float3 m_LegSize = new float3(0.3f, 2.5f, 0.3f);

	public float2 m_LegGap;

	public bool m_CircularLeg = true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<ObjectGeometryData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		ObjectGeometryData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ObjectGeometryData>(entity);
		componentData.m_LegSize = m_LegSize;
		componentData.m_LegOffset = math.select(default(float2), (m_LegGap + ((float3)(ref m_LegSize)).xz) * 0.5f, m_LegGap != 0f);
		componentData.m_Flags |= (GeometryFlags)(m_CircularLeg ? 384 : 128);
		((EntityManager)(ref entityManager)).SetComponentData<ObjectGeometryData>(entity, componentData);
	}
}
