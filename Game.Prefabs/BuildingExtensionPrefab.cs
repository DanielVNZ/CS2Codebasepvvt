using System;
using System.Collections.Generic;
using Game.Buildings;
using Game.Effects;
using Game.Objects;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[ComponentMenu("Buildings/", new Type[] { })]
public class BuildingExtensionPrefab : StaticObjectPrefab
{
	public bool m_ExternalLot;

	public float3 m_Position;

	public int2 m_OverrideLotSize;

	public float m_OverrideHeight;

	public override IEnumerable<string> modTags
	{
		get
		{
			foreach (string modTag in base.modTags)
			{
				yield return modTag;
			}
			yield return "Building";
		}
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<BuildingExtensionData>());
		components.Add(ComponentType.ReadWrite<Effect>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Extension>());
		components.Add(ComponentType.ReadWrite<SpawnLocationElement>());
		components.Add(ComponentType.ReadWrite<Color>());
		components.Add(ComponentType.ReadWrite<EnabledEffect>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
	}
}
