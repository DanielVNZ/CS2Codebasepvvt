using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;

namespace Game.Prefabs.Modes;

public abstract class ModePrefab : PrefabBase
{
	public class ModeDebugUILogInfo
	{
		public Type m_Key;

		public object m_ValueBefore;

		public object m_ValueAfter;
	}

	public Dictionary<Entity, List<ModeDebugUILogInfo>> modeDebugUILogs { get; private set; }

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.GetPrefabComponents(components);
		components.Add(ComponentType.ReadWrite<GameModeComponent>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	protected void RecordLog<T>(Entity entity, ref T value) where T : unmanaged
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (modeDebugUILogs == null)
		{
			Dictionary<Entity, List<ModeDebugUILogInfo>> dictionary = (modeDebugUILogs = new Dictionary<Entity, List<ModeDebugUILogInfo>>());
		}
		if (!modeDebugUILogs.ContainsKey(entity))
		{
			modeDebugUILogs.Add(entity, new List<ModeDebugUILogInfo>());
		}
		List<ModeDebugUILogInfo> list = modeDebugUILogs[entity];
		Type key = value.GetType();
		ModeDebugUILogInfo modeDebugUILogInfo = list.Find((ModeDebugUILogInfo x) => x.m_Key == key);
		if (modeDebugUILogInfo == null)
		{
			list.Add(new ModeDebugUILogInfo
			{
				m_Key = key,
				m_ValueBefore = value,
				m_ValueAfter = null
			});
		}
		else
		{
			modeDebugUILogInfo.m_ValueAfter = value;
		}
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	protected void RecordLog<T>(Entity entity, ref DynamicBuffer<T> value) where T : unmanaged
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (modeDebugUILogs == null)
		{
			Dictionary<Entity, List<ModeDebugUILogInfo>> dictionary = (modeDebugUILogs = new Dictionary<Entity, List<ModeDebugUILogInfo>>());
		}
		if (!modeDebugUILogs.ContainsKey(entity))
		{
			modeDebugUILogs.Add(entity, new List<ModeDebugUILogInfo>());
		}
		T[] array = new T[value.Length];
		for (int i = 0; i < value.Length; i++)
		{
			array[i] = value[i];
		}
		List<ModeDebugUILogInfo> list = modeDebugUILogs[entity];
		Type key = ((object)value).GetType();
		ModeDebugUILogInfo modeDebugUILogInfo = list.Find((ModeDebugUILogInfo x) => x.m_Key == key);
		if (modeDebugUILogInfo == null)
		{
			list.Add(new ModeDebugUILogInfo
			{
				m_Key = key,
				m_ValueBefore = array,
				m_ValueAfter = null
			});
		}
		else
		{
			modeDebugUILogInfo.m_ValueAfter = array;
		}
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	public void ClearLog()
	{
		modeDebugUILogs?.Clear();
	}
}
