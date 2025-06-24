using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct EmergencyGeneratorData : IComponentData, IQueryTypeParameter, ICombineData<EmergencyGeneratorData>, ISerializable
{
	public int m_ElectricityProduction;

	public Bounds1 m_ActivationThreshold;

	public void Combine(EmergencyGeneratorData otherData)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		m_ElectricityProduction += otherData.m_ElectricityProduction;
		m_ActivationThreshold = new Bounds1(math.max(otherData.m_ActivationThreshold.min, m_ActivationThreshold.min), math.max(otherData.m_ActivationThreshold.max, m_ActivationThreshold.max));
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int electricityProduction = m_ElectricityProduction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(electricityProduction);
		float min = m_ActivationThreshold.min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float max = m_ActivationThreshold.max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int electricityProduction = ref m_ElectricityProduction;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref electricityProduction);
		ref float min = ref m_ActivationThreshold.min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float max = ref m_ActivationThreshold.max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
	}
}
