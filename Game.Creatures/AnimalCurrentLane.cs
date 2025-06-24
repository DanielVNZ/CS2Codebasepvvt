using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Creatures;

public struct AnimalCurrentLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Lane;

	public Entity m_NextLane;

	public Entity m_QueueEntity;

	public Sphere3 m_QueueArea;

	public float2 m_CurvePosition;

	public float2 m_NextPosition;

	public CreatureLaneFlags m_Flags;

	public CreatureLaneFlags m_NextFlags;

	public float m_LanePosition;

	public AnimalCurrentLane(Entity lane, float curvePosition, CreatureLaneFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = lane;
		m_NextLane = Entity.Null;
		m_QueueEntity = Entity.Null;
		m_QueueArea = default(Sphere3);
		m_CurvePosition = float2.op_Implicit(curvePosition);
		m_NextPosition = float2.op_Implicit(0f);
		m_Flags = flags;
		m_NextFlags = (CreatureLaneFlags)0u;
		m_LanePosition = 0f;
	}

	public AnimalCurrentLane(CreatureLaneFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = Entity.Null;
		m_NextLane = Entity.Null;
		m_QueueEntity = Entity.Null;
		m_QueueArea = default(Sphere3);
		m_CurvePosition = float2.op_Implicit(0f);
		m_NextPosition = float2.op_Implicit(0f);
		m_Flags = flags;
		m_NextFlags = (CreatureLaneFlags)0u;
		m_LanePosition = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = m_Lane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lane);
		float2 curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		CreatureLaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		float lanePosition = m_LanePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lanePosition);
		Entity nextLane = m_NextLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nextLane);
		float2 nextPosition = m_NextPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nextPosition);
		CreatureLaneFlags nextFlags = m_NextFlags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)nextFlags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		ref Entity lane = ref m_Lane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lane);
		ref float2 curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float lanePosition = ref m_LanePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lanePosition);
		m_Flags = (CreatureLaneFlags)flags;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.animalNavigation)
		{
			ref Entity nextLane = ref m_NextLane;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref nextLane);
			ref float2 nextPosition = ref m_NextPosition;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref nextPosition);
			uint nextFlags = default(uint);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref nextFlags);
			m_NextFlags = (CreatureLaneFlags)nextFlags;
		}
	}
}
