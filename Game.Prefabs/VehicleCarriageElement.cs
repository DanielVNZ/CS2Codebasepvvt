using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct VehicleCarriageElement : IBufferElementData
{
	public Entity m_Prefab;

	public int2 m_Count;

	public VehicleCarriageDirection m_Direction;

	public VehicleCarriageElement(Entity carriage, int minCount, int maxCount, VehicleCarriageDirection direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		m_Prefab = carriage;
		m_Count = new int2(minCount, maxCount);
		m_Direction = direction;
	}
}
