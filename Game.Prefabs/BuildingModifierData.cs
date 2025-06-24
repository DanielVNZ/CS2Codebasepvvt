using Colossal.Mathematics;
using Game.Buildings;
using Unity.Entities;

namespace Game.Prefabs;

[InternalBufferCapacity(0)]
public struct BuildingModifierData : IBufferElementData
{
	public BuildingModifierType m_Type;

	public ModifierValueMode m_Mode;

	public Bounds1 m_Range;

	public BuildingModifierData(BuildingModifierType type, ModifierValueMode mode, Bounds1 range)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_Type = type;
		m_Mode = mode;
		m_Range = range;
	}
}
