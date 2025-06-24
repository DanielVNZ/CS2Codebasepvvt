using System;
using System.Collections.Generic;
using Colossal.Mathematics;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Rendering/", new Type[] { typeof(RenderPrefab) })]
public class OverlayProperties : ComponentBase
{
	public bool m_IsWaterway;

	public Bounds2 m_TextureArea = new Bounds2(float2.op_Implicit(0f), float2.op_Implicit(1f));

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
	}
}
