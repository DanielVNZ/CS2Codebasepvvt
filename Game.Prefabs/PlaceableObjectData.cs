using Colossal.Serialization.Entities;
using Game.Net;
using Game.Objects;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public struct PlaceableObjectData : IComponentData, IQueryTypeParameter, ISerializable
{
	public float3 m_PlacementOffset;

	public uint m_ConstructionCost;

	public int m_XPReward;

	public byte m_DefaultProbability;

	public RotationSymmetry m_RotationSymmetry;

	public SubReplacementType m_SubReplacementType;

	public Game.Objects.PlacementFlags m_Flags;

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref float3 placementOffset = ref m_PlacementOffset;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref placementOffset);
		ref uint constructionCost = ref m_ConstructionCost;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref constructionCost);
		ref int xPReward = ref m_XPReward;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref xPReward);
		uint num = default(uint);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_Flags = (Game.Objects.PlacementFlags)((int)num & -32769);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float3 placementOffset = m_PlacementOffset;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(placementOffset);
		uint constructionCost = m_ConstructionCost;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(constructionCost);
		int xPReward = m_XPReward;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(xPReward);
		Game.Objects.PlacementFlags flags = m_Flags;
		((IWriter)writer/*cast due to .constrained prefix*/).Write((uint)flags);
	}
}
