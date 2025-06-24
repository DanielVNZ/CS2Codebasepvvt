using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Colossal.UI.Binding;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Game.Input;

[DebuggerDisplay("{name} ({displayName})")]
public struct ControlPath : IJsonWritable
{
	private static Dictionary<string, bool> m_IsLatinLayout = new Dictionary<string, bool>();

	public string name;

	public InputManager.DeviceType device;

	public string displayName;

	public static ControlPath Get(string path)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(path))
		{
			return new ControlPath
			{
				name = string.Empty,
				device = InputManager.DeviceType.None,
				displayName = string.Empty
			};
		}
		ParsedPathComponent[] source = InputControlPath.Parse(path).ToArray();
		string text = string.Join("/", from p in source
			where string.IsNullOrEmpty(((ParsedPathComponent)(ref p)).layout)
			select ((ParsedPathComponent)(ref p)).name);
		ParsedPathComponent val = source.FirstOrDefault((ParsedPathComponent p) => !string.IsNullOrEmpty(((ParsedPathComponent)(ref p)).layout));
		string layout = ((ParsedPathComponent)(ref val)).layout;
		return new ControlPath
		{
			name = text,
			device = layout.ToDeviceType(),
			displayName = ((text.Length == 1 && char.IsLetter(text[0])) ? text.ToUpper() : text)
		};
	}

	public static bool IsLatinLikeLayout(Keyboard keyboard)
	{
		if (!m_IsLatinLayout.TryGetValue(keyboard.keyboardLayout, out var value))
		{
			value = Enumerable.Range(15, 26).All((int k) => IsLatinOrPunctuation(((InputControl)keyboard[(Key)k]).displayName));
			m_IsLatinLayout[keyboard.keyboardLayout] = value;
		}
		return value;
	}

	public static bool NeedLocalName(Keyboard keyboard, KeyControl control)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected I4, but got Unknown
		Key keyCode = control.keyCode;
		if (keyCode - 1 > 2)
		{
			switch (keyCode - 41)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 30:
			case 43:
			case 44:
			case 45:
			case 46:
			case 47:
			case 48:
			case 49:
			case 50:
			case 51:
			case 52:
				break;
			case 65:
			case 66:
			case 67:
			case 68:
			case 69:
				return true;
			default:
				return IsLatinLikeLayout(keyboard);
			}
		}
		return false;
	}

	private static bool IsLatinOrPunctuation(string displayName)
	{
		if (!string.IsNullOrEmpty(displayName))
		{
			if (!IsLatinLater(displayName))
			{
				return char.IsPunctuation(displayName[0]);
			}
			return true;
		}
		return false;
	}

	private static bool IsLatinLater(string displayName)
	{
		if (!string.IsNullOrEmpty(displayName) && char.IsLetterOrDigit(displayName[0]))
		{
			return displayName[0] <= 'ÿ';
		}
		return false;
	}

	public void Write(IJsonWriter writer)
	{
		writer.TypeBegin(typeof(ControlPath).FullName);
		writer.PropertyName("name");
		writer.Write(name);
		writer.PropertyName("device");
		writer.Write(device.ToString());
		writer.PropertyName("displayName");
		writer.Write(displayName);
		writer.TypeEnd();
	}

	public static string ToHumanReadablePath(string path, HumanReadableStringOptions options = (HumanReadableStringOptions)2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return InputControlPath.ToHumanReadableString(path, options, (InputControl)null);
	}
}
