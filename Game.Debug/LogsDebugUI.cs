using System.Collections.Generic;
using System.Linq;
using Colossal.Logging;
using Game.SceneFlow;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public static class LogsDebugUI
{
	private static void Rebuild()
	{
		DebugSystem.Rebuild(BuildLogsDebugUI);
	}

	[DebugTab("Logs", 0)]
	private static List<Widget> BuildLogsDebugUI()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Expected O, but got Unknown
		//IL_011d: Expected O, but got Unknown
		//IL_0122: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0169: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_01a9: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		//IL_01fa: Expected O, but got Unknown
		//IL_0293: Expected O, but got Unknown
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Expected O, but got Unknown
		//IL_02d2: Expected O, but got Unknown
		Container val = new Container();
		List<Level> levels = Level.GetLevels().ToList();
		foreach (ILog log in LogManager.GetAllLoggers())
		{
			ObservableList<Widget> children = val.children;
			EnumField val2 = new EnumField
			{
				displayName = log.name
			};
			((Field<int>)val2).getter = () => log.effectivenessLevel.severity;
			((Field<int>)val2).setter = delegate(int value)
			{
				log.effectivenessLevel = Level.GetLevel(value);
			};
			val2.getIndex = () => levels.FindIndex((Level level) => level == log.effectivenessLevel);
			val2.setIndex = delegate(int index)
			{
				log.effectivenessLevel = levels[index = (index + levels.Count) % levels.Count];
			};
			((EnumField<int>)val2).enumNames = levels.Select((Level level) => ToTitleCase(level.name)).ToArray();
			((EnumField<int>)val2).enumValues = levels.Select((Level level) => level.severity).ToArray();
			children.Add((Widget)val2);
			Container val3 = new Container();
			ObservableList<Widget> children2 = val3.children;
			BoolField val4 = new BoolField
			{
				displayName = "Show errors in UI"
			};
			((Field<bool>)val4).getter = () => log.showsErrorsInUI;
			((Field<bool>)val4).setter = delegate(bool v)
			{
				log.showsErrorsInUI = v;
			};
			children2.Add((Widget)val4);
			ObservableList<Widget> children3 = val3.children;
			BoolField val5 = new BoolField
			{
				displayName = "Log stack trace"
			};
			((Field<bool>)val5).getter = () => log.logStackTrace;
			((Field<bool>)val5).setter = delegate(bool v)
			{
				log.logStackTrace = v;
			};
			children3.Add((Widget)val5);
			if (GameManager.instance.configuration.qaDeveloperMode)
			{
				ObservableList<Widget> children4 = val3.children;
				BoolField val6 = new BoolField
				{
					displayName = "Disable backtrace"
				};
				((Field<bool>)val6).getter = () => log.disableBacktrace;
				((Field<bool>)val6).setter = delegate(bool v)
				{
					log.disableBacktrace = v;
				};
				children4.Add((Widget)val6);
			}
			ObservableList<Widget> children5 = val3.children;
			EnumField val7 = new EnumField
			{
				displayName = "Show stack trace below levels"
			};
			((Field<int>)val7).getter = () => log.showsStackTraceAboveLevels.severity;
			((Field<int>)val7).setter = delegate(int value)
			{
				log.showsStackTraceAboveLevels = Level.GetLevel(value);
			};
			val7.getIndex = () => levels.FindIndex((Level level) => level == log.showsStackTraceAboveLevels);
			val7.setIndex = delegate(int index)
			{
				log.showsStackTraceAboveLevels = levels[index = (index + levels.Count) % levels.Count];
			};
			((EnumField<int>)val7).enumNames = levels.Select((Level level) => ToTitleCase(level.name)).ToArray();
			((EnumField<int>)val7).enumValues = levels.Select((Level level) => level.severity).ToArray();
			children5.Add((Widget)val7);
			val.children.Add((Widget)(object)val3);
		}
		return new List<Widget>
		{
			(Widget)new Button
			{
				displayName = "Refresh",
				action = Rebuild
			},
			(Widget)(object)val
		};
		static GUIContent ToTitleCase(string input)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			return new GUIContent(char.ToUpper(input[0]) + input.Substring(1).ToLower());
		}
	}
}
