using System;
using System.Collections.Generic;
using Game.Common;
using Game.Net;
using Game.Rendering;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Net/Lane/", new Type[] { })]
public class NetLaneGeometryPrefab : NetLanePrefab
{
	public NetLaneMeshInfo[] m_Meshes;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		for (int i = 0; i < m_Meshes.Length; i++)
		{
			prefabs.Add(m_Meshes[i].m_Mesh);
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<NetLaneGeometryData>());
		components.Add(ComponentType.ReadWrite<SubMesh>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		if (components.Contains(ComponentType.ReadWrite<MasterLane>()))
		{
			return;
		}
		components.Add(ComponentType.ReadWrite<LaneGeometry>());
		components.Add(ComponentType.ReadWrite<CullingInfo>());
		components.Add(ComponentType.ReadWrite<MeshBatch>());
		bool flag = false;
		if (m_Meshes != null)
		{
			for (int i = 0; i < m_Meshes.Length; i++)
			{
				RenderPrefab mesh = m_Meshes[i].m_Mesh;
				flag |= mesh.Has<ColorProperties>();
			}
		}
		if (flag)
		{
			components.Add(ComponentType.ReadWrite<PseudoRandomSeed>());
			components.Add(ComponentType.ReadWrite<MeshColor>());
		}
	}
}
