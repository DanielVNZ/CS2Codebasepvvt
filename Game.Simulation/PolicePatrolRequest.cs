using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct PolicePatrolRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Target;

	public float m_Priority;

	public byte m_DispatchIndex;

	public PolicePatrolRequest(Entity target, float priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Target = target;
		m_Priority = priority;
		m_DispatchIndex = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity target = m_Target;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(target);
		float priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
		byte dispatchIndex = m_DispatchIndex;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(dispatchIndex);
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
		if (((Context)(ref context)).version >= Version.requestDispatchIndex)
		{
			ref byte dispatchIndex = ref m_DispatchIndex;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref dispatchIndex);
		}
	}
}
