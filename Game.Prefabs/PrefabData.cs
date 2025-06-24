using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PrefabData : IComponentData, IQueryTypeParameter, IEnableableComponent, ISerializable, ISerializeAsEnabled
{
	public int m_Index;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Index);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Index);
	}
}
