using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct Employee : IBufferElementData, ISerializable
{
	public Entity m_Worker;

	public byte m_Level;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity worker = m_Worker;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(worker);
		byte level = m_Level;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(level);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity worker = ref m_Worker;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref worker);
		ref byte level = ref m_Level;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref level);
	}
}
