using System;
using Unity.Mathematics;

namespace Game.Common;

public struct RandomSeed
{
	private static Random m_Random = new Random((uint)DateTime.Now.Ticks);

	private uint m_Seed;

	public static RandomSeed Next()
	{
		return new RandomSeed
		{
			m_Seed = ((Random)(ref m_Random)).NextUInt()
		};
	}

	public Random GetRandom(int index)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		uint num = m_Seed ^ (uint)(370248451 * index);
		return new Random(math.select(num, 1851936439u, num == 0));
	}
}
