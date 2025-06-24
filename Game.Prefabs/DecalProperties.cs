using System;
using System.Collections.Generic;
using Colossal;
using Colossal.Mathematics;
using Game.Rendering;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[] { typeof(RenderPrefab) })]
public class DecalProperties : ComponentBase
{
	public Bounds2 m_TextureArea = new Bounds2(float2.op_Implicit(0f), float2.op_Implicit(1f));

	public int m_RendererPriority;

	[BitMask]
	public DecalLayers m_LayerMask = DecalLayers.Terrain | DecalLayers.Roads | DecalLayers.Buildings;

	public bool m_EnableInfoviewColor;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}
}
