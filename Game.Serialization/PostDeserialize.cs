using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.Serialization;

public class PostDeserialize<T> : GameSystemBase where T : ComponentSystemBase, IPostDeserialize
{
	private LoadGameSystem m_LoadGameSystem;

	private T m_System;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_System = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<T>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		m_System.PostDeserialize(m_LoadGameSystem.context);
	}

	[Preserve]
	public PostDeserialize()
	{
	}
}
