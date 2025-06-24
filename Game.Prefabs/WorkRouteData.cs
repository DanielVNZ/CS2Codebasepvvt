using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public struct WorkRouteData : IComponentData, IQueryTypeParameter, ISerializable
{
	public SizeClass m_SizeClass;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_SizeClass);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte sizeClass = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sizeClass);
		m_SizeClass = (SizeClass)sizeClass;
	}
}
