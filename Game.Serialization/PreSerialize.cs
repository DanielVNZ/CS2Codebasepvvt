using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Serialization;

public class PreSerialize<T> : GameSystemBase where T : ComponentSystemBase, IPreSerialize
{
	private SaveGameSystem m_SaveGameSystem;

	private T m_System;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_SaveGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SaveGameSystem>();
		m_System = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<T>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		m_System.PreSerialize(m_SaveGameSystem.context);
	}

	[Preserve]
	public PreSerialize()
	{
	}
}
