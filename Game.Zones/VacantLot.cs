using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Zones;

[InternalBufferCapacity(1)]
public struct VacantLot : IBufferElementData, IEquatable<VacantLot>, ISerializable
{
	public int4 m_Area;

	public ZoneType m_Type;

	public short m_Height;

	public LotFlags m_Flags;

	public VacantLot(int2 min, int2 max, ZoneType type, int height, LotFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		m_Area = new int4(min.x, max.x, min.y, max.y);
		m_Type = type;
		m_Height = (short)height;
		m_Flags = flags;
	}

	public bool Equals(VacantLot other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((int4)(ref m_Area)).Equals(other.m_Area);
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<int4, int4>(ref m_Area)/*cast due to .constrained prefix*/).GetHashCode()) * 31 + m_Type.GetHashCode();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		int4 area = m_Area;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(area);
		ZoneType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<ZoneType>(type);
		short height = m_Height;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(height);
		LotFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		ref int4 area = ref m_Area;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref area);
		ref ZoneType type = ref m_Type;
		((IReader)reader/*cast due to .constrained prefix*/).Read<ZoneType>(ref type);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.zoneHeightLimit)
		{
			ref short height = ref m_Height;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref height);
		}
		else
		{
			m_Height = short.MaxValue;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.cornerBuildings)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (LotFlags)flags;
		}
	}
}
