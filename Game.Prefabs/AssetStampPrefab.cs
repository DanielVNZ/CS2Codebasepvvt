using System;
using System.Collections.Generic;
using Game.Common;
using Game.Objects;
using Game.Rendering;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[ComponentMenu("Objects/", new Type[] { })]
public class AssetStampPrefab : ObjectPrefab
{
	[InputField]
	[Range(1f, 1000f)]
	public int m_Width = 4;

	[InputField]
	[Range(1f, 1000f)]
	public int m_Depth = 4;

	public uint m_ConstructionCost;

	public uint m_UpKeepCost;

	public override bool canIgnoreUnlockDependencies => false;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ObjectGeometryData>());
		components.Add(ComponentType.ReadWrite<AssetStampData>());
		components.Add(ComponentType.ReadWrite<PlaceableObjectData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Static>());
		components.Add(ComponentType.ReadWrite<AssetStamp>());
		components.Add(ComponentType.ReadWrite<CullingInfo>());
		components.Add(ComponentType.ReadWrite<PseudoRandomSeed>());
	}
}
