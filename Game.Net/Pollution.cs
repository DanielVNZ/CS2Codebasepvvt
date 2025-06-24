using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Net;

public struct Pollution : IComponentData, IQueryTypeParameter, ISerializable
{
	public float2 m_Pollution;

	public float2 m_Accumulation;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float2 pollution = m_Pollution;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(pollution);
		float2 accumulation = m_Accumulation;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accumulation);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		ref float2 pollution = ref m_Pollution;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref pollution);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.netPollutionAccumulation)
		{
			ref float2 accumulation = ref m_Accumulation;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref accumulation);
		}
		else
		{
			m_Accumulation = m_Pollution * 2f;
		}
	}
}
