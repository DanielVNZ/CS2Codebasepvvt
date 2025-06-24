using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Game.Input;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class UpgradeMenuUISystem : UISystemBase
{
	public struct SortableEntity : IComparable<SortableEntity>
	{
		public Entity m_Entity;

		public int m_Priority;

		public int CompareTo(SortableEntity other)
		{
			return m_Priority.CompareTo(other.m_Priority);
		}
	}

	private const string kGroup = "upgradeMenu";

	private EntityQuery m_UnlockedUpgradeQuery;

	private EntityQuery m_CreatedExtensionQuery;

	private EntityQuery m_DeletedExtensionQuery;

	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultTool;

	private ObjectToolSystem m_ObjectToolSystem;

	private AreaToolSystem m_AreaToolSystem;

	private NetToolSystem m_NetToolSystem;

	private RouteToolSystem m_RouteToolSystem;

	private UpgradeToolSystem m_UpgradeToolSystem;

	private PrefabSystem m_PrefabSystem;

	private PrefabUISystem m_PrefabUISystem;

	private ToolbarUISystem m_ToolbarUISystem;

	private SelectedInfoUISystem m_SelectedInfoUISystem;

	private UniqueAssetTrackingSystem m_UniqueAssetTrackingSystem;

	private RawMapBinding<Entity> m_UpgradesBinding;

	private RawMapBinding<Entity> m_UpgradeDetailsBinding;

	private ValueBinding<Entity> m_SelectedUpgradeBinding;

	private ValueBinding<bool> m_UpgradingBinding;

	private NativeList<SortableEntity> m_Upgrades;

	private NativeList<SortableEntity> m_Modules;

	private bool m_UniqueAssetStatusChanged;

	public override GameMode gameMode => GameMode.Game;

	public bool upgrading
	{
		get
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			ValueBinding<Entity> val = m_SelectedUpgradeBinding;
			if (val != null && ((EventBindingBase)val).active)
			{
				return m_SelectedUpgradeBinding.value != Entity.Null;
			}
			return false;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Expected O, but got Unknown
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UnlockedUpgradeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		m_CreatedExtensionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Extension>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Temp>()
		});
		m_DeletedExtensionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Extension>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultTool = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_ToolbarUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolbarUISystem>();
		m_SelectedInfoUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		m_UpgradeToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpgradeToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_AreaToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaToolSystem>();
		m_NetToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetToolSystem>();
		m_RouteToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RouteToolSystem>();
		m_UniqueAssetTrackingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UniqueAssetTrackingSystem>();
		UniqueAssetTrackingSystem uniqueAssetTrackingSystem = m_UniqueAssetTrackingSystem;
		uniqueAssetTrackingSystem.EventUniqueAssetStatusChanged = (Action<Entity, bool>)Delegate.Combine(uniqueAssetTrackingSystem.EventUniqueAssetStatusChanged, new Action<Entity, bool>(OnUniqueAssetStatusChanged));
		AddBinding((IBinding)(object)(m_UpgradesBinding = new RawMapBinding<Entity>("upgradeMenu", "upgrades", (Action<IJsonWriter, Entity>)BindUpgrades, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_SelectedUpgradeBinding = new ValueBinding<Entity>("upgradeMenu", "selectedUpgrade", Entity.Null, (IWriter<Entity>)null, (EqualityComparer<Entity>)null)));
		AddBinding((IBinding)(object)(m_UpgradeDetailsBinding = new RawMapBinding<Entity>("upgradeMenu", "upgradeDetails", (Action<IJsonWriter, Entity>)BindUpgradeDetails, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<Entity, Entity>("upgradeMenu", "selectUpgrade", (Action<Entity, Entity>)SelectUpgrade, (IReader<Entity>)null, (IReader<Entity>)null));
		AddBinding((IBinding)new TriggerBinding("upgradeMenu", "clearUpgradeSelection", (Action)ClearUpgradeSelection));
		AddBinding((IBinding)(object)(m_UpgradingBinding = new ValueBinding<bool>("upgradeMenu", "upgrading", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		m_Upgrades = new NativeList<SortableEntity>(9, AllocatorHandle.op_Implicit((Allocator)4));
		m_Modules = new NativeList<SortableEntity>(9, AllocatorHandle.op_Implicit((Allocator)4));
	}

	private void OnUniqueAssetStatusChanged(Entity prefabEntity, bool placed)
	{
		m_UniqueAssetStatusChanged = true;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Upgrades.Dispose();
		m_Modules.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		SelectedInfoUISystem selectedInfoUISystem = m_SelectedInfoUISystem;
		selectedInfoUISystem.eventSelectionChanged = (Action<Entity, Entity, float3>)Delegate.Combine(selectedInfoUISystem.eventSelectionChanged, new Action<Entity, Entity, float3>(OnSelectionChanged));
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Combine(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		SelectedInfoUISystem selectedInfoUISystem = m_SelectedInfoUISystem;
		selectedInfoUISystem.eventSelectionChanged = (Action<Entity, Entity, float3>)Delegate.Remove(selectedInfoUISystem.eventSelectionChanged, new Action<Entity, Entity, float3>(OnSelectionChanged));
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Remove(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
		((COSystemBase)this).OnStopRunning();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		Entity upgradable = GetUpgradable(m_SelectedInfoUISystem.selectedEntity);
		if (PrefabUtils.HasUnlockedPrefabAll<BuildingUpgradeElement, UIObjectData>(((ComponentSystemBase)this).EntityManager, m_UnlockedUpgradeQuery) || PrefabUtils.HasUnlockedPrefabAll<BuildingModuleData, UIObjectData>(((ComponentSystemBase)this).EntityManager, m_UnlockedUpgradeQuery) || !((EntityQuery)(ref m_CreatedExtensionQuery)).IsEmptyIgnoreFilter || !((EntityQuery)(ref m_DeletedExtensionQuery)).IsEmptyIgnoreFilter || m_UniqueAssetStatusChanged)
		{
			goto IL_008a;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Updated>(upgradable))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Destroyed>(upgradable) != (m_Upgrades.Length == 0))
			{
				goto IL_008a;
			}
		}
		goto IL_00a0;
		IL_008a:
		((MapBindingBase<Entity>)(object)m_UpgradesBinding).UpdateAll();
		((MapBindingBase<Entity>)(object)m_UpgradeDetailsBinding).UpdateAll();
		goto IL_00a0;
		IL_00a0:
		m_UniqueAssetStatusChanged = false;
	}

	private Entity GetUpgradable(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Attached attached = default(Attached);
		if (EntitiesExtensions.TryGetComponent<Attached>(((ComponentSystemBase)this).EntityManager, entity, ref attached))
		{
			return attached.m_Parent;
		}
		return entity;
	}

	private void ClearUpgradeSelection()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (m_SelectedUpgradeBinding.value != Entity.Null)
		{
			m_ToolSystem.activeTool = m_DefaultTool;
			SelectUpgrade(Entity.Null, Entity.Null);
		}
	}

	private void BindUpgrades(IJsonWriter writer, Entity upgradable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		upgradable = GetUpgradable(upgradable);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PrefabRef prefabRef = default(PrefabRef);
		if (!((EntityManager)(ref entityManager)).Exists(upgradable) || !EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, upgradable, ref prefabRef))
		{
			JsonWriterExtensions.WriteEmptyArray(writer);
			return;
		}
		m_Upgrades.Clear();
		m_Modules.Clear();
		DynamicBuffer<BuildingUpgradeElement> val = default(DynamicBuffer<BuildingUpgradeElement>);
		if (EntitiesExtensions.TryGetBuffer<BuildingUpgradeElement>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Destroyed>(upgradable))
			{
				UIObjectData uIObjectData = default(UIObjectData);
				for (int i = 0; i < val.Length; i++)
				{
					Entity upgrade = val[i].m_Upgrade;
					if (EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, upgrade, ref uIObjectData))
					{
						ref NativeList<SortableEntity> reference = ref m_Upgrades;
						SortableEntity sortableEntity = new SortableEntity
						{
							m_Entity = upgrade,
							m_Priority = uIObjectData.m_Priority
						};
						reference.Add(ref sortableEntity);
					}
				}
			}
		}
		DynamicBuffer<BuildingModule> val2 = default(DynamicBuffer<BuildingModule>);
		if (EntitiesExtensions.TryGetBuffer<BuildingModule>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val2))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Destroyed>(upgradable))
			{
				UIObjectData uIObjectData2 = default(UIObjectData);
				for (int j = 0; j < val2.Length; j++)
				{
					Entity module = val2[j].m_Module;
					if (EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, module, ref uIObjectData2))
					{
						ref NativeList<SortableEntity> reference2 = ref m_Modules;
						SortableEntity sortableEntity = new SortableEntity
						{
							m_Entity = module,
							m_Priority = uIObjectData2.m_Priority
						};
						reference2.Add(ref sortableEntity);
					}
				}
			}
		}
		NativeSortExtension.Sort<SortableEntity>(m_Upgrades);
		NativeSortExtension.Sort<SortableEntity>(m_Modules);
		JsonWriterExtensions.ArrayBegin(writer, m_Upgrades.Length + m_Modules.Length);
		for (int k = 0; k < m_Upgrades.Length; k++)
		{
			SortableEntity sortableEntity2 = m_Upgrades[k];
			var (unique, placed) = CheckExtensionBuiltStatus(upgradable, sortableEntity2.m_Entity);
			m_ToolbarUISystem.BindAsset(writer, sortableEntity2.m_Entity, unique, placed);
		}
		for (int l = 0; l < m_Modules.Length; l++)
		{
			SortableEntity sortableEntity3 = m_Modules[l];
			m_ToolbarUISystem.BindAsset(writer, sortableEntity3.m_Entity);
		}
		writer.ArrayEnd();
	}

	private void BindUpgradeDetails(IJsonWriter writer, Entity upgrade)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Entity upgradable = GetUpgradable(m_SelectedInfoUISystem.selectedEntity);
		var (unique, placed) = CheckExtensionBuiltStatus(upgradable, upgrade);
		m_PrefabUISystem.BindPrefabDetails(writer, upgrade, unique, placed);
	}

	private (bool extension, bool built) CheckExtensionBuiltStatus(Entity upgradableEntity, Entity upgradeEntity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		bool flag2 = false;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
		if (((EntityManager)(ref entityManager)).HasComponent<BuildingExtensionData>(upgradeEntity) && EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, upgradableEntity, true, ref val))
		{
			flag = true;
			PrefabRef prefabRef = default(PrefabRef);
			for (int i = 0; i < val.Length; i++)
			{
				if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val[i].m_Upgrade, ref prefabRef) && prefabRef.m_Prefab == upgradeEntity)
				{
					flag2 = true;
				}
			}
		}
		bool flag3 = m_UniqueAssetTrackingSystem.IsUniqueAsset(upgradeEntity);
		bool flag4 = m_UniqueAssetTrackingSystem.IsPlacedUniqueAsset(upgradeEntity);
		return (extension: flag || flag3, built: flag2 || flag4);
	}

	private void SelectUpgrade(Entity upgradable, Entity upgrade)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		upgradable = GetUpgradable(upgradable);
		m_SelectedUpgradeBinding.Update(upgrade);
		bool item = CheckExtensionBuiltStatus(upgradable, upgrade).built;
		if (upgradable != Entity.Null && upgrade != Entity.Null && !EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, upgrade) && !item)
		{
			PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(upgrade);
			m_UpgradingBinding.Update(true);
			((MapBindingBase<Entity>)(object)m_UpgradesBinding).UpdateAll();
			m_ToolSystem.ActivatePrefabTool(prefab);
			return;
		}
		PrefabBase prefab2 = m_ToolSystem.activeTool.GetPrefab();
		if ((Object)(object)prefab2 == (Object)null)
		{
			m_ToolSystem.activeTool = m_DefaultTool;
			return;
		}
		Entity entity = m_PrefabSystem.GetEntity(prefab2);
		DynamicBuffer<BuildingUpgradeElement> val = default(DynamicBuffer<BuildingUpgradeElement>);
		if (!(m_SelectedUpgradeBinding.value != Entity.Null) || !EntitiesExtensions.TryGetBuffer<BuildingUpgradeElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			if (!(m_SelectedUpgradeBinding.value != val[i].m_Upgrade))
			{
				m_ToolSystem.activeTool = m_DefaultTool;
				break;
			}
		}
	}

	private void OnSelectionChanged(Entity entity, Entity prefab, float3 position)
	{
		if (InputManager.instance.activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse)
		{
			ClearUpgradeSelection();
		}
	}

	private void OnToolChanged(ToolBaseSystem tool)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (tool == m_DefaultTool && InputManager.instance.activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse)
		{
			ClearUpgradeSelection();
		}
		bool flag = tool == m_ObjectToolSystem && m_ObjectToolSystem.mode == ObjectToolSystem.Mode.Upgrade;
		bool flag2 = tool == m_NetToolSystem && m_NetToolSystem.serviceUpgrade;
		bool flag3 = tool == m_RouteToolSystem && m_RouteToolSystem.serviceUpgrade;
		Owner owner = default(Owner);
		EntityManager entityManager;
		int num;
		if (m_ToolSystem.activeTool == m_AreaToolSystem && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, m_AreaToolSystem.recreate, ref owner))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			num = (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(owner.m_Owner) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		bool flag4 = (byte)num != 0;
		PrefabBase prefab = tool.GetPrefab();
		Entity val = ((prefab != null) ? m_PrefabSystem.GetEntity(prefab) : Entity.Null);
		ValueBinding<bool> obj = m_UpgradingBinding;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		obj.Update(((EntityManager)(ref entityManager)).HasComponent<BuildingModuleData>(val) || flag || flag2 || flag3 || flag4 || tool == m_UpgradeToolSystem);
	}

	[Preserve]
	public UpgradeMenuUISystem()
	{
	}
}
