using System;
using Colossal.UI.Binding;
using Unity.Entities;

namespace Game.UI.InGame;

public abstract class EntityGamePanel : GamePanel, IEquatable<EntityGamePanel>
{
	public virtual Entity selectedEntity { get; set; } = Entity.Null;

	protected override void BindProperties(IJsonWriter writer)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.BindProperties(writer);
		writer.PropertyName("selectedEntity");
		UnityWriters.Write(writer, selectedEntity);
	}

	public bool Equals(EntityGamePanel other)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (other == null)
		{
			return false;
		}
		if (this != other)
		{
			Entity val = selectedEntity;
			return ((Entity)(ref val)).Equals(other.selectedEntity);
		}
		return true;
	}
}
