using Colossal.Entities;
using Game.UI.InGame;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

public class EditorPhotoModePanel : EditorPanelSystemBase
{
	private PhotoModeUISystem m_PhotoModeUISystem;

	public override EditorPanelWidgetRenderer widgetRenderer => EditorPanelWidgetRenderer.PhotoMode;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		title = "PhotoMode.TITLE";
		m_PhotoModeUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PhotoModeUISystem>();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		m_PhotoModeUISystem.Activate(enabled: true);
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
		m_PhotoModeUISystem.Activate(enabled: false);
	}

	[Preserve]
	public EditorPhotoModePanel()
	{
	}
}
