using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Audio;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class UpgradesSection : InfoSectionBase
{
	private ToolSystem m_ToolSystem;

	private ObjectToolSystem m_ObjectToolSystem;

	private UIInitializeSystem m_UIInitializeSystem;

	private PoliciesUISystem m_PoliciesUISystem;

	private PolicyPrefab m_BuildingOutOfServicePolicy;

	private AudioManager m_AudioManager;

	private EntityQuery m_SoundQuery;

	protected override string group => "UpgradesSection";

	private NativeList<Entity> extensions { get; set; }

	private NativeList<Entity> subBuildings { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<ToolSystem>();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<ObjectToolSystem>();
		m_UIInitializeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UIInitializeSystem>();
		m_PoliciesUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliciesUISystem>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		extensions = new NativeList<Entity>(5, AllocatorHandle.op_Implicit((Allocator)4));
		subBuildings = new NativeList<Entity>(10, AllocatorHandle.op_Implicit((Allocator)4));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>(group, "delete", (Action<Entity>)OnDelete, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>(group, "relocate", (Action<Entity>)OnRelocate, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>(group, "focus", (Action<Entity>)OnFocus, (IReader<Entity>)null));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>(group, "toggle", (Action<Entity>)OnToggle, (IReader<Entity>)null));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		base.OnDestroy();
		extensions.Dispose();
		subBuildings.Dispose();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		foreach (PolicyPrefab policy in m_UIInitializeSystem.policies)
		{
			if (((Object)policy).name == "Out of Service")
			{
				m_BuildingOutOfServicePolicy = policy;
			}
		}
	}

	private void OnToggle(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Building building = default(Building);
		Extension extension = default(Extension);
		bool flag = (EntitiesExtensions.TryGetComponent<Building>(((ComponentSystemBase)this).EntityManager, entity, ref building) && BuildingUtils.CheckOption(building, BuildingOption.Inactive)) || (EntitiesExtensions.TryGetComponent<Extension>(((ComponentSystemBase)this).EntityManager, entity, ref extension) && (extension.m_Flags & ExtensionFlags.Disabled) != 0);
		m_PoliciesUISystem.SetSelectedInfoPolicy(entity, m_PrefabSystem.GetEntity(m_BuildingOutOfServicePolicy), !flag);
	}

	private void OnRelocate(Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_ObjectToolSystem.StartMoving(entity);
		m_ToolSystem.activeTool = m_ObjectToolSystem;
	}

	private void OnDelete(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_BulldozeSound);
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			((EntityCommandBuffer)(ref val)).AddComponent<Deleted>(entity);
		}
	}

	private void OnFocus(Entity entity)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (Object)(object)SelectedInfoUISystem.s_CameraController != (Object)null && SelectedInfoUISystem.s_CameraController.controllerEnabled && SelectedInfoUISystem.s_CameraController.followedEntity == entity;
		m_InfoUISystem.Focus((!flag) ? entity : Entity.Null);
	}

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		extensions.Clear();
		subBuildings.Clear();
	}

	private bool Visible()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<BuildingUpgradeElement> val = default(DynamicBuffer<BuildingUpgradeElement>);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, GetUpgradable(selectedEntity), ref prefabRef) && EntitiesExtensions.TryGetBuffer<BuildingUpgradeElement>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity upgrade = val[i].m_Upgrade;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<UIObjectData>(upgrade))
				{
					result = true;
					break;
				}
			}
		}
		return result;
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

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
		if (!EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, GetUpgradable(selectedEntity), true, ref val))
		{
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			Entity upgrade = val[i].m_Upgrade;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<UIObjectData>(((EntityManager)(ref entityManager2)).GetComponentData<PrefabRef>(upgrade).m_Prefab))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Extension>(upgrade))
				{
					extensions.Add(ref upgrade);
				}
				else
				{
					subBuildings.Add(ref upgrade);
				}
			}
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("extensions");
		JsonWriterExtensions.ArrayBegin(writer, extensions.Length);
		Extension extension = default(Extension);
		for (int i = 0; i < extensions.Length; i++)
		{
			Entity val = extensions[i];
			writer.TypeBegin(group + ".Upgrade");
			writer.PropertyName("name");
			m_NameSystem.BindName(writer, val);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, val);
			bool flag = EntitiesExtensions.TryGetComponent<Extension>(((ComponentSystemBase)this).EntityManager, val, ref extension) && (extension.m_Flags & ExtensionFlags.Disabled) != 0;
			writer.PropertyName("disabled");
			writer.Write(flag);
			bool flag2 = (Object)(object)SelectedInfoUISystem.s_CameraController != (Object)null && SelectedInfoUISystem.s_CameraController.controllerEnabled && SelectedInfoUISystem.s_CameraController.followedEntity == val;
			writer.PropertyName("focused");
			writer.Write(flag2);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
		writer.PropertyName("subBuildings");
		JsonWriterExtensions.ArrayBegin(writer, subBuildings.Length);
		Building building = default(Building);
		for (int j = 0; j < subBuildings.Length; j++)
		{
			Entity val2 = subBuildings[j];
			writer.TypeBegin(group + ".Upgrade");
			writer.PropertyName("name");
			m_NameSystem.BindName(writer, val2);
			writer.PropertyName("entity");
			UnityWriters.Write(writer, val2);
			bool flag3 = EntitiesExtensions.TryGetComponent<Building>(((ComponentSystemBase)this).EntityManager, val2, ref building) && BuildingUtils.CheckOption(building, BuildingOption.Inactive);
			writer.PropertyName("disabled");
			writer.Write(flag3);
			bool flag4 = (Object)(object)SelectedInfoUISystem.s_CameraController != (Object)null && SelectedInfoUISystem.s_CameraController.controllerEnabled && SelectedInfoUISystem.s_CameraController.followedEntity == val2;
			writer.PropertyName("focused");
			writer.Write(flag4);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public UpgradesSection()
	{
	}
}
