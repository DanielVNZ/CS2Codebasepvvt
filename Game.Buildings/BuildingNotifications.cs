using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct BuildingNotifications : IComponentData, IQueryTypeParameter, ISerializable
{
	public BuildingNotification m_Notifications;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_Notifications);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte notifications = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref notifications);
		m_Notifications = (BuildingNotification)notifications;
	}

	public bool HasNotification(BuildingNotification notification)
	{
		return (m_Notifications & notification) != 0;
	}
}
