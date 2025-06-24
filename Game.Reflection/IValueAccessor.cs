using System;

namespace Game.Reflection;

public interface IValueAccessor
{
	Type valueType { get; }

	object GetValue();

	void SetValue(object value);
}
