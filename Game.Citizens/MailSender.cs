using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct MailSender : IComponentData, IQueryTypeParameter, ISerializable, IEnableableComponent
{
	public ushort m_Amount;

	public MailSender(ushort amount)
	{
		m_Amount = amount;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Amount);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Amount);
	}
}
