using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct Occupant : IBufferElementData, IEquatable<Occupant>, ISerializable
{
	public Entity m_Occupant;

	public Occupant(Entity occupant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Occupant = occupant;
	}

	public bool Equals(Occupant other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Occupant)).Equals(other.m_Occupant);
	}

	public override int GetHashCode()
	{
		return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Occupant)/*cast due to .constrained prefix*/).GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Occupant);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Occupant);
	}

	public static implicit operator Entity(Occupant occupant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return occupant.m_Occupant;
	}
}
