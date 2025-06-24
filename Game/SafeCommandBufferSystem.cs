using System;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game;

public class SafeCommandBufferSystem : EntityCommandBufferSystem
{
	private bool m_IsAllowed = true;

	public void AllowUsage()
	{
		m_IsAllowed = true;
	}

	public EntityCommandBuffer CreateCommandBuffer()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsAllowed)
		{
			return ((EntityCommandBufferSystem)this).CreateCommandBuffer();
		}
		throw new Exception("Trying to create EntityCommandBuffer when it's not allowed!");
	}

	[Preserve]
	protected override void OnUpdate()
	{
		m_IsAllowed = false;
		((EntityCommandBufferSystem)this).OnUpdate();
	}

	[Preserve]
	public SafeCommandBufferSystem()
	{
	}
}
