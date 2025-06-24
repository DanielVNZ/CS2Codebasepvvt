using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct CoordinatedMeeting : IComponentData, IQueryTypeParameter, ISerializable
{
	public MeetingStatus m_Status;

	public int m_Phase;

	public Entity m_Target;

	public uint m_PhaseEndTime;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		int status = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref status);
		m_Status = (MeetingStatus)status;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.timoSerializationFlow)
		{
			TravelPurpose travelPurpose = default(TravelPurpose);
			((IReader)reader/*cast due to .constrained prefix*/).Read<TravelPurpose>(ref travelPurpose);
		}
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.timoSerializationFlow)
		{
			m_Phase = 0;
			m_PhaseEndTime = 0u;
			return;
		}
		ref int phase = ref m_Phase;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref phase);
		ref uint phaseEndTime = ref m_PhaseEndTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref phaseEndTime);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		MeetingStatus status = m_Status;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)status);
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		int phase = m_Phase;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(phase);
		uint phaseEndTime = m_PhaseEndTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(phaseEndTime);
	}
}
