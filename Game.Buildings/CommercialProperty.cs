using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Buildings;

public struct CommercialProperty : IComponentData, IQueryTypeParameter, ISerializable
{
	public Resource m_Resources;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)m_Resources);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint num = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_Resources = (Resource)num;
	}
}
