using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct WatercraftData : IComponentData, IQueryTypeParameter, ISerializable
{
	public SizeClass m_SizeClass;

	public EnergyTypes m_EnergyType;

	public float m_MaxSpeed;

	public float m_Acceleration;

	public float m_Braking;

	public float2 m_Turning;

	public float m_AngularAcceleration;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		SizeClass sizeClass = m_SizeClass;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)sizeClass);
		EnergyTypes energyType = m_EnergyType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)energyType);
		float maxSpeed = m_MaxSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxSpeed);
		float acceleration = m_Acceleration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(acceleration);
		float braking = m_Braking;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(braking);
		float2 turning = m_Turning;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(turning);
		float angularAcceleration = m_AngularAcceleration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(angularAcceleration);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte sizeClass = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sizeClass);
		byte energyType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref energyType);
		ref float maxSpeed = ref m_MaxSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxSpeed);
		ref float acceleration = ref m_Acceleration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref acceleration);
		ref float braking = ref m_Braking;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref braking);
		ref float2 turning = ref m_Turning;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref turning);
		ref float angularAcceleration = ref m_AngularAcceleration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref angularAcceleration);
		m_SizeClass = (SizeClass)sizeClass;
		m_EnergyType = (EnergyTypes)energyType;
	}
}
