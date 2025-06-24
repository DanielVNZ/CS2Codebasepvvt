using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using Unity.Entities;

namespace Game.UI.InGame;

public class InfoList : ISubsectionSource, IJsonWritable
{
	public readonly struct Item : IJsonWritable
	{
		public static readonly Entity kNullEntity = Entity.Null;

		public string text { get; }

		public Entity entity { get; }

		public Item(string text, Entity entity)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			this.text = text;
			this.entity = entity;
		}

		public Item(string text)
			: this(text, kNullEntity)
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


		public void Write(IJsonWriter writer)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("text");
			writer.Write(text);
			writer.PropertyName("entity");
			if (entity == Entity.Null)
			{
				writer.WriteNull();
			}
			else
			{
				UnityWriters.Write(writer, entity);
			}
			writer.TypeEnd();
		}
	}

	private readonly Func<Entity, Entity, bool> m_ShouldDisplay;

	private readonly Action<Entity, Entity, InfoList> m_OnUpdate;

	public string label { get; set; }

	private List<Item> list { get; set; }

	private bool expanded { get; set; }

	public InfoList(Func<Entity, Entity, bool> shouldDisplay, Action<Entity, Entity, InfoList> onUpdate)
	{
		list = new List<Item>();
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		list.Clear();
		m_OnUpdate(entity, prefab, this);
	}

	public void Add(Item item)
	{
		list.Add(item);
	}

	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("expanded");
		writer.Write(expanded);
		writer.PropertyName("label");
		writer.Write(label);
		writer.PropertyName("list");
		JsonWriterExtensions.ArrayBegin(writer, list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			JsonWriterExtensions.Write<Item>(writer, list[i]);
		}
		writer.ArrayEnd();
		writer.TypeEnd();
	}
}
