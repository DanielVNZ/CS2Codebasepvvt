using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct MasterLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_Group;

	public ushort m_MinIndex;

	public ushort m_MaxIndex;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Group);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref uint reference = ref m_Group;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.laneCountOverflowFix)
		{
			byte b = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
		}
	}
}
