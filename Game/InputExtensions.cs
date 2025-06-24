using System;
using UnityEngine.InputSystem;

namespace Game;

public static class InputExtensions
{
	public static bool TryGetCompositeOfActionWithName(this InputAction action, string compositeName, out BindingSyntax iterator)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		BindingSyntax val = new BindingSyntax(action.actionMap, -1, action);
		iterator = ((BindingSyntax)(ref val)).NextCompositeBinding((string)null);
		InputBinding binding;
		while (((BindingSyntax)(ref iterator)).valid)
		{
			binding = ((BindingSyntax)(ref iterator)).binding;
			if (!((InputBinding)(ref binding)).TriggersAction(action))
			{
				iterator = ((BindingSyntax)(ref iterator)).NextCompositeBinding((string)null);
				continue;
			}
			break;
		}
		while (((BindingSyntax)(ref iterator)).valid)
		{
			binding = ((BindingSyntax)(ref iterator)).binding;
			if (!((InputBinding)(ref binding)).TriggersAction(action))
			{
				break;
			}
			binding = ((BindingSyntax)(ref iterator)).binding;
			if (!(((InputBinding)(ref binding)).name != compositeName))
			{
				break;
			}
			iterator = ((BindingSyntax)(ref iterator)).NextCompositeBinding((string)null);
		}
		if (((BindingSyntax)(ref iterator)).valid)
		{
			binding = ((BindingSyntax)(ref iterator)).binding;
			return ((InputBinding)(ref binding)).TriggersAction(action);
		}
		return false;
	}

	public static bool TryGetFirstCompositeOfAction(this InputAction action, out BindingSyntax iterator)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		BindingSyntax val = new BindingSyntax(action.actionMap, -1, action);
		iterator = ((BindingSyntax)(ref val)).NextCompositeBinding((string)null);
		InputBinding binding;
		while (((BindingSyntax)(ref iterator)).valid)
		{
			binding = ((BindingSyntax)(ref iterator)).binding;
			if (((InputBinding)(ref binding)).TriggersAction(action))
			{
				break;
			}
			iterator = ((BindingSyntax)(ref iterator)).NextCompositeBinding((string)null);
		}
		if (((BindingSyntax)(ref iterator)).valid)
		{
			binding = ((BindingSyntax)(ref iterator)).binding;
			return ((InputBinding)(ref binding)).TriggersAction(action);
		}
		return false;
	}

	public static bool ForEachCompositeOfAction(this InputAction inputAction, BindingSyntax startIterator, Func<BindingSyntax, bool> action, out BindingSyntax endIterator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		endIterator = startIterator;
		if (action == null)
		{
			return false;
		}
		InputBinding binding = ((BindingSyntax)(ref startIterator)).binding;
		if (!((InputBinding)(ref binding)).isComposite)
		{
			startIterator = ((BindingSyntax)(ref startIterator)).NextCompositeBinding((string)null);
		}
		while (((BindingSyntax)(ref startIterator)).valid)
		{
			binding = ((BindingSyntax)(ref startIterator)).binding;
			if (!((InputBinding)(ref binding)).TriggersAction(inputAction))
			{
				break;
			}
			if (!action(startIterator))
			{
				return false;
			}
			endIterator = startIterator;
			startIterator = ((BindingSyntax)(ref startIterator)).NextCompositeBinding((string)null);
		}
		return true;
	}

	public static bool ForEachCompositeOfAction(this InputAction inputAction, Func<BindingSyntax, bool> action)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (action == null)
		{
			return false;
		}
		if (!inputAction.TryGetFirstCompositeOfAction(out var iterator))
		{
			return false;
		}
		BindingSyntax endIterator;
		return inputAction.ForEachCompositeOfAction(iterator, action, out endIterator);
	}

	public static bool ForEachPartOfCompositeWithName(this InputAction inputAction, BindingSyntax startIterator, string partName, Func<BindingSyntax, bool> action, out BindingSyntax endIterator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		endIterator = startIterator;
		if (string.IsNullOrEmpty(partName))
		{
			return false;
		}
		if (action == null)
		{
			return false;
		}
		InputBinding binding = ((BindingSyntax)(ref startIterator)).binding;
		if (((InputBinding)(ref binding)).isComposite)
		{
			startIterator = ((BindingSyntax)(ref startIterator)).NextPartBinding(partName);
		}
		while (((BindingSyntax)(ref startIterator)).valid)
		{
			binding = ((BindingSyntax)(ref startIterator)).binding;
			if (!((InputBinding)(ref binding)).isPartOfComposite)
			{
				break;
			}
			binding = ((BindingSyntax)(ref startIterator)).binding;
			if (!((InputBinding)(ref binding)).TriggersAction(inputAction))
			{
				break;
			}
			if (!action(startIterator))
			{
				return false;
			}
			endIterator = startIterator;
			startIterator = ((BindingSyntax)(ref startIterator)).NextPartBinding(partName);
		}
		return true;
	}

	public static bool ForEachPartOfCompositeWithName(this InputAction inputAction, string partName, Func<BindingSyntax, bool> action)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (action == null)
		{
			return false;
		}
		if (!inputAction.TryGetFirstCompositeOfAction(out var iterator))
		{
			return false;
		}
		BindingSyntax endIterator;
		return inputAction.ForEachPartOfCompositeWithName(iterator, partName, action, out endIterator);
	}
}
