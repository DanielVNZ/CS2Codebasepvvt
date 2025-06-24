using System;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Prefab/", new Type[] { })]
public class MapTileMode : LocalModePrefab
{
	public MapTilePrefab m_Prefab;

	public MapTilePrefab.FeatureInfo[] m_MapFeatures;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		MapTilePrefab mapTilePrefab = m_Prefab;
		if ((Object)(object)mapTilePrefab == (Object)null)
		{
			ComponentBase.baseLog.Critical((object)$"Target not found {this}");
			return;
		}
		Entity entity = prefabSystem.GetEntity(mapTilePrefab);
		((EntityManager)(ref entityManager)).GetComponentData<TilePurchaseCostFactor>(entity);
		((EntityManager)(ref entityManager)).GetBuffer<MapFeatureData>(entity, false);
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		MapTilePrefab mapTilePrefab = m_Prefab;
		if ((Object)(object)mapTilePrefab == (Object)null)
		{
			ComponentBase.baseLog.Critical((object)$"Target not found {this}");
			return;
		}
		Entity entity = prefabSystem.GetEntity(mapTilePrefab);
		DynamicBuffer<MapFeatureData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<MapFeatureData>(entity, false);
		for (int i = 0; i < m_MapFeatures.Length; i++)
		{
			MapTilePrefab.FeatureInfo featureInfo = m_MapFeatures[i];
			buffer[(int)featureInfo.m_MapFeature] = new MapFeatureData(featureInfo.m_Cost);
		}
		TilePurchaseCostFactor tilePurchaseCostFactor = new TilePurchaseCostFactor(mapTilePrefab.m_PurchaseCostFactor);
		((EntityManager)(ref entityManager)).SetComponentData<TilePurchaseCostFactor>(entity, tilePurchaseCostFactor);
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		MapTilePrefab mapTilePrefab = m_Prefab;
		if ((Object)(object)mapTilePrefab == (Object)null)
		{
			ComponentBase.baseLog.Critical((object)$"Target not found {this}");
			return;
		}
		Entity entity = prefabSystem.GetEntity(mapTilePrefab);
		DynamicBuffer<MapFeatureData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<MapFeatureData>(entity, false);
		for (int i = 0; i < mapTilePrefab.m_MapFeatures.Length; i++)
		{
			MapTilePrefab.FeatureInfo featureInfo = mapTilePrefab.m_MapFeatures[i];
			buffer[(int)featureInfo.m_MapFeature] = new MapFeatureData(featureInfo.m_Cost);
		}
		TilePurchaseCostFactor tilePurchaseCostFactor = new TilePurchaseCostFactor(mapTilePrefab.m_PurchaseCostFactor);
		((EntityManager)(ref entityManager)).SetComponentData<TilePurchaseCostFactor>(entity, tilePurchaseCostFactor);
	}
}
