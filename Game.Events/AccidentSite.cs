using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Events;

public struct AccidentSite : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public Entity m_PoliceRequest;

	public AccidentSiteFlags m_Flags;

	public uint m_CreationFrame;

	public uint m_SecuredFrame;

	public AccidentSite(Entity _event, AccidentSiteFlags flags, uint currentFrame)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Event = _event;
		m_PoliceRequest = Entity.Null;
		m_Flags = flags;
		m_CreationFrame = currentFrame;
		m_SecuredFrame = 0u;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		Entity policeRequest = m_PoliceRequest;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(policeRequest);
		AccidentSiteFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		uint creationFrame = m_CreationFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(creationFrame);
		uint securedFrame = m_SecuredFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(securedFrame);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref Entity policeRequest = ref m_PoliceRequest;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref policeRequest);
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		ref uint creationFrame = ref m_CreationFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref creationFrame);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeImprovement)
		{
			ref uint securedFrame = ref m_SecuredFrame;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref securedFrame);
		}
		m_Flags = (AccidentSiteFlags)flags;
	}
}
