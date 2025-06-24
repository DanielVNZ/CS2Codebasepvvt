using Unity.Entities;

namespace Game.Events;

public struct AddEventJournalData : IComponentData, IQueryTypeParameter
{
	public EventDataTrackingType m_Type;

	public Entity m_Event;

	public int m_Count;

	public AddEventJournalData(Entity eventEntity, EventDataTrackingType type, int count = 1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Event = eventEntity;
		m_Type = type;
		m_Count = count;
	}
}
