using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Agents;

public struct MovingAway : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Target);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Target);
	}
}
