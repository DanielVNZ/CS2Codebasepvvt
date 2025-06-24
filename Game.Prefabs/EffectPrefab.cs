using System.Collections.Generic;
using Game.Common;
using Game.Effects;
using Unity.Entities;

namespace Game.Prefabs;

public class EffectPrefab : TransformPrefab
{
	public EffectCondition m_Conditions;

	public bool m_DisableDistanceCulling;

	public override void GetDependencies(List<PrefabBase> prefabs)
	{
		base.GetDependencies(prefabs);
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<EffectInstance>());
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<EffectData>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<EffectData>(entity, new EffectData
		{
			m_Archetype = GetArchetype(entityManager, entity),
			m_Flags = new EffectCondition
			{
				m_RequiredFlags = m_Conditions.m_RequiredFlags,
				m_ForbiddenFlags = m_Conditions.m_ForbiddenFlags,
				m_IntensityFlags = m_Conditions.m_IntensityFlags
			},
			m_OwnerCulling = !m_DisableDistanceCulling
		});
	}

	private EntityArchetype GetArchetype(EntityManager entityManager, Entity entity)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		return ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet));
	}
}
