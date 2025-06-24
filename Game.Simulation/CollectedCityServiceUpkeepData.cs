using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Simulation;

public struct CollectedCityServiceUpkeepData : IBufferElementData, ISerializable
{
	public Resource m_Resource;

	public int m_FullCost;

	public int m_Amount;

	public int m_Cost;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_Resource = (Resource)num;
		ref int fullCost = ref m_FullCost;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref fullCost);
		ref int amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		ref int cost = ref m_Cost;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref cost);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int num = (int)m_Resource;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int fullCost = m_FullCost;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(fullCost);
		int amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		int cost = m_Cost;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cost);
	}
}
