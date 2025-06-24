using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct CompanyNotifications : IComponentData, IQueryTypeParameter, ISerializable
{
	public short m_NoInputCounter;

	public short m_NoCustomersCounter;

	public Entity m_NoInputEntity;

	public Entity m_NoCustomersEntity;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref short noInputCounter = ref m_NoInputCounter;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref noInputCounter);
		ref short noCustomersCounter = ref m_NoCustomersCounter;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref noCustomersCounter);
		ref Entity noInputEntity = ref m_NoInputEntity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref noInputEntity);
		ref Entity noCustomersEntity = ref m_NoCustomersEntity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref noCustomersEntity);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		short noInputCounter = m_NoInputCounter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(noInputCounter);
		short noCustomersCounter = m_NoCustomersCounter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(noCustomersCounter);
		Entity noInputEntity = m_NoInputEntity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(noInputEntity);
		Entity noCustomersEntity = m_NoCustomersEntity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(noCustomersEntity);
	}
}
