using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct Edge : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Start;

	public Entity m_End;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity start = m_Start;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(start);
		Entity end = m_End;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(end);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity start = ref m_Start;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref start);
		ref Entity end = ref m_End;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref end);
	}
}
