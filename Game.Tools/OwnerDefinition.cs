using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Tools;

public struct OwnerDefinition : IComponentData, IQueryTypeParameter, IEquatable<OwnerDefinition>
{
	public Entity m_Prefab;

	public float3 m_Position;

	public quaternion m_Rotation;

	public bool Equals(OwnerDefinition other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((Entity)(ref m_Prefab)).Equals(other.m_Prefab) && ((float3)(ref m_Position)).Equals(other.m_Position))
		{
			return ((quaternion)(ref m_Rotation)).Equals(other.m_Rotation);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_Position)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<quaternion, quaternion>(ref m_Rotation)/*cast due to .constrained prefix*/).GetHashCode();
	}
}
