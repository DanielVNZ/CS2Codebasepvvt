using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Areas;

[InternalBufferCapacity(2)]
public struct LabelExtents : IBufferElementData, IEmptySerializable
{
	public Bounds2 m_Bounds;

	public LabelExtents(float2 min, float2 max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_Bounds = new Bounds2(min, max);
	}
}
