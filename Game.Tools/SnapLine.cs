using Colossal.Mathematics;

namespace Game.Tools;

public struct SnapLine
{
	public ControlPoint m_ControlPoint;

	public Bezier4x3 m_Curve;

	public SnapLineFlags m_Flags;

	public float m_HeightWeight;

	public SnapLine(ControlPoint position, Bezier4x3 curve, SnapLineFlags flags, float heightWeight)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_ControlPoint = position;
		m_Curve = curve;
		m_Flags = flags;
		m_HeightWeight = heightWeight;
	}
}
