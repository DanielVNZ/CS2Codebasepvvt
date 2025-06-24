using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct HospitalData : IComponentData, IQueryTypeParameter, ICombineData<HospitalData>, ISerializable
{
	public int m_AmbulanceCapacity;

	public int m_MedicalHelicopterCapacity;

	public int m_PatientCapacity;

	public int m_TreatmentBonus;

	public int2 m_HealthRange;

	public bool m_TreatDiseases;

	public bool m_TreatInjuries;

	public void Combine(HospitalData otherData)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		m_AmbulanceCapacity += otherData.m_AmbulanceCapacity;
		m_MedicalHelicopterCapacity += otherData.m_MedicalHelicopterCapacity;
		m_PatientCapacity += otherData.m_PatientCapacity;
		m_TreatmentBonus += otherData.m_TreatmentBonus;
		m_HealthRange.x = math.min(m_HealthRange.x, otherData.m_HealthRange.x);
		m_HealthRange.y = math.max(m_HealthRange.y, otherData.m_HealthRange.y);
		m_TreatDiseases |= otherData.m_TreatDiseases;
		m_TreatInjuries |= otherData.m_TreatInjuries;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		int ambulanceCapacity = m_AmbulanceCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(ambulanceCapacity);
		int medicalHelicopterCapacity = m_MedicalHelicopterCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(medicalHelicopterCapacity);
		int patientCapacity = m_PatientCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(patientCapacity);
		int treatmentBonus = m_TreatmentBonus;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(treatmentBonus);
		int2 healthRange = m_HealthRange;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(healthRange);
		bool treatDiseases = m_TreatDiseases;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(treatDiseases);
		bool treatInjuries = m_TreatInjuries;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(treatInjuries);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref int ambulanceCapacity = ref m_AmbulanceCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref ambulanceCapacity);
		ref int medicalHelicopterCapacity = ref m_MedicalHelicopterCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref medicalHelicopterCapacity);
		ref int patientCapacity = ref m_PatientCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref patientCapacity);
		ref int treatmentBonus = ref m_TreatmentBonus;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref treatmentBonus);
		ref int2 healthRange = ref m_HealthRange;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref healthRange);
		ref bool treatDiseases = ref m_TreatDiseases;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref treatDiseases);
		ref bool treatInjuries = ref m_TreatInjuries;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref treatInjuries);
	}
}
