using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct LookingForPartner : IBufferElementData, ISerializable
{
	public Entity m_Citizen;

	public PartnerType m_PartnerType;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity citizen = ref m_Citizen;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref citizen);
		int partnerType = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref partnerType);
		m_PartnerType = (PartnerType)partnerType;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity citizen = m_Citizen;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(citizen);
		PartnerType partnerType = m_PartnerType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)partnerType);
	}
}
