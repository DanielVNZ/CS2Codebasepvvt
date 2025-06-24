using Unity.Entities;
using Unity.Mathematics;

namespace Game.Tools;

public struct LocalTransformCache : IComponentData, IQueryTypeParameter
{
	public float3 m_Position;

	public quaternion m_Rotation;

	public int m_ParentMesh;

	public int m_GroupIndex;

	public int m_Probability;

	public int m_PrefabSubIndex;
}
