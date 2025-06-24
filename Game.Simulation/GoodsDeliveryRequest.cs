using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Simulation;

public struct GoodsDeliveryRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public GoodsDeliveryFlags m_Flags;

	public Resource m_Resource;

	public int m_Amount;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		GoodsDeliveryFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)flags);
		int resourceIndex = EconomyUtils.GetResourceIndex(m_Resource);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(resourceIndex);
		int amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		ushort flags = default(ushort);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (GoodsDeliveryFlags)flags;
		int index = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
		m_Resource = EconomyUtils.GetResource(index);
		ref int amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
	}
}
