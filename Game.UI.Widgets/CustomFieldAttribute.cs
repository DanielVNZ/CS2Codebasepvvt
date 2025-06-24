using System;
using Colossal.Annotations;
using UnityEngine;

namespace Game.UI.Widgets;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class CustomFieldAttribute : PropertyAttribute
{
	[NotNull]
	public Type Factory { get; set; }

	public CustomFieldAttribute([NotNull] Type factory)
	{
		Factory = factory ?? throw new ArgumentNullException("factory");
	}
}
