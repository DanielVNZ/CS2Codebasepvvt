using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.VFX;

namespace Game.Prefabs;

[ComponentMenu("VFX/", new Type[] { typeof(EffectPrefab) })]
public class VFX : ComponentBase
{
	public VisualEffectAsset m_Effect;

	public int m_MaxCount;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<VFXData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}
}
