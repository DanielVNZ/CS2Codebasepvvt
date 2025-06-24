using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct School : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_AverageGraduationTime;

	public float m_AverageFailProbability;

	public sbyte m_StudentWellbeing;

	public sbyte m_StudentHealth;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float averageGraduationTime = m_AverageGraduationTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(averageGraduationTime);
		float averageFailProbability = m_AverageFailProbability;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(averageFailProbability);
		sbyte studentWellbeing = m_StudentWellbeing;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(studentWellbeing);
		sbyte studentHealth = m_StudentHealth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(studentHealth);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref float averageGraduationTime = ref m_AverageGraduationTime;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref averageGraduationTime);
		ref float averageFailProbability = ref m_AverageFailProbability;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref averageFailProbability);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.happinessAdjustRefactoring)
		{
			ref sbyte studentWellbeing = ref m_StudentWellbeing;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref studentWellbeing);
			ref sbyte studentHealth = ref m_StudentHealth;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref studentHealth);
		}
	}
}
