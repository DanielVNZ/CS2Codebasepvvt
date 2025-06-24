using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Settings;
using Game.UI.Localization;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

public class EditorPanelUISystem : UISystemBase
{
	private const string kGroup = "editorPanel";

	[CanBeNull]
	private IEditorPanel m_LastPanel;

	private ValueBinding<bool> m_ActiveBinding;

	private ValueBinding<LocalizedString?> m_TitleBinding;

	private WidgetBindings m_WidgetBindings;

	public override GameMode gameMode => GameMode.GameOrEditor;

	[CanBeNull]
	public IEditorPanel activePanel { get; set; }

	private EditorPanelWidgetRenderer widgetRenderer
	{
		get
		{
			if (activePanel != null)
			{
				return activePanel.widgetRenderer;
			}
			return EditorPanelWidgetRenderer.Editor;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		base.OnCreate();
		AddBinding((IBinding)(object)(m_ActiveBinding = new ValueBinding<bool>("editorPanel", "active", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)(object)(m_TitleBinding = new ValueBinding<LocalizedString?>("editorPanel", "title", (LocalizedString?)null, (IWriter<LocalizedString?>)(object)ValueWritersStruct.Nullable<LocalizedString>((IWriter<LocalizedString>)(object)new ValueWriter<LocalizedString>()), (EqualityComparer<LocalizedString?>)null)));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("editorPanel", "width", (Func<int>)GetWidth, (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<int>("editorPanel", "widgetRenderer", (Func<int>)(() => (int)widgetRenderer), (IWriter<int>)null, (EqualityComparer<int>)null));
		AddUpdateBinding((IUpdateBinding)(object)(m_WidgetBindings = new WidgetBindings("editorPanel")));
		AddEditorWidgetBindings(m_WidgetBindings);
		m_WidgetBindings.EventValueChanged += OnValueChanged;
		AddBinding((IBinding)new TriggerBinding("editorPanel", "cancel", (Action)Cancel));
		AddBinding((IBinding)new TriggerBinding("editorPanel", "close", (Action)Close));
		AddBinding((IBinding)(object)new TriggerBinding<int>("editorPanel", "setWidth", (Action<int>)SetWidth, (IReader<int>)null));
	}

	public static void AddEditorWidgetBindings(WidgetBindings widgetBindings)
	{
		widgetBindings.AddDefaultBindings();
		widgetBindings.AddBindings<EditorSection.Bindings>();
		widgetBindings.AddBindings<SeasonsField.Bindings>();
		widgetBindings.AddBindings<IItemPicker.Bindings>();
		widgetBindings.AddBindings<PopupSearchField.Bindings>();
		widgetBindings.AddBindings<AnimationCurveField.Bindings>();
		widgetBindings.AddBindings<LocalizationField.Bindings>();
		widgetBindings.AddBindings<FilterMenu.Bindings>();
		widgetBindings.AddBindings<HierarchyMenu.Bindings>();
		widgetBindings.AddBindings<ExternalLinkField.Bindings>();
		widgetBindings.AddBindings<ListField.Bindings>();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		activePanel = null;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (activePanel != m_LastPanel)
		{
			IEditorPanel lastPanel = m_LastPanel;
			ComponentSystemBase val = (ComponentSystemBase)((lastPanel is ComponentSystemBase) ? lastPanel : null);
			if (val != null)
			{
				val.Enabled = false;
				val.Update();
			}
			m_LastPanel = activePanel;
			IEditorPanel editorPanel = activePanel;
			ComponentSystemBase val2 = (ComponentSystemBase)((editorPanel is ComponentSystemBase) ? editorPanel : null);
			if (val2 != null)
			{
				val2.Enabled = true;
			}
		}
		if (activePanel != null)
		{
			IEditorPanel editorPanel2 = activePanel;
			ComponentSystemBase val3 = (ComponentSystemBase)((editorPanel2 is ComponentSystemBase) ? editorPanel2 : null);
			if (val3 != null)
			{
				val3.Update();
			}
			m_ActiveBinding.Update(true);
			m_TitleBinding.Update((LocalizedString?)activePanel.title);
			m_WidgetBindings.children = activePanel.children;
		}
		else
		{
			m_ActiveBinding.Update(false);
			m_TitleBinding.Update((LocalizedString?)null);
			m_WidgetBindings.children = Array.Empty<IWidget>();
		}
		base.OnUpdate();
	}

	public void OnValueChanged(IWidget widget)
	{
		activePanel?.OnValueChanged(widget);
	}

	private int GetWidth()
	{
		return (SharedSettings.instance?.editor)?.inspectorWidth ?? 450;
	}

	private void SetWidth(int width)
	{
		EditorSettings editorSettings = SharedSettings.instance?.editor;
		if (editorSettings != null)
		{
			editorSettings.inspectorWidth = width;
		}
	}

	private void Cancel()
	{
		if (activePanel != null && activePanel.OnCancel())
		{
			activePanel = null;
		}
	}

	private void Close()
	{
		if (activePanel != null && activePanel.OnClose())
		{
			activePanel = null;
		}
	}

	[Preserve]
	public EditorPanelUISystem()
	{
	}
}
