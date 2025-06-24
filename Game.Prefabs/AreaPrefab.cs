using System;
using System.Collections.Generic;
using Game.Areas;
using Game.Common;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Areas/", new Type[] { })]
public class AreaPrefab : PrefabBase
{
	public Color m_Color = Color.white;

	public Color m_EdgeColor = Color.white;

	public Color m_SelectionColor = Color.white;

	public Color m_SelectionEdgeColor = Color.white;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<AreaData>());
		components.Add(ComponentType.ReadWrite<AreaColorData>());
		components.Add(ComponentType.ReadWrite<PlaceableInfoviewItem>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Node>());
		components.Add(ComponentType.ReadWrite<Triangle>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		hashSet.Add(ComponentType.ReadWrite<Area>());
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		AreaData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AreaData>(entity);
		componentData.m_Archetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		((EntityManager)(ref entityManager)).SetComponentData<AreaData>(entity, componentData);
	}
}
