using System;
using System.Collections.Generic;
using Game.Common;
using Game.Triggers;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Triggers/", new Type[] { })]
public class ChirpPrefab : PrefabBase
{
	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<ChirpData>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.GetArchetypeComponents(components);
		components.Add(ComponentType.ReadWrite<Game.Triggers.Chirp>());
		components.Add(ComponentType.ReadWrite<ChirpEntity>());
		components.Add(ComponentType.ReadWrite<PrefabRef>());
	}

	public override void LateInitialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.LateInitialize(entityManager, entity);
		RefreshArchetype(entityManager, entity);
	}

	protected virtual void RefreshArchetype(EntityManager entityManager, Entity entity)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		List<ComponentBase> list = new List<ComponentBase>();
		GetComponents(list);
		HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].GetArchetypeComponents(hashSet);
		}
		hashSet.Add(ComponentType.ReadWrite<Created>());
		hashSet.Add(ComponentType.ReadWrite<Updated>());
		((EntityManager)(ref entityManager)).SetComponentData<ChirpData>(entity, new ChirpData
		{
			m_Archetype = ((EntityManager)(ref entityManager)).CreateArchetype(PrefabUtils.ToArray(hashSet)),
			m_Flags = ChirpDataFlags.None
		});
	}
}
