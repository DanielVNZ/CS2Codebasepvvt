using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PoliceStationData : IComponentData, IQueryTypeParameter, ICombineData<PoliceStationData>, ISerializable
{
	public int m_PatrolCarCapacity;

	public int m_PoliceHelicopterCapacity;

	public int m_JailCapacity;

	public PolicePurpose m_PurposeMask;

	public void Combine(PoliceStationData otherData)
	{
		m_PatrolCarCapacity += otherData.m_PatrolCarCapacity;
		m_PoliceHelicopterCapacity += otherData.m_PoliceHelicopterCapacity;
		m_JailCapacity += otherData.m_JailCapacity;
		m_PurposeMask |= otherData.m_PurposeMask;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int patrolCarCapacity = m_PatrolCarCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(patrolCarCapacity);
		int policeHelicopterCapacity = m_PoliceHelicopterCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(policeHelicopterCapacity);
		int jailCapacity = m_JailCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(jailCapacity);
		byte num = (byte)m_PurposeMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int patrolCarCapacity = ref m_PatrolCarCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref patrolCarCapacity);
		ref int policeHelicopterCapacity = ref m_PoliceHelicopterCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref policeHelicopterCapacity);
		ref int jailCapacity = ref m_JailCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref jailCapacity);
		byte purposeMask = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref purposeMask);
		m_PurposeMask = (PolicePurpose)purposeMask;
	}
}
