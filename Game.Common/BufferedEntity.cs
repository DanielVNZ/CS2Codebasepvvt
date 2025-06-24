using Unity.Entities;

namespace Game.Common;

public struct BufferedEntity
{
	public Entity m_Value;

	public bool m_Stored;

	public BufferedEntity(Entity value, bool stored)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Value = value;
		m_Stored = stored;
	}

	public override string ToString()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return string.Format("{0}: {1}, {2}: {3}", "m_Value", m_Value, "m_Stored", m_Stored);
	}
}
