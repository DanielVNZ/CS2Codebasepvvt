using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct TrainEngineData : IComponentData, IQueryTypeParameter, IEmptySerializable
{
	public int2 m_Count;

	public TrainEngineData(int minCount, int maxCount)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		m_Count = new int2(minCount, maxCount);
	}
}
