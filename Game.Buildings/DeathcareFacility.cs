using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct DeathcareFacility : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public DeathcareFacilityFlags m_Flags;

	public float m_ProcessingState;

	public int m_LongTermStoredCount;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		DeathcareFacilityFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		float processingState = m_ProcessingState;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(processingState);
		int longTermStoredCount = m_LongTermStoredCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(longTermStoredCount);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		byte flags = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float processingState = ref m_ProcessingState;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref processingState);
		ref int longTermStoredCount = ref m_LongTermStoredCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref longTermStoredCount);
		m_Flags = (DeathcareFacilityFlags)flags;
	}
}
