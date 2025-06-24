using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Colossal.PSI.Common;
using Game.Input;
using UnityEngine.InputSystem;

namespace Game.SceneFlow;

public interface IScreenState
{
	static Task WaitForWaitingState(InputAction inputAction)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		if ((int)inputAction.phase == 1)
		{
			return Task.CompletedTask;
		}
		TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
		System.Timers.Timer timer = new System.Timers.Timer(33.333333333333336);
		timer.Elapsed += delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)inputAction.phase == 1)
			{
				taskCompletionSource.SetResult(result: true);
				timer.Stop();
				timer.Dispose();
			}
		};
		timer.AutoReset = true;
		timer.Start();
		return taskCompletionSource.Task;
	}

	static async Task<(bool ok, InputDevice device)> WaitForInput(InputAction inputContinue, InputAction inputCancel, Action cancel, CancellationToken token)
	{
		TaskCompletionSource<(bool ok, InputDevice device)> performed = new TaskCompletionSource<(bool, InputDevice)>();
		using (token.Register(delegate
		{
			performed.TrySetCanceled();
		}))
		{
			inputContinue.performed += Handler;
			if (inputCancel != null)
			{
				inputCancel.performed += Handler;
			}
			if (cancel != null)
			{
				cancel = (Action)Delegate.Combine(cancel, new Action(CancelHandler));
			}
			GameManager.instance.userInterface.inputHintBindings.onInputHintPerformed += InputHintPerformedHandler;
			try
			{
				return await performed.Task;
			}
			finally
			{
				inputContinue.performed -= Handler;
				inputContinue.Reset();
				if (inputCancel != null)
				{
					inputCancel.performed -= Handler;
					inputCancel.Reset();
				}
				if (cancel != null)
				{
					cancel = (Action)Delegate.Remove(cancel, new Action(CancelHandler));
				}
				GameManager.instance.userInterface.inputHintBindings.onInputHintPerformed -= InputHintPerformedHandler;
			}
		}
		void CancelHandler()
		{
			performed.TrySetCanceled();
		}
		void Handler(CallbackContext c)
		{
			TaskCompletionSource<(bool ok, InputDevice device)> taskCompletionSource = performed;
			bool item = inputContinue == ((CallbackContext)(ref c)).action;
			InputControl activeControl = ((CallbackContext)(ref c)).action.activeControl;
			taskCompletionSource.TrySetResult((item, (activeControl != null) ? activeControl.device : null));
		}
		void InputHintPerformedHandler(ProxyAction action)
		{
			if (action.sourceAction == inputContinue)
			{
				performed.TrySetResult((true, (InputDevice)(object)Mouse.current));
			}
			else if (action.sourceAction == inputCancel)
			{
				performed.TrySetCanceled();
			}
		}
	}

	static async Task<object> WaitForDevice(Action cancel, CancellationToken token)
	{
		TaskCompletionSource<object> devicePaired = new TaskCompletionSource<object>();
		using (token.Register(delegate
		{
			devicePaired.TrySetCanceled();
		}))
		{
			InputManager.instance.EventDevicePaired += Handler;
			if (cancel != null)
			{
				cancel = (Action)Delegate.Combine(cancel, new Action(CancelHandler));
			}
			try
			{
				return await devicePaired.Task;
			}
			finally
			{
				InputManager.instance.EventDevicePaired -= Handler;
				if (cancel != null)
				{
					cancel = (Action)Delegate.Remove(cancel, new Action(CancelHandler));
				}
			}
		}
		void CancelHandler()
		{
			devicePaired.TrySetCanceled();
		}
		void Handler()
		{
			devicePaired.TrySetResult(null);
		}
	}

	static async Task<UserChangedFlags> WaitForUser(Action cancel, CancellationToken token)
	{
		TaskCompletionSource<UserChangedFlags> userSignedBackIn = new TaskCompletionSource<UserChangedFlags>();
		using (token.Register(delegate
		{
			userSignedBackIn.TrySetCanceled();
		}))
		{
			PlatformManager.instance.onUserUpdated += new OnUserUpdatedEventHandler(Handler);
			if (cancel != null)
			{
				cancel = (Action)Delegate.Combine(cancel, new Action(CancelHandler));
			}
			try
			{
				return await userSignedBackIn.Task;
			}
			finally
			{
				PlatformManager.instance.onUserUpdated -= new OnUserUpdatedEventHandler(Handler);
				if (cancel != null)
				{
					cancel = (Action)Delegate.Remove(cancel, new Action(CancelHandler));
				}
			}
		}
		void CancelHandler()
		{
			userSignedBackIn.TrySetCanceled();
		}
		void Handler(IPlatformServiceIntegration psi, UserChangedFlags flags)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (PlatformManager.instance.IsPrincipalUserIntegration(psi) && ((Enum)flags).HasFlag((Enum)(object)(UserChangedFlags)32))
			{
				userSignedBackIn.TrySetResult(flags);
			}
		}
	}

	Task Execute(GameManager manager, CancellationToken token);
}
