using System;
using System.Collections.Generic;
using Game.Areas;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { typeof(LotPrefab) })]
public class TerrainArea : ComponentBase
{
	public float m_HeightOffset = 20f;

	public float m_SlopeWidth = 20f;

	public float m_NoiseScale = 100f;

	public float m_NoiseFactor = 1f;

	public bool m_AbsoluteHeight;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<TerrainAreaData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Terrain>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		TerrainAreaData terrainAreaData = default(TerrainAreaData);
		terrainAreaData.m_HeightOffset = m_HeightOffset;
		terrainAreaData.m_SlopeWidth = m_SlopeWidth;
		terrainAreaData.m_NoiseScale = 1f / math.max(0.001f, m_NoiseScale);
		terrainAreaData.m_NoiseFactor = m_NoiseFactor;
		terrainAreaData.m_AbsoluteHeight = (m_AbsoluteHeight ? 1f : 0f);
		((EntityManager)(ref entityManager)).SetComponentData<TerrainAreaData>(entity, terrainAreaData);
	}
}
