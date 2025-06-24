using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct SchoolData : IComponentData, IQueryTypeParameter, ICombineData<SchoolData>, ISerializable
{
	public int m_StudentCapacity;

	public float m_GraduationModifier;

	public byte m_EducationLevel;

	public sbyte m_StudentWellbeing;

	public sbyte m_StudentHealth;

	public void Combine(SchoolData otherData)
	{
		m_StudentCapacity += otherData.m_StudentCapacity;
		m_EducationLevel = (byte)math.max((int)m_EducationLevel, (int)otherData.m_EducationLevel);
		m_GraduationModifier += otherData.m_GraduationModifier;
		m_StudentWellbeing += otherData.m_StudentWellbeing;
		m_StudentHealth += otherData.m_StudentHealth;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int studentCapacity = m_StudentCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(studentCapacity);
		float graduationModifier = m_GraduationModifier;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(graduationModifier);
		byte educationLevel = m_EducationLevel;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(educationLevel);
		sbyte studentWellbeing = m_StudentWellbeing;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(studentWellbeing);
		sbyte studentHealth = m_StudentHealth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(studentHealth);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref int studentCapacity = ref m_StudentCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref studentCapacity);
		ref float graduationModifier = ref m_GraduationModifier;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref graduationModifier);
		ref byte educationLevel = ref m_EducationLevel;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref educationLevel);
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
