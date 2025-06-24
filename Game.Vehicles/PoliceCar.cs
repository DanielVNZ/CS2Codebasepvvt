using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;

namespace Game.Vehicles;

public struct PoliceCar : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public PoliceCarFlags m_State;

	public int m_RequestCount;

	public float m_PathElementTime;

	public uint m_ShiftTime;

	public uint m_EstimatedShift;

	public PolicePurpose m_PurposeMask;

	public PoliceCar(PoliceCarFlags flags, int requestCount, PolicePurpose purposeMask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TargetRequest = Entity.Null;
		m_State = flags;
		m_RequestCount = requestCount;
		m_PathElementTime = 0f;
		m_ShiftTime = 0u;
		m_EstimatedShift = 0u;
		m_PurposeMask = purposeMask;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		PoliceCarFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		int requestCount = m_RequestCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(requestCount);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
		uint shiftTime = m_ShiftTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(shiftTime);
		uint estimatedShift = m_EstimatedShift;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(estimatedShift);
		PolicePurpose purposeMask = m_PurposeMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)purposeMask);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.reverseServiceRequests2)
		{
			ref Entity targetRequest = ref m_TargetRequest;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRequest);
		}
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref int requestCount = ref m_RequestCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref requestCount);
		ref float pathElementTime = ref m_PathElementTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathElementTime);
		ref uint shiftTime = ref m_ShiftTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref shiftTime);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeShiftEstimate)
		{
			ref uint estimatedShift = ref m_EstimatedShift;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref estimatedShift);
		}
		m_State = (PoliceCarFlags)state;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeImprovement3)
		{
			int purposeMask = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref purposeMask);
			m_PurposeMask = (PolicePurpose)purposeMask;
		}
		else
		{
			m_PurposeMask = PolicePurpose.Patrol | PolicePurpose.Emergency;
		}
	}
}
