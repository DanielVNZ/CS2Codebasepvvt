using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.City;
using Game.Input;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Serialization;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ToolbarUISystem : UISystemBase, IPreDeserialize
{
	private enum ToolbarItemType
	{
		Asset,
		Menu
	}

	private const string kGroup = "toolbar";

	private PrefabSystem m_PrefabSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ToolSystem m_ToolSystem;

	private ObjectToolSystem m_ObjectToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private UniqueAssetTrackingSystem m_UniqueAssetTrackingSystem;

	private PrefabUISystem m_PrefabUISystem;

	private ImageSystem m_ImageSystem;

	private UpgradeMenuUISystem m_UpgradeMenuUISystem;

	private ActionsSection m_ActionsSection;

	private EntityQuery m_ThemeQuery;

	private EntityQuery m_AssetPackQuery;

	private EntityQuery m_ToolbarGroupQuery;

	private EntityQuery m_UnlockedPrefabQuery;

	private RawValueBinding m_ToolbarGroupsBinding;

	private RawMapBinding<Entity> m_AssetMenuCategoriesBinding;

	private RawMapBinding<Entity> m_AssetsBinding;

	private RawValueBinding m_ThemesBinding;

	private RawValueBinding m_AssetPacksBinding;

	private ValueBinding<int> m_AgeMaskBinding;

	private GetterValueBinding<List<Entity>> m_SelectedThemesBinding;

	private GetterValueBinding<List<Entity>> m_SelectedAssetPacksBinding;

	private ValueBinding<Entity> m_SelectedAssetMenuBinding;

	private ValueBinding<Entity> m_SelectedAssetCategoryBinding;

	private ValueBinding<Entity> m_SelectedAssetBinding;

	private Dictionary<Entity, Entity> m_LastSelectedCategories;

	private Dictionary<Entity, Entity> m_LastSelectedAssets;

	private List<Entity> m_SelectedThemes;

	private List<Entity> m_SelectedAssetPacks;

	private bool m_UniqueAssetStatusChanged;

	private bool m_HasUnlockedPrefabLastFrame;

	public bool hasActiveSelection
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (!(m_SelectedAssetMenuBinding.value != Entity.Null))
			{
				return m_SelectedAssetBinding.value != Entity.Null;
			}
			return true;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_01a3: Expected O, but got Unknown
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Expected O, but got Unknown
		//IL_0225: Expected O, but got Unknown
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Expected O, but got Unknown
		//IL_024f: Expected O, but got Unknown
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_UniqueAssetTrackingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UniqueAssetTrackingSystem>();
		UniqueAssetTrackingSystem uniqueAssetTrackingSystem = m_UniqueAssetTrackingSystem;
		uniqueAssetTrackingSystem.EventUniqueAssetStatusChanged = (Action<Entity, bool>)Delegate.Combine(uniqueAssetTrackingSystem.EventUniqueAssetStatusChanged, new Action<Entity, bool>(OnUniqueAssetStatusChanged));
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		m_UpgradeMenuUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpgradeMenuUISystem>();
		m_ActionsSection = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ActionsSection>();
		m_ThemeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ThemeData>()
		});
		m_AssetPackQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<AssetPackData>()
		});
		m_ToolbarGroupQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<UIGroupElement>(),
			ComponentType.ReadOnly<UIToolbarGroupData>()
		});
		m_UnlockedPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		RawValueBinding val = new RawValueBinding("toolbar", "toolbarGroups", (Action<IJsonWriter>)BindToolbarGroups);
		RawValueBinding binding = val;
		m_ToolbarGroupsBinding = val;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_AssetMenuCategoriesBinding = new RawMapBinding<Entity>("toolbar", "assetCategories", (Action<IJsonWriter, Entity>)BindAssetCategories, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_AssetsBinding = new RawMapBinding<Entity>("toolbar", "assets", (Action<IJsonWriter, Entity>)BindAssets, (IReader<Entity>)null, (IWriter<Entity>)null)));
		RawValueBinding val2 = new RawValueBinding("toolbar", "themes", (Action<IJsonWriter>)BindThemes);
		binding = val2;
		m_ThemesBinding = val2;
		AddBinding((IBinding)(object)binding);
		RawValueBinding val3 = new RawValueBinding("toolbar", "assetPacks", (Action<IJsonWriter>)BindPacks);
		binding = val3;
		m_AssetPacksBinding = val3;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_AgeMaskBinding = new ValueBinding<int>("toolbar", "ageMask", (int)(m_ObjectToolSystem.allowAge ? m_ObjectToolSystem.actualAgeMask : ((Game.Tools.AgeMask)0)), (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_SelectedThemesBinding = new GetterValueBinding<List<Entity>>("toolbar", "selectedThemes", (Func<List<Entity>>)(() => m_SelectedThemes), (IWriter<List<Entity>>)(object)new ListWriter<Entity>((IWriter<Entity>)null), (EqualityComparer<List<Entity>>)null)));
		AddBinding((IBinding)(object)(m_SelectedAssetPacksBinding = new GetterValueBinding<List<Entity>>("toolbar", "selectedAssetPacks", (Func<List<Entity>>)(() => m_SelectedAssetPacks), (IWriter<List<Entity>>)(object)new ListWriter<Entity>((IWriter<Entity>)null), (EqualityComparer<List<Entity>>)null)));
		AddBinding((IBinding)(object)(m_SelectedAssetMenuBinding = new ValueBinding<Entity>("toolbar", "selectedAssetMenu", Entity.Null, (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)(m_SelectedAssetCategoryBinding = new ValueBinding<Entity>("toolbar", "selectedAssetCategory", Entity.Null, (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)(m_SelectedAssetBinding = new ValueBinding<Entity>("toolbar", "selectedAsset", Entity.Null, (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<List<Entity>>("toolbar", "setSelectedThemes", (Action<List<Entity>>)SetSelectedThemes, (IReader<List<Entity>>)(object)new ListReader<Entity>((IReader<Entity>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<List<Entity>>("toolbar", "setSelectedAssetPacks", (Action<List<Entity>>)SetSelectedAssetPacks, (IReader<List<Entity>>)(object)new ListReader<Entity>((IReader<Entity>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("toolbar", "selectAssetMenu", (Action<Entity>)SelectAssetMenu, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("toolbar", "selectAssetCategory", (Action<Entity>)SelectAssetCategory, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, bool>("toolbar", "selectAsset", (Action<Entity, bool>)SelectAsset, (IReader<Entity>)null, (IReader<bool>)null));
		AddBinding((IBinding)new TriggerBinding("toolbar", "clearAssetSelection", (Action)ClearAssetSelection));
		AddBinding((IBinding)(object)new TriggerBinding<bool>("toolbar", "toggleToolOptions", (Action<bool>)ToggleToolOptions, (IReader<bool>)null));
		AddBinding((IBinding)(object)new TriggerBinding<int>("toolbar", "setAgeMask", (Action<int>)SetAgeMask, (IReader<int>)null));
		m_LastSelectedCategories = new Dictionary<Entity, Entity>();
		m_LastSelectedAssets = new Dictionary<Entity, Entity>();
		m_SelectedThemes = new List<Entity>();
		m_SelectedAssetPacks = new List<Entity>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		UniqueAssetTrackingSystem uniqueAssetTrackingSystem = m_UniqueAssetTrackingSystem;
		uniqueAssetTrackingSystem.EventUniqueAssetStatusChanged = (Action<Entity, bool>)Delegate.Remove(uniqueAssetTrackingSystem.EventUniqueAssetStatusChanged, new Action<Entity, bool>(OnUniqueAssetStatusChanged));
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		PrefabBase activePrefab = m_ToolSystem.activePrefab;
		Entity value = m_SelectedAssetBinding.value;
		Entity value2 = m_SelectedAssetMenuBinding.value;
		if ((Object)(object)activePrefab != (Object)null)
		{
			Entity entity = m_PrefabSystem.GetEntity(activePrefab);
			if (entity != value && !(m_ToolSystem.activeTool is AreaToolSystem) && !m_UpgradeMenuUISystem.upgrading && !m_ActionsSection.editingLot)
			{
				SelectAsset(entity, updateTool: false);
			}
		}
		else if (m_ToolSystem.activeTool != m_DefaultTool || (value2 == Entity.Null && value != Entity.Null))
		{
			bool updateTool = m_ToolSystem.activeTool is DefaultToolSystem;
			ClearAssetSelection(updateTool);
		}
		if (m_HasUnlockedPrefabLastFrame)
		{
			m_ToolbarGroupsBinding.Update();
			m_HasUnlockedPrefabLastFrame = false;
		}
		if (PrefabUtils.HasUnlockedPrefab<UIObjectData>(((ComponentSystemBase)this).EntityManager, m_UnlockedPrefabQuery) || m_UniqueAssetStatusChanged)
		{
			if (value != Entity.Null && value2 != Entity.Null && (Object)(object)activePrefab == (Object)null && InputManager.instance.activeControlScheme != InputManager.ControlScheme.Gamepad)
			{
				ActivatePrefabTool(value);
			}
			m_ToolbarGroupsBinding.Update();
			((MapBindingBase<Entity>)(object)m_AssetMenuCategoriesBinding).UpdateAll();
			((MapBindingBase<Entity>)(object)m_AssetsBinding).UpdateAll();
			m_ThemesBinding.Update();
			m_AssetPacksBinding.Update();
			m_HasUnlockedPrefabLastFrame = true;
		}
		m_UniqueAssetStatusChanged = false;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		m_SelectedThemes.Clear();
		m_SelectedAssetPacks.Clear();
		m_SelectedThemes.Add(m_CityConfigurationSystem.defaultTheme);
		m_LastSelectedCategories.Clear();
		m_LastSelectedAssets.Clear();
		Apply(m_SelectedThemes, m_SelectedAssetPacks, Entity.Null, Entity.Null, Entity.Null);
		m_ToolbarGroupsBinding.Update();
	}

	private void OnUniqueAssetStatusChanged(Entity prefabEntity, bool placed)
	{
		m_UniqueAssetStatusChanged = true;
	}

	public void PreDeserialize(Context context)
	{
		ClearAssetSelection();
		m_LastSelectedAssets.Clear();
		m_LastSelectedCategories.Clear();
	}

	private void BindToolbarGroups(IJsonWriter writer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<UIObjectInfo> sortedToolbarGroups = GetSortedToolbarGroups();
		JsonWriterExtensions.ArrayBegin(writer, sortedToolbarGroups.Length);
		for (int i = 0; i < sortedToolbarGroups.Length; i++)
		{
			UIObjectInfo uIObjectInfo = sortedToolbarGroups[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
			NativeList<UIObjectInfo> objects = UIObjectInfo.GetObjects(entityManager, ((EntityManager)(ref entityManager2)).GetBuffer<UIGroupElement>(uIObjectInfo.entity, true), (Allocator)3);
			NativeSortExtension.Sort<UIObjectInfo>(objects);
			writer.TypeBegin("toolbar.ToolbarGroup");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, uIObjectInfo.entity);
			writer.PropertyName("children");
			JsonWriterExtensions.ArrayBegin(writer, objects.Length);
			for (int j = 0; j < objects.Length; j++)
			{
				Entity entity = objects[j].entity;
				PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(objects[j].prefabData);
				entityManager2 = ((ComponentSystemBase)this).EntityManager;
				bool flag = ((EntityManager)(ref entityManager2)).HasComponent<UIAssetMenuData>(entity);
				writer.TypeBegin("toolbar.ToolbarItem");
				writer.PropertyName("entity");
				UnityWriters.Write(writer, entity);
				writer.PropertyName("name");
				writer.Write(((Object)prefab).name);
				writer.PropertyName("type");
				writer.Write(flag ? 1 : 0);
				writer.PropertyName("icon");
				writer.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
				writer.PropertyName("locked");
				writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity));
				writer.PropertyName("uiTag");
				writer.Write(prefab.uiTag);
				writer.PropertyName("requirements");
				m_PrefabUISystem.BindPrefabRequirements(writer, entity);
				writer.PropertyName("highlight");
				entityManager2 = ((ComponentSystemBase)this).EntityManager;
				writer.Write(((EntityManager)(ref entityManager2)).HasComponent<UIHighlight>(entity));
				writer.PropertyName("selectSound");
				writer.Write((prefab is BulldozePrefab) ? "bulldoze" : null);
				writer.PropertyName("deselectSound");
				writer.Write((prefab is BulldozePrefab) ? "bulldoze-end" : null);
				writer.PropertyName("shortcut");
				writer.Write(prefab.TryGet<UIShortcut>(out var component) ? component.m_Action.m_AliasName : null);
				writer.TypeEnd();
			}
			writer.ArrayEnd();
			writer.TypeEnd();
			objects.Dispose();
		}
		writer.ArrayEnd();
		sortedToolbarGroups.Dispose();
	}

	private void BindAssetCategories(IJsonWriter writer, Entity assetMenu)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<UIGroupElement> elements = default(DynamicBuffer<UIGroupElement>);
		if (((EntityManager)(ref entityManager)).HasComponent<UIAssetMenuData>(assetMenu) && EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, assetMenu, true, ref elements))
		{
			NativeList<UIObjectInfo> sortedCategories = GetSortedCategories(elements);
			JsonWriterExtensions.ArrayBegin(writer, sortedCategories.Length);
			for (int i = 0; i < sortedCategories.Length; i++)
			{
				Entity entity = sortedCategories[i].entity;
				PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(sortedCategories[i].prefabData);
				writer.TypeBegin("toolbar.AssetCategory");
				writer.PropertyName("entity");
				UnityWriters.Write(writer, entity);
				writer.PropertyName("name");
				writer.Write(((Object)prefab).name);
				writer.PropertyName("icon");
				writer.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
				writer.PropertyName("locked");
				writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity));
				writer.PropertyName("uiTag");
				writer.Write(prefab.uiTag);
				writer.PropertyName("highlight");
				entityManager = ((ComponentSystemBase)this).EntityManager;
				writer.Write(((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(entity));
				writer.TypeEnd();
			}
			writer.ArrayEnd();
			sortedCategories.Dispose();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
	}

	private NativeList<UIObjectInfo> GetSortedCategories(DynamicBuffer<UIGroupElement> elements)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		NativeList<UIObjectInfo> objects = UIObjectInfo.GetObjects(((ComponentSystemBase)this).EntityManager, elements, (Allocator)3);
		DynamicBuffer<UIGroupElement> val = default(DynamicBuffer<UIGroupElement>);
		for (int num = objects.Length - 1; num >= 0; num--)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<UIAssetCategoryData>(objects[num].entity) || !EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, objects[num].entity, true, ref val) || val.Length == 0)
			{
				objects.RemoveAtSwapBack(num);
			}
		}
		NativeSortExtension.Sort<UIObjectInfo>(objects);
		return objects;
	}

	private void BindAssets(IJsonWriter writer, Entity assetCategory)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<UIGroupElement> elements = default(DynamicBuffer<UIGroupElement>);
		if (((EntityManager)(ref entityManager)).HasComponent<UIAssetCategoryData>(assetCategory) && EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, assetCategory, true, ref elements))
		{
			NativeList<UIObjectInfo> objects = UIObjectInfo.GetObjects(((ComponentSystemBase)this).EntityManager, elements, (Allocator)3);
			FilterByThemes(objects, m_SelectedThemes);
			FilterByPacks(objects, m_SelectedAssetPacks);
			FilterOutUpgrades(objects);
			NativeSortExtension.Sort<UIObjectInfo>(objects);
			JsonWriterExtensions.ArrayBegin(writer, objects.Length);
			for (int i = 0; i < objects.Length; i++)
			{
				Entity entity = objects[i].entity;
				bool unique = m_UniqueAssetTrackingSystem.IsUniqueAsset(entity);
				bool placed = m_UniqueAssetTrackingSystem.IsPlacedUniqueAsset(entity);
				BindAsset(writer, entity, unique, placed);
			}
			writer.ArrayEnd();
			objects.Dispose();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
	}

	public void BindAsset(IJsonWriter writer, Entity entity, bool unique = false, bool placed = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(entity);
		string text = null;
		if (prefab.TryGet<ContentPrerequisite>(out var component) && component.m_ContentPrerequisite.TryGet<DlcRequirement>(out var component2))
		{
			text = PlatformManager.instance.GetDlcName(component2.m_Dlc);
		}
		string text2 = null;
		DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
		EntityManager entityManager;
		if (EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity requirement = val[i].m_Requirement;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<ThemeData>(requirement))
				{
					text2 = ImageSystem.GetIcon(m_PrefabSystem.GetPrefab<PrefabBase>(requirement)) ?? m_ImageSystem.placeholderIcon;
				}
			}
		}
		int num = 0;
		UIObjectData uIObjectData = default(UIObjectData);
		if (EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, entity, ref uIObjectData))
		{
			num = uIObjectData.m_Priority;
		}
		writer.TypeBegin("toolbar.Asset");
		writer.PropertyName("entity");
		UnityWriters.Write(writer, entity);
		writer.PropertyName("name");
		writer.Write(((Object)prefab).name);
		writer.PropertyName("priority");
		writer.Write(num);
		writer.PropertyName("icon");
		writer.Write(ImageSystem.GetThumbnail(prefab) ?? m_ImageSystem.placeholderIcon);
		writer.PropertyName("dlc");
		if (text != null)
		{
			writer.Write("Media/DLC/" + text + ".svg");
		}
		else
		{
			writer.WriteNull();
		}
		writer.PropertyName("theme");
		if (text2 != null)
		{
			writer.Write(text2);
		}
		else
		{
			writer.WriteNull();
		}
		writer.PropertyName("locked");
		writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity));
		writer.PropertyName("uiTag");
		writer.Write(prefab.uiTag);
		writer.PropertyName("highlight");
		entityManager = ((ComponentSystemBase)this).EntityManager;
		writer.Write(((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(entity));
		writer.PropertyName("unique");
		writer.Write(unique);
		writer.PropertyName("placed");
		writer.Write(placed);
		writer.PropertyName("constructionCost");
		m_PrefabUISystem.BindConstructionCost(writer, entity);
		writer.TypeEnd();
	}

	private void BindThemes(IJsonWriter writer)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode.IsEditor())
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
			return;
		}
		Entity value = m_SelectedAssetCategoryBinding.value;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<UIGroupElement> val = default(DynamicBuffer<UIGroupElement>);
		if (((EntityManager)(ref entityManager)).HasComponent<UIAssetCategoryData>(value) && EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, value, true, ref val))
		{
			NativeParallelHashMap<Entity, bool> val2 = default(NativeParallelHashMap<Entity, bool>);
			val2._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
			DynamicBuffer<ObjectRequirementElement> val3 = default(DynamicBuffer<ObjectRequirementElement>);
			bool flag2 = default(bool);
			for (int num = val.Length - 1; num >= 0; num--)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				bool flag = ((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(val[num].m_Prefab);
				if (EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, val[num].m_Prefab, true, ref val3))
				{
					for (int i = 0; i < val3.Length; i++)
					{
						Entity requirement = val3[i].m_Requirement;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<ThemeData>(requirement))
						{
							if (val2.TryGetValue(requirement, ref flag2))
							{
								val2[requirement] = flag || flag2;
							}
							else
							{
								val2[requirement] = flag;
							}
						}
					}
				}
			}
			if (!val2.IsEmpty)
			{
				NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(((ComponentSystemBase)this).EntityManager, m_ThemeQuery, (Allocator)3);
				JsonWriterExtensions.ArrayBegin(writer, sortedObjects.Length);
				bool flag3 = default(bool);
				for (int j = 0; j < sortedObjects.Length; j++)
				{
					ThemePrefab prefab = m_PrefabSystem.GetPrefab<ThemePrefab>(sortedObjects[j].prefabData);
					writer.TypeBegin("toolbar.Theme");
					writer.PropertyName("entity");
					UnityWriters.Write(writer, sortedObjects[j].entity);
					writer.PropertyName("name");
					writer.Write(((Object)prefab).name);
					writer.PropertyName("icon");
					writer.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
					writer.PropertyName("highlight");
					writer.Write(val2.TryGetValue(sortedObjects[j].entity, ref flag3) && flag3);
					writer.TypeEnd();
				}
				writer.ArrayEnd();
				sortedObjects.Dispose();
			}
			else
			{
				JsonWriterExtensions.WriteEmptyArray(writer);
			}
			val2.Dispose();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
	}

	private void SetSelectedThemes(List<Entity> themes)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Entity value = m_SelectedAssetMenuBinding.value;
		Entity value2 = m_SelectedAssetCategoryBinding.value;
		Entity val = m_SelectedAssetBinding.value;
		List<Entity> packs = m_SelectedAssetPacks;
		if (value2 != Entity.Null)
		{
			val = ((!(val != Entity.Null)) ? GetFirstUnlockedItem(value2, themes, packs) : GetClosestAssetInThemes(val, value2, themes));
		}
		else if (!IsMatchingTheme(val, themes))
		{
			val = Entity.Null;
		}
		Apply(themes, packs, value, value2, val, InputManager.instance.activeControlScheme != InputManager.ControlScheme.Gamepad);
	}

	private void SetAgeMask(int ageMask)
	{
		m_ObjectToolSystem.ageMask = (Game.Tools.AgeMask)ageMask;
		m_AgeMaskBinding.Update((int)((m_ToolSystem.activeTool == m_ObjectToolSystem && m_ObjectToolSystem.allowAge) ? m_ObjectToolSystem.actualAgeMask : ((Game.Tools.AgeMask)0)));
	}

	private void SetSelectedAssetPacks(List<Entity> packs)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Entity value = m_SelectedAssetMenuBinding.value;
		Entity value2 = m_SelectedAssetCategoryBinding.value;
		Entity val = m_SelectedAssetBinding.value;
		List<Entity> themes = m_SelectedThemes;
		if (value2 != Entity.Null)
		{
			val = ((!(val != Entity.Null)) ? GetFirstUnlockedItem(value2, themes, packs) : GetClosestAssetInPacks(val, value2, packs));
		}
		else if (!IsMatchingPack(val, packs))
		{
			val = Entity.Null;
		}
		Apply(themes, packs, value, value2, val, InputManager.instance.activeControlScheme != InputManager.ControlScheme.Gamepad);
	}

	private void BindPacks(IJsonWriter writer)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode.IsEditor())
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
			return;
		}
		Entity value = m_SelectedAssetCategoryBinding.value;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<UIGroupElement> val = default(DynamicBuffer<UIGroupElement>);
		if (((EntityManager)(ref entityManager)).HasComponent<UIAssetCategoryData>(value) && EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, value, true, ref val))
		{
			NativeParallelHashMap<Entity, bool> val2 = default(NativeParallelHashMap<Entity, bool>);
			val2._002Ector(10, AllocatorHandle.op_Implicit((Allocator)3));
			DynamicBuffer<AssetPackElement> val3 = default(DynamicBuffer<AssetPackElement>);
			bool flag2 = default(bool);
			for (int num = val.Length - 1; num >= 0; num--)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				bool flag = ((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(val[num].m_Prefab);
				if (EntitiesExtensions.TryGetBuffer<AssetPackElement>(((ComponentSystemBase)this).EntityManager, val[num].m_Prefab, true, ref val3))
				{
					for (int i = 0; i < val3.Length; i++)
					{
						Entity pack = val3[i].m_Pack;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<AssetPackData>(pack))
						{
							if (val2.TryGetValue(pack, ref flag2))
							{
								val2[pack] = flag || flag2;
							}
							else
							{
								val2[pack] = flag;
							}
						}
					}
				}
			}
			if (!val2.IsEmpty)
			{
				NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(((ComponentSystemBase)this).EntityManager, m_AssetPackQuery, (Allocator)3);
				for (int num2 = sortedObjects.Length - 1; num2 >= 0; num2--)
				{
					if (!val2.ContainsKey(sortedObjects[num2].entity))
					{
						sortedObjects.RemoveAt(num2);
					}
				}
				JsonWriterExtensions.ArrayBegin(writer, sortedObjects.Length);
				bool flag3 = default(bool);
				for (int j = 0; j < sortedObjects.Length; j++)
				{
					AssetPackPrefab prefab = m_PrefabSystem.GetPrefab<AssetPackPrefab>(sortedObjects[j].prefabData);
					writer.TypeBegin("toolbar.AssetPack");
					writer.PropertyName("entity");
					UnityWriters.Write(writer, sortedObjects[j].entity);
					writer.PropertyName("name");
					writer.Write(((Object)prefab).name);
					writer.PropertyName("icon");
					writer.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
					writer.PropertyName("highlight");
					writer.Write(val2.TryGetValue(sortedObjects[j].entity, ref flag3) && flag3);
					writer.TypeEnd();
				}
				writer.ArrayEnd();
				sortedObjects.Dispose();
			}
			else
			{
				JsonWriterExtensions.WriteEmptyArray(writer);
			}
			val2.Dispose();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
		}
	}

	private void SelectAssetMenu(Entity assetMenu)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		m_SelectedAssetPacks = new List<Entity>();
		List<Entity> themes = m_SelectedThemes;
		List<Entity> packs = m_SelectedAssetPacks;
		if (!(assetMenu != Entity.Null))
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<UIAssetMenuData>(assetMenu))
		{
			return;
		}
		if (!m_LastSelectedCategories.TryGetValue(assetMenu, out var value))
		{
			value = GetFirstItem(assetMenu, themes, packs);
		}
		Entity value2 = Entity.Null;
		if (value != Entity.Null)
		{
			if (m_LastSelectedAssets.TryGetValue(value, out value2))
			{
				value2 = GetClosestAssetInThemes(value2, value, themes);
				value2 = GetClosestAssetInPacks(value2, value, packs);
			}
			else
			{
				value2 = GetFirstUnlockedItem(value, themes, packs);
			}
		}
		Apply(themes, packs, assetMenu, value, value2, InputManager.instance.activeControlScheme != InputManager.ControlScheme.Gamepad);
	}

	private void SelectAssetCategory(Entity assetCategory)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		UIAssetCategoryData uIAssetCategoryData = default(UIAssetCategoryData);
		if (assetCategory != Entity.Null && EntitiesExtensions.TryGetComponent<UIAssetCategoryData>(((ComponentSystemBase)this).EntityManager, assetCategory, ref uIAssetCategoryData))
		{
			m_SelectedAssetPacks = new List<Entity>();
			Entity menu = uIAssetCategoryData.m_Menu;
			List<Entity> themes = m_SelectedThemes;
			List<Entity> list = m_SelectedAssetPacks;
			if (m_LastSelectedAssets.TryGetValue(assetCategory, out var value))
			{
				value = GetClosestAssetInThemes(value, assetCategory, themes);
				value = GetClosestAssetInPacks(value, assetCategory, list);
			}
			else
			{
				value = GetFirstUnlockedItem(assetCategory, themes, list);
			}
			if (value == Entity.Null)
			{
				list.Clear();
				value = GetFirstUnlockedItem(assetCategory, themes, list);
			}
			Apply(themes, list, menu, assetCategory, value, InputManager.instance.activeControlScheme != InputManager.ControlScheme.Gamepad);
		}
	}

	private void SelectAsset(Entity assetEntity, bool updateTool)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		List<Entity> themes = m_SelectedThemes;
		List<Entity> packs = m_SelectedAssetPacks;
		if (!IsMatchingTheme(assetEntity, themes))
		{
			NativeList<UIObjectInfo> sortedObjects = UIObjectInfo.GetSortedObjects(((ComponentSystemBase)this).EntityManager, m_ThemeQuery, (Allocator)3);
			themes = FilterThemesByAsset(sortedObjects, assetEntity);
			sortedObjects.Dispose();
		}
		if (!IsMatchingPack(assetEntity, packs))
		{
			NativeList<UIObjectInfo> sortedObjects2 = UIObjectInfo.GetSortedObjects(((ComponentSystemBase)this).EntityManager, m_AssetPackQuery, (Allocator)3);
			packs = FilterPacksByAsset(sortedObjects2, assetEntity);
			sortedObjects2.Dispose();
		}
		Entity assetMenuEntity = m_SelectedAssetMenuBinding.value;
		Entity assetCategoryEntity = m_SelectedAssetCategoryBinding.value;
		UIObjectData uIObjectData = default(UIObjectData);
		if (!IsMatchingAssetCategory(assetEntity, m_SelectedAssetCategoryBinding.value) && assetEntity != Entity.Null && EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, assetEntity, ref uIObjectData))
		{
			UIAssetCategoryData uIAssetCategoryData = default(UIAssetCategoryData);
			if (uIObjectData.m_Group != Entity.Null && EntitiesExtensions.TryGetComponent<UIAssetCategoryData>(((ComponentSystemBase)this).EntityManager, uIObjectData.m_Group, ref uIAssetCategoryData))
			{
				assetMenuEntity = uIAssetCategoryData.m_Menu;
				assetCategoryEntity = uIObjectData.m_Group;
			}
			else
			{
				assetMenuEntity = Entity.Null;
				assetCategoryEntity = Entity.Null;
			}
		}
		Apply(themes, packs, assetMenuEntity, assetCategoryEntity, assetEntity, updateTool);
	}

	public void ClearAssetSelection()
	{
		ClearAssetSelection(updateTool: true);
	}

	private void ClearAssetSelection(bool updateTool)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Apply(m_SelectedThemes, m_SelectedAssetPacks, Entity.Null, Entity.Null, Entity.Null, updateTool);
	}

	private void Apply(List<Entity> themes, List<Entity> packs, Entity assetMenuEntity, Entity assetCategoryEntity, Entity assetEntity, bool updateTool = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (updateTool)
		{
			ActivatePrefabTool(assetEntity);
		}
		UpdateHighlights(themes, packs, assetMenuEntity, assetCategoryEntity, assetEntity);
		List<Entity> first = m_SelectedThemes;
		List<Entity> first2 = m_SelectedAssetPacks;
		Entity value = m_SelectedAssetCategoryBinding.value;
		m_SelectedThemes = themes;
		m_SelectedThemesBinding.Update();
		m_SelectedAssetPacks = packs;
		m_SelectedAssetPacksBinding.Update();
		m_SelectedAssetMenuBinding.Update(assetMenuEntity);
		m_SelectedAssetCategoryBinding.Update(assetCategoryEntity);
		if (updateTool)
		{
			m_SelectedAssetBinding.Update(assetEntity);
		}
		if (assetMenuEntity != Entity.Null && assetCategoryEntity != Entity.Null)
		{
			m_LastSelectedCategories[assetMenuEntity] = assetCategoryEntity;
		}
		if (assetCategoryEntity != Entity.Null && assetEntity != Entity.Null)
		{
			m_LastSelectedAssets[assetCategoryEntity] = assetEntity;
		}
		if (!first.SequenceEqual(themes) || !first2.SequenceEqual(packs))
		{
			((MapBindingBase<Entity>)(object)m_AssetsBinding).UpdateAll();
		}
		if (!first.SequenceEqual(themes) || value != assetCategoryEntity)
		{
			m_ThemesBinding.Update();
		}
		if (!first2.SequenceEqual(packs) || value != assetCategoryEntity)
		{
			m_AssetPacksBinding.Update();
		}
		if (m_ToolSystem.activeTool == m_ObjectToolSystem && m_ObjectToolSystem.allowAge)
		{
			m_AgeMaskBinding.Update((int)m_ObjectToolSystem.actualAgeMask);
		}
		else
		{
			m_AgeMaskBinding.Update(0);
		}
	}

	private void UpdateHighlights(List<Entity> themes, List<Entity> packs, Entity assetMenuEntity, Entity assetCategoryEntity, Entity assetEntity)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		List<Entity> list = m_SelectedThemes;
		List<Entity> list2 = m_SelectedAssetPacks;
		Entity value = m_SelectedAssetMenuBinding.value;
		Entity value2 = m_SelectedAssetCategoryBinding.value;
		Entity value3 = m_SelectedAssetBinding.value;
		EntityManager entityManager;
		if ((value2 != Entity.Null && ((list.Count > 0 && !list.SequenceEqual(themes)) || (list2.Count > 0 && !list2.SequenceEqual(packs)))) || value2 != assetCategoryEntity)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			bool flag = ((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(value2);
			DynamicBuffer<UIGroupElement> val = default(DynamicBuffer<UIGroupElement>);
			if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, value2, true, ref val))
			{
				NativeList<Entity> val2 = default(NativeList<Entity>);
				val2._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)3));
				for (int i = 0; i < val.Length; i++)
				{
					Entity prefab = val[i].m_Prefab;
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(prefab))
					{
						if (IsMatchingTheme(prefab, list) || IsMatchingPack(prefab, list2))
						{
							val2.Add(ref prefab);
						}
						else
						{
							flag = false;
						}
					}
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).RemoveComponent<UIHighlight>(val2.AsArray());
				val2.Dispose();
			}
			if (flag)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).RemoveComponent<UIHighlight>(value2);
				((MapBindingBase<Entity>)(object)m_AssetMenuCategoriesBinding).UpdateAll();
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(value))
			{
				bool flag2 = true;
				DynamicBuffer<UIGroupElement> val3 = default(DynamicBuffer<UIGroupElement>);
				if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, value, true, ref val3))
				{
					for (int j = 0; j < val3.Length; j++)
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(val3[j].m_Prefab))
						{
							flag2 = false;
							break;
						}
					}
				}
				if (flag2)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).RemoveComponent<UIHighlight>(value);
					m_ToolbarGroupsBinding.Update();
				}
			}
		}
		if (value == Entity.Null && value2 == Entity.Null && assetEntity != value3)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<UIHighlight>(value3))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).RemoveComponent<UIHighlight>(value3);
				m_ToolbarGroupsBinding.Update();
			}
		}
	}

	private void ActivatePrefabTool(Entity assetEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (assetEntity != Entity.Null && !EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, assetEntity) && m_PrefabSystem.TryGetPrefab<PrefabBase>(assetEntity, out var prefab))
		{
			m_ToolSystem.ActivatePrefabTool(prefab);
		}
		else
		{
			m_ToolSystem.activeTool = m_DefaultTool;
		}
	}

	private bool IsMatchingTheme(Entity assetEntity, List<Entity> themes)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (assetEntity == Entity.Null)
		{
			return false;
		}
		DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
		if (themes.Count > 0 && EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, assetEntity, true, ref val))
		{
			bool flag = false;
			for (int i = 0; i < val.Length; i++)
			{
				Entity requirement = val[i].m_Requirement;
				for (int j = 0; j < themes.Count; j++)
				{
					if (requirement == themes[j])
					{
						return true;
					}
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<ThemeData>(requirement))
					{
						flag = true;
					}
				}
			}
			return !flag;
		}
		return true;
	}

	private bool IsMatchingPack(Entity assetEntity, List<Entity> packs)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (assetEntity == Entity.Null)
		{
			return false;
		}
		DynamicBuffer<AssetPackElement> val = default(DynamicBuffer<AssetPackElement>);
		if (packs.Count > 0 && EntitiesExtensions.TryGetBuffer<AssetPackElement>(((ComponentSystemBase)this).EntityManager, assetEntity, true, ref val))
		{
			bool flag = false;
			for (int i = 0; i < val.Length; i++)
			{
				Entity pack = val[i].m_Pack;
				for (int j = 0; j < packs.Count; j++)
				{
					if (pack == packs[j])
					{
						return true;
					}
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<AssetPackData>(pack))
					{
						flag = true;
					}
				}
			}
			return !flag;
		}
		return packs.Count == 0;
	}

	private bool IsMatchingAssetCategory(Entity assetEntity, Entity assetCategoryEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		UIObjectData uIObjectData = default(UIObjectData);
		if (assetEntity != Entity.Null && EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, assetEntity, ref uIObjectData))
		{
			return uIObjectData.m_Group == assetCategoryEntity;
		}
		return false;
	}

	private Entity GetFirstTheme(Entity assetEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
		if (assetEntity != Entity.Null && EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, assetEntity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity requirement = val[i].m_Requirement;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<ThemeData>(requirement))
				{
					return requirement;
				}
			}
		}
		return Entity.Null;
	}

	private Entity GetFirstPack(Entity assetEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<AssetPackElement> val = default(DynamicBuffer<AssetPackElement>);
		if (assetEntity != Entity.Null && EntitiesExtensions.TryGetBuffer<AssetPackElement>(((ComponentSystemBase)this).EntityManager, assetEntity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity pack = val[i].m_Pack;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<AssetPackData>(pack))
				{
					return pack;
				}
			}
		}
		return Entity.Null;
	}

	private Entity GetClosestAssetInThemes(Entity oldAssetEntity, Entity groupEntity, List<Entity> themes)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		if (IsMatchingTheme(oldAssetEntity, themes))
		{
			return oldAssetEntity;
		}
		Entity firstTheme = GetFirstTheme(oldAssetEntity);
		DynamicBuffer<UIGroupElement> elements = default(DynamicBuffer<UIGroupElement>);
		if (firstTheme != Entity.Null && EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, groupEntity, true, ref elements))
		{
			for (int i = 0; i < themes.Count; i++)
			{
				Entity val = themes[i];
				PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(oldAssetEntity);
				ThemePrefab prefab2 = m_PrefabSystem.GetPrefab<ThemePrefab>(firstTheme);
				ThemePrefab prefab3 = m_PrefabSystem.GetPrefab<ThemePrefab>(val);
				if (!((Object)prefab).name.StartsWith(prefab2.assetPrefix))
				{
					continue;
				}
				string text = ((Object)prefab).name.Substring(prefab2.assetPrefix.Length);
				string text2 = prefab3.assetPrefix + text;
				NativeList<UIObjectInfo> objects = UIObjectInfo.GetObjects(((ComponentSystemBase)this).EntityManager, elements, (Allocator)3);
				FilterByTheme(objects, val);
				FilterOutUpgrades(objects);
				try
				{
					for (int j = 0; j < objects.Length; j++)
					{
						Entity entity = objects[j].entity;
						if (((Object)m_PrefabSystem.GetPrefab<PrefabBase>(objects[j].prefabData)).name == text2)
						{
							return entity;
						}
					}
					for (int k = 0; k < objects.Length; k++)
					{
						Entity entity2 = objects[k].entity;
						if (((Object)m_PrefabSystem.GetPrefab<PrefabBase>(objects[k].prefabData)).name == text)
						{
							return entity2;
						}
					}
				}
				finally
				{
					objects.Dispose();
				}
			}
		}
		return GetFirstUnlockedItem(groupEntity, themes, m_SelectedAssetPacks);
	}

	private Entity GetClosestAssetInPacks(Entity oldAssetEntity, Entity groupEntity, List<Entity> packs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (IsMatchingPack(oldAssetEntity, packs))
		{
			return oldAssetEntity;
		}
		DynamicBuffer<UIGroupElement> elements = default(DynamicBuffer<UIGroupElement>);
		if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, groupEntity, true, ref elements))
		{
			NativeList<UIObjectInfo> objects = UIObjectInfo.GetObjects(((ComponentSystemBase)this).EntityManager, elements, (Allocator)3);
			FilterByPacks(objects, packs);
			FilterOutUpgrades(objects);
			try
			{
				for (int i = 0; i < objects.Length; i++)
				{
					Entity entity = objects[i].entity;
					if (IsMatchingPack(entity, packs))
					{
						return entity;
					}
				}
			}
			finally
			{
				objects.Dispose();
			}
		}
		return GetFirstUnlockedItem(groupEntity, m_SelectedThemes, packs);
	}

	private Entity GetFirstItem(Entity groupEntity, List<Entity> themes, List<Entity> packs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<UIGroupElement> elements = default(DynamicBuffer<UIGroupElement>);
		if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, groupEntity, true, ref elements))
		{
			NativeList<UIObjectInfo> objects = UIObjectInfo.GetObjects(((ComponentSystemBase)this).EntityManager, elements, (Allocator)3);
			FilterByThemes(objects, themes);
			FilterByPacks(objects, packs);
			FilterOutUpgrades(objects);
			NativeSortExtension.Sort<UIObjectInfo>(objects);
			try
			{
				if (objects.Length > 0)
				{
					return objects[0].entity;
				}
			}
			finally
			{
				objects.Dispose();
			}
		}
		return Entity.Null;
	}

	private Entity GetFirstUnlockedItem(Entity groupEntity, List<Entity> themes, List<Entity> packs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<UIGroupElement> elements = default(DynamicBuffer<UIGroupElement>);
		if (EntitiesExtensions.TryGetBuffer<UIGroupElement>(((ComponentSystemBase)this).EntityManager, groupEntity, true, ref elements))
		{
			NativeList<UIObjectInfo> objects = UIObjectInfo.GetObjects(((ComponentSystemBase)this).EntityManager, elements, (Allocator)3);
			FilterByThemes(objects, themes);
			FilterByPacks(objects, packs);
			FilterOutUpgrades(objects);
			NativeSortExtension.Sort<UIObjectInfo>(objects);
			try
			{
				for (int i = 0; i < objects.Length; i++)
				{
					Entity entity = objects[i].entity;
					if (!EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, entity))
					{
						return entity;
					}
				}
				if (objects.Length > 0)
				{
					return objects[0].entity;
				}
			}
			finally
			{
				objects.Dispose();
			}
		}
		return Entity.Null;
	}

	private NativeArray<UIObjectInfo> GetSortedToolbarGroups()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_ToolbarGroupQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<UIToolbarGroupData> val2 = ((EntityQuery)(ref m_ToolbarGroupQuery)).ToComponentDataArray<UIToolbarGroupData>(AllocatorHandle.op_Implicit((Allocator)3));
		int length = val.Length;
		NativeArray<UIObjectInfo> val3 = default(NativeArray<UIObjectInfo>);
		val3._002Ector(length, (Allocator)2, (NativeArrayOptions)1);
		for (int i = 0; i < length; i++)
		{
			val3[i] = new UIObjectInfo(val[i], val2[i].m_Priority);
		}
		NativeSortExtension.Sort<UIObjectInfo>(val3);
		val.Dispose();
		val2.Dispose();
		return val3;
	}

	private void FilterByTheme(NativeList<UIObjectInfo> elementInfos, Entity themeEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (themeEntity == Entity.Null)
		{
			return;
		}
		DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
		for (int num = elementInfos.Length - 1; num >= 0; num--)
		{
			Entity entity = elementInfos[num].entity;
			if (EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
			{
				bool flag = false;
				for (int i = 0; i < val.Length; i++)
				{
					Entity requirement = val[i].m_Requirement;
					if (requirement == themeEntity)
					{
						flag = false;
						break;
					}
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<ThemeData>(requirement))
					{
						flag = true;
					}
				}
				if (flag)
				{
					elementInfos.RemoveAtSwapBack(num);
				}
			}
		}
	}

	private List<Entity> FilterThemesByAsset(NativeList<UIObjectInfo> themes, Entity asset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (asset == Entity.Null)
		{
			return m_SelectedThemes;
		}
		DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
		for (int num = themes.Length - 1; num >= 0; num--)
		{
			Entity entity = themes[num].entity;
			if (EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, asset, true, ref val))
			{
				bool flag = false;
				for (int i = 0; i < val.Length; i++)
				{
					Entity requirement = val[i].m_Requirement;
					if (requirement == entity)
					{
						flag = false;
						break;
					}
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<ThemeData>(requirement))
					{
						flag = true;
					}
				}
				if (flag)
				{
					themes.RemoveAtSwapBack(num);
				}
			}
		}
		m_SelectedThemes.Clear();
		for (int j = 0; j < themes.Length; j++)
		{
			m_SelectedThemes.Add(themes[j].entity);
		}
		return m_SelectedThemes;
	}

	private List<Entity> FilterPacksByAsset(NativeList<UIObjectInfo> assetPacks, Entity asset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (asset == Entity.Null)
		{
			return m_SelectedAssetPacks;
		}
		DynamicBuffer<AssetPackElement> val = default(DynamicBuffer<AssetPackElement>);
		for (int num = assetPacks.Length - 1; num >= 0; num--)
		{
			Entity entity = assetPacks[num].entity;
			bool flag = true;
			if (EntitiesExtensions.TryGetBuffer<AssetPackElement>(((ComponentSystemBase)this).EntityManager, asset, true, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (val[i].m_Pack == entity)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				assetPacks.RemoveAtSwapBack(num);
			}
		}
		m_SelectedAssetPacks.Clear();
		for (int j = 0; j < assetPacks.Length; j++)
		{
			m_SelectedAssetPacks.Add(assetPacks[j].entity);
		}
		return m_SelectedAssetPacks;
	}

	private void FilterByPack(NativeList<UIObjectInfo> elementInfos, Entity packEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (packEntity == Entity.Null)
		{
			return;
		}
		DynamicBuffer<AssetPackElement> val = default(DynamicBuffer<AssetPackElement>);
		for (int num = elementInfos.Length - 1; num >= 0; num--)
		{
			Entity entity = elementInfos[num].entity;
			bool flag = true;
			if (EntitiesExtensions.TryGetBuffer<AssetPackElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					if (val[i].m_Pack == packEntity)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				elementInfos.RemoveAtSwapBack(num);
			}
		}
	}

	private void FilterByThemes(NativeList<UIObjectInfo> elementInfos, List<Entity> themes)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<ObjectRequirementElement> val = default(DynamicBuffer<ObjectRequirementElement>);
		for (int num = elementInfos.Length - 1; num >= 0; num--)
		{
			Entity entity = elementInfos[num].entity;
			if (EntitiesExtensions.TryGetBuffer<ObjectRequirementElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
			{
				bool flag = false;
				for (int i = 0; i < val.Length; i++)
				{
					Entity requirement = val[i].m_Requirement;
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<ThemeData>(requirement))
					{
						flag = true;
					}
					for (int j = 0; j < themes.Count; j++)
					{
						Entity val2 = themes[j];
						if (requirement == val2)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					elementInfos.RemoveAtSwapBack(num);
				}
			}
		}
	}

	private void FilterByPacks(NativeList<UIObjectInfo> elementInfos, List<Entity> packs)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (packs == null || packs.Count == 0)
		{
			return;
		}
		DynamicBuffer<AssetPackElement> val = default(DynamicBuffer<AssetPackElement>);
		for (int num = elementInfos.Length - 1; num >= 0; num--)
		{
			Entity entity = elementInfos[num].entity;
			bool flag = true;
			if (EntitiesExtensions.TryGetBuffer<AssetPackElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity pack = val[i].m_Pack;
					for (int j = 0; j < packs.Count; j++)
					{
						Entity val2 = packs[j];
						if (pack == val2)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
				}
			}
			if (flag)
			{
				elementInfos.RemoveAtSwapBack(num);
			}
		}
	}

	private void FilterOutUpgrades(NativeList<UIObjectInfo> elementInfos)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		for (int num = elementInfos.Length - 1; num >= 0; num--)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ServiceUpgradeData>(elementInfos[num].entity))
			{
				elementInfos.RemoveAtSwapBack(num);
			}
		}
	}

	private void ToggleToolOptions(bool enabled)
	{
		m_ToolSystem.activeTool?.ToggleToolOptions(enabled);
	}

	[Preserve]
	public ToolbarUISystem()
	{
	}
}
