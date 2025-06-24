using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct TrainBogieCache : ISerializable
{
	public Entity m_Lane;

	public float2 m_CurvePosition;

	public TrainLaneFlags m_LaneFlags;

	public TrainBogieCache(TrainBogieLane lane)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane.m_Lane;
		m_CurvePosition = ((float4)(ref lane.m_CurvePosition)).xw;
		m_LaneFlags = lane.m_LaneFlags;
	}

	public TrainBogieCache(PathElement pathElement)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = pathElement.m_Target;
		m_CurvePosition = ((float2)(ref pathElement.m_TargetDelta)).xx;
		m_LaneFlags = (TrainLaneFlags)0u;
	}

	public TrainBogieCache(Entity lane, float curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane;
		m_CurvePosition = float2.op_Implicit(curvePosition);
		m_LaneFlags = (TrainLaneFlags)0u;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = m_Lane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lane);
		float2 curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		TrainLaneFlags laneFlags = m_LaneFlags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)laneFlags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity lane = ref m_Lane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lane);
		ref float2 curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
		uint laneFlags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref laneFlags);
		m_LaneFlags = (TrainLaneFlags)laneFlags;
	}
}
