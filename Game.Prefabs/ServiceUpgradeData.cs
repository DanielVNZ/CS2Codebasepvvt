using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Prefabs;

public struct ServiceUpgradeData : IComponentData, IQueryTypeParameter, ISerializable
{
	public uint m_UpgradeCost;

	public int m_XPReward;

	public int m_MaxPlacementOffset;

	public float m_MaxPlacementDistance;

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		uint upgradeCost = m_UpgradeCost;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(upgradeCost);
		int xPReward = m_XPReward;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(xPReward);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref uint upgradeCost = ref m_UpgradeCost;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref upgradeCost);
		ref int xPReward = ref m_XPReward;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref xPReward);
	}
}
