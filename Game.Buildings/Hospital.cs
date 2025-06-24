using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct Hospital : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public HospitalFlags m_Flags;

	public byte m_TreatmentBonus;

	public byte m_MinHealth;

	public byte m_MaxHealth;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		HospitalFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		byte treatmentBonus = m_TreatmentBonus;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(treatmentBonus);
		byte minHealth = m_MinHealth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(minHealth);
		byte maxHealth = m_MaxHealth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxHealth);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.healthcareImprovement)
		{
			ref byte treatmentBonus = ref m_TreatmentBonus;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref treatmentBonus);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.healthcareImprovement2)
		{
			ref byte minHealth = ref m_MinHealth;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref minHealth);
			ref byte maxHealth = ref m_MaxHealth;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxHealth);
		}
		m_Flags = (HospitalFlags)flags;
	}
}
