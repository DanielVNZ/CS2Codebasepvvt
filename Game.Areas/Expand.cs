using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Areas;

[InternalBufferCapacity(4)]
public struct Expand : IBufferElementData, IEmptySerializable
{
	public float2 m_Offset;

	public Expand(float2 offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Offset = offset;
	}
}
