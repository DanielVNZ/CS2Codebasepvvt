using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct WorkplaceData : IComponentData, IQueryTypeParameter, ICombineData<WorkplaceData>, ISerializable
{
	public WorkplaceComplexity m_Complexity;

	public int m_MaxWorkers;

	public float m_EveningShiftProbability;

	public float m_NightShiftProbability;

	public int m_MinimumWorkersLimit;

	public void Combine(WorkplaceData other)
	{
		int maxWorkers = m_MaxWorkers;
		m_MaxWorkers += other.m_MaxWorkers;
		m_MinimumWorkersLimit += m_MinimumWorkersLimit;
		m_EveningShiftProbability = math.lerp(other.m_EveningShiftProbability, m_EveningShiftProbability, (float)(maxWorkers / m_MaxWorkers));
		m_NightShiftProbability = math.lerp(other.m_NightShiftProbability, m_NightShiftProbability, (float)(maxWorkers / m_MaxWorkers));
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int maxWorkers = m_MaxWorkers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxWorkers);
		float eveningShiftProbability = m_EveningShiftProbability;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(eveningShiftProbability);
		float nightShiftProbability = m_NightShiftProbability;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(nightShiftProbability);
		byte num = (byte)m_Complexity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int minimumWorkersLimit = m_MinimumWorkersLimit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(minimumWorkersLimit);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		ref int maxWorkers = ref m_MaxWorkers;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxWorkers);
		ref float eveningShiftProbability = ref m_EveningShiftProbability;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref eveningShiftProbability);
		ref float nightShiftProbability = ref m_NightShiftProbability;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref nightShiftProbability);
		byte complexity = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref complexity);
		m_Complexity = (WorkplaceComplexity)complexity;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version > Version.addMinimumWorkersLimit)
		{
			ref int minimumWorkersLimit = ref m_MinimumWorkersLimit;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref minimumWorkersLimit);
		}
	}
}
