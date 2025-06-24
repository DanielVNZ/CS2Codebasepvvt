using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Areas;

[InternalBufferCapacity(0)]
public struct WoodResource : IBufferElementData, ISerializable
{
	public Entity m_Tree;

	public WoodResource(Entity tree)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Tree = tree;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Tree);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Tree);
	}
}
