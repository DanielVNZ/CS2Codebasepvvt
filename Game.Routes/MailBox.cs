using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct MailBox : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_CollectRequest;

	public int m_MailAmount;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity collectRequest = m_CollectRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(collectRequest);
		int mailAmount = m_MailAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(mailAmount);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity collectRequest = ref m_CollectRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref collectRequest);
		ref int mailAmount = ref m_MailAmount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref mailAmount);
	}
}
