using System;
using System.Collections.Generic;
using Game.Common;
using Game.Objects;
using Game.Rendering;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { })]
public class MarkerObjectPrefab : ObjectPrefab
{
	public RenderPrefab m_Mesh;

	public bool m_Circular;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_Mesh);
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ObjectGeometryData>());
		components.Add(ComponentType.ReadWrite<SubMesh>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Static>());
		components.Add(ComponentType.ReadWrite<Marker>());
		components.Add(ComponentType.ReadWrite<CullingInfo>());
		components.Add(ComponentType.ReadWrite<MeshBatch>());
		components.Add(ComponentType.ReadWrite<PseudoRandomSeed>());
	}
}
