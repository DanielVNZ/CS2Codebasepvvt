using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.City;

public struct ServiceImportBudget : IBufferElementData, ISerializable
{
	public PlayerResource m_Resource;

	public int m_MaximumBudget;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		PlayerResource resource = m_Resource;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)resource);
		int maximumBudget = m_MaximumBudget;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maximumBudget);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		int resource = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref resource);
		m_Resource = (PlayerResource)resource;
		ref int maximumBudget = ref m_MaximumBudget;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maximumBudget);
	}
}
