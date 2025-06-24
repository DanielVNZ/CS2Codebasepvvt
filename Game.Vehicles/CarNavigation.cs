using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct CarNavigation : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_TargetPosition;

	public quaternion m_TargetRotation;

	public float m_MaxSpeed;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 targetPosition = m_TargetPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetPosition);
		quaternion targetRotation = m_TargetRotation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetRotation);
		float maxSpeed = m_MaxSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxSpeed);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		ref float3 targetPosition = ref m_TargetPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetPosition);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.parkingRotation)
		{
			ref quaternion targetRotation = ref m_TargetRotation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetRotation);
		}
		else
		{
			float3 val = default(float3);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			if (!((float3)(ref val)).Equals(default(float3)))
			{
				m_TargetRotation = quaternion.LookRotationSafe(val, math.up());
			}
		}
		ref float maxSpeed = ref m_MaxSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxSpeed);
	}
}
