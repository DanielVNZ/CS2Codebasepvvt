using Colossal.Serialization.Entities;
using Game.Objects;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Creatures;

public struct HumanNavigation : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_TargetPosition;

	public float2 m_TargetDirection;

	public float m_MaxSpeed;

	public TransformState m_TransformState;

	public byte m_LastActivity;

	public byte m_TargetActivity;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 targetPosition = m_TargetPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetPosition);
		float2 targetDirection = m_TargetDirection;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetDirection);
		byte targetActivity = m_TargetActivity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetActivity);
		TransformState transformState = m_TransformState;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)transformState);
		byte lastActivity = m_LastActivity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastActivity);
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
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ref float3 targetPosition = ref m_TargetPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetPosition);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.creatureTargetDirection)
		{
			ref float2 targetDirection = ref m_TargetDirection;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetDirection);
			ref byte targetActivity = ref m_TargetActivity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetActivity);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.animationStateFix)
		{
			byte transformState = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref transformState);
			ref byte lastActivity = ref m_LastActivity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastActivity);
			m_TransformState = (TransformState)transformState;
		}
		ref float maxSpeed = ref m_MaxSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxSpeed);
	}
}
