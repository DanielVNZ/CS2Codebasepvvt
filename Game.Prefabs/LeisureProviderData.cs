using Colossal.Serialization.Entities;
using Game.Agents;
using Game.Economy;
using Unity.Entities;

namespace Game.Prefabs;

public struct LeisureProviderData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Efficiency;

	public Resource m_Resources;

	public LeisureType m_LeisureType;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int efficiency = m_Efficiency;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(efficiency);
		sbyte num = (sbyte)EconomyUtils.GetResourceIndex(m_Resources);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		byte num2 = (byte)m_LeisureType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int efficiency = ref m_Efficiency;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref efficiency);
		sbyte index = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
		byte leisureType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref leisureType);
		m_Resources = EconomyUtils.GetResource(index);
		m_LeisureType = (LeisureType)leisureType;
	}
}
