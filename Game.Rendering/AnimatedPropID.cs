namespace Game.Rendering;

public struct AnimatedPropID
{
	private int m_Index;

	public bool isValid => m_Index >= 0;

	public AnimatedPropID(int index)
	{
		m_Index = index;
	}

	public static bool operator ==(AnimatedPropID a, AnimatedPropID b)
	{
		return a.m_Index == b.m_Index;
	}

	public static bool operator !=(AnimatedPropID a, AnimatedPropID b)
	{
		return a.m_Index != b.m_Index;
	}

	public override bool Equals(object obj)
	{
		if (obj is AnimatedPropID animatedPropID)
		{
			return this == animatedPropID;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return m_Index.GetHashCode();
	}
}
