using Game.Objects;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Tools;

public struct Animation : IComponentData, IQueryTypeParameter
{
	public float3 m_TargetPosition;

	public float3 m_Position;

	public quaternion m_Rotation;

	public float3 m_SwayPivot;

	public float3 m_SwayPosition;

	public float3 m_SwayVelocity;

	public float m_PushFactor;

	public Transform ToTransform()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new Transform(m_Position, m_Rotation);
	}
}
