using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Citizens;

public struct TripNeeded : IBufferElementData, ISerializable
{
	public Entity m_TargetAgent;

	public Purpose m_Purpose;

	public int m_Data;

	public Resource m_Resource;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetAgent = m_TargetAgent;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetAgent);
		Purpose purpose = m_Purpose;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)purpose);
		int data = m_Data;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(data);
		sbyte num = (sbyte)EconomyUtils.GetResourceIndex(m_Resource);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		ref Entity targetAgent = ref m_TargetAgent;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetAgent);
		byte purpose = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref purpose);
		m_Purpose = (Purpose)purpose;
		ref int data = ref m_Data;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref data);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.resource32bitFix)
		{
			sbyte index = default(sbyte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
			m_Resource = EconomyUtils.GetResource(index);
		}
		else
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_Resource = (Resource)num;
		}
	}
}
