using System;
using System.Threading;
using System.Threading.Tasks;
using Colossal.PSI.Common;
using Game.Input;
using UnityEngine.InputSystem;

namespace Game.SceneFlow;

public class EngagementScreen : FullScreenOverlay
{
	protected override OverlayScreen overlayScreen => OverlayScreen.Engagement;

	protected override string actionA => "AnyKey";

	protected override string continueDisplayProperty => "Start Game";

	public override async Task Execute(GameManager manager, CancellationToken token)
	{
		using (EnabledActionScoped continueAction = new EnabledActionScoped(manager, "Engagement", actionA, HandleScreenChange, continueDisplayProperty, continueDisplayPriority))
		{
			using (InputManager.instance.CreateOverlayBarrier("EngagementScreen"))
			{
				OverlayBindings overlayBindings = manager.userInterface.overlayBindings;
				using (overlayBindings.ActivateScreenScoped(overlayScreen))
				{
					while (!m_Done)
					{
						Task<(bool ok, InputDevice device)> input = IScreenState.WaitForInput(continueAction, null, m_CompletedEvent, token);
						await input;
						if (((Task)input).IsCompletedSuccessfully)
						{
							if (!PlatformManager.instance.isUserSignedIn)
							{
								m_Done = ((Enum)(await PlatformManager.instance.SignIn((SignInOptions)3, (Action<Task>)UserChangingCallback))).HasFlag((Enum)(object)(SignInFlags)2);
							}
							else
							{
								m_Done = await PlatformManager.instance.AssociateDevice(input.Result.device);
							}
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
