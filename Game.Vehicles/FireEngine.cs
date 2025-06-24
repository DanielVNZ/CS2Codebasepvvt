using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct FireEngine : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public FireEngineFlags m_State;

	public int m_RequestCount;

	public float m_PathElementTime;

	public float m_ExtinguishingAmount;

	public float m_Efficiency;

	public FireEngine(FireEngineFlags state, int requestCount, float extinguishingAmount, float efficiency)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TargetRequest = Entity.Null;
		m_State = state;
		m_RequestCount = requestCount;
		m_PathElementTime = 0f;
		m_ExtinguishingAmount = extinguishingAmount;
		m_Efficiency = efficiency;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		FireEngineFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		int requestCount = m_RequestCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(requestCount);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
		float extinguishingAmount = m_ExtinguishingAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(extinguishingAmount);
		float efficiency = m_Efficiency;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(efficiency);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
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
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.aircraftNavigation)
		{
			ref float extinguishingAmount = ref m_ExtinguishingAmount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref extinguishingAmount);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.disasterResponse)
		{
			ref float efficiency = ref m_Efficiency;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref efficiency);
		}
		else
		{
			m_Efficiency = 1f;
		}
		m_State = (FireEngineFlags)state;
	}
}
