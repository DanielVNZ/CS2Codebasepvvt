using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Zones;

[InternalBufferCapacity(4)]
public struct SubBlock : IBufferElementData, IEquatable<SubBlock>, IEmptySerializable
{
	public Entity m_SubBlock;

	public SubBlock(Entity block)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_SubBlock = block;
	}

	public bool Equals(SubBlock other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_SubBlock)).Equals(other.m_SubBlock);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_SubBlock)/*cast due to .constrained prefix*/).GetHashCode();
	}
}
