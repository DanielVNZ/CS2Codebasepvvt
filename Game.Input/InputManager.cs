using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Colossal;
using Colossal.Logging;
using Colossal.PSI.Common;
using Game.Modding;
using Game.PSI;
using Game.SceneFlow;
using Game.UI;
using Game.UI.Localization;
using Game.UI.Menu;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;

namespace Game.Input;

public class InputManager : IDisposable, IInputActionCollection, IEnumerable<InputAction>, IEnumerable
{
	public readonly struct CompositeData
	{
		public readonly string m_TypeName;

		public readonly ActionType m_ActionType;

		public readonly IReadOnlyDictionary<ActionComponent, CompositeComponentData> m_Data;

		public CompositeData(string typeName, ActionType actionType, CompositeComponentData[] data)
		{
			m_TypeName = typeName;
			m_ActionType = actionType;
			m_Data = new ReadOnlyDictionary<ActionComponent, CompositeComponentData>(data.ToDictionary((CompositeComponentData d) => d.m_Component));
		}

		public bool TryGetData(ActionComponent component, out CompositeComponentData data)
		{
			return m_Data.TryGetValue(component, out data);
		}

		public bool TryFindByBindingName(string bindingName, out CompositeComponentData data)
		{
			foreach (CompositeComponentData value in m_Data.Values)
			{
				if (value.m_BindingName == bindingName)
				{
					data = value;
					return true;
				}
			}
			data = default(CompositeComponentData);
			return false;
		}
	}

	public readonly struct CompositeComponentData
	{
		public static CompositeComponentData defaultData = new CompositeComponentData(ActionComponent.Press, "binding", "modifier");

		public readonly ActionComponent m_Component;

		public readonly string m_BindingName;

		public readonly string m_ModifierName;

		public CompositeComponentData(ActionComponent component, string bindingName, string modifierName)
		{
			m_Component = component;
			m_BindingName = bindingName;
			m_ModifierName = modifierName;
		}
	}

	public delegate void ActiveDeviceChanged(InputDevice newDevice, InputDevice oldDevice, bool schemeChanged);

	public enum PathType
	{
		Effective,
		Original,
		Overridden
	}

	[Flags]
	public enum BindingOptions
	{
		None = 0,
		OnlyOriginal = 1,
		OnlyRebindable = 2,
		OnlyRebound = 4,
		OnlyBuiltIn = 8,
		ExcludeDummy = 0x10,
		ExcludeHidden = 0x20
	}

	public enum ControlScheme : byte
	{
		KeyboardAndMouse,
		Gamepad
	}

	[Flags]
	public enum DeviceType
	{
		None = 0,
		Keyboard = 1,
		Mouse = 2,
		Gamepad = 4,
		All = 7
	}

	public enum GamepadType
	{
		Xbox,
		PS
	}

	internal class DeferManagerUpdatingWrapper : IDisposable
	{
		private static int sDeferUpdating;

		private readonly DeferBindingResolutionWrapper m_BindingResolution;

		public bool isDeferred => sDeferUpdating != 0;

		internal DeferManagerUpdatingWrapper()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			m_BindingResolution = new DeferBindingResolutionWrapper();
		}

		public void Acquire()
		{
			sDeferUpdating++;
			m_BindingResolution.Acquire();
			ProxyAction.sDeferUpdatingWrapper.Acquire();
		}

		public void Dispose()
		{
			m_BindingResolution.Dispose();
			if (InputActionMap.s_DeferBindingResolution == 0 && instance != null)
			{
				string text = default(string);
				ProxyActionMap proxyActionMap = default(ProxyActionMap);
				foreach (KeyValuePair<string, ProxyActionMap> map in instance.m_Maps)
				{
					map.Deconstruct(ref text, ref proxyActionMap);
					proxyActionMap.sourceMap.ResolveBindingsIfNecessary();
				}
			}
			ProxyAction.sDeferUpdatingWrapper.Dispose();
			if (sDeferUpdating > 0)
			{
				sDeferUpdating--;
			}
			if (sDeferUpdating == 0)
			{
				try
				{
					sDeferUpdating++;
					instance?.ProcessActionsUpdate(ignoreDefer: true);
				}
				finally
				{
					sDeferUpdating--;
				}
			}
		}
	}

	private bool m_NeedUpdate;

	private static IReadOnlyList<CompositeData> m_Composites = Array.Empty<CompositeData>();

	private static Cache m_LayoutCache;

	private static StringBuilder m_PathBuilder;

	public const string kShiftName = "<Keyboard>/shift";

	public const string kCtrlName = "<Keyboard>/ctrl";

	public const string kAltName = "<Keyboard>/alt";

	public const string kLeftStick = "<Gamepad>/leftStickPress";

	public const string kRightStick = "<Gamepad>/rightStickPress";

	public const string kSplashScreenMap = "Splash screen";

	public const string kNavigationMap = "Navigation";

	public const string kMenuMap = "Menu";

	public const string kCameraMap = "Camera";

	public const string kToolMap = "Tool";

	public const string kShortcutsMap = "Shortcuts";

	public const string kPhotoModeMap = "Photo mode";

	public const string kEditorMap = "Editor";

	public const string kDebugMap = "Debug";

	public const string kEngagementMap = "Engagement";

	public const int kIdleDelay = 30;

	private static Dictionary<DeviceType, HashSet<string>> kModifiers = new Dictionary<DeviceType, HashSet<string>>
	{
		{
			DeviceType.Keyboard,
			new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "<Keyboard>/shift", "<Keyboard>/ctrl", "<Keyboard>/alt" }
		},
		{
			DeviceType.Mouse,
			new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "<Keyboard>/shift", "<Keyboard>/ctrl", "<Keyboard>/alt" }
		},
		{
			DeviceType.Gamepad,
			new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "<Gamepad>/leftStickPress", "<Gamepad>/rightStickPress" }
		}
	};

	private static InputManager s_Instance;

	public static readonly ILog log = LogManager.GetLogger("InputManager");

	private readonly InputConflictResolution m_ConflictResolution = new InputConflictResolution();

	private readonly InputActionAsset m_ActionAsset;

	private readonly UIInputActionCollection m_UIActionCollection;

	private readonly UIInputActionCollection m_ToolActionCollection;

	private readonly Dictionary<string, ProxyActionMap> m_Maps = new Dictionary<string, ProxyActionMap>();

	private Dictionary<InputDevice, DeviceListener> m_DeviceListeners;

	private InputDevice m_LastActiveDevice;

	private bool m_MouseOverUI;

	private float m_AccumulatedIdleDelay;

	private bool m_WasWorldReady;

	private bool m_Idle;

	private bool m_HasFocus;

	private bool m_HasInputFieldFocus;

	private bool m_OverlayActive;

	private bool m_HideCursor;

	private ControlScheme m_ActiveControlScheme;

	private DeviceArray m_Devices;

	private DeviceType m_ConnectedDeviceTypes;

	private DeviceType m_BlockedControlTypes;

	private DeviceType m_Mask;

	private readonly Dictionary<ProxyBinding, ProxyBinding.Watcher> m_ProxyBindingWatchers = new Dictionary<ProxyBinding, ProxyBinding.Watcher>(new ProxyBinding.Comparer(ProxyBinding.Comparer.Options.MapName | ProxyBinding.Comparer.Options.ActionName | ProxyBinding.Comparer.Options.Name | ProxyBinding.Comparer.Options.Device | ProxyBinding.Comparer.Options.Component));

	private static string m_ProhibitionModifierProcessor;

	private const string kKeyBindingConflict = "KeyBindingConflict";

	private const string kKeyBindingConflictResolved = "KeyBindingConflictResolved";

	private static readonly DeferManagerUpdatingWrapper sDeferUpdatingWrapper = new DeferManagerUpdatingWrapper();

	public IEnumerable<ProxyAction> actions
	{
		get
		{
			string text = default(string);
			ProxyActionMap proxyActionMap = default(ProxyActionMap);
			ProxyAction proxyAction = default(ProxyAction);
			foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
			{
				map.Deconstruct(ref text, ref proxyActionMap);
				ProxyActionMap proxyActionMap2 = proxyActionMap;
				foreach (KeyValuePair<string, ProxyAction> action in proxyActionMap2.actions)
				{
					action.Deconstruct(ref text, ref proxyAction);
					yield return proxyAction;
				}
			}
		}
	}

	public static InputManager instance => s_Instance;

	public bool mouseOverUI
	{
		get
		{
			return m_MouseOverUI;
		}
		set
		{
			if (value != m_MouseOverUI)
			{
				m_MouseOverUI = value;
				this.EventMouseOverUIChanged?.Invoke(value);
			}
		}
	}

	public bool hasInputFieldFocus
	{
		get
		{
			return m_HasInputFieldFocus;
		}
		set
		{
			if (value != m_HasInputFieldFocus)
			{
				log.VerboseFormat("Has input field focus: {0}", (object)value);
			}
			m_HasInputFieldFocus = value;
		}
	}

	public bool overlayActive => m_OverlayActive;

	public (Vector2, Vector2) caretRect { get; set; }

	public bool controlOverWorld
	{
		get
		{
			if (!mouseOverUI || activeControlScheme != ControlScheme.KeyboardAndMouse)
			{
				return mouseOnScreen;
			}
			return false;
		}
	}

	InputBinding? IInputActionCollection.bindingMask
	{
		get
		{
			return mask.ToInputBinding();
		}
		set
		{
			mask = value.ToDeviceType();
		}
	}

	ReadOnlyArray<InputDevice>? IInputActionCollection.devices
	{
		get
		{
			return ((DeviceArray)(ref m_Devices)).Get();
		}
		set
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (!((DeviceArray)(ref m_Devices)).Set(value))
			{
				return;
			}
			if (value.HasValue)
			{
				Enumerator<InputDevice> enumerator = value.Value.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						InputDevice current = enumerator.Current;
						log.VerboseFormat("Device: {0}", (object)((object)current.description/*cast due to .constrained prefix*/).ToString());
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			else
			{
				log.VerboseFormat("Device: null", Array.Empty<object>());
			}
			string text = default(string);
			ProxyActionMap proxyActionMap = default(ProxyActionMap);
			foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
			{
				map.Deconstruct(ref text, ref proxyActionMap);
				proxyActionMap.sourceMap.devices = value;
			}
		}
	}

	ReadOnlyArray<InputControlScheme> IInputActionCollection.controlSchemes => m_ActionAsset.controlSchemes;

	public ControlScheme activeControlScheme
	{
		get
		{
			return m_ActiveControlScheme;
		}
		private set
		{
			if (m_ActiveControlScheme != value)
			{
				log.VerboseFormat("Active control scheme set: {0}", (object)value);
				m_ActiveControlScheme = value;
				UpdateCursorVisibility();
				RefreshActiveControl();
				this.EventControlSchemeChanged?.Invoke(value);
				Telemetry.ControlSchemeChanged(value);
			}
		}
	}

	public bool isGamepadControlSchemeActive => activeControlScheme == ControlScheme.Gamepad;

	public bool isKeyboardAndMouseControlSchemeActive => activeControlScheme == ControlScheme.KeyboardAndMouse;

	public DeviceType connectedDeviceTypes => m_ConnectedDeviceTypes;

	internal DeviceType mask
	{
		get
		{
			return m_Mask;
		}
		set
		{
			if (value == m_Mask)
			{
				return;
			}
			log.VerboseFormat("Set mask: {0}", (object)value);
			m_Mask = value;
			string text = default(string);
			ProxyActionMap proxyActionMap = default(ProxyActionMap);
			foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
			{
				map.Deconstruct(ref text, ref proxyActionMap);
				proxyActionMap.mask = value;
			}
		}
	}

	internal DeviceType blockedControlTypes
	{
		get
		{
			return m_BlockedControlTypes;
		}
		set
		{
			if (value != m_BlockedControlTypes)
			{
				log.VerboseFormat("Block control types: {0}", (object)value);
				m_BlockedControlTypes = value;
				RefreshActiveControl();
			}
		}
	}

	public bool mouseOnScreen
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			if (mousePosition.x >= 0f && mousePosition.x < (float)Screen.width && mousePosition.y >= 0f && mousePosition.y < (float)Screen.height)
			{
				return m_HasFocus;
			}
			return false;
		}
	}

	public Vector2 gamepadPointerPosition => new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);

	public Vector3 mousePosition
	{
		get
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			Mouse current = Mouse.current;
			if (activeControlScheme == ControlScheme.KeyboardAndMouse && current != null)
			{
				return Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)current).position).ReadValue());
			}
			return Vector2.op_Implicit(gamepadPointerPosition);
		}
	}

	public bool hideCursor
	{
		get
		{
			return m_HideCursor;
		}
		set
		{
			if (value != m_HideCursor)
			{
				m_HideCursor = value;
				UpdateCursorVisibility();
			}
		}
	}

	public CursorLockMode cursorLockMode
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return Cursor.lockState;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Cursor.lockState = value;
		}
	}

	public InputUser inputUser { get; private set; }

	public int actionVersion { get; private set; }

	internal Dictionary<string, HashSet<ProxyAction>> keyActionMap { get; } = new Dictionary<string, HashSet<ProxyAction>>(StringComparer.OrdinalIgnoreCase);

	internal Dictionary<ProxyAction, HashSet<string>> actionKeyMap { get; } = new Dictionary<ProxyAction, HashSet<string>>();

	internal Dictionary<int, ProxyAction> actionIndex { get; } = new Dictionary<int, ProxyAction>();

	private static string prohibitionModifierProcessor => m_ProhibitionModifierProcessor ?? (m_ProhibitionModifierProcessor = InternedString.op_Implicit(((TypeTable)(ref InputProcessor.s_Processors)).FindNameForType(typeof(ProhibitionModifierProcessor))));

	internal UIInputActionCollection uiActionCollection => m_UIActionCollection;

	internal UIInputActionCollection toolActionCollection => m_ToolActionCollection;

	public DeviceType bindingConflicts { get; private set; }

	public static bool IsKeyboardConnected => (instance.connectedDeviceTypes & DeviceType.Keyboard) != 0;

	public static bool IsMouseConnected => (instance.connectedDeviceTypes & DeviceType.Mouse) != 0;

	public static bool IsGamepadConnected => (instance.connectedDeviceTypes & DeviceType.Gamepad) != 0;

	public event Action<ControlScheme> EventControlSchemeChanged;

	public event ActiveDeviceChanged EventActiveDeviceChanged;

	public event Action EventActiveDeviceDisconnected;

	public event Action EventActiveDeviceAssociationLost;

	public event Action EventDevicePaired;

	public event Action EventActionsChanged;

	public event Action EventEnabledActionsChanged;

	public event Action EventActionMasksChanged;

	public event Action EventActionDisplayNamesChanged;

	public event Action<bool> EventMouseOverUIChanged;

	internal event Action EventPreResolvedActionChanged;

	public ProxyAction FindAction(string mapName, string actionName)
	{
		return FindActionMap(mapName)?.FindAction(actionName);
	}

	public bool TryFindAction(string mapName, string actionName, out ProxyAction action)
	{
		action = FindAction(mapName, actionName);
		return action != null;
	}

	public ProxyAction FindAction(ProxyBinding binding)
	{
		return FindActionMap(binding.mapName)?.FindAction(binding.actionName);
	}

	public bool TryFindAction(ProxyBinding binding, out ProxyAction action)
	{
		action = FindAction(binding.mapName, binding.actionName);
		return action != null;
	}

	public ProxyAction FindAction(InputAction action)
	{
		return FindActionMap((action != null) ? action.actionMap : null)?.FindAction(action);
	}

	public bool TryFindAction(InputAction action, out ProxyAction proxyAction)
	{
		proxyAction = FindAction(action);
		return proxyAction != null;
	}

	public ProxyAction FindAction(Guid guid)
	{
		string text = default(string);
		ProxyActionMap proxyActionMap = default(ProxyActionMap);
		ProxyAction proxyAction = default(ProxyAction);
		foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
		{
			map.Deconstruct(ref text, ref proxyActionMap);
			foreach (KeyValuePair<string, ProxyAction> action in proxyActionMap.actions)
			{
				action.Deconstruct(ref text, ref proxyAction);
				ProxyAction proxyAction2 = proxyAction;
				if (proxyAction2.sourceAction.id == guid)
				{
					proxyAction = proxyAction2;
					return proxyAction;
				}
			}
		}
		return null;
	}

	public bool TryFindAction(Guid guid, out ProxyAction proxyAction)
	{
		proxyAction = FindAction(guid);
		return proxyAction != null;
	}

	internal bool TryFindAction(int index, out ProxyAction action)
	{
		return actionIndex.TryGetValue(index, out action);
	}

	private void RefreshActiveControl()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		mask = GetMaskForControlScheme();
		if (m_ActiveControlScheme == ControlScheme.KeyboardAndMouse && ((InputDevice)Keyboard.current).added)
		{
			Input.imeCompositionMode = (IMECompositionMode)(hasInputFieldFocus ? 1 : 2);
			Keyboard.current.SetIMEEnabled(hasInputFieldFocus);
			Keyboard.current.SetIMECursorPosition(caretRect.Item1 + caretRect.Item2);
		}
	}

	private DeviceType GetMaskForControlScheme()
	{
		return (DeviceType)((activeControlScheme switch
		{
			ControlScheme.KeyboardAndMouse => (!overlayActive) ? (hasInputFieldFocus ? 2 : 3) : 0, 
			ControlScheme.Gamepad => (!overlayActive) ? 4 : 0, 
			_ => 0, 
		}) & (int)(~blockedControlTypes));
	}

	internal void OnActionChanged()
	{
		if (sDeferUpdatingWrapper.isDeferred)
		{
			m_NeedUpdate = true;
		}
		else
		{
			ProcessActionsUpdate();
		}
	}

	private void ProcessActionsUpdate(bool ignoreDefer = false)
	{
		if ((!sDeferUpdatingWrapper.isDeferred || ignoreDefer) && m_NeedUpdate)
		{
			m_NeedUpdate = false;
			actionVersion++;
			CheckConflicts();
			this.EventActionsChanged?.Invoke();
		}
	}

	internal void AddActions(ProxyAction.Info[] actionsToAdd)
	{
		ProxyAction[] array = new ProxyAction[actionsToAdd.Length];
		using (DeferUpdating())
		{
			for (int i = 0; i < actionsToAdd.Length; i++)
			{
				ProxyActionMap orCreateMap = GetOrCreateMap(actionsToAdd[i].m_Map);
				array[i] = orCreateMap.AddAction(actionsToAdd[i], bulk: true);
			}
		}
		ProxyActionMap[] array2 = array.Select((ProxyAction a) => a.map).Distinct().ToArray();
		for (int num = 0; num < array2.Length; num++)
		{
			array2[num].UpdateState();
		}
	}

	internal void UpdateActionInKeyActionMap(ProxyAction action)
	{
		string[] array;
		string[] array2;
		if (!actionKeyMap.TryGetValue(action, out var value))
		{
			array = action.usedKeys.ToArray();
			array2 = Array.Empty<string>();
			actionKeyMap[action] = new HashSet<string>(array);
		}
		else
		{
			HashSet<string> hashSet = action.usedKeys.ToHashSet();
			array = hashSet.Except(value).ToArray();
			array2 = value.Except(hashSet).ToArray();
			actionKeyMap[action] = hashSet;
		}
		string[] array3 = array2;
		foreach (string key in array3)
		{
			if (keyActionMap.TryGetValue(key, out var value2))
			{
				value2.Remove(action);
			}
		}
		array3 = array;
		foreach (string key2 in array3)
		{
			if (!keyActionMap.TryGetValue(key2, out var value3))
			{
				value3 = new HashSet<ProxyAction>();
				keyActionMap.Add(key2, value3);
			}
			value3.Add(action);
		}
	}

	public static bool HasConflicts(ProxyAction action1, ProxyAction action2, DeviceType? maskOverride1 = null, DeviceType? maskOverride2 = null)
	{
		DeviceType deviceType = maskOverride1 ?? action1.mask;
		DeviceType deviceType2 = maskOverride2 ?? action2.mask;
		DeviceType deviceType3 = default(DeviceType);
		ProxyComposite proxyComposite = default(ProxyComposite);
		ActionComponent actionComponent = default(ActionComponent);
		ProxyBinding proxyBinding = default(ProxyBinding);
		foreach (KeyValuePair<DeviceType, ProxyComposite> composite in action1.composites)
		{
			composite.Deconstruct(ref deviceType3, ref proxyComposite);
			ProxyComposite proxyComposite2 = proxyComposite;
			if ((proxyComposite2.m_Device & deviceType) == 0)
			{
				continue;
			}
			foreach (KeyValuePair<DeviceType, ProxyComposite> composite2 in action2.composites)
			{
				composite2.Deconstruct(ref deviceType3, ref proxyComposite);
				ProxyComposite proxyComposite3 = proxyComposite;
				if ((proxyComposite3.m_Device & deviceType2) == 0)
				{
					continue;
				}
				foreach (KeyValuePair<ActionComponent, ProxyBinding> binding in proxyComposite2.bindings)
				{
					binding.Deconstruct(ref actionComponent, ref proxyBinding);
					ProxyBinding x = proxyBinding;
					foreach (KeyValuePair<ActionComponent, ProxyBinding> binding2 in proxyComposite3.bindings)
					{
						binding2.Deconstruct(ref actionComponent, ref proxyBinding);
						ProxyBinding y = proxyBinding;
						if ((action1 != action2 || x.component != y.component) && ProxyBinding.ConflictsWith(x, y, checkUsage: false))
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public static bool CanConflict(ProxyAction action1, ProxyAction action2, DeviceType device)
	{
		if (action1 == action2)
		{
			return false;
		}
		if (action1.m_LinkedActions.Contains(new ProxyAction.LinkInfo
		{
			m_Action = action2,
			m_Device = device
		}))
		{
			return false;
		}
		if (action2.m_LinkedActions.Contains(new ProxyAction.LinkInfo
		{
			m_Action = action1,
			m_Device = device
		}))
		{
			return false;
		}
		return true;
	}

	public List<ProxyBinding> GetBindings(PathType pathType, BindingOptions bindingOptions)
	{
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.TraceFormat("Get {1} bindings {2} in {0}ms", (object)t.TotalMilliseconds, (object)pathType, (object)bindingOptions);
		});
		try
		{
			List<ProxyBinding> bindingsList = new List<ProxyBinding>();
			foreach (ProxyActionMap value in m_Maps.Values)
			{
				InputAction[] array = value.sourceMap.m_Actions;
				foreach (InputAction action in array)
				{
					action.ForEachCompositeOfAction(delegate(BindingSyntax iterator)
					{
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						if (TryGetComposite(action, iterator, pathType, bindingOptions, out var proxyComposite))
						{
							ActionComponent actionComponent = default(ActionComponent);
							ProxyBinding proxyBinding = default(ProxyBinding);
							foreach (KeyValuePair<ActionComponent, ProxyBinding> binding in proxyComposite.bindings)
							{
								binding.Deconstruct(ref actionComponent, ref proxyBinding);
								ProxyBinding item = proxyBinding;
								bindingsList.Add(item);
							}
						}
						return true;
					});
				}
			}
			return bindingsList;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public bool TryGetBinding(ProxyBinding bindingToGet, PathType pathType, BindingOptions bindingOptions, out ProxyBinding foundBinding)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		foundBinding = default(ProxyBinding);
		if (!TryFindAction(bindingToGet, out var action) || action.sourceAction == null)
		{
			return false;
		}
		if (!TryGetIterators(bindingToGet, action.sourceAction, out var compositeIterator, out var bindingIterator, out var compositeInstance, out var componentData))
		{
			return false;
		}
		return TryGetBinding(action.sourceAction, compositeIterator, bindingIterator, compositeInstance, componentData, pathType, bindingOptions, out foundBinding);
	}

	private bool TryGetBinding(InputAction action, BindingSyntax compositeIterator, BindingSyntax bindingIterator, CompositeInstance compositeInstance, CompositeComponentData componentData, PathType pathType, BindingOptions bindingOptions, out ProxyBinding foundBinding)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		bool num = (bindingOptions & BindingOptions.OnlyRebound) != 0;
		InputBinding binding = ((BindingSyntax)(ref bindingIterator)).binding;
		bool flag = TryGetMainBinding(bindingIterator, pathType, out var currentPath, out var originalPath);
		flag |= TryGetModifierBindings(action, compositeInstance, compositeIterator, bindingIterator, pathType, componentData, out var currentModifiers, out var originalModifiers);
		if (num && !flag)
		{
			foundBinding = default(ProxyBinding);
			return false;
		}
		ProxyBinding proxyBinding = new ProxyBinding(action, componentData.m_Component, ((InputBinding)(ref binding)).name, compositeInstance)
		{
			path = currentPath,
			modifiers = currentModifiers,
			originalPath = originalPath,
			originalModifiers = originalModifiers
		};
		InputBinding binding2 = ((BindingSyntax)(ref compositeIterator)).binding;
		proxyBinding.device = ((InputBinding)(ref binding2)).name.ToDeviceType();
		foundBinding = proxyBinding;
		return true;
	}

	public bool TryGetModifierBindings(InputAction action, CompositeInstance compositeInstance, BindingSyntax compositeIterator, BindingSyntax iterator, PathType pathType, CompositeComponentData componentData, out ProxyModifier[] currentModifiers, out ProxyModifier[] originalModifiers)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		currentModifiers = null;
		originalModifiers = null;
		if (!compositeInstance.allowModifiers)
		{
			return false;
		}
		Dictionary<DeviceType, HashSet<string>> dictionary = kModifiers;
		InputBinding binding = ((BindingSyntax)(ref compositeIterator)).binding;
		if (!dictionary.TryGetValue(((InputBinding)(ref binding)).name.ToDeviceType(), out var supportedModifiers))
		{
			return false;
		}
		bool isRebound = false;
		List<ProxyModifier> currentModifierList = new List<ProxyModifier>();
		List<ProxyModifier> originalModifierList = new List<ProxyModifier>();
		action.ForEachPartOfCompositeWithName(compositeIterator, componentData.m_ModifierName, delegate(BindingSyntax modifierIterator)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			InputBinding binding2 = ((BindingSyntax)(ref modifierIterator)).binding;
			if (string.IsNullOrEmpty(((InputBinding)(ref binding2)).path))
			{
				return true;
			}
			if (!supportedModifiers.Contains(((InputBinding)(ref binding2)).path))
			{
				return true;
			}
			isRebound |= ((InputBinding)(ref binding2)).overrideProcessors != null;
			if (!binding2.GetProcessors(pathType).Contains(prohibitionModifierProcessor))
			{
				currentModifierList.Add(new ProxyModifier
				{
					m_Component = componentData.m_Component,
					m_Name = ((InputBinding)(ref binding2)).name,
					m_Path = ((InputBinding)(ref binding2)).path
				});
			}
			if (!((InputBinding)(ref binding2)).processors.Contains(prohibitionModifierProcessor))
			{
				originalModifierList.Add(new ProxyModifier
				{
					m_Component = componentData.m_Component,
					m_Name = ((InputBinding)(ref binding2)).name,
					m_Path = ((InputBinding)(ref binding2)).path
				});
			}
			return true;
		}, out var _);
		currentModifiers = currentModifierList.ToArray();
		originalModifiers = originalModifierList.ToArray();
		return isRebound;
	}

	public bool TryGetMainBinding(BindingSyntax iterator, PathType pathType, out string currentPath, out string originalPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		InputBinding binding = ((BindingSyntax)(ref iterator)).binding;
		currentPath = binding.GetPath(pathType);
		originalPath = ((InputBinding)(ref binding)).path;
		return ((InputBinding)(ref binding)).overridePath != null;
	}

	public bool SetBindings(IEnumerable<ProxyBinding> newBindings, out List<ProxyBinding> resultBindings)
	{
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.TraceFormat("Set bindings in {0}ms", (object)t.TotalMilliseconds);
		});
		try
		{
			resultBindings = new List<ProxyBinding>();
			using (DeferUpdating())
			{
				foreach (ProxyBinding newBinding2 in newBindings)
				{
					SetBindingImpl(newBinding2, out var newBinding);
					resultBindings.Add(newBinding);
				}
			}
			return true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public bool SetBinding(ProxyBinding newBinding, out ProxyBinding result)
	{
		string bindingName = newBinding.ToString();
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.TraceFormat("Set binding {1} in {0}ms", (object)t.TotalMilliseconds, (object)bindingName);
		});
		try
		{
			using (DeferUpdating())
			{
				if (!SetBindingImpl(newBinding, out result))
				{
					return false;
				}
			}
			return true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private bool SetBindingImpl(ProxyBinding bindingToSet, out ProxyBinding newBinding)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (!TryFindAction(bindingToSet.mapName, bindingToSet.actionName, out var action) || action.sourceAction == null)
		{
			newBinding = default(ProxyBinding);
			return false;
		}
		if (!TryGetIterators(bindingToSet, action.sourceAction, out var compositeIterator, out var bindingIterator, out var compositeInstance, out var componentData))
		{
			newBinding = default(ProxyBinding);
			return false;
		}
		if (!compositeInstance.isRebindable)
		{
			newBinding = default(ProxyBinding);
			return false;
		}
		if (!compositeInstance.isModifiersRebindable && TryGetModifierBindings(action.sourceAction, compositeInstance, compositeIterator, bindingIterator, PathType.Original, componentData, out var _, out var originalModifiers) && !ProxyBinding.ModifiersListComparer.defaultComparer.Equals(bindingToSet.modifiers, (IReadOnlyCollection<ProxyModifier>)(object)originalModifiers))
		{
			newBinding = default(ProxyBinding);
			return false;
		}
		if (string.IsNullOrEmpty(bindingToSet.path) && !compositeInstance.canBeEmpty)
		{
			newBinding = default(ProxyBinding);
			return false;
		}
		if (!TrySetMainBinding(bindingToSet, action.sourceAction, bindingIterator, out var changed) || !TrySetModifierBindings(bindingToSet, action.sourceAction, compositeInstance, componentData, compositeIterator, bindingIterator, out var changed2))
		{
			newBinding = default(ProxyBinding);
			return false;
		}
		if (!changed && !changed2)
		{
			newBinding = default(ProxyBinding);
			return false;
		}
		action.Update();
		return TryGetBinding(action.sourceAction, compositeIterator, bindingIterator, compositeInstance, componentData, PathType.Effective, BindingOptions.None, out newBinding);
	}

	private bool TrySetMainBinding(ProxyBinding bindingToSet, InputAction action, BindingSyntax bindingIterator, out bool changed)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		InputBinding binding = ((BindingSyntax)(ref bindingIterator)).binding;
		if (bindingToSet.path == ((InputBinding)(ref binding)).path)
		{
			if (((InputBinding)(ref binding)).overridePath != null)
			{
				((InputBinding)(ref binding)).overridePath = null;
				InputActionRebindingExtensions.ApplyBindingOverride(action.actionMap, bindingIterator.m_BindingIndexInMap, binding);
				changed = true;
				return true;
			}
		}
		else if (bindingToSet.path != ((InputBinding)(ref binding)).overridePath)
		{
			((InputBinding)(ref binding)).overridePath = bindingToSet.path;
			InputActionRebindingExtensions.ApplyBindingOverride(action.actionMap, bindingIterator.m_BindingIndexInMap, binding);
			changed = true;
			return true;
		}
		changed = false;
		return true;
	}

	private bool TrySetModifierBindings(ProxyBinding bindingToSet, InputAction action, CompositeInstance compositeInstance, CompositeComponentData componentData, BindingSyntax compositeIterator, BindingSyntax bindingIterator, out bool changed)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!compositeInstance.allowModifiers)
		{
			changed = false;
			return true;
		}
		Dictionary<DeviceType, HashSet<string>> dictionary = kModifiers;
		InputBinding binding = ((BindingSyntax)(ref compositeIterator)).binding;
		if (!dictionary.TryGetValue(((InputBinding)(ref binding)).name.ToDeviceType(), out var supportedModifiers))
		{
			supportedModifiers = new HashSet<string>();
		}
		bool changedModifier = false;
		IReadOnlyList<ProxyModifier> modifiers = bindingToSet.modifiers;
		action.ForEachPartOfCompositeWithName(compositeIterator, componentData.m_ModifierName, delegate(BindingSyntax modifierIterator)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			InputBinding modifierBinding = ((BindingSyntax)(ref modifierIterator)).binding;
			if (string.IsNullOrEmpty(((InputBinding)(ref modifierBinding)).path))
			{
				return true;
			}
			if (!supportedModifiers.Contains(((InputBinding)(ref modifierBinding)).path))
			{
				return true;
			}
			bool allow = modifiers.Any((ProxyModifier m) => StringComparer.OrdinalIgnoreCase.Equals(m.m_Path, ((InputBinding)(ref modifierBinding)).path));
			changedModifier |= TrySetBindingModifierProcessor(action, modifierIterator, allow);
			return true;
		}, out var _);
		changed = changedModifier;
		return true;
	}

	private bool TrySetBindingModifierProcessor(InputAction action, BindingSyntax modifierIterator, bool allow)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		InputBinding binding = ((BindingSyntax)(ref modifierIterator)).binding;
		string text;
		if (allow)
		{
			if (string.IsNullOrEmpty(((InputBinding)(ref binding)).effectiveProcessors) || ((InputBinding)(ref binding)).effectiveProcessors == prohibitionModifierProcessor)
			{
				text = string.Empty;
			}
			else
			{
				string[] source = ((InputBinding)(ref binding)).effectiveProcessors.Split(';', StringSplitOptions.RemoveEmptyEntries);
				text = string.Join(";", source.Select((string p) => p != prohibitionModifierProcessor));
			}
		}
		else if (string.IsNullOrEmpty(((InputBinding)(ref binding)).effectiveProcessors) || ((InputBinding)(ref binding)).effectiveProcessors == prohibitionModifierProcessor)
		{
			text = prohibitionModifierProcessor;
		}
		else
		{
			string[] source2 = ((InputBinding)(ref binding)).effectiveProcessors.Split(';', StringSplitOptions.RemoveEmptyEntries);
			text = (source2.Any((string p) => p == prohibitionModifierProcessor) ? ((InputBinding)(ref binding)).effectiveProcessors : string.Join(";", source2.Append(prohibitionModifierProcessor)));
		}
		if (text == ((InputBinding)(ref binding)).processors)
		{
			if (((InputBinding)(ref binding)).overrideProcessors != null)
			{
				((InputBinding)(ref binding)).overrideProcessors = null;
				InputActionRebindingExtensions.ApplyBindingOverride(action.actionMap, modifierIterator.m_BindingIndexInMap, binding);
				return true;
			}
		}
		else if (text != ((InputBinding)(ref binding)).overrideProcessors)
		{
			((InputBinding)(ref binding)).overrideProcessors = text;
			InputActionRebindingExtensions.ApplyBindingOverride(action.actionMap, modifierIterator.m_BindingIndexInMap, binding);
			return true;
		}
		return false;
	}

	public void ResetAllBindings(bool onlyBuiltIn = true)
	{
		List<ProxyBinding> bindings = GetBindings(PathType.Original, (BindingOptions)(4 | (onlyBuiltIn ? 8 : 0)));
		SetBindings(bindings, out var _);
	}

	private bool TryGetIterators(ProxyBinding bindingSample, InputAction action, out BindingSyntax compositeIterator, out BindingSyntax bindingIterator, out CompositeInstance compositeInstance, out CompositeComponentData componentData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		compositeIterator = default(BindingSyntax);
		bindingIterator = default(BindingSyntax);
		compositeInstance = null;
		componentData = default(CompositeComponentData);
		if (!action.TryGetCompositeOfActionWithName(bindingSample.device.ToString(), out compositeIterator))
		{
			return false;
		}
		if (!TryGetCompositeInstance(compositeIterator, out compositeInstance))
		{
			return false;
		}
		if (bindingSample.component == ActionComponent.None)
		{
			if (!compositeInstance.compositeData.TryFindByBindingName(bindingSample.name, out componentData))
			{
				return false;
			}
		}
		else if (!compositeInstance.compositeData.TryGetData(bindingSample.component, out componentData))
		{
			return false;
		}
		bindingIterator = ((BindingSyntax)(ref compositeIterator)).NextPartBinding(componentData.m_BindingName);
		return ((BindingSyntax)(ref bindingIterator)).valid;
	}

	public void ResetGroupBindings(DeviceType device, bool onlyBuiltIn = true)
	{
		List<ProxyBinding> bindings = GetBindings(PathType.Original, (BindingOptions)(4 | (onlyBuiltIn ? 8 : 0)));
		SetBindings(bindings.Where((ProxyBinding b) => b.device == device), out var _);
	}

	internal ProxyBinding.Watcher GetOrCreateBindingWatcher(ProxyBinding binding)
	{
		if (!m_ProxyBindingWatchers.TryGetValue(binding, out var value))
		{
			value = new ProxyBinding.Watcher(binding);
			if (value.isValid)
			{
				m_ProxyBindingWatchers[binding] = value;
			}
		}
		return value;
	}

	public List<ProxyComposite> GetComposites(InputAction action)
	{
		List<ProxyComposite> composites = new List<ProxyComposite>();
		action.ForEachCompositeOfAction(delegate(BindingSyntax iterator)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			if (TryGetComposite(action, iterator, PathType.Effective, BindingOptions.None, out var proxyComposite))
			{
				composites.Add(proxyComposite);
			}
			return true;
		});
		return composites;
	}

	private bool TryGetComposite(InputAction action, BindingSyntax compositeIterator, PathType pathType, BindingOptions bindingOptions, out ProxyComposite proxyComposite)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		List<ProxyBinding> bindingsList = new List<ProxyBinding>();
		proxyComposite = null;
		if (!TryGetCompositeInstance(compositeIterator, out var compositeInstance))
		{
			return false;
		}
		if (compositeInstance.developerOnly && (Object)(object)GameManager.instance != (Object)null && !GameManager.instance.configuration.developerMode)
		{
			return false;
		}
		if (!PlatformExt.IsPlatformSet(compositeInstance.platform, Application.platform, false))
		{
			return false;
		}
		if (!compositeInstance.builtIn && (bindingOptions & BindingOptions.OnlyBuiltIn) != BindingOptions.None)
		{
			return false;
		}
		if (!compositeInstance.isRebindable && (bindingOptions & BindingOptions.OnlyRebindable) != BindingOptions.None)
		{
			return false;
		}
		if (compositeInstance.isDummy && (bindingOptions & BindingOptions.ExcludeDummy) != BindingOptions.None)
		{
			return false;
		}
		if (compositeInstance.isHidden && (bindingOptions & BindingOptions.ExcludeHidden) != BindingOptions.None)
		{
			return false;
		}
		foreach (CompositeComponentData componentData in compositeInstance.compositeData.m_Data.Values)
		{
			action.ForEachPartOfCompositeWithName(compositeIterator, componentData.m_BindingName, delegate(BindingSyntax bindingIterator)
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				if (TryGetBinding(action, compositeIterator, bindingIterator, compositeInstance, componentData, pathType, bindingOptions, out var foundBinding))
				{
					bindingsList.Add(foundBinding);
				}
				return true;
			}, out var _);
		}
		if (bindingsList.Count == 0)
		{
			return false;
		}
		InputBinding binding = ((BindingSyntax)(ref compositeIterator)).binding;
		proxyComposite = new ProxyComposite(((InputBinding)(ref binding)).name.ToDeviceType(), compositeInstance.compositeData.m_ActionType, compositeInstance, bindingsList);
		return true;
	}

	private bool TryGetCompositeInstance(BindingSyntax compositeIterator, out CompositeInstance compositeInstance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		InputBinding binding = ((BindingSyntax)(ref compositeIterator)).binding;
		NameAndParameters[] array = NameAndParameters.ParseMultiple(((InputBinding)(ref binding)).effectivePath).ToArray();
		if (array.Length == 2 && ((NameAndParameters)(ref array[1])).name == "Usages")
		{
			compositeInstance = new CompositeInstance(array[0], array[1]);
		}
		else
		{
			compositeInstance = new CompositeInstance(array[0]);
		}
		return compositeInstance != null;
	}

	public static string GeneratePathForControl(InputControl control)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		InputDevice device = control.device;
		Debug.Assert((object)control != device, "Control must not be a device");
		InternedString val = ((Collection)(ref InputControlLayout.s_Layouts)).FindLayoutThatIntroducesControl(control, m_LayoutCache);
		if (m_PathBuilder == null)
		{
			m_PathBuilder = new StringBuilder();
		}
		m_PathBuilder.Length = 0;
		InputControlExtensions.BuildPath(control, InternedString.op_Implicit(val), m_PathBuilder);
		return m_PathBuilder.ToString();
	}

	internal static bool TryGetCompositeData(string name, out CompositeData data)
	{
		for (int i = 0; i < m_Composites.Count; i++)
		{
			if (m_Composites[i].m_TypeName == name)
			{
				data = m_Composites[i];
				return true;
			}
		}
		data = default(CompositeData);
		return false;
	}

	internal static bool TryGetCompositeData(ActionType actionType, out CompositeData data)
	{
		return TryGetCompositeData(actionType.GetCompositeTypeName(), out data);
	}

	public static string GetBindingName(ActionComponent component)
	{
		if (TryGetCompositeData(component.GetActionType(), out var data) && data.TryGetData(component, out var data2))
		{
			return data2.m_BindingName;
		}
		return CompositeComponentData.defaultData.m_BindingName;
	}

	public static string GetModifierName(ActionComponent component)
	{
		if (TryGetCompositeData(component.GetActionType(), out var data) && data.TryGetData(component, out var data2))
		{
			return data2.m_ModifierName;
		}
		return CompositeComponentData.defaultData.m_ModifierName;
	}

	public static void CreateInstance()
	{
		s_Instance = new InputManager();
		s_Instance.Initialize();
	}

	public static void DestroyInstance()
	{
		s_Instance?.Dispose();
		s_Instance = null;
	}

	public InputManager()
	{
		log.Debug((object)"Creating InputManager");
		OnFocusChanged(Application.isFocused);
		m_ActionAsset = Resources.Load<InputActionAsset>("Input/InputActions");
		m_UIActionCollection = Resources.Load<UIInputActionCollection>("Input/UI Input Actions");
		m_ToolActionCollection = Resources.Load<UIInputActionCollection>("Input/Tool Input Actions");
		InputActionMap[] actionMaps = m_ActionAsset.m_ActionMaps;
		foreach (InputActionMap obj in actionMaps)
		{
			obj.m_Asset = null;
			ProxyActionMap proxyActionMap = new ProxyActionMap(obj);
			m_Maps.Add(proxyActionMap.name, proxyActionMap);
		}
	}

	public void Dispose()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		log.Debug((object)"Disposing InputManager");
		InputUser val = inputUser;
		if (((InputUser)(ref val)).valid)
		{
			val = inputUser;
			((InputUser)(ref val)).UnpairDevicesAndRemoveUser();
		}
		InputSystem.onDeviceChange -= OnDeviceChange;
		InputDevice val2 = default(InputDevice);
		DeviceListener deviceListener = default(DeviceListener);
		foreach (KeyValuePair<InputDevice, DeviceListener> deviceListener2 in m_DeviceListeners)
		{
			deviceListener2.Deconstruct(ref val2, ref deviceListener);
			deviceListener.StopListening();
		}
		PlatformManager.instance.onDeviceAssociationChanged -= new OnDeviceAssociationChangedEventHandler(OnDeviceAssociationChanged);
		PlatformManager.instance.onOverlayStateChanged -= new OnOverlayStateChanged(OnOverlayStateChanged);
	}

	public unsafe void Update()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!m_OverlayActive)
		{
			InputDevice val = default(InputDevice);
			DeviceListener deviceListener = default(DeviceListener);
			foreach (KeyValuePair<InputDevice, DeviceListener> deviceListener2 in m_DeviceListeners)
			{
				deviceListener2.Deconstruct(ref val, ref deviceListener);
				deviceListener.Tick();
			}
		}
		if (m_ActiveControlScheme == ControlScheme.KeyboardAndMouse)
		{
			Mouse current = Mouse.current;
			if (current != null)
			{
				Vector2 value = System.Runtime.CompilerServices.Unsafe.Read<Vector2>((void*)((InputControl<Vector2>)(object)((Pointer)current).delta).value);
				if (((Vector2)(ref value)).magnitude > 0.2f)
				{
					m_AccumulatedIdleDelay = 0f;
					if (m_Idle)
					{
						m_Idle = false;
						Telemetry.InputIdleEnd();
					}
				}
			}
		}
		if (!m_Idle)
		{
			if (GameManager.instance.state == GameManager.State.WorldReady)
			{
				if (m_WasWorldReady)
				{
					m_AccumulatedIdleDelay += Time.unscaledDeltaTime;
				}
				m_WasWorldReady = true;
			}
			else
			{
				m_AccumulatedIdleDelay = 0f;
				m_WasWorldReady = false;
			}
			if (m_AccumulatedIdleDelay >= 30f)
			{
				m_AccumulatedIdleDelay = 30f;
				m_Idle = true;
				log.Debug((object)"Input idle");
				Telemetry.InputIdleStart();
			}
		}
		m_ConflictResolution.Update();
		RefreshActiveControl();
	}

	private void UpdateCursorVisibility()
	{
		Cursor.visible = activeControlScheme == ControlScheme.KeyboardAndMouse && !hideCursor;
	}

	internal void CheckConflicts()
	{
		if ((Object)(object)GameManager.instance != (Object)null && GameManager.instance.state < GameManager.State.UIReady)
		{
			return;
		}
		bindingConflicts = DeviceType.None;
		string text = default(string);
		ProxyActionMap proxyActionMap = default(ProxyActionMap);
		ProxyAction proxyAction = default(ProxyAction);
		DeviceType deviceType = default(DeviceType);
		ProxyComposite proxyComposite = default(ProxyComposite);
		ActionComponent actionComponent = default(ActionComponent);
		ProxyBinding proxyBinding = default(ProxyBinding);
		foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
		{
			map.Deconstruct(ref text, ref proxyActionMap);
			ProxyActionMap proxyActionMap2 = proxyActionMap;
			bool flag = false;
			foreach (KeyValuePair<string, ProxyAction> action in proxyActionMap2.actions)
			{
				action.Deconstruct(ref text, ref proxyAction);
				ProxyAction proxyAction2 = proxyAction;
				if ((proxyAction2.availableDevices & ~bindingConflicts) == 0)
				{
					continue;
				}
				foreach (KeyValuePair<DeviceType, ProxyComposite> composite in proxyAction2.composites)
				{
					composite.Deconstruct(ref deviceType, ref proxyComposite);
					ProxyComposite proxyComposite2 = proxyComposite;
					if ((proxyComposite2.m_Device & ~bindingConflicts) == 0)
					{
						continue;
					}
					foreach (KeyValuePair<ActionComponent, ProxyBinding> binding in proxyComposite2.bindings)
					{
						binding.Deconstruct(ref actionComponent, ref proxyBinding);
						ProxyBinding proxyBinding2 = proxyBinding;
						if ((proxyBinding2.hasConflicts & ProxyBinding.ConflictType.WithBuiltIn) != ProxyBinding.ConflictType.None)
						{
							if (proxyBinding2.isBuiltIn)
							{
								bindingConflicts |= proxyBinding2.device;
							}
							else
							{
								flag = true;
							}
						}
					}
				}
				if (bindingConflicts == DeviceType.All && flag)
				{
					break;
				}
			}
			SetModConflictNotification(proxyActionMap2, flag);
		}
		SetBuiltInConflictNotification(bindingConflicts != DeviceType.None);
	}

	private void SetBuiltInConflictNotification(bool conflict)
	{
		if (conflict == NotificationSystem.Exist("KeyBindingConflict"))
		{
			return;
		}
		if (conflict)
		{
			ProgressState? progressState = (ProgressState)5;
			NotificationSystem.Push("KeyBindingConflict", null, null, "KeyBindingConflict", "KeyBindingConflict", null, progressState, null, delegate
			{
				LocalizedString value = LocalizedString.Id("Common.DIALOG_TITLE_INPUT");
				LocalizedString message = LocalizedString.Id("Common.DIALOG_MESSAGE_INPUT");
				LocalizedString confirmAction = LocalizedString.Id("Common.OK");
				LocalizedString localizedString = LocalizedString.Id("Common.DIALOG_ACTION_INPUT[Reset]");
				LocalizedString localizedString2 = LocalizedString.Id("Common.DIALOG_ACTION_INPUT[OpenOptions]");
				MessageDialog dialog = new MessageDialog(value, message, confirmAction, localizedString, localizedString2);
				GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog, Callback);
			});
		}
		else
		{
			ProgressState? progressState = (ProgressState)3;
			NotificationSystem.Pop("KeyBindingConflict", 2f, null, null, null, "KeyBindingConflictResolved", null, progressState);
		}
		void Callback(int msg)
		{
			switch (msg)
			{
			case 0:
				NotificationSystem.Pop("KeyBindingConflict");
				break;
			case 2:
				ResetAllBindings();
				break;
			case 3:
			{
				OptionsUISystem orCreateSystemManaged = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<OptionsUISystem>();
				string sectionID = bindingConflicts switch
				{
					DeviceType.Keyboard => "Keyboard", 
					DeviceType.Mouse => "Mouse", 
					DeviceType.Gamepad => "Gamepad", 
					DeviceType.Keyboard | DeviceType.Mouse => "Keyboard", 
					DeviceType.Keyboard | DeviceType.Gamepad => (activeControlScheme == ControlScheme.Gamepad) ? "Gamepad" : "Keyboard", 
					DeviceType.Mouse | DeviceType.Gamepad => (activeControlScheme == ControlScheme.Gamepad) ? "Gamepad" : "Mouse", 
					DeviceType.All => (activeControlScheme == ControlScheme.Gamepad) ? "Gamepad" : "Keyboard", 
					_ => null, 
				};
				orCreateSystemManaged?.OpenPage("Input", sectionID, isAdvanced: false);
				break;
			}
			case 1:
				break;
			}
		}
	}

	private void SetModConflictNotification(ProxyActionMap map, bool conflict)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (conflict == NotificationSystem.Exist(map.name))
		{
			return;
		}
		if (conflict)
		{
			string text = null;
			Action action = null;
			LocalizedString value = LocalizedString.IdWithFallback("Options.INPUT_MAP[" + map.name + "]", map.name);
			if (ModSetting.instances.TryGetValue(map.name, out var value2) && GameManager.instance.modManager.TryGetExecutableAsset(value2.mod, out var asset))
			{
				value = LocalizedString.Value(asset.mod.displayName);
				if (!string.IsNullOrEmpty(asset.mod.thumbnailPath))
				{
					text = $"{asset.mod.thumbnailPath}?width={NotificationUISystem.width})";
				}
				action = delegate
				{
					World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<OptionsUISystem>()?.OpenPage(map.name, null, isAdvanced: false);
				};
			}
			string name = map.name;
			LocalizedString? title = value;
			string thumbnail = text;
			ProgressState? progressState = (ProgressState)5;
			Action onClicked = action;
			NotificationSystem.Push(name, title, null, null, "KeyBindingConflict", thumbnail, progressState, null, onClicked);
		}
		else
		{
			string name2 = map.name;
			ProgressState? progressState = (ProgressState)3;
			NotificationSystem.Pop(name2, 2f, null, null, null, "KeyBindingConflictResolved", null, progressState);
		}
	}

	public void OnFocusChanged(bool hasFocus)
	{
		log.VerboseFormat("Has focus {0}", (object)hasFocus);
		m_HasFocus = hasFocus;
	}

	private void OnOverlayStateChanged(IOverlaySupport psi, bool active)
	{
		log.VerboseFormat("Overlay active {0}", (object)active);
		m_OverlayActive = active;
		if (!active)
		{
			return;
		}
		ReadOnlyArray<InputDevice>? val = ((DeviceArray)(ref m_Devices)).Get();
		if (!val.HasValue)
		{
			return;
		}
		foreach (Keyboard item in ((IEnumerable)(object)val).OfType<Keyboard>())
		{
			InputSystem.ResetDevice((InputDevice)(object)item, false);
		}
	}

	bool IInputActionCollection.Contains(InputAction action)
	{
		InputActionMap sourceMap = ((action != null) ? action.actionMap : null);
		if (sourceMap != null)
		{
			return m_Maps.Any((KeyValuePair<string, ProxyActionMap> m) => m.Value.sourceMap == sourceMap);
		}
		return false;
	}

	void IInputActionCollection.Enable()
	{
		string text = default(string);
		ProxyActionMap proxyActionMap = default(ProxyActionMap);
		foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
		{
			map.Deconstruct(ref text, ref proxyActionMap);
			proxyActionMap.sourceMap.Enable();
		}
	}

	void IInputActionCollection.Disable()
	{
		string text = default(string);
		ProxyActionMap proxyActionMap = default(ProxyActionMap);
		foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
		{
			map.Deconstruct(ref text, ref proxyActionMap);
			proxyActionMap.sourceMap.Disable();
		}
	}

	IEnumerator<InputAction> IEnumerable<InputAction>.GetEnumerator()
	{
		return m_Maps.SelectMany((KeyValuePair<string, ProxyActionMap> map) => (IEnumerable<InputAction>)(object)map.Value.sourceMap.actions).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<InputAction>)this).GetEnumerator();
	}

	internal void OnEnabledActionsChanged()
	{
		this.EventEnabledActionsChanged?.Invoke();
	}

	internal void OnActionMasksChanged()
	{
		this.EventActionMasksChanged?.Invoke();
	}

	internal void OnActionDisplayNamesChanged()
	{
		this.EventActionDisplayNamesChanged?.Invoke();
	}

	internal void OnPreResolvedActionChanged()
	{
		this.EventPreResolvedActionChanged?.Invoke();
	}

	internal static DeferManagerUpdatingWrapper DeferUpdating()
	{
		sDeferUpdatingWrapper.Acquire();
		return sDeferUpdatingWrapper;
	}

	public void SetDefaultControlScheme()
	{
		activeControlScheme = ControlScheme.KeyboardAndMouse;
	}

	private void OnDeviceChange(InputDevice device, InputDeviceChange change)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		if ((int)change != 0)
		{
			if ((int)change == 1)
			{
				OnRemoveDevice(device);
			}
		}
		else
		{
			OnAddDevice(device);
		}
	}

	private void OnAddDevice(InputDevice device)
	{
		if (!m_DeviceListeners.TryGetValue(device, out var value))
		{
			value = new DeviceListener(device, 50f);
			((UnityEvent<InputDevice>)value.EventDeviceActivated).AddListener((UnityAction<InputDevice>)OnDeviceActivated);
			m_DeviceListeners.Add(device, value);
			value.StartListening();
		}
		value.StartListening();
		TryPairDevice(device);
	}

	private void OnRemoveDevice(InputDevice device)
	{
		if (m_DeviceListeners.TryGetValue(device, out var value))
		{
			value.StopListening();
		}
		if (TryUnpairDevice(device) && ((activeControlScheme == ControlScheme.KeyboardAndMouse && (device is Keyboard || device is Mouse)) || (activeControlScheme == ControlScheme.Gamepad && device is Gamepad)))
		{
			this.EventActiveDeviceDisconnected?.Invoke();
		}
	}

	private void OnDeviceActivated(InputDevice newDevice)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (newDevice != m_LastActiveDevice)
		{
			InputDevice lastActiveDevice = m_LastActiveDevice;
			ControlScheme controlScheme = activeControlScheme;
			m_LastActiveDevice = newDevice;
			if (!(newDevice is Mouse) && !(newDevice is Keyboard))
			{
				if (newDevice is Gamepad)
				{
					activeControlScheme = ControlScheme.Gamepad;
				}
			}
			else
			{
				activeControlScheme = ControlScheme.KeyboardAndMouse;
			}
			this.EventActiveDeviceChanged?.Invoke(newDevice, lastActiveDevice, activeControlScheme != controlScheme);
		}
		if (m_Idle)
		{
			m_Idle = false;
			Telemetry.InputIdleEnd();
		}
		m_AccumulatedIdleDelay = 0f;
		InputUser val = inputUser;
		if (!((IEnumerable<InputDevice>)(object)((InputUser)(ref val)).pairedDevices).Contains(newDevice))
		{
			OnUnpairedDeviceUsed(newDevice);
		}
	}

	private void OnUnpairedDeviceUsed(InputDevice device)
	{
		if (!(device is Mouse))
		{
			if (!(device is Keyboard))
			{
				if (device is Gamepad)
				{
					if (!PlatformManager.instance.IsDeviceAssociated(device))
					{
						return;
					}
					UnpairAll<Gamepad>();
				}
			}
			else
			{
				UnpairAll<Keyboard>();
			}
		}
		else
		{
			UnpairAll<Mouse>();
		}
		PairDevice(device);
	}

	private void OnDeviceAssociationChanged(IPlatformServiceIntegration psi, DeviceAssociationChange change)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!PlatformManager.instance.IsPrincipalDeviceAssociationIntegration(psi))
		{
			return;
		}
		InputDevice val = ((IEnumerable<InputDevice>)(object)InputSystem.devices).FirstOrDefault((InputDevice device) => device.deviceId == change.deviceId) ?? ((IEnumerable<InputDevice>)(object)InputSystem.disconnectedDevices).FirstOrDefault((InputDevice device) => device.deviceId == change.deviceId);
		if (val != null)
		{
			if (!change.associated)
			{
				if (TryUnpairDevice(val))
				{
					this.EventActiveDeviceAssociationLost?.Invoke();
				}
			}
			else if (change.associated)
			{
				TryPairDevice(val);
			}
		}
		else
		{
			log.Error((object)$"No matching device found with ID: {change.deviceId}.");
		}
	}

	public void AddInitialDevices()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<InputDevice> enumerator = InputSystem.devices.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				InputDevice current = enumerator.Current;
				OnAddDevice(current);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private bool TryPairDevice(InputDevice device)
	{
		if ((device is Mouse && !IsDeviceTypePaired<Mouse>()) || (device is Keyboard && !IsDeviceTypePaired<Keyboard>()) || (device is Gamepad && !IsDeviceTypePaired<Gamepad>() && PlatformManager.instance.IsDeviceAssociated(device)))
		{
			PairDevice(device);
			return true;
		}
		return false;
	}

	private void PairDevice(InputDevice device)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		log.InfoFormat("Pair {0} [{1}]", (object)((InputControl)device).displayName, (object)device.deviceId);
		InputUser.PerformPairingWithDevice(device, inputUser, (InputUserPairingOptions)0);
		this.EventDevicePaired?.Invoke();
		UpdateConnectedDeviceTypes();
	}

	private bool TryUnpairDevice(InputDevice device)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		InputUser val = inputUser;
		if (!((IEnumerable<InputDevice>)(object)((InputUser)(ref val)).pairedDevices).Contains(device))
		{
			val = inputUser;
			if (!((IEnumerable<InputDevice>)(object)((InputUser)(ref val)).lostDevices).Contains(device))
			{
				return false;
			}
		}
		UnpairDevice(device);
		return true;
	}

	private void UnpairDevice(InputDevice device)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		log.InfoFormat("Unpair {0} [{1}]", (object)((InputControl)device).displayName, (object)device.deviceId);
		InputUser val = inputUser;
		((InputUser)(ref val)).UnpairDevice(device);
		UpdateConnectedDeviceTypes();
	}

	private void UnpairAll<T>() where T : InputDevice
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		InputUser val = inputUser;
		foreach (InputDevice item in ((IEnumerable<InputDevice>)(object)((InputUser)(ref val)).pairedDevices).Where((InputDevice x) => x is T))
		{
			UnpairDevice(item);
		}
		val = inputUser;
		foreach (InputDevice item2 in ((IEnumerable<InputDevice>)(object)((InputUser)(ref val)).lostDevices).Where((InputDevice x) => x is T))
		{
			UnpairDevice(item2);
		}
	}

	private bool IsDeviceTypePaired<T>() where T : InputDevice
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		InputUser val = inputUser;
		if (((IEnumerable<InputDevice>)(object)((InputUser)(ref val)).pairedDevices).Any((InputDevice d) => d is T))
		{
			return true;
		}
		val = inputUser;
		if (((IEnumerable<InputDevice>)(object)((InputUser)(ref val)).lostDevices).Any((InputDevice d) => d is T))
		{
			return true;
		}
		return false;
	}

	public static bool IsGamepadActive()
	{
		return instance.activeControlScheme == ControlScheme.Gamepad;
	}

	private void UpdateConnectedDeviceTypes()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		InputUser val = inputUser;
		m_ConnectedDeviceTypes = ((IEnumerable<InputDevice>)(object)((InputUser)(ref val)).pairedDevices).Aggregate(DeviceType.None, delegate(DeviceType result, InputDevice device)
		{
			DeviceType deviceType = ((device is Keyboard) ? DeviceType.Keyboard : ((device is Mouse) ? DeviceType.Mouse : ((device is Gamepad) ? DeviceType.Gamepad : DeviceType.None)));
			return result | deviceType;
		});
	}

	public GamepadType GetActiveGamepadType()
	{
		return GetGamepadType(Gamepad.current);
	}

	public GamepadType GetGamepadType(Gamepad gamepad)
	{
		if (!(gamepad is DualShockGamepad))
		{
			if (gamepad is XInputController)
			{
				return GamepadType.Xbox;
			}
			return GamepadType.Xbox;
		}
		return GamepadType.PS;
	}

	public void Initialize()
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		m_DeviceListeners = new Dictionary<InputDevice, DeviceListener>();
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			log.InfoFormat("Input initialized in {0}ms", (object)t.TotalMilliseconds);
		});
		try
		{
			using (DeferUpdating())
			{
				InitializeComposites();
				InitializeModifiers();
				string text = default(string);
				ProxyActionMap proxyActionMap = default(ProxyActionMap);
				foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
				{
					map.Deconstruct(ref text, ref proxyActionMap);
					proxyActionMap.InitActions();
				}
				InitializeMasks();
				InitializeAliases();
				InitializeLinkedActions();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		inputUser = InputUser.CreateUserWithoutPairedDevices();
		AssociateActionsWithUser(associate: true);
		InputSystem.onDeviceChange += OnDeviceChange;
		AddInitialDevices();
		PlatformManager.instance.onDeviceAssociationChanged += new OnDeviceAssociationChangedEventHandler(OnDeviceAssociationChanged);
		PlatformManager.instance.onOverlayStateChanged += new OnOverlayStateChanged(OnOverlayStateChanged);
		m_ConflictResolution.Initialize();
	}

	private void InitializeComposites()
	{
		m_Composites = new List<CompositeData>
		{
			AxisSeparatedWithModifiersComposite.GetCompositeData(),
			AxisWithModifiersComposite.GetCompositeData(),
			ButtonWithModifiersComposite.GetCompositeData(),
			CameraVector2WithModifiersComposite.GetCompositeData(),
			Vector2SeparatedWithModifiersComposite.GetCompositeData(),
			Vector2WithModifiersComposite.GetCompositeData()
		};
	}

	private void InitializeModifiers()
	{
		foreach (InputAction action in m_Maps.Values.SelectMany((ProxyActionMap map) => (IEnumerable<InputAction>)(object)map.sourceMap.actions))
		{
			action.ForEachCompositeOfAction(delegate(BindingSyntax iterator)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				InputBinding binding = ((BindingSyntax)(ref iterator)).binding;
				CompositeInstance compositeInstance = new CompositeInstance(NameAndParameters.Parse(((InputBinding)(ref binding)).effectivePath));
				InitializeModifiers(iterator, action, compositeInstance);
				return true;
			});
		}
	}

	private void InitializeModifiers(BindingSyntax compositeIterator, InputAction action, CompositeInstance compositeInstance)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!compositeInstance.allowModifiers)
		{
			return;
		}
		foreach (CompositeComponentData componentData in compositeInstance.compositeData.m_Data.Values)
		{
			action.ForEachPartOfCompositeWithName(compositeIterator, componentData.m_BindingName, delegate(BindingSyntax mainIterator)
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				InputBinding binding = ((BindingSyntax)(ref mainIterator)).binding;
				Dictionary<DeviceType, HashSet<string>> dictionary = kModifiers;
				InputBinding binding2 = ((BindingSyntax)(ref compositeIterator)).binding;
				if (!dictionary.TryGetValue(((InputBinding)(ref binding2)).name.ToDeviceType(), out var value))
				{
					return true;
				}
				HashSet<string> missedModifiers = new HashSet<string>(value, StringComparer.OrdinalIgnoreCase);
				action.ForEachPartOfCompositeWithName(mainIterator, componentData.m_ModifierName, delegate(BindingSyntax modifierIterator)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					InputBinding binding3 = ((BindingSyntax)(ref modifierIterator)).binding;
					if (!string.Equals(((InputBinding)(ref binding3)).name, componentData.m_ModifierName, StringComparison.Ordinal))
					{
						return true;
					}
					if (string.IsNullOrEmpty(((InputBinding)(ref binding3)).path))
					{
						return true;
					}
					missedModifiers.Remove(((InputBinding)(ref binding3)).path);
					return true;
				}, out var endIterator2);
				foreach (string item in missedModifiers)
				{
					BindingSyntax val = ((BindingSyntax)(ref endIterator2)).InsertPartBinding(componentData.m_ModifierName, item);
					val = ((BindingSyntax)(ref val)).WithGroups(((InputBinding)(ref binding)).groups);
					val = ((BindingSyntax)(ref val)).WithProcessor(prohibitionModifierProcessor);
					endIterator2 = ((BindingSyntax)(ref val)).Triggering(action);
				}
				return true;
			}, out var _);
		}
	}

	private void InitializeMasks()
	{
		string text = default(string);
		ProxyActionMap proxyActionMap = default(ProxyActionMap);
		ProxyAction proxyAction = default(ProxyAction);
		foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
		{
			map.Deconstruct(ref text, ref proxyActionMap);
			foreach (KeyValuePair<string, ProxyAction> action2 in proxyActionMap.actions)
			{
				action2.Deconstruct(ref text, ref proxyAction);
				ProxyAction action = proxyAction;
				InitializeMasks(action);
			}
		}
	}

	internal unsafe void InitializeMasks(ProxyAction action)
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		InputAction sourceAction = action.sourceAction;
		string expectedControlType = sourceAction.expectedControlType;
		Type typeFromHandle;
		switch (expectedControlType)
		{
		default:
			if (expectedControlType.Length == 0)
			{
				goto case "Button";
			}
			goto case null;
		case "Dpad":
			typeFromHandle = typeof(MaskVector2Processor);
			break;
		case "Stick":
			typeFromHandle = typeof(MaskVector2Processor);
			break;
		case "Vector2":
			typeFromHandle = typeof(MaskVector2Processor);
			break;
		case "Axis":
			typeFromHandle = typeof(MaskFloatProcessor);
			break;
		case "Button":
			typeFromHandle = typeof(MaskFloatProcessor);
			break;
		case null:
			throw new ArgumentException("Unexpected type of control", "expectedControlType");
		}
		InternedString processorName = ((TypeTable)(ref InputProcessor.s_Processors)).FindNameForType(typeFromHandle);
		sourceAction.ForEachCompositeOfAction(delegate(BindingSyntax iterator)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			NameAndParameters val = default(NameAndParameters);
			((NameAndParameters)(ref val)).name = InternedString.op_Implicit(processorName);
			NamedValue[] obj = new NamedValue[2]
			{
				NamedValue.From<int>("m_Index", action.m_GlobalIndex),
				default(NamedValue)
			};
			InputBinding binding = ((BindingSyntax)(ref iterator)).binding;
			obj[1] = NamedValue.From<DeviceType>("m_Mask", ((InputBinding)(ref binding)).name.ToDeviceType());
			((NameAndParameters)(ref val)).parameters = new ReadOnlyArray<NamedValue>((NamedValue[])(object)obj);
			NameAndParameters val2 = val;
			ref InputBinding reference = ref sourceAction.m_ActionMap.m_Bindings[iterator.m_BindingIndexInMap];
			binding = ((BindingSyntax)(ref iterator)).binding;
			string processors;
			if (!string.IsNullOrEmpty(((InputBinding)(ref binding)).processors))
			{
				binding = ((BindingSyntax)(ref iterator)).binding;
				processors = string.Format("{0}{1}{2}", ((InputBinding)(ref binding)).processors, ",", val2);
			}
			else
			{
				processors = ((object)(*(NameAndParameters*)(&val2))/*cast due to .constrained prefix*/).ToString();
			}
			((InputBinding)(ref reference)).processors = processors;
			sourceAction.m_ActionMap.OnBindingModified();
			return true;
		});
	}

	private void InitializeAliases()
	{
		UIBaseInputAction[] inputActions = uiActionCollection.m_InputActions;
		foreach (UIBaseInputAction uIBaseInputAction in inputActions)
		{
			foreach (UIInputActionPart actionPart in uIBaseInputAction.actionParts)
			{
				if (actionPart.TryGetProxyAction(out var action))
				{
					action.m_UIAliases.Add(uIBaseInputAction);
				}
			}
		}
		inputActions = toolActionCollection.m_InputActions;
		foreach (UIBaseInputAction uIBaseInputAction2 in inputActions)
		{
			foreach (UIInputActionPart actionPart2 in uIBaseInputAction2.actionParts)
			{
				if (actionPart2.TryGetProxyAction(out var action2))
				{
					action2.m_UIAliases.Add(uIBaseInputAction2);
				}
			}
		}
	}

	private void InitializeLinkedActions()
	{
		string text = default(string);
		ProxyActionMap proxyActionMap = default(ProxyActionMap);
		ProxyAction proxyAction = default(ProxyAction);
		foreach (KeyValuePair<string, ProxyActionMap> map in m_Maps)
		{
			map.Deconstruct(ref text, ref proxyActionMap);
			foreach (KeyValuePair<string, ProxyAction> action2 in proxyActionMap.actions)
			{
				action2.Deconstruct(ref text, ref proxyAction);
				ProxyAction action = proxyAction;
				action.sourceAction.ForEachCompositeOfAction(delegate(BindingSyntax iterator)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_001c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_0029: Unknown result type (might be due to invalid IL or missing references)
					InputBinding binding = ((BindingSyntax)(ref iterator)).binding;
					DeviceType deviceType = ((InputBinding)(ref binding)).name.ToDeviceType();
					if (deviceType == DeviceType.None)
					{
						return true;
					}
					binding = ((BindingSyntax)(ref iterator)).binding;
					CompositeInstance compositeInstance = new CompositeInstance(NameAndParameters.Parse(((InputBinding)(ref binding)).effectivePath));
					if (compositeInstance.linkedGuid != Guid.Empty && TryFindAction(compositeInstance.linkedGuid, out var proxyAction2))
					{
						ProxyAction.LinkActions(new ProxyAction.LinkInfo
						{
							m_Action = action,
							m_Device = deviceType
						}, new ProxyAction.LinkInfo
						{
							m_Action = proxyAction2,
							m_Device = deviceType
						});
					}
					return true;
				});
			}
		}
	}

	internal void CreateCompositeBinding(InputAction action, ProxyComposite.Info info)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{info.m_Source.parameters}{';'}{info.m_Source.usages.parameters}";
		string text2 = string.Join(";", info.m_Source.interactions);
		string text3 = string.Join(";", info.m_Source.processors);
		CompositeSyntax val = InputActionSetupExtensions.AddCompositeBinding(action, text, text2, text3);
		BindingSyntax val2 = new BindingSyntax(action.m_ActionMap, action.BindingIndexOnActionToBindingIndexOnMap(((CompositeSyntax)(ref val)).bindingIndex), action);
		((BindingSyntax)(ref val2)).WithName(info.m_Device.ToString());
		foreach (ProxyBinding binding in info.m_Bindings)
		{
			if (!info.m_Source.compositeData.TryGetData(binding.component, out var data))
			{
				continue;
			}
			((CompositeSyntax)(ref val)).With(data.m_BindingName, binding.path, binding.device.ToString(), (string)null);
			if (!info.m_Source.allowModifiers || !kModifiers.TryGetValue(binding.device, out var value))
			{
				continue;
			}
			foreach (string supportedModifier in value)
			{
				string text4 = (binding.modifiers.Any((ProxyModifier m) => m.m_Path == supportedModifier) ? string.Empty : prohibitionModifierProcessor);
				((CompositeSyntax)(ref val)).With(data.m_ModifierName, supportedModifier, binding.device.ToString(), text4);
			}
		}
	}

	public InputBarrier CreateGlobalBarrier(string barrierName)
	{
		return new InputBarrier(barrierName, m_Maps.Values.ToArray());
	}

	public InputBarrier CreateOverlayBarrier(string barrierName)
	{
		ProxyActionMap[] maps = m_Maps.Values.Where((ProxyActionMap actionMap) => actionMap.name != "Engagement" && actionMap.name != "Splash screen").ToArray();
		return new InputBarrier(barrierName, maps, DeviceType.All, blocked: true);
	}

	public InputBarrier CreateMapBarrier(string map, string barrierName)
	{
		return new InputBarrier(barrierName, FindActionMap(map));
	}

	public InputBarrier CreateActionBarrier(string map, string name, string barrierName)
	{
		return new InputBarrier(barrierName, FindAction(map, name));
	}

	public ProxyActionMap FindActionMap(string name)
	{
		if (!m_Maps.TryGetValue(name, out var value))
		{
			return null;
		}
		return value;
	}

	public bool TryFindActionMap(string name, out ProxyActionMap map)
	{
		return m_Maps.TryGetValue(name, out map);
	}

	internal ProxyActionMap FindActionMap(InputActionMap map)
	{
		return FindActionMap((map != null) ? map.name : null);
	}

	internal bool TryFindActionMap(InputActionMap map, out ProxyActionMap proxyMap)
	{
		return TryFindActionMap(map.name, out proxyMap);
	}

	private ProxyActionMap AddActionMap(string name)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		using (DeferUpdating())
		{
			InputActionMap val = new InputActionMap(name);
			val.GenerateId();
			ProxyActionMap proxyActionMap = new ProxyActionMap(val);
			m_Maps.Add(proxyActionMap.name, proxyActionMap);
			return proxyActionMap;
		}
	}

	private ProxyActionMap GetOrCreateMap(string name)
	{
		if (!TryFindActionMap(name, out var map))
		{
			return AddActionMap(name);
		}
		return map;
	}

	public void AssociateActionsWithUser(bool associate)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		InputUser val = inputUser;
		if (((InputUser)(ref val)).valid)
		{
			if (associate)
			{
				val = inputUser;
				((InputUser)(ref val)).AssociateActionsWithUser((IInputActionCollection)(object)this);
			}
			else
			{
				val = inputUser;
				((InputUser)(ref val)).AssociateActionsWithUser((IInputActionCollection)null);
			}
		}
	}
}
