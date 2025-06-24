using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { })]
public class OverlayConfigurationPrefab : PrefabBase
{
	public Material m_CurveMaterial;

	public Material m_ObjectBrushMaterial;

	public FontInfo[] m_FontInfos;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<OverlayConfigurationData>());
	}
}
