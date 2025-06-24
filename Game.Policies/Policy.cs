using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Policies;

[InternalBufferCapacity(0)]
public struct Policy : IBufferElementData, ISerializable
{
	public Entity m_Policy;

	public PolicyFlags m_Flags;

	public float m_Adjustment;

	public Policy(Entity policy, PolicyFlags flags, float adjustment)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Policy = policy;
		m_Flags = flags;
		m_Adjustment = adjustment;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity policy = m_Policy;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(policy);
		PolicyFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		float adjustment = m_Adjustment;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(adjustment);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity policy = ref m_Policy;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref policy);
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float adjustment = ref m_Adjustment;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref adjustment);
		m_Flags = (PolicyFlags)flags;
	}
}
