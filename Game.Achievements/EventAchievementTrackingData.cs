using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Achievements;

public struct EventAchievementTrackingData : IComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_StartFrame;

	public AchievementId m_ID;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		ref uint startFrame = ref m_StartFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startFrame);
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_ID = new AchievementId(num);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		uint startFrame = m_StartFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startFrame);
		int id = m_ID.id;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(id);
	}
}
