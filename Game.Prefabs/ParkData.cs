using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct ParkData : IComponentData, IQueryTypeParameter, ICombineData<ParkData>, ISerializable
{
	public short m_MaintenancePool;

	public bool m_AllowHomeless;

	public void Combine(ParkData otherData)
	{
		m_MaintenancePool += otherData.m_MaintenancePool;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		short maintenancePool = m_MaintenancePool;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintenancePool);
		bool allowHomeless = m_AllowHomeless;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(allowHomeless);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref short maintenancePool = ref m_MaintenancePool;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenancePool);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.homelessPark)
		{
			ref bool allowHomeless = ref m_AllowHomeless;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref allowHomeless);
		}
	}
}
