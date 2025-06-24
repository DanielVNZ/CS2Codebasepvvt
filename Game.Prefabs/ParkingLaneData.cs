using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct ParkingLaneData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float2 m_SlotSize;

	public float m_SlotAngle;

	public float m_SlotInterval;

	public float m_MaxCarLength;

	public RoadTypes m_RoadTypes;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float2 slotSize = m_SlotSize;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(slotSize);
		float slotAngle = m_SlotAngle;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(slotAngle);
		float slotInterval = m_SlotInterval;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(slotInterval);
		float maxCarLength = m_MaxCarLength;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxCarLength);
		RoadTypes roadTypes = m_RoadTypes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)roadTypes);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		ref float2 slotSize = ref m_SlotSize;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref slotSize);
		ref float slotAngle = ref m_SlotAngle;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref slotAngle);
		ref float slotInterval = ref m_SlotInterval;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref slotInterval);
		ref float maxCarLength = ref m_MaxCarLength;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxCarLength);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.roadPatchImprovements)
		{
			byte roadTypes = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref roadTypes);
			m_RoadTypes = (RoadTypes)roadTypes;
		}
		else
		{
			m_RoadTypes = RoadTypes.Car;
		}
	}
}
