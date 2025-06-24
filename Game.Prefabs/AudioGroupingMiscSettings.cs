using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Settings/", new Type[] { typeof(AudioGroupingSettingsPrefab) })]
public class AudioGroupingMiscSettings : ComponentBase
{
	public float m_ForestFireDistance = 100f;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<AudioGroupingMiscSetting>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		AudioGroupingMiscSetting audioGroupingMiscSetting = new AudioGroupingMiscSetting
		{
			m_ForestFireDistance = m_ForestFireDistance
		};
		((EntityManager)(ref entityManager)).SetComponentData<AudioGroupingMiscSetting>(entity, audioGroupingMiscSetting);
	}
}
