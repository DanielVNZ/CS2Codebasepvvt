using System;
using System.Collections.Generic;
using Colossal.Entities;
using Game.Common;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs.Modes;

[ComponentMenu("Modes/Mode Component/", new Type[] { })]
public class UnlockAtStartMode : LocalModePrefab
{
	public PrefabBase[] m_Prefabs;

	public PrefabBase m_Requirement;

	public override void RecordChanges(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Prefabs.Length; i++)
		{
			PrefabBase prefabBase = m_Prefabs[i];
			prefabSystem.GetEntity(prefabBase);
		}
	}

	public override void ApplyModeData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		if ((Object)(object)m_Requirement != (Object)null)
		{
			val = prefabSystem.GetEntity(m_Requirement);
		}
		DynamicBuffer<UnlockRequirement> val2 = default(DynamicBuffer<UnlockRequirement>);
		for (int i = 0; i < m_Prefabs.Length; i++)
		{
			PrefabBase prefabBase = m_Prefabs[i];
			Entity entity = prefabSystem.GetEntity(prefabBase);
			if (EntitiesExtensions.TryGetBuffer<UnlockRequirement>(entityManager, entity, false, ref val2))
			{
				val2.Clear();
				if (val != Entity.Null)
				{
					val2.Add(new UnlockRequirement(val, UnlockFlags.RequireAll));
				}
				((EntityManager)(ref entityManager)).AddComponentData<Updated>(entity, default(Updated));
			}
		}
	}

	public override void RestoreDefaultData(EntityManager entityManager, PrefabSystem prefabSystem)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Prefabs.Length; i++)
		{
			PrefabBase prefabBase = m_Prefabs[i];
			if (prefabBase.TryGetExactly<Unlockable>(out var component))
			{
				Entity entity = prefabSystem.GetEntity(prefabBase);
				List<PrefabBase> list = new List<PrefabBase>();
				prefabBase.GetDependencies(list);
				component.LateInitialize(entityManager, entity, list);
			}
		}
	}
}
