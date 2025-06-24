using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Objects;

public struct Elevation : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Elevation;

	public ElevationFlags m_Flags;

	public Elevation(float elevation, ElevationFlags flags)
	{
		m_Elevation = elevation;
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float elevation = m_Elevation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(elevation);
		ElevationFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref float elevation = ref m_Elevation;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref elevation);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.stackedObjects)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (ElevationFlags)flags;
		}
	}
}
