using System;
using System.Collections.Generic;
using Game.Net;
using Game.Objects;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[]
{
	typeof(StaticObjectPrefab),
	typeof(MarkerObjectPrefab)
})]
public class NetObject : ComponentBase
{
	public NetPieceRequirements[] m_SetCompositionState;

	public RoadTypes m_RequireRoad;

	public RoadTypes m_RoadPassThrough;

	public TrackTypes m_TrackPassThrough;

	public float m_NodeOffset;

	public bool m_Attached = true;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<NetObjectData>());
		components.Add(ComponentType.ReadWrite<PlaceableObjectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<Game.Objects.NetObject>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		NetObjectData netObjectData = default(NetObjectData);
		NetCompositionHelpers.GetRequirementFlags(m_SetCompositionState, out netObjectData.m_CompositionFlags, out var sectionFlags);
		if (sectionFlags != 0)
		{
			ComponentBase.baseLog.ErrorFormat((Object)(object)base.prefab, "NetObject ({0}) cannot set section flags: {1}", (object)((Object)base.prefab).name, (object)sectionFlags);
		}
		netObjectData.m_RequireRoad = m_RequireRoad;
		netObjectData.m_RoadPassThrough = m_RoadPassThrough;
		netObjectData.m_TrackPassThrough = m_TrackPassThrough;
		if (m_RequireRoad == RoadTypes.Car && m_SetCompositionState != null)
		{
			for (int i = 0; i < m_SetCompositionState.Length; i++)
			{
				if (m_SetCompositionState[i] == NetPieceRequirements.ShipStop)
				{
					netObjectData.m_RequireRoad |= RoadTypes.Watercraft;
				}
			}
		}
		((EntityManager)(ref entityManager)).SetComponentData<NetObjectData>(entity, netObjectData);
		PlaceableObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlaceableObjectData>(entity);
		componentData.m_Flags |= Game.Objects.PlacementFlags.NetObject;
		componentData.m_PlacementOffset.z = m_NodeOffset;
		bool flag = (netObjectData.m_CompositionFlags & CompositionFlags.nodeMask) != default(CompositionFlags);
		bool num = (netObjectData.m_CompositionFlags & ~CompositionFlags.nodeMask) != default(CompositionFlags);
		if (flag)
		{
			componentData.m_Flags |= Game.Objects.PlacementFlags.RoadNode;
			componentData.m_SubReplacementType = SubReplacementType.None;
		}
		if (num || !flag)
		{
			componentData.m_Flags |= Game.Objects.PlacementFlags.RoadEdge;
			componentData.m_SubReplacementType = SubReplacementType.None;
		}
		if ((m_RequireRoad & RoadTypes.Watercraft) != RoadTypes.None)
		{
			componentData.m_Flags |= Game.Objects.PlacementFlags.Waterway;
		}
		if (m_Attached)
		{
			componentData.m_Flags |= Game.Objects.PlacementFlags.Attached;
		}
		((EntityManager)(ref entityManager)).SetComponentData<PlaceableObjectData>(entity, componentData);
	}
}
