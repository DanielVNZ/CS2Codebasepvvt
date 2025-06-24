using Colossal.UI.Binding;
using Unity.Entities;

namespace Game.UI.InGame;

public readonly struct UITransportType
{
	private readonly Entity m_Prefab;

	public string id { get; }

	public string icon { get; }

	public bool locked { get; }

	public UITransportType(Entity prefab, string id, string icon, bool locked)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Prefab = prefab;
		this.id = id;
		this.icon = icon;
		this.locked = locked;
	}

	public void Write(PrefabUISystem prefabUISystem, IJsonWriter writer)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("id");
		writer.Write(id);
		writer.PropertyName("icon");
		writer.Write(icon);
		writer.PropertyName("locked");
		writer.Write(locked);
		writer.PropertyName("requirements");
		prefabUISystem.BindPrefabRequirements(writer, m_Prefab);
		writer.TypeEnd();
	}
}
