using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Citizens;

public struct Criminal : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Event;

	public ushort m_JailTime;

	public CriminalFlags m_Flags;

	public Criminal(Entity _event, CriminalFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Event = _event;
		m_JailTime = 0;
		m_Flags = flags;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_Event;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		ushort jailTime = m_JailTime;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(jailTime);
		CriminalFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((ushort)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref Entity reference = ref m_Event;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.policeImprovement2)
		{
			ref ushort jailTime = ref m_JailTime;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref jailTime);
			ushort flags = default(ushort);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (CriminalFlags)flags;
		}
		else
		{
			byte flags2 = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags2);
			m_Flags = (CriminalFlags)flags2;
		}
	}
}
