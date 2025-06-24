using Colossal.Collections;
using Unity.Entities;

namespace Game.Common;

public struct RaycastResult : IAccumulable<RaycastResult>
{
	public RaycastHit m_Hit;

	public Entity m_Owner;

	public void Accumulate(RaycastResult other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (m_Owner == Entity.Null || (other.m_Owner != Entity.Null && (other.m_Hit.m_NormalizedDistance < m_Hit.m_NormalizedDistance || (other.m_Hit.m_NormalizedDistance == m_Hit.m_NormalizedDistance && other.m_Hit.m_HitEntity.Index < m_Hit.m_HitEntity.Index))))
		{
			this = other;
		}
	}
}
