using UnityEngine;

namespace Game.Prefabs;

public abstract class ColorInfomodeBasePrefab : InfomodeBasePrefab, IColorInfomode
{
	public Color m_Color;

	public Color color => m_Color;

	public override void GetColors(out Color color0, out Color color1, out Color color2, out float steps, out float speed, out float tiling, out float fill)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		color0 = m_Color;
		color1 = m_Color;
		color2 = m_Color;
		steps = 1f;
		speed = 0f;
		tiling = 0f;
		fill = 0f;
	}
}
