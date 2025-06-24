using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct CollectedServiceBuildingBudgetData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Count;

	public int m_Workers;

	public int m_Workplaces;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int count = ref m_Count;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref count);
		ref int workers = ref m_Workers;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref workers);
		ref int workplaces = ref m_Workplaces;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref workplaces);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int count = m_Count;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(count);
		int workers = m_Workers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(workers);
		int workplaces = m_Workplaces;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(workplaces);
	}
}
