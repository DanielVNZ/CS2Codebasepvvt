using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Economy;
using Game.Objects;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { typeof(StaticObjectPrefab) })]
public class QuantityObject : ComponentBase
{
	public ResourceInEditor[] m_Resources;

	public MapFeature m_MapFeature = MapFeature.None;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<QuantityObjectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Quantity>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		QuantityObjectData quantityObjectData = default(QuantityObjectData);
		if (m_Resources != null)
		{
			for (int i = 0; i < m_Resources.Length; i++)
			{
				quantityObjectData.m_Resources |= EconomyUtils.GetResource(m_Resources[i]);
			}
		}
		quantityObjectData.m_MapFeature = m_MapFeature;
		((EntityManager)(ref entityManager)).SetComponentData<QuantityObjectData>(entity, quantityObjectData);
		if (quantityObjectData.m_Resources == Resource.NoResource && quantityObjectData.m_MapFeature == MapFeature.None && !base.prefab.Has<PlaceholderObject>())
		{
			ComponentBase.baseLog.WarnFormat((Object)(object)base.prefab, "QuantityObject has no resource: {0}", (object)((Object)base.prefab).name);
		}
	}
}
