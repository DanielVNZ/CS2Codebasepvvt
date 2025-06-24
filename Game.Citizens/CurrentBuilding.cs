using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct CurrentBuilding : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_CurrentBuilding;

	public CurrentBuilding(Entity building)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_CurrentBuilding = building;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_CurrentBuilding);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_CurrentBuilding);
	}
}
