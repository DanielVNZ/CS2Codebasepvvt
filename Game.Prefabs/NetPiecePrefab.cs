using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Net/Piece/", new Type[] { })]
public class NetPiecePrefab : RenderPrefab
{
	public NetPieceLayer m_Layer;

	public float m_Width = 3f;

	public float m_Length = 64f;

	public Bounds1 m_HeightRange = new Bounds1(0f, 3f);

	public float m_WidthOffset;

	public float m_NodeOffset = 0.5f;

	public float m_SideConnectionOffset;

	public float4 m_SurfaceHeights = float4.op_Implicit(0f);

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<NetPieceData>());
		components.Add(ComponentType.ReadWrite<MeshMaterial>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		if (!TryGet<LodProperties>(out var component) || component.m_LodMeshes == null)
		{
			return;
		}
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		for (int i = 0; i < component.m_LodMeshes.Length; i++)
		{
			Entity entity2 = orCreateSystemManaged.GetEntity(component.m_LodMeshes[i]);
			if (!((EntityManager)(ref entityManager)).HasBuffer<MeshMaterial>(entity2))
			{
				((EntityManager)(ref entityManager)).AddComponent<MeshMaterial>(entity2);
			}
		}
	}
}
