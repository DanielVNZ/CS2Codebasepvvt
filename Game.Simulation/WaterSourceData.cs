using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct WaterSourceData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_ConstantDepth;

	public float m_Amount;

	public float m_Radius;

	public float m_Multiplier;

	public float m_Polluted;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int constantDepth = m_ConstantDepth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(constantDepth);
		float amount = m_Amount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(amount);
		float radius = m_Radius;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(radius);
		float multiplier = m_Multiplier;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(multiplier);
		float polluted = m_Polluted;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(polluted);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		ref int constantDepth = ref m_ConstantDepth;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref constantDepth);
		ref float amount = ref m_Amount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref amount);
		ref float radius = ref m_Radius;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref radius);
		ref float multiplier = ref m_Multiplier;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref multiplier);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.waterPollution)
		{
			ref float polluted = ref m_Polluted;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref polluted);
		}
	}
}
