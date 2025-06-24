using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input;

public class UIButtonInteraction : IInputInteraction
{
	public float repeatDelay = 0.5f;

	public float repeatRate = 0.1f;

	public float pressPoint;

	private float pressPointOrDefault
	{
		get
		{
			if (!((double)pressPoint > 0.0))
			{
				return InputSystem.settings.defaultButtonPressPoint;
			}
			return pressPoint;
		}
	}

	public void Process(ref InputInteractionContext context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		InputActionPhase phase = ((InputInteractionContext)(ref context)).phase;
		if ((int)phase != 1)
		{
			if ((int)phase == 2)
			{
				if (((InputInteractionContext)(ref context)).timerHasExpired)
				{
					((InputInteractionContext)(ref context)).PerformedAndStayStarted();
					((InputInteractionContext)(ref context)).SetTimeout(repeatRate);
				}
				else if (!((InputInteractionContext)(ref context)).ControlIsActuated(pressPointOrDefault))
				{
					((InputInteractionContext)(ref context)).Canceled();
				}
			}
		}
		else if (((InputInteractionContext)(ref context)).ControlIsActuated(pressPointOrDefault))
		{
			((InputInteractionContext)(ref context)).Started();
			((InputInteractionContext)(ref context)).PerformedAndStayStarted();
			((InputInteractionContext)(ref context)).SetTimeout(repeatDelay);
		}
	}

	public void Reset()
	{
	}

	static UIButtonInteraction()
	{
		InputSystem.RegisterInteraction<UIButtonInteraction>((string)null);
	}

	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void Init()
	{
	}
}
