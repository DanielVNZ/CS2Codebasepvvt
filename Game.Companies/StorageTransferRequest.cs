using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Entities;

namespace Game.Companies;

public struct StorageTransferRequest : IBufferElementData, ISerializable
{
	public StorageTransferFlags m_Flags;

	public Resource m_Resource;

	public int m_Amount;

	public Entity m_Target;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		StorageTransferFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		sbyte num = (sbyte)EconomyUtils.GetResourceIndex(m_Resource);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		sbyte index = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
		ref int amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		m_Flags = (StorageTransferFlags)flags;
		m_Resource = EconomyUtils.GetResource(index);
	}
}
