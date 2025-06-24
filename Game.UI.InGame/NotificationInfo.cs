using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Game.UI.InGame;

public class NotificationInfo : IComparable<NotificationInfo>
{
	private readonly List<Entity> m_Targets;

	public Entity entity { get; }

	public Entity target { get; }

	public int priority { get; }

	public int count => m_Targets?.Count ?? 0;

	public NotificationInfo(Notification notification)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		entity = notification.entity;
		target = notification.target;
		priority = (int)notification.priority;
		m_Targets = new List<Entity>(10) { notification.target };
	}

	public void AddTarget(Entity otherTarget)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Targets.Contains(otherTarget))
		{
			m_Targets.Add(otherTarget);
		}
	}

	public int CompareTo(NotificationInfo other)
	{
		return priority - other.priority;
	}
}
