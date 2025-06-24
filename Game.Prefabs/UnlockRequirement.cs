using System;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct UnlockRequirement : IBufferElementData, IEquatable<UnlockRequirement>
{
	public Entity m_Prefab;

	public UnlockFlags m_Flags;

	public UnlockRequirement(Entity prefab, UnlockFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Prefab = prefab;
		m_Flags = flags;
	}

	public bool Equals(UnlockRequirement other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((Entity)(ref m_Prefab)).Equals(other.m_Prefab))
		{
			return m_Flags == other.m_Flags;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is UnlockRequirement other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Prefab)/*cast due to .constrained prefix*/).GetHashCode() * 397) ^ (int)m_Flags;
	}
}
