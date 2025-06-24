using System;
using System.Diagnostics;
using cohtml.Net;
using Colossal.FileSystem;
using Colossal.PSI.Environment;
using Colossal.UI;
using Colossal.UI.Binding;
using Colossal.UI.Fatal;
using Game.Input;
using Game.SceneFlow;
using Game.UI.Localization;
using UnityEngine;

namespace Game.UI;

public class UISystemBootstrapper : MonoBehaviour, IUIViewComponent
{
	private UIView m_View;

	private UIView m_FatalView;

	private UIManager m_UIManager;

	private UIInputSystem m_UIInputSystem;

	private UIInputSystem m_FallbackUIInputSystem;

	private InputBindings m_InputBindings;

	public bool m_EnableFatalUI;

	public string m_Url;

	public string m_FatalUrl;

	public View View => m_View.View;

	public IUnityViewListener Listener => (IUnityViewListener)(object)m_View.Listener;

	private async void Awake()
	{
		Debug.LogWarning((object)"UISystemBootstrapper is only meant for development purpose");
		await Capabilities.CacheCapabilities();
		UIManager.log.Info((object)"Bootstrapping cohtmlUISystem");
		InputManager.CreateInstance();
		m_UIManager = new UIManager(true);
		Settings val = Settings.New;
		val.enableDebugger = true;
		if ((Object)(object)GameManager.instance != (Object)null)
		{
			((Settings)(ref val)).localizationManager = (ILocalizationManager)(object)new UILocalizationManager(GameManager.instance.localizationManager);
		}
		((Settings)(ref val)).resourceHandler = (IResourceHandler)(object)new GameUIResourceHandler((MonoBehaviour)(object)this);
		val.enableDebugger = true;
		UISystem val2 = m_UIManager.CreateUISystem(val);
		val2.AddHostLocation("gameui", EnvPath.kContentPath + "/Game/UI", true, 0);
		m_View = val2.CreateView(m_Url, Settings.New, ((Component)this).GetComponent<Camera>());
		m_View.enabled = true;
		m_View.AudioSource = ((Component)this).GetComponent<AudioSource>();
		m_View.Listener.ReadyForBindings += OnReadyForBindings;
		m_UIInputSystem = new UIInputSystem(val2, true);
		m_InputBindings = new InputBindings();
		if (!m_EnableFatalUI)
		{
			return;
		}
		ErrorPage val3 = new ErrorPage();
		val3.AddAction("quit", (Action)delegate
		{
			Application.Quit();
		});
		val3.AddAction("visit", (Action)delegate
		{
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = "https://pdxint.at/3Do979W",
					UseShellExecute = true
				});
			}
			catch
			{
				Application.Quit();
			}
		});
		val3.SetRoot(EnvPath.kContentPath + "/Game/UI/.fatal", EnvPath.kContentPath + "/Game/.fatal");
		val3.SetFonts(EnvPath.kContentPath + "/Game/UI/Fonts", EnvPath.kContentPath + "/Game/Fonts.cok");
		val3.SetStopCode((Exception)new AggregateException());
		Settings val4 = Settings.New;
		((Settings)(ref val4)).resourceHandler = (IResourceHandler)new FatalResourceHandler(val3);
		val4.enableDebugger = true;
		val4.debuggerPort = 9445;
		UISystem val5 = m_UIManager.CreateUISystem(val4);
		Settings val6 = Settings.New;
		val6.liveReload = true;
		m_FatalView = val5.CreateView(m_FatalUrl, val6, ((Component)this).GetComponent<Camera>());
		m_FatalView.enabled = true;
		m_FallbackUIInputSystem = new UIInputSystem(val5, true);
	}

	private void Update()
	{
		if (m_FatalView != null)
		{
			m_FatalView.enabled = m_EnableFatalUI;
		}
		InputManager.instance?.Update();
		UIManager uIManager = m_UIManager;
		if (uIManager != null)
		{
			uIManager.Update();
		}
		InputBindings inputBindings = m_InputBindings;
		if (inputBindings != null)
		{
			((CompositeBinding)inputBindings).Update();
		}
	}

	private void LateUpdate()
	{
		UIInputSystem uIInputSystem = m_UIInputSystem;
		if (uIInputSystem != null)
		{
			uIInputSystem.DispatchInputEvents(true);
		}
		UIInputSystem fallbackUIInputSystem = m_FallbackUIInputSystem;
		if (fallbackUIInputSystem != null)
		{
			fallbackUIInputSystem.DispatchInputEvents(true);
		}
	}

	private void OnReadyForBindings()
	{
		InputBindings inputBindings = m_InputBindings;
		if (inputBindings != null)
		{
			((CompositeBinding)inputBindings).Attach(m_View.View);
		}
	}

	private void OnDestroy()
	{
		if (m_View != null)
		{
			m_View.Listener.ReadyForBindings -= OnReadyForBindings;
		}
		InputBindings inputBindings = m_InputBindings;
		if (inputBindings != null)
		{
			((CompositeBinding)inputBindings).Detach();
		}
		m_InputBindings?.Dispose();
		UIInputSystem uIInputSystem = m_UIInputSystem;
		if (uIInputSystem != null)
		{
			uIInputSystem.Dispose();
		}
		UIInputSystem fallbackUIInputSystem = m_FallbackUIInputSystem;
		if (fallbackUIInputSystem != null)
		{
			fallbackUIInputSystem.Dispose();
		}
		UIManager uIManager = m_UIManager;
		if (uIManager != null)
		{
			uIManager.Dispose();
		}
		InputManager.DestroyInstance();
	}

	bool IUIViewComponent.get_enabled()
	{
		return ((Behaviour)this).enabled;
	}
}
