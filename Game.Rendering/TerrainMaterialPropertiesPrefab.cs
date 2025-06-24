using System.Collections.Generic;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine;

namespace Game.Rendering;

public class TerrainMaterialPropertiesPrefab : PrefabBase
{
	public Material m_SplatmapMaterial;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<TerrainMaterialPropertiesData>());
	}
}
