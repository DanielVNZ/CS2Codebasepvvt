using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct CommuterHousehold : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_OriginalFrom;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_OriginalFrom);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.commuterOriginalFrom)
		{
			ref Entity originalFrom = ref m_OriginalFrom;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref originalFrom);
		}
	}
}
