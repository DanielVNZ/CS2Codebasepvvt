using Game.Common;
using Game.Input;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public class TutorialInputTriggerSystem : TutorialTriggerSystemBase
{
	private EntityArchetype m_UnlockEventArchetype;

	private PrefabSystem m_PrefabSystem;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ActiveTriggerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<InputTriggerData>(),
			ComponentType.ReadOnly<TriggerActive>(),
			ComponentType.Exclude<TriggerCompleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		((ComponentSystemBase)this).RequireForUpdate(m_ActiveTriggerQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		NativeArray<InputTriggerData> val = ((EntityQuery)(ref m_ActiveTriggerQuery)).ToComponentDataArray<InputTriggerData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Entity> val2 = ((EntityQuery)(ref m_ActiveTriggerQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityCommandBuffer commandBuffer = m_BarrierSystem.CreateCommandBuffer();
		for (int i = 0; i < val.Length; i++)
		{
			TutorialInputTriggerPrefab prefab = m_PrefabSystem.GetPrefab<TutorialInputTriggerPrefab>(val2[i]);
			if (Performed(prefab))
			{
				((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TriggerCompleted>(val2[i]);
				TutorialSystem.ManualUnlock(val2[i], m_UnlockEventArchetype, ((ComponentSystemBase)this).EntityManager, commandBuffer);
			}
		}
		val.Dispose();
		val2.Dispose();
	}

	private bool Performed(TutorialInputTriggerPrefab prefab)
	{
		for (int i = 0; i < prefab.m_Actions.Length; i++)
		{
			if (InputManager.instance.TryFindAction(prefab.m_Actions[i].m_Map, prefab.m_Actions[i].m_Action, out var action) && action.WasPerformedThisFrame())
			{
				return true;
			}
		}
		return false;
	}

	[Preserve]
	public TutorialInputTriggerSystem()
	{
	}
}
