using Game.Notifications;
using Unity.Entities;

namespace Game.UI.InGame;

public readonly struct Notification
{
	public Entity entity { get; }

	public Entity target { get; }

	public IconPriority priority { get; }

	public Notification(Entity entity, Entity target, IconPriority priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.entity = entity;
		this.target = target;
		this.priority = priority;
	}
}
