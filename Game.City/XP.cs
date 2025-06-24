using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.City;

public struct XP : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_XP;

	public int m_MaximumPopulation;

	public int m_MaximumIncome;

	public XPRewardFlags m_XPRewardRecord;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int xP = m_XP;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(xP);
		int maximumPopulation = m_MaximumPopulation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maximumPopulation);
		int maximumIncome = m_MaximumIncome;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maximumIncome);
		XPRewardFlags xPRewardRecord = m_XPRewardRecord;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)xPRewardRecord);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ref int xP = ref m_XP;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref xP);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.xpMaximumStats)
		{
			ref int maximumPopulation = ref m_MaximumPopulation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maximumPopulation);
			ref int maximumIncome = ref m_MaximumIncome;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maximumIncome);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.xpRewardRecord)
		{
			byte xPRewardRecord = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref xPRewardRecord);
			m_XPRewardRecord = (XPRewardFlags)xPRewardRecord;
		}
	}
}
