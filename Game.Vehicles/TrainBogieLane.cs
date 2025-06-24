using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct TrainBogieLane : ISerializable
{
	public Entity m_Lane;

	public float4 m_CurvePosition;

	public TrainLaneFlags m_LaneFlags;

	public TrainBogieLane(TrainBogieCache cache)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = cache.m_Lane;
		m_CurvePosition = ((float2)(ref cache.m_CurvePosition)).xxxy;
		m_LaneFlags = cache.m_LaneFlags;
	}

	public TrainBogieLane(Entity lane, float4 curvePosition, TrainLaneFlags laneFlags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane;
		m_CurvePosition = curvePosition;
		m_LaneFlags = laneFlags;
	}

	public TrainBogieLane(TrainNavigationLane navLane)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = navLane.m_Lane;
		m_CurvePosition = ((float2)(ref navLane.m_CurvePosition)).xxxy;
		m_LaneFlags = navLane.m_Flags;
	}

	public TrainBogieLane(PathElement pathElement)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = pathElement.m_Target;
		m_CurvePosition = ((float2)(ref pathElement.m_TargetDelta)).xxxx;
		m_LaneFlags = (TrainLaneFlags)0u;
	}

	public TrainBogieLane(Entity lane, float curvePosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane;
		m_CurvePosition = float4.op_Implicit(curvePosition);
		m_LaneFlags = (TrainLaneFlags)0u;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = m_Lane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lane);
		float4 curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		TrainLaneFlags laneFlags = m_LaneFlags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)laneFlags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		ref Entity lane = ref m_Lane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lane);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.tramNavigationImprovement)
		{
			ref float4 curvePosition = ref m_CurvePosition;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
		}
		else
		{
			float3 val = default(float3);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			m_CurvePosition = ((float3)(ref val)).xyyz;
		}
		uint laneFlags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref laneFlags);
		m_LaneFlags = (TrainLaneFlags)laneFlags;
	}
}
