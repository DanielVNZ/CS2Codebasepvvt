using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Net;

public struct CarLane : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_AccessRestriction;

	public CarLaneFlags m_Flags;

	public float m_DefaultSpeedLimit;

	public float m_SpeedLimit;

	public float m_Curviness;

	public ushort m_CarriagewayGroup;

	public byte m_BlockageStart;

	public byte m_BlockageEnd;

	public byte m_CautionStart;

	public byte m_CautionEnd;

	public byte m_FlowOffset;

	public byte m_LaneCrossCount;

	public Bounds1 blockageBounds => new Bounds1((float)(int)m_BlockageStart * 0.003921569f, (float)(int)m_BlockageEnd * 0.003921569f);

	public Bounds1 cautionBounds => new Bounds1((float)(int)m_CautionStart * 0.003921569f, (float)(int)m_CautionEnd * 0.003921569f);

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity accessRestriction = m_AccessRestriction;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(accessRestriction);
		CarLaneFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		float defaultSpeedLimit = m_DefaultSpeedLimit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(defaultSpeedLimit);
		float speedLimit = m_SpeedLimit;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(speedLimit);
		float curviness = m_Curviness;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(curviness);
		ushort carriagewayGroup = m_CarriagewayGroup;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(carriagewayGroup);
		byte blockageStart = m_BlockageStart;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(blockageStart);
		byte blockageEnd = m_BlockageEnd;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(blockageEnd);
		byte cautionStart = m_CautionStart;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cautionStart);
		byte cautionEnd = m_CautionEnd;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(cautionEnd);
		byte flowOffset = m_FlowOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(flowOffset);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindAccessRestriction)
		{
			ref Entity accessRestriction = ref m_AccessRestriction;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref accessRestriction);
		}
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref float defaultSpeedLimit = ref m_DefaultSpeedLimit;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref defaultSpeedLimit);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.modifiedSpeedLimit)
		{
			ref float speedLimit = ref m_SpeedLimit;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref speedLimit);
		}
		else
		{
			m_SpeedLimit = m_DefaultSpeedLimit;
		}
		ref float curviness = ref m_Curviness;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref curviness);
		ref ushort carriagewayGroup = ref m_CarriagewayGroup;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref carriagewayGroup);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.carLaneBlockage)
		{
			ref byte blockageStart = ref m_BlockageStart;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref blockageStart);
			ref byte blockageEnd = ref m_BlockageEnd;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref blockageEnd);
		}
		else
		{
			m_BlockageStart = byte.MaxValue;
			m_BlockageEnd = 0;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.trafficImprovements)
		{
			ref byte cautionStart = ref m_CautionStart;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref cautionStart);
			ref byte cautionEnd = ref m_CautionEnd;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref cautionEnd);
		}
		else
		{
			m_CautionStart = byte.MaxValue;
			m_CautionEnd = 0;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.pathfindImprovement)
		{
			ref byte flowOffset = ref m_FlowOffset;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flowOffset);
		}
		m_Flags = (CarLaneFlags)flags;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.roadSideConnectionImprovements)
		{
			if ((m_Flags & CarLaneFlags.Unsafe) != 0)
			{
				m_Flags |= CarLaneFlags.SideConnection;
			}
			else
			{
				m_Flags &= ~CarLaneFlags.SideConnection;
			}
		}
	}
}
