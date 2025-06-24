using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct SlaveLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public SlaveLaneFlags m_Flags;

	public uint m_Group;

	public ushort m_MinIndex;

	public ushort m_MaxIndex;

	public ushort m_SubIndex;

	public ushort m_MasterIndex;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		SlaveLaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		uint num = m_Group;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		ushort subIndex = m_SubIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(subIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref uint reference = ref m_Group;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.laneCountOverflowFix)
		{
			ref ushort subIndex = ref m_SubIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref subIndex);
		}
		else
		{
			byte subIndex2 = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref subIndex2);
			byte b = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
			m_SubIndex = subIndex2;
		}
		m_Flags = (SlaveLaneFlags)flags;
	}
}
