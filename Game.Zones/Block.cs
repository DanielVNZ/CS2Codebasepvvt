using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Zones;

public struct Block : IComponentData, IQueryTypeParameter, IEquatable<Block>, ISerializable
{
	public float3 m_Position;

	public float2 m_Direction;

	public int2 m_Size;

	public bool Equals(Block other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((float3)(ref m_Position)).Equals(other.m_Position) && ((float2)(ref m_Direction)).Equals(other.m_Direction))
		{
			return ((int2)(ref m_Size)).Equals(other.m_Size);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		float3 position = m_Position;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		float2 direction = m_Direction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(direction);
		int2 size = m_Size;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(size);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float3 position = ref m_Position;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		ref float2 direction = ref m_Direction;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref direction);
		ref int2 size = ref m_Size;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref size);
	}
}
