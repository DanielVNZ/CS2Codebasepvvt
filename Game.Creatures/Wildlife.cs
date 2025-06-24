using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Creatures;

public struct Wildlife : IComponentData, IQueryTypeParameter, ISerializable
{
	public WildlifeFlags m_Flags;

	public ushort m_StateTime;

	public ushort m_LifeTime;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		WildlifeFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		ushort stateTime = m_StateTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(stateTime);
		ushort lifeTime = m_LifeTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lifeTime);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref ushort stateTime = ref m_StateTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref stateTime);
		ref ushort lifeTime = ref m_LifeTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lifeTime);
		m_Flags = (WildlifeFlags)flags;
	}
}
