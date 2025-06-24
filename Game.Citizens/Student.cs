using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct Student : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_School;

	public float m_LastCommuteTime;

	public byte m_Level;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity school = m_School;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(school);
		float lastCommuteTime = m_LastCommuteTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastCommuteTime);
		byte level = m_Level;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(level);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref Entity school = ref m_School;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref school);
		ref float lastCommuteTime = ref m_LastCommuteTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastCommuteTime);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.educationTrading)
		{
			ref byte level = ref m_Level;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref level);
		}
		else
		{
			m_Level = byte.MaxValue;
		}
	}
}
