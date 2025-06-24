using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct WatercraftCurrentLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Lane;

	public Entity m_ChangeLane;

	public float3 m_CurvePosition;

	public WatercraftLaneFlags m_LaneFlags;

	public float m_ChangeProgress;

	public float m_Duration;

	public float m_Distance;

	public float m_LanePosition;

	public WatercraftCurrentLane(PathElement pathElement, WatercraftLaneFlags flags)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = pathElement.m_Target;
		m_ChangeLane = Entity.Null;
		m_CurvePosition = ((float2)(ref pathElement.m_TargetDelta)).xxx;
		m_LaneFlags = flags;
		m_ChangeProgress = 0f;
		m_Duration = 0f;
		m_Distance = 0f;
		m_LanePosition = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = m_Lane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lane);
		Entity changeLane = m_ChangeLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(changeLane);
		float3 curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		WatercraftLaneFlags laneFlags = m_LaneFlags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)laneFlags);
		float changeProgress = m_ChangeProgress;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(changeProgress);
		float duration = m_Duration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(duration);
		float distance = m_Distance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(distance);
		float lanePosition = m_LanePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lanePosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		ref Entity lane = ref m_Lane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lane);
		ref Entity changeLane = ref m_ChangeLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref changeLane);
		ref float3 curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
		uint laneFlags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref laneFlags);
		ref float changeProgress = ref m_ChangeProgress;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref changeProgress);
		ref float duration = ref m_Duration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref duration);
		ref float distance = ref m_Distance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref distance);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.lanePosition)
		{
			ref float lanePosition = ref m_LanePosition;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lanePosition);
		}
		m_LaneFlags = (WatercraftLaneFlags)laneFlags;
	}
}
