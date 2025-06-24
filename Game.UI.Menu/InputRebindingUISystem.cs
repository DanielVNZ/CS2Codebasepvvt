using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.UI.Binding;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace Game.UI.Menu;

public class InputRebindingUISystem : UISystemBase
{
	[Flags]
	private enum Options
	{
		None = 0,
		Unsolved = 1,
		Swap = 2,
		Unset = 4,
		Forward = 8,
		Backward = 0x10
	}

	private record BindingPair(ProxyBinding oldBinding, ProxyBinding newBinding);

	private struct ConflictInfo : IJsonWritable
	{
		public ProxyBinding binding;

		public ConflictInfoItem[] conflicts;

		public bool unsolved { get; set; }

		public bool swap { get; set; }

		public bool unset { get; set; }

		public bool batchSwap { get; set; }

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(typeof(ConflictInfo).FullName);
			writer.PropertyName("binding");
			JsonWriterExtensions.Write<ProxyBinding>(writer, binding);
			writer.PropertyName("conflicts");
			JsonWriterExtensions.Write<ConflictInfoItem>(writer, (IList<ConflictInfoItem>)conflicts.Where((ConflictInfoItem c) => !c.isHidden).ToArray());
			writer.PropertyName("unsolved");
			writer.Write(unsolved);
			writer.PropertyName("swap");
			writer.Write(swap);
			writer.PropertyName("unset");
			writer.Write(unset);
			writer.PropertyName("batchSwap");
			writer.Write(batchSwap);
			writer.TypeEnd();
		}
	}

	private struct ConflictInfoItem : IJsonWritable
	{
		public ProxyBinding binding;

		public ProxyBinding resolution;

		public Options options;

		public bool isAlias;

		public bool isHidden;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(typeof(ConflictInfoItem).FullName);
			writer.PropertyName("binding");
			JsonWriterExtensions.Write<ProxyBinding>(writer, binding);
			writer.PropertyName("resolution");
			JsonWriterExtensions.Write<ProxyBinding>(writer, resolution);
			writer.PropertyName("isHidden");
			writer.Write(isHidden);
			writer.TypeEnd();
		}
	}

	private const string kGroup = "inputRebinding";

	private ValueBinding<ProxyBinding?> m_ActiveRebindingBinding;

	private ValueBinding<ConflictInfo?> m_ActiveConflictBinding;

	private RebindingOperation m_Operation;

	private RebindingOperation m_ModifierOperation;

	private ProxyBinding? m_ActiveRebinding;

	private Action<ProxyBinding> m_OnSetBinding;

	private ProxyBinding? m_PendingRebinding;

	private Dictionary<string, ConflictInfoItem> m_Conflicts = new Dictionary<string, ConflictInfoItem>();

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		base.OnCreate();
		AddBinding((IBinding)(object)(m_ActiveRebindingBinding = new ValueBinding<ProxyBinding?>("inputRebinding", "activeRebinding", (ProxyBinding?)null, (IWriter<ProxyBinding?>)(object)ValueWritersStruct.Nullable<ProxyBinding>((IWriter<ProxyBinding>)(object)new ValueWriter<ProxyBinding>()), (EqualityComparer<ProxyBinding?>)null)));
		AddBinding((IBinding)(object)(m_ActiveConflictBinding = new ValueBinding<ConflictInfo?>("inputRebinding", "activeConflict", (ConflictInfo?)null, (IWriter<ConflictInfo?>)(object)ValueWritersStruct.Nullable<ConflictInfo>((IWriter<ConflictInfo>)(object)new ValueWriter<ConflictInfo>()), (EqualityComparer<ConflictInfo?>)null)));
		AddBinding((IBinding)new TriggerBinding("inputRebinding", "cancelRebinding", (Action)Cancel));
		AddBinding((IBinding)new TriggerBinding("inputRebinding", "completeAndSwapConflicts", (Action)CompleteAndSwapConflicts));
		AddBinding((IBinding)new TriggerBinding("inputRebinding", "completeAndUnsetConflicts", (Action)CompleteAndUnsetConflicts));
		m_Operation = new RebindingOperation();
		m_Operation.OnApplyBinding((Action<RebindingOperation, string>)OnApplyBinding);
		m_Operation.OnComplete((Action<RebindingOperation>)OnComplete);
		m_Operation.OnCancel((Action<RebindingOperation>)OnCancel);
		m_ModifierOperation = new RebindingOperation();
		m_ModifierOperation.OnPotentialMatch((Action<RebindingOperation>)OnModifierPotentialMatch);
		m_ModifierOperation.OnApplyBinding((Action<RebindingOperation, string>)OnModifierApplyBinding);
		InputSystem.onDeviceChange += OnDeviceChange;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Operation.Dispose();
		m_ModifierOperation.Dispose();
		InputSystem.onDeviceChange -= OnDeviceChange;
		base.OnDestroy();
	}

	private void OnDeviceChange(InputDevice changedDevice, InputDeviceChange change)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (((int)change != 0 && (int)change != 1) || !m_ActiveRebinding.HasValue)
		{
			return;
		}
		Enumerator<InputDevice> enumerator = InputSystem.devices.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				InputDevice current = enumerator.Current;
				if (current.added && ((current is Keyboard && m_ActiveRebinding.Value.isKeyboard) || (current is Mouse && m_ActiveRebinding.Value.isMouse) || (current is Gamepad && m_ActiveRebinding.Value.isGamepad)))
				{
					return;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		Cancel();
	}

	public void Start(ProxyBinding binding, Action<ProxyBinding> onSetBinding)
	{
		if (m_ActiveRebinding == binding || onSetBinding == null)
		{
			return;
		}
		m_ActiveRebinding = binding;
		m_OnSetBinding = onSetBinding;
		m_Conflicts.Clear();
		if (m_ActiveRebinding.Value.isKeyboard)
		{
			InputManager.instance.blockedControlTypes = InputManager.DeviceType.Keyboard;
		}
		else if (m_ActiveRebinding.Value.isMouse)
		{
			InputManager.instance.blockedControlTypes = InputManager.DeviceType.Mouse;
		}
		else if (m_ActiveRebinding.Value.isGamepad)
		{
			InputManager.instance.blockedControlTypes = InputManager.DeviceType.Gamepad;
		}
		else
		{
			InputManager.instance.blockedControlTypes = InputManager.DeviceType.None;
		}
		m_ActiveRebindingBinding.Update((ProxyBinding?)binding);
		m_ActiveConflictBinding.Update((ConflictInfo?)null);
		m_Operation.Reset().WithMagnitudeHavingToBeGreaterThan(0.6f).OnMatchWaitForAnother(0.1f);
		m_ModifierOperation.Reset().WithMagnitudeHavingToBeGreaterThan(0.6f);
		if (binding.isKeyboard)
		{
			m_Operation.WithControlsHavingToMatchPath("<Keyboard>/<Key>").WithControlsExcluding("<Keyboard>/leftShift").WithControlsExcluding("<Keyboard>/rightShift")
				.WithControlsExcluding("<Keyboard>/leftCtrl")
				.WithControlsExcluding("<Keyboard>/rightCtrl")
				.WithControlsExcluding("<Keyboard>/leftAlt")
				.WithControlsExcluding("<Keyboard>/rightAlt")
				.WithControlsExcluding("<Keyboard>/capsLock")
				.WithControlsExcluding("<Keyboard>/leftWindows")
				.WithControlsExcluding("<Keyboard>/rightWindow")
				.WithControlsExcluding("<Keyboard>/leftMeta")
				.WithControlsExcluding("<Keyboard>/rightMeta")
				.WithControlsExcluding("<Keyboard>/numLock")
				.WithControlsExcluding("<Keyboard>/printScreen")
				.WithControlsExcluding("<Keyboard>/scrollLock")
				.WithControlsExcluding("<Keyboard>/insert")
				.WithControlsExcluding("<Keyboard>/contextMenu")
				.WithControlsExcluding("<Keyboard>/pause")
				.Start();
			if (binding.allowModifiers && binding.isModifiersRebindable)
			{
				m_ModifierOperation.WithControlsHavingToMatchPath("<Keyboard>/shift").WithControlsHavingToMatchPath("<Keyboard>/ctrl").WithControlsHavingToMatchPath("<Keyboard>/alt")
					.Start();
			}
		}
		else if (binding.isMouse)
		{
			m_Operation.WithControlsHavingToMatchPath("<Mouse>/<Button>").Start();
			if (binding.allowModifiers && binding.isModifiersRebindable)
			{
				m_ModifierOperation.WithControlsHavingToMatchPath("<Keyboard>/shift").WithControlsHavingToMatchPath("<Keyboard>/ctrl").WithControlsHavingToMatchPath("<Keyboard>/alt")
					.Start();
			}
		}
		else if (binding.isGamepad)
		{
			m_Operation.WithControlsHavingToMatchPath("<Gamepad>/<Button>").WithControlsHavingToMatchPath("<Gamepad>/*/<Button>").WithControlsExcluding("<Gamepad>/leftStickPress")
				.WithControlsExcluding("<Gamepad>/rightStickPress")
				.WithControlsExcluding("<DualSenseGamepadHID>/leftTriggerButton")
				.WithControlsExcluding("<DualSenseGamepadHID>/rightTriggerButton")
				.WithControlsExcluding("<DualSenseGamepadHID>/systemButton")
				.WithControlsExcluding("<DualSenseGamepadHID>/micButton")
				.Start();
			if (binding.allowModifiers && binding.isModifiersRebindable)
			{
				m_ModifierOperation.WithControlsHavingToMatchPath("<Gamepad>/leftStickPress").WithControlsHavingToMatchPath("<Gamepad>/rightStickPress").Start();
			}
		}
	}

	public void Start(ProxyBinding binding, ProxyBinding newBinding, Action<ProxyBinding> onSetBinding)
	{
		if (!(m_ActiveRebinding == binding) && onSetBinding != null)
		{
			m_ActiveRebinding = binding;
			m_OnSetBinding = onSetBinding;
			m_ActiveRebindingBinding.Update((ProxyBinding?)binding);
			m_ActiveConflictBinding.Update((ConflictInfo?)null);
			Process(binding, newBinding);
		}
	}

	public void Cancel()
	{
		m_Operation.Reset();
		Reset();
	}

	private void CompleteAndSwapConflicts()
	{
		if (m_PendingRebinding.HasValue)
		{
			using (InputManager.DeferUpdating())
			{
				IEnumerable<ProxyBinding> newBindings = from c in m_Conflicts.Values
					where !c.isAlias
					select c.resolution;
				InputManager.instance.SetBindings(newBindings, out var _);
				Apply(m_PendingRebinding.Value);
			}
		}
		Reset();
	}

	private void CompleteAndUnsetConflicts()
	{
		if (m_PendingRebinding.HasValue)
		{
			using (InputManager.DeferUpdating())
			{
				IEnumerable<ProxyBinding> newBindings = from c in m_Conflicts.Values
					where !c.isAlias
					select c.binding.WithPath(string.Empty).WithModifiers(Array.Empty<ProxyModifier>());
				InputManager.instance.SetBindings(newBindings, out var _);
				Apply(m_PendingRebinding.Value);
			}
		}
		Reset();
	}

	private void OnApplyBinding(RebindingOperation operation, string path)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!m_ActiveRebinding.HasValue)
		{
			return;
		}
		InputManager.instance.blockedControlTypes = InputManager.DeviceType.None;
		if (path != null && path.StartsWith("<DualShockGamepad>"))
		{
			path = path.Replace("<DualShockGamepad>", "<Gamepad>");
		}
		ProxyBinding oldBinding = m_ActiveRebinding.Value;
		ProxyBinding newBinding = oldBinding.Copy();
		newBinding.path = path;
		if (newBinding.isModifiersRebindable)
		{
			newBinding.modifiers = (from c in (IEnumerable<InputControl>)(object)m_ModifierOperation.candidates
				where InputControlExtensions.IsPressed(c, 0f)
				select new ProxyModifier
				{
					m_Component = oldBinding.component,
					m_Name = InputManager.GetModifierName(oldBinding.component),
					m_Path = InputManager.GeneratePathForControl(c)
				}).ToList();
		}
		m_ModifierOperation.Reset();
		Process(oldBinding, newBinding);
	}

	private void OnComplete(RebindingOperation operation)
	{
		InputManager.instance.blockedControlTypes = InputManager.DeviceType.None;
		if (!m_PendingRebinding.HasValue)
		{
			Reset();
		}
	}

	private void OnCancel(RebindingOperation operation)
	{
		InputManager.instance.blockedControlTypes = InputManager.DeviceType.None;
		Reset();
	}

	private void Process(ProxyBinding oldBinding, ProxyBinding newBinding)
	{
		UISystemBase.log.InfoFormat("Rebinding from {0} to {1}", (object)oldBinding, (object)newBinding);
		if (newBinding.action == null)
		{
			Reset();
			return;
		}
		if (!NeedAskUser(newBinding))
		{
			Apply(newBinding);
			Reset();
			return;
		}
		m_Conflicts.Clear();
		GetRebindOptions(m_Conflicts, oldBinding, newBinding, out var unsolved, out var batchSwap, out var swap, out var unset);
		if (m_Conflicts.Count == 0)
		{
			Apply(newBinding);
			Reset();
			return;
		}
		m_PendingRebinding = newBinding;
		m_ActiveConflictBinding.Update((ConflictInfo?)new ConflictInfo
		{
			binding = newBinding,
			conflicts = m_Conflicts.Values.OrderBy((ConflictInfoItem b) => b.binding.mapName).ToArray(),
			unsolved = unsolved,
			swap = swap,
			unset = unset,
			batchSwap = batchSwap
		});
		static bool NeedAskUser(ProxyBinding binding)
		{
			ProxyAction action = binding.action;
			if (action.m_LinkedActions.Count != 0)
			{
				return true;
			}
			foreach (UIBaseInputAction uIAlias in action.m_UIAliases)
			{
				if (!uIAlias.showInOptions)
				{
					break;
				}
				foreach (UIInputActionPart actionPart in uIAlias.actionParts)
				{
					if ((actionPart.m_Mask & binding.device) != InputManager.DeviceType.None)
					{
						return true;
					}
				}
			}
			return binding.hasConflicts != ProxyBinding.ConflictType.None;
		}
	}

	private void GetRebindOptions(Dictionary<string, ConflictInfoItem> conflictInfos, ProxyBinding oldBinding, ProxyBinding newBinding, out bool unsolved, out bool batchSwap, out bool swap, out bool unset)
	{
		unsolved = false;
		batchSwap = false;
		swap = false;
		unset = false;
		if (!CollectLinkedBindings(oldBinding, newBinding, out var rebindingMap))
		{
			return;
		}
		ProxyBinding oldBinding2 = default(ProxyBinding);
		List<BindingPair> list = default(List<BindingPair>);
		foreach (KeyValuePair<ProxyBinding, List<BindingPair>> item in rebindingMap)
		{
			item.Deconstruct(ref oldBinding2, ref list);
			List<BindingPair> list2 = list;
			GetRebindOptions(conflictInfos, list2);
		}
		unsolved = conflictInfos.Values.Any((ConflictInfoItem c) => (c.options & Options.Unsolved) != 0);
		swap = conflictInfos.Values.All((ConflictInfoItem c) => (c.options & Options.Swap) != Options.None && (c.options & Options.Backward) == 0);
		unset = conflictInfos.Values.All((ConflictInfoItem c) => (c.options & Options.Unset) != Options.None && (c.options & Options.Backward) == 0);
		batchSwap = conflictInfos.Values.Any((ConflictInfoItem c) => (c.options & Options.Backward) != 0);
		int count = conflictInfos.Count;
		foreach (KeyValuePair<ProxyBinding, List<BindingPair>> item2 in rebindingMap)
		{
			item2.Deconstruct(ref oldBinding2, ref list);
			List<BindingPair> list3 = list;
			foreach (BindingPair item3 in list3)
			{
				item3.Deconstruct(out oldBinding2, out var newBinding2);
				ProxyBinding binding = oldBinding2;
				ProxyBinding resolution = newBinding2;
				ConflictInfoItem conflictInfoItem = new ConflictInfoItem
				{
					binding = binding,
					resolution = resolution
				};
				if (!conflictInfos.ContainsKey(conflictInfoItem.binding.title))
				{
					CollectAliases(conflictInfos, conflictInfoItem);
					if ((rebindingMap.Count > 1 || list3.Count > 1 || conflictInfos.Count != count) | batchSwap)
					{
						conflictInfos.TryAdd(conflictInfoItem.binding.title, conflictInfoItem);
					}
				}
			}
		}
		batchSwap |= conflictInfos.Count != count;
		swap &= !batchSwap;
		unset &= !batchSwap;
	}

	private void GetRebindOptions(Dictionary<string, ConflictInfoItem> conflictInfos, List<BindingPair> list)
	{
		List<BindingPair> list2 = new List<BindingPair>();
		List<BindingPair> list3 = new List<BindingPair>();
		Usages otherUsages = new Usages(0, readOnly: false);
		Usages otherUsages2 = new Usages(0, readOnly: false);
		ProxyBinding newBinding;
		ProxyBinding oldBinding;
		foreach (BindingPair item in list)
		{
			item.Deconstruct(out newBinding, out oldBinding);
			ProxyBinding proxyBinding = newBinding;
			ProxyBinding proxyBinding2 = oldBinding;
			CollectBindingConflicts(list2, proxyBinding, proxyBinding2);
			CollectBindingConflicts(list3, proxyBinding2, proxyBinding);
			otherUsages2 = Usages.Combine(otherUsages2, proxyBinding2.usages);
		}
		foreach (BindingPair item2 in list)
		{
			item2.Deconstruct(out oldBinding, out newBinding);
			ProxyBinding x = oldBinding;
			ProxyBinding y = newBinding;
			if (ProxyBinding.PathEquals(x, y))
			{
				ProcessConflict(conflictInfos, list3, ref otherUsages2, ref otherUsages2, Options.None, out var _);
			}
		}
		bool changed2 = true;
		while (changed2)
		{
			ProcessConflict(conflictInfos, list3, ref otherUsages2, ref otherUsages, Options.Forward, out changed2);
			if (changed2)
			{
				ProcessConflict(conflictInfos, list2, ref otherUsages, ref otherUsages2, Options.Backward, out changed2);
				if (!changed2)
				{
					break;
				}
				continue;
			}
			break;
		}
	}

	private void CollectBindingConflicts(List<BindingPair> conflicts, ProxyBinding toCheck, ProxyBinding resolution)
	{
		if (!InputManager.instance.keyActionMap.TryGetValue(toCheck.path, out var value))
		{
			return;
		}
		ProxyAction action = toCheck.action;
		InputManager.DeviceType deviceType = default(InputManager.DeviceType);
		ProxyComposite proxyComposite = default(ProxyComposite);
		ActionComponent actionComponent = default(ActionComponent);
		ProxyBinding proxyBinding = default(ProxyBinding);
		foreach (ProxyAction item3 in value)
		{
			foreach (KeyValuePair<InputManager.DeviceType, ProxyComposite> composite2 in item3.composites)
			{
				composite2.Deconstruct(ref deviceType, ref proxyComposite);
				ProxyComposite proxyComposite2 = proxyComposite;
				if (proxyComposite2.isDummy || proxyComposite2.m_Device != toCheck.device)
				{
					continue;
				}
				bool flag = InputManager.CanConflict(action, item3, proxyComposite2.m_Device);
				foreach (KeyValuePair<ActionComponent, ProxyBinding> binding in proxyComposite2.bindings)
				{
					binding.Deconstruct(ref actionComponent, ref proxyBinding);
					ProxyBinding proxyBinding2 = proxyBinding;
					if ((!flag && ProxyBinding.componentComparer.Equals(proxyBinding2, toCheck)) || !ProxyBinding.PathEquals(proxyBinding2, toCheck) || proxyBinding2.usages.isNone)
					{
						continue;
					}
					BindingPair item = new BindingPair(proxyBinding2, resolution);
					if (!conflicts.Contains(item))
					{
						conflicts.Add(item);
					}
					foreach (ProxyAction.LinkInfo linkedAction in item3.m_LinkedActions)
					{
						if (linkedAction.m_Device == proxyBinding2.device && linkedAction.m_Action.TryGetComposite(proxyBinding2.device, out var composite) && composite.TryGetBinding(proxyBinding2.component, out var foundBinding) && !foundBinding.usages.isNone)
						{
							ProxyBinding newBinding = resolution.Copy();
							if (!foundBinding.isModifiersRebindable)
							{
								newBinding.modifiers = foundBinding.modifiers;
							}
							BindingPair item2 = new BindingPair(foundBinding, newBinding);
							if (!conflicts.Contains(item2))
							{
								conflicts.Add(item2);
							}
						}
					}
				}
			}
		}
	}

	private bool CollectLinkedBindings(ProxyBinding oldBinding, ProxyBinding newBinding, out Dictionary<ProxyBinding, List<BindingPair>> rebindingMap)
	{
		rebindingMap = new Dictionary<ProxyBinding, List<BindingPair>>(ProxyBinding.pathAndModifiersComparer) { 
		{
			oldBinding,
			new List<BindingPair>
			{
				new BindingPair(oldBinding, newBinding)
			}
		} };
		ProxyAction action = oldBinding.action;
		if (action == null)
		{
			return true;
		}
		InputManager.DeviceType deviceType = default(InputManager.DeviceType);
		ProxyComposite proxyComposite = default(ProxyComposite);
		ActionComponent actionComponent = default(ActionComponent);
		ProxyBinding proxyBinding = default(ProxyBinding);
		foreach (ProxyAction.LinkInfo linkedAction in action.m_LinkedActions)
		{
			if (linkedAction.m_Device != oldBinding.device)
			{
				continue;
			}
			foreach (KeyValuePair<InputManager.DeviceType, ProxyComposite> composite in linkedAction.m_Action.composites)
			{
				composite.Deconstruct(ref deviceType, ref proxyComposite);
				ProxyComposite proxyComposite2 = proxyComposite;
				if (proxyComposite2.isDummy)
				{
					continue;
				}
				foreach (KeyValuePair<ActionComponent, ProxyBinding> binding in proxyComposite2.bindings)
				{
					binding.Deconstruct(ref actionComponent, ref proxyBinding);
					ProxyBinding proxyBinding2 = proxyBinding;
					if (ProxyBinding.componentComparer.Equals(oldBinding, proxyBinding2))
					{
						if (!proxyBinding2.isRebindable)
						{
							return false;
						}
						ProxyBinding newBinding2 = proxyBinding2.Copy();
						newBinding2.path = newBinding.path;
						if (newBinding2.allowModifiers && newBinding2.isModifiersRebindable)
						{
							newBinding2.modifiers = newBinding.modifiers;
						}
						if (!rebindingMap.TryGetValue(proxyBinding2, out var value))
						{
							value = new List<BindingPair>();
							rebindingMap[proxyBinding2] = value;
						}
						value.Add(new BindingPair(proxyBinding2, newBinding2));
					}
				}
			}
		}
		return true;
	}

	private void CollectAliases(Dictionary<string, ConflictInfoItem> conflictInfos, ConflictInfoItem mainInfo)
	{
		foreach (UIBaseInputAction uIAlias in mainInfo.binding.action.m_UIAliases)
		{
			if ((Object)(object)mainInfo.binding.alies == (Object)(object)uIAlias || !uIAlias.showInOptions)
			{
				continue;
			}
			foreach (UIInputActionPart actionPart in uIAlias.actionParts)
			{
				if ((actionPart.m_Transform == UIBaseInputAction.Transform.None || (mainInfo.binding.component.ToTransform() & actionPart.m_Transform) != UIBaseInputAction.Transform.None) && (actionPart.m_Mask & mainInfo.binding.device) != InputManager.DeviceType.None)
				{
					ProxyBinding binding = mainInfo.binding.Copy();
					binding.alies = uIAlias;
					ProxyBinding resolution = mainInfo.resolution.Copy();
					resolution.alies = uIAlias;
					conflictInfos.TryAdd(binding.title, new ConflictInfoItem
					{
						binding = binding,
						resolution = resolution,
						options = mainInfo.options,
						isAlias = true
					});
				}
			}
		}
		if (mainInfo.binding.isAlias)
		{
			ProxyBinding binding2 = mainInfo.binding.Copy();
			binding2.alies = null;
			ProxyBinding resolution2 = mainInfo.resolution.Copy();
			resolution2.alies = null;
			conflictInfos.TryAdd(binding2.title, new ConflictInfoItem
			{
				binding = binding2,
				resolution = resolution2,
				options = mainInfo.options,
				isAlias = true
			});
		}
	}

	private void ProcessConflict(Dictionary<string, ConflictInfoItem> conflictInfos, List<BindingPair> bindingConflicts, ref Usages usages, ref Usages otherUsages, Options direction, out bool changed)
	{
		changed = false;
		for (int i = 0; i < bindingConflicts.Count; i++)
		{
			var (y, x) = bindingConflicts[i];
			if (!Usages.TestAny(usages, y.usages))
			{
				continue;
			}
			bool flag = CanSwap(x, y, direction == Options.Backward);
			bool canBeEmpty = y.canBeEmpty;
			if (!flag && !canBeEmpty)
			{
				AddToConflictInfos(new ConflictInfoItem
				{
					binding = y.Copy(),
					resolution = y.Copy(),
					options = (direction | Options.Unsolved)
				});
				changed = true;
			}
			else if (flag)
			{
				ProxyBinding resolution = y.Copy();
				resolution.path = x.path;
				if (y.allowModifiers && y.isModifiersRebindable)
				{
					resolution.modifiers = x.modifiers;
				}
				AddToConflictInfos(new ConflictInfoItem
				{
					binding = y.Copy(),
					resolution = resolution,
					options = (direction | Options.Swap)
				});
				changed = true;
				otherUsages = Usages.Combine(otherUsages, y.usages);
				foreach (ProxyAction.LinkInfo linkedAction in y.action.m_LinkedActions)
				{
					if (linkedAction.m_Device == x.device && linkedAction.m_Action.TryGetComposite(x.device, out var composite) && composite.TryGetBinding(x.component, out var foundBinding))
					{
						usages = Usages.Combine(usages, foundBinding.usages);
					}
				}
			}
			else if (canBeEmpty)
			{
				ProxyBinding resolution2 = y.Copy();
				resolution2.path = string.Empty;
				resolution2.modifiers = Array.Empty<ProxyModifier>();
				AddToConflictInfos(new ConflictInfoItem
				{
					binding = y.Copy(),
					resolution = resolution2,
					options = (direction | Options.Unset)
				});
				changed = true;
			}
			bindingConflicts.RemoveAt(i);
			i--;
		}
		void AddToConflictInfos(ConflictInfoItem info)
		{
			if (conflictInfos.TryAdd(info.binding.title, info))
			{
				CollectAliases(conflictInfos, info);
			}
		}
	}

	private static bool CanSwap(ProxyBinding x, ProxyBinding y, bool checkUsage)
	{
		if (!x.isSet || !y.isSet)
		{
			return false;
		}
		if (!x.isRebindable || !y.isRebindable)
		{
			return false;
		}
		if (ProxyBinding.PathEquals(x, y))
		{
			return false;
		}
		if (checkUsage && Usages.TestAny(x.usages, y.usages))
		{
			return false;
		}
		if (ProxyBinding.defaultModifiersComparer.Equals(x.modifiers, y.modifiers))
		{
			return true;
		}
		if (!x.allowModifiers || !y.allowModifiers)
		{
			return false;
		}
		if (!x.isModifiersRebindable || !y.isModifiersRebindable)
		{
			return false;
		}
		return true;
	}

	private void Apply(ProxyBinding newBinding)
	{
		using (InputManager.DeferUpdating())
		{
			m_OnSetBinding?.Invoke(newBinding);
		}
	}

	private void Reset()
	{
		m_ActiveRebindingBinding.Update((ProxyBinding?)null);
		m_ActiveConflictBinding.Update((ConflictInfo?)null);
		m_ModifierOperation.Reset();
		m_ActiveRebinding = null;
		m_OnSetBinding = null;
		m_PendingRebinding = null;
		m_Conflicts.Clear();
	}

	private void OnModifierPotentialMatch(RebindingOperation operation)
	{
	}

	private void OnModifierApplyBinding(RebindingOperation operation, string path)
	{
	}

	[Preserve]
	public InputRebindingUISystem()
	{
	}
}
