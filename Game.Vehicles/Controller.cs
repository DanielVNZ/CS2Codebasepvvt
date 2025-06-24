using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

public struct Controller : IComponentData, IQueryTypeParameter, IEmptySerializable
{
	public Entity m_Controller;

	public Controller(Entity controller)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Controller = controller;
	}
}
