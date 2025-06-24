using Colossal.Mathematics;
using Unity.Entities;

namespace Game.Prefabs;

public struct AnimalData : IComponentData, IQueryTypeParameter
{
	public float m_MoveSpeed;

	public float m_SwimSpeed;

	public float m_FlySpeed;

	public float m_Acceleration;

	public Bounds1 m_SwimDepth;

	public Bounds1 m_FlyHeight;
}
