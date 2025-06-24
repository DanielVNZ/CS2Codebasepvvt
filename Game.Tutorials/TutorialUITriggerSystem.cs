using System;
using System.Collections.Generic;
using Game.Common;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public class TutorialUITriggerSystem : TutorialTriggerSystemBase, ITutorialUITriggerSystem
{
	private PrefabSystem m_PrefabSystem;

	private EntityArchetype m_UnlockEventArchetype;

	private readonly HashSet<string> m_ActivatedTriggers = new HashSet<string>();

	public void ActivateTrigger(string trigger)
	{
		m_ActivatedTriggers.Add(trigger);
	}

	public void DisactivateTrigger(string trigger)
	{
		m_ActivatedTriggers.Remove(trigger);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ActiveTriggerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<UITriggerData>(),
			ComponentType.ReadOnly<TriggerActive>(),
			ComponentType.Exclude<TriggerCompleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		if (m_ActivatedTriggers.Count <= 0 || ((EntityQuery)(ref m_ActiveTriggerQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<Entity> val = ((EntityQuery)(ref m_ActiveTriggerQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityCommandBuffer commandBuffer = m_BarrierSystem.CreateCommandBuffer();
		for (int i = 0; i < val.Length; i++)
		{
			TutorialUITriggerPrefab.UITriggerInfo[] uITriggers = m_PrefabSystem.GetPrefab<TutorialUITriggerPrefab>(val[i]).m_UITriggers;
			foreach (TutorialUITriggerPrefab.UITriggerInfo uITriggerInfo in uITriggers)
			{
				string[] array = uITriggerInfo.m_UITagProvider.uiTag?.Split('|', StringSplitOptions.None);
				if (array == null)
				{
					continue;
				}
				bool flag = false;
				for (int k = 0; k < array.Length; k++)
				{
					if (m_ActivatedTriggers.Contains(array[k]))
					{
						if ((Object)(object)uITriggerInfo.m_GoToPhase != (Object)null)
						{
							Entity entity = m_PrefabSystem.GetEntity(uITriggerInfo.m_GoToPhase);
							((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TutorialNextPhase>(val[i], new TutorialNextPhase
							{
								m_NextPhase = entity
							});
							((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TriggerPreCompleted>(val[i]);
						}
						else if (uITriggerInfo.m_CompleteManually)
						{
							((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TriggerPreCompleted>(val[i]);
						}
						else
						{
							((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TriggerCompleted>(val[i]);
						}
						TutorialSystem.ManualUnlock(val[i], m_UnlockEventArchetype, ((ComponentSystemBase)this).EntityManager, commandBuffer);
						DisactivateTrigger(array[k]);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		val.Dispose();
	}

	[Preserve]
	public TutorialUITriggerSystem()
	{
	}
}
