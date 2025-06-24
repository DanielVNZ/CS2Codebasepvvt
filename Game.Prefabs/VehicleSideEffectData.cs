using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct VehicleSideEffectData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_Min;

	public float3 m_Max;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float3 min = m_Min;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(min);
		float3 max = m_Max;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(max);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float3 min = ref m_Min;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref min);
		ref float3 max = ref m_Max;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref max);
	}
}
