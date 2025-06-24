using System;
using Colossal.Annotations;
using Unity.Collections;

namespace Game.Reflection;

public class NativeArrayElementAccessor<T> : ITypedValueAccessor<T>, IValueAccessor, IEquatable<NativeArrayElementAccessor<T>> where T : struct
{
	[NotNull]
	private readonly ITypedValueAccessor<NativeArray<T>> m_Parent;

	private readonly int m_Index;

	public Type valueType => typeof(T);

	public NativeArrayElementAccessor([NotNull] ITypedValueAccessor<NativeArray<T>> parent, int index)
	{
		m_Parent = parent ?? throw new ArgumentNullException("parent");
		m_Index = index;
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return m_Parent.GetTypedValue()[m_Index];
	}

	public void SetTypedValue(T value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<T> typedValue = m_Parent.GetTypedValue();
		typedValue[m_Index] = value;
	}

	public bool Equals(NativeArrayElementAccessor<T> other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (m_Parent.Equals(other.m_Parent))
		{
			return m_Index == other.m_Index;
		}
		return false;
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
		return Equals((NativeArrayElementAccessor<T>)obj);
	}

	public override int GetHashCode()
	{
		return (m_Parent.GetHashCode() * 397) ^ m_Index;
	}
}
