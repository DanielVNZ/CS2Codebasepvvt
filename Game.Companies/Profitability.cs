using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct Profitability : IComponentData, IQueryTypeParameter, ISerializable
{
	public byte m_Profitability;

	public int m_LastTotalWorth;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte profitability = m_Profitability;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(profitability);
		int lastTotalWorth = m_LastTotalWorth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastTotalWorth);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref byte profitability = ref m_Profitability;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref profitability);
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.CompanyAndCargoFix))
		{
			ref int lastTotalWorth = ref m_LastTotalWorth;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastTotalWorth);
		}
	}
}
