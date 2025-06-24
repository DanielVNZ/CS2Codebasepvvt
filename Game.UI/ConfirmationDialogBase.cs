using Colossal.Annotations;
using Colossal.UI.Binding;
using Game.UI.Localization;

namespace Game.UI;

public abstract class ConfirmationDialogBase : IJsonWritable
{
	protected const string kDefaultSkin = "Default";

	protected const string kParadoxSkin = "Paradox";

	private LocalizedString? title;

	private LocalizedString message;

	private LocalizedString confirmAction;

	private LocalizedString? cancelAction;

	[CanBeNull]
	private LocalizedString[] otherActions;

	private LocalizedString? details;

	private bool copyButton;

	protected virtual string skin => "Default";

	protected virtual bool dismissible => false;

	protected ConfirmationDialogBase(LocalizedString? title, LocalizedString message, LocalizedString? details, bool copyButton, LocalizedString confirmAction, LocalizedString? cancelAction, [CanBeNull] params LocalizedString[] otherActions)
	{
		this.title = title;
		this.message = message;
		this.confirmAction = confirmAction;
		this.cancelAction = cancelAction;
		this.otherActions = otherActions;
		this.details = details;
		this.copyButton = copyButton;
	}

	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("dismissible");
		writer.Write(dismissible);
		writer.PropertyName("skin");
		writer.Write(skin);
		writer.PropertyName("title");
		JsonWriterExtensions.Write<LocalizedString>(writer, title);
		writer.PropertyName("message");
		JsonWriterExtensions.Write<LocalizedString>(writer, message);
		writer.PropertyName("confirmAction");
		JsonWriterExtensions.Write<LocalizedString>(writer, confirmAction);
		writer.PropertyName("cancelAction");
		JsonWriterExtensions.Write<LocalizedString>(writer, cancelAction);
		writer.PropertyName("otherActions");
		if (otherActions != null)
		{
			JsonWriterExtensions.ArrayBegin(writer, otherActions.Length);
			for (int i = 0; i < otherActions.Length; i++)
			{
				JsonWriterExtensions.Write<LocalizedString>(writer, otherActions[i]);
			}
			writer.ArrayEnd();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
		writer.PropertyName("details");
		JsonWriterExtensions.Write<LocalizedString>(writer, details);
		writer.PropertyName("copyButton");
		writer.Write(copyButton);
		writer.TypeEnd();
	}
}
