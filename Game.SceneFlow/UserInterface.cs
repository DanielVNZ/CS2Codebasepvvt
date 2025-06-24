using System;
using System.Threading.Tasks;
using cohtml.Net;
using Colossal.Localization;
using Colossal.Logging;
using Colossal.UI;
using Colossal.UI.Binding;
using Game.Input;
using Game.PSI;
using Game.Settings;
using Game.UI;
using Game.UI.Localization;
using Game.UI.Menu;
using UnityEngine;

namespace Game.SceneFlow;

public class UserInterface : IDisposable
{
	private static ILog log = UIManager.log;

	private UICursorCollection m_CursorCollection;

	private CompositeBinding m_Bindings;

	private TaskCompletionSource<bool> m_BindingsReady;

	public UIView view { get; private set; }

	public LocalizationBindings localizationBindings { get; private set; }

	public OverlayBindings overlayBindings { get; private set; }

	public AppBindings appBindings { get; private set; }

	public InputHintBindings inputHintBindings { get; private set; }

	public ParadoxBindings paradoxBindings { get; private set; }

	public VirtualKeyboard virtualKeyboard { get; private set; }

	public IBindingRegistry bindings => (IBindingRegistry)(object)m_Bindings;

	public UserInterface(string url, LocalizationManager localizationManager, UISystem uiSystem)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		m_BindingsReady = new TaskCompletionSource<bool>();
		m_CursorCollection = Resources.Load<UICursorCollection>("Input/UI Cursors");
		m_Bindings = new CompositeBinding();
		virtualKeyboard = new VirtualKeyboard();
		Settings val = Settings.New;
		((Settings)(ref val)).textInputHandler = (TextInputHandler)(object)virtualKeyboard;
		val.acceptsInput = true;
		view = uiSystem.CreateView(url, val, (Camera)null);
		view.Listener.ReadyForBindings += OnReadyForBindings;
		view.Listener.NavigateTo += OnNavigateTo;
		view.Listener.NodeMouseEvent += OnNodeMouseEvent;
		view.Listener.CursorChanged += OnCursorChanged;
		view.Listener.TextInputTypeChanged += OnTextInputTypeChanged;
		view.Listener.CaretRectChanged += OnCaretRectChanged;
		view.enabled = true;
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)(this.localizationBindings = new LocalizationBindings(localizationManager)));
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)(this.appBindings = new AppBindings()));
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)(this.overlayBindings = new OverlayBindings()));
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)new AudioBindings());
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)new UserBindings());
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)new InputBindings());
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)new InputActionBindings());
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)(this.inputHintBindings = new InputHintBindings()));
		m_Bindings.AddUpdateBinding((IUpdateBinding)(object)(this.paradoxBindings = new ParadoxBindings()));
		this.overlayBindings.hintMessages = localizationManager.activeDictionary.GetIndexedLocaleIDs("Loading.HINTMESSAGE");
		if (view.View.IsReadyForBindings())
		{
			OnReadyForBindings();
		}
		SharedSettings.instance.userState.onSettingsApplied += OnSettingsApplied;
	}

	private void OnSettingsApplied(Setting setting)
	{
		if (setting is UserState)
		{
			GameManager.instance.RunOnMainThread(delegate
			{
				appBindings.UpdateCanContinueBinding();
			});
		}
	}

	public void Update()
	{
		m_Bindings.Update();
	}

	public void Dispose()
	{
		SharedSettings.instance.userState.onSettingsApplied -= OnSettingsApplied;
		overlayBindings.DeactivateAllScreens();
		appBindings.activeUI = null;
		m_Bindings.DisposeBindings();
		if (m_Bindings.attached)
		{
			m_Bindings.Detach();
		}
		if (view != null)
		{
			view.Listener.ReadyForBindings -= OnReadyForBindings;
			view.Listener.NavigateTo -= OnNavigateTo;
			view.Listener.NodeMouseEvent -= OnNodeMouseEvent;
			view.Listener.CursorChanged -= OnCursorChanged;
			view.Listener.TextInputTypeChanged -= OnTextInputTypeChanged;
			view.uiSystem.DestroyView(view);
			view = null;
		}
	}

	public Task WaitForBindings()
	{
		return m_BindingsReady.Task;
	}

	private void OnReadyForBindings()
	{
		log.Debug((object)"Ready for bindings");
		m_Bindings.Attach(view.View);
		appBindings.ready = true;
		m_BindingsReady.TrySetResult(result: true);
	}

	private bool OnNavigateTo(string url)
	{
		m_Bindings.Detach();
		return true;
	}

	private Actions OnNodeMouseEvent(INodeProxy node, IMouseEventData ev, IntPtr userData, PhaseType phaseType)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		if (InputManager.instance.activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse && (int)phaseType == 2)
		{
			if ((int)node.GetTag() == 10)
			{
				InputManager.instance.mouseOverUI = false;
			}
			else
			{
				InputManager.instance.mouseOverUI = true;
			}
		}
		return (Actions)0;
	}

	private void OnCursorChanged(Cursors cursor, string url)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_CursorCollection == (Object)null)
		{
			UICursorCollection.ResetCursor();
		}
		else if ((int)cursor == 31 && url != null)
		{
			m_CursorCollection.SetCursor(url);
		}
		else
		{
			m_CursorCollection.SetCursor(cursor);
		}
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
