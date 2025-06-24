using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct GarbageCollectionRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public int m_Priority;

	public GarbageCollectionRequestFlags m_Flags;

	public byte m_DispatchIndex;

	public GarbageCollectionRequest(Entity target, int priority, GarbageCollectionRequestFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_Priority = priority;
		m_Flags = flags;
		m_DispatchIndex = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		int priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		GarbageCollectionRequestFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
		byte dispatchIndex = m_DispatchIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dispatchIndex);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		ref int priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.industrialWaste)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (GarbageCollectionRequestFlags)flags;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.requestDispatchIndex)
		{
			ref byte dispatchIndex = ref m_DispatchIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dispatchIndex);
		}
	}
}
