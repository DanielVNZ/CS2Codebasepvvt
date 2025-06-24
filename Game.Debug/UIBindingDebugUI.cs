using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.UI.Binding;
using Game.SceneFlow;
using Game.UI.Debug;
using Unity.Entities;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public static class UIBindingDebugUI
{
	private static void Rebuild()
	{
		DebugSystem.Rebuild(BuildUIBindingsDebugUI);
	}

	private static void AddBindingButtonsRecursive(DebugUISystem debugUIsystem, Dictionary<string, Container> containers, IBindingGroup group)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Invalid comparison between Unknown and I4
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Invalid comparison between Unknown and I4
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0133: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Invalid comparison between Unknown and I4
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		foreach (IBinding binding in group.bindings)
		{
			IDebugBinding debugBinding = (IDebugBinding)(object)((binding is IDebugBinding) ? binding : null);
			if (debugBinding != null)
			{
				if (!containers.TryGetValue(debugBinding.group, out var value))
				{
					value = (Container)new Foldout(debugBinding.group, new ObservableList<Widget>(), (string[])null, (string[])null);
					containers.Add(debugBinding.group, value);
				}
				if ((int)debugBinding.debugType == 1)
				{
					value.children.Add((Widget)new Button
					{
						displayName = debugBinding.name,
						action = delegate
						{
							debugUIsystem.Trigger(debugBinding);
						}
					});
				}
				else if ((int)debugBinding.debugType == 2 || (int)debugBinding.debugType == 3)
				{
					ObservableList<Widget> children = value.children;
					BoolField val = new BoolField
					{
						displayName = debugBinding.name
					};
					((Field<bool>)val).getter = () => debugUIsystem.observedBinding == debugBinding;
					((Field<bool>)val).setter = delegate(bool v)
					{
						debugUIsystem.observedBinding = (v ? debugBinding : null);
					};
					children.Add((Widget)val);
				}
				else
				{
					value.children.Add((Widget)new Value
					{
						displayName = debugBinding.name,
						getter = () => FormatGenericTypeString(((object)debugBinding).GetType())
					});
				}
			}
			IBindingGroup val2 = (IBindingGroup)(object)((binding is IBindingGroup) ? binding : null);
			if (val2 != null)
			{
				AddBindingButtonsRecursive(debugUIsystem, containers, val2);
			}
		}
	}

	private static string FormatGenericTypeString(Type t)
	{
		if (!t.IsGenericType)
		{
			return t.Name;
		}
		string name = t.GetGenericTypeDefinition().Name;
		name = name.Substring(0, name.IndexOf('`'));
		string text = string.Join(",", t.GetGenericArguments().Select(FormatGenericTypeString).ToArray());
		return name + "<" + text + ">";
	}

	[DebugTab("UI Bindings", -970)]
	private static List<Widget> BuildUIBindingsDebugUI(World world)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		DebugUISystem debugUISystem = world.GetOrCreateSystemManaged<DebugUISystem>();
		IBindingRegistry bindings = GameManager.instance.userInterface.bindings;
		Dictionary<string, Container> dictionary = new Dictionary<string, Container>();
		if (bindings != null)
		{
			AddBindingButtonsRecursive(debugUISystem, dictionary, (IBindingGroup)(object)bindings);
		}
		List<Widget> list = new List<Widget>();
		list.Add((Widget)new Button
		{
			displayName = "Refresh",
			action = Rebuild
		});
		list.Add((Widget)new Button
		{
			displayName = "Clear",
			action = delegate
			{
				debugUISystem.observedBinding = null;
			}
		});
		list.AddRange((IEnumerable<Widget>)dictionary.Values.OrderBy((Container container) => ((Widget)container).displayName));
		return list;
	}
}
