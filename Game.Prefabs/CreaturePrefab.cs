using System;
using System.Collections.Generic;
using Game.Creatures;
using Game.Objects;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Creatures/", new Type[] { })]
public class CreaturePrefab : MovingObjectPrefab
{
	public GenderMask m_Gender = GenderMask.Any;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<CreatureData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Creature>());
		components.Add(ComponentType.ReadWrite<Color>());
		components.Add(ComponentType.ReadWrite<Surface>());
	}
}
