using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Colossal;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Game.Input;

[DebuggerDisplay("{name}")]
public class ProxyActionMap
{
	private readonly InputActionMap m_SourceMap;

	private readonly Dictionary<string, ProxyAction> m_Actions = new Dictionary<string, ProxyAction>();

	internal HashSet<InputBarrier> m_Barriers = new HashSet<InputBarrier>();

	private bool m_Enabled;

	private InputManager.DeviceType m_Mask = InputManager.DeviceType.All;

	internal InputActionMap sourceMap => m_SourceMap;

	public string name => m_SourceMap.name;

	public IReadOnlyDictionary<string, ProxyAction> actions => m_Actions;

	internal IReadOnlyCollection<InputBarrier> barriers => m_Barriers;

	public IEnumerable<ProxyBinding> bindings
	{
		get
		{
			string text = default(string);
			ProxyAction proxyAction = default(ProxyAction);
			InputManager.DeviceType deviceType = default(InputManager.DeviceType);
			ProxyComposite proxyComposite = default(ProxyComposite);
			ActionComponent actionComponent = default(ActionComponent);
			ProxyBinding proxyBinding = default(ProxyBinding);
			foreach (KeyValuePair<string, ProxyAction> action in m_Actions)
			{
				action.Deconstruct(ref text, ref proxyAction);
				ProxyAction proxyAction2 = proxyAction;
				foreach (KeyValuePair<InputManager.DeviceType, ProxyComposite> composite in proxyAction2.composites)
				{
					composite.Deconstruct(ref deviceType, ref proxyComposite);
					ProxyComposite proxyComposite2 = proxyComposite;
					foreach (KeyValuePair<ActionComponent, ProxyBinding> binding in proxyComposite2.bindings)
					{
						binding.Deconstruct(ref actionComponent, ref proxyBinding);
						yield return proxyBinding;
					}
				}
			}
		}
	}

	public bool enabled => m_Enabled;

	public InputManager.DeviceType mask
	{
		get
		{
			return m_Mask;
		}
		internal set
		{
			if (value == m_Mask)
			{
				return;
			}
			m_Mask = value;
			m_SourceMap.bindingMask = value.ToInputBinding();
			string text = default(string);
			ProxyAction proxyAction = default(ProxyAction);
			foreach (KeyValuePair<string, ProxyAction> action in m_Actions)
			{
				action.Deconstruct(ref text, ref proxyAction);
				proxyAction.UpdateState();
			}
		}
	}

	internal ProxyActionMap(InputActionMap sourceMap)
	{
		m_SourceMap = sourceMap;
	}

	internal void InitActions()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<InputAction> enumerator = sourceMap.actions.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				InputAction current = enumerator.Current;
				ProxyAction proxyAction = new ProxyAction(this, current);
				m_Actions.Add(proxyAction.name, proxyAction);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		UpdateState();
	}

	public ProxyAction FindAction(string name)
	{
		return MiscHelpers.GetValueOrDefault<string, ProxyAction>(m_Actions, name);
	}

	internal ProxyAction FindAction(InputAction action)
	{
		return FindAction(action.name);
	}

	public bool TryFindAction(string name, out ProxyAction action)
	{
		return m_Actions.TryGetValue(name, out action);
	}

	public ProxyAction AddAction(ProxyAction.Info actionInfo, bool bulk = false)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		PerformanceCounter val = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
		{
			InputManager.log.InfoFormat("Action \"{1}\" added in {0}ms", (object)t.TotalMilliseconds, (object)actionInfo.m_Name);
		});
		try
		{
			using (InputManager.DeferUpdating())
			{
				if (TryFindAction(actionInfo.m_Name, out var action))
				{
					return action;
				}
				InputAction val2 = InputActionSetupExtensions.AddAction(m_SourceMap, actionInfo.m_Name, actionInfo.m_Type.GetInputActionType(), (string)null, (string)null, (string)null, (string)null, actionInfo.m_Type.GetExpectedControlLayout());
				foreach (ProxyComposite.Info composite in actionInfo.m_Composites)
				{
					InputManager.instance.CreateCompositeBinding(val2, composite);
				}
				action = new ProxyAction(this, val2);
				m_Actions.Add(action.name, action);
				InputManager.instance.InitializeMasks(action);
				return action;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal void UpdateState()
	{
		bool flag = m_Barriers.All((InputBarrier b) => !b.blocked);
		if (flag == m_Enabled)
		{
			return;
		}
		m_Enabled = flag;
		string text = default(string);
		ProxyAction proxyAction = default(ProxyAction);
		foreach (KeyValuePair<string, ProxyAction> action in m_Actions)
		{
			action.Deconstruct(ref text, ref proxyAction);
			proxyAction.UpdateState();
		}
	}
}
