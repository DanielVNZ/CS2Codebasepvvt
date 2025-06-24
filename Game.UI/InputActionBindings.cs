using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.UI.Binding;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI;

public class InputActionBindings : CompositeBinding, IDisposable
{
	private class ActionState : IComparable<ActionState>, IDisposable
	{
		public enum State
		{
			Enabled,
			Disabled,
			DisabledNoConsumer,
			DisabledNotSet,
			DisabledMaskMismatch,
			DisabledConflict,
			DisabledDuplicate
		}

		private bool m_Disposed;

		private readonly string m_Name;

		private readonly ProxyAction m_Action;

		private readonly UIBaseInputAction.ProcessAs m_ProcessAs;

		private readonly InputActivator m_Activator;

		private readonly DisplayNameOverride m_NameOverride;

		private readonly InputManager.DeviceType m_Mask;

		private int m_Priority;

		private State m_State;

		private readonly UIBaseInputAction.Transform m_Transform;

		public bool isDisposed => m_Disposed;

		public ProxyAction action => m_Action;

		public string name => m_Name;

		public InputManager.DeviceType mask => m_Mask;

		public int priority
		{
			get
			{
				return m_Priority;
			}
			set
			{
				if (!m_Disposed && value != m_Priority)
				{
					m_Priority = value;
					this.onChanged?.Invoke();
				}
			}
		}

		public State state
		{
			get
			{
				return m_State;
			}
			set
			{
				if (!m_Disposed && value != m_State)
				{
					m_State = value;
					this.onChanged?.Invoke();
				}
			}
		}

		public UIBaseInputAction.Transform transform => m_Transform;

		public UIBaseInputAction.ProcessAs processAs => m_ProcessAs;

		public event Action onChanged;

		public ActionState(ProxyAction action, string name, DisplayNameOverride displayOverride, UIBaseInputAction.ProcessAs processAs = UIBaseInputAction.ProcessAs.AutoDetect, UIBaseInputAction.Transform transform = UIBaseInputAction.Transform.None, InputManager.DeviceType mask = InputManager.DeviceType.All)
		{
			m_Action = action ?? throw new ArgumentNullException("action");
			m_Name = name ?? throw new ArgumentNullException("name");
			m_Mask = mask;
			m_Activator = new InputActivator(ignoreIsBuiltIn: true, m_Name, action, mask);
			m_Priority = -1;
			m_NameOverride = displayOverride;
			m_ProcessAs = processAs;
			m_Transform = transform;
			UpdateState();
		}

		public void Dispose()
		{
			if (!m_Disposed)
			{
				m_Disposed = true;
				m_Activator?.Dispose();
				m_NameOverride?.Dispose();
				this.onChanged = null;
			}
		}

		public void UpdateState()
		{
			if (!m_Action.isSet)
			{
				state = State.DisabledNotSet;
			}
			else if ((m_Action.availableDevices & InputManager.instance.mask & m_Mask) == 0)
			{
				state = State.DisabledMaskMismatch;
			}
			else if (m_Priority == -1)
			{
				state = State.DisabledNoConsumer;
			}
			else
			{
				state = State.Enabled;
			}
		}

		public int CompareTo(ActionState other)
		{
			return -m_Priority.CompareTo(other.m_Priority);
		}

		public void Apply()
		{
			if (!m_Disposed)
			{
				if (m_Activator != null)
				{
					m_Activator.enabled = state == State.Enabled;
				}
				if (m_NameOverride != null)
				{
					m_NameOverride.active = state == State.Enabled;
				}
			}
		}

		public override string ToString()
		{
			return $"{m_Name} ({m_Action})";
		}
	}

	private interface IEventTrigger : IDisposable
	{
		HashSet<ActionState> states { get; }

		static IEventTrigger GetTrigger(InputActionBindings parent, ProxyAction action, UIBaseInputAction.ProcessAs processAs)
		{
			string expectedControlType = action.sourceAction.expectedControlType;
			switch (expectedControlType)
			{
			default:
				if (expectedControlType.Length != 0)
				{
					break;
				}
				goto case "Button";
			case "Dpad":
			case "Stick":
			case "Vector2":
				return processAs switch
				{
					UIBaseInputAction.ProcessAs.Button => new Vector2ToButtonEventTrigger(parent, action), 
					UIBaseInputAction.ProcessAs.Axis => new Vector2ToAxisEventTrigger(parent, action), 
					UIBaseInputAction.ProcessAs.Vector2 => new Vector2EventTrigger(parent, action), 
					_ => new Vector2EventTrigger(parent, action), 
				};
			case "Axis":
				return processAs switch
				{
					UIBaseInputAction.ProcessAs.Button => new AxisToButtonEventTrigger(parent, action), 
					UIBaseInputAction.ProcessAs.Axis => new AxisEventTrigger(parent, action), 
					UIBaseInputAction.ProcessAs.Vector2 => new AxisToVector2EventTrigger(parent, action), 
					_ => new AxisEventTrigger(parent, action), 
				};
			case "Button":
				return processAs switch
				{
					UIBaseInputAction.ProcessAs.Button => new ButtonEventTrigger(parent, action), 
					UIBaseInputAction.ProcessAs.Axis => new ButtonToAxisEventTrigger(parent, action), 
					UIBaseInputAction.ProcessAs.Vector2 => new ButtonToVector2EventTrigger(parent, action), 
					_ => new ButtonEventTrigger(parent, action), 
				};
			case null:
				break;
			}
			return new DefaultEventTrigger(parent, action);
		}
	}

	private abstract class EventTrigger<TRawValue, TValue> : IEventTrigger, IDisposable where TRawValue : struct where TValue : struct
	{
		private bool m_Disposed;

		private readonly InputActionBindings m_Parent;

		private readonly ProxyAction m_Action;

		private readonly IWriter<TValue> m_ValueWriter;

		public HashSet<ActionState> states { get; } = new HashSet<ActionState>();

		public EventTrigger(InputActionBindings parent, ProxyAction action, IWriter<TValue> valueWriter = null)
		{
			m_Parent = parent;
			m_Action = action ?? throw new ArgumentNullException("action");
			m_ValueWriter = valueWriter ?? ValueWriters.Create<TValue>();
			m_Action.onInteraction += OnInteraction;
		}

		private void OnInteraction(ProxyAction _, InputActionPhase phase)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Invalid comparison between Unknown and I4
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Invalid comparison between Unknown and I4
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Invalid comparison between Unknown and I4
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Invalid comparison between Unknown and I4
			if (m_Disposed || ((int)phase != 3 && ((int)phase != 4 || (int)m_Action.sourceAction.type != 2)))
			{
				return;
			}
			TRawValue value = m_Action.ReadValue<TRawValue>();
			foreach (ActionState state in states)
			{
				if (state.state != ActionState.State.Enabled)
				{
					continue;
				}
				if ((int)phase != 3)
				{
					if ((int)phase == 4 && (int)m_Action.sourceAction.type == 2)
					{
						TriggerEvent(m_Parent.m_ActionReleasedBinding, state.name, default(TValue));
					}
					continue;
				}
				TValue value2 = TransformValue(value, state.transform);
				if (GetMagnitude(value2) != 0f)
				{
					TriggerEvent(m_Parent.m_ActionPerformedBinding, state.name, value2);
				}
				else if ((int)m_Action.sourceAction.type == 2)
				{
					TriggerEvent(m_Parent.m_ActionReleasedBinding, state.name, value2);
				}
			}
		}

		private void TriggerEvent(RawEventBinding binding, string action, TValue value)
		{
			IJsonWriter val = binding.EventBegin();
			val.TypeBegin("input.InputActionEvent");
			val.PropertyName("action");
			val.Write(action);
			val.PropertyName("value");
			((IWriter<_003F>)(object)m_ValueWriter).Write(val, value);
			val.TypeEnd();
			binding.EventEnd();
		}

		protected abstract TValue TransformValue(TRawValue value, UIBaseInputAction.Transform transform);

		protected abstract float GetMagnitude(TValue value);

		public void Dispose()
		{
			if (!m_Disposed)
			{
				m_Disposed = true;
				states.Clear();
				m_Action.onInteraction -= OnInteraction;
			}
		}
	}

	private class DefaultEventTrigger : EventTrigger<float, float>
	{
		public DefaultEventTrigger(InputActionBindings parent, ProxyAction action, IWriter<float> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override float TransformValue(float value, UIBaseInputAction.Transform transform)
		{
			return value;
		}

		protected override float GetMagnitude(float value)
		{
			return Mathf.Abs(value);
		}
	}

	private class ButtonEventTrigger : EventTrigger<float, float>
	{
		public ButtonEventTrigger(InputActionBindings parent, ProxyAction action, IWriter<float> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override float TransformValue(float value, UIBaseInputAction.Transform transform)
		{
			return Mathf.Clamp(value, 0f, 1f);
		}

		protected override float GetMagnitude(float value)
		{
			return Mathf.Abs(value);
		}
	}

	private class AxisEventTrigger : EventTrigger<float, float>
	{
		public AxisEventTrigger(InputActionBindings parent, ProxyAction action, IWriter<float> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override float TransformValue(float value, UIBaseInputAction.Transform transform)
		{
			return value;
		}

		protected override float GetMagnitude(float value)
		{
			return Mathf.Abs(value);
		}
	}

	private class Vector2EventTrigger : EventTrigger<Vector2, Vector2>
	{
		public Vector2EventTrigger(InputActionBindings parent, ProxyAction action, IWriter<Vector2> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override Vector2 TransformValue(Vector2 value, UIBaseInputAction.Transform transform)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return value;
		}

		protected override float GetMagnitude(Vector2 value)
		{
			return ((Vector2)(ref value)).magnitude;
		}
	}

	private class AxisToButtonEventTrigger : EventTrigger<float, float>
	{
		public AxisToButtonEventTrigger(InputActionBindings parent, ProxyAction action, IWriter<float> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override float TransformValue(float value, UIBaseInputAction.Transform transform)
		{
			return transform switch
			{
				UIBaseInputAction.Transform.Negative => Mathf.Clamp(0f - value, 0f, 1f), 
				UIBaseInputAction.Transform.Positive => Mathf.Clamp(value, 0f, 1f), 
				_ => Mathf.Abs(value), 
			};
		}

		protected override float GetMagnitude(float value)
		{
			return Mathf.Abs(value);
		}
	}

	private class Vector2ToButtonEventTrigger : EventTrigger<Vector2, float>
	{
		public Vector2ToButtonEventTrigger(InputActionBindings parent, ProxyAction action, IWriter<float> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override float TransformValue(Vector2 value, UIBaseInputAction.Transform transform)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			return transform switch
			{
				UIBaseInputAction.Transform.Down => Mathf.Clamp(0f - value.y, 0f, 1f), 
				UIBaseInputAction.Transform.Up => Mathf.Clamp(value.y, 0f, 1f), 
				UIBaseInputAction.Transform.Left => Mathf.Clamp(0f - value.x, 0f, 1f), 
				UIBaseInputAction.Transform.Right => Mathf.Clamp(value.x, 0f, 1f), 
				UIBaseInputAction.Transform.Horizontal => Mathf.Abs(value.x), 
				UIBaseInputAction.Transform.Vertical => Mathf.Abs(value.y), 
				_ => ((Vector2)(ref value)).magnitude, 
			};
		}

		protected override float GetMagnitude(float value)
		{
			return Mathf.Abs(value);
		}
	}

	private class ButtonToAxisEventTrigger : EventTrigger<float, float>
	{
		public ButtonToAxisEventTrigger(InputActionBindings parent, ProxyAction action, IWriter<float> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override float TransformValue(float value, UIBaseInputAction.Transform transform)
		{
			return transform switch
			{
				UIBaseInputAction.Transform.Negative => 0f - value, 
				UIBaseInputAction.Transform.Positive => value, 
				UIBaseInputAction.Transform.Down => 0f - value, 
				UIBaseInputAction.Transform.Up => value, 
				UIBaseInputAction.Transform.Left => 0f - value, 
				UIBaseInputAction.Transform.Right => value, 
				_ => value, 
			};
		}

		protected override float GetMagnitude(float value)
		{
			return Mathf.Abs(value);
		}
	}

	private class Vector2ToAxisEventTrigger : EventTrigger<Vector2, float>
	{
		public Vector2ToAxisEventTrigger(InputActionBindings parent, ProxyAction action, IWriter<float> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override float TransformValue(Vector2 value, UIBaseInputAction.Transform transform)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			return transform switch
			{
				UIBaseInputAction.Transform.Horizontal => value.x, 
				UIBaseInputAction.Transform.Vertical => value.y, 
				_ => ((Vector2)(ref value)).magnitude, 
			};
		}

		protected override float GetMagnitude(float value)
		{
			return Mathf.Abs(value);
		}
	}

	private class ButtonToVector2EventTrigger : EventTrigger<float, Vector2>
	{
		public ButtonToVector2EventTrigger(InputActionBindings parent, ProxyAction action, IWriter<Vector2> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override Vector2 TransformValue(float value, UIBaseInputAction.Transform transform)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			return (Vector2)(transform switch
			{
				UIBaseInputAction.Transform.Left => new Vector2(0f - value, 0f), 
				UIBaseInputAction.Transform.Right => new Vector2(value, 0f), 
				UIBaseInputAction.Transform.Down => new Vector2(0f, 0f - value), 
				UIBaseInputAction.Transform.Up => new Vector2(0f, value), 
				_ => new Vector2(value, value), 
			});
		}

		protected override float GetMagnitude(Vector2 value)
		{
			return ((Vector2)(ref value)).magnitude;
		}
	}

	private class AxisToVector2EventTrigger : EventTrigger<float, Vector2>
	{
		public AxisToVector2EventTrigger(InputActionBindings parent, ProxyAction action, IWriter<Vector2> valueWriter = null)
			: base(parent, action, valueWriter)
		{
		}

		protected override Vector2 TransformValue(float value, UIBaseInputAction.Transform transform)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			return (Vector2)(transform switch
			{
				UIBaseInputAction.Transform.Horizontal => new Vector2(value, 0f), 
				UIBaseInputAction.Transform.Vertical => new Vector2(0f, value), 
				_ => Vector2.zero, 
			});
		}

		protected override float GetMagnitude(Vector2 value)
		{
			return ((Vector2)(ref value)).magnitude;
		}
	}

	private const int kDisabledPriority = -1;

	private const string kGroup = "input";

	private RawEventBinding m_ActionPerformedBinding;

	private RawEventBinding m_ActionReleasedBinding;

	private EventBinding m_ActionRefreshedBinding;

	private readonly List<ActionState> m_UIActionStates = new List<ActionState>();

	private readonly Dictionary<(ProxyAction, UIBaseInputAction.ProcessAs), IEventTrigger> m_Triggers = new Dictionary<(ProxyAction, UIBaseInputAction.ProcessAs), IEventTrigger>();

	private readonly Dictionary<string, int> m_ActionOrder = new Dictionary<string, int>();

	private bool m_ActionsDirty = true;

	private bool m_ConflictsDirty = true;

	private bool m_UpdateInProgress;

	public InputActionBindings()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004d: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_006b: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0103: Expected O, but got Unknown
		RawEventBinding val = new RawEventBinding("input", "onActionPerformed");
		RawEventBinding val2 = val;
		m_ActionPerformedBinding = val;
		((CompositeBinding)this).AddBinding((IBinding)(object)val2);
		RawEventBinding val3 = new RawEventBinding("input", "onActionReleased");
		val2 = val3;
		m_ActionReleasedBinding = val3;
		((CompositeBinding)this).AddBinding((IBinding)(object)val2);
		((CompositeBinding)this).AddBinding((IBinding)(object)new TriggerBinding<string, int>("input", "setActionPriority", (Action<string, int>)SetActionPriority, (IReader<string>)null, (IReader<int>)null));
		string[] array = InputManager.instance.uiActionCollection.m_InputActions.Select((UIBaseInputAction a) => a.aliasName).ToArray();
		((CompositeBinding)this).AddBinding((IBinding)(object)new ValueBinding<string[]>("input", "actionNames", array, (IWriter<string[]>)(object)new ArrayWriter<string>((IWriter<string>)null, false), (EqualityComparer<string[]>)null));
		EventBinding val4 = new EventBinding("input", "onActionsRefreshed");
		EventBinding val5 = val4;
		m_ActionRefreshedBinding = val4;
		((CompositeBinding)this).AddBinding((IBinding)(object)val5);
		InputManager.instance.EventActionsChanged += OnActionsChanged;
		InputManager.instance.EventControlSchemeChanged += OnControlSchemeChanged;
	}

	public void Dispose()
	{
		InputManager.instance.EventActionsChanged -= OnActionsChanged;
		InputManager.instance.EventControlSchemeChanged -= OnControlSchemeChanged;
		foreach (ActionState uIActionState in m_UIActionStates)
		{
			uIActionState.Dispose();
		}
		foreach (IEventTrigger value in m_Triggers.Values)
		{
			value.Dispose();
		}
		m_Triggers.Clear();
	}

	private void SetActionPriority(string action, int priority)
	{
		if (m_ActionOrder.TryGetValue(action, out var i))
		{
			for (; i < m_UIActionStates.Count && m_UIActionStates[i].name == action; i++)
			{
				m_UIActionStates[i].priority = priority;
			}
		}
	}

	private void SetConflictsDirty()
	{
		if (!m_UpdateInProgress)
		{
			m_ConflictsDirty = true;
		}
	}

	private void OnActionsChanged()
	{
		if (!m_UpdateInProgress)
		{
			m_ActionsDirty = true;
			m_ConflictsDirty = true;
		}
	}

	private void OnControlSchemeChanged(InputManager.ControlScheme scheme)
	{
		if (!m_UpdateInProgress)
		{
			m_ConflictsDirty = true;
		}
	}

	public override bool Update()
	{
		m_UpdateInProgress = true;
		if (m_ActionsDirty)
		{
			RefreshActions();
			m_ActionsDirty = false;
			m_ActionRefreshedBinding.Trigger();
		}
		if (m_ConflictsDirty)
		{
			ResolveConflicts();
			m_ConflictsDirty = false;
		}
		m_UpdateInProgress = false;
		return ((CompositeBinding)this).Update();
	}

	private void RefreshActions()
	{
		for (int i = 0; i < m_UIActionStates.Count; i++)
		{
			m_UIActionStates[i].Dispose();
			if (m_Triggers.TryGetValue((m_UIActionStates[i].action, m_UIActionStates[i].processAs), out var value))
			{
				value.states.Remove(m_UIActionStates[i]);
				if (value.states.Count == 0)
				{
					value.Dispose();
					m_Triggers.Remove((m_UIActionStates[i].action, m_UIActionStates[i].processAs));
				}
			}
		}
		m_UIActionStates.Clear();
		m_ActionOrder.Clear();
		for (int j = 0; j < InputManager.instance.uiActionCollection.m_InputActions.Length; j++)
		{
			UIBaseInputAction uIBaseInputAction = InputManager.instance.uiActionCollection.m_InputActions[j];
			int count = m_UIActionStates.Count;
			for (int k = 0; k < uIBaseInputAction.actionParts.Count; k++)
			{
				UIInputActionPart uIInputActionPart = uIBaseInputAction.actionParts[k];
				ProxyAction proxyAction = uIInputActionPart.GetProxyAction();
				if (proxyAction.isSet)
				{
					DisplayNameOverride displayName = uIBaseInputAction.GetDisplayName(uIInputActionPart, "InputActionBindings");
					ActionState actionState = new ActionState(proxyAction, uIBaseInputAction.aliasName, displayName, uIInputActionPart.m_ProcessAs, uIInputActionPart.m_Transform, uIInputActionPart.m_Mask);
					actionState.onChanged += SetConflictsDirty;
					if (!m_Triggers.TryGetValue((actionState.action, actionState.processAs), out var value2))
					{
						value2 = IEventTrigger.GetTrigger(this, actionState.action, actionState.processAs);
						m_Triggers.Add((actionState.action, actionState.processAs), value2);
					}
					value2.states.Add(actionState);
					m_UIActionStates.Add(actionState);
				}
			}
			if (count != m_UIActionStates.Count)
			{
				m_ActionOrder[uIBaseInputAction.aliasName] = count;
			}
		}
	}

	private void ResolveConflicts()
	{
		ActionState[] array = m_UIActionStates.OrderBy((ActionState a) => a).ToArray();
		InputManager.DeviceType mask = InputManager.instance.mask;
		for (int num = 0; num < m_UIActionStates.Count; num++)
		{
			m_UIActionStates[num].UpdateState();
		}
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			ActionState actionState = array[num2];
			if (actionState.state != ActionState.State.Enabled)
			{
				continue;
			}
			for (int num3 = num2 + 1; num3 < array.Length; num3++)
			{
				ActionState actionState2 = array[num3];
				if (actionState2.state != ActionState.State.Enabled)
				{
					continue;
				}
				if (actionState2.action == actionState.action)
				{
					if (actionState2.transform == actionState.transform || (actionState2.transform & actionState.transform) != UIBaseInputAction.Transform.None)
					{
						actionState2.state = ActionState.State.DisabledDuplicate;
					}
				}
				else if (InputManager.HasConflicts(actionState2.action, actionState.action, actionState.mask & mask, actionState2.mask & mask))
				{
					actionState2.state = ActionState.State.DisabledConflict;
				}
			}
		}
		using (ProxyAction.DeferStateUpdating())
		{
			for (int num4 = 0; num4 < m_UIActionStates.Count; num4++)
			{
				m_UIActionStates[num4].Apply();
			}
		}
	}
}
