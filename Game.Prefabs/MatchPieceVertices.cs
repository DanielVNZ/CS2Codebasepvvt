using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Net/", new Type[] { typeof(NetPiecePrefab) })]
public class MatchPieceVertices : ComponentBase
{
	public float[] m_Offsets;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<NetVertexMatchData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		NetVertexMatchData netVertexMatchData = new NetVertexMatchData
		{
			m_Offsets = float3.op_Implicit(float.NaN)
		};
		if (m_Offsets != null)
		{
			if (m_Offsets.Length >= 1)
			{
				netVertexMatchData.m_Offsets.x = m_Offsets[0];
			}
			if (m_Offsets.Length >= 2)
			{
				netVertexMatchData.m_Offsets.y = m_Offsets[1];
			}
			if (m_Offsets.Length >= 3)
			{
				netVertexMatchData.m_Offsets.z = m_Offsets[2];
			}
		}
		((EntityManager)(ref entityManager)).SetComponentData<NetVertexMatchData>(entity, netVertexMatchData);
	}
}
