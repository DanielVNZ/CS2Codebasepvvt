using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Asset Packs/", new Type[]
{
	typeof(ZonePrefab),
	typeof(ObjectPrefab),
	typeof(NetPrefab),
	typeof(AreaPrefab),
	typeof(RoutePrefab),
	typeof(NetLanePrefab)
})]
public class AssetPackItem : ComponentBase
{
	[NotNull]
	public AssetPackPrefab[] m_Packs;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if (m_Packs == null)
		{
			return;
		}
		for (int i = 0; i < m_Packs.Length; i++)
		{
			if ((Object)(object)m_Packs[i] != (Object)null)
			{
				prefabs.Add(m_Packs[i]);
			}
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AssetPackElement>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		DynamicBuffer<AssetPackElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<AssetPackElement>(entity, false);
		buffer.Clear();
		if (m_Packs == null || m_Packs.Length == 0)
		{
			return;
		}
		PrefabSystem existingSystemManaged = ((EntityManager)(ref entityManager)).World.GetExistingSystemManaged<PrefabSystem>();
		AssetPackElement assetPackElement = default(AssetPackElement);
		for (int i = 0; i < m_Packs.Length; i++)
		{
			AssetPackPrefab assetPackPrefab = m_Packs[i];
			if (!((Object)(object)assetPackPrefab == (Object)null))
			{
				assetPackElement.m_Pack = existingSystemManaged.GetEntity(assetPackPrefab);
				buffer.Add(assetPackElement);
			}
		}
	}
}
