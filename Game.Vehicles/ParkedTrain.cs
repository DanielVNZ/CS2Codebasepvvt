using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct ParkedTrain : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_ParkingLocation;

	public Entity m_FrontLane;

	public Entity m_RearLane;

	public float2 m_CurvePosition;

	public ParkedTrain(Entity location)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		m_ParkingLocation = location;
		m_FrontLane = Entity.Null;
		m_RearLane = Entity.Null;
		m_CurvePosition = float2.op_Implicit(0f);
	}

	public ParkedTrain(Entity location, TrainCurrentLane currentLane)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		m_ParkingLocation = location;
		m_FrontLane = currentLane.m_Front.m_Lane;
		m_RearLane = currentLane.m_Rear.m_Lane;
		m_CurvePosition = new float2(currentLane.m_Front.m_CurvePosition.y, currentLane.m_Rear.m_CurvePosition.y);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Entity parkingLocation = m_ParkingLocation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(parkingLocation);
		Entity frontLane = m_FrontLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(frontLane);
		Entity rearLane = m_RearLane;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(rearLane);
		float2 curvePosition = m_CurvePosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curvePosition);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity parkingLocation = ref m_ParkingLocation;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref parkingLocation);
		ref Entity frontLane = ref m_FrontLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref frontLane);
		ref Entity rearLane = ref m_RearLane;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref rearLane);
		ref float2 curvePosition = ref m_CurvePosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curvePosition);
	}
}
