using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Areas;

[InternalBufferCapacity(2)]
public struct Triangle : IBufferElementData, IEmptySerializable
{
	public int3 m_Indices;

	public Bounds1 m_HeightRange;

	public int m_MinLod;

	public Triangle(int a, int b, int c)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		m_Indices = new int3(a, b, c);
		m_HeightRange = default(Bounds1);
		m_MinLod = 0;
	}
}
