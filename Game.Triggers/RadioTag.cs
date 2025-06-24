using System;
using Game.Audio.Radio;
using Unity.Entities;

namespace Game.Triggers;

public struct RadioTag : IEquatable<RadioTag>
{
	public Entity m_Event;

	public Entity m_Target;

	public Radio.SegmentType m_SegmentType;

	public int m_EmergencyFrameDelay;

	public bool Equals(RadioTag other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (m_Event == other.m_Event && m_Target == other.m_Target)
		{
			return m_SegmentType == other.m_SegmentType;
		}
		return false;
	}
}
