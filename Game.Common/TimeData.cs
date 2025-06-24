using Colossal.Serialization.Entities;
using Unity.Entities;
using UnityEngine;

namespace Game.Common;

public struct TimeData : IComponentData, IQueryTypeParameter, IDefaultSerializable, ISerializable
{
	public uint m_FirstFrame;

	public int m_StartingYear;

	public byte m_StartingMonth;

	public byte m_StartingHour;

	public byte m_StartingMinutes;

	public float TimeOffset
	{
		get
		{
			return (float)(int)m_StartingHour / 24f + (float)(int)m_StartingMinutes / 1440f + 1E-05f;
		}
		set
		{
			m_StartingHour = (byte)Mathf.FloorToInt((value * 24f + 1E-05f) % 24f);
			m_StartingMinutes = (byte)(Mathf.RoundToInt(value * 1440f) % 60);
		}
	}

	public float GetDateOffset(int daysPerYear)
	{
		return (float)(int)m_StartingMonth / (float)daysPerYear;
	}

	public void SetDefaults(Context context)
	{
		m_FirstFrame = 0u;
		m_StartingYear = 2021;
		m_StartingMonth = 5;
		m_StartingHour = 7;
		m_StartingMinutes = 0;
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		ref uint firstFrame = ref m_FirstFrame;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref firstFrame);
		ref int startingYear = ref m_StartingYear;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startingYear);
		ref byte startingMonth = ref m_StartingMonth;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startingMonth);
		ref byte startingHour = ref m_StartingHour;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startingHour);
		ref byte startingMinutes = ref m_StartingMinutes;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref startingMinutes);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		uint firstFrame = m_FirstFrame;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(firstFrame);
		int startingYear = m_StartingYear;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startingYear);
		byte startingMonth = m_StartingMonth;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startingMonth);
		byte startingHour = m_StartingHour;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startingHour);
		byte startingMinutes = m_StartingMinutes;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(startingMinutes);
	}

	public static TimeData GetSingleton(EntityQuery query)
	{
		if (!((EntityQuery)(ref query)).IsEmptyIgnoreFilter)
		{
			return ((EntityQuery)(ref query)).GetSingleton<TimeData>();
		}
		return default(TimeData);
	}
}
