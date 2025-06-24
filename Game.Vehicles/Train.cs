using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct Train : IComponentData, IQueryTypeParameter, ISerializable
{
	public TrainFlags m_Flags;

	public Train(TrainFlags flags)
	{
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)m_Flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (TrainFlags)flags;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.trainPrefabFlags)
		{
			m_Flags |= TrainFlags.Pantograph;
		}
	}
}
