using System;
using System.Collections.Generic;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Game.UI.Menu;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

public class EditorAssetUploadPanel : EditorPanelSystemBase
{
	private AssetUploadPanelUISystem m_AssetUploadPanelSystem;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_AssetUploadPanelSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AssetUploadPanelUISystem>();
		((ComponentSystemBase)m_AssetUploadPanelSystem).Enabled = false;
		title = "Menu.ASSET_UPLOAD";
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		AssetUploadPanelUISystem assetUploadPanelSystem = m_AssetUploadPanelSystem;
		assetUploadPanelSystem.onChildrenChange = (Action<IList<IWidget>>)Delegate.Combine(assetUploadPanelSystem.onChildrenChange, new Action<IList<IWidget>>(OnChildrenChange));
		((ComponentSystemBase)m_AssetUploadPanelSystem).Enabled = true;
		children = m_AssetUploadPanelSystem.children;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
		((ComponentSystemBase)m_AssetUploadPanelSystem).Enabled = false;
		AssetUploadPanelUISystem assetUploadPanelSystem = m_AssetUploadPanelSystem;
		assetUploadPanelSystem.onChildrenChange = (Action<IList<IWidget>>)Delegate.Remove(assetUploadPanelSystem.onChildrenChange, new Action<IList<IWidget>>(OnChildrenChange));
	}

	private void OnChildrenChange(IList<IWidget> _children)
	{
		children = _children;
	}

	public void Show(AssetData mainAsset, bool allowManualFileCopy = true)
	{
		m_AssetUploadPanelSystem.Show(mainAsset, allowManualFileCopy);
	}

	protected override bool OnClose()
	{
		return m_AssetUploadPanelSystem.Close();
	}

	[Preserve]
	public EditorAssetUploadPanel()
	{
	}
}
