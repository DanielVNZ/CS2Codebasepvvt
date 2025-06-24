using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct Household : IComponentData, IQueryTypeParameter, ISerializable
{
	public HouseholdFlags m_Flags;

	public int m_Resources;

	public short m_ConsumptionPerDay;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		HouseholdFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		int resources = m_Resources;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(resources);
		short consumptionPerDay = m_ConsumptionPerDay;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(consumptionPerDay);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.householdRandomSeedRemoved)
		{
			uint num = default(uint);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		ref int resources = ref m_Resources;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref resources);
		ref short consumptionPerDay = ref m_ConsumptionPerDay;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref consumptionPerDay);
		m_Flags = (HouseholdFlags)flags;
		if (m_Resources < 0)
		{
			m_Resources = 0;
		}
	}
}
