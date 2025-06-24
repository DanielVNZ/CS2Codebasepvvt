using UnityEngine;

namespace Game.Rendering;

public struct ColorSet
{
	public Color m_Channel0;

	public Color m_Channel1;

	public Color m_Channel2;

	public Color this[int index]
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			return (Color)(index switch
			{
				0 => m_Channel0, 
				1 => m_Channel1, 
				2 => m_Channel2, 
				_ => default(Color), 
			});
		}
		set
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			switch (index)
			{
			case 0:
				m_Channel0 = value;
				break;
			case 1:
				m_Channel1 = value;
				break;
			case 2:
				m_Channel2 = value;
				break;
			}
		}
	}

	public ColorSet(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		m_Channel0 = color;
		m_Channel1 = color;
		m_Channel2 = color;
	}
}
