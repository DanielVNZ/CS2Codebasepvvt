using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialUIDeactivationSystem : TutorialDeactivationSystemBase, ITutorialUIDeactivationSystem
{
	private PrefabSystem m_PrefabSystem;

	private readonly HashSet<string> m_Deactivate = new HashSet<string>();

	private EntityQuery m_PendingTutorialQuery;

	private EntityQuery m_ActiveTutorialQuery;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PendingTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<UIActivationData>(),
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<ForceActivation>()
		});
		m_ActiveTutorialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<UIActivationData>(),
			ComponentType.ReadOnly<TutorialData>(),
			ComponentType.ReadOnly<TutorialActivated>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<TutorialActive>(),
			ComponentType.Exclude<TutorialCompleted>(),
			ComponentType.Exclude<ForceActivation>()
		});
	}

	public void DeactivateTag(string tag)
	{
		m_Deactivate.Add(tag);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (m_Deactivate.Count > 0)
		{
			if (!((EntityQuery)(ref m_PendingTutorialQuery)).IsEmptyIgnoreFilter)
			{
				CheckDeactivate(m_PendingTutorialQuery);
			}
			if (!((EntityQuery)(ref m_ActiveTutorialQuery)).IsEmptyIgnoreFilter && base.phaseCanDeactivate)
			{
				CheckDeactivate(m_ActiveTutorialQuery);
			}
		}
		m_Deactivate.Clear();
	}

	private void CheckDeactivate(EntityQuery query)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<PrefabData> val = ((EntityQuery)(ref query)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Entity> val2 = ((EntityQuery)(ref query)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityCommandBuffer val3 = m_BarrierSystem.CreateCommandBuffer();
		for (int i = 0; i < val2.Length; i++)
		{
			string[] array = m_PrefabSystem.GetPrefab<TutorialPrefab>(val[i]).GetComponent<TutorialUIActivation>().m_UITagProvider.uiTag?.Split('|', StringSplitOptions.None);
			if (array == null)
			{
				continue;
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (m_Deactivate.Contains(array[j].Trim()))
				{
					((EntityCommandBuffer)(ref val3)).RemoveComponent<TutorialActivated>(val2[i]);
				}
			}
		}
		val.Dispose();
		val2.Dispose();
	}

	[Preserve]
	public TutorialUIDeactivationSystem()
	{
	}
}
