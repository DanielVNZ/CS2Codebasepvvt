using System;
using System.Collections.Generic;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.SceneFlow;

namespace Game.UI.Menu;

public class UserBindings : CompositeBinding, IDisposable
{
	private const string kGroup = "user";

	private ValueBinding<bool> m_SwitchPromptVisible;

	private ValueBinding<string> m_AvatarBinding;

	private ValueBinding<string> m_UserIDBinding;

	private ValueBinding<string> m_SwitchUserHintOverload;

	private static int s_AvatarVersion;

	public UserBindings()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		GameManager.instance.onGameLoadingComplete += OnMainMenuReached;
		bool flag = !GameManager.instance.configuration.disableUserSection && (PlatformManager.instance.supportsUserSwitching || PlatformManager.instance.supportsUserSection);
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_SwitchPromptVisible = new ValueBinding<bool>("user", "switchPromptVisible", flag, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		string text = string.Format("{0}/UserAvatar#{1}?size={2}", "useravatar://", s_AvatarVersion++, (object)(AvatarSize)0);
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_AvatarBinding = new ValueBinding<string>("user", "avatar", text, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_UserIDBinding = new ValueBinding<string>("user", "userID", PlatformManager.instance.userName, (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_SwitchUserHintOverload = new ValueBinding<string>("user", "switchUserHintOverload", getSwitchUserHintOverload(), (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null)));
		((CompositeBinding)this).AddBinding((IBinding)new TriggerBinding("user", "switchUser", (Action)SwitchUser));
		PlatformManager.instance.onStatusChanged += (OnStatusChangedEventHandler)delegate(IPlatformServiceIntegration psi)
		{
			if (PlatformManager.instance.IsPrincipalOverlayIntegration(psi))
			{
				m_SwitchPromptVisible.Update(!GameManager.instance.configuration.disableUserSection && (PlatformManager.instance.supportsUserSwitching || PlatformManager.instance.supportsUserSection));
			}
		};
		PlatformManager.instance.onUserUpdated += (OnUserUpdatedEventHandler)delegate(IUserSupport psi, UserChangedFlags flags)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (PlatformManager.instance.IsPrincipalUserIntegration((IPlatformServiceIntegration)(object)psi))
			{
				if (UserChangedHelper.HasChanged(flags, (UserChangedFlags)1))
				{
					m_UserIDBinding.Update(PlatformManager.instance.userName);
				}
				if (UserChangedHelper.HasChanged(flags, (UserChangedFlags)4))
				{
					m_AvatarBinding.Update(string.Format("{0}/UserAvatar#{1}?size={2}", "useravatar://", s_AvatarVersion++, (object)(AvatarSize)0));
				}
			}
		};
	}

	private void OnMainMenuReached(Purpose purpose, GameMode mode)
	{
		if (mode == GameMode.MainMenu)
		{
			m_SwitchPromptVisible.Update(!GameManager.instance.configuration.disableUserSection && (PlatformManager.instance.supportsUserSwitching || PlatformManager.instance.supportsUserSection));
		}
	}

	public void Dispose()
	{
		GameManager.instance.onGameLoadingComplete -= OnMainMenuReached;
	}

	public string getSwitchUserHintOverload()
	{
		if (PlatformManager.instance.supportsUserSwitching)
		{
			return null;
		}
		return "Steam Overlay";
	}

	private void SwitchUser()
	{
		if (m_SwitchPromptVisible.value)
		{
			PlatformManager instance = PlatformManager.instance;
			if (instance.supportsUserSwitching)
			{
				GameManager.instance.SetScreenActive<SwitchUserScreen>();
			}
			else if (instance.supportsUserSection)
			{
				instance.ShowOverlay((Page)2, (string)null);
			}
		}
	}
}
