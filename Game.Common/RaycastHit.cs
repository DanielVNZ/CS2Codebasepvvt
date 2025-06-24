using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Common;

public struct RaycastHit : IEquatable<RaycastHit>
{
	public Entity m_HitEntity;

	public float3 m_Position;

	public float3 m_HitPosition;

	public float3 m_HitDirection;

	public int2 m_CellIndex;

	public float m_NormalizedDistance;

	public float m_CurvePosition;

	public bool Equals(RaycastHit other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (((Entity)(ref m_HitEntity)).Equals(other.m_HitEntity) && ((float3)(ref m_Position)).Equals(other.m_Position) && ((float3)(ref m_HitPosition)).Equals(other.m_HitPosition) && ((float3)(ref m_HitDirection)).Equals(other.m_HitDirection) && ((int2)(ref m_CellIndex)).Equals(other.m_CellIndex) && m_NormalizedDistance == other.m_NormalizedDistance)
		{
			return m_CurvePosition == other.m_CurvePosition;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((((((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_HitEntity)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_HitPosition)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_HitDirection)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<int2, int2>(ref m_CellIndex)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_NormalizedDistance.GetHashCode()) * 31 + m_CurvePosition.GetHashCode();
	}
}
