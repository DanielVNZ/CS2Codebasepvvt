using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using cohtml.Net;
using Colossal;
using Colossal.UI;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public class UIDebugUI
{
	[DebugTab("UI", -965)]
	private static List<Widget> BuildUIBindingsDebugUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Expected O, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Expected O, but got Unknown
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Expected O, but got Unknown
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Expected O, but got Unknown
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Expected O, but got Unknown
		//IL_0536: Expected O, but got Unknown
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Expected O, but got Unknown
		//IL_0577: Expected O, but got Unknown
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Expected O, but got Unknown
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Expected O, but got Unknown
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Expected O, but got Unknown
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Expected O, but got Unknown
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Expected O, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Expected O, but got Unknown
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Expected O, but got Unknown
		Foldout val = new Foldout
		{
			displayName = "UI Manager"
		};
		foreach (UISystem uiSystem in UIManager.UISystems)
		{
			Foldout val2 = new Foldout
			{
				displayName = "UISystem #" + uiSystem.id
			};
			((Container)val).children.Add((Widget)new Value
			{
				displayName = "Static User Images",
				getter = () => uiSystem.userImagesManager.staticUserImageCount
			});
			((Container)val).children.Add((Widget)new Value
			{
				displayName = "Dynamic User Images",
				getter = () => uiSystem.userImagesManager.dynamicUserImageCount
			});
			((Container)val).children.Add((Widget)(object)val2);
			((Container)val).children.Add((Widget)new Button
			{
				displayName = "Clear cached unused images",
				action = delegate
				{
					uiSystem.ClearCachedUnusedImages();
				}
			});
			foreach (UIView uiView in uiSystem.UIViews)
			{
				Container val3 = new Container();
				val3.children.Add((Widget)new Value
				{
					displayName = "UIView #" + uiView.id,
					getter = () => string.Format("{0} #{1}", uiView.enabled ? "[x]" : "[ ]", uiView.id)
				});
				val3.children.Add((Widget)new Value
				{
					displayName = "URL",
					getter = () => uiView.url
				});
				((Container)val2).children.Add((Widget)(object)val3);
			}
			IResourceHandler resourceHandler = uiSystem.resourceHandler;
			DefaultResourceHandler val4 = (DefaultResourceHandler)(object)((resourceHandler is DefaultResourceHandler) ? resourceHandler : null);
			if (val4 != null && val4.HostLocationsMap.Count > 0)
			{
				Foldout val5 = new Foldout
				{
					displayName = "coui Hosts"
				};
				foreach (KeyValuePair<string, List<(string, int)>> item2 in val4.HostLocationsMap)
				{
					Foldout val6 = new Foldout
					{
						displayName = item2.Key
					};
					foreach (var path in item2.Value)
					{
						ObservableList<Widget> children = ((Container)val6).children;
						Value val7 = new Value();
						int item = path.Item2;
						((Widget)val7).displayName = item.ToString();
						val7.getter = () => path.Item1;
						children.Add((Widget)val7);
					}
					((Container)val5).children.Add((Widget)(object)val6);
				}
				((Container)val2).children.Add((Widget)(object)val5);
			}
			if (val4 == null || val4.DatabaseHostLocationsMap.Count <= 0)
			{
				continue;
			}
			Foldout val8 = new Foldout
			{
				displayName = "assetdb Hosts"
			};
			foreach (KeyValuePair<string, List<(Uri, int)>> item3 in val4.DatabaseHostLocationsMap)
			{
				Foldout val9 = new Foldout
				{
					displayName = item3.Key
				};
				foreach (var path2 in item3.Value)
				{
					ObservableList<Widget> children2 = ((Container)val9).children;
					Value val10 = new Value();
					int item = path2.Item2;
					((Widget)val10).displayName = item.ToString();
					val10.getter = () => path2.Item1;
					children2.Add((Widget)val10);
				}
				((Container)val8).children.Add((Widget)(object)val9);
			}
			((Container)val2).children.Add((Widget)(object)val8);
		}
		List<Widget> list = new List<Widget>
		{
			(Widget)new Button
			{
				displayName = "Refresh",
				action = delegate
				{
					DebugSystem.Rebuild(BuildUIBindingsDebugUI);
				}
			},
			(Widget)(object)val
		};
		if (UIManager.instance.enableMemoryTracking)
		{
			Foldout val11 = new Foldout
			{
				displayName = "UI Memory"
			};
			((Container)val11).children.Add((Widget)new Value
			{
				displayName = "Allocated memory",
				getter = () => FormatUtils.FormatBytes((long)UnityPlugin.Instance.GetAllocatedMemory())
			});
			((Container)val11).children.Add((Widget)new Value
			{
				displayName = "Time spent in allocations (ns)",
				getter = () => UnityPlugin.Instance.GetTimeSpentInAllocationsNs()
			});
			((Container)val11).children.Add((Widget)new Value
			{
				displayName = "Allocation count",
				getter = () => UnityPlugin.Instance.GetAllocationCount()
			});
			((Container)val11).children.Add((Widget)new Value
			{
				displayName = "Total allocations",
				getter = () => UnityPlugin.Instance.GetTotalAllocations()
			});
			Foldout val12 = new Foldout
			{
				displayName = "Mem tags"
			};
			MemTagsType[] array = (MemTagsType[])Enum.GetValues(typeof(MemTagsType));
			for (int num = 0; num < array.Length - 1; num++)
			{
				MemTagsType tag = array[num];
				Foldout val13 = new Foldout
				{
					displayName = ((object)System.Runtime.CompilerServices.Unsafe.As<MemTagsType, MemTagsType>(ref tag)/*cast due to .constrained prefix*/).ToString(),
					opened = true
				};
				((Container)val13).children.Add((Widget)new Value
				{
					displayName = "Allocated",
					getter = () => FormatUtils.FormatBytes((long)UnityPlugin.Instance.GetCurrentBytesByType(tag))
				});
				((Container)val13).children.Add((Widget)new Value
				{
					displayName = "Count",
					getter = () => UnityPlugin.Instance.GetCurrentCountsByType(tag)
				});
				((Container)val13).children.Add((Widget)new Value
				{
					displayName = "Totals",
					getter = () => UnityPlugin.Instance.GetTotalsByType(tag)
				});
				((Container)val12).children.Add((Widget)(object)val13);
			}
			((Container)val11).children.Add((Widget)(object)val12);
			list.Insert(2, (Widget)(object)val11);
		}
		return list;
	}
}
