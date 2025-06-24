using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct InDanger : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public Entity m_EvacuationRequest;

	public DangerFlags m_Flags;

	public uint m_EndFrame;

	public InDanger(Entity _event, Entity evacuationRequest, DangerFlags flags, uint endFrame)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Event = _event;
		m_EvacuationRequest = evacuationRequest;
		m_Flags = flags;
		m_EndFrame = endFrame;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		Entity evacuationRequest = m_EvacuationRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(evacuationRequest);
		DangerFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		uint endFrame = m_EndFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(endFrame);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref Entity evacuationRequest = ref m_EvacuationRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref evacuationRequest);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (DangerFlags)flags;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.dangerTimeout)
		{
			ref uint endFrame = ref m_EndFrame;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref endFrame);
		}
	}
}
