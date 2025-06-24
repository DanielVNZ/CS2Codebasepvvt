using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Objects;

public struct Transform : IComponentData, IQueryTypeParameter, IEquatable<Transform>, ISerializable
{
	public float3 m_Position;

	public quaternion m_Rotation;

	public Transform(float3 position, quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Position = position;
		m_Rotation = rotation;
	}

	public bool Equals(Transform other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (((float3)(ref m_Position)).Equals(other.m_Position))
		{
			return ((quaternion)(ref m_Rotation)).Equals(other.m_Rotation);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<quaternion, quaternion>(ref m_Rotation)/*cast due to .constrained prefix*/).GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 position = m_Position;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		quaternion rotation = m_Rotation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(rotation);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		ref float3 position = ref m_Position;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		ref quaternion rotation = ref m_Rotation;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref rotation);
		if (!math.all(m_Position >= -100000f) || !math.all(m_Position <= 100000f) || !math.all(math.isfinite(m_Rotation.value)) || math.all(m_Rotation.value == 0f))
		{
			m_Position = default(float3);
			m_Rotation = quaternion.identity;
		}
	}
}
