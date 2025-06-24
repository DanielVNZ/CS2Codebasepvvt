using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Game.UI.Widgets;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class PrefabEdítorPanelSystem : EditorPanelSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private PrefabSystem m_PrefabSystem;

	private ToolSystem m_ToolSystem;

	private EditorAssetCategorySystem m_CategorySystem;

	private InspectorPanelSystem m_InspectorPanelSystem;

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_ModifiedPrefabQuery;

	private PrefabPickerAdapter m_Adapter;

	private HierarchyMenu<EditorAssetCategory> m_CategoryMenu;

	private EditorAssetCategory m_AllCategory;

	private bool m_PrefabsDirty;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CategorySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EditorAssetCategorySystem>();
		m_InspectorPanelSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<InspectorPanelSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabData>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_ModifiedPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_Adapter = new PrefabPickerAdapter
		{
			displayPrefabTypeTooltip = true
		};
		PrefabPickerAdapter prefabPickerAdapter = m_Adapter;
		prefabPickerAdapter.EventPrefabSelected = (Action<PrefabBase>)Delegate.Combine(prefabPickerAdapter.EventPrefabSelected, new Action<PrefabBase>(OnPrefabSelected));
		title = "Editor.TOOL[PrefabEditorTool]";
		IWidget[] obj = new IWidget[5]
		{
			new PopupSearchField
			{
				adapter = m_Adapter
			},
			new FilterMenu
			{
				adapter = m_Adapter
			},
			null,
			null,
			null
		};
		Row row = new Row
		{
			flex = FlexLayout.Fill
		};
		IWidget[] array2 = new IWidget[2];
		HierarchyMenu<EditorAssetCategory> obj2 = new HierarchyMenu<EditorAssetCategory>
		{
			selectionType = HierarchyMenu.SelectionType.singleSelection,
			onSelectionChange = UpdatePrefabs,
			flex = new FlexLayout(1f, 0f, 0),
			path = "PrefabEditorToolCategories"
		};
		HierarchyMenu<EditorAssetCategory> hierarchyMenu = obj2;
		m_CategoryMenu = obj2;
		array2[0] = hierarchyMenu;
		array2[1] = new ItemPicker<PrefabItem>
		{
			adapter = m_Adapter,
			hasFavorites = true,
			flex = new FlexLayout(2f, 0f, 0)
		};
		row.children = array2;
		obj[2] = row;
		obj[3] = new ItemPickerFooter
		{
			adapter = m_Adapter
		};
		obj[4] = new Button
		{
			displayName = "Editor.CREATE_NEW_PREFAB",
			action = OnCreatePrefabSelected
		};
		children = obj;
		GenerateCategories();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_Adapter.searchQuery = string.Empty;
		m_Adapter.selectedPrefab = null;
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		m_Adapter.LoadSettings();
		m_CategoryMenu.items = GetHierarchy();
		UpdatePrefabs();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (!((EntityQuery)(ref m_ModifiedPrefabQuery)).IsEmptyIgnoreFilter)
		{
			m_PrefabsDirty = true;
		}
		if (m_PrefabsDirty && base.activeSubPanel == null)
		{
			UpdatePrefabs();
		}
		m_Adapter.Update();
	}

	private void OnPrefabSelected(PrefabBase prefab)
	{
		m_InspectorPanelSystem.SelectPrefab(prefab);
		base.activeSubPanel = m_InspectorPanelSystem;
	}

	private void UpdatePrefabs()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabsDirty = false;
		if (m_CategoryMenu.GetSelectedItem(out var selection))
		{
			HashSet<PrefabBase> prefabs = selection.GetPrefabs(((ComponentSystemBase)this).EntityManager, m_PrefabSystem, InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef));
			m_Adapter.SetPrefabs(prefabs);
		}
	}

	private void GenerateCategories()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "All",
			path = "All"
		};
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabData>() };
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshData>() };
		array[0] = val;
		editorAssetCategory.entityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		editorAssetCategory.defaultSelection = true;
		editorAssetCategory.includeChildCategories = false;
		m_AllCategory = editorAssetCategory;
	}

	private IEnumerable<HierarchyItem<EditorAssetCategory>> GetHierarchy()
	{
		yield return m_AllCategory.ToHierarchyItem();
		foreach (HierarchyItem<EditorAssetCategory> item in m_CategorySystem.GetHierarchy())
		{
			yield return item;
		}
	}

	private void OnCreatePrefabSelected()
	{
		base.activeSubPanel = new TypePickerPanel("Editor.CREATE_NEW_PREFAB", "Editor.PREFAB_TYPES", GetPrefabTypeItems().ToList(), OnCreatePrefab, base.CloseSubPanel);
	}

	private void OnCreatePrefab(Type type)
	{
		CloseSubPanel();
		PrefabBase prefabBase = (PrefabBase)(object)ScriptableObject.CreateInstance(type);
		((Object)prefabBase).name = type.Name;
		m_PrefabSystem.AddPrefab(prefabBase);
		OnPrefabSelected(prefabBase);
	}

	private IEnumerable<Item> GetPrefabTypeItems()
	{
		foreach (Type item in TypePickerPanel.GetAllConcreteTypesDerivedFrom<PrefabBase>())
		{
			ComponentMenu customAttribute = item.GetCustomAttribute<ComponentMenu>();
			yield return new Item
			{
				type = item,
				name = WidgetReflectionUtils.NicifyVariableName(item.Name),
				parentDir = customAttribute?.menu
			};
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public PrefabEdítorPanelSystem()
	{
	}
}
