using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Pathfind;

[InternalBufferCapacity(0)]
public struct PathInformations : IBufferElementData, ISerializable
{
	public Entity m_Origin;

	public Entity m_Destination;

	public float m_Distance;

	public float m_Duration;

	public float m_TotalCost;

	public PathMethod m_Methods;

	public PathFlags m_State;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity origin = m_Origin;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(origin);
		Entity destination = m_Destination;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(destination);
		float distance = m_Distance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(distance);
		float duration = m_Duration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(duration);
		float totalCost = m_TotalCost;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(totalCost);
		PathMethod methods = m_Methods;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)methods);
		PathFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)state);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity origin = ref m_Origin;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref origin);
		ref Entity destination = ref m_Destination;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref destination);
		ref float distance = ref m_Distance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref distance);
		ref float duration = ref m_Duration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref duration);
		ref float totalCost = ref m_TotalCost;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref totalCost);
		ushort methods = default(ushort);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref methods);
		ushort state = default(ushort);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		m_Methods = (PathMethod)methods;
		m_State = (PathFlags)state;
		if ((m_State & PathFlags.Pending) != 0)
		{
			m_State &= ~PathFlags.Pending;
			m_State |= PathFlags.Obsolete;
		}
	}
}
