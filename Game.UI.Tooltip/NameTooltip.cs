using Colossal.Annotations;
using Colossal.UI.Binding;
using Game.UI.Widgets;
using Unity.Entities;

namespace Game.UI.Tooltip;

public class NameTooltip : Widget
{
	[CanBeNull]
	private string m_Icon;

	public Entity m_Entity;

	[CanBeNull]
	public string icon
	{
		get
		{
			return m_Icon;
		}
		set
		{
			if (value != m_Icon)
			{
				m_Icon = value;
				SetPropertiesChanged();
			}
		}
	}

	public Entity entity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Entity;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (value != m_Entity)
			{
				m_Entity = value;
				SetPropertiesChanged();
			}
		}
	}

	public NameSystem nameBinder { get; set; }

	protected override void WriteProperties(IJsonWriter writer)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		base.WriteProperties(writer);
		writer.PropertyName("icon");
		writer.Write(icon);
		writer.PropertyName("name");
		nameBinder.BindName(writer, entity);
	}
}
