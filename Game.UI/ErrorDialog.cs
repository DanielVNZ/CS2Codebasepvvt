using System;
using Colossal.Annotations;
using Colossal.UI.Binding;
using Game.UI.Localization;

namespace Game.UI;

public class ErrorDialog : IJsonWritable
{
	public enum Severity
	{
		Warning,
		Error
	}

	[Flags]
	public enum Actions
	{
		None = 0,
		Quit = 1,
		SaveAndQuit = 2,
		SaveAndContinue = 4,
		Default = 3
	}

	public Severity severity = Severity.Error;

	public Actions actions = Actions.Default;

	public LocalizedString localizedTitle;

	public LocalizedString localizedMessage;

	[CanBeNull]
	public string errorDetails;

	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("severity");
		writer.Write((int)severity);
		writer.PropertyName("actions");
		writer.Write((int)actions);
		writer.PropertyName("localizedTitle");
		JsonWriterExtensions.Write<LocalizedString>(writer, localizedTitle);
		writer.PropertyName("localizedMessage");
		JsonWriterExtensions.Write<LocalizedString>(writer, localizedMessage);
		writer.PropertyName("errorDetails");
		writer.Write(errorDetails);
		writer.TypeEnd();
	}
}
