using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Citizens;

public struct TravelPurpose : IComponentData, IQueryTypeParameter, ISerializable
{
	public Purpose m_Purpose;

	public int m_Data;

	public Resource m_Resource;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		Purpose purpose = m_Purpose;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)purpose);
		int data = m_Data;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(data);
		sbyte num = (sbyte)EconomyUtils.GetResourceIndex(m_Resource);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte purpose = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref purpose);
		ref int data = ref m_Data;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref data);
		sbyte index = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
		m_Purpose = (Purpose)purpose;
		m_Resource = EconomyUtils.GetResource(index);
	}
}
