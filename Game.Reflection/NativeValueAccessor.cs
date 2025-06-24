using System;
using Colossal.Annotations;
using Colossal.Collections;

namespace Game.Reflection;

public class NativeValueAccessor<T> : ITypedValueAccessor<T>, IValueAccessor, IEquatable<NativeValueAccessor<T>> where T : unmanaged
{
	[NotNull]
	private readonly IValueAccessor m_Parent;

	public Type valueType => typeof(T);

	public NativeValueAccessor([NotNull] IValueAccessor parent)
	{
		m_Parent = parent ?? throw new ArgumentNullException("parent");
	}

	public object GetValue()
	{
		return GetTypedValue();
	}

	public void SetValue(object value)
	{
		SetTypedValue((T)value);
	}

	public T GetTypedValue()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return ((NativeValue<T>)m_Parent.GetValue()).value;
	}

	public void SetTypedValue(T value)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		NativeValue<T> val = (NativeValue<T>)m_Parent.GetValue();
		val.value = value;
	}

	public bool Equals(NativeValueAccessor<T> other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return m_Parent.Equals(other.m_Parent);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((NativeValueAccessor<T>)obj);
	}

	public override int GetHashCode()
	{
		return m_Parent.GetHashCode();
	}
}
