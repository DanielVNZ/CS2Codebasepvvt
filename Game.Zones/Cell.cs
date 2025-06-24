using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Zones;

[InternalBufferCapacity(60)]
public struct Cell : IBufferElementData, IStrideSerializable, ISerializable
{
	public CellFlags m_State;

	public ZoneType m_Zone;

	public short m_Height;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		CellFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)state);
		ZoneType zone = m_Zone;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<ZoneType>(zone);
		short height = m_Height;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(height);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.cornerBuildings)
		{
			ushort state = default(ushort);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
			m_State = (CellFlags)state;
		}
		else
		{
			byte state2 = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref state2);
			m_State = (CellFlags)state2;
		}
		ref ZoneType zone = ref m_Zone;
		((IReader)reader/*cast due to .constrained prefix*/).Read<ZoneType>(ref zone);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.zoneHeightLimit)
		{
			ref short height = ref m_Height;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref height);
		}
		else
		{
			m_Height = short.MaxValue;
		}
	}

	public int GetStride(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (((Context)(ref context)).version >= Version.zoneHeightLimit)
		{
			return 4 + m_Zone.GetStride(context);
		}
		return 2 + m_Zone.GetStride(context);
	}
}
