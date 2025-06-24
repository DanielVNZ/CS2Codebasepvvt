using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct AircraftNavigation : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_TargetPosition;

	public float3 m_TargetDirection;

	public float m_MaxSpeed;

	public float m_MinClimbAngle;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 targetPosition = m_TargetPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetPosition);
		float3 targetDirection = m_TargetDirection;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetDirection);
		float maxSpeed = m_MaxSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxSpeed);
		float minClimbAngle = m_MinClimbAngle;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(minClimbAngle);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref float3 targetPosition = ref m_TargetPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetPosition);
		ref float3 targetDirection = ref m_TargetDirection;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetDirection);
		ref float maxSpeed = ref m_MaxSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxSpeed);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.aircraftNavigation)
		{
			ref float minClimbAngle = ref m_MinClimbAngle;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref minClimbAngle);
		}
	}
}
