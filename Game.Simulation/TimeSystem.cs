using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TimeSystem : GameSystemBase, ITimeSystem, IPostDeserialize
{
	private SimulationSystem m_SimulationSystem;

	public const int kTicksPerDay = 262144;

	private float m_Time;

	private float m_Date;

	private int m_Year = 1;

	private int m_DaysPerYear = 1;

	private uint m_InitialFrame;

	private EntityQuery m_TimeSettingGroup;

	private EntityQuery m_TimeDataQuery;

	public int startingYear { get; set; }

	public float normalizedTime => m_Time;

	public float normalizedDate => m_Date;

	public int year => m_Year;

	public int daysPerYear
	{
		get
		{
			if (m_DaysPerYear == 0 && !((EntityQuery)(ref m_TimeSettingGroup)).IsEmptyIgnoreFilter)
			{
				m_DaysPerYear = ((EntityQuery)(ref m_TimeSettingGroup)).GetSingleton<TimeSettingsData>().m_DaysPerYear;
				if (m_DaysPerYear == 0)
				{
					m_DaysPerYear = 1;
				}
			}
			return m_DaysPerYear;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TimeSettingGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_TimeSettingGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_TimeDataQuery);
	}

	public void PostDeserialize(Context context)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager;
		if (((EntityQuery)(ref m_TimeDataQuery)).IsEmpty)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity val = ((EntityManager)(ref entityManager)).CreateEntity();
			TimeData timeData = default(TimeData);
			timeData.SetDefaults(context);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<TimeData>(val, timeData);
		}
		if ((int)((Context)(ref context)).purpose == 1)
		{
			TimeData singleton = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>();
			Entity singletonEntity = ((EntityQuery)(ref m_TimeDataQuery)).GetSingletonEntity();
			singleton.m_FirstFrame = m_SimulationSystem.frameIndex;
			singleton.m_StartingYear = startingYear;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<TimeData>(singletonEntity, singleton);
		}
		UpdateTime();
	}

	protected int GetTicks(uint frameIndex, TimeSettingsData settings, TimeData data)
	{
		return (int)(frameIndex - data.m_FirstFrame) + Mathf.RoundToInt(data.TimeOffset * 262144f) + Mathf.RoundToInt(data.GetDateOffset(settings.m_DaysPerYear) * 262144f * (float)settings.m_DaysPerYear);
	}

	protected int GetTicks(TimeSettingsData settings, TimeData data)
	{
		return (int)(m_SimulationSystem.frameIndex - data.m_FirstFrame) + Mathf.RoundToInt(data.TimeOffset * 262144f) + Mathf.RoundToInt(data.GetDateOffset(settings.m_DaysPerYear) * 262144f * (float)settings.m_DaysPerYear);
	}

	protected double GetTimeWithOffset(TimeSettingsData settings, TimeData data, double renderingFrame)
	{
		return renderingFrame + (double)(data.TimeOffset * 262144f) + (double)(data.GetDateOffset(settings.m_DaysPerYear) * 262144f * (float)settings.m_DaysPerYear);
	}

	public float GetTimeOfDay(TimeSettingsData settings, TimeData data, double renderingFrame)
	{
		return (float)(GetTimeWithOffset(settings, data, renderingFrame) % 262144.0 / 262144.0);
	}

	protected float GetTimeOfDay(TimeSettingsData settings, TimeData data)
	{
		return (float)(GetTicks(settings, data) % 262144) / 262144f;
	}

	public float GetTimeOfYear(TimeSettingsData settings, TimeData data, double renderingFrame)
	{
		int num = 262144 * settings.m_DaysPerYear;
		return (float)(GetTimeWithOffset(settings, data, renderingFrame % (double)num) / (double)num);
	}

	protected float GetTimeOfYear(TimeSettingsData settings, TimeData data)
	{
		int num = 262144 * settings.m_DaysPerYear;
		return (float)(GetTicks(settings, data) % num) / (float)num;
	}

	public float GetElapsedYears(TimeSettingsData settings, TimeData data)
	{
		int num = 262144 * settings.m_DaysPerYear;
		return (float)(m_SimulationSystem.frameIndex - data.m_FirstFrame) / (float)num;
	}

	public float GetStartingDate(TimeSettingsData settings, TimeData data)
	{
		int num = 262144 * settings.m_DaysPerYear;
		return (float)(GetTicks(data.m_FirstFrame, settings, data) % num) / (float)num;
	}

	public int GetYear(TimeSettingsData settings, TimeData data)
	{
		int num = 262144 * settings.m_DaysPerYear;
		return data.m_StartingYear + Mathf.FloorToInt((float)(GetTicks(settings, data) / num));
	}

	public static int GetDay(uint frame, TimeData data)
	{
		return Mathf.FloorToInt((float)(frame - data.m_FirstFrame) / 262144f + data.TimeOffset);
	}

	public void DebugAdvanceTime(int minutes)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		TimeData singleton = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>();
		Entity singletonEntity = ((EntityQuery)(ref m_TimeDataQuery)).GetSingletonEntity();
		singleton.m_FirstFrame -= (uint)(minutes * 262144) / 1440u;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<TimeData>(singletonEntity, singleton);
	}

	private static DateTime CreateDateTime(int year, int day, int hour, int minute, float second)
	{
		DateTime result = new DateTime(0L, DateTimeKind.Utc).AddYears(year - 1).AddDays(day - 1).AddHours(hour)
			.AddMinutes(minute)
			.AddSeconds(second);
		if (result.IsDaylightSavingTime())
		{
			result = result.AddHours(1.0);
		}
		return result;
	}

	public DateTime GetDateTime(double renderingFrame)
	{
		TimeSettingsData singleton = ((EntityQuery)(ref m_TimeSettingGroup)).GetSingleton<TimeSettingsData>();
		TimeData singleton2 = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>();
		float timeOfDay = GetTimeOfDay(singleton, singleton2, renderingFrame);
		float timeOfYear = GetTimeOfYear(singleton, singleton2, renderingFrame);
		int num = Mathf.FloorToInt(24f * timeOfDay);
		int minute = Mathf.FloorToInt(60f * (24f * timeOfDay - (float)num));
		int day = 1 + Mathf.FloorToInt((float)daysPerYear * timeOfYear) % daysPerYear;
		return CreateDateTime(year, day, num, minute, Mathf.Repeat(timeOfDay, 1f));
	}

	public DateTime GetCurrentDateTime()
	{
		float num = normalizedTime;
		float num2 = normalizedDate;
		int num3 = Mathf.FloorToInt(24f * num);
		int minute = Mathf.FloorToInt(60f * (24f * num - (float)num3));
		int day = 1 + Mathf.FloorToInt((float)daysPerYear * num2) % daysPerYear;
		return CreateDateTime(year, day, num3, minute, Mathf.Repeat(num, 1f));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		UpdateTime();
	}

	private void UpdateTime()
	{
		TimeSettingsData singleton = ((EntityQuery)(ref m_TimeSettingGroup)).GetSingleton<TimeSettingsData>();
		TimeData singleton2 = ((EntityQuery)(ref m_TimeDataQuery)).GetSingleton<TimeData>();
		m_Time = GetTimeOfDay(singleton, singleton2);
		m_Date = GetTimeOfYear(singleton, singleton2);
		m_Year = GetYear(singleton, singleton2);
		m_DaysPerYear = singleton.m_DaysPerYear;
	}

	[Preserve]
	public TimeSystem()
	{
	}
}
