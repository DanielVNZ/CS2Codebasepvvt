using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct PrisonData : IComponentData, IQueryTypeParameter, ICombineData<PrisonData>, ISerializable
{
	public int m_PrisonVanCapacity;

	public int m_PrisonerCapacity;

	public sbyte m_PrisonerWellbeing;

	public sbyte m_PrisonerHealth;

	public void Combine(PrisonData otherData)
	{
		m_PrisonVanCapacity += otherData.m_PrisonVanCapacity;
		m_PrisonerCapacity += otherData.m_PrisonerCapacity;
		m_PrisonerWellbeing += otherData.m_PrisonerWellbeing;
		m_PrisonerHealth += otherData.m_PrisonerHealth;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int prisonVanCapacity = m_PrisonVanCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prisonVanCapacity);
		int prisonerCapacity = m_PrisonerCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prisonerCapacity);
		sbyte prisonerWellbeing = m_PrisonerWellbeing;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prisonerWellbeing);
		sbyte prisonerHealth = m_PrisonerHealth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prisonerHealth);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref int prisonVanCapacity = ref m_PrisonVanCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref prisonVanCapacity);
		ref int prisonerCapacity = ref m_PrisonerCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref prisonerCapacity);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.happinessAdjustRefactoring)
		{
			ref sbyte prisonerWellbeing = ref m_PrisonerWellbeing;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref prisonerWellbeing);
			ref sbyte prisonerHealth = ref m_PrisonerHealth;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref prisonerHealth);
		}
	}
}
