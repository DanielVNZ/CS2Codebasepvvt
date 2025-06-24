using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/", new Type[] { })]
public class ModeSettingParameters : PrefabBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ModeSettingData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<ModeSettingData>(entity, new ModeSettingData
		{
			m_Enable = false
		});
	}
}
