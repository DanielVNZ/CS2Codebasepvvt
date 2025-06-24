using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Pathfind;
using Game.Routes;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Creatures;

public struct HumanCurrentLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Lane;

	public Entity m_QueueEntity;

	public Sphere3 m_QueueArea;

	public float2 m_CurvePosition;

	public CreatureLaneFlags m_Flags;

	public float m_LanePosition;

	public HumanCurrentLane(AccessLane accessLane, CreatureLaneFlags flags)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = accessLane.m_Lane;
		m_QueueEntity = Entity.Null;
		m_QueueArea = default(Sphere3);
		m_CurvePosition = float2.op_Implicit(accessLane.m_CurvePos);
		m_Flags = flags;
		m_LanePosition = 0f;
	}

	public HumanCurrentLane(PathElement pathElement, CreatureLaneFlags flags)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = pathElement.m_Target;
		m_QueueEntity = Entity.Null;
		m_QueueArea = default(Sphere3);
		m_CurvePosition = ((float2)(ref pathElement.m_TargetDelta)).xx;
		m_Flags = flags;
		m_LanePosition = 0f;
	}

	public HumanCurrentLane(CreatureLaneFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = Entity.Null;
		m_QueueEntity = Entity.Null;
		m_QueueArea = default(Sphere3);
		m_CurvePosition = float2.op_Implicit(0f);
		m_Flags = flags;
		m_LanePosition = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = m_Lane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lane);
		float2 curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		CreatureLaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		float lanePosition = m_LanePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lanePosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ref Entity lane = ref m_Lane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lane);
		ref float2 curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.lanePosition)
		{
			ref float lanePosition = ref m_LanePosition;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lanePosition);
		}
		m_Flags = (CreatureLaneFlags)flags;
	}
}
