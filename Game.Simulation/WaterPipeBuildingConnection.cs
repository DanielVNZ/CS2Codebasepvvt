using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct WaterPipeBuildingConnection : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_ProducerEdge;

	public Entity m_ConsumerEdge;

	public Entity GetProducerNode(ref ComponentLookup<WaterPipeEdge> flowEdges)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return flowEdges[m_ProducerEdge].m_End;
	}

	public Entity GetConsumerNode(ref ComponentLookup<WaterPipeEdge> flowEdges)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return flowEdges[m_ConsumerEdge].m_Start;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity producerEdge = m_ProducerEdge;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(producerEdge);
		Entity consumerEdge = m_ConsumerEdge;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(consumerEdge);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity producerEdge = ref m_ProducerEdge;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref producerEdge);
		ref Entity consumerEdge = ref m_ConsumerEdge;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref consumerEdge);
	}
}
