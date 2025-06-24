using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs.Effects;

[ComponentMenu("Effects/", new Type[] { })]
public class SelectedSound : ComponentBase
{
	public EffectPrefab m_SelectedSound;

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<SelectedSoundData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		PrefabSystem orCreateSystemManaged = ((EntityManager)(ref entityManager)).World.GetOrCreateSystemManaged<PrefabSystem>();
		SelectedSoundData selectedSoundData = new SelectedSoundData
		{
			m_selectedSound = orCreateSystemManaged.GetEntity(m_SelectedSound)
		};
		((EntityManager)(ref entityManager)).SetComponentData<SelectedSoundData>(entity, selectedSoundData);
	}

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
		prefabs.Add(m_SelectedSound);
	}
}
