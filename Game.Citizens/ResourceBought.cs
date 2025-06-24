using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Citizens;

public struct ResourceBought : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Seller;

	public Entity m_Payer;

	public Resource m_Resource;

	public int m_Amount;

	public float m_Distance;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity seller = m_Seller;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(seller);
		Entity payer = m_Payer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(payer);
		sbyte num = (sbyte)EconomyUtils.GetResourceIndex(m_Resource);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		float distance = m_Distance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(distance);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity seller = ref m_Seller;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref seller);
		ref Entity payer = ref m_Payer;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref payer);
		sbyte index = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
		ref int amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		ref float distance = ref m_Distance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref distance);
		m_Resource = EconomyUtils.GetResource(index);
	}
}
