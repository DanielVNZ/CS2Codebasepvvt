using System;
using UnityEngine.InputSystem;

namespace Game.Input;

public interface IProxyAction
{
	bool enabled { get; set; }

	event Action<ProxyAction, InputActionPhase> onInteraction;

	bool WasPressedThisFrame();

	bool WasReleasedThisFrame();

	bool IsPressed();

	bool IsInProgress();

	float GetMagnitude();

	T ReadValue<T>() where T : struct;
}
