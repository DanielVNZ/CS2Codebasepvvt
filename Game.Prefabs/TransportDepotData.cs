using Colossal.Serialization.Entities;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public struct TransportDepotData : IComponentData, IQueryTypeParameter, ICombineData<TransportDepotData>, ISerializable
{
	public TransportType m_TransportType;

	public EnergyTypes m_EnergyTypes;

	public SizeClass m_SizeClass;

	public bool m_DispatchCenter;

	public int m_VehicleCapacity;

	public float m_ProductionDuration;

	public float m_MaintenanceDuration;

	public void Combine(TransportDepotData otherData)
	{
		m_EnergyTypes |= otherData.m_EnergyTypes;
		m_DispatchCenter |= otherData.m_DispatchCenter;
		m_VehicleCapacity += otherData.m_VehicleCapacity;
		m_ProductionDuration += otherData.m_ProductionDuration;
		m_MaintenanceDuration += otherData.m_MaintenanceDuration;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		bool dispatchCenter = m_DispatchCenter;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dispatchCenter);
		int vehicleCapacity = m_VehicleCapacity;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(vehicleCapacity);
		float productionDuration = m_ProductionDuration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(productionDuration);
		float maintenanceDuration = m_MaintenanceDuration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maintenanceDuration);
		TransportType transportType = m_TransportType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((int)transportType);
		EnergyTypes energyTypes = m_EnergyTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)energyTypes);
		SizeClass sizeClass = m_SizeClass;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)sizeClass);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		ref bool dispatchCenter = ref m_DispatchCenter;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref dispatchCenter);
		ref int vehicleCapacity = ref m_VehicleCapacity;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref vehicleCapacity);
		ref float productionDuration = ref m_ProductionDuration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref productionDuration);
		ref float maintenanceDuration = ref m_MaintenanceDuration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maintenanceDuration);
		int transportType = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref transportType);
		byte energyTypes = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref energyTypes);
		m_TransportType = (TransportType)transportType;
		m_EnergyTypes = (EnergyTypes)energyTypes;
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.BpPrefabData))
		{
			byte sizeClass = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref sizeClass);
			m_SizeClass = (SizeClass)sizeClass;
		}
		else
		{
			m_SizeClass = SizeClass.Large;
		}
	}
}
