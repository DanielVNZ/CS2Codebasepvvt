using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Vehicles;

public struct Blocker : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Blocker;

	public BlockerType m_Type;

	public byte m_MaxSpeed;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity blocker = m_Blocker;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(blocker);
		BlockerType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)type);
		byte maxSpeed = m_MaxSpeed;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(maxSpeed);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref Entity blocker = ref m_Blocker;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref blocker);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.trafficBottlenecks)
		{
			byte type = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
			ref byte maxSpeed = ref m_MaxSpeed;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref maxSpeed);
			m_Type = (BlockerType)type;
		}
		else
		{
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_MaxSpeed = (byte)math.clamp(num * 5f, 0f, 255f);
			m_Type = BlockerType.None;
		}
	}
}
