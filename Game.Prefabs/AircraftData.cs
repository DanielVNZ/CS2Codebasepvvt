using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct AircraftData : IComponentData, IQueryTypeParameter, ISerializable
{
	public SizeClass m_SizeClass;

	public float m_GroundMaxSpeed;

	public float m_GroundAcceleration;

	public float m_GroundBraking;

	public float2 m_GroundTurning;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		SizeClass sizeClass = m_SizeClass;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)sizeClass);
		float groundMaxSpeed = m_GroundMaxSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groundMaxSpeed);
		float groundAcceleration = m_GroundAcceleration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groundAcceleration);
		float groundBraking = m_GroundBraking;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groundBraking);
		float2 groundTurning = m_GroundTurning;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(groundTurning);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte sizeClass = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref sizeClass);
		ref float groundMaxSpeed = ref m_GroundMaxSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref groundMaxSpeed);
		ref float groundAcceleration = ref m_GroundAcceleration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref groundAcceleration);
		ref float groundBraking = ref m_GroundBraking;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref groundBraking);
		ref float2 groundTurning = ref m_GroundTurning;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref groundTurning);
		m_SizeClass = (SizeClass)sizeClass;
	}
}
