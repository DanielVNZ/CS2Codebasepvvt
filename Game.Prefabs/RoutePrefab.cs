using System;
using System.Collections.Generic;
using Game.Common;
using Game.Rendering;
using Game.Routes;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Routes/", new Type[] { })]
public class RoutePrefab : PrefabBase, IColored
{
	public Material m_Material;

	public float m_Width = 4f;

	public float m_SegmentLength = 64f;

	public Color m_Color = Color.magenta;

	public string m_LocaleID;

	public Color32 color => Color32.op_Implicit(m_Color);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<RouteData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<Route>()))
		{
			components.Add(ComponentType.ReadWrite<RouteWaypoint>());
			components.Add(ComponentType.ReadWrite<RouteSegment>());
			components.Add(ComponentType.ReadWrite<Color>());
			components.Add(ComponentType.ReadWrite<RouteBufferIndex>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Waypoint>()))
		{
			components.Add(ComponentType.ReadWrite<Position>());
			components.Add(ComponentType.ReadWrite<Owner>());
		}
		else if (components.Contains(ComponentType.ReadWrite<Segment>()))
		{
			components.Add(ComponentType.ReadWrite<CurveElement>());
			components.Add(ComponentType.ReadWrite<Owner>());
		}
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		HashSet<ComponentType> hashSet2 = new HashSet<ComponentType>();
		HashSet<ComponentType> hashSet3 = new HashSet<ComponentType>();
		HashSet<ComponentType> hashSet4 = new HashSet<ComponentType>();
		hashSet.Add(ComponentType.ReadWrite<Route>());
		hashSet2.Add(ComponentType.ReadWrite<Waypoint>());
		hashSet3.Add(ComponentType.ReadWrite<Waypoint>());
		hashSet3.Add(ComponentType.ReadWrite<Connected>());
		hashSet4.Add(ComponentType.ReadWrite<Segment>());
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
			list[i].GetArchetypeComponents(hashSet2);
			list[i].GetArchetypeComponents(hashSet3);
			list[i].GetArchetypeComponents(hashSet4);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet2.Add(ComponentType.ReadWrite<Created>());
		hashSet3.Add(ComponentType.ReadWrite<Created>());
		hashSet4.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		hashSet2.Add(ComponentType.ReadWrite<Updated>());
		hashSet3.Add(ComponentType.ReadWrite<Updated>());
		hashSet4.Add(ComponentType.ReadWrite<Updated>());
		RouteData componentData = ((EntityManager)(ref entityManager)).GetComponentData<RouteData>(entity);
		componentData.m_RouteArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
		componentData.m_WaypointArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet2));
		componentData.m_ConnectedArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet3));
		componentData.m_SegmentArchetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet4));
		((EntityManager)(ref entityManager)).SetComponentData<RouteData>(entity, componentData);
	}
}
