using System;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace Game.Prefabs;

public struct ZoneBuiltDataKey : IEquatable<ZoneBuiltDataKey>
{
	public Entity m_Zone;

	public int m_Level;

	public bool Equals(ZoneBuiltDataKey other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((Entity)(ref m_Zone)).Equals(other.m_Zone))
		{
			return m_Level.Equals(other.m_Level);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Zone)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_Level.GetHashCode();
	}
}
