using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct ElectricityBuildingConnection : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TransformerNode;

	public Entity m_ProducerEdge;

	public Entity m_ConsumerEdge;

	public Entity m_ChargeEdge;

	public Entity m_DischargeEdge;

	public Entity GetProducerNode(ref ComponentLookup<ElectricityFlowEdge> flowEdges)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return flowEdges[m_ProducerEdge].m_End;
	}

	public Entity GetConsumerNode(ref ComponentLookup<ElectricityFlowEdge> flowEdges)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return flowEdges[m_ConsumerEdge].m_Start;
	}

	public Entity GetChargeNode(ref ComponentLookup<ElectricityFlowEdge> flowEdges)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return flowEdges[m_ChargeEdge].m_Start;
	}

	public Entity GetDischargeNode(ref ComponentLookup<ElectricityFlowEdge> flowEdges)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return flowEdges[m_DischargeEdge].m_End;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Entity transformerNode = m_TransformerNode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(transformerNode);
		Entity producerEdge = m_ProducerEdge;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(producerEdge);
		Entity consumerEdge = m_ConsumerEdge;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(consumerEdge);
		Entity chargeEdge = m_ChargeEdge;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(chargeEdge);
		Entity dischargeEdge = m_DischargeEdge;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dischargeEdge);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity transformerNode = ref m_TransformerNode;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref transformerNode);
		ref Entity producerEdge = ref m_ProducerEdge;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref producerEdge);
		ref Entity consumerEdge = ref m_ConsumerEdge;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref consumerEdge);
		ref Entity chargeEdge = ref m_ChargeEdge;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref chargeEdge);
		ref Entity dischargeEdge = ref m_DischargeEdge;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref dischargeEdge);
	}
}
