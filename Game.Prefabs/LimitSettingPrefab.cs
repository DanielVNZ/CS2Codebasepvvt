using System.Collections.Generic;
using Unity.Entities;

namespace Game.Prefabs;

public class LimitSettingPrefab : PrefabBase
{
	public int m_MaxChirpsLimit = 100;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<LimitSettingData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		base.GetArchetypeComponents(components);
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<LimitSettingData>(entity, new LimitSettingData
		{
			m_MaxChirpsLimit = m_MaxChirpsLimit
		});
	}
}
