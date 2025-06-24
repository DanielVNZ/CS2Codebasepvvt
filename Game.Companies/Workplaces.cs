using System;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Unity.Collections;

namespace Game.Companies;

public struct Workplaces : IAccumulable<Workplaces>, ISerializable
{
	public int m_Uneducated;

	public int m_PoorlyEducated;

	public int m_Educated;

	public int m_WellEducated;

	public int m_HighlyEducated;

	public int TotalCount => m_Uneducated + m_PoorlyEducated + m_Educated + m_WellEducated + m_HighlyEducated;

	public int SimpleWorkplacesCount => m_Uneducated + m_PoorlyEducated;

	public int ComplexWorkplacesCount => m_Educated + m_WellEducated + m_HighlyEducated;

	public int this[int index]
	{
		get
		{
			return index switch
			{
				0 => m_Uneducated, 
				1 => m_PoorlyEducated, 
				2 => m_Educated, 
				3 => m_WellEducated, 
				4 => m_HighlyEducated, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		set
		{
			switch (index)
			{
			case 0:
				m_Uneducated = value;
				break;
			case 1:
				m_PoorlyEducated = value;
				break;
			case 2:
				m_Educated = value;
				break;
			case 3:
				m_WellEducated = value;
				break;
			case 4:
				m_HighlyEducated = value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	public void ToArray(NativeArray<int> array)
	{
		array[0] = m_Uneducated;
		array[1] = m_PoorlyEducated;
		array[2] = m_Educated;
		array[3] = m_WellEducated;
		array[4] = m_HighlyEducated;
	}

	public void Accumulate(Workplaces other)
	{
		m_Uneducated += other.m_Uneducated;
		m_PoorlyEducated += other.m_PoorlyEducated;
		m_Educated += other.m_Educated;
		m_WellEducated += other.m_WellEducated;
		m_HighlyEducated += other.m_HighlyEducated;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int uneducated = m_Uneducated;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(uneducated);
		int poorlyEducated = m_PoorlyEducated;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(poorlyEducated);
		int educated = m_Educated;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(educated);
		int wellEducated = m_WellEducated;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(wellEducated);
		int highlyEducated = m_HighlyEducated;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(highlyEducated);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int uneducated = ref m_Uneducated;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref uneducated);
		ref int poorlyEducated = ref m_PoorlyEducated;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref poorlyEducated);
		ref int educated = ref m_Educated;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref educated);
		ref int wellEducated = ref m_WellEducated;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref wellEducated);
		ref int highlyEducated = ref m_HighlyEducated;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref highlyEducated);
	}
}
