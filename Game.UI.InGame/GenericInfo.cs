using System;
using Colossal.UI.Binding;
using Unity.Entities;

namespace Game.UI.InGame;

public class GenericInfo : ISubsectionSource, IJsonWritable
{
	private readonly Func<Entity, Entity, bool> m_ShouldDisplay;

	private readonly Action<Entity, Entity, GenericInfo> m_OnUpdate;

	public string label { get; set; }

	public string value { get; set; }

	public Entity target { get; set; }

	public GenericInfo(Func<Entity, Entity, bool> shouldDisplay, Action<Entity, Entity, GenericInfo> onUpdate)
	{
		m_ShouldDisplay = shouldDisplay;
		m_OnUpdate = onUpdate;
	}

	public bool DisplayFor(Entity entity, Entity prefab)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return m_ShouldDisplay(entity, prefab);
	}

	public void OnRequestUpdate(Entity entity, Entity prefab)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_OnUpdate(entity, prefab, this);
	}

	public void Write(IJsonWriter writer)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("label");
		writer.Write(label ?? string.Empty);
		writer.PropertyName("value");
		writer.Write(value ?? string.Empty);
		writer.PropertyName("target");
		if (target == Entity.Null)
		{
			writer.WriteNull();
		}
		else
		{
			UnityWriters.Write(writer, target);
		}
		writer.TypeEnd();
	}
}
