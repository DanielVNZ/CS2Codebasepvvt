using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.UI.Binding;
using Game.Reflection;

namespace Game.UI.Widgets;

public class EnumField : Field<ulong>, IWarning
{
	private int m_ItemsVersion = -1;

	private bool m_Warning;

	[CanBeNull]
	public Func<bool> warningAction { get; set; }

	public EnumMember[] enumMembers { get; set; } = Array.Empty<EnumMember>();

	[CanBeNull]
	public Func<int> itemsVersion { get; set; }

	public ITypedValueAccessor<EnumMember[]> itemsAccessor { get; set; }

	public bool warning
	{
		get
		{
			return m_Warning;
		}
		set
		{
			warningAction = null;
			m_Warning = value;
		}
	}

	public EnumField()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		base.valueWriter = (IWriter<ulong>)new ULongWriter();
		base.valueReader = (IReader<ulong>)new ULongReader();
	}

	protected override WidgetChanges Update()
	{
		WidgetChanges widgetChanges = base.Update();
		if (itemsAccessor != null)
		{
			int num = itemsVersion?.Invoke() ?? 0;
			if (num != m_ItemsVersion)
			{
				widgetChanges |= WidgetChanges.Properties;
				m_ItemsVersion = num;
				enumMembers = itemsAccessor.GetTypedValue() ?? Array.Empty<EnumMember>();
			}
		}
		if (warningAction != null)
		{
			bool flag = warningAction();
			if (flag != m_Warning)
			{
				m_Warning = flag;
				widgetChanges |= WidgetChanges.Properties;
			}
		}
		return widgetChanges;
	}

	protected override void WriteProperties(IJsonWriter writer)
	{
		base.WriteProperties(writer);
		writer.PropertyName("enumMembers");
		JsonWriterExtensions.Write<EnumMember>(writer, (IList<EnumMember>)enumMembers);
		writer.PropertyName("warning");
		writer.Write(warning);
	}
}
