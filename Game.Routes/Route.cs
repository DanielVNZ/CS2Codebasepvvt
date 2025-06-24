using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct Route : IComponentData, IQueryTypeParameter, ISerializable
{
	public RouteFlags m_Flags;

	public uint m_OptionMask;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		RouteFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
		uint optionMask = m_OptionMask;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(optionMask);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		uint flags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
		m_Flags = (RouteFlags)flags;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.routePolicies)
		{
			ref uint optionMask = ref m_OptionMask;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref optionMask);
		}
	}
}
