using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct TakeoffLocation : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_AccessRestriction;

	public TakeoffLocationFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity accessRestriction = m_AccessRestriction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accessRestriction);
		TakeoffLocationFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref Entity accessRestriction = ref m_AccessRestriction;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessRestriction);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindRestrictions)
		{
			uint flags = default(uint);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (TakeoffLocationFlags)flags;
		}
	}
}
