using System.Collections.Generic;
using System.Threading;
using Colossal.TestFramework;
using Game.SceneFlow;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public static class TestsDebugUI
{
	[DebugTab("Test Scenarios", -21)]
	private unsafe static List<Widget> BuildTestScenariosDebugUI()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.instance.configuration.qaDeveloperMode)
		{
			return null;
		}
		List<Widget> list = new List<Widget>();
		TestScenarioSystem tss = TestScenarioSystem.instance;
		Dictionary<Category, Foldout> dictionary = new Dictionary<Category, Foldout>();
		foreach (KeyValuePair<string, Scenario> scenario in tss.scenarios)
		{
			if (!dictionary.TryGetValue(scenario.Value.category, out var value))
			{
				Foldout val = new Foldout();
				Scenario value2 = scenario.Value;
				((Widget)val).displayName = ((object)(*(Category*)(&value2.category))/*cast due to .constrained prefix*/).ToString();
				value = val;
				dictionary.Add(scenario.Value.category, value);
				list.Add((Widget)(object)value);
			}
			((Container)value).children.Add((Widget)new Button
			{
				displayName = scenario.Key,
				action = delegate
				{
					tss.RunScenario(scenario.Key, CancellationToken.None);
				}
			});
		}
		return list;
	}
}
