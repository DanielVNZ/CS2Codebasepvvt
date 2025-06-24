using Colossal.Serialization.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct GroundWater : IStrideSerializable, ISerializable
{
	public short m_Amount;

	public short m_Polluted;

	public short m_Max;

	public void Consume(int amount)
	{
		if (m_Amount > 0)
		{
			float num = (float)m_Polluted / (float)m_Amount;
			m_Amount -= (short)math.clamp(amount, 0, (int)m_Amount);
			m_Polluted = (short)math.clamp(math.round(num * (float)m_Amount), 0f, (float)m_Amount);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		short amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		short polluted = m_Polluted;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(polluted);
		short max = m_Max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref short amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		ref short polluted = ref m_Polluted;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref polluted);
		ref short max = ref m_Max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.groundWaterPollutionFix)
		{
			m_Amount = (short)math.clamp((int)m_Amount, 0, (int)m_Max);
			m_Polluted = (short)math.clamp((int)m_Polluted, 0, (int)m_Amount);
		}
	}

	public int GetStride(Context context)
	{
		return 6;
	}
}
