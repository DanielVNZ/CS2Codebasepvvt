using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal;
using Unity.Profiling;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public class ProfilerMetricsDebugUI : IDisposable
{
	private struct StatInfo
	{
		public string categoryName;

		public ProfilerCategory category;

		public string name;

		public ProfilerRecorder profilerRecorder;

		public StatInfo(string categoryName, ProfilerCategory category, string name, int sampleCount = 0)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			this.categoryName = categoryName;
			this.category = category;
			this.name = name;
			profilerRecorder = ((sampleCount > 0) ? ProfilerRecorder.StartNew(category, name, sampleCount, (ProfilerRecorderOptions)24) : ProfilerRecorder.StartNew(category, name, 1, (ProfilerRecorderOptions)24));
		}
	}

	private List<StatInfo> m_AvailableStats = new List<StatInfo>();

	public ProfilerMetricsDebugUI()
	{
		CollectProfilerMetrics();
	}

	public void Dispose()
	{
		DisposeProfilerMetrics();
	}

	private unsafe static double GetRecorderFrameAverage(ProfilerRecorder recorder)
	{
		int capacity = ((ProfilerRecorder)(ref recorder)).Capacity;
		if (capacity == 0)
		{
			return 0.0;
		}
		double num = 0.0;
		ProfilerRecorderSample* ptr = (ProfilerRecorderSample*)stackalloc ProfilerRecorderSample[capacity];
		((ProfilerRecorder)(ref recorder)).CopyTo(ptr, capacity, false);
		for (int i = 0; i < capacity; i++)
		{
			num += (double)((ProfilerRecorderSample)((byte*)ptr + (nint)i * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<ProfilerRecorderSample>())).Value;
		}
		return num / (double)capacity;
	}

	private void CollectProfilerMetrics()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Total Used Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Total Reserved Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "GC Used Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "GC Reserved Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Gfx Used Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Gfx Reserved Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Audio Used Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Audio Reserved Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Video Used Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Video Reserved Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Profiler Used Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "Profiler Reserved Memory"));
		m_AvailableStats.Add(new StatInfo("Memory", ProfilerCategory.Memory, "System Used Memory"));
	}

	private void DisposeProfilerMetrics()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		foreach (StatInfo availableStat in m_AvailableStats)
		{
			ProfilerRecorder profilerRecorder = availableStat.profilerRecorder;
			((ProfilerRecorder)(ref profilerRecorder)).Dispose();
		}
		m_AvailableStats.Clear();
	}

	[DebugTab("Profiler Metrics", 0)]
	private List<Widget> BuildProfilerMetricsDebugUI()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected I4, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		List<Widget> list = new List<Widget>();
		Dictionary<string, Foldout> dictionary = new Dictionary<string, Foldout>();
		foreach (StatInfo stat in m_AvailableStats)
		{
			if (!dictionary.TryGetValue(stat.categoryName, out var value))
			{
				value = new Foldout
				{
					displayName = stat.categoryName
				};
				list.Add((Widget)(object)value);
				dictionary.Add(stat.categoryName, value);
			}
			ProfilerRecorder profilerRecorder = stat.profilerRecorder;
			ProfilerMarkerDataUnit unitType = ((ProfilerRecorder)(ref profilerRecorder)).UnitType;
			switch (unitType - 1)
			{
			case 1:
				((Container)value).children.Add((Widget)new Value
				{
					displayName = stat.name,
					getter = delegate
					{
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						ProfilerRecorder profilerRecorder2 = stat.profilerRecorder;
						return FormatUtils.FormatBytes(((ProfilerRecorder)(ref profilerRecorder2)).LastValue);
					}
				});
				break;
			case 0:
				((Container)value).children.Add((Widget)new Value
				{
					displayName = stat.name,
					getter = () => $"{GetRecorderFrameAverage(stat.profilerRecorder) * 9.999999974752427E-07:F2}ms"
				});
				break;
			case 2:
				((Container)value).children.Add((Widget)new Value
				{
					displayName = stat.name,
					getter = delegate
					{
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						ProfilerRecorder profilerRecorder2 = stat.profilerRecorder;
						return ((ProfilerRecorder)(ref profilerRecorder2)).Count;
					}
				});
				break;
			case 4:
				((Container)value).children.Add((Widget)new Value
				{
					displayName = stat.name,
					getter = delegate
					{
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0010: Unknown result type (might be due to invalid IL or missing references)
						ProfilerRecorder profilerRecorder2 = stat.profilerRecorder;
						return $"{((ProfilerRecorder)(ref profilerRecorder2)).LastValue}Hz";
					}
				});
				break;
			case 3:
				((Container)value).children.Add((Widget)new Value
				{
					displayName = stat.name,
					getter = delegate
					{
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0010: Unknown result type (might be due to invalid IL or missing references)
						ProfilerRecorder profilerRecorder2 = stat.profilerRecorder;
						return $"{((ProfilerRecorder)(ref profilerRecorder2)).LastValue}%";
					}
				});
				break;
			}
		}
		return list;
	}
}
