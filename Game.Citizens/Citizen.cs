using Colossal.Serialization.Entities;
using Game.Common;
using Game.Simulation;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Citizens;

public struct Citizen : IComponentData, IQueryTypeParameter, ISerializable
{
	public ushort m_PseudoRandom;

	public CitizenFlags m_State;

	public byte m_WellBeing;

	public byte m_Health;

	public byte m_LeisureCounter;

	public byte m_PenaltyCounter;

	public int m_UnemploymentCounter;

	public short m_BirthDay;

	public int Happiness => (m_WellBeing + m_Health) / 2;

	public float GetAgeInDays(uint simulationFrame, TimeData timeData)
	{
		return TimeSystem.GetDay(simulationFrame, timeData) - m_BirthDay;
	}

	public Random GetPseudoRandom(CitizenPseudoRandom reason)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Random val = default(Random);
		((Random)(ref val))._002Ector((uint)((ulong)reason ^ (ulong)((m_PseudoRandom << 16) | m_PseudoRandom)));
		((Random)(ref val)).NextUInt();
		uint num = ((Random)(ref val)).NextUInt();
		num = math.select(num, uint.MaxValue, num == 0);
		return new Random(num);
	}

	public int GetEducationLevel()
	{
		if ((m_State & CitizenFlags.EducationBit3) != CitizenFlags.None)
		{
			return 4;
		}
		return (((m_State & CitizenFlags.EducationBit1) != CitizenFlags.None) ? 2 : 0) + (((m_State & CitizenFlags.EducationBit2) != CitizenFlags.None) ? 1 : 0);
	}

	public void SetEducationLevel(int level)
	{
		if (level == 4)
		{
			m_State |= CitizenFlags.EducationBit3;
		}
		else
		{
			m_State &= ~CitizenFlags.EducationBit3;
		}
		if (level >= 2)
		{
			m_State |= CitizenFlags.EducationBit1;
		}
		else
		{
			m_State &= ~CitizenFlags.EducationBit1;
		}
		if (level % 2 != 0)
		{
			m_State |= CitizenFlags.EducationBit2;
		}
		else
		{
			m_State &= ~CitizenFlags.EducationBit2;
		}
	}

	public int GetFailedEducationCount()
	{
		return (((m_State & CitizenFlags.FailedEducationBit1) != CitizenFlags.None) ? 2 : 0) + (((m_State & CitizenFlags.FailedEducationBit2) != CitizenFlags.None) ? 1 : 0);
	}

	public void SetFailedEducationCount(int fails)
	{
		if (fails >= 2)
		{
			m_State |= CitizenFlags.FailedEducationBit1;
		}
		else
		{
			m_State &= ~CitizenFlags.FailedEducationBit1;
		}
		if (fails % 2 != 0)
		{
			m_State |= CitizenFlags.FailedEducationBit2;
		}
		else
		{
			m_State &= ~CitizenFlags.FailedEducationBit2;
		}
	}

	public void SetAge(CitizenAge newAge)
	{
		m_State = (CitizenFlags)((int)((uint)(m_State & ~(CitizenFlags.AgeBit1 | CitizenFlags.AgeBit2)) | (uint)(((newAge & CitizenAge.Adult) != CitizenAge.Child) ? 1 : 0)) | (((int)newAge % 2 != 0) ? 2 : 0));
	}

	public CitizenAge GetAge()
	{
		return (CitizenAge)(2 * (((m_State & CitizenFlags.AgeBit1) != CitizenFlags.None) ? 1 : 0) + (((m_State & CitizenFlags.AgeBit2) != CitizenFlags.None) ? 1 : 0));
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		CitizenFlags state = m_State;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((short)state);
		byte wellBeing = m_WellBeing;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(wellBeing);
		byte health = m_Health;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(health);
		byte leisureCounter = m_LeisureCounter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(leisureCounter);
		byte penaltyCounter = m_PenaltyCounter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(penaltyCounter);
		short birthDay = m_BirthDay;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(birthDay);
		ushort pseudoRandom = m_PseudoRandom;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pseudoRandom);
		int unemploymentCounter = m_UnemploymentCounter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(unemploymentCounter);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.saveOptimizations)
		{
			uint num = default(uint);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		short state = default(short);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref state);
		ref byte wellBeing = ref m_WellBeing;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref wellBeing);
		ref byte health = ref m_Health;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref health);
		ref byte leisureCounter = ref m_LeisureCounter;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref leisureCounter);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.penaltyCounter)
		{
			ref byte penaltyCounter = ref m_PenaltyCounter;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref penaltyCounter);
		}
		ref short birthDay = ref m_BirthDay;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref birthDay);
		m_State = (CitizenFlags)state;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.snow)
		{
			ref ushort pseudoRandom = ref m_PseudoRandom;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref pseudoRandom);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.economyFix)
		{
			ref int unemploymentCounter = ref m_UnemploymentCounter;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref unemploymentCounter);
		}
	}
}
