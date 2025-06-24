using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Colossal.Annotations;
using Colossal.PSI.Common;
using Colossal.PSI.PdxSdk;
using Colossal.UI.Binding;
using PDX.SDK.Contracts.Enums;
using PDX.SDK.Contracts.Service.Mods.Models;
using UnityEngine;

namespace Game.UI.Menu;

public class ParadoxBindings : CompositeBinding
{
	public abstract class ParadoxDialog : IJsonWritable
	{
		public virtual void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.TypeEnd();
		}
	}

	public class LoginFormData : IJsonReadable
	{
		public string email;

		public string password;

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("email");
			reader.Read(ref email);
			reader.ReadProperty("password");
			reader.Read(ref password);
			reader.ReadMapEnd();
		}
	}

	public class RegistrationFormData : IJsonReadable
	{
		public string email;

		public string password;

		public string country;

		public string dateOfBirth;

		public bool marketingPermission;

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("email");
			reader.Read(ref email);
			reader.ReadProperty("password");
			reader.Read(ref password);
			reader.ReadProperty("country");
			reader.Read(ref country);
			reader.ReadProperty("dateOfBirth");
			reader.Read(ref dateOfBirth);
			reader.ReadProperty("marketingPermission");
			reader.Read(ref marketingPermission);
			reader.ReadMapEnd();
		}
	}

	public abstract class MessageDialog : ParadoxDialog
	{
		[CanBeNull]
		public readonly string icon;

		[CanBeNull]
		public readonly string titleId;

		[NotNull]
		public readonly string messageId;

		[CanBeNull]
		public readonly Dictionary<string, string> messageArgs;

		protected MessageDialog(string icon, string titleId, string messageId, Dictionary<string, string> messageArgs)
		{
			this.icon = icon;
			this.titleId = titleId;
			this.messageId = messageId;
			this.messageArgs = messageArgs;
		}

		public override void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("icon");
			writer.Write(icon);
			writer.PropertyName("titleId");
			writer.Write(titleId);
			writer.PropertyName("messageId");
			writer.Write(messageId);
			writer.PropertyName("messageArgs");
			JsonWriterExtensions.Write(writer, (IReadOnlyDictionary<string, string>)messageArgs);
			writer.TypeEnd();
		}
	}

	public class LoginDialog : ParadoxDialog
	{
	}

	public class RegistrationDialog : ParadoxDialog
	{
	}

	public class AccountLinkDialog : MessageDialog
	{
		public AccountLinkDialog(string icon, string messageId)
			: base(icon, "Paradox.ACCOUNT_LINK_PROMPT_TITLE", messageId, null)
		{
		}
	}

	public class AccountLinkOverwriteDialog : MessageDialog
	{
		public AccountLinkOverwriteDialog(string icon, string messageId)
			: base(icon, "Paradox.ACCOUNT_LINK_OVERWRITE_TITLE", messageId, null)
		{
		}
	}

	public class LegalDocumentDialog : ParadoxDialog
	{
		[NotNull]
		public readonly LegalDocument document;

		public readonly bool agreementRequired;

		public readonly string confirmLabel;

		public LegalDocumentDialog(LegalDocument document, bool agreementRequired = true)
		{
			this.document = document;
			this.agreementRequired = agreementRequired;
			confirmLabel = document.confirmLabel;
		}

		public override void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("text");
			writer.Write(document.content);
			writer.PropertyName("agreementRequired");
			writer.Write(agreementRequired);
			writer.PropertyName("confirmLabel");
			writer.Write(confirmLabel);
			writer.TypeEnd();
		}
	}

	public class ConfirmationDialog : MessageDialog
	{
		public ConfirmationDialog(string icon, string titleId, string messageId, Dictionary<string, string> messageArgs)
			: base(icon, titleId, messageId, messageArgs)
		{
		}
	}

	public class ErrorDialog : ParadoxDialog
	{
		[CanBeNull]
		public readonly string messageId;

		[CanBeNull]
		public readonly string message;

		public ErrorDialog(string messageId, string message)
		{
			this.messageId = messageId;
			this.message = message;
		}

		public override void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("messageId");
			writer.Write(messageId);
			writer.PropertyName("message");
			writer.Write(message);
			writer.TypeEnd();
		}
	}

	public class MultiOptionDialog : ParadoxDialog
	{
		public struct Option : IJsonWritable
		{
			public string m_Id;

			public Action m_OnSelect;

			public void Write(IJsonWriter writer)
			{
				writer.TypeBegin(GetType().Name);
				writer.PropertyName("id");
				writer.Write(m_Id);
				writer.TypeEnd();
			}
		}

		public string m_TitleId;

		public string m_MessageId;

		public Option[] m_Options;

		public MultiOptionDialog(string titleId, string messageId, params Option[] options)
		{
			m_TitleId = titleId;
			m_MessageId = messageId;
			m_Options = options;
		}

		public override void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("titleId");
			writer.Write(m_TitleId);
			writer.PropertyName("messageId");
			writer.Write(m_MessageId);
			writer.PropertyName("options");
			JsonWriterExtensions.Write<Option>(writer, (IList<Option>)m_Options);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "paradox";

	private readonly ValueBinding<bool> m_RequestActiveBinding;

	private readonly ValueBinding<bool> m_LoggedInBinding;

	private readonly ValueBinding<AccountLinkProvider> m_AccountLinkProviderBinding;

	private readonly ValueBinding<int> m_AccountLinkStateBinding;

	private readonly ValueBinding<string> m_UserNameBinding;

	private readonly ValueBinding<string> m_EmailBinding;

	private readonly ValueBinding<string> m_AvatarBinding;

	private readonly ValueBinding<bool> m_HasInternetConnection;

	private readonly ValueBinding<bool> m_IsPDXSDKEnabled;

	private readonly StackBinding<ParadoxDialog> m_ActiveDialogsBinding;

	private PdxSdkPlatform m_PdxPlatform;

	private static readonly string kTermsOfUse = "TERMS_OF_USE";

	private static readonly string kPrivacyPolicy = "PRIVACY_POLICY";

	public ParadoxBindings()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Expected O, but got Unknown
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Expected O, but got Unknown
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Expected O, but got Unknown
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Expected O, but got Unknown
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Expected O, but got Unknown
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Expected O, but got Unknown
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Expected O, but got Unknown
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Expected O, but got Unknown
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_RequestActiveBinding = new ValueBinding<bool>("paradox", "requestActive", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_LoggedInBinding = new ValueBinding<bool>("paradox", "loggedIn", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_AccountLinkProviderBinding = new ValueBinding<AccountLinkProvider>("paradox", "accountLinkProvider", (AccountLinkProvider)2, (IWriter<AccountLinkProvider>)(object)new EnumNameWriter<AccountLinkProvider>(), (EqualityComparer<AccountLinkProvider>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_AccountLinkStateBinding = new ValueBinding<int>("paradox", "accountLinkState", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_UserNameBinding = new ValueBinding<string>("paradox", "userName", (string)null, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_EmailBinding = new ValueBinding<string>("paradox", "email", (string)null, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_AvatarBinding = new ValueBinding<string>("paradox", "avatar", (string)null, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "linkAccount", (Action)LinkAccount));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "unlinkAccount", (Action)UnlinkAccount));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "logout", (Action)Logout));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_ActiveDialogsBinding = new StackBinding<ParadoxDialog>("paradox", "activeDialogs", (IWriter<ParadoxDialog>)(object)new ValueWriter<ParadoxDialog>())));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "closeActiveDialog", (Action)CloseActiveDialog));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "showLoginForm", (Action)ShowLoginForm));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<string>("paradox", "submitPasswordReset", (Action<string>)SubmitPasswordReset, (IReader<string>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<LoginFormData>("paradox", "submitLoginForm", (Action<LoginFormData>)SubmitLoginForm, (IReader<LoginFormData>)(object)new ValueReader<LoginFormData>()));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_HasInternetConnection = new ValueBinding<bool>("paradox", "hasInternetConnection", PlatformManager.instance.hasConnectivity, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)new GetterValueBinding<List<string>>("paradox", "countryCodes", (Func<List<string>>)GetCountryCodes, (IWriter<List<string>>)(object)new ListWriter<string>((IWriter<string>)new StringWriter()), (EqualityComparer<List<string>>)null));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "showRegistrationForm", (Action)ShowRegistrationForm));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<string>("paradox", "showLink", (Action<string>)ShowLink, (IReader<string>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<RegistrationFormData>("paradox", "submitRegistrationForm", (Action<RegistrationFormData>)SubmitRegistrationForm, (IReader<RegistrationFormData>)(object)new ValueReader<RegistrationFormData>()));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "confirmAccountLink", (Action)ConfirmAccountLink));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "confirmAccountLinkOverwrite", (Action)ConfirmAccountLinkOverwrite));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "markLegalDocumentAsViewed", (Action)MarkLegalDocumentAsViewed));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "showTermsOfUse", (Action)ShowTermsOfUse));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("paradox", "showPrivacyPolicy", (Action)ShowPrivacyPolicy));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<int>("paradox", "onOptionSelected", (Action<int>)OnOptionSelected, (IReader<int>)null));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_IsPDXSDKEnabled = new ValueBinding<bool>("paradox", "pdxSDKEnabled", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		m_PdxPlatform = PlatformManager.instance.GetPSI<PdxSdkPlatform>("PdxSdk");
		PlatformManager.instance.onPlatformRegistered += (PlatformRegisteredHandler)delegate(IPlatformServiceIntegration psi)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			PdxSdkPlatform val = (PdxSdkPlatform)(object)((psi is PdxSdkPlatform) ? psi : null);
			if (val != null)
			{
				m_PdxPlatform = val;
				m_PdxPlatform.onLoggedIn += new OnLoggedInEventHandler(OnUserLoggedIn);
				m_PdxPlatform.onLoggedOut += new OnLoggedOutEventHandler(OnUserLoggedOut);
				m_PdxPlatform.onAccountLinkChanged += new OnAccountLinkChangeEventHandler(OnAccountLinkChanged);
				m_PdxPlatform.onLegalDocumentStatusChanged += new OnLegalDocumentStatusChangedEventHandler(OnLegalDocumentStatusChanged);
				m_PdxPlatform.onStatusChanged += new OnStatusChangedEventHandler(OnStatusChanged);
			}
		};
		PlatformManager.instance.onConnectivityStatusChanged += new OnConnectivityStatusChanged(OnInternetConnectionStatusChanged);
	}

	public void OnPSModsUIOpened(Action onContinue)
	{
		m_ActiveDialogsBinding.Push((ParadoxDialog)new MultiOptionDialog("Menu.PDX_MODS", "Paradox.PS_MODS_DISCLAIMER", new MultiOptionDialog.Option
		{
			m_Id = "Common.OK",
			m_OnSelect = onContinue
		}));
	}

	public void OnPSModsUIClosed(Action onKeepMods, Action onDisableMods, Action onBack)
	{
		m_ActiveDialogsBinding.Push((ParadoxDialog)new MultiOptionDialog("Menu.PDX_MODS", "Paradox.PS_MODS_EXIT_DISCLAIMER", new MultiOptionDialog.Option
		{
			m_Id = "Paradox.PS_MODS_EXIT_KEEP_MODS",
			m_OnSelect = onKeepMods
		}, new MultiOptionDialog.Option
		{
			m_Id = "Paradox.PS_MODS_EXIT_DISABLE_MODS",
			m_OnSelect = onDisableMods
		}, new MultiOptionDialog.Option
		{
			m_Id = "Paradox.PS_MODS_EXIT_GO_BACK",
			m_OnSelect = onBack
		}));
	}

	public void PushDialog(ParadoxDialog dialog)
	{
		m_ActiveDialogsBinding.Push(dialog);
	}

	private void OnOptionSelected(int index)
	{
		if (m_ActiveDialogsBinding.Peek() is MultiOptionDialog multiOptionDialog)
		{
			m_ActiveDialogsBinding.Pop();
			multiOptionDialog.m_Options[index].m_OnSelect?.Invoke();
		}
	}

	private void OnAccountLinkChanged(AccountLinkState state, AccountLinkProvider provider)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		m_AccountLinkProviderBinding.Update(provider);
		m_AccountLinkStateBinding.Update((int)state);
	}

	private async void Logout()
	{
		await m_PdxPlatform.Logout();
	}

	private List<string> GetCountryCodes()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>(Enum.GetNames(typeof(Country)));
		list.Remove(((object)(Country)249/*cast due to .constrained prefix*/).ToString());
		return list;
	}

	private void OnInternetConnectionStatusChanged(bool connected)
	{
		m_HasInternetConnection.Update(connected);
	}

	private void OnStatusChanged(IPlatformServiceIntegration psi)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected I4, but got Unknown
		if ((object)psi == m_PdxPlatform)
		{
			m_IsPDXSDKEnabled.Update(m_PdxPlatform.isInitialized);
			m_AccountLinkProviderBinding.Update(m_PdxPlatform.accountLinkProvider);
			m_AccountLinkStateBinding.Update((int)m_PdxPlatform.accountLinkState);
		}
	}

	private async void OnUserLoggedIn(string firstName, string lastName, string email, AccountLinkState accountLinkState, bool firstTime)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		m_LoggedInBinding.Update(true);
		m_AccountLinkStateBinding.Update((int)accountLinkState);
		m_EmailBinding.Update(email);
		ModCreator val = await m_PdxPlatform.GetCreatorProfile();
		if (val != null)
		{
			m_UserNameBinding.Update(val.Username);
			m_AvatarBinding.Update(val.Avatar.Url);
		}
	}

	private void OnUserLoggedOut(string id)
	{
		m_UserNameBinding.Update((string)null);
		m_EmailBinding.Update((string)null);
		m_AvatarBinding.Update((string)null);
		m_LoggedInBinding.Update(false);
	}

	private void OnLegalDocumentStatusChanged(LegalDocument doc, int remainingCount)
	{
		if (m_ActiveDialogsBinding.Peek() is LegalDocumentDialog)
		{
			m_ActiveDialogsBinding.Pop();
		}
		if (doc != null)
		{
			m_ActiveDialogsBinding.Push((ParadoxDialog)new LegalDocumentDialog(doc));
		}
	}

	private void CloseActiveDialog()
	{
		if (!m_RequestActiveBinding.value && !(m_ActiveDialogsBinding.Peek() is LegalDocumentDialog { agreementRequired: not false }))
		{
			m_ActiveDialogsBinding.Pop();
			if (m_ActiveDialogsBinding.count == 0)
			{
				PlatformManager.instance.EnableSharing();
			}
		}
	}

	public void ShowLoginForm()
	{
		if (Connectivity.hasConnectivity)
		{
			PlatformManager.instance.DisableSharing();
			m_ActiveDialogsBinding.ClearAndPush((ParadoxDialog)new LoginDialog());
		}
		else
		{
			m_ActiveDialogsBinding.Push((ParadoxDialog)new ErrorDialog("Failed to connect", "Please check your internet connection"));
		}
	}

	private async void SubmitLoginForm(LoginFormData data)
	{
		if (m_RequestActiveBinding.value)
		{
			return;
		}
		RequestReport val = await RunForegroundRequest(m_PdxPlatform.Login(data.email, data.password, CancellationToken.None));
		if (val == null)
		{
			m_ActiveDialogsBinding.Clear();
			if ((int)m_PdxPlatform.accountLinkProvider != 2 && (int)m_PdxPlatform.accountLinkState == 1)
			{
				LinkAccount();
			}
		}
		else
		{
			m_ActiveDialogsBinding.Push((ParadoxDialog)new ErrorDialog(val.messageId, val.message));
		}
	}

	private async void SubmitPasswordReset(string email)
	{
		if (!m_RequestActiveBinding.value)
		{
			RequestReport val = await RunForegroundRequest(m_PdxPlatform.ResetPassword(email));
			if (val == null)
			{
				m_ActiveDialogsBinding.Push((ParadoxDialog)new ConfirmationDialog(null, null, "Paradox.PASSWORD_RESET_CONFIRMATION_TEXT", new Dictionary<string, string> { { "EMAIL", email } }));
			}
			else
			{
				m_ActiveDialogsBinding.Push((ParadoxDialog)new ErrorDialog(val.messageId, val.message));
			}
		}
	}

	private void ShowRegistrationForm()
	{
		m_ActiveDialogsBinding.ClearAndPush((ParadoxDialog)new RegistrationDialog());
	}

	private async void ShowLink(string link)
	{
		if (link == kTermsOfUse)
		{
			LegalDocument val = await RunForegroundRequest(m_PdxPlatform.ShowTermsOfUse());
			if (val != null)
			{
				m_ActiveDialogsBinding.Push((ParadoxDialog)new LegalDocumentDialog(val, agreementRequired: false));
			}
		}
		else if (link == kPrivacyPolicy)
		{
			LegalDocument val2 = await RunForegroundRequest(m_PdxPlatform.ShowPrivacyPolicy());
			if (val2 != null)
			{
				m_ActiveDialogsBinding.Push((ParadoxDialog)new LegalDocumentDialog(val2, agreementRequired: false));
			}
		}
		else
		{
			Application.OpenURL(link);
		}
	}

	private async void SubmitRegistrationForm(RegistrationFormData data)
	{
		if (m_RequestActiveBinding.value)
		{
			return;
		}
		if (Enum.TryParse<Country>(data.country, out Country result) && DateTime.TryParseExact(data.dateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result2))
		{
			RequestReport val = await RunForegroundRequest(m_PdxPlatform.CreateParadoxAccount(data.email, data.password, (Language)36, result, result2, data.marketingPermission));
			if (val == null)
			{
				m_ActiveDialogsBinding.Clear();
				if ((int)m_PdxPlatform.accountLinkProvider != 2 && (int)m_PdxPlatform.accountLinkState == 1)
				{
					LinkAccount();
				}
				m_ActiveDialogsBinding.Push((ParadoxDialog)new ConfirmationDialog(null, "Paradox.REGISTRATION_CONFIRMATION_TITLE", "Paradox.REGISTRATION_CONFIRMATION_TEXT", null));
			}
			else
			{
				m_ActiveDialogsBinding.Push((ParadoxDialog)new ErrorDialog(val.messageId, val.message));
			}
		}
		else
		{
			m_ActiveDialogsBinding.Push((ParadoxDialog)new ErrorDialog(null, "Internal error: Invalid Country Code string or Invalid date string"));
		}
	}

	private async void ConfirmAccountLink()
	{
		if (m_RequestActiveBinding.value)
		{
			return;
		}
		if ((int)m_PdxPlatform.AccountLinkMismatch == 0)
		{
			RequestReport val = await RunForegroundRequest(m_PdxPlatform.LinkAccount());
			if (val == null)
			{
				m_AccountLinkStateBinding.Update(2);
				m_ActiveDialogsBinding.ClearAndPush((ParadoxDialog)new ConfirmationDialog(GetAccountLinkProviderIcon(), "Paradox.ACCOUNT_LINK_PROMPT_TITLE", $"Paradox.ACCOUNT_LINK_CONFIRMATION_TEXT[{m_PdxPlatform.accountLinkProvider:G}]", null));
			}
			else
			{
				m_ActiveDialogsBinding.Push((ParadoxDialog)new ErrorDialog(val.messageId, val.message));
			}
		}
		else
		{
			AccountLinkMismatch accountLinkMismatch = m_PdxPlatform.AccountLinkMismatch;
			string messageId = (accountLinkMismatch - 1) switch
			{
				0 => $"Paradox.PDX_ACCOUNT_LINK_OVERWRITE_PROMPT_TEXT[{m_PdxPlatform.accountLinkProvider:G}]", 
				1 => $"Paradox.PLATFORM_ACCOUNT_LINK_OVERWRITE_PROMPT_TEXT[{m_PdxPlatform.accountLinkProvider:G}]", 
				2 => $"Paradox.PDX_PLATFORM_ACCOUNT_LINK_OVERWRITE_PROMPT_TEXT[{m_PdxPlatform.accountLinkProvider:G}]", 
				_ => null, 
			};
			m_ActiveDialogsBinding.Push((ParadoxDialog)new AccountLinkOverwriteDialog(GetAccountLinkProviderIcon(), messageId));
		}
	}

	private async void ConfirmAccountLinkOverwrite()
	{
		if (!m_RequestActiveBinding.value)
		{
			RequestReport val = await RunForegroundRequest(m_PdxPlatform.OverwriteAccountLinks());
			if (val == null)
			{
				m_AccountLinkStateBinding.Update(2);
				m_ActiveDialogsBinding.ClearAndPush((ParadoxDialog)new ConfirmationDialog(GetAccountLinkProviderIcon(), "Paradox.ACCOUNT_LINK_PROMPT_TITLE", $"Paradox.ACCOUNT_LINK_CONFIRMATION_TEXT[{m_PdxPlatform.accountLinkProvider:G}]", null));
			}
			else
			{
				m_ActiveDialogsBinding.Push((ParadoxDialog)new ErrorDialog(val.messageId, val.message));
			}
		}
	}

	private void LinkAccount()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_ActiveDialogsBinding.Push((ParadoxDialog)new AccountLinkDialog(GetAccountLinkProviderIcon(), $"Paradox.ACCOUNT_LINK_PROMPT_TEXT[{m_PdxPlatform.accountLinkProvider:G}]"));
	}

	private async void UnlinkAccount()
	{
		if (!m_RequestActiveBinding.value)
		{
			RequestReport val = await RunForegroundRequest(m_PdxPlatform.UnlinkThirdPartyAccount());
			if (val == null)
			{
				m_AccountLinkStateBinding.Update(1);
			}
			else
			{
				m_ActiveDialogsBinding.Push((ParadoxDialog)new ErrorDialog(val.messageId, val.message));
			}
		}
	}

	private void ShowTermsOfUse()
	{
		ShowLink(kTermsOfUse);
	}

	private void ShowPrivacyPolicy()
	{
		ShowLink(kPrivacyPolicy);
	}

	private string GetAccountLinkProviderIcon()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return $"Media/Menu/Platforms/{m_PdxPlatform.accountLinkProvider:G}.svg";
	}

	private async void MarkLegalDocumentAsViewed()
	{
		if (m_ActiveDialogsBinding.Peek() is LegalDocumentDialog { document: var document })
		{
			await RunForegroundRequest(m_PdxPlatform.MarkLegalDocumentAsViewed(document));
		}
	}

	private async Task<T> RunForegroundRequest<T>(Task<T> task)
	{
		m_RequestActiveBinding.Update(true);
		try
		{
			return await task;
		}
		finally
		{
			m_RequestActiveBinding.Update(false);
		}
	}
}
