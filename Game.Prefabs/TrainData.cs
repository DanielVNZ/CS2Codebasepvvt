using Colossal.Serialization.Entities;
using Game.Net;
using Game.Vehicles;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct TrainData : IComponentData, IQueryTypeParameter, ISerializable
{
	public TrackTypes m_TrackType;

	public EnergyTypes m_EnergyType;

	public TrainFlags m_TrainFlags;

	public float m_MaxSpeed;

	public float m_Acceleration;

	public float m_Braking;

	public float2 m_Turning;

	public float2 m_BogieOffsets;

	public float2 m_AttachOffsets;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		TrackTypes trackType = m_TrackType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trackType);
		EnergyTypes energyType = m_EnergyType;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)energyType);
		TrainFlags trainFlags = m_TrainFlags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)trainFlags);
		float maxSpeed = m_MaxSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxSpeed);
		float acceleration = m_Acceleration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(acceleration);
		float braking = m_Braking;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(braking);
		float2 turning = m_Turning;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(turning);
		float2 bogieOffsets = m_BogieOffsets;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(bogieOffsets);
		float2 attachOffsets = m_AttachOffsets;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(attachOffsets);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		byte trackType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref trackType);
		byte energyType = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref energyType);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.trainPrefabFlags)
		{
			byte trainFlags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref trainFlags);
			m_TrainFlags = (TrainFlags)trainFlags;
		}
		ref float maxSpeed = ref m_MaxSpeed;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxSpeed);
		ref float acceleration = ref m_Acceleration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref acceleration);
		ref float braking = ref m_Braking;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref braking);
		ref float2 turning = ref m_Turning;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref turning);
		ref float2 bogieOffsets = ref m_BogieOffsets;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref bogieOffsets);
		ref float2 attachOffsets = ref m_AttachOffsets;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref attachOffsets);
		m_TrackType = (TrackTypes)trackType;
		m_EnergyType = (EnergyTypes)energyType;
	}
}
