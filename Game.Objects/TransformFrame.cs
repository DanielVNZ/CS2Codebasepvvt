using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Objects;

[InternalBufferCapacity(4)]
public struct TransformFrame : IBufferElementData, ISerializable
{
	public float3 m_Position;

	public float3 m_Velocity;

	public quaternion m_Rotation;

	public TransformFlags m_Flags;

	public ushort m_StateTimer;

	public TransformState m_State;

	public byte m_Activity;

	public TransformFrame(Transform transform)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		m_Position = transform.m_Position;
		m_Velocity = default(float3);
		m_Rotation = transform.m_Rotation;
		m_Flags = (TransformFlags)0u;
		m_StateTimer = 0;
		m_State = TransformState.Default;
		m_Activity = 0;
	}

	public TransformFrame(Transform transform, Moving moving)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		m_Position = transform.m_Position;
		m_Velocity = moving.m_Velocity;
		m_Rotation = transform.m_Rotation;
		m_Flags = (TransformFlags)0u;
		m_StateTimer = 0;
		m_State = TransformState.Default;
		m_Activity = 0;
	}

	public TransformFrame(float3 position, quaternion rotation, float3 velocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_Position = position;
		m_Velocity = velocity;
		m_Rotation = rotation;
		m_Flags = (TransformFlags)0u;
		m_StateTimer = 0;
		m_State = TransformState.Default;
		m_Activity = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		ushort stateTimer = m_StateTimer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(stateTimer);
		TransformState state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)state);
		byte activity = m_Activity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(activity);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref ushort stateTimer = ref m_StateTimer;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref stateTimer);
		byte state = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref byte activity = ref m_Activity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref activity);
		m_State = (TransformState)state;
	}
}
