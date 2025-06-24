using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Economy;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Prefabs;

public struct WorkVehicleData : IComponentData, IQueryTypeParameter, ISerializable
{
	public VehicleWorkType m_WorkType;

	public MapFeature m_MapFeature;

	public Resource m_Resources;

	public float m_MaxWorkAmount;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte num = (byte)m_WorkType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		sbyte num2 = (sbyte)m_MapFeature;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		float maxWorkAmount = m_MaxWorkAmount;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxWorkAmount);
		Resource resources = m_Resources;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ulong)resources);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		byte workType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref workType);
		sbyte mapFeature = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref mapFeature);
		ref float maxWorkAmount = ref m_MaxWorkAmount;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxWorkAmount);
		m_WorkType = (VehicleWorkType)workType;
		m_MapFeature = (MapFeature)mapFeature;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.landfillVehicles)
		{
			ulong resources = default(ulong);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref resources);
			m_Resources = (Resource)resources;
		}
	}
}
