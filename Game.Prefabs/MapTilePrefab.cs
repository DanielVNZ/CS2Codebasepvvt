using System;
using System.Collections.Generic;
using Colossal.Collections;
using Game.Areas;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { })]
public class MapTilePrefab : AreaPrefab
{
	[Serializable]
	public class FeatureInfo
	{
		public MapFeature m_MapFeature;

		public float m_Cost;
	}

	public float m_PurchaseCostFactor = 2500f;

	public FeatureInfo[] m_MapFeatures;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<MapTileData>());
		components.Add(ComponentType.ReadWrite<MapFeatureData>());
		components.Add(ComponentType.ReadWrite<AreaGeometryData>());
		components.Add(ComponentType.ReadWrite<TilePurchaseCostFactor>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<MapTile>());
		components.Add(ComponentType.ReadWrite<MapFeatureElement>());
		components.Add(ComponentType.ReadWrite<Geometry>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		DynamicBuffer<MapFeatureData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<MapFeatureData>(entity, false);
		CollectionUtils.ResizeInitialized<MapFeatureData>(buffer, 9, default(MapFeatureData));
		for (int i = 0; i < m_MapFeatures.Length; i++)
		{
			FeatureInfo featureInfo = m_MapFeatures[i];
			buffer[(int)featureInfo.m_MapFeature] = new MapFeatureData(featureInfo.m_Cost);
		}
		TilePurchaseCostFactor tilePurchaseCostFactor = new TilePurchaseCostFactor(m_PurchaseCostFactor);
		((EntityManager)(ref entityManager)).SetComponentData<TilePurchaseCostFactor>(entity, tilePurchaseCostFactor);
	}
}
