using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetPiecePrefab) })]
public class NetTerrainPiece : ComponentBase
{
	public float2 m_WidthOffset;

	public float2 m_ClipHeightOffset;

	public float3 m_MinHeightOffset;

	public float3 m_MaxHeightOffset;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<NetTerrainData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}
