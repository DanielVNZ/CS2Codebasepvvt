using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct GarbageTruck : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_TargetRequest;

	public GarbageTruckFlags m_State;

	public int m_RequestCount;

	public int m_Garbage;

	public int m_EstimatedGarbage;

	public float m_PathElementTime;

	public GarbageTruck(GarbageTruckFlags flags, int requestCount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_TargetRequest = Entity.Null;
		m_State = flags;
		m_RequestCount = requestCount;
		m_Garbage = 0;
		m_EstimatedGarbage = 0;
		m_PathElementTime = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity targetRequest = m_TargetRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRequest);
		int estimatedGarbage = m_EstimatedGarbage;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(estimatedGarbage);
		GarbageTruckFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)state);
		int requestCount = m_RequestCount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(requestCount);
		int garbage = m_Garbage;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(garbage);
		float pathElementTime = m_PathElementTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pathElementTime);
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
			ref int estimatedGarbage = ref m_EstimatedGarbage;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref estimatedGarbage);
		}
		uint state = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref int requestCount = ref m_RequestCount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref requestCount);
		ref int garbage = ref m_Garbage;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref garbage);
		ref float pathElementTime = ref m_PathElementTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pathElementTime);
		m_State = (GarbageTruckFlags)state;
	}
}
