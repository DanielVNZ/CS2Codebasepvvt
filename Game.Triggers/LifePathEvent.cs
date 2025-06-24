using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Triggers;

public struct LifePathEvent : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_EventPrefab;

	public Entity m_Target;

	public uint m_Date;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity eventPrefab = m_EventPrefab;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(eventPrefab);
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		uint date = m_Date;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(date);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity eventPrefab = ref m_EventPrefab;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref eventPrefab);
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		ref uint date = ref m_Date;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref date);
	}
}
