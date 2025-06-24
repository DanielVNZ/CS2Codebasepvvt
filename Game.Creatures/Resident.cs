using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Creatures;

public struct Resident : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Citizen;

	public ResidentFlags m_Flags;

	public int m_Timer;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		ResidentFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		int timer = m_Timer;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(timer);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.transportWaitTimer)
		{
			ref int timer = ref m_Timer;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref timer);
		}
		m_Flags = (ResidentFlags)flags;
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.yogaAreaFix)
		{
			m_Flags &= ~ResidentFlags.IgnoreAreas;
		}
	}
}
