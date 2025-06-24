using System;
using System.Threading;
using System.Threading.Tasks;
using Colossal.PSI.Common;
using Game.Input;
using UnityEngine.InputSystem;

namespace Game.SceneFlow;

public class ControllerPairingScreen : FullScreenOverlay
{
	protected override OverlayScreen overlayScreen => OverlayScreen.ControllerPairingChanged;

	protected override string continueDisplayProperty => "Switch User";

	protected override string cancelDisplayProperty => "Cancel";

	protected override int cancelDisplayPriority => 30;

	public override async Task Execute(GameManager manager, CancellationToken token)
	{
		using (EnabledActionScoped continueAction = new EnabledActionScoped(manager, "Engagement", actionA, HandleScreenChange, continueDisplayProperty, continueDisplayPriority))
		{
			using EnabledActionScoped cancelAction = new EnabledActionScoped(manager, "Engagement", actionB, HandleScreenChange, cancelDisplayProperty, cancelDisplayPriority);
			using (InputManager.instance.CreateOverlayBarrier("ControllerPairingScreen"))
			{
				OverlayBindings overlayBindings = manager.userInterface.overlayBindings;
				using (overlayBindings.ActivateScreenScoped(overlayScreen))
				{
					while (!m_Done)
					{
						Task<(bool ok, InputDevice device)> input = IScreenState.WaitForInput(continueAction, cancelAction, m_CompletedEvent, token);
						Task<object> device = IScreenState.WaitForDevice(m_CompletedEvent, token);
						await Task.WhenAny(input, device);
						m_CompletedEvent?.Invoke();
						if (((Task)input).IsCompletedSuccessfully)
						{
							if (input.Result.ok)
							{
								SignInFlags val = await PlatformManager.instance.SignIn((SignInOptions)0, (Action<Task>)UserChangingCallback);
								m_Done = ((Enum)val).HasFlag((Enum)(object)(SignInFlags)2);
								if (((Enum)val).HasFlag((Enum)(object)(SignInFlags)4) && manager.gameMode.IsGameOrEditor())
								{
									manager.MainMenu();
								}
							}
							else
							{
								m_Done = await PlatformManager.instance.AssociateDevice(input.Result.device);
							}
						}
						else if (((Task)device).IsCompletedSuccessfully)
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
