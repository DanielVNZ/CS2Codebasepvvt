using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Routes;

public struct RouteInfo : IComponentData, IQueryTypeParameter, ISerializable
{
	public float m_Duration;

	public float m_Distance;

	public RouteInfoFlags m_Flags;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float duration = m_Duration;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(duration);
		float distance = m_Distance;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(distance);
		RouteInfoFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)flags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref float duration = ref m_Duration;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref duration);
		ref float distance = ref m_Distance;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref distance);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.transportLinePolicies)
		{
			byte flags = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref flags);
			m_Flags = (RouteInfoFlags)flags;
		}
	}
}
