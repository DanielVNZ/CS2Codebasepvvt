using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.UI.Binding;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI;

public class InputHintBindings : CompositeBinding, IDisposable
{
	internal class InputHint : IJsonWritable
	{
		public readonly ProxyAction action;

		public readonly int version = InputManager.instance.actionVersion;

		public string name;

		public int priority;

		public bool show;

		public readonly List<InputHintItem> items = new List<InputHintItem>();

		public InputHint(ProxyAction action)
		{
			this.action = action;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().Name);
			writer.PropertyName("name");
			writer.Write(name);
			writer.PropertyName("items");
			JsonWriterExtensions.Write<InputHintItem>(writer, (IList<InputHintItem>)items);
			writer.PropertyName("show");
			writer.Write(show);
			writer.TypeEnd();
		}

		public static InputHint Create(ProxyAction action)
		{
			DisplayNameOverride displayOverride = action.displayOverride;
			if (displayOverride != null)
			{
				return new InputHint(action)
				{
					name = displayOverride.displayName,
					priority = displayOverride.priority,
					show = (displayOverride.active && displayOverride.priority > 0)
				};
			}
			return new InputHint(action)
			{
				name = action.title,
				priority = -1,
				show = false
			};
		}
	}

	internal class InputHintItem : IJsonWritable
	{
		public ControlPath[] bindings;

		public ControlPath[] modifiers;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().Name);
			writer.PropertyName("bindings");
			JsonWriterExtensions.Write<ControlPath>(writer, (IList<ControlPath>)bindings);
			writer.PropertyName("modifiers");
			JsonWriterExtensions.Write<ControlPath>(writer, (IList<ControlPath>)modifiers);
			writer.TypeEnd();
		}
	}

	private struct TutorialInputHintQuery : IJsonReadable, IJsonWritable
	{
		public string map;

		public string action;

		public int index;

		public InputManager.ControlScheme controlScheme;

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("map");
			reader.Read(ref map);
			reader.ReadProperty("action");
			reader.Read(ref action);
			reader.ReadProperty("controlScheme");
			int num = default(int);
			reader.Read(ref num);
			controlScheme = (InputManager.ControlScheme)num;
			reader.ReadProperty("index");
			reader.Read(ref index);
			reader.ReadMapEnd();
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(typeof(TutorialInputHintQuery).FullName);
			writer.PropertyName("map");
			writer.Write(map);
			writer.PropertyName("action");
			writer.Write(action);
			writer.PropertyName("controlScheme");
			writer.Write((int)controlScheme);
			writer.PropertyName("index");
			writer.Write(index);
			writer.TypeEnd();
		}
	}

	private struct InputHintQuery : IJsonReadable, IJsonWritable, IEquatable<InputHintQuery>
	{
		public string action;

		public InputManager.ControlScheme controlScheme;

		public void Read(IJsonReader reader)
		{
			reader.ReadMapBegin();
			reader.ReadProperty("action");
			reader.Read(ref action);
			reader.ReadProperty("controlScheme");
			int num = default(int);
			reader.Read(ref num);
			controlScheme = (InputManager.ControlScheme)num;
			reader.ReadMapEnd();
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(typeof(InputHintQuery).FullName);
			writer.PropertyName("action");
			writer.Write(action);
			writer.PropertyName("controlScheme");
			writer.Write((int)controlScheme);
			writer.TypeEnd();
		}

		public bool Equals(InputHintQuery other)
		{
			if (other.action == action)
			{
				return other.controlScheme == controlScheme;
			}
			return false;
		}
	}

	private const string kGroup = "input";

	private static readonly string[] axisControls = new string[3] { "<Gamepad>/leftStick", "<Gamepad>/rightStick", "<Gamepad>/dpad" };

	private static readonly string[] allDirs = new string[4] { "/up", "/down", "/left", "/right" };

	private static readonly string[] horizontal = new string[2] { "/left", "/right" };

	private static readonly string[] vertical = new string[2] { "/up", "/down" };

	private static readonly string[] axes = new string[2] { "/x", "/y" };

	private readonly ValueBinding<InputHint[]> m_ActiveHintsBinding;

	private readonly GetterMapBinding<InputHintQuery, InputHint> m_HintsMapBinding;

	private readonly ValueBinding<int> m_GamepadTypeBinding;

	private readonly GetterMapBinding<TutorialInputHintQuery, InputHint[]> m_TutorialHints;

	private Dictionary<(string name, int priority), InputHint> m_Hints = new Dictionary<(string, int), InputHint>();

	private bool m_HintsDirty = true;

	private bool m_TutorialHintsDirty = true;

	private static Dictionary<IReadOnlyList<ProxyModifier>, List<ProxyBinding>> modifiersGroups = new Dictionary<IReadOnlyList<ProxyModifier>, List<ProxyBinding>>(new ProxyBinding.ModifiersListComparer(ProxyModifier.pathComparer));

	public event Action<ProxyAction> onInputHintPerformed;

	public InputHintBindings()
	{
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_HintsMapBinding = new GetterMapBinding<InputHintQuery, InputHint>("input", "hints", (Func<InputHintQuery, InputHint>)GetInputHint, (IReader<InputHintQuery>)(object)new ValueReader<InputHintQuery>(), (IWriter<InputHintQuery>)(object)new ValueWriter<InputHintQuery>(), (IWriter<InputHint>)(object)new ValueWriter<InputHint>(), (EqualityComparer<InputHint>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_ActiveHintsBinding = new ValueBinding<InputHint[]>("input", "activeHints", Array.Empty<InputHint>(), (IWriter<InputHint[]>)(object)new ArrayWriter<InputHint>((IWriter<InputHint>)(object)new ValueWriter<InputHint>(), false), (EqualityComparer<InputHint[]>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_GamepadTypeBinding = new ValueBinding<int>("input", "gamepadType", (int)InputManager.instance.GetActiveGamepadType(), (IWriter<int>)null, (EqualityComparer<int>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)(m_TutorialHints = new GetterMapBinding<TutorialInputHintQuery, InputHint[]>("input", "tutorialHints", (Func<TutorialInputHintQuery, InputHint[]>)GetTutorialHints, (IReader<TutorialInputHintQuery>)(object)new ValueReader<TutorialInputHintQuery>(), (IWriter<TutorialInputHintQuery>)(object)new ValueWriter<TutorialInputHintQuery>(), (IWriter<InputHint[]>)(object)new ArrayWriter<InputHint>((IWriter<InputHint>)(object)new ValueWriter<InputHint>(), false), (EqualityComparer<InputHint[]>)null)));
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<string>("input", "onInputHintPerformed", (Action<string>)HandleInputHintPerformed, (IReader<string>)null));
		InputManager.instance.EventActionsChanged += OnActionsChanged;
		InputManager.instance.EventEnabledActionsChanged += OnEnabledActionsChanged;
		InputManager.instance.EventActionDisplayNamesChanged += OnActionDisplayNamesChanged;
		InputManager.instance.EventControlSchemeChanged += OnControlSchemeChanged;
		InputManager.instance.EventActiveDeviceChanged += OnActiveDeviceChanged;
	}

	public void Dispose()
	{
		InputManager.instance.EventActionsChanged -= OnActionsChanged;
		InputManager.instance.EventEnabledActionsChanged -= OnEnabledActionsChanged;
		InputManager.instance.EventActionDisplayNamesChanged -= OnActionDisplayNamesChanged;
		InputManager.instance.EventControlSchemeChanged -= OnControlSchemeChanged;
		InputManager.instance.EventActiveDeviceChanged -= OnActiveDeviceChanged;
	}

	private void OnActionsChanged()
	{
		m_HintsDirty = true;
		m_TutorialHintsDirty = true;
		((MapBindingBase<InputHintQuery>)(object)m_HintsMapBinding).UpdateAll();
	}

	private void OnEnabledActionsChanged()
	{
		m_HintsDirty = true;
	}

	private void OnActionDisplayNamesChanged()
	{
		m_HintsDirty = true;
	}

	private void OnControlSchemeChanged(InputManager.ControlScheme controlScheme)
	{
		m_HintsDirty = true;
	}

	private void OnActiveDeviceChanged(InputDevice newDevice, InputDevice oldDevice, bool schemeChanged)
	{
		if (InputManager.instance.activeControlScheme == InputManager.ControlScheme.Gamepad)
		{
			m_GamepadTypeBinding.Update((int)InputManager.instance.GetActiveGamepadType());
		}
	}

	public override bool Update()
	{
		if (m_TutorialHintsDirty)
		{
			((MapBindingBase<TutorialInputHintQuery>)(object)m_TutorialHints).Update();
			m_TutorialHintsDirty = false;
		}
		if (m_HintsDirty)
		{
			m_HintsDirty = false;
			RebuildHints();
			m_ActiveHintsBinding.Update(m_Hints.Values.OrderBy((InputHint h) => h.priority).ToArray());
		}
		return ((CompositeBinding)this).Update();
	}

	private void HandleInputHintPerformed(string action)
	{
		foreach (InputHint value in m_Hints.Values)
		{
			if (value.name == action)
			{
				this.onInputHintPerformed?.Invoke(value.action);
				break;
			}
		}
	}

	private void RebuildHints()
	{
		m_Hints.Clear();
		foreach (ProxyAction action in InputManager.instance.actions)
		{
			if (action.displayOverride != null)
			{
				InputManager.ControlScheme activeControlScheme = InputManager.instance.activeControlScheme;
				CollectHints(m_Hints, action, activeControlScheme switch
				{
					InputManager.ControlScheme.Gamepad => InputManager.DeviceType.Gamepad, 
					InputManager.ControlScheme.KeyboardAndMouse => InputManager.DeviceType.Keyboard | InputManager.DeviceType.Mouse, 
					_ => InputManager.DeviceType.None, 
				});
			}
		}
	}

	private static void CollectHints(Dictionary<(string name, int priority), InputHint> hints, ProxyAction action, InputManager.DeviceType device, bool ignoreMask = false)
	{
		string item = action.displayOverride?.displayName ?? action.title;
		int item2 = action.displayOverride?.priority ?? (-1);
		if (!hints.TryGetValue((item, item2), out var value))
		{
			value = InputHint.Create(action);
			hints[(item, item2)] = value;
		}
		CollectHintItems(value, action, device, action.displayOverride?.transform ?? UIBaseInputAction.Transform.None, ignoreMask);
	}

	internal static void CollectHintItems(InputHint hint, ProxyAction action, InputManager.DeviceType device, UIBaseInputAction.Transform transform, bool ignoreMask = true)
	{
		InputManager.DeviceType deviceType = default(InputManager.DeviceType);
		ProxyComposite proxyComposite = default(ProxyComposite);
		ActionComponent actionComponent = default(ActionComponent);
		ProxyBinding proxyBinding = default(ProxyBinding);
		IReadOnlyList<ProxyModifier> readOnlyList = default(IReadOnlyList<ProxyModifier>);
		List<ProxyBinding> list = default(List<ProxyBinding>);
		foreach (KeyValuePair<InputManager.DeviceType, ProxyComposite> composite in action.composites)
		{
			composite.Deconstruct(ref deviceType, ref proxyComposite);
			ProxyComposite proxyComposite2 = proxyComposite;
			if ((!ignoreMask && (proxyComposite2.m_Device & action.mask) == 0) || (proxyComposite2.m_Device & device) == 0)
			{
				continue;
			}
			modifiersGroups.Clear();
			foreach (KeyValuePair<ActionComponent, ProxyBinding> binding in proxyComposite2.bindings)
			{
				binding.Deconstruct(ref actionComponent, ref proxyBinding);
				ProxyBinding item = proxyBinding;
				if (item.isSet && !item.isDummy && (transform == UIBaseInputAction.Transform.None || (item.component.ToTransform() & transform) != UIBaseInputAction.Transform.None))
				{
					if (!modifiersGroups.TryGetValue(item.modifiers, out var value))
					{
						value = new List<ProxyBinding>();
						modifiersGroups[item.modifiers] = value;
					}
					value.Add(item);
				}
			}
			foreach (KeyValuePair<IReadOnlyList<ProxyModifier>, List<ProxyBinding>> modifiersGroup in modifiersGroups)
			{
				modifiersGroup.Deconstruct(ref readOnlyList, ref list);
				IReadOnlyList<ProxyModifier> readOnlyList2 = readOnlyList;
				List<ProxyBinding> list2 = list;
				string[] paths = new string[list2.Count];
				for (int i = 0; i < list2.Count; i++)
				{
					string[] array = paths;
					int num = i;
					proxyBinding = list2[i];
					array[num] = proxyBinding.path;
				}
				SimplifyPaths(ref paths);
				ControlPath[] array2 = new ControlPath[paths.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = ControlPath.Get(paths[j]);
				}
				ControlPath[] array3 = new ControlPath[readOnlyList2.Count];
				for (int k = 0; k < array3.Length; k++)
				{
					array3[k] = ControlPath.Get(readOnlyList2[k].m_Path);
				}
				hint.items.Add(new InputHintItem
				{
					bindings = array2,
					modifiers = array3
				});
			}
		}
	}

	private static InputHint[] GetTutorialHints(TutorialInputHintQuery query)
	{
		switch (query.action)
		{
		case "Rotate Mouse":
			query.action = "Rotate";
			break;
		case "Zoom Mouse":
			query.action = "Zoom";
			break;
		case "Tool Options":
			query.map = "Navigation";
			query.action = "Secondary Action";
			break;
		case "Cancel":
			if (query.map == "Tool")
			{
				query.map = "Navigation";
				query.action = "Back";
			}
			break;
		}
		ProxyAction action = InputManager.instance.FindAction(query.map, query.action);
		if (action == null)
		{
			return Array.Empty<InputHint>();
		}
		if (query.controlScheme == InputManager.ControlScheme.Gamepad)
		{
			Dictionary<(string, int), InputHint> dictionary = new Dictionary<(string, int), InputHint>();
			InputManager.ControlScheme controlScheme = query.controlScheme;
			CollectHints(dictionary, action, controlScheme switch
			{
				InputManager.ControlScheme.Gamepad => InputManager.DeviceType.Gamepad, 
				InputManager.ControlScheme.KeyboardAndMouse => InputManager.DeviceType.Keyboard | InputManager.DeviceType.Mouse, 
				_ => InputManager.DeviceType.None, 
			}, ignoreMask: true);
			return dictionary.Values.OrderBy((InputHint h) => h.priority).ToArray();
		}
		if (query.index >= 0)
		{
			ProxyBinding proxyBinding = action.bindings.Where((ProxyBinding b) => MatchesControlScheme(b, query.controlScheme)).Skip(query.index).FirstOrDefault();
			return new InputHint[1]
			{
				new InputHint(action)
				{
					name = action.title,
					items = 
					{
						new InputHintItem
						{
							bindings = new ControlPath[1] { ControlPath.Get(proxyBinding.path) },
							modifiers = proxyBinding.modifiers.Select((ProxyModifier m) => ControlPath.Get(m.m_Path)).ToArray()
						}
					}
				}
			};
		}
		return (from b in action.bindings
			where b.isSet && MatchesControlScheme(b, query.controlScheme)
			select new InputHint(action)
			{
				name = action.title,
				items = 
				{
					new InputHintItem
					{
						bindings = new ControlPath[1] { ControlPath.Get(b.path) },
						modifiers = b.modifiers.Select((ProxyModifier m) => ControlPath.Get(m.m_Path)).ToArray()
					}
				}
			}).ToArray();
	}

	private InputHint GetInputHint(InputHintQuery query)
	{
		if (m_HintsMapBinding.values.TryGetValue(query, out var value) && value.version == InputManager.instance.actionVersion)
		{
			return value;
		}
		UIBaseInputAction[] inputActions = InputManager.instance.uiActionCollection.m_InputActions;
		UIBaseInputAction uIBaseInputAction = null;
		for (int i = 0; i < inputActions.Length; i++)
		{
			if (inputActions[i].aliasName == query.action)
			{
				uIBaseInputAction = inputActions[i];
				break;
			}
		}
		if ((Object)(object)uIBaseInputAction == (Object)null)
		{
			return null;
		}
		value = new InputHint(null)
		{
			name = uIBaseInputAction.aliasName,
			priority = uIBaseInputAction.displayPriority,
			show = true
		};
		foreach (UIInputActionPart actionPart in uIBaseInputAction.actionParts)
		{
			if (InputManager.instance.TryFindAction(InputActionReference.op_Implicit(actionPart.m_Action), out var proxyAction))
			{
				InputManager.ControlScheme controlScheme = query.controlScheme;
				CollectHintItems(value, proxyAction, controlScheme switch
				{
					InputManager.ControlScheme.Gamepad => InputManager.DeviceType.Gamepad, 
					InputManager.ControlScheme.KeyboardAndMouse => InputManager.DeviceType.Keyboard | InputManager.DeviceType.Mouse, 
					_ => InputManager.DeviceType.None, 
				}, actionPart.m_Transform);
			}
		}
		return value;
	}

	private static bool MatchesControlScheme(ProxyBinding binding, InputManager.ControlScheme controlScheme)
	{
		if (controlScheme != InputManager.ControlScheme.Gamepad || !binding.isGamepad)
		{
			if (controlScheme == InputManager.ControlScheme.KeyboardAndMouse)
			{
				if (!binding.isKeyboard)
				{
					return binding.isMouse;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	private static void SimplifyPaths(ref string[] paths)
	{
		for (int i = 0; i < axisControls.Length; i++)
		{
			string text = axisControls[i];
			if (MatchesDirections(paths, text, allDirs) || MatchesDirections(paths, text, axes))
			{
				paths = new string[1] { text };
				break;
			}
			if (MatchesDirections(paths, text, horizontal))
			{
				paths = new string[1] { text + "/x" };
				break;
			}
			if (MatchesDirections(paths, text, vertical))
			{
				paths = new string[1] { text + "/y" };
				break;
			}
		}
	}

	private static bool MatchesDirections(string[] bindings, string basePath, string[] dirs)
	{
		if (bindings.Length != dirs.Length)
		{
			return false;
		}
		foreach (string text in dirs)
		{
			bool flag = false;
			foreach (string text2 in bindings)
			{
				if (text2.Length == basePath.Length + text.Length && text2.StartsWith(basePath) && text2.EndsWith(text))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}
}
