using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct GarbageTransferRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Facility;

	public GarbageTransferRequestFlags m_Flags;

	public float m_Priority;

	public int m_Amount;

	public GarbageTransferRequest(Entity facility, GarbageTransferRequestFlags flags, float priority, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Facility = facility;
		m_Flags = flags;
		m_Priority = priority;
		m_Amount = amount;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity facility = m_Facility;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(facility);
		GarbageTransferRequestFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)flags);
		float priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		int amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity facility = ref m_Facility;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref facility);
		ushort flags = default(ushort);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		ref int amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		m_Flags = (GarbageTransferRequestFlags)flags;
	}
}
