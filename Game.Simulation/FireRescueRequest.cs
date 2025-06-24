using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct FireRescueRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public float m_Priority;

	public FireRescueRequestType m_Type;

	public FireRescueRequest(Entity target, float priority, FireRescueRequestType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_Priority = priority;
		m_Type = type;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		float priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		FireRescueRequestType type = m_Type;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)type);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ref Entity target = ref m_Target;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref target);
		ref float priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.disasterResponse)
		{
			byte type = default(byte);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref type);
			m_Type = (FireRescueRequestType)type;
		}
	}
}
