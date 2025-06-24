using System;
using System.Collections.Generic;
using Game.Common;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/Prefab/", new Type[] { })]
public class NetPrefab : PrefabBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<NetData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Node>()))
		{
			components.Add(ComponentType.ReadWrite<ConnectedEdge>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Edge>()))
		{
			components.Add(ComponentType.ReadWrite<ConnectedNode>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		HashSet<ComponentType> hashSet2 = new HashSet<ComponentType>();
		hashSet.Add(ComponentType.ReadWrite<Node>());
		hashSet2.Add(ComponentType.ReadWrite<Edge>());
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
			list[i].GetArchetypeComponents(hashSet2);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet2.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		hashSet2.Add(ComponentType.ReadWrite<Updated>());
		NetData componentData = ((EntityManager)(ref entityManager)).GetComponentData<NetData>(entity);
		componentData.m_NodeArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		componentData.m_EdgeArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet2));
		((EntityManager)(ref entityManager)).SetComponentData<NetData>(entity, componentData);
	}
}
