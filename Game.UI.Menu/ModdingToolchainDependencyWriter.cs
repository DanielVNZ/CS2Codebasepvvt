using System;
using Colossal.UI.Binding;
using Game.Modding.Toolchain;
using Game.UI.Localization;

namespace Game.UI.Menu;

public class ModdingToolchainDependencyWriter : IWriter<IToolchainDependency>
{
	public void Write(IJsonWriter writer, IToolchainDependency value)
	{
		if (value != null)
		{
			writer.TypeBegin(value.GetType().FullName);
			writer.PropertyName("name");
			JsonWriterExtensions.Write<LocalizedString>(writer, value.localizedName);
			writer.PropertyName("state");
			writer.Write((int)value.state.m_State);
			writer.PropertyName("progress");
			writer.Write(value.state.m_Progress ?? (-1));
			writer.PropertyName("details");
			JsonWriterExtensions.Write<LocalizedString>(writer, value.GetLocalizedState(includeProgress: false));
			writer.PropertyName("version");
			JsonWriterExtensions.Write<LocalizedString>(writer, value.GetLocalizedVersion());
			writer.PropertyName("icon");
			writer.Write(value.icon);
			writer.TypeEnd();
			return;
		}
		writer.WriteNull();
		throw new ArgumentNullException("value", "Null passed to non-nullable value writer");
	}
}
