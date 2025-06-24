using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Simulation;

public struct TransportVehicleRequest : IComponentData, IQueryTypeParameter, ISerializable
{
	public Entity m_Route;

	public float m_Priority;

	public TransportVehicleRequest(Entity route, float priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Route = route;
		m_Priority = priority;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Entity route = m_Route;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(route);
		float priority = m_Priority;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(priority);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref Entity route = ref m_Route;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref route);
		ref float priority = ref m_Priority;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref priority);
	}
}
