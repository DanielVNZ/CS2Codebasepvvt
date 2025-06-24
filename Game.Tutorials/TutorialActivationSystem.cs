using System;
using System.Collections.Generic;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Tutorials;

public class TutorialActivationSystem : GameSystemBase
{
	private readonly List<GameSystemBase> m_Systems = new List<GameSystemBase>();

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialUIActivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialAutoActivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialControlSchemeActivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialObjectSelectedActivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialInfoviewActivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialFireActivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialHealthProblemActivationSystem>());
		m_Systems.Add(((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialEventActivationSystem>());
		((ComponentSystemBase)this).Enabled = false;
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame() || mode.IsEditor();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		foreach (GameSystemBase system in m_Systems)
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
	public TutorialActivationSystem()
	{
	}
}
