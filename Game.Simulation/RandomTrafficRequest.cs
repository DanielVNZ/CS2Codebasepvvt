using Colossal.Serialization.Entities;
using Game.Net;
using Game.Vehicles;
using Unity.Entities;

namespace Game.Simulation;

public struct RandomTrafficRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public RoadTypes m_RoadType;

	public TrackTypes m_TrackType;

	public EnergyTypes m_EnergyTypes;

	public SizeClass m_SizeClass;

	public RandomTrafficRequestFlags m_Flags;

	public RandomTrafficRequest(Entity target, RoadTypes roadType, TrackTypes trackType, EnergyTypes energyTypes, SizeClass sizeClass, RandomTrafficRequestFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_RoadType = roadType;
		m_TrackType = trackType;
		m_EnergyTypes = energyTypes;
		m_SizeClass = sizeClass;
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		RoadTypes roadType = m_RoadType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)roadType);
		TrackTypes trackType = m_TrackType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trackType);
		EnergyTypes energyTypes = m_EnergyTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)energyTypes);
		SizeClass sizeClass = m_SizeClass;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)sizeClass);
		RandomTrafficRequestFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.randomTrafficTypes)
		{
			byte roadType = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref roadType);
			byte trackType = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref trackType);
			byte energyTypes = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref energyTypes);
			byte sizeClass = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref sizeClass);
			m_RoadType = (RoadTypes)roadType;
			m_TrackType = (TrackTypes)trackType;
			m_EnergyTypes = (EnergyTypes)energyTypes;
			m_SizeClass = (SizeClass)sizeClass;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.randomTrafficFlags)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (RandomTrafficRequestFlags)flags;
		}
	}
}
