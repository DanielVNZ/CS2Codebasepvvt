using Colossal.Serialization.Entities;
using Game.Economy;
using Unity.Collections;
using Unity.Entities;

namespace Game.Prefabs;

public struct UpkeepModifierData : IBufferElementData, ICombineBuffer<UpkeepModifierData>, ISerializable
{
	public Resource m_Resource;

	public float m_Multiplier;

	public float Transform(float upkeep)
	{
		upkeep *= m_Multiplier;
		return upkeep;
	}

	public void Combine(NativeList<UpkeepModifierData> result)
	{
		for (int i = 0; i < result.Length; i++)
		{
			ref UpkeepModifierData reference = ref result.ElementAt(i);
			if (reference.m_Resource == m_Resource)
			{
				reference.m_Multiplier *= m_Multiplier;
				return;
			}
		}
		result.Add(ref this);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float multiplier = m_Multiplier;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(multiplier);
		sbyte num = (sbyte)EconomyUtils.GetResourceIndex(m_Resource);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ref float multiplier = ref m_Multiplier;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref multiplier);
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.upkeepModifierRelative)
		{
			float num = default(float);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		}
		sbyte index = default(sbyte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref index);
		m_Resource = EconomyUtils.GetResource(index);
	}
}
