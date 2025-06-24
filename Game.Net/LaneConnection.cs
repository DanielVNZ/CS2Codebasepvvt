using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct LaneConnection : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_StartLane;

	public Entity m_EndLane;

	public float m_StartPosition;

	public float m_EndPosition;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity startLane = m_StartLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startLane);
		Entity endLane = m_EndLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(endLane);
		float startPosition = m_StartPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startPosition);
		float endPosition = m_EndPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(endPosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity startLane = ref m_StartLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startLane);
		ref Entity endLane = ref m_EndLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref endLane);
		ref float startPosition = ref m_StartPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startPosition);
		ref float endPosition = ref m_EndPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref endPosition);
	}
}
