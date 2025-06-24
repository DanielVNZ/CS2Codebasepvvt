using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class TimeUISystem : UISystemBase
{
	private struct TimeSettings : IJsonWritable, IEquatable<TimeSettings>
	{
		public int ticksPerDay;

		public int daysPerYear;

		public int epochTicks;

		public int epochYear;

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("ticksPerDay");
			writer.Write(ticksPerDay);
			writer.PropertyName("daysPerYear");
			writer.Write(daysPerYear);
			writer.PropertyName("epochTicks");
			writer.Write(epochTicks);
			writer.PropertyName("epochYear");
			writer.Write(epochYear);
			writer.TypeEnd();
		}

		public bool Equals(TimeSettings other)
		{
			if (ticksPerDay == other.ticksPerDay && daysPerYear == other.daysPerYear && epochTicks == other.epochTicks)
			{
				return epochYear == other.epochYear;
			}
			return false;
		}
	}

	private const string kGroup = "time";

	private SimulationSystem m_SimulationSystem;

	private TimeSystem m_TimeSystem;

	private LightingSystem m_LightingSystem;

	private EntityQuery m_TimeSettingsQuery;

	private EntityQuery m_TimeDataQuery;

	private EventBinding<bool> m_SimulationPausedBarrierBinding;

	private float m_SpeedBeforePause = 1f;

	private bool m_UnpausedBeforeForcedPause;

	private bool m_HasFocus = true;

	private bool pausedBarrierActive => ((EventBindingBase)m_SimulationPausedBarrierBinding).observerCount > 0;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected O, but got Unknown
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_LightingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LightingSystem>();
		m_TimeSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeSettingsData>() });
		m_TimeDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<TimeSettings>("time", "timeSettings", (Func<TimeSettings>)GetTimeSettings, (IWriter<TimeSettings>)(object)new ValueWriter<TimeSettings>(), (EqualityComparer<TimeSettings>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("time", "ticks", (Func<int>)GetTicks, (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("time", "day", (Func<int>)GetDay, (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<LightingSystem.State>("time", "lightingState", (Func<LightingSystem.State>)GetLightingState, (IWriter<LightingSystem.State>)(object)new DelegateWriter<LightingSystem.State>((WriterDelegate<LightingSystem.State>)delegate(IJsonWriter writer, LightingSystem.State value)
		{
			writer.Write((int)value);
		}), (EqualityComparer<LightingSystem.State>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("time", "simulationPaused", (Func<bool>)IsPaused, (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("time", "simulationSpeed", (Func<int>)GetSimulationSpeed, (IWriter<int>)null, (EqualityComparer<int>)null));
		AddBinding((IBinding)(object)(m_SimulationPausedBarrierBinding = new EventBinding<bool>("time", "simulationPausedBarrier", (IWriter<bool>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("time", "setSimulationPaused", (Action<bool>)SetSimulationPaused, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int>("time", "setSimulationSpeed", (Action<int>)SetSimulationSpeed, (IReader<int>)null));
		PlatformManager.instance.onAppStateChanged += new OnAppStateChanged(HandleAppStateChanged);
	}

	private void HandleAppStateChanged(IPlatformServiceIntegration psi, AppState state)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)state == 0)
		{
			m_HasFocus = true;
		}
		else if ((int)state == 1)
		{
			m_HasFocus = false;
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_SpeedBeforePause = 1f;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (m_SimulationSystem.selectedSpeed > 0f)
		{
			m_SpeedBeforePause = m_SimulationSystem.selectedSpeed;
		}
		if (!m_HasFocus || ((EventBindingBase)m_SimulationPausedBarrierBinding).observerCount > 0)
		{
			if (!IsPaused())
			{
				m_UnpausedBeforeForcedPause = true;
			}
			m_SimulationSystem.selectedSpeed = 0f;
		}
		else
		{
			if (m_UnpausedBeforeForcedPause)
			{
				m_SimulationSystem.selectedSpeed = m_SpeedBeforePause;
			}
			m_UnpausedBeforeForcedPause = false;
		}
	}

	private TimeSettings GetTimeSettings()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TimeSettingsData timeSettingsData = GetTimeSettingsData();
		TimeData singleton = TimeData.GetSingleton(m_TimeDataQuery);
		return new TimeSettings
		{
			ticksPerDay = 262144,
			daysPerYear = timeSettingsData.m_DaysPerYear,
			epochTicks = Mathf.RoundToInt(singleton.TimeOffset * 262144f) + Mathf.RoundToInt(singleton.GetDateOffset(timeSettingsData.m_DaysPerYear) * 262144f * (float)timeSettingsData.m_DaysPerYear),
			epochYear = singleton.m_StartingYear
		};
	}

	public int GetTicks()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		float num = 182.04445f;
		return Mathf.FloorToInt(Mathf.Floor((float)(m_SimulationSystem.frameIndex - TimeData.GetSingleton(m_TimeDataQuery).m_FirstFrame) / num) * num);
	}

	public int GetDay()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return TimeSystem.GetDay(m_SimulationSystem.frameIndex, TimeData.GetSingleton(m_TimeDataQuery));
	}

	public LightingSystem.State GetLightingState()
	{
		LightingSystem.State state = m_LightingSystem.state;
		if (state != LightingSystem.State.Invalid)
		{
			return state;
		}
		float normalizedTime = m_TimeSystem.normalizedTime;
		if (!(normalizedTime < 7f / 24f) && !(normalizedTime > 0.875f))
		{
			return LightingSystem.State.Day;
		}
		return LightingSystem.State.Night;
	}

	public bool IsPaused()
	{
		return m_SimulationSystem.selectedSpeed == 0f;
	}

	public int GetSimulationSpeed()
	{
		return SpeedToIndex(IsPaused() ? m_SpeedBeforePause : m_SimulationSystem.selectedSpeed);
	}

	private TimeSettingsData GetTimeSettingsData()
	{
		if (((EntityQuery)(ref m_TimeSettingsQuery)).IsEmptyIgnoreFilter)
		{
			return new TimeSettingsData
			{
				m_DaysPerYear = 12
			};
		}
		return ((EntityQuery)(ref m_TimeSettingsQuery)).GetSingleton<TimeSettingsData>();
	}

	private void SetSimulationPaused(bool paused)
	{
		if (!pausedBarrierActive)
		{
			m_SimulationSystem.selectedSpeed = (paused ? 0f : m_SpeedBeforePause);
		}
		else
		{
			m_UnpausedBeforeForcedPause = !paused;
		}
	}

	private void SetSimulationSpeed(int speedIndex)
	{
		if (!pausedBarrierActive)
		{
			m_SimulationSystem.selectedSpeed = IndexToSpeed(speedIndex);
			return;
		}
		m_SpeedBeforePause = IndexToSpeed(speedIndex);
		m_UnpausedBeforeForcedPause = true;
	}

	private static float IndexToSpeed(int index)
	{
		return Mathf.Pow(2f, (float)Mathf.Clamp(index, 0, 2));
	}

	private static int SpeedToIndex(float speed)
	{
		if (!(speed > 0f))
		{
			return 0;
		}
		return Mathf.Clamp((int)Mathf.Log(speed, 2f), 0, 2);
	}

	[Preserve]
	public TimeUISystem()
	{
	}
}
