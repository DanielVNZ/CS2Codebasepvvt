using System;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace Game.Effects;

public struct SourceInfo : IEquatable<SourceInfo>
{
	public Entity m_Entity;

	public int m_EffectIndex;

	public SourceInfo(Entity entity, int effectIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Entity = entity;
		m_EffectIndex = effectIndex;
	}

	public bool Equals(SourceInfo other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (m_Entity == other.m_Entity)
		{
			return m_EffectIndex == other.m_EffectIndex;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Entity)/*cast due to .constrained prefix*/).GetHashCode() ^ m_EffectIndex;
	}
}
