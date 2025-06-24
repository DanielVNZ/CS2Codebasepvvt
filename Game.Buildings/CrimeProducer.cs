using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct CrimeProducer : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_PatrolRequest;

	public float m_Crime;

	public byte m_DispatchIndex;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity patrolRequest = m_PatrolRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(patrolRequest);
		float crime = m_Crime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(crime);
		byte dispatchIndex = m_DispatchIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dispatchIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref Entity patrolRequest = ref m_PatrolRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref patrolRequest);
		ref float crime = ref m_Crime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref crime);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.requestDispatchIndex)
		{
			ref byte dispatchIndex = ref m_DispatchIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dispatchIndex);
		}
	}
}
