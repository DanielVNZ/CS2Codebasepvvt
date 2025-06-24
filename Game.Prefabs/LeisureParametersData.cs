using Game.Agents;
using Unity.Entities;

namespace Game.Prefabs;

public struct LeisureParametersData : IComponentData, IQueryTypeParameter
{
	public Entity m_TravelingPrefab;

	public Entity m_AttractionPrefab;

	public Entity m_SightseeingPrefab;

	public int m_LeisureRandomFactor;

	public int m_TouristLodgingConsumePerDay;

	public int m_TouristServiceConsumePerDay;

	public Entity GetPrefab(LeisureType type)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return (Entity)(type switch
		{
			LeisureType.Travel => m_TravelingPrefab, 
			LeisureType.Attractions => m_AttractionPrefab, 
			LeisureType.Sightseeing => m_SightseeingPrefab, 
			_ => default(Entity), 
		});
	}
}
