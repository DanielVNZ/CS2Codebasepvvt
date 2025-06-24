using System;
using Colossal.UI.Binding;

namespace Game.UI.Widgets;

public class Button : NamedWidgetWithTooltip, IInvokable, IWidget, IJsonWritable, IDisableCallback
{
	public Action action { get; set; }

	public void Invoke()
	{
		action();
	}
}
