using Colossal.Serialization.Entities;
using Game.Citizens;
using Game.Economy;
using Unity.Entities;

namespace Game.Creatures;

public struct Divert : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public Resource m_Resource;

	public int m_Data;

	public Purpose m_Purpose;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		Purpose purpose = m_Purpose;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)purpose);
		int data = m_Data;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(data);
		sbyte num = (sbyte)EconomyUtils.GetResourceIndex(m_Resource);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		byte purpose = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref purpose);
		m_Purpose = (Purpose)purpose;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.divertResources)
		{
			ref int data = ref m_Data;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref data);
			sbyte index = default(sbyte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
			m_Resource = EconomyUtils.GetResource(index);
		}
	}
}
