using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cohtml.Net;
using Colossal.Logging;
using Colossal.PSI.Common;
using Colossal.PSI.PdxSdk;
using Colossal.UI;
using Game.Input;
using Game.SceneFlow;
using Game.Settings;
using PDX.ModsUI;
using PDX.ModsUI.Adapters;
using PDX.ModsUI.Services;
using PDX.SDK.Contracts.Enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.PSI.PdxSdk;

public class PdxModsUI : IPdxModsUI, IDisposable
{
	private class ColossalUIViewAdapter : ICohtmlViewAdapter, IDisposable
	{
		private readonly InputBarrier m_InputBarrier;

		private UIView m_View;

		private bool isAvailable
		{
			get
			{
				if (m_View != null && m_View.enabled)
				{
					return m_View.View.IsReadyForBindings();
				}
				return false;
			}
		}

		public bool IsActiveAndEnabled
		{
			get
			{
				UIView view = m_View;
				if (view != null)
				{
					return view.enabled;
				}
				return false;
			}
		}

		public event Action ReadyForBindings;

		public ColossalUIViewAdapter()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			m_InputBarrier = InputManager.instance.CreateGlobalBarrier("ColossalUIViewAdapter");
			Settings val = Settings.New;
			((Settings)(ref val)).textInputHandler = (TextInputHandler)(object)GameManager.instance.userInterface.virtualKeyboard;
			m_View = UIManager.defaultUISystem.CreateView(kModsUIUri, val, (Camera)null);
			m_View.Listener.ReadyForBindings += OnReadyForBindings;
			m_View.Listener.TextInputTypeChanged += OnTextInputTypeChanged;
			m_View.Listener.CaretRectChanged += OnCaretRectChanged;
		}

		public void Disable()
		{
			m_View.enabled = false;
			m_InputBarrier.blocked = false;
		}

		public void Enable()
		{
			m_InputBarrier.blocked = true;
			m_View.enabled = true;
		}

		public void Reload()
		{
			m_View.View.Reload();
		}

		public BoundEventHandle BindCall(string callName, Delegate handler)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (!isAvailable)
			{
				throw new Exception("Not ready for bindings");
			}
			return m_View.View.BindCall(callName, handler);
		}

		public BoundEventHandle RegisterForEvent(string callName, Delegate handler)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (!isAvailable)
			{
				throw new Exception("Not ready for bindings");
			}
			return m_View.View.RegisterForEvent(callName, handler);
		}

		public void UnbindCall(BoundEventHandle boundEventHandle)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (isAvailable)
			{
				m_View.View.UnbindCall(boundEventHandle);
			}
		}

		public void UnregisterFromEvent(BoundEventHandle boundEventHandle)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (isAvailable)
			{
				m_View.View.UnregisterFromEvent(boundEventHandle);
			}
		}

		public void TriggerEvent<T>(string eventName, T message)
		{
			if (isAvailable)
			{
				m_View.View.TriggerEvent<T>(eventName, message);
			}
		}

		public void AddHostLocation(string key, List<string> value)
		{
			m_View.uiSystem.AddHostLocation(key, value.Select((string x) => (x: x, 0)), false);
		}

		public void RemoveHostLocation(string key)
		{
			m_View.uiSystem.RemoveHostLocation(key);
		}

		public void Dispose()
		{
			m_View.Listener.ReadyForBindings -= OnReadyForBindings;
			m_View.Listener.TextInputTypeChanged -= OnTextInputTypeChanged;
			m_View.Listener.CaretRectChanged -= OnCaretRectChanged;
			m_View.uiSystem.DestroyView(m_View);
			m_View = null;
			m_InputBarrier.Dispose();
		}

		private void OnReadyForBindings()
		{
			this.ReadyForBindings?.Invoke();
		}

		private void OnTextInputTypeChanged(ControlType type)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			InputManager.instance.hasInputFieldFocus = (int)type == 0;
		}

		private void OnCaretRectChanged(int x, int y, uint width, uint height)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			InputManager.instance.caretRect = (new Vector2((float)x, (float)y), new Vector2((float)width, (float)height));
		}
	}

	private class ModsUILogger : LogService
	{
		public ModsUILogger(LogLevel level)
			: base(level)
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


		public override void WriteLogEntry(string message, LogLevel logLevel, string source = null, string callerFilePath = null)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Invalid comparison between Unknown and I4
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Invalid comparison between Unknown and I4
			if (logLevel >= ((LogService)this).LogLevel && (int)logLevel != 999)
			{
				if (source == null && callerFilePath != null)
				{
					source = Path.GetFileNameWithoutExtension(callerFilePath);
				}
				string text = ((source == null) ? message : (source + ": " + message));
				if ((int)logLevel == 2)
				{
					log.Warn((object)text);
				}
				else if ((int)logLevel == 3)
				{
					log.Error((object)text);
				}
				else
				{
					log.Info((object)text);
				}
			}
		}
	}

	private static ILog log = LogManager.GetLogger("PdxModsUI").SetShowsErrorsInUI(false);

	private static readonly string kModsUIHost = "ModsUI".ToLowerInvariant();

	private static readonly string kModsUIUri = "assetdb://" + kModsUIHost + "/index.html";

	private PdxSdkPlatform m_PdxPlatform;

	public PdxSdkPlatform platform => m_PdxPlatform;

	public string locale => ((object)PdxSdkExtensions.ToPdxLanguage(GameManager.instance.localizationManager.activeLocaleId)/*cast due to .constrained prefix*/).ToString().Replace('_', '-');

	public ICohtmlViewAdapter uiViewAdapter => (ICohtmlViewAdapter)(object)new ColossalUIViewAdapter();

	public ILogService logger => (ILogService)(object)new ModsUILogger((LogLevel)2);

	public bool isActive
	{
		get
		{
			if (m_PdxPlatform != null)
			{
				return m_PdxPlatform.isModsUIActive;
			}
			return false;
		}
	}

	public PdxModsUI()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		InputManager.instance.EventActiveDeviceChanged += OnActiveDeviceChanged;
		GameManager.instance.localizationManager.onActiveDictionaryChanged += UpdateLocale;
		m_PdxPlatform = PlatformManager.instance.GetPSI<PdxSdkPlatform>("PdxSdk");
		PdxSdkPlatform pdxPlatform = m_PdxPlatform;
		if (pdxPlatform != null)
		{
			pdxPlatform.SetPdxModsUI((IPdxModsUI)(object)this);
		}
		PlatformManager.instance.onPlatformRegistered += (PlatformRegisteredHandler)delegate(IPlatformServiceIntegration psi)
		{
			PdxSdkPlatform val = (PdxSdkPlatform)(object)((psi is PdxSdkPlatform) ? psi : null);
			if (val != null)
			{
				m_PdxPlatform = val;
				m_PdxPlatform.SetPdxModsUI((IPdxModsUI)(object)this);
			}
		};
	}

	public void Show()
	{
		PdxSdkPlatform pdxPlatform = m_PdxPlatform;
		if (pdxPlatform != null)
		{
			pdxPlatform.ShowModsUI();
		}
	}

	public void Destroy()
	{
		PdxSdkPlatform pdxPlatform = m_PdxPlatform;
		if (pdxPlatform != null)
		{
			pdxPlatform.DestroyModsUI();
		}
	}

	private void OnActiveDeviceChanged(InputDevice newDevice, InputDevice oldDevice, bool schemeChanged)
	{
		if (schemeChanged || InputManager.instance.activeControlScheme == InputManager.ControlScheme.Gamepad)
		{
			PdxSdkPlatform pdxPlatform = m_PdxPlatform;
			if (pdxPlatform != null)
			{
				pdxPlatform.UpdateInputMode();
			}
		}
	}

	private void UpdateLocale()
	{
		PdxSdkPlatform pdxPlatform = m_PdxPlatform;
		if (pdxPlatform != null)
		{
			pdxPlatform.ChangeModsUILanguage(locale);
		}
	}

	public void Dispose()
	{
		InputManager.instance.EventActiveDeviceChanged -= OnActiveDeviceChanged;
		GameManager.instance.localizationManager.onActiveDictionaryChanged -= UpdateLocale;
	}

	public InputMode GetInputMode()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		InputManager.ControlScheme activeControlScheme = InputManager.instance.activeControlScheme;
		InputManager.GamepadType finalInputHintsType = SharedSettings.instance.userInterface.GetFinalInputHintsType();
		return (InputMode)(activeControlScheme switch
		{
			InputManager.ControlScheme.KeyboardAndMouse => 0, 
			InputManager.ControlScheme.Gamepad => finalInputHintsType switch
			{
				InputManager.GamepadType.Xbox => 110, 
				InputManager.GamepadType.PS => 210, 
				_ => throw new Exception($"Unknown control scheme {activeControlScheme} with gamepad {finalInputHintsType}"), 
			}, 
			_ => throw new Exception($"Unknown control scheme {activeControlScheme}"), 
		});
	}
}
