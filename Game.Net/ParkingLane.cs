using Colossal.Serialization.Entities;
using Game.Pathfind;
using Unity.Entities;

namespace Game.Net;

public struct ParkingLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_AccessRestriction;

	public PathNode m_SecondaryStartNode;

	public ParkingLaneFlags m_Flags;

	public float m_FreeSpace;

	public ushort m_ParkingFee;

	public ushort m_ComfortFactor;

	public ushort m_TaxiAvailability;

	public ushort m_TaxiFee;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity accessRestriction = m_AccessRestriction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accessRestriction);
		PathNode secondaryStartNode = m_SecondaryStartNode;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<PathNode>(secondaryStartNode);
		ParkingLaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		float freeSpace = m_FreeSpace;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(freeSpace);
		ushort parkingFee = m_ParkingFee;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(parkingFee);
		ushort comfortFactor = m_ComfortFactor;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(comfortFactor);
		ushort taxiAvailability = m_TaxiAvailability;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(taxiAvailability);
		ushort taxiFee = m_TaxiFee;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(taxiFee);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindAccessRestriction)
		{
			ref Entity accessRestriction = ref m_AccessRestriction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessRestriction);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.parkingLaneImprovement)
		{
			ref PathNode secondaryStartNode = ref m_SecondaryStartNode;
			((IReader)reader/*cast due to .constrained prefix*/).Read<PathNode>(ref secondaryStartNode);
		}
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float freeSpace = ref m_FreeSpace;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref freeSpace);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.parkingLaneImprovement2)
		{
			ref ushort parkingFee = ref m_ParkingFee;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref parkingFee);
			ref ushort comfortFactor = ref m_ComfortFactor;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref comfortFactor);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiDispatchCenter)
		{
			ref ushort taxiAvailability = ref m_TaxiAvailability;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref taxiAvailability);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.taxiFee)
		{
			ref ushort taxiFee = ref m_TaxiFee;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref taxiFee);
		}
		m_Flags = (ParkingLaneFlags)flags;
	}
}
