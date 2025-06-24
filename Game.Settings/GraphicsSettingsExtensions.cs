using System;
using UnityEngine;

namespace Game.Settings;

public static class GraphicsSettingsExtensions
{
	public static CursorLockMode ToUnityCursorMode(this GraphicsSettings.CursorMode mode)
	{
		return (CursorLockMode)(mode switch
		{
			GraphicsSettings.CursorMode.Free => 0, 
			GraphicsSettings.CursorMode.ConfinedToWindow => 2, 
			_ => throw new ArgumentException($"Unsupported cursor mode: {mode}", "mode"), 
		});
	}
}
