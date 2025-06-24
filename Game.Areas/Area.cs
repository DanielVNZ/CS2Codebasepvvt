using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Areas;

public struct Area : IComponentData, IQueryTypeParameter, ISerializable
{
	public AreaFlags m_Flags;

	public Area(AreaFlags flags)
	{
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)m_Flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (AreaFlags)flags;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.mapTileCompleteFix)
		{
			m_Flags |= AreaFlags.Complete;
		}
	}
}
