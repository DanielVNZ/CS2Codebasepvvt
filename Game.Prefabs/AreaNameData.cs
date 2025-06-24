using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

public struct AreaNameData : IComponentData, IQueryTypeParameter
{
	public Color32 m_Color;

	public Color32 m_SelectedColor;

	public static AreaNameData GetDefaults()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		return new AreaNameData
		{
			m_Color = new Color32((byte)128, (byte)128, (byte)128, (byte)128),
			m_SelectedColor = new Color32((byte)128, (byte)128, (byte)128, byte.MaxValue)
		};
	}
}
