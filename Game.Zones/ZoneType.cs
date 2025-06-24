using System;
using Colossal.Serialization.Entities;

namespace Game.Zones;

public struct ZoneType : IEquatable<ZoneType>, IStrideSerializable, ISerializable
{
	public ushort m_Index;

	public static ZoneType None => default(ZoneType);

	public bool Equals(ZoneType other)
	{
		return m_Index.Equals(other.m_Index);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Index);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.cornerBuildings)
		{
			ref ushort index = ref m_Index;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
		}
		else
		{
			byte index2 = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref index2);
			m_Index = index2;
		}
	}

	public int GetStride(Context context)
	{
		return 2;
	}
}
