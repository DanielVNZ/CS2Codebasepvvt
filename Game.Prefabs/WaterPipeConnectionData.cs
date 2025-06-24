using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct WaterPipeConnectionData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_FreshCapacity;

	public int m_SewageCapacity;

	public int m_StormCapacity;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int freshCapacity = m_FreshCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(freshCapacity);
		int sewageCapacity = m_SewageCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(sewageCapacity);
		int stormCapacity = m_StormCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(stormCapacity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int freshCapacity = ref m_FreshCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref freshCapacity);
		ref int sewageCapacity = ref m_SewageCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sewageCapacity);
		ref int stormCapacity = ref m_StormCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref stormCapacity);
	}
}
