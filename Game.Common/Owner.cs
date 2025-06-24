using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Common;

public struct Owner : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Owner;

	public Owner(Entity owner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Owner = owner;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Owner);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Owner);
	}
}
