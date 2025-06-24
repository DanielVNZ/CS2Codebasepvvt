using System;
using System.Collections.Generic;
using Game.SceneFlow;
using Game.UI;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Game.Debug;

[DebugContainer]
public class SceneFlowDebugUI
{
	[DebugTab("Scene Flow", -10)]
	private static List<Widget> BuildSceneFlowDebugUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Expected O, but got Unknown
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Expected O, but got Unknown
		Foldout val = new Foldout();
		((Widget)val).displayName = "Loaded Scenes + RootCount";
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			((Container)val).children.Add((Widget)new Value
			{
				displayName = ((Scene)(ref scene)).name,
				getter = () => ((Scene)(ref scene)).rootCount
			});
		}
		Foldout val2 = new Foldout
		{
			displayName = "Crash tests"
		};
		((Container)val2).children.Add((Widget)new Button
		{
			displayName = "Exception",
			action = delegate
			{
				throw new Exception("Test exception");
			}
		});
		((Container)val2).children.Add((Widget)new Button
		{
			displayName = ((object)(ForcedCrashCategory)0/*cast due to .constrained prefix*/).ToString(),
			action = delegate
			{
				Utils.ForceCrash((ForcedCrashCategory)0);
			}
		});
		((Container)val2).children.Add((Widget)new Button
		{
			displayName = ((object)(ForcedCrashCategory)2/*cast due to .constrained prefix*/).ToString(),
			action = delegate
			{
				Utils.ForceCrash((ForcedCrashCategory)2);
			}
		});
		((Container)val2).children.Add((Widget)new Button
		{
			displayName = ((object)(ForcedCrashCategory)1/*cast due to .constrained prefix*/).ToString(),
			action = delegate
			{
				Utils.ForceCrash((ForcedCrashCategory)1);
			}
		});
		((Container)val2).children.Add((Widget)new Button
		{
			displayName = ((object)(ForcedCrashCategory)4/*cast due to .constrained prefix*/).ToString(),
			action = delegate
			{
				Utils.ForceCrash((ForcedCrashCategory)4);
			}
		});
		((Container)val2).children.Add((Widget)new Button
		{
			displayName = ((object)(ForcedCrashCategory)3/*cast due to .constrained prefix*/).ToString(),
			action = delegate
			{
				Utils.ForceCrash((ForcedCrashCategory)3);
			}
		});
		return new List<Widget>
		{
			(Widget)new Value
			{
				displayName = "Mode",
				getter = () => GameManager.instance.gameMode
			},
			(Widget)(object)val2,
			(Widget)(object)val,
			(Widget)new Button
			{
				displayName = "Refresh",
				action = delegate
				{
					DebugSystem.Rebuild(BuildSceneFlowDebugUI);
				}
			},
			(Widget)new Button
			{
				displayName = "Dismiss all errors",
				action = delegate
				{
					ErrorDialogManager.DismissAllErrors();
				}
			}
		};
	}
}
