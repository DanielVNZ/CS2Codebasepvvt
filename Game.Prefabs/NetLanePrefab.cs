using System;
using System.Collections.Generic;
using Game.Common;
using Game.Net;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/Lane/", new Type[] { })]
public class NetLanePrefab : PrefabBase
{
	public PathfindPrefab m_PathfindPrefab;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		if ((Object)(object)m_PathfindPrefab != (Object)null)
		{
			prefabs.Add(m_PathfindPrefab);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<NetLaneData>());
		components.Add(ComponentType.ReadWrite<NetLaneArchetypeData>());
		if (!base.prefab.Has<SecondaryLane>())
		{
			components.Add(ComponentType.ReadWrite<SecondaryNetLane>());
		}
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Curve>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		hashSet.Add(ComponentType.ReadWrite<Lane>());
		NetLaneArchetypeData netLaneArchetypeData = default(NetLaneArchetypeData);
		netLaneArchetypeData.m_LaneArchetype = CreateArchetype(entityManager, list, hashSet);
		hashSet.Add(ComponentType.ReadWrite<Lane>());
		hashSet.Add(ComponentType.ReadWrite<AreaLane>());
		netLaneArchetypeData.m_AreaLaneArchetype = CreateArchetype(entityManager, list, hashSet);
		hashSet.Add(ComponentType.ReadWrite<Lane>());
		hashSet.Add(ComponentType.ReadWrite<EdgeLane>());
		netLaneArchetypeData.m_EdgeLaneArchetype = CreateArchetype(entityManager, list, hashSet);
		hashSet.Add(ComponentType.ReadWrite<Lane>());
		hashSet.Add(ComponentType.ReadWrite<NodeLane>());
		netLaneArchetypeData.m_NodeLaneArchetype = CreateArchetype(entityManager, list, hashSet);
		hashSet.Add(ComponentType.ReadWrite<Lane>());
		hashSet.Add(ComponentType.ReadWrite<SlaveLane>());
		hashSet.Add(ComponentType.ReadWrite<EdgeLane>());
		netLaneArchetypeData.m_EdgeSlaveArchetype = CreateArchetype(entityManager, list, hashSet);
		hashSet.Add(ComponentType.ReadWrite<Lane>());
		hashSet.Add(ComponentType.ReadWrite<SlaveLane>());
		hashSet.Add(ComponentType.ReadWrite<NodeLane>());
		netLaneArchetypeData.m_NodeSlaveArchetype = CreateArchetype(entityManager, list, hashSet);
		hashSet.Add(ComponentType.ReadWrite<Lane>());
		hashSet.Add(ComponentType.ReadWrite<MasterLane>());
		hashSet.Add(ComponentType.ReadWrite<EdgeLane>());
		netLaneArchetypeData.m_EdgeMasterArchetype = CreateArchetype(entityManager, list, hashSet);
		hashSet.Add(ComponentType.ReadWrite<Lane>());
		hashSet.Add(ComponentType.ReadWrite<MasterLane>());
		hashSet.Add(ComponentType.ReadWrite<NodeLane>());
		netLaneArchetypeData.m_NodeMasterArchetype = CreateArchetype(entityManager, list, hashSet);
		((EntityManager)(ref entityManager)).SetComponentData<NetLaneArchetypeData>(entity, netLaneArchetypeData);
	}

	private EntityArchetype CreateArchetype(EntityManager entityManager, List<ComponentBase> unityComponents, HashSet<ComponentType> laneComponents)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < unityComponents.Count; i++)
		{
			unityComponents[i].GetArchetypeComponents(laneComponents);
		}
		laneComponents.Add(ComponentType.ReadWrite<Created>());
		laneComponents.Add(ComponentType.ReadWrite<Updated>());
		EntityArchetype result = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(laneComponents));
		laneComponents.Clear();
		return result;
	}
}
