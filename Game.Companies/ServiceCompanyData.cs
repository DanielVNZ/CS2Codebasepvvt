using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Companies;

public struct ServiceCompanyData : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_MaxService;

	public int m_WorkPerUnit;

	public float m_MaxWorkersPerCell;

	public int m_ServiceConsuming;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int maxService = m_MaxService;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxService);
		int workPerUnit = m_WorkPerUnit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(workPerUnit);
		float maxWorkersPerCell = m_MaxWorkersPerCell;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxWorkersPerCell);
		int serviceConsuming = m_ServiceConsuming;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(serviceConsuming);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ref int maxService = ref m_MaxService;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxService);
		ref int workPerUnit = ref m_WorkPerUnit;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref workPerUnit);
		ref float maxWorkersPerCell = ref m_MaxWorkersPerCell;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxWorkersPerCell);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.serviceCompanyConsuming)
		{
			ref int serviceConsuming = ref m_ServiceConsuming;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref serviceConsuming);
		}
	}
}
