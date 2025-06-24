using Colossal.Serialization.Entities;

namespace Game.City;

[FormerlySerializedAs("Game.City.StatisticsEvent2, Game")]
public struct StatisticsEvent : ISerializable
{
	public StatisticType m_Statistic;

	public int m_Parameter;

	public float m_Change;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		StatisticType statistic = m_Statistic;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)statistic);
		int parameter = m_Parameter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(parameter);
		float change = m_Change;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(change);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		int statistic = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref statistic);
		m_Statistic = (StatisticType)statistic;
		ref int parameter = ref m_Parameter;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref parameter);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.statisticPrecisionFix)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_Change = num;
		}
		else
		{
			ref float change = ref m_Change;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref change);
		}
	}
}
