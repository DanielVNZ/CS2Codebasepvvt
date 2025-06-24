using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialUIActivationSystem : GameSystemBase, ITutorialUIActivationSystem
{
	protected EntityCommandBufferSystem m_BarrierSystem;

	private readonly Dictionary<string, List<Entity>> m_TutorialMap = new Dictionary<string, List<Entity>>();

	private readonly List<string> m_ActiveTags = new List<string>();

	private EntityQuery m_TutorialQuery;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BarrierSystem = (EntityCommandBufferSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_TutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<UIActivationData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		RebuildTutorialMap();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		RebuildTutorialMap();
		m_ActiveTags.Clear();
	}

	private void RebuildTutorialMap()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		m_TutorialMap.Clear();
		if (((EntityQuery)(ref m_TutorialQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<Entity> val = ((EntityQuery)(ref m_TutorialQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		PrefabSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		for (int i = 0; i < val.Length; i++)
		{
			Entity item = val[i];
			string[] array = orCreateSystemManaged.GetPrefab<TutorialPrefab>(val[i]).GetComponent<TutorialUIActivation>().m_UITagProvider?.uiTag?.Split('|', StringSplitOptions.None);
			if (array == null)
			{
				continue;
			}
			for (int j = 0; j < array.Length; j++)
			{
				string key = array[j].Trim();
				if (!m_TutorialMap.ContainsKey(key))
				{
					m_TutorialMap[key] = new List<Entity>();
				}
				if (!m_TutorialMap[key].Contains(item))
				{
					m_TutorialMap[key].Add(item);
				}
			}
		}
		val.Dispose();
	}

	public void SetTag(string tag, bool active)
	{
		if (m_TutorialMap.ContainsKey(tag))
		{
			m_ActiveTags.Remove(tag);
			if (active)
			{
				m_ActiveTags.Add(tag);
			}
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (m_ActiveTags.Count <= 0)
		{
			return;
		}
		EntityCommandBuffer val = m_BarrierSystem.CreateCommandBuffer();
		foreach (string item in m_ActiveTags)
		{
			if (!m_TutorialMap.TryGetValue(item, out var value))
			{
				continue;
			}
			foreach (Entity item2 in value)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<TutorialCompleted>(item2))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).AddComponent<TutorialActivated>(item2);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).GetComponentData<UIActivationData>(item2).m_CanDeactivate)
					{
						((EntityCommandBuffer)(ref val)).AddComponent<ForceActivation>(item2);
					}
				}
			}
		}
	}

	[Preserve]
	public TutorialUIActivationSystem()
	{
	}
}
