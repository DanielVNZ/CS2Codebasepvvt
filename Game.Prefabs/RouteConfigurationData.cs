using Unity.Entities;

namespace Game.Prefabs;

public struct RouteConfigurationData : IComponentData, IQueryTypeParameter
{
	public Entity m_PathfindNotification;

	public Entity m_CarPathVisualization;

	public Entity m_WatercraftPathVisualization;

	public Entity m_AircraftPathVisualization;

	public Entity m_TrainPathVisualization;

	public Entity m_HumanPathVisualization;

	public Entity m_MissingRoutePrefab;
}
