using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct CarTrailerLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Lane;

	public Entity m_NextLane;

	public float2 m_CurvePosition;

	public float2 m_NextPosition;

	public float m_Duration;

	public float m_Distance;

	public CarTrailerLane(ParkedCar parkedCar)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = parkedCar.m_Lane;
		m_NextLane = Entity.Null;
		m_CurvePosition = float2.op_Implicit(parkedCar.m_CurvePosition);
		m_NextPosition = float2.op_Implicit(0f);
		m_Duration = 0f;
		m_Distance = 0f;
	}

	public CarTrailerLane(CarCurrentLane currentLane)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_Lane = currentLane.m_Lane;
		m_NextLane = Entity.Null;
		m_CurvePosition = ((float3)(ref currentLane.m_CurvePosition)).xy;
		m_NextPosition = float2.op_Implicit(0f);
		m_Duration = 0f;
		m_Distance = 0f;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Entity lane = m_Lane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lane);
		Entity nextLane = m_NextLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nextLane);
		float2 curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
		float2 nextPosition = m_NextPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nextPosition);
		float duration = m_Duration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(duration);
		float distance = m_Distance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(distance);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity lane = ref m_Lane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref lane);
		ref Entity nextLane = ref m_NextLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref nextLane);
		ref float2 curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
		ref float2 nextPosition = ref m_NextPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref nextPosition);
		ref float duration = ref m_Duration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref duration);
		ref float distance = ref m_Distance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref distance);
	}
}
