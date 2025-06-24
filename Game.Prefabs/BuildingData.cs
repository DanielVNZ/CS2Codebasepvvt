using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct BuildingData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int2 m_LotSize;

	public BuildingFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		int2 lotSize = m_LotSize;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lotSize);
		BuildingFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int2 lotSize = ref m_LotSize;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lotSize);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (BuildingFlags)flags;
	}
}
