using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct WorkVehicle : IComponentData, IQueryTypeParameter, ISerializable
{
	public WorkVehicleFlags m_State;

	public float m_WorkAmount;

	public float m_DoneAmount;

	public WorkVehicle(WorkVehicleFlags flags, float workAmount)
	{
		m_State = flags;
		m_WorkAmount = workAmount;
		m_DoneAmount = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		WorkVehicleFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		float workAmount = m_WorkAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(workAmount);
		float doneAmount = m_DoneAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(doneAmount);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref float workAmount = ref m_WorkAmount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref workAmount);
		ref float doneAmount = ref m_DoneAmount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref doneAmount);
		m_State = (WorkVehicleFlags)state;
	}
}
