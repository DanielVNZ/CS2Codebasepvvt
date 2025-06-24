using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct CitizenPresence : IComponentData, IQueryTypeParameter, ISerializable
{
	public sbyte m_Delta;

	public byte m_Presence;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		sbyte delta = m_Delta;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(delta);
		byte presence = m_Presence;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(presence);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref sbyte delta = ref m_Delta;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref delta);
		ref byte presence = ref m_Presence;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref presence);
	}
}
