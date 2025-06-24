using System;
using System.Threading;
using System.Threading.Tasks;
using Colossal.PSI.Common;
using Game.Input;
using UnityEngine.InputSystem;

namespace Game.SceneFlow;

public class LoggedOutScreen : FullScreenOverlay
{
	protected override OverlayScreen overlayScreen => OverlayScreen.UserLoggedOut;

	protected override string actionA => "AnyKey";

	protected override string continueDisplayProperty => "Continue";

	public override async Task Execute(GameManager manager, CancellationToken token)
	{
		using (EnabledActionScoped continueAction = new EnabledActionScoped(manager, "Engagement", actionA, HandleScreenChange, continueDisplayProperty, continueDisplayPriority))
		{
			using (InputManager.instance.CreateOverlayBarrier("LoggedOutScreen"))
			{
				OverlayBindings overlayBindings = manager.userInterface.overlayBindings;
				using (overlayBindings.ActivateScreenScoped(OverlayScreen.UserLoggedOut))
				{
					while (!m_Done)
					{
						Task<(bool ok, InputDevice device)> input = IScreenState.WaitForInput(continueAction, null, m_CompletedEvent, token);
						Task<UserChangedFlags> user = IScreenState.WaitForUser(m_CompletedEvent, token);
						await Task.WhenAny(input, user);
						m_CompletedEvent?.Invoke();
						if (((Task)input).IsCompletedSuccessfully)
						{
							SignInFlags val = await PlatformManager.instance.SignIn((SignInOptions)0, (Action<Task>)UserChangingCallback);
							m_Done = ((Enum)val).HasFlag((Enum)(object)(SignInFlags)2);
							if (((Enum)val).HasFlag((Enum)(object)(SignInFlags)4) && manager.gameMode.IsGameOrEditor())
							{
								manager.MainMenu();
							}
						}
						else if (((Task)user).IsCompletedSuccessfully)
						{
							m_Done = true;
						}
						else
						{
							m_Done = true;
						}
					}
				}
			}
		}
		void UserChangingCallback(Task signInTask)
		{
			new WaitScreen().Execute(manager, token, signInTask);
		}
	}
}
