using System;
using Colossal.Serialization.Entities;
using Game.Tools;
using Unity.Entities;

namespace Game.Net;

[InternalBufferCapacity(2)]
public struct SubReplacement : IBufferElementData, IEquatable<SubReplacement>, ISerializable
{
	public Entity m_Prefab;

	public SubReplacementType m_Type;

	public SubReplacementSide m_Side;

	public AgeMask m_AgeMask;

	public bool Equals(SubReplacement other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (m_Prefab == other.m_Prefab && m_Type == other.m_Type && m_Side == other.m_Side)
		{
			return m_AgeMask == other.m_AgeMask;
		}
		return false;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity prefab = m_Prefab;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(prefab);
		SubReplacementType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)type);
		SubReplacementSide side = m_Side;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((sbyte)side);
		AgeMask ageMask = m_AgeMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)ageMask);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity prefab = ref m_Prefab;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref prefab);
		byte type = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
		sbyte side = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref side);
		byte ageMask = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref ageMask);
		m_Type = (SubReplacementType)type;
		m_Side = (SubReplacementSide)side;
		m_AgeMask = (AgeMask)ageMask;
	}
}
