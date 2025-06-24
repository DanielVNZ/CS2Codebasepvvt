using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct SchoolSeekerCooldown : IComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_SimulationFrame;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_SimulationFrame);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_SimulationFrame);
	}
}
