using Colossal.Serialization.Entities;
using Game.Objects;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Creatures;

public struct AnimalNavigation : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_TargetPosition;

	public float3 m_TargetDirection;

	public float m_MaxSpeed;

	public TransformState m_TransformState;

	public byte m_LastActivity;

	public byte m_TargetActivity;

	public AnimalNavigation(float3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_TargetPosition = position;
		m_TargetDirection = default(float3);
		m_MaxSpeed = 0f;
		m_TransformState = TransformState.Default;
		m_LastActivity = 0;
		m_TargetActivity = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 targetPosition = m_TargetPosition;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(targetPosition);
		float3 targetDirection = m_TargetDirection;
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
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		ref float3 targetPosition = ref m_TargetPosition;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetPosition);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.creatureTargetDirection)
		{
			ref float3 targetDirection = ref m_TargetDirection;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref targetDirection);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.creatureTargetDirection2)
		{
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
