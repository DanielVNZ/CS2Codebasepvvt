using Unity.Entities;

namespace Game.Tools;

public struct Temp : IComponentData, IQueryTypeParameter
{
	public Entity m_Original;

	public float m_CurvePosition;

	public int m_Value;

	public int m_Cost;

	public TempFlags m_Flags;

	public Temp(Entity original, TempFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Original = original;
		m_CurvePosition = 0f;
		m_Value = 0;
		m_Cost = 0;
		m_Flags = flags;
	}
}
