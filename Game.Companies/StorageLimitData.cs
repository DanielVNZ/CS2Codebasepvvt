using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;

namespace Game.Companies;

public struct StorageLimitData : IComponentData, IQueryTypeParameter, ISerializable, ICombineData<StorageLimitData>
{
	public int m_Limit;

	public int GetAdjustedLimitForWarehouse(SpawnableBuildingData spawnable, BuildingData building)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return m_Limit * spawnable.m_Level * building.m_LotSize.x * building.m_LotSize.y;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Limit);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Limit);
	}

	public void Combine(StorageLimitData otherData)
	{
		m_Limit += otherData.m_Limit;
	}
}
