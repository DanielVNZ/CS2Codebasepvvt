using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

[InternalBufferCapacity(1)]
public struct PetTrip : IBufferElementData, ISerializable
{
	public Entity m_Source;

	public Entity m_Target;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity source = m_Source;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(source);
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity source = ref m_Source;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref source);
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
	}
}
