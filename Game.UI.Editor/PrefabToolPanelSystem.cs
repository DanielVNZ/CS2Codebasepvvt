using System;
using System.Collections.Generic;
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
public class PrefabToolPanelSystem : EditorPanelSystemBase
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

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_ModifiedPrefabQuery;

	private PrefabPickerAdapter m_Adapter;

	private HierarchyMenu<EditorAssetCategory> m_CategoryMenu;

	private EditorAssetCategory m_AllCategory;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CategorySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EditorAssetCategorySystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabData>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		array[0] = val;
		m_ModifiedPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_Adapter = new PrefabPickerAdapter();
		PrefabPickerAdapter prefabPickerAdapter = m_Adapter;
		prefabPickerAdapter.EventPrefabSelected = (Action<PrefabBase>)Delegate.Combine(prefabPickerAdapter.EventPrefabSelected, new Action<PrefabBase>(OnPrefabSelected));
		title = "Editor.TOOL[PrefabTool]";
		IWidget[] obj = new IWidget[4]
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
			path = "PrefabToolCategories"
		};
		HierarchyMenu<EditorAssetCategory> hierarchyMenu = obj2;
		m_CategoryMenu = obj2;
		array2[0] = hierarchyMenu;
		array2[1] = new ItemPicker<PrefabItem>
		{
			adapter = m_Adapter,
			hasFavorites = true,
			flex = new FlexLayout(2f, 0f, 0),
			selectOnFocus = true
		};
		row.children = array2;
		obj[2] = row;
		obj[3] = new ItemPickerFooter
		{
			adapter = m_Adapter
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
		m_ToolSystem.ActivatePrefabTool(m_Adapter.selectedPrefab);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (!((EntityQuery)(ref m_ModifiedPrefabQuery)).IsEmptyIgnoreFilter)
		{
			UpdatePrefabs();
		}
		m_Adapter.Update();
		m_Adapter.selectedPrefab = m_ToolSystem.activePrefab;
	}

	protected override bool OnCancel()
	{
		if ((Object)(object)m_Adapter.selectedPrefab != (Object)null)
		{
			m_Adapter.selectedPrefab = null;
			m_ToolSystem.ActivatePrefabTool(null);
			return false;
		}
		return base.OnCancel();
	}

	private void OnPrefabSelected(PrefabBase prefab)
	{
		m_ToolSystem.ActivatePrefabTool(m_Adapter.selectedPrefab);
	}

	private void UpdatePrefabs()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "All",
			path = "All"
		};
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabData>() };
		val.Any = (ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<EffectData>(),
			ComponentType.ReadOnly<ActivityLocationData>(),
			ComponentType.ReadOnly<NetData>(),
			ComponentType.ReadOnly<NetLaneData>(),
			ComponentType.ReadOnly<AreaData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<BrandObjectData>(),
			ComponentType.ReadOnly<CarLaneData>(),
			ComponentType.ReadOnly<TrackLaneData>(),
			ComponentType.ReadOnly<ConnectionLaneData>()
		};
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
	public PrefabToolPanelSystem()
	{
	}
}
