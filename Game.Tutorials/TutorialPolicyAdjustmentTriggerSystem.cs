using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Common;
using Game.Policies;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialPolicyAdjustmentTriggerSystem : TutorialTriggerSystemBase
{
	private EntityQuery m_AdjustmentQuery;

	private EntityQuery m_PolicyQuery;

	private EntityArchetype m_UnlockEventArchetype;

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
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ActiveTriggerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PolicyAdjustmentTriggerData>(),
			ComponentType.ReadOnly<TriggerActive>(),
			ComponentType.Exclude<TriggerCompleted>()
		});
		m_PolicyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Policy>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_AdjustmentQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<Modify>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ActiveTriggerQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		if (base.triggersChanged && !((EntityQuery)(ref m_PolicyQuery)).IsEmptyIgnoreFilter)
		{
			EntityCommandBuffer commandBuffer = m_BarrierSystem.CreateCommandBuffer();
			NativeArray<Entity> policyEntities = ((EntityQuery)(ref m_PolicyQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<PolicyAdjustmentTriggerData> val = ((EntityQuery)(ref m_ActiveTriggerQuery)).ToComponentDataArray<PolicyAdjustmentTriggerData>(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<Entity> val2 = ((EntityQuery)(ref m_ActiveTriggerQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int i = 0; i < val2.Length; i++)
			{
				if (FirstTimeCheck(val[i], policyEntities))
				{
					((EntityCommandBuffer)(ref commandBuffer)).AddComponent<TriggerPreCompleted>(val2[i]);
					TutorialSystem.ManualUnlock(val2[i], m_UnlockEventArchetype, ((ComponentSystemBase)this).EntityManager, commandBuffer);
				}
			}
			val.Dispose();
			val2.Dispose();
			policyEntities.Dispose();
		}
		if (((EntityQuery)(ref m_AdjustmentQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		EntityCommandBuffer commandBuffer2 = m_BarrierSystem.CreateCommandBuffer();
		NativeArray<PolicyAdjustmentTriggerData> val3 = ((EntityQuery)(ref m_ActiveTriggerQuery)).ToComponentDataArray<PolicyAdjustmentTriggerData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Entity> val4 = ((EntityQuery)(ref m_ActiveTriggerQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Modify> adjustments = ((EntityQuery)(ref m_AdjustmentQuery)).ToComponentDataArray<Modify>(AllocatorHandle.op_Implicit((Allocator)3));
		for (int j = 0; j < val3.Length; j++)
		{
			if (Check(val3[j], adjustments))
			{
				((EntityCommandBuffer)(ref commandBuffer2)).AddComponent<TriggerCompleted>(val4[j]);
				TutorialSystem.ManualUnlock(val4[j], m_UnlockEventArchetype, ((ComponentSystemBase)this).EntityManager, commandBuffer2);
			}
		}
		val3.Dispose();
		val4.Dispose();
		adjustments.Dispose();
	}

	private bool Check(PolicyAdjustmentTriggerData data, NativeArray<Modify> adjustments)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < adjustments.Length; i++)
		{
			if ((data.m_TargetFlags & PolicyAdjustmentTriggerTargetFlags.District) != 0)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<District>(adjustments[i].m_Entity) && (adjustments[i].m_Flags & PolicyFlags.Active) != 0 && (data.m_Flags & PolicyAdjustmentTriggerFlags.Activated) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool FirstTimeCheck(PolicyAdjustmentTriggerData data, NativeArray<Entity> policyEntities)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < policyEntities.Length; i++)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Policy> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Policy>(policyEntities[i], true);
			if ((data.m_TargetFlags & PolicyAdjustmentTriggerTargetFlags.District) != 0)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<District>(policyEntities[i]) && AnyActive(buffer) && (data.m_Flags & PolicyAdjustmentTriggerFlags.Activated) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool AnyActive(DynamicBuffer<Policy> policies)
	{
		for (int i = 0; i < policies.Length; i++)
		{
			if ((policies[i].m_Flags & PolicyFlags.Active) != 0)
			{
				return true;
			}
		}
		return false;
	}

	[Preserve]
	public TutorialPolicyAdjustmentTriggerSystem()
	{
	}
}
