using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Net;
using Unity.Entities;

namespace Game.Prefabs;

public struct PlaceableNetData : IComponentData, IQueryTypeParameter, ISerializable
{
	public Bounds1 m_ElevationRange;

	public Entity m_UndergroundPrefab;

	public PlacementFlags m_PlacementFlags;

	public CompositionFlags m_SetUpgradeFlags;

	public CompositionFlags m_UnsetUpgradeFlags;

	public uint m_DefaultConstructionCost;

	public float m_DefaultUpkeepCost;

	public float m_SnapDistance;

	public float m_MinWaterElevation;

	public int m_XPReward;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)m_PlacementFlags);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		uint placementFlags = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref placementFlags);
		m_PlacementFlags = (PlacementFlags)placementFlags;
		m_SnapDistance = 8f;
	}
}
