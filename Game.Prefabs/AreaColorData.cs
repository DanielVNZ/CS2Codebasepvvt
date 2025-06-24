using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

public struct AreaColorData : IComponentData, IQueryTypeParameter
{
	public Color32 m_FillColor;

	public Color32 m_EdgeColor;

	public Color32 m_SelectionFillColor;

	public Color32 m_SelectionEdgeColor;

	public static AreaColorData GetDefaults()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		return new AreaColorData
		{
			m_FillColor = new Color32((byte)128, (byte)128, (byte)128, (byte)64),
			m_EdgeColor = new Color32((byte)128, (byte)128, (byte)128, (byte)128),
			m_SelectionFillColor = new Color32((byte)128, (byte)128, (byte)128, (byte)128),
			m_SelectionEdgeColor = new Color32((byte)128, (byte)128, (byte)128, byte.MaxValue)
		};
	}
}
