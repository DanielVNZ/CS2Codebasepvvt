using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Common;

[InternalBufferCapacity(1)]
public struct RandomLocalizationIndex : IBufferElementData, ISerializable
{
	public static readonly RandomLocalizationIndex kNone = new RandomLocalizationIndex(-1);

	public int m_Index;

	public RandomLocalizationIndex(int index)
	{
		m_Index = index;
	}

	public static void GenerateRandomIndices(DynamicBuffer<RandomLocalizationIndex> indices, DynamicBuffer<LocalizationCount> counts, ref Random random)
	{
		indices.ResizeUninitialized(counts.Length);
		for (int i = 0; i < counts.Length; i++)
		{
			int count = counts[i].m_Count;
			indices[i] = new RandomLocalizationIndex((count > 0) ? ((Random)(ref random)).NextInt(count) : (-1));
		}
	}

	public static void EnsureValidRandomIndices(DynamicBuffer<RandomLocalizationIndex> indices, DynamicBuffer<LocalizationCount> counts, ref Random random)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (indices.Length == counts.Length)
		{
			for (int i = 0; i < counts.Length; i++)
			{
				int count = counts[i].m_Count;
				if (indices[i].m_Index < 0 || indices[i].m_Index >= count)
				{
					indices[i] = new RandomLocalizationIndex((count > 0) ? ((Random)(ref random)).NextInt(count) : (-1));
				}
			}
		}
		else
		{
			GenerateRandomIndices(indices, counts, ref random);
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Index);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref m_Index);
	}
}
