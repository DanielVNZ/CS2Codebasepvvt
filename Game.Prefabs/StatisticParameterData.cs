using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

public struct StatisticParameterData : IBufferElementData
{
	public int m_Value;

	public Color m_Color;

	public StatisticParameterData(int value, Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_Value = value;
		m_Color = color;
	}
}
