using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct ElectricityProducer : IComponentData, IQueryTypeParameter, ISerializable
{
	public int m_Capacity;

	public int m_LastProduction;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		int lastProduction = m_LastProduction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastProduction);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		ref int capacity = ref m_Capacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.powerPlantConsumption)
		{
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version < Version.serviceConsumption)
			{
				int num = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			}
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.powerPlantLastFlow)
		{
			ref int lastProduction = ref m_LastProduction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastProduction);
		}
	}
}
