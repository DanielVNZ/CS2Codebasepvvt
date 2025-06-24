using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct AircraftCurrentLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Lane;

	public float3 m_CurvePosition;

	public AircraftLaneFlags m_LaneFlags;

	public float m_Duration;

	public float m_Distance;

	public float m_LanePosition;

	public AircraftCurrentLane(ParkedCar parkedCar, AircraftLaneFlags flags)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = parkedCar.m_Lane;
		m_CurvePosition = float3.op_Implicit(parkedCar.m_CurvePosition);
		m_LaneFlags = flags;
		m_Duration = 0f;
		m_Distance = 0f;
		m_LanePosition = 0f;
	}

	public AircraftCurrentLane(PathElement pathElement, AircraftLaneFlags laneFlags)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = pathElement.m_Target;
		m_CurvePosition = ((float2)(ref pathElement.m_TargetDelta)).xxx;
		m_LaneFlags = laneFlags;
		m_Duration = 0f;
		m_Distance = 0f;
		m_LanePosition = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = m_Lane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lane);
		float3 curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		AircraftLaneFlags laneFlags = m_LaneFlags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)laneFlags);
		float duration = m_Duration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(duration);
		float distance = m_Distance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(distance);
		float lanePosition = m_LanePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lanePosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity lane = ref m_Lane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lane);
		ref float3 curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
		uint laneFlags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref laneFlags);
		ref float duration = ref m_Duration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref duration);
		ref float distance = ref m_Distance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref distance);
		ref float lanePosition = ref m_LanePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lanePosition);
		m_LaneFlags = (AircraftLaneFlags)laneFlags;
	}
}
