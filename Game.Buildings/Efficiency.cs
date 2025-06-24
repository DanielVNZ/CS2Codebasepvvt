using System;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

[InternalBufferCapacity(8)]
public struct Efficiency : IBufferElementData, ISerializable, IComparable<Efficiency>
{
	public EfficiencyFactor m_Factor;

	public float m_Efficiency;

	public Efficiency(EfficiencyFactor factor, float efficiency)
	{
		m_Factor = factor;
		m_Efficiency = efficiency;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		EfficiencyFactor factor = m_Factor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)factor);
		float efficiency = m_Efficiency;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(efficiency);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte factor = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref factor);
		m_Factor = (EfficiencyFactor)factor;
		ref float efficiency = ref m_Efficiency;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref efficiency);
	}

	public int CompareTo(Efficiency other)
	{
		int num = other.m_Efficiency.CompareTo(m_Efficiency);
		if (num != 0)
		{
			return num;
		}
		return m_Factor.CompareTo(other.m_Factor);
	}
}
