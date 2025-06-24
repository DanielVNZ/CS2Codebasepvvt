using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.PSI.Common;
using Colossal.UI.Binding;
using Game.Input;
using Game.Modding.Toolchain;
using Game.PSI;
using Game.SceneFlow;
using Game.Settings;
using Game.UI.Editor;
using Game.UI.InGame;
using Game.UI.Localization;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace Game.UI.Menu;

[CompilerGenerated]
public class OptionsUISystem : UISystemBase
{
	[DebuggerDisplay("{id}")]
	public class Page : IJsonWritable
	{
		internal bool builtIn { get; set; }

		public bool warning { get; private set; }

		public string id { get; set; }

		public int index { get; set; }

		public bool beta { get; set; }

		public Func<bool> warningGetter { get; set; }

		[NotNull]
		public List<Section> sections { get; } = new List<Section>();

		public ReadOnlyCollection<Section> visibleSections => new ReadOnlyCollection<Section>(sections.Where((Section s) => s.isVisible).ToArray());

		public bool UpdateVisibility(bool isAdvanced)
		{
			bool flag = false;
			foreach (Section section in sections)
			{
				flag |= section.UpdateVisibility(isAdvanced);
			}
			return flag;
		}

		public bool UpdateNameAndDescription(bool isAdvanced)
		{
			bool flag = false;
			foreach (Section section in sections)
			{
				if (section.isVisible)
				{
					flag |= section.UpdateNameAndDescription(isAdvanced);
				}
			}
			return flag;
		}

		public bool UpdateWarning(bool isAdvanced)
		{
			bool flag = false;
			bool flag2 = false;
			if (warningGetter != null)
			{
				flag2 = warningGetter();
			}
			foreach (Section section in sections)
			{
				if (section.isVisible)
				{
					flag |= section.UpdateWarning(isAdvanced);
					flag2 |= section.warning;
				}
			}
			if (flag2 != warning)
			{
				warning = flag2;
				flag = true;
			}
			return flag;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("sections");
			JsonWriterExtensions.Write<Section>(writer, (IList<Section>)sections);
			writer.PropertyName("beta");
			writer.Write(beta);
			writer.PropertyName("warning");
			writer.Write(warning);
			writer.PropertyName("builtIn");
			writer.Write(builtIn);
			writer.TypeEnd();
		}
	}

	[DebuggerDisplay("{id}")]
	public class Section : IJsonWritable
	{
		public Page page { get; }

		public string id { get; set; }

		public int index { get; set; }

		public bool warning { get; private set; }

		public Func<bool> warningGetter { get; set; }

		public string[] groupNames { get; }

		public HashSet<string> groupToShowName { get; }

		private Dictionary<int, bool> groupVisible { get; set; } = new Dictionary<int, bool>();

		private Dictionary<int, bool> prevGroupVisible { get; set; } = new Dictionary<int, bool>();

		[NotNull]
		public List<Option> options { get; } = new List<Option>();

		public bool isVisible { get; private set; }

		public Section(string id, Page page, AutomaticSettings.SettingPageData pageData)
		{
			this.page = page;
			this.id = id;
			groupNames = pageData.groupNames.ToArray();
			groupToShowName = new HashSet<string>(pageData.groupToShowName);
		}

		public List<IWidget> GetItems(bool isAdvanced)
		{
			List<IWidget> list = new List<IWidget>();
			int num = -1;
			foreach (Option item in isAdvanced ? options.OrderBy((Option o) => o.advancedGroupIndex) : (from o in options
				where !o.isAdvanced
				orderby o.simpleGroupIndex
				select o))
			{
				int groupIndex = item.GetGroupIndex(isAdvanced);
				if (!groupVisible.TryGetValue(groupIndex, out var value) || !value)
				{
					continue;
				}
				if (num != groupIndex)
				{
					Breadcrumbs breadcrumbs = new Breadcrumbs();
					if (groupNames != null && groupIndex < groupNames.Length)
					{
						string text = groupNames[groupIndex];
						if (groupToShowName.Contains(text))
						{
							Label label = BuildGroupLabel(page.id, text);
							breadcrumbs.WithLabel(label);
						}
						list.Add(breadcrumbs);
					}
				}
				list.Add(item.widget);
				num = groupIndex;
			}
			return list;
		}

		public bool UpdateVisibility(bool isAdvanced)
		{
			bool flag = false;
			Dictionary<int, bool> dictionary = groupVisible;
			Dictionary<int, bool> dictionary2 = prevGroupVisible;
			Dictionary<int, bool> dictionary3 = (prevGroupVisible = dictionary);
			dictionary3 = (groupVisible = dictionary2);
			groupVisible.Clear();
			foreach (Option option in options)
			{
				if (option.widget is Widget widget)
				{
					widget.UpdateVisibility();
					option.isVisible = widget.isVisible;
					if (option.isVisible && (isAdvanced || !option.isAdvanced))
					{
						flag = true;
						int groupIndex = option.GetGroupIndex(isAdvanced);
						groupVisible[groupIndex] = true;
					}
				}
			}
			if (isVisible != flag)
			{
				isVisible = flag;
				return true;
			}
			int num = default(int);
			bool flag2 = default(bool);
			foreach (KeyValuePair<int, bool> item in groupVisible)
			{
				item.Deconstruct(ref num, ref flag2);
				int key = num;
				bool flag3 = flag2;
				if (!prevGroupVisible.TryGetValue(key, out var value) || value != flag3)
				{
					return true;
				}
			}
			bool flag4 = default(bool);
			foreach (KeyValuePair<int, bool> item2 in prevGroupVisible)
			{
				item2.Deconstruct(ref num, ref flag4);
				int key2 = num;
				bool flag5 = flag4;
				if (!groupVisible.TryGetValue(key2, out var value2) || value2 != flag5)
				{
					return true;
				}
			}
			return false;
		}

		public bool UpdateNameAndDescription(bool isAdvanced)
		{
			WidgetChanges widgetChanges = WidgetChanges.None;
			foreach (NamedWidget item in (from o in options
				where isAdvanced || !o.isAdvanced
				select o.widget).OfType<NamedWidget>())
			{
				if (item.isVisible)
				{
					widgetChanges |= item.UpdateNameAndDescription();
				}
			}
			return (widgetChanges & WidgetChanges.Properties) != 0;
		}

		public bool UpdateWarning(bool isAdvanced)
		{
			bool result = false;
			if (warningGetter != null)
			{
				bool flag = warningGetter();
				if (flag != warning)
				{
					warning = flag;
					result = true;
				}
			}
			if (warningGetter == null || !warning)
			{
				bool flag2 = false;
				foreach (IWarning item in (from o in options
					where isAdvanced || !o.isAdvanced
					select o.widget).OfType<IWarning>())
				{
					if (!(item is IVisibleWidget { isVisible: false }) && item.warning)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2 != warning)
				{
					warning = flag2;
					result = true;
				}
			}
			return result;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("isVisible");
			writer.Write(isVisible);
			writer.PropertyName("warning");
			writer.Write(warning);
			List<WidgetInfo> list = new List<WidgetInfo>();
			for (int i = 0; i < options.Count; i++)
			{
				Option option = options[i];
				if (option.widget is INamed named)
				{
					list.Add(new WidgetInfo
					{
						id = WidgetInfo.GetId(page.index, index, i),
						displayName = named.displayName,
						isVisible = option.isVisible,
						isAdvanced = option.isAdvanced,
						searchHidden = option.searchHidden
					});
				}
				else if (option.widget is ButtonRow { children: var children })
				{
					foreach (Button button in children)
					{
						list.Add(new WidgetInfo
						{
							id = WidgetInfo.GetId(page.index, index, i),
							displayName = button.displayName,
							isVisible = option.isVisible,
							isAdvanced = option.isAdvanced,
							searchHidden = option.searchHidden
						});
					}
				}
			}
			writer.PropertyName("items");
			JsonWriterExtensions.Write<WidgetInfo>(writer, (IList<WidgetInfo>)list);
			writer.TypeEnd();
		}
	}

	[DebuggerDisplay("{widget}, isAdvanced={isAdvanced}")]
	public class Option
	{
		public IWidget widget { get; set; }

		public bool isVisible { get; set; }

		public bool isAdvanced { get; set; }

		public int simpleGroupIndex { get; set; }

		public int advancedGroupIndex { get; set; }

		public bool searchHidden { get; set; }

		public int GetGroupIndex(bool isAdvancedIndex)
		{
			if (!isAdvancedIndex)
			{
				return simpleGroupIndex;
			}
			return advancedGroupIndex;
		}
	}

	public struct WidgetInfo : IJsonWritable
	{
		public int id { get; set; }

		public LocalizedString displayName { get; set; }

		public bool isVisible { get; set; }

		public bool isAdvanced { get; set; }

		public bool searchHidden { get; set; }

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("id");
			writer.Write(id);
			writer.PropertyName("displayName");
			JsonWriterExtensions.Write<LocalizedString>(writer, displayName);
			writer.PropertyName("isVisible");
			writer.Write(isVisible);
			writer.PropertyName("isAdvanced");
			writer.Write(isAdvanced);
			writer.PropertyName("searchHidden");
			writer.Write(searchHidden);
			writer.TypeEnd();
		}

		public static int GetId(int page, int section, int widget)
		{
			return ((page & 0xFFFF) << 16) + ((section & 0xFF) << 8) + (widget & 0xFF);
		}

		public static void GetIndices(int id, out int page, out int section, out int widget)
		{
			page = (id >> 16) & 0xFFFF;
			section = (id >> 8) & 0xFF;
			widget = id & 0xFF;
		}
	}

	public struct UnitSettings : IJsonWritable, IEquatable<UnitSettings>
	{
		public InterfaceSettings.TimeFormat timeFormat;

		public InterfaceSettings.TemperatureUnit temperatureUnit;

		public InterfaceSettings.UnitSystem unitSystem;

		public UnitSettings(InterfaceSettings settings)
		{
			timeFormat = settings.timeFormat;
			temperatureUnit = settings.temperatureUnit;
			unitSystem = settings.unitSystem;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("timeFormat");
			writer.Write((int)timeFormat);
			writer.PropertyName("temperatureUnit");
			writer.Write((int)temperatureUnit);
			writer.PropertyName("unitSystem");
			writer.Write((int)unitSystem);
			writer.TypeEnd();
		}

		public bool Equals(UnitSettings other)
		{
			if (timeFormat == other.timeFormat && temperatureUnit == other.temperatureUnit)
			{
				return unitSystem == other.unitSystem;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is UnitSettings other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, int, int>((int)timeFormat, (int)temperatureUnit, (int)unitSystem);
		}
	}

	private const string kGroup = "options";

	private GameScreenUISystem m_GameScreenUISystem;

	private EditorScreenUISystem m_EditorScreenUISystem;

	private MenuUISystem m_MenuUISystem;

	private bool m_IsAdvanced;

	private string m_SearchQuery;

	private List<int> m_SearchIds = new List<int>();

	private string m_LastLayout;

	private ValueBinding<string> m_ActivePageBinding;

	private ValueBinding<string> m_ActiveSectionBinding;

	private GetterValueBinding<List<Page>> m_PagesBinding;

	private WidgetBindings m_WidgetBindings;

	private ValueBinding<Dictionary<string, ControlPath>> m_LayoutMapBinding;

	private ValueBinding<bool> m_DirectoryBrowserActive;

	private WidgetBindings m_DirectoryBrowserBinding;

	private DirectoryBrowserPanel m_DirectoryBrowser;

	private DirectoryBrowserPanel m_LastDirectoryBrowser;

	private float m_DisplayConfirmationTime;

	private DisplayMode m_LastDisplayMode;

	private ScreenResolution m_LastResolution;

	private int m_LastDisplayIndex;

	private Dictionary<string, Page> pages { get; } = new Dictionary<string, Page>();

	private List<Page> sortedPages => (from p in pages.Values
		orderby p.builtIn descending, p.index
		select p).ToList();

	public static void UpdateNotificationState(ModdingToolStatus toolStatus, DeploymentState deploymentState)
	{
		if (toolStatus != ModdingToolStatus.Idle)
		{
			NotificationSystem.Pop("ToolchainStatus");
		}
		else if (deploymentState == DeploymentState.Outdated || deploymentState == DeploymentState.Invalid)
		{
			string textId = "ToolchainStatus" + deploymentState;
			ProgressState? progressState = (ProgressState)5;
			NotificationSystem.Push("ToolchainStatus", null, null, "ActionRequired", textId, null, progressState, null, delegate
			{
				ToolchainDeployment.RunWithUI(DeploymentAction.Install);
			});
		}
	}

	private async void QueryToolchainState()
	{
		try
		{
			UpdateNotificationState(ModdingToolStatus.Idle, await ToolchainDeployment.dependencyManager.GetCurrentState());
		}
		catch (Exception ex)
		{
			UISystemBase.log.Error(ex, (object)"Toolchain state query failed");
		}
	}

	private void OnModdingToolchainStateChanged(ToolchainDependencyManager.State newState)
	{
		GameManager.instance.RunOnMainThread(delegate
		{
			UpdateNotificationState(newState.m_Status, newState.m_State);
		});
	}

	public void OpenDirectoryBrowser(string root, Action<string> onSelect)
	{
		if (m_DirectoryBrowser == null)
		{
			m_DirectoryBrowser = new DirectoryBrowserPanel(root, null, delegate(string directory)
			{
				onSelect?.Invoke(directory);
				OnCancelDirectory();
			}, OnCancelDirectory);
		}
	}

	private void OnCancelDirectory()
	{
		if (m_DirectoryBrowser != null)
		{
			m_DirectoryBrowser = null;
		}
	}

	private void SwitchBindings(bool browserActive)
	{
		foreach (IBinding binding in ((CompositeBinding)m_WidgetBindings).bindings)
		{
			RawTriggerBindingBase val = (RawTriggerBindingBase)(object)((binding is RawTriggerBindingBase) ? binding : null);
			if (val != null)
			{
				val.active = !browserActive;
			}
		}
		foreach (IBinding binding2 in ((CompositeBinding)m_DirectoryBrowserBinding).bindings)
		{
			RawTriggerBindingBase val2 = (RawTriggerBindingBase)(object)((binding2 is RawTriggerBindingBase) ? binding2 : null);
			if (val2 != null)
			{
				val2.active = browserActive;
			}
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Expected O, but got Unknown
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Expected O, but got Unknown
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Expected O, but got Unknown
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Expected O, but got Unknown
		base.OnCreate();
		AddUpdateBinding((IUpdateBinding)(object)(m_WidgetBindings = new WidgetBindings("options")));
		m_WidgetBindings.AddDefaultBindings();
		m_WidgetBindings.AddBindings<InputBindingField.Bindings>();
		AddBinding((IBinding)(object)(m_ActivePageBinding = new ValueBinding<string>("options", "activePage", string.Empty, (IWriter<string>)null, (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)(m_ActiveSectionBinding = new ValueBinding<string>("options", "activeSection", string.Empty, (IWriter<string>)null, (EqualityComparer<string>)null)));
		AddBinding((IBinding)(object)(m_PagesBinding = new GetterValueBinding<List<Page>>("options", "pages", (Func<List<Page>>)(() => sortedPages), (IWriter<List<Page>>)(object)new ListWriter<Page>((IWriter<Page>)(object)new ValueWriter<Page>()), (EqualityComparer<List<Page>>)null)));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("options", "displayConfirmationVisible", (Func<bool>)(() => m_DisplayConfirmationTime > 0f), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("options", "displayConfirmationTime", (Func<int>)(() => Mathf.Max(Mathf.CeilToInt(m_DisplayConfirmationTime), 0)), (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<string>("options", "interfaceStyle", (Func<string>)(() => SharedSettings.instance.userInterface.interfaceStyle), (IWriter<string>)null, (EqualityComparer<string>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("options", "interfaceTransparency", (Func<float>)(() => SharedSettings.instance.userInterface.interfaceTransparency), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("options", "interfaceScaling", (Func<bool>)(() => SharedSettings.instance.userInterface.interfaceScaling), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<float>("options", "textScale", (Func<float>)(() => SharedSettings.instance.userInterface.textScale), (IWriter<float>)null, (EqualityComparer<float>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<string>("options", "interfaceStyle", (Func<string>)(() => SharedSettings.instance.userInterface.interfaceStyle), (IWriter<string>)null, (EqualityComparer<string>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("options", "unlockHighlightsEnabled", (Func<bool>)(() => SharedSettings.instance.userInterface.unlockHighlightsEnabled), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("options", "chirperPopupsEnabled", (Func<bool>)(() => SharedSettings.instance.userInterface.chirperPopupsEnabled), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("options", "inputHintsType", (Func<int>)(() => (int)SharedSettings.instance.userInterface.GetFinalInputHintsType()), (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("options", "keyboardLayout", (Func<int>)(() => (int)SharedSettings.instance.userInterface.keyboardLayout), (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("options", "shortcutHints", (Func<bool>)(() => SharedSettings.instance.userInterface.shortcutHints), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddBinding((IBinding)(object)(m_LayoutMapBinding = new ValueBinding<Dictionary<string, ControlPath>>("options", "layoutMap", new Dictionary<string, ControlPath>(), (IWriter<Dictionary<string, ControlPath>>)(object)new DictionaryWriter<string, ControlPath>((IWriter<string>)null, (IWriter<ControlPath>)(object)new ValueWriter<ControlPath>()), (EqualityComparer<Dictionary<string, ControlPath>>)null)));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<UnitSettings>("options", "unitSettings", (Func<UnitSettings>)(() => new UnitSettings(SharedSettings.instance.userInterface)), (IWriter<UnitSettings>)(object)new ValueWriter<UnitSettings>(), (EqualityComparer<UnitSettings>)null));
		AddBinding((IBinding)new TriggerBinding("options", "confirmDisplay", (Action)ConfirmDisplay));
		AddBinding((IBinding)new TriggerBinding("options", "revertDisplay", (Action)RevertDisplay));
		AddBinding((IBinding)(object)new TriggerBinding<string>("options", "onOptionsPageClosed", (Action<string>)OnPageClosed, (IReader<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string, string, bool>("options", "selectPage", (Action<string, string, bool>)SelectSection, (IReader<string>)null, (IReader<string>)null, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<List<int>, string>("options", "filteredWidgets", (Action<List<int>, string>)FilterWidgets, (IReader<List<int>>)(object)new ListReader<int>((IReader<int>)null), (IReader<string>)null));
		AddBinding((IBinding)new TriggerBinding("options", "toolchain.dependency.action", (Action)ConfirmDisplay));
		AddUpdateBinding((IUpdateBinding)(object)(m_DirectoryBrowserBinding = new WidgetBindings("options", "directoryBrowser")));
		m_DirectoryBrowserBinding.AddDefaultBindings();
		m_DirectoryBrowserBinding.AddBindings<IItemPicker.Bindings>();
		AddBinding((IBinding)(object)(m_DirectoryBrowserActive = new ValueBinding<bool>("options", "directoryBrowserActive", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)new TriggerBinding("options", "cancelDirectoryBrowser", (Action)OnCancelDirectory));
		SwitchBindings(browserActive: false);
		SelectDefaultPage();
		QueryToolchainState();
		ToolchainDeployment.dependencyManager.OnStateChanged += OnModdingToolchainStateChanged;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		ToolchainDeployment.dependencyManager.OnStateChanged -= OnModdingToolchainStateChanged;
	}

	public void RegisterSetting(Setting setting, string id, bool addPrefix = false)
	{
		bool flag = pages.Count == 0;
		Page page = setting.GetPageData(id, addPrefix).BuildPage();
		page.builtIn = setting.builtIn;
		if (pages.TryGetValue(page.id, out var value))
		{
			page.index = value.index;
		}
		else
		{
			page.index = pages.Count;
		}
		for (int i = 0; i < page.sections.Count; i++)
		{
			page.sections[i].index = i;
		}
		page.UpdateVisibility(m_IsAdvanced);
		page.UpdateNameAndDescription(m_IsAdvanced);
		page.UpdateWarning(m_IsAdvanced);
		pages[page.id] = page;
		if (flag && page.sections.Count != 0)
		{
			SelectVisibleSection(page);
		}
		m_PagesBinding.Update();
		RefreshPage();
	}

	public void UnregisterSettings(string id)
	{
		pages.Remove(id);
		if (m_ActivePageBinding.value == id && pages.Count != 0)
		{
			SelectVisibleSection(pages.Values.First());
		}
		m_PagesBinding.Update();
		RefreshPage();
	}

	private void SelectSection(string pageID, string sectionID, bool isAdvanced)
	{
		m_IsAdvanced = isAdvanced;
		SelectSection(pageID, sectionID);
	}

	private void SelectSection(string pageID, string sectionID)
	{
		Page value;
		if (m_SearchQuery != null)
		{
			FilterWidgets();
		}
		else if (pages.TryGetValue(pageID, out value))
		{
			if (!string.IsNullOrEmpty(sectionID))
			{
				Section section = value.visibleSections.FirstOrDefault((Section s) => s.id == sectionID);
				if (section != null)
				{
					SelectSection(value, section);
					return;
				}
			}
			SelectVisibleSection(value);
		}
		else
		{
			SelectDefaultPage();
		}
	}

	private void FilterWidgets(List<int> ids, string query)
	{
		bool flag = false;
		if (query != m_SearchQuery)
		{
			m_SearchQuery = query;
			m_SearchIds = ids.Distinct().ToList();
			flag = true;
		}
		else
		{
			List<int> list = ids.Distinct().ToList();
			if (list.Count != m_SearchIds.Count)
			{
				m_SearchIds = list;
				flag = true;
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != m_SearchIds[i])
					{
						m_SearchIds = list;
						flag = true;
						break;
					}
				}
			}
		}
		if (flag)
		{
			SelectSection(m_ActivePageBinding.value, m_ActiveSectionBinding.value);
		}
	}

	private void FilterWidgets()
	{
		List<IWidget> list = new List<IWidget>();
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		List<Page> list2 = sortedPages;
		for (int i = 0; i < m_SearchIds.Count; i++)
		{
			WidgetInfo.GetIndices(m_SearchIds[i], out var page, out var section, out var widget);
			if (page >= list2.Count)
			{
				continue;
			}
			bool flag = page != num;
			if (flag)
			{
				num2 = -1;
			}
			Page page2 = list2[page];
			if (section >= page2.sections.Count)
			{
				continue;
			}
			bool flag2 = section != num2;
			if (flag2)
			{
				num3 = -1;
			}
			Section section2 = page2.sections[section];
			if (widget >= section2.options.Count)
			{
				continue;
			}
			Option option = section2.options[widget];
			if (option.searchHidden || !option.isVisible)
			{
				continue;
			}
			string text = section2.groupNames[option.advancedGroupIndex];
			bool flag3 = option.advancedGroupIndex != num3;
			if (flag || flag2 || flag3)
			{
				Breadcrumbs breadcrumbs = new Breadcrumbs();
				Label label = new Label
				{
					displayName = LocalizedString.Id("Options.SECTION[" + page2.id + "]"),
					level = Label.Level.Title,
					beta = BetaFilter.options.Contains(page2.id)
				};
				breadcrumbs.WithLabel(label);
				if (page2.sections.Count > 1)
				{
					Label label2 = new Label
					{
						displayName = LocalizedString.Id("Options.TAB[" + section2.id + "]"),
						level = Label.Level.SubTitle,
						beta = BetaFilter.options.Contains(section2.id)
					};
					breadcrumbs.WithLabel(label2);
				}
				if (section2.groupNames.Length > 1 && section2.groupToShowName.Contains(text))
				{
					Label label3 = BuildGroupLabel(page2.id, text);
					breadcrumbs.WithLabel(label3);
				}
				list.Add(breadcrumbs);
			}
			list.Add(option.widget);
			num = page;
			num2 = section;
			num3 = option.advancedGroupIndex;
		}
		m_WidgetBindings.children = list;
	}

	private void SelectVisibleSection(Page page)
	{
		SelectSection(page, page.visibleSections.FirstOrDefault());
	}

	private void SelectSection(Page page, Section section, bool isAdvanced)
	{
		m_IsAdvanced = isAdvanced;
		SelectSection(page, section);
	}

	private void SelectSection(Page page, Section section)
	{
		m_ActivePageBinding.Update(page?.id ?? string.Empty);
		m_ActiveSectionBinding.Update(section?.id ?? string.Empty);
		m_WidgetBindings.children = section.GetItems(m_IsAdvanced);
	}

	private void RefreshPage()
	{
		SelectSection(m_ActivePageBinding.value, m_ActiveSectionBinding.value);
	}

	private void SelectDefaultPage()
	{
		foreach (Page value in pages.Values)
		{
			foreach (Section section in value.sections)
			{
				if (section.isVisible)
				{
					SelectSection(value, section);
					return;
				}
			}
		}
	}

	public void OpenPage(string pageID, string sectionID, bool isAdvanced)
	{
		if (string.IsNullOrEmpty(pageID))
		{
			throw new ArgumentException("pageID can not be null or empty", "pageID");
		}
		switch (GameManager.instance.gameMode)
		{
		case GameMode.MainMenu:
			if (m_MenuUISystem == null)
			{
				m_MenuUISystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<MenuUISystem>();
			}
			if (m_MenuUISystem != null)
			{
				m_MenuUISystem.activeScreen = MenuUISystem.MenuScreen.Options;
				SelectSection(pageID, sectionID, isAdvanced);
			}
			break;
		case GameMode.Game:
			if (m_GameScreenUISystem == null)
			{
				m_GameScreenUISystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<GameScreenUISystem>();
			}
			if (m_GameScreenUISystem != null)
			{
				m_GameScreenUISystem.activeScreen = GameScreenUISystem.GameScreen.Options;
				SelectSection(pageID, sectionID, isAdvanced);
			}
			break;
		case GameMode.Editor:
			if (m_EditorScreenUISystem == null)
			{
				m_EditorScreenUISystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<EditorScreenUISystem>();
			}
			if (m_EditorScreenUISystem != null)
			{
				m_EditorScreenUISystem.activeScreen = EditorScreenUISystem.EditorScreen.Options;
				SelectSection(pageID, sectionID, isAdvanced);
			}
			break;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		switch (GameManager.instance.gameMode)
		{
		case GameMode.MainMenu:
			if (m_MenuUISystem == null)
			{
				m_MenuUISystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<MenuUISystem>();
			}
			if (m_MenuUISystem != null && m_MenuUISystem.activeScreen != MenuUISystem.MenuScreen.Options)
			{
				return;
			}
			break;
		case GameMode.Game:
			if (m_GameScreenUISystem == null)
			{
				m_GameScreenUISystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<GameScreenUISystem>();
			}
			if (m_GameScreenUISystem != null && m_GameScreenUISystem.activeScreen != GameScreenUISystem.GameScreen.Options)
			{
				return;
			}
			break;
		case GameMode.Editor:
			if (m_EditorScreenUISystem == null)
			{
				m_EditorScreenUISystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<EditorScreenUISystem>();
			}
			if (m_EditorScreenUISystem != null && m_EditorScreenUISystem.activeScreen != EditorScreenUISystem.EditorScreen.Options)
			{
				return;
			}
			break;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (Page value in pages.Values)
		{
			if (!string.IsNullOrEmpty(m_SearchQuery) || m_ActivePageBinding.value == value.id)
			{
				flag |= value.UpdateNameAndDescription(m_IsAdvanced);
				bool flag3 = value.UpdateVisibility(m_IsAdvanced);
				flag = flag || flag3;
				flag2 = flag2 || flag3;
			}
			if (string.IsNullOrEmpty(m_SearchQuery))
			{
				bool flag4 = value.UpdateWarning(m_IsAdvanced);
				flag = flag || flag4;
				if (m_ActivePageBinding.value == value.id)
				{
					flag2 = flag2 || flag4;
				}
			}
		}
		if (flag)
		{
			m_PagesBinding.Update();
		}
		if (flag2)
		{
			RefreshPage();
		}
		base.OnUpdate();
		if (m_DisplayConfirmationTime > 0f)
		{
			m_DisplayConfirmationTime -= Time.deltaTime;
			if (m_DisplayConfirmationTime <= 0f)
			{
				RevertDisplay();
			}
		}
		if (m_DirectoryBrowser != null)
		{
			m_DirectoryBrowserActive.Update(true);
			m_DirectoryBrowserBinding.children = m_DirectoryBrowser.children;
		}
		else
		{
			m_DirectoryBrowserActive.Update(false);
			m_DirectoryBrowserBinding.children = Array.Empty<IWidget>();
		}
		if (m_LastDirectoryBrowser != m_DirectoryBrowser)
		{
			SwitchBindings(m_DirectoryBrowser != null);
			m_LastDirectoryBrowser = m_DirectoryBrowser;
		}
		object obj;
		if (SharedSettings.instance.userInterface.keyboardLayout != InterfaceSettings.KeyboardLayout.AutoDetect)
		{
			obj = null;
		}
		else
		{
			Keyboard current2 = Keyboard.current;
			obj = ((current2 != null) ? current2.keyboardLayout : null);
		}
		string text = (string)obj;
		if (!(text != m_LastLayout))
		{
			return;
		}
		m_LastLayout = text;
		Dictionary<string, ControlPath> dictionary = new Dictionary<string, ControlPath>();
		Keyboard current3 = Keyboard.current;
		if (m_LastLayout != null && current3 != null)
		{
			Enumerator<KeyControl> enumerator2 = current3.allKeys.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					KeyControl key = enumerator2.Current;
					if (ControlPath.NeedLocalName(current3, key))
					{
						InputControl val = ((IEnumerable<InputControl>)(object)((InputDevice)current3).allControls).FirstOrDefault(delegate(InputControl c)
						{
							_ = c.displayName;
							return c.m_DisplayNameFromLayout == ((InputControl)key).displayName;
						});
						if (val == null)
						{
							dictionary[((InputControl)key).name] = new ControlPath
							{
								name = ((InputControl)key).displayName,
								displayName = ((InputControl)key).displayName
							};
						}
						else if (((InputControl)key).name != val.name)
						{
							dictionary[((InputControl)key).name] = new ControlPath
							{
								name = val.name,
								displayName = ((val.name.Length == 1) ? val.name.ToUpper() : val.name)
							};
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
			}
		}
		m_LayoutMapBinding.Update(dictionary);
	}

	public void ShowDisplayConfirmation()
	{
		m_DisplayConfirmationTime = 15f;
		m_LastResolution = SharedSettings.instance.graphics.resolution;
		m_LastDisplayMode = SharedSettings.instance.graphics.displayMode;
		m_LastDisplayIndex = SharedSettings.instance.graphics.displayIndex;
	}

	private void ConfirmDisplay()
	{
		m_DisplayConfirmationTime = -1f;
	}

	private void RevertDisplay()
	{
		m_DisplayConfirmationTime = -1f;
		SharedSettings.instance.graphics.resolution = m_LastResolution;
		SharedSettings.instance.graphics.displayMode = m_LastDisplayMode;
		SharedSettings.instance.graphics.displayIndex = m_LastDisplayIndex;
		SharedSettings.instance.graphics.ApplyAndSave();
	}

	private void OnPageClosed(string pageId)
	{
		if (pageId == "Graphics")
		{
			Telemetry.GraphicsSettings();
		}
	}

	private static Label BuildGroupLabel(string pageId, string groupId)
	{
		string text = pageId + "." + groupId;
		return new Label
		{
			displayName = LocalizedString.Id("Options.GROUP[" + text + "]"),
			level = Label.Level.SubTitle,
			beta = BetaFilter.options.Contains(text)
		};
	}

	[Preserve]
	public OptionsUISystem()
	{
	}
}
