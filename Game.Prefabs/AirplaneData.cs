using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct AirplaneData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float2 m_FlyingSpeed;

	public float m_FlyingAcceleration;

	public float m_FlyingBraking;

	public float m_FlyingTurning;

	public float m_FlyingAngularAcceleration;

	public float m_ClimbAngle;

	public float m_SlowPitchAngle;

	public float m_TurningRollFactor;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float2 flyingSpeed = m_FlyingSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flyingSpeed);
		float flyingAcceleration = m_FlyingAcceleration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flyingAcceleration);
		float flyingBraking = m_FlyingBraking;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flyingBraking);
		float flyingTurning = m_FlyingTurning;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flyingTurning);
		float flyingAngularAcceleration = m_FlyingAngularAcceleration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flyingAngularAcceleration);
		float climbAngle = m_ClimbAngle;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(climbAngle);
		float slowPitchAngle = m_SlowPitchAngle;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(slowPitchAngle);
		float turningRollFactor = m_TurningRollFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(turningRollFactor);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float2 flyingSpeed = ref m_FlyingSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flyingSpeed);
		ref float flyingAcceleration = ref m_FlyingAcceleration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flyingAcceleration);
		ref float flyingBraking = ref m_FlyingBraking;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flyingBraking);
		ref float flyingTurning = ref m_FlyingTurning;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flyingTurning);
		ref float flyingAngularAcceleration = ref m_FlyingAngularAcceleration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flyingAngularAcceleration);
		ref float climbAngle = ref m_ClimbAngle;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref climbAngle);
		ref float slowPitchAngle = ref m_SlowPitchAngle;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref slowPitchAngle);
		ref float turningRollFactor = ref m_TurningRollFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref turningRollFactor);
	}
}
