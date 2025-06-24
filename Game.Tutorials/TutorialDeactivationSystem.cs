using System;
using System.Collections.Generic;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public class TutorialDeactivationSystem : GameSystemBase
{
	private List<TutorialDeactivationSystemBase> m_Systems = new List<TutorialDeactivationSystemBase>();

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialControlSchemeDeactivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialUIDeactivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialObjectSelectionDeactivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialInfoviewDeactivationSystem>());
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
		foreach (TutorialDeactivationSystemBase system in m_Systems)
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

	[Preserve]
	public TutorialDeactivationSystem()
	{
	}
}
