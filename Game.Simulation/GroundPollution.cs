using Colossal.Serialization.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public struct GroundPollution : IPollution, IStrideSerializable, ISerializable
{
	public short m_Pollution;

	public void Add(short amount)
	{
		m_Pollution = (short)math.min(32767, m_Pollution + amount);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Pollution);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		ref short pollution = ref m_Pollution;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pollution);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.groundPollutionDelta)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.removeGroundPollutionDelta)
			{
				short num = default(short);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			}
		}
	}

	public int GetStride(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((Context)(ref context)).version < Version.removeGroundPollutionDelta)
		{
			return 4;
		}
		return 2;
	}
}
