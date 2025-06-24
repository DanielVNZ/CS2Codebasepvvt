using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Colossal.Localization;
using Colossal.PSI.Environment;
using Game.Input;
using Game.SceneFlow;
using Game.UI.Editor;
using Game.UI.Localization;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Debug;

[DebugContainer]
public class LocalizationDebugUI : IDisposable
{
	private static readonly GUIContent[] kLocalizationDebugModeStrings = (GUIContent[])(object)new GUIContent[3]
	{
		new GUIContent("Show Translations"),
		new GUIContent("Show IDs"),
		new GUIContent("Show Fallback")
	};

	private int m_SelectedContentId;

	public LocalizationDebugUI()
	{
		InitLocalization(m_SelectedContentId);
	}

	private void InitLocalization(int contentId)
	{
	}

	public void Dispose()
	{
	}

	private void Rebuild()
	{
		DebugSystem.Rebuild(BuildLocalizationDebugUI);
	}

	[DebugTab("Localization", -5)]
	private List<Widget> BuildLocalizationDebugUI(World world)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Expected O, but got Unknown
		LocalizationManager manager = GameManager.instance.localizationManager;
		if (manager == null)
		{
			return null;
		}
		string[] locales = manager.GetSupportedLocales();
		GUIContent[] array = (GUIContent[])(object)new GUIContent[locales.Length];
		int[] array2 = new int[locales.Length];
		for (int i = 0; i < locales.Length; i++)
		{
			array[i] = new GUIContent(locales[i]);
			array2[i] = i;
		}
		List<Widget> list = new List<Widget>();
		EnumField val = new EnumField
		{
			displayName = "Language"
		};
		((Field<int>)val).getter = () => Array.IndexOf(locales, manager.activeDictionary.localeID);
		((Field<int>)val).setter = delegate(int value)
		{
			manager.SetActiveLocale(locales[value]);
		};
		((EnumField<int>)val).enumNames = array;
		((EnumField<int>)val).enumValues = array2;
		val.getIndex = () => Array.IndexOf(locales, manager.activeDictionary.localeID);
		val.setIndex = delegate
		{
		};
		list.Add((Widget)val);
		EnumField val2 = new EnumField
		{
			displayName = "Debug Mode"
		};
		((Field<int>)val2).getter = () => (int)GameManager.instance.userInterface.localizationBindings.debugMode;
		((Field<int>)val2).setter = delegate(int value)
		{
			GameManager.instance.userInterface.localizationBindings.debugMode = (LocalizationBindings.DebugMode)value;
		};
		((EnumField<int>)val2).enumNames = kLocalizationDebugModeStrings;
		val2.autoEnum = typeof(LocalizationBindings.DebugMode);
		val2.getIndex = () => (int)GameManager.instance.userInterface.localizationBindings.debugMode;
		val2.setIndex = delegate
		{
		};
		list.Add((Widget)val2);
		list.Add((Widget)new Button
		{
			displayName = "Print input bindings and controls",
			action = delegate
			{
				List<string> list2 = new List<string>();
				List<string> list3 = new List<string>();
				foreach (ProxyBinding binding in InputManager.instance.GetBindings(InputManager.PathType.Effective, InputManager.BindingOptions.OnlyRebindable))
				{
					if (!list2.Contains(binding.title))
					{
						list2.Add(binding.title);
					}
					foreach (string item2 in binding.ToHumanReadablePath())
					{
						string item = binding.device.ToString() + "." + item2;
						if (!list3.Contains(item))
						{
							list3.Add(item);
						}
					}
				}
				Debug.Log((object)string.Join("\n", list2.Select(delegate(string b)
				{
					string text = b.Substring(b.IndexOf("/", StringComparison.InvariantCulture) + 1).Replace("/binding", "");
					return "Options.OPTION[" + b + "]\t" + text + "\nOptions.OPTION_DESCRIPTION[" + b + "]\tTBD";
				})));
				list3.Sort();
				Debug.Log((object)string.Join("\n", list3.Select((string p) => "Options.INPUT_CONTROL[" + p + "]\t" + p.Substring(p.IndexOf(".", StringComparison.InvariantCulture) + 1))));
			}
		});
		list.Add((Widget)new Button
		{
			displayName = "Print asset categories",
			action = delegate
			{
				EditorAssetCategorySystem orCreateSystemManaged = world.GetOrCreateSystemManaged<EditorAssetCategorySystem>();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (EditorAssetCategory category in orCreateSystemManaged.GetCategories())
				{
					stringBuilder.AppendLine(category.GetLocalizationID() + "," + category.id);
				}
				string path = Path.Combine(EnvPath.kUserDataPath, "category_locale.csv");
				using FileStream stream = (File.Exists(path) ? File.OpenWrite(path) : File.Create(path));
				using StreamWriter streamWriter = new StreamWriter(stream);
				streamWriter.Write(stringBuilder);
			}
		});
		return list;
	}
}
