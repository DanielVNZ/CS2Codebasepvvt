using Colossal.Serialization.Entities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct Node : IComponentData, IQueryTypeParameter, IStrideSerializable, ISerializable
{
	public float3 m_Position;

	public quaternion m_Rotation;

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
		ref float3 position = ref m_Position;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		ref quaternion rotation = ref m_Rotation;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref rotation);
	}

	public int GetStride(Context context)
	{
		return UnsafeUtility.SizeOf<float3>() + UnsafeUtility.SizeOf<quaternion>();
	}
}
