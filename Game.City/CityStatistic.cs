using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.City;

public struct CityStatistic : IBufferElementData, ISerializable
{
	public double m_Value;

	public double m_TotalValue;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		double value = m_Value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value);
		double totalValue = m_TotalValue;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(totalValue);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.statisticOverflowFix)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			int num2 = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
			m_Value = num;
			m_TotalValue = num2;
			return;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.statisticPrecisionFix)
		{
			long num3 = default(long);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num3);
			long num4 = default(long);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num4);
			m_Value = num3;
			m_TotalValue = num4;
		}
		else
		{
			ref double value = ref m_Value;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref value);
			ref double totalValue = ref m_TotalValue;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref totalValue);
		}
	}
}
