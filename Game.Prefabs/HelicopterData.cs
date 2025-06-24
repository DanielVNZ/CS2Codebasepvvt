using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public struct HelicopterData : IComponentData, IQueryTypeParameter, ISerializable
{
	public HelicopterType m_HelicopterType;

	public float m_FlyingMaxSpeed;

	public float m_FlyingAcceleration;

	public float m_FlyingAngularAcceleration;

	public float m_AccelerationSwayFactor;

	public float m_VelocitySwayFactor;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		HelicopterType helicopterType = m_HelicopterType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)helicopterType);
		float flyingMaxSpeed = m_FlyingMaxSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flyingMaxSpeed);
		float flyingAcceleration = m_FlyingAcceleration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flyingAcceleration);
		float flyingAngularAcceleration = m_FlyingAngularAcceleration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flyingAngularAcceleration);
		float accelerationSwayFactor = m_AccelerationSwayFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accelerationSwayFactor);
		float velocitySwayFactor = m_VelocitySwayFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(velocitySwayFactor);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte helicopterType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref helicopterType);
		ref float flyingMaxSpeed = ref m_FlyingMaxSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flyingMaxSpeed);
		ref float flyingAcceleration = ref m_FlyingAcceleration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flyingAcceleration);
		ref float flyingAngularAcceleration = ref m_FlyingAngularAcceleration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flyingAngularAcceleration);
		ref float accelerationSwayFactor = ref m_AccelerationSwayFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref accelerationSwayFactor);
		ref float velocitySwayFactor = ref m_VelocitySwayFactor;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref velocitySwayFactor);
		m_HelicopterType = (HelicopterType)helicopterType;
	}
}
