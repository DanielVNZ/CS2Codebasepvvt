using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.City;

public struct Tourism : IComponentData, IQueryTypeParameter, IDefaultSerializable, ISerializable
{
	public int m_CurrentTourists;

	public int m_AverageTourists;

	public int m_Attractiveness;

	public int2 m_Lodging;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int currentTourists = ref m_CurrentTourists;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref currentTourists);
		ref int averageTourists = ref m_AverageTourists;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref averageTourists);
		ref int attractiveness = ref m_Attractiveness;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref attractiveness);
		ref int2 lodging = ref m_Lodging;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lodging);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		int currentTourists = m_CurrentTourists;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(currentTourists);
		int averageTourists = m_AverageTourists;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(averageTourists);
		int attractiveness = m_Attractiveness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(attractiveness);
		int2 lodging = m_Lodging;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lodging);
	}

	public void SetDefaults(Context context)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		m_CurrentTourists = 0;
		m_AverageTourists = 0;
		m_Attractiveness = 0;
		m_Lodging = default(int2);
	}
}
