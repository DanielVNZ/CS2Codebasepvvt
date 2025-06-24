using Colossal.Serialization.Entities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Areas;

[InternalBufferCapacity(4)]
public struct Node : IBufferElementData, IStrideSerializable, ISerializable
{
	public float3 m_Position;

	public float m_Elevation;

	public Node(float3 position, float elevation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Position = position;
		m_Elevation = elevation;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float3 position = m_Position;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(position);
		float elevation = m_Elevation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(elevation);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref float3 position = ref m_Position;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref position);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.laneElevation)
		{
			ref float elevation = ref m_Elevation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref elevation);
		}
	}

	public int GetStride(Context context)
	{
		return UnsafeUtility.SizeOf<float3>() + 4;
	}
}
