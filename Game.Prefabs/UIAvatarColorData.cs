using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

public struct UIAvatarColorData : IBufferElementData
{
	public Color32 m_Color;

	public UIAvatarColorData(Color32 color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Color = color;
	}
}
