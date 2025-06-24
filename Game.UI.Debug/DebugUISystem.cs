using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.Annotations;
using Colossal.PSI.Common;
using Colossal.UI.Binding;
using Game.Debug;
using Game.SceneFlow;
using Game.Settings;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.UI.Debug;

public class DebugUISystem : UISystemBase
{
	private class Panel : IJsonWritable, IDisposable
	{
		public readonly Panel panel;

		[CanBeNull]
		private string displayName;

		private bool childrenDirty;

		public List<IWidget> children { get; set; }

		public Panel(Panel panel)
		{
			this.panel = panel;
			displayName = panel.displayName;
			children = new List<IWidget>();
			childrenDirty = true;
			panel.onSetDirty += OnSetDirty;
		}

		private void OnSetDirty(Panel panel)
		{
			childrenDirty = true;
		}

		public bool Update()
		{
			bool result = false;
			if (panel.displayName != displayName)
			{
				result = true;
				displayName = panel.displayName;
			}
			if (childrenDirty)
			{
				childrenDirty = false;
				result = true;
				children.Clear();
				children.AddRange(DebugWidgetBuilders.BuildWidgets(panel.children));
			}
			return result;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("displayName");
			writer.Write(displayName);
			writer.TypeEnd();
		}

		public void Dispose()
		{
			panel.onSetDirty -= OnSetDirty;
		}
	}

	private const string kGroup = "debug";

	private ValueBinding<bool> m_EnabledBinding;

	private ValueBinding<bool> m_VisibleBinding;

	private ValueBinding<Panel> m_SelectedPanelBinding;

	private WidgetBindings m_WidgetBindings;

	private ValueBinding<IDebugBinding> m_ObservedBindingBinding;

	private EventBinding<IDebugBinding> m_BindingTriggeredBinding;

	private GetterValueBinding<List<DebugWatchSystem.Watch>> m_WatchesBinding;

	private string m_SelectedPanel;

	private bool m_ShowDeveloperInfo;

	public bool visible => m_VisibleBinding.value;

	[CanBeNull]
	public IDebugBinding observedBinding
	{
		get
		{
			return m_ObservedBindingBinding.value;
		}
		set
		{
			m_ObservedBindingBinding.Update(value);
		}
	}

	private string selectedPanel
	{
		get
		{
			return m_SelectedPanel;
		}
		set
		{
			m_SelectedPanel = value;
		}
	}

	public bool developerInfoVisible
	{
		get
		{
			return m_ShowDeveloperInfo;
		}
		set
		{
			m_ShowDeveloperInfo = value;
		}
	}

	private static IEnumerable<Panel> visiblePanels => DebugManager.instance.panels.Where((Panel panel) => !panel.isEditorOnly && ((IEnumerable<Widget>)panel.children).Any((Widget x) => !x.isEditorOnly));

	private static bool debugSystemEnabled => (GameManager.instance?.configuration)?.developerMode ?? false;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		base.OnCreate();
		AddBinding((IBinding)(object)(m_EnabledBinding = new ValueBinding<bool>("debug", "enabled", debugSystemEnabled, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_VisibleBinding = new ValueBinding<bool>("debug", "visible", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<List<string>>("debug", "panels", (Func<List<string>>)GetPanels, (IWriter<List<string>>)(object)new ListWriter<string>((IWriter<string>)null), (EqualityComparer<List<string>>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("debug", "selectedIndex", (Func<int>)(() => GetPanelIndex(selectedPanel)), (IWriter<int>)null, (EqualityComparer<int>)null));
		AddBinding((IBinding)(object)(m_SelectedPanelBinding = new ValueBinding<Panel>("debug", "selectedPanel", (Panel)null, (IWriter<Panel>)(object)ValueWriters.Nullable<Panel>((IWriter<Panel>)(object)new ValueWriter<Panel>()), (EqualityComparer<Panel>)null)));
		AddUpdateBinding((IUpdateBinding)(object)(m_WidgetBindings = new WidgetBindings("debug")));
		m_WidgetBindings.AddDefaultBindings();
		AddBinding((IBinding)(object)(m_ObservedBindingBinding = new ValueBinding<IDebugBinding>("debug", "observedBinding", (IDebugBinding)null, (IWriter<IDebugBinding>)(object)ValueWriters.Nullable<IDebugBinding>((IWriter<IDebugBinding>)new DebugBindingWriter()), (EqualityComparer<IDebugBinding>)null)));
		AddBinding((IBinding)(object)(m_BindingTriggeredBinding = new EventBinding<IDebugBinding>("debug", "bindingTriggered", (IWriter<IDebugBinding>)new DebugBindingWriter())));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<bool>("debug", "developerInfoVisible", (Func<bool>)(() => developerInfoVisible), (IWriter<bool>)null, (EqualityComparer<bool>)null));
		AddBinding((IBinding)(object)(m_WatchesBinding = new GetterValueBinding<List<DebugWatchSystem.Watch>>("debug", "watches", (Func<List<DebugWatchSystem.Watch>>)(() => ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DebugWatchSystem>().watches), (IWriter<List<DebugWatchSystem.Watch>>)(object)new ListWriter<DebugWatchSystem.Watch>((IWriter<DebugWatchSystem.Watch>)(object)new ValueWriter<DebugWatchSystem.Watch>()), (EqualityComparer<List<DebugWatchSystem.Watch>>)null)));
		AddBinding((IBinding)new TriggerBinding("debug", "show", (Action)Show));
		AddBinding((IBinding)new TriggerBinding("debug", "hide", (Action)Hide));
		AddBinding((IBinding)(object)new TriggerBinding<int>("debug", "selectPanel", (Action<int>)SelectPanel, (IReader<int>)null));
		AddBinding((IBinding)new TriggerBinding("debug", "selectPreviousPanel", (Action)SelectPreviousPanel));
		AddBinding((IBinding)new TriggerBinding("debug", "selectNextPanel", (Action)SelectNextPanel));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (visible)
		{
			UpdateSelectedPanel();
			DebugWatchSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DebugWatchSystem>();
			if (orCreateSystemManaged.watchesChanged)
			{
				m_WatchesBinding.TriggerUpdate();
				orCreateSystemManaged.ClearWatchesChanged();
			}
		}
		base.OnUpdate();
	}

	public void Trigger(IDebugBinding binding)
	{
		m_BindingTriggeredBinding.Trigger(binding);
	}

	public void Show()
	{
		if (visible || !m_EnabledBinding.value)
		{
			return;
		}
		m_VisibleBinding.Update(true);
		if (!SharedSettings.instance.userInterface.dismissedConfirmations.Contains("DebugMenu") && PlatformManager.instance.achievementsEnabled)
		{
			GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(new DismissibleConfirmationDialog("Common.DIALOG_TITLE[Warning]", "Common.DIALOG_MESSAGE[DisableAchievements]", "Common.DIALOG_ACTION[Yes]", "Common.DIALOG_ACTION[No]"), delegate(int msg, bool dismiss)
			{
				if (msg == 0)
				{
					if (dismiss)
					{
						SharedSettings.instance.userInterface.dismissedConfirmations.Add("DebugMenu");
						SharedSettings.instance.userInterface.ApplyAndSave();
					}
					((ComponentSystemBase)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DebugSystem>()).Enabled = true;
					PlatformManager.instance.achievementsEnabled = false;
				}
			});
		}
		else
		{
			((ComponentSystemBase)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DebugSystem>()).Enabled = true;
			PlatformManager.instance.achievementsEnabled = false;
		}
		UpdateSelectedPanel();
	}

	public void Hide()
	{
		if (visible)
		{
			m_VisibleBinding.Update(false);
			((ComponentSystemBase)((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DebugSystem>()).Enabled = false;
			UpdateSelectedPanel();
		}
	}

	private void SelectPanel(int index)
	{
		if (visible)
		{
			Panel panel = GetPanel(index);
			selectedPanel = ((panel != null) ? panel.displayName : null);
			UpdateSelectedPanel();
		}
	}

	private void SelectPreviousPanel()
	{
		if (!visible)
		{
			return;
		}
		int panelCount = GetPanelCount();
		if (panelCount != 0)
		{
			int panelIndex = GetPanelIndex(selectedPanel);
			if (panelIndex <= 0 || panelIndex >= panelCount)
			{
				SelectPanel(panelCount - 1);
			}
			else
			{
				SelectPanel(panelIndex - 1);
			}
		}
	}

	private void SelectNextPanel()
	{
		if (!visible)
		{
			return;
		}
		int panelCount = GetPanelCount();
		if (panelCount != 0)
		{
			int panelIndex = GetPanelIndex(selectedPanel);
			if (panelIndex < 0 || panelIndex >= panelCount - 1)
			{
				SelectPanel(0);
			}
			else
			{
				SelectPanel(panelIndex + 1);
			}
		}
	}

	private List<string> GetPanels()
	{
		return visiblePanels.Select((Panel p) => p.displayName).ToList();
	}

	private void UpdateSelectedPanel()
	{
		Panel value = m_SelectedPanelBinding.value;
		Panel obj = value?.panel;
		Panel val = (visible ? GetPanel(GetPanelIndex(selectedPanel)) : null);
		if (obj != val)
		{
			value?.Dispose();
			m_SelectedPanelBinding.Update((val != null) ? new Panel(val) : null);
			m_WidgetBindings.children = ((m_SelectedPanelBinding.value != null) ? m_SelectedPanelBinding.value.children : new List<IWidget>());
		}
		else if (value != null && value.Update())
		{
			m_SelectedPanelBinding.TriggerUpdate();
			m_WidgetBindings.children = ((m_SelectedPanelBinding.value != null) ? m_SelectedPanelBinding.value.children : new List<IWidget>());
		}
	}

	private static int GetPanelCount()
	{
		return visiblePanels.Count();
	}

	[CanBeNull]
	private static Panel GetPanel(int panelIndex)
	{
		if (panelIndex < 0)
		{
			return null;
		}
		return visiblePanels.Skip(panelIndex).FirstOrDefault();
	}

	private static int GetPanelIndex(string name)
	{
		return visiblePanels.ToList().FindIndex((Panel panel) => panel.displayName == name);
	}

	[Preserve]
	public DebugUISystem()
	{
	}
}
