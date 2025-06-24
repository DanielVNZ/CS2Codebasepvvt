using System;
using System.Collections.Generic;
using Game.Tutorials;
using Unity.Entities;

namespace Game.Prefabs;

[ComponentMenu("Tutorials/Triggers/", new Type[] { })]
public class TutorialPolicyAdjustmentTriggerPrefab : TutorialTriggerPrefabBase
{
	public PolicyAdjustmentTriggerFlags m_Flags;

	public PolicyAdjustmentTriggerTargetFlags m_TargetFlags;

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<PolicyAdjustmentTriggerData>());
	}

	public override void Initialize(EntityManager entityManager, Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(entityManager, entity);
		((EntityManager)(ref entityManager)).SetComponentData<PolicyAdjustmentTriggerData>(entity, new PolicyAdjustmentTriggerData(m_Flags, m_TargetFlags));
	}
}
