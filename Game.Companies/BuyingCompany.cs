using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct BuyingCompany : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_LastTradePartner;

	public float m_MeanInputTripLength;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity lastTradePartner = m_LastTradePartner;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastTradePartner);
		float meanInputTripLength = m_MeanInputTripLength;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(meanInputTripLength);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity lastTradePartner = ref m_LastTradePartner;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastTradePartner);
		ref float meanInputTripLength = ref m_MeanInputTripLength;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref meanInputTripLength);
	}
}
