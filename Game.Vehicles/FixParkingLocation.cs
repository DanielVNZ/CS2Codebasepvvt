using Unity.Entities;

namespace Game.Vehicles;

public struct FixParkingLocation : IComponentData, IQueryTypeParameter
{
	public Entity m_ChangeLane;

	public Entity m_ResetLocation;

	public FixParkingLocation(Entity changeLane, Entity resetLocation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_ChangeLane = changeLane;
		m_ResetLocation = resetLocation;
	}
}
