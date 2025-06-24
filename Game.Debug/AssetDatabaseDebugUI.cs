using System;
using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.SceneFlow;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public class AssetDatabaseDebugUI : IDisposable
{
	public AssetDatabaseDebugUI()
	{
		AssetDatabase.global.onAssetDatabaseChanged.Subscribe((EventDelegate<AssetChangedEventArgs>)Rebuild, (Predicate<AssetChangedEventArgs>)delegate(AssetChangedEventArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			ChangeType change = ((AssetChangedEventArgs)(ref args)).change;
			return (int)change == 0 || (int)change == 1;
		});
	}

	public void Dispose()
	{
		AssetDatabase.global.onAssetDatabaseChanged.Unsubscribe((EventDelegate<AssetChangedEventArgs>)Rebuild);
	}

	private void Rebuild(AssetChangedEventArgs args = default(AssetChangedEventArgs))
	{
		DebugSystem.Rebuild(BuildAssetDatabaseDebugUI);
	}

	[DebugTab("Asset Database", -20)]
	private List<Widget> BuildAssetDatabaseDebugUI()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_01be: Expected O, but got Unknown
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_0265: Expected O, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Expected O, but got Unknown
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Expected O, but got Unknown
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Expected O, but got Unknown
		//IL_03da: Expected O, but got Unknown
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Expected O, but got Unknown
		Foldout val = new Foldout
		{
			displayName = "Asset Database Changed Handlers"
		};
		ICollection<(EventDelegate<AssetChangedEventArgs> handler, Predicate<AssetChangedEventArgs> filter)> globalChangedHandlers = AssetDatabase.global.onAssetDatabaseChanged.Handlers;
		((Container)val).children.Add((Widget)new Value
		{
			displayName = "Active Count",
			getter = () => globalChangedHandlers.Count
		});
		foreach (var item in globalChangedHandlers)
		{
			((Container)val).children.Add((Widget)new Container
			{
				displayName = ((Delegate)(object)item.handler).Method.DeclaringType.Name + "." + ((Delegate)(object)item.handler).Method.Name
			});
		}
		Container val2 = new Container();
		ObservableList<Widget> children = val2.children;
		Foldout val3 = new Foldout
		{
			displayName = "Global database"
		};
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Global mipbias",
			getter = () => AssetDatabase.global.mipBias
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Asset count",
			getter = () => AssetDatabase.global.count
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Hostname",
			getter = () => AssetDatabase.global.hostname
		});
		((Container)val3).children.Add((Widget)(object)val);
		children.Add((Widget)val3);
		Container val4 = val2;
		foreach (IAssetDatabase database in AssetDatabase.global.databases)
		{
			Foldout val5 = new Foldout
			{
				displayName = database.name
			};
			((Container)val5).children.Add((Widget)new Value
			{
				displayName = "Asset count",
				getter = () => database.count
			});
			((Container)val5).children.Add((Widget)new Value
			{
				displayName = "Hostname",
				getter = () => database.hostname
			});
			Foldout val6 = val5;
			Foldout val7 = new Foldout
			{
				displayName = "Asset Database Changed Handlers"
			};
			ICollection<(EventDelegate<AssetChangedEventArgs> handler, Predicate<AssetChangedEventArgs> filter)> changedHandlers = database.onAssetDatabaseChanged.Handlers;
			((Container)val7).children.Add((Widget)new Value
			{
				displayName = "Active Count",
				getter = () => changedHandlers.Count
			});
			foreach (var item2 in changedHandlers)
			{
				((Container)val7).children.Add((Widget)new Container
				{
					displayName = ((Delegate)(object)item2.handler).Method.DeclaringType.Name + "." + ((Delegate)(object)item2.handler).Method.Name
				});
			}
			val4.children.Add((Widget)(object)val6);
		}
		return new List<Widget>
		{
			(Widget)(object)val4,
			(Widget)new Button
			{
				displayName = "Apply settings",
				action = delegate
				{
					GameManager.instance.settings.Apply();
				}
			},
			(Widget)new Button
			{
				displayName = "Reset settings",
				action = delegate
				{
					GameManager.instance.settings.Reset();
				}
			},
			(Widget)new Button
			{
				displayName = "Refresh",
				action = delegate
				{
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					Rebuild();
				}
			}
		};
	}
}
