using System;
using System.Collections.Generic;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Serialization;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public class TutorialTriggerSystem : GameSystemBase, IPreDeserialize
{
	private readonly List<TutorialTriggerSystemBase> m_Systems = new List<TutorialTriggerSystemBase>();

	private EntityQuery m_TriggerQuery;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialObjectPlacementTriggerSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialInputTriggerSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialAreaTriggerSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialObjectSelectionTriggerSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialUpgradeTriggerSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialUITriggerSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialPolicyAdjustmentTriggerSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialZoningTriggerSystem>());
		m_TriggerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TutorialTriggerData>() });
		((ComponentSystemBase)this).Enabled = false;
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		foreach (TutorialTriggerSystemBase system in m_Systems)
		{
			try
			{
				((ComponentSystemBase)system).Update();
			}
			catch (Exception ex)
			{
				COSystemBase.baseLog.Critical(ex);
			}
		}
	}

	public void PreDeserialize(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent<TriggerActive>(m_TriggerQuery);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent<TriggerPreCompleted>(m_TriggerQuery);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent<TriggerCompleted>(m_TriggerQuery);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent<TutorialNextPhase>(m_TriggerQuery);
	}

	[Preserve]
	public TutorialTriggerSystem()
	{
	}
}
