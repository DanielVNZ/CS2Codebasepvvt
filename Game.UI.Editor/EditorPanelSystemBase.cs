using System;
using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.Logging;
using Game.UI.Localization;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

public abstract class EditorPanelSystemBase : GameSystemBase, IEditorPanel
{
	[CanBeNull]
	private IEditorPanel m_LastSubPanel;

	[CanBeNull]
	protected IEditorPanel activeSubPanel { get; set; }

	protected virtual LocalizedString title { get; set; }

	protected virtual IList<IWidget> children { get; set; } = Array.Empty<IWidget>();

	public virtual EditorPanelWidgetRenderer widgetRenderer => EditorPanelWidgetRenderer.Editor;

	protected ILog log { get; } = LogManager.GetLogger("Editor");

	LocalizedString IEditorPanel.title
	{
		get
		{
			if (activeSubPanel == null)
			{
				return title;
			}
			return activeSubPanel.title;
		}
	}

	IList<IWidget> IEditorPanel.children
	{
		get
		{
			if (activeSubPanel == null)
			{
				return children;
			}
			return activeSubPanel.children;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		((ComponentSystemBase)this).Enabled = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (activeSubPanel != m_LastSubPanel)
		{
			IEditorPanel lastSubPanel = m_LastSubPanel;
			ComponentSystemBase val = (ComponentSystemBase)((lastSubPanel is ComponentSystemBase) ? lastSubPanel : null);
			if (val != null)
			{
				val.Enabled = false;
				val.Update();
			}
			m_LastSubPanel = activeSubPanel;
			IEditorPanel editorPanel = activeSubPanel;
			ComponentSystemBase val2 = (ComponentSystemBase)((editorPanel is ComponentSystemBase) ? editorPanel : null);
			if (val2 != null)
			{
				val2.Enabled = true;
			}
		}
		IEditorPanel editorPanel2 = activeSubPanel;
		ComponentSystemBase val3 = (ComponentSystemBase)((editorPanel2 is ComponentSystemBase) ? editorPanel2 : null);
		if (val3 != null)
		{
			val3.Update();
		}
	}

	protected virtual void OnValueChanged(IWidget widget)
	{
	}

	protected virtual bool OnCancel()
	{
		return OnClose();
	}

	protected virtual bool OnClose()
	{
		return true;
	}

	public void CloseSubPanel()
	{
		activeSubPanel = null;
	}

	void IEditorPanel.OnValueChanged(IWidget widget)
	{
		if (activeSubPanel != null)
		{
			activeSubPanel.OnValueChanged(widget);
		}
		else
		{
			OnValueChanged(widget);
		}
	}

	bool IEditorPanel.OnCancel()
	{
		if (activeSubPanel != null)
		{
			if (activeSubPanel.OnCancel())
			{
				activeSubPanel = null;
			}
			return false;
		}
		return OnCancel();
	}

	bool IEditorPanel.OnClose()
	{
		if (activeSubPanel != null)
		{
			if (activeSubPanel.OnClose())
			{
				activeSubPanel = null;
			}
			return false;
		}
		return OnClose();
	}

	[Preserve]
	protected EditorPanelSystemBase()
	{
	}
}
