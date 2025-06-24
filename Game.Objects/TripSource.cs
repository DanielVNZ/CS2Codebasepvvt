using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct TripSource : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Source;

	public int m_Timer;

	public TripSource(Entity source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Source = source;
		m_Timer = 0;
	}

	public TripSource(Entity source, uint delay)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Source = source;
		m_Timer = (int)delay;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity source = m_Source;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(source);
		int timer = m_Timer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(timer);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity source = ref m_Source;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref source);
		ref int timer = ref m_Timer;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref timer);
	}
}
