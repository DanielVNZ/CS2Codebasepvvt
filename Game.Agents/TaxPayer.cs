using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Agents;

public struct TaxPayer : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_UntaxedIncome;

	public int m_AverageTaxRate;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int untaxedIncome = m_UntaxedIncome;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(untaxedIncome);
		int averageTaxRate = m_AverageTaxRate;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(averageTaxRate);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref int untaxedIncome = ref m_UntaxedIncome;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref untaxedIncome);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.averageTaxRate)
		{
			ref int averageTaxRate = ref m_AverageTaxRate;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref averageTaxRate);
		}
		else
		{
			m_AverageTaxRate = 10;
		}
	}
}
