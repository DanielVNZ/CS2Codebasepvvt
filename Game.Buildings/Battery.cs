using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct Battery : IComponentData, IQueryTypeParameter, ISerializable
{
	public long m_StoredEnergy;

	public int m_Capacity;

	public int m_LastFlow;

	public int storedEnergyHours => (int)(m_StoredEnergy / 85);

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		long storedEnergy = m_StoredEnergy;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(storedEnergy);
		int capacity = m_Capacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(capacity);
		int lastFlow = m_LastFlow;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(lastFlow);
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
		ref long storedEnergy = ref m_StoredEnergy;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref storedEnergy);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.batteryStats)
		{
			ref int capacity = ref m_Capacity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref capacity);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.batteryLastFlow)
		{
			ref int lastFlow = ref m_LastFlow;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref lastFlow);
		}
	}
}
