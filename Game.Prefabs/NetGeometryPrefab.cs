using System;
using System.Collections.Generic;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Net/Prefab/", new Type[] { })]
public class NetGeometryPrefab : NetPrefab
{
	public NetSectionInfo[] m_Sections;

	public NetEdgeStateInfo[] m_EdgeStates;

	public NetNodeStateInfo[] m_NodeStates;

	public float m_MaxSlopeSteepness = 0.2f;

	public AggregateNetPrefab m_AggregateType;

	public GroupPrefab m_StyleType;

	public CompositionInvertMode m_InvertMode;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Sections.Length; i++)
		{
			prefabs.Add(m_Sections[i].m_Section);
		}
		if ((Object)(object)m_AggregateType != (Object)null)
		{
			prefabs.Add(m_AggregateType);
		}
		if ((Object)(object)m_StyleType != (Object)null)
		{
			prefabs.Add(m_StyleType);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<NetGeometryData>());
		components.Add(ComponentType.ReadWrite<NetGeometryComposition>());
		components.Add(ComponentType.ReadWrite<NetGeometrySection>());
		components.Add(ComponentType.ReadWrite<NetGeometryEdgeState>());
		components.Add(ComponentType.ReadWrite<NetGeometryNodeState>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Node>()))
		{
			components.Add(ComponentType.ReadWrite<Game.Net.SubLane>());
			components.Add(ComponentType.ReadWrite<Game.Objects.SubObject>());
			components.Add(ComponentType.ReadWrite<NodeGeometry>());
			components.Add(ComponentType.ReadWrite<CullingInfo>());
			components.Add(ComponentType.ReadWrite<MeshBatch>());
			components.Add(ComponentType.ReadWrite<PseudoRandomSeed>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Edge>()))
		{
			if ((Object)(object)m_AggregateType != (Object)null)
			{
				components.Add(ComponentType.ReadWrite<Aggregated>());
			}
			components.Add(ComponentType.ReadWrite<Game.Net.SubLane>());
			components.Add(ComponentType.ReadWrite<Game.Objects.SubObject>());
			components.Add(ComponentType.ReadWrite<Curve>());
			components.Add(ComponentType.ReadWrite<Composition>());
			components.Add(ComponentType.ReadWrite<EdgeGeometry>());
			components.Add(ComponentType.ReadWrite<StartNodeGeometry>());
			components.Add(ComponentType.ReadWrite<EndNodeGeometry>());
			components.Add(ComponentType.ReadWrite<BuildOrder>());
			components.Add(ComponentType.ReadWrite<CullingInfo>());
			components.Add(ComponentType.ReadWrite<MeshBatch>());
			components.Add(ComponentType.ReadWrite<PseudoRandomSeed>());
		}
		else if (components.Contains(ComponentType.ReadWrite<NetCompositionData>()))
		{
			components.Add(ComponentType.ReadWrite<NetCompositionPiece>());
			components.Add(ComponentType.ReadWrite<NetCompositionMeshRef>());
			components.Add(ComponentType.ReadWrite<NetCompositionArea>());
			components.Add(ComponentType.ReadWrite<NetCompositionObject>());
			components.Add(ComponentType.ReadWrite<NetCompositionCarriageway>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		NetGeometryData componentData = ((EntityManager)(ref entityManager)).GetComponentData<NetGeometryData>(entity);
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		hashSet.Add(ComponentType.ReadWrite<NetCompositionData>());
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		hashSet.Add(ComponentType.ReadWrite<NetCompositionCrosswalk>());
		componentData.m_NodeCompositionArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		hashSet.Remove(ComponentType.ReadWrite<NetCompositionCrosswalk>());
		hashSet.Add(ComponentType.ReadWrite<NetCompositionLane>());
		componentData.m_EdgeCompositionArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		((EntityManager)(ref entityManager)).SetComponentData<NetGeometryData>(entity, componentData);
	}
}
