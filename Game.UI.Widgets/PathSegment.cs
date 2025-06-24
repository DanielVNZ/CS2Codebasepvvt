using System;
using cohtml.Net;
using Colossal.Annotations;
using Colossal.UI.Binding;

namespace Game.UI.Widgets;

public struct PathSegment : IJsonWritable, IJsonReadable, IEquatable<PathSegment>
{
	[CanBeNull]
	public string m_Key;

	public int m_Index;

	public static PathSegment Empty => new PathSegment
	{
		m_Index = -1
	};

	public PathSegment([NotNull] string key)
	{
		m_Key = key;
		m_Index = -1;
	}

	public PathSegment(int index)
	{
		m_Key = null;
		m_Index = index;
	}

	public void Write(IJsonWriter writer)
	{
		if (m_Key != null)
		{
			writer.Write(m_Key);
		}
		else if (m_Index != -1)
		{
			writer.Write(m_Index);
		}
		else
		{
			writer.WriteNull();
		}
	}

	public void Read(IJsonReader reader)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		ValueType val = reader.PeekValueType();
		if ((int)val == 4)
		{
			reader.Read(ref m_Key);
			m_Index = -1;
		}
		else if ((int)val == 3)
		{
			m_Key = null;
			reader.Read(ref m_Index);
		}
		else
		{
			reader.SkipValue();
			m_Key = null;
			m_Index = -1;
		}
	}

	public bool Equals(PathSegment other)
	{
		if (m_Key == other.m_Key)
		{
			return m_Index == other.m_Index;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is PathSegment other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((m_Key != null) ? m_Key.GetHashCode() : 0) * 397) ^ m_Index;
	}

	public static bool operator ==(PathSegment left, PathSegment right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(PathSegment left, PathSegment right)
	{
		return !left.Equals(right);
	}

	public override string ToString()
	{
		return m_Key ?? m_Index.ToString();
	}

	public static implicit operator PathSegment(string key)
	{
		return new PathSegment(key);
	}

	public static implicit operator PathSegment(int index)
	{
		return new PathSegment(index);
	}
}
