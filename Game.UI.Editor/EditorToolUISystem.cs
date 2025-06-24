using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.Annotations;
using Colossal.UI.Binding;
using Game.Tools;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

public class EditorToolUISystem : UISystemBase
{
	private const string kGroup = "editorTool";

	private ToolSystem m_ToolSystem;

	private EditorPanelUISystem m_EditorPanelUISystem;

	private InspectorPanelSystem m_InspectorPanelSystem;

	private GetterValueBinding<IEditorTool[]> m_ToolsBinding;

	private IEditorTool[] m_Tools;

	private bool[] m_Disabled;

	private IEditorTool m_ActiveTool;

	private Entity m_LastSelectedEntity;

	public override GameMode gameMode => GameMode.Editor;

	public IEditorTool[] tools
	{
		get
		{
			return m_Tools;
		}
		set
		{
			m_Tools = value;
			m_Disabled = value.Select((IEditorTool t) => t.disabled).ToArray();
		}
	}

	[CanBeNull]
	public IEditorTool activeTool
	{
		get
		{
			return m_ActiveTool;
		}
		set
		{
			if (value != m_ActiveTool || (m_ActiveTool != null && !m_ActiveTool.active))
			{
				if (m_ActiveTool != null)
				{
					m_ActiveTool.active = false;
				}
				m_ActiveTool = value;
				if (m_ActiveTool != null)
				{
					m_ActiveTool.active = true;
				}
			}
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_EditorPanelUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EditorPanelUISystem>();
		m_InspectorPanelSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<InspectorPanelSystem>();
		tools = new IEditorTool[6]
		{
			new EditorAssetImportTool(((ComponentSystemBase)this).World),
			new EditorTerrainTool(((ComponentSystemBase)this).World),
			new EditorPrefabTool(((ComponentSystemBase)this).World),
			new EditorPrefabEditorTool(((ComponentSystemBase)this).World),
			new EditorPhotoTool(((ComponentSystemBase)this).World),
			new EditorBulldozeTool(((ComponentSystemBase)this).World)
		};
		AddUpdateBinding((IUpdateBinding)(object)(m_ToolsBinding = new GetterValueBinding<IEditorTool[]>("editorTool", "tools", (Func<IEditorTool[]>)(() => tools), (IWriter<IEditorTool[]>)(object)new ArrayWriter<IEditorTool>((IWriter<IEditorTool>)(object)new ValueWriter<IEditorTool>(), false), (EqualityComparer<IEditorTool[]>)null)));
		AddUpdateBinding((IUpdateBinding)(object)new GetterValueBinding<string>("editorTool", "activeTool", (Func<string>)(() => activeTool?.id), (IWriter<string>)(object)ValueWriters.Nullable<string>((IWriter<string>)new StringWriter()), (EqualityComparer<string>)null));
		AddBinding((IBinding)(object)new TriggerBinding<string>("editorTool", "selectTool", (Action<string>)SelectTool, (IReader<string>)null));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		if (activeTool != null && !activeTool.active)
		{
			activeTool = null;
		}
		if (m_ToolSystem.selected != m_LastSelectedEntity)
		{
			SelectEntity(m_ToolSystem.selected);
		}
		if (UpdateToolState())
		{
			m_ToolsBinding.TriggerUpdate();
		}
	}

	public void SelectEntity(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_LastSelectedEntity = entity;
		if (m_InspectorPanelSystem.SelectEntity(entity))
		{
			activeTool = null;
			m_EditorPanelUISystem.activePanel = m_InspectorPanelSystem;
		}
		else if (m_EditorPanelUISystem.activePanel == m_InspectorPanelSystem)
		{
			m_EditorPanelUISystem.activePanel = null;
		}
	}

	public void SelectEntitySubMesh(Entity entity, int subMeshIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_LastSelectedEntity = entity;
		if (m_InspectorPanelSystem.SelectMesh(entity, subMeshIndex))
		{
			activeTool = null;
			m_EditorPanelUISystem.activePanel = m_InspectorPanelSystem;
		}
		else if (m_EditorPanelUISystem.activePanel == m_InspectorPanelSystem)
		{
			m_EditorPanelUISystem.activePanel = null;
		}
	}

	public void SelectTool([CanBeNull] string id)
	{
		activeTool = tools.FirstOrDefault((IEditorTool t) => t.id == id);
	}

	private bool UpdateToolState()
	{
		bool result = false;
		for (int i = 0; i < m_Tools.Length; i++)
		{
			bool disabled = m_Tools[i].disabled;
			if (disabled != m_Disabled[i])
			{
				m_Disabled[i] = disabled;
				result = true;
			}
		}
		return result;
	}

	[Preserve]
	public EditorToolUISystem()
	{
	}
}
