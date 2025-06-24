using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetPiecePrefab) })]
public class BuildableNetPiece : ComponentBase
{
	public float3 m_Position;

	public float m_Width;

	public float3 m_SnapPosition;

	public float m_SnapWidth;

	public bool m_AllowOnBridge;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<NetPieceArea>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}
