using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PoliceCarData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_CriminalCapacity;

	public float m_CrimeReductionRate;

	public uint m_ShiftDuration;

	public PolicePurpose m_PurposeMask;

	public PoliceCarData(int criminalCapacity, float crimeReductionRate, uint shiftDuration, PolicePurpose purposeMask)
	{
		m_CriminalCapacity = criminalCapacity;
		m_CrimeReductionRate = crimeReductionRate;
		m_ShiftDuration = shiftDuration;
		m_PurposeMask = purposeMask;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		PolicePurpose purposeMask = m_PurposeMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)purposeMask);
		int criminalCapacity = m_CriminalCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(criminalCapacity);
		float crimeReductionRate = m_CrimeReductionRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(crimeReductionRate);
		uint shiftDuration = m_ShiftDuration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(shiftDuration);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint purposeMask = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref purposeMask);
		ref int criminalCapacity = ref m_CriminalCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref criminalCapacity);
		ref float crimeReductionRate = ref m_CrimeReductionRate;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref crimeReductionRate);
		ref uint shiftDuration = ref m_ShiftDuration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref shiftDuration);
		m_PurposeMask = (PolicePurpose)purposeMask;
	}
}
