using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct PostVanRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public PostVanRequestFlags m_Flags;

	public byte m_DispatchIndex;

	public ushort m_Priority;

	public PostVanRequest(Entity target, PostVanRequestFlags flags, ushort priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_Flags = flags;
		m_Priority = priority;
		m_DispatchIndex = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		PostVanRequestFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)flags);
		ushort priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		byte dispatchIndex = m_DispatchIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dispatchIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		ushort num = default(ushort);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		ref ushort priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		m_Flags = (PostVanRequestFlags)num;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.requestDispatchIndex)
		{
			ref byte dispatchIndex = ref m_DispatchIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dispatchIndex);
		}
	}
}
