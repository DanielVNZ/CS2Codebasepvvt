using System;
using System.Collections.Generic;
using Colossal.IO.AssetDatabase;
using Colossal.UI.Binding;
using Game.UI.Editor;
using Game.UI.Widgets;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Menu;

public class StandaloneAssetUploadPanelUISystem : UISystemBase
{
	private static readonly string kGroup = "assetUploadPanel";

	private AssetUploadPanelUISystem m_AssetUploadPanelUISystem;

	private WidgetBindings m_WidgetBindings;

	private ValueBinding<bool> m_Visible;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		base.OnCreate();
		m_AssetUploadPanelUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AssetUploadPanelUISystem>();
		((ComponentSystemBase)m_AssetUploadPanelUISystem).Enabled = false;
		AddUpdateBinding((IUpdateBinding)(object)(m_WidgetBindings = new WidgetBindings(kGroup)));
		EditorPanelUISystem.AddEditorWidgetBindings(m_WidgetBindings);
		AddBinding((IBinding)(object)(m_Visible = new ValueBinding<bool>(kGroup, "visible", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		AddBinding((IBinding)new TriggerBinding(kGroup, "close", (Action)OnClose));
	}

	public void Show(AssetData mainAsset, bool allowManualFileCopy = true)
	{
		m_AssetUploadPanelUISystem.Show(mainAsset, allowManualFileCopy);
		AssetUploadPanelUISystem assetUploadPanelUISystem = m_AssetUploadPanelUISystem;
		assetUploadPanelUISystem.onChildrenChange = (Action<IList<IWidget>>)Delegate.Remove(assetUploadPanelUISystem.onChildrenChange, new Action<IList<IWidget>>(OnChildrenChange));
		AssetUploadPanelUISystem assetUploadPanelUISystem2 = m_AssetUploadPanelUISystem;
		assetUploadPanelUISystem2.onChildrenChange = (Action<IList<IWidget>>)Delegate.Combine(assetUploadPanelUISystem2.onChildrenChange, new Action<IList<IWidget>>(OnChildrenChange));
		((ComponentSystemBase)m_AssetUploadPanelUISystem).Enabled = true;
		m_WidgetBindings.children = m_AssetUploadPanelUISystem.children;
		m_Visible.Update(true);
	}

	private void OnClose()
	{
		if (m_AssetUploadPanelUISystem.Close())
		{
			AssetUploadPanelUISystem assetUploadPanelUISystem = m_AssetUploadPanelUISystem;
			assetUploadPanelUISystem.onChildrenChange = (Action<IList<IWidget>>)Delegate.Remove(assetUploadPanelUISystem.onChildrenChange, new Action<IList<IWidget>>(OnChildrenChange));
			((ComponentSystemBase)m_AssetUploadPanelUISystem).Enabled = false;
			m_WidgetBindings.children = Array.Empty<IWidget>();
			m_Visible.Update(false);
		}
	}

	private void OnChildrenChange(IList<IWidget> children)
	{
		m_WidgetBindings.children = children;
	}

	[Preserve]
	public StandaloneAssetUploadPanelUISystem()
	{
	}
}
