using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct Prison : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public PrisonFlags m_Flags;

	public sbyte m_PrisonerWellbeing;

	public sbyte m_PrisonerHealth;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		PrisonFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		sbyte prisonerWellbeing = m_PrisonerWellbeing;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prisonerWellbeing);
		sbyte prisonerHealth = m_PrisonerHealth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prisonerHealth);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (PrisonFlags)flags;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.happinessAdjustRefactoring)
		{
			ref sbyte prisonerWellbeing = ref m_PrisonerWellbeing;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref prisonerWellbeing);
			ref sbyte prisonerHealth = ref m_PrisonerHealth;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref prisonerHealth);
		}
	}
}
