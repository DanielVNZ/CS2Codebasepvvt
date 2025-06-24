using System.Collections.Generic;
using Colossal.PSI.Common;
using Game.Settings;
using UnityEngine;

namespace Game.PSI.Internal;

public static class Helpers
{
	public enum json_displaymode
	{
		fullscreen,
		windowed,
		borderless_window
	}

	public enum json_gameplay_mode
	{
		sandbox,
		editor
	}

	private static readonly IReadOnlyDictionary<SystemLanguage, string> s_SystemLanguageToISO = new Dictionary<SystemLanguage, string>
	{
		{
			(SystemLanguage)0,
			"af"
		},
		{
			(SystemLanguage)1,
			"ar"
		},
		{
			(SystemLanguage)2,
			"eu"
		},
		{
			(SystemLanguage)3,
			"be"
		},
		{
			(SystemLanguage)4,
			"bg"
		},
		{
			(SystemLanguage)5,
			"ca"
		},
		{
			(SystemLanguage)6,
			"zh"
		},
		{
			(SystemLanguage)7,
			"cs"
		},
		{
			(SystemLanguage)8,
			"da"
		},
		{
			(SystemLanguage)9,
			"nl"
		},
		{
			(SystemLanguage)10,
			"en"
		},
		{
			(SystemLanguage)11,
			"et"
		},
		{
			(SystemLanguage)12,
			"fo"
		},
		{
			(SystemLanguage)13,
			"fi"
		},
		{
			(SystemLanguage)14,
			"fr"
		},
		{
			(SystemLanguage)15,
			"de"
		},
		{
			(SystemLanguage)16,
			"el"
		},
		{
			(SystemLanguage)17,
			"he"
		},
		{
			(SystemLanguage)42,
			"hi"
		},
		{
			(SystemLanguage)18,
			"hu"
		},
		{
			(SystemLanguage)19,
			"is"
		},
		{
			(SystemLanguage)20,
			"id"
		},
		{
			(SystemLanguage)21,
			"it"
		},
		{
			(SystemLanguage)22,
			"ja"
		},
		{
			(SystemLanguage)23,
			"ko"
		},
		{
			(SystemLanguage)24,
			"lv"
		},
		{
			(SystemLanguage)25,
			"lt"
		},
		{
			(SystemLanguage)26,
			"no"
		},
		{
			(SystemLanguage)27,
			"pl"
		},
		{
			(SystemLanguage)28,
			"pt"
		},
		{
			(SystemLanguage)29,
			"ro"
		},
		{
			(SystemLanguage)30,
			"ru"
		},
		{
			(SystemLanguage)31,
			"sh"
		},
		{
			(SystemLanguage)32,
			"sk"
		},
		{
			(SystemLanguage)33,
			"sl"
		},
		{
			(SystemLanguage)34,
			"es"
		},
		{
			(SystemLanguage)35,
			"sv"
		},
		{
			(SystemLanguage)36,
			"th"
		},
		{
			(SystemLanguage)37,
			"tr"
		},
		{
			(SystemLanguage)38,
			"uk"
		},
		{
			(SystemLanguage)39,
			"vi"
		},
		{
			(SystemLanguage)40,
			"zh-HANS"
		},
		{
			(SystemLanguage)41,
			"zh-HANT"
		}
	};

	public static string GetSystemLanguage()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (s_SystemLanguageToISO.TryGetValue(Application.systemLanguage, out var value))
		{
			return value;
		}
		return string.Empty;
	}

	public static json_displaymode ToTelemetry(this DisplayMode mode)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return mode switch
		{
			DisplayMode.Fullscreen => json_displaymode.fullscreen, 
			DisplayMode.Window => json_displaymode.windowed, 
			DisplayMode.FullscreenWindow => json_displaymode.borderless_window, 
			_ => throw new TelemetryException($"Invalid display mode {mode}"), 
		};
	}

	public static string ToTelemetry(this ScreenResolution resolution)
	{
		return $"{resolution.width}x{resolution.height}";
	}

	public static int AsInt(this bool value)
	{
		if (!value)
		{
			return 0;
		}
		return 1;
	}

	public static json_gameplay_mode ToTelemetry(this GameMode gameMode)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return gameMode switch
		{
			GameMode.Game => json_gameplay_mode.sandbox, 
			GameMode.Editor => json_gameplay_mode.editor, 
			_ => throw new TelemetryException($"Invalid game mode {gameMode}"), 
		};
	}
}
