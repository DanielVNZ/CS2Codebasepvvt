using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Areas;

public struct Extractor : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_ResourceAmount;

	public float m_MaxConcentration;

	public float m_ExtractedAmount;

	public float m_WorkAmount;

	public float m_HarvestedAmount;

	public float m_TotalExtracted;

	public VehicleWorkType m_WorkType;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float resourceAmount = m_ResourceAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(resourceAmount);
		float maxConcentration = m_MaxConcentration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxConcentration);
		float extractedAmount = m_ExtractedAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(extractedAmount);
		float workAmount = m_WorkAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(workAmount);
		float harvestedAmount = m_HarvestedAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(harvestedAmount);
		VehicleWorkType workType = m_WorkType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)workType);
		float totalExtracted = m_TotalExtracted;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(totalExtracted);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		ref float resourceAmount = ref m_ResourceAmount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref resourceAmount);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.resourceConcentration)
		{
			ref float maxConcentration = ref m_MaxConcentration;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxConcentration);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.extractedResources)
		{
			ref float extractedAmount = ref m_ExtractedAmount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref extractedAmount);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.harvestedResources)
		{
			ref float workAmount = ref m_WorkAmount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref workAmount);
			ref float harvestedAmount = ref m_HarvestedAmount;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref harvestedAmount);
			uint workType = default(uint);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref workType);
			m_WorkType = (VehicleWorkType)workType;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.totalExtractedResources)
		{
			ref float totalExtracted = ref m_TotalExtracted;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref totalExtracted);
		}
	}
}
