using cohtml.Net;
using Colossal.PSI.Common;
using Colossal.UI;
using Game.Input;
using Game.SceneFlow;

namespace Game.PSI;

public class VirtualKeyboard : TextInputHandler
{
	public VirtualKeyboard()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		PlatformManager.instance.onInputDismissed += (InputDismissedEventHandler)delegate(IVirtualKeyboardSupport psi, string text)
		{
			if (!psi.passThroughVKeyboard)
			{
				((TextInputHandler)this).RefreshText(text);
			}
		};
	}

	private string GetVkTitle()
	{
		string attribute = ((INodeProxy)((TextInputHandler)this).proxy).GetAttribute("vk-title");
		if (!string.IsNullOrEmpty(attribute))
		{
			return attribute;
		}
		return "Input";
	}

	private string GetVkDescription()
	{
		string attribute = ((INodeProxy)((TextInputHandler)this).proxy).GetAttribute("vk-description");
		if (!string.IsNullOrEmpty(attribute))
		{
			return attribute;
		}
		return string.Empty;
	}

	private InputType TextToInputType(string text)
	{
		return (InputType)(text switch
		{
			"text" => 0, 
			"password" => 1, 
			"email" => 2, 
			_ => 3, 
		});
	}

	private InputType GetVkType()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		string attribute = ((INodeProxy)((TextInputHandler)this).proxy).GetAttribute("vk-type");
		return TextToInputType(attribute);
	}

	protected override void OnFocusCallback(string str)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (InputManager.instance.activeControlScheme == InputManager.ControlScheme.Gamepad)
		{
			bool flag = PlatformManager.instance.ShowVirtualKeyboard(GetVkType(), GetVkTitle(), GetVkDescription(), 100, str);
			GameManager.UIInputSystem.emulateBackspaceOnTextEvent = flag && PlatformManager.instance.passThroughVKeyboard;
		}
	}

	protected override void OnBlurCallback()
	{
		GameManager.UIInputSystem.emulateBackspaceOnTextEvent = false;
		PlatformManager.instance.DismissVirtualKeyboard();
	}
}
