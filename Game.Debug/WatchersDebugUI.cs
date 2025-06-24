using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public class WatchersDebugUI : IDisposable
{
	private DebugWatchSystem m_WatchSystem;

	private void Rebuild()
	{
		DebugSystem.Rebuild(BuildWatchersDebugUI);
	}

	public void Dispose()
	{
		((ComponentSystemBase)m_WatchSystem).Enabled = m_WatchSystem.watches.Count > 0;
	}

	[DebugTab("Watchers", -975)]
	private List<Widget> BuildWatchersDebugUI(World world)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		m_WatchSystem = world.GetOrCreateSystemManaged<DebugWatchSystem>();
		((ComponentSystemBase)m_WatchSystem).Enabled = true;
		List<Widget> list = new List<Widget>();
		list.Add((Widget)new Button
		{
			displayName = "Refresh System List",
			action = Rebuild
		});
		list.Add((Widget)new Button
		{
			displayName = "Clear Watches",
			action = m_WatchSystem.ClearWatches
		});
		list.AddRange(m_WatchSystem.BuildSystemFoldouts());
		return list;
	}
}
