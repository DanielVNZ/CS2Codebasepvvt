using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game.Common;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class PrefabSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public delegate void EventContentAvailabilityChanged(ContentPrefab contentPrefab);

	private struct ObsoleteData
	{
		public Entity m_Entity;

		public PrefabID m_ID;
	}

	private struct LoadedIndexData
	{
		public Entity m_Entity;

		public int m_Index;
	}

	private ILog m_UnlockingLog;

	private UpdateSystem m_UpdateSystem;

	private List<PrefabBase> m_Prefabs;

	private List<ObsoleteData> m_ObsoleteIDs;

	private List<LoadedIndexData> m_LoadedIndexData;

	private Dictionary<PrefabBase, Entity> m_UpdateMap;

	private Dictionary<PrefabBase, Entity> m_Entities;

	private Dictionary<PrefabBase, bool> m_IsUnlockable;

	private Dictionary<ContentPrefab, bool> m_IsAvailable;

	private Dictionary<int, PrefabID> m_LoadedObsoleteIDs;

	private Dictionary<PrefabID, int> m_PrefabIndices;

	private ComponentTypeSet m_UnlockableTypes;

	internal IEnumerable<PrefabBase> prefabs => m_Prefabs;

	public event EventContentAvailabilityChanged onContentAvailabilityChanged;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		m_Prefabs = new List<PrefabBase>();
		m_ObsoleteIDs = new List<ObsoleteData>();
		m_LoadedIndexData = new List<LoadedIndexData>();
		m_UpdateMap = new Dictionary<PrefabBase, Entity>();
		m_Entities = new Dictionary<PrefabBase, Entity>();
		m_IsUnlockable = new Dictionary<PrefabBase, bool>();
		m_IsAvailable = new Dictionary<ContentPrefab, bool>();
		m_LoadedObsoleteIDs = new Dictionary<int, PrefabID>();
		m_PrefabIndices = new Dictionary<PrefabID, int>();
		m_UnlockingLog = LogManager.GetLogger("Unlocking");
	}

	public bool AddPrefab(PrefabBase prefab, string parentName = null, PrefabBase parentPrefab = null, ComponentBase parentComponent = null)
	{
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)prefab == (Object)null)
		{
			if (parentName != null)
			{
				COSystemBase.baseLog.WarnFormat("Trying to add null prefab in {0}", (object)parentName);
			}
			else if ((Object)(object)parentPrefab != (Object)null && (Object)(object)parentComponent != (Object)null)
			{
				COSystemBase.baseLog.WarnFormat("Trying to add null prefab in {0}/{1}", (object)((Object)parentPrefab).name, (object)((Object)parentComponent).name);
			}
			else if ((Object)(object)parentPrefab != (Object)null)
			{
				COSystemBase.baseLog.WarnFormat("Trying to add null prefab in {0}", (object)((Object)parentPrefab).name);
			}
			else
			{
				COSystemBase.baseLog.WarnFormat("Trying to add null prefab", Array.Empty<object>());
			}
			return false;
		}
		try
		{
			if (!m_Entities.ContainsKey(prefab))
			{
				if (!IsAvailable(prefab))
				{
					if ((Object)(object)parentPrefab != (Object)null)
					{
						if ((Object)(object)parentComponent != (Object)null)
						{
							COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, "Dependency not available in {0}/{1}: {2}", (object)((Object)parentPrefab).name, (object)((Object)parentComponent).name, (object)((Object)prefab).name);
						}
						else
						{
							COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, "Dependency not available in {0}: {1}", (object)((Object)parentPrefab).name, (object)((Object)prefab).name);
						}
					}
					return false;
				}
				COSystemBase.baseLog.VerboseFormat((Object)(object)prefab, "Adding prefab '{0}'", (object)((Object)prefab).name);
				List<ComponentBase> list = new List<ComponentBase>();
				prefab.GetComponents(list);
				HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
				for (int i = 0; i < list.Count; i++)
				{
					list[i].GetPrefabComponents(hashSet);
				}
				if (IsUnlockable(prefab))
				{
					hashSet.Add(ComponentType.ReadWrite<UnlockRequirement>());
					hashSet.Add(ComponentType.ReadWrite<Locked>());
					m_UnlockingLog.DebugFormat("Prefab locked: {0}", (object)prefab);
				}
				hashSet.Add(ComponentType.ReadWrite<Created>());
				hashSet.Add(ComponentType.ReadWrite<Updated>());
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity val = ((EntityManager)(ref entityManager)).CreateEntity(PrefabUtils.ToArray(hashSet));
				PrefabData prefabData = new PrefabData
				{
					m_Index = m_Prefabs.Count
				};
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PrefabData>(val, prefabData);
				PrefabID prefabID = prefab.GetPrefabID();
				if (m_PrefabIndices.ContainsKey(prefabID))
				{
					COSystemBase.baseLog.WarnFormat((Object)(object)prefab, "Duplicate prefab ID: {0}", (object)prefabID);
				}
				else
				{
					m_PrefabIndices.Add(prefabID, m_Prefabs.Count);
				}
				if (prefab.TryGet<ObsoleteIdentifiers>(out var component) && component.m_PrefabIdentifiers != null)
				{
					for (int j = 0; j < component.m_PrefabIdentifiers.Length; j++)
					{
						PrefabIdentifierInfo prefabIdentifierInfo = component.m_PrefabIdentifiers[j];
						prefabID = new PrefabID(prefabIdentifierInfo.m_Type, prefabIdentifierInfo.m_Name);
						if (m_PrefabIndices.ContainsKey(prefabID))
						{
							COSystemBase.baseLog.WarnFormat((Object)(object)prefab, "Duplicate prefab ID: {0} ({1})", (object)prefabID, (object)(((AssetData)(object)prefab.asset != (IAssetData)null) ? ((string)(object)prefab.asset) : ((Object)prefab).name));
						}
						else
						{
							m_PrefabIndices.Add(prefabID, m_Prefabs.Count);
						}
					}
				}
				m_Prefabs.Add(prefab);
				m_Entities.Add(prefab, val);
				return true;
			}
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, ex, "Error when adding prefab: {0}", (object)((Object)prefab).name);
		}
		return false;
	}

	public bool RemovePrefab(PrefabBase prefab)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)prefab == (Object)null)
		{
			COSystemBase.baseLog.WarnFormat("Trying to remove null prefab", Array.Empty<object>());
			return false;
		}
		try
		{
			if (m_Entities.TryGetValue(prefab, out var value))
			{
				COSystemBase.baseLog.VerboseFormat((Object)(object)prefab, "Removing prefab '{0}'", (object)((Object)prefab).name);
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<Deleted>(value);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(value);
				PrefabID prefabID = prefab.GetPrefabID();
				if (m_PrefabIndices.TryGetValue(prefabID, out var value2) && value2 == componentData.m_Index)
				{
					m_PrefabIndices.Remove(prefabID);
				}
				if (prefab.TryGet<ObsoleteIdentifiers>(out var component) && component.m_PrefabIdentifiers != null)
				{
					for (int i = 0; i < component.m_PrefabIdentifiers.Length; i++)
					{
						PrefabIdentifierInfo prefabIdentifierInfo = component.m_PrefabIdentifiers[i];
						prefabID = new PrefabID(prefabIdentifierInfo.m_Type, prefabIdentifierInfo.m_Name);
						if (m_PrefabIndices.TryGetValue(prefabID, out value2) && value2 == componentData.m_Index)
						{
							m_PrefabIndices.Remove(prefabID);
						}
					}
				}
				if (componentData.m_Index != m_Prefabs.Count - 1)
				{
					PrefabBase prefabBase = m_Prefabs[m_Prefabs.Count - 1];
					Entity val = m_Entities[prefabBase];
					entityManager = ((ComponentSystemBase)this).EntityManager;
					PrefabData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(val);
					PrefabID prefabID2 = prefabBase.GetPrefabID();
					if (m_PrefabIndices.TryGetValue(prefabID2, out var value3) && value3 == componentData2.m_Index)
					{
						m_PrefabIndices[prefabID2] = componentData.m_Index;
					}
					if (prefabBase.TryGet<ObsoleteIdentifiers>(out var component2) && component2.m_PrefabIdentifiers != null)
					{
						for (int j = 0; j < component2.m_PrefabIdentifiers.Length; j++)
						{
							PrefabIdentifierInfo prefabIdentifierInfo2 = component2.m_PrefabIdentifiers[j];
							prefabID2 = new PrefabID(prefabIdentifierInfo2.m_Type, prefabIdentifierInfo2.m_Name);
							if (m_PrefabIndices.TryGetValue(prefabID2, out value3) && value3 == componentData2.m_Index)
							{
								m_PrefabIndices[prefabID2] = componentData.m_Index;
							}
						}
					}
					componentData2.m_Index = componentData.m_Index;
					entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).SetComponentData<PrefabData>(val, componentData2);
					m_Prefabs[componentData.m_Index] = prefabBase;
				}
				componentData.m_Index = -1000000000;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PrefabData>(value, componentData);
				m_Prefabs.RemoveAt(m_Prefabs.Count - 1);
				m_Entities.Remove(prefab);
				m_IsUnlockable.Remove(prefab);
				return true;
			}
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, ex, "Error when removing prefab: {0}", (object)((Object)prefab).name);
		}
		return false;
	}

	public PrefabBase DuplicatePrefab(PrefabBase template, string name = null)
	{
		PrefabBase prefabBase = template.Clone(name);
		prefabBase.Remove<ObsoleteIdentifiers>();
		AddPrefab(prefabBase);
		return prefabBase;
	}

	public void UpdatePrefab(PrefabBase prefab, Entity sourceInstance = default(Entity))
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_UpdateMap[prefab] = sourceInstance;
	}

	public string[] GetAvailablePrerequisitesNames()
	{
		string[] array = (from entry in m_IsAvailable
			where entry.Value
			select ((Object)entry.Key).name).ToArray();
		if (array.Length == 0)
		{
			return null;
		}
		return array;
	}

	public IEnumerable<ContentPrefab> GetAvailableContentPrefabs()
	{
		return from entry in m_IsAvailable
			where entry.Value
			select entry.Key;
	}

	public bool IsAvailable(PrefabBase prefab)
	{
		if (prefab.TryGet<ContentPrerequisite>(out var component))
		{
			if (!m_IsAvailable.TryGetValue(component.m_ContentPrerequisite, out var value))
			{
				value = component.m_ContentPrerequisite.IsAvailable();
				m_IsAvailable.Add(component.m_ContentPrerequisite, value);
				this.onContentAvailabilityChanged?.Invoke(component.m_ContentPrerequisite);
			}
			return value;
		}
		return true;
	}

	public void UpdateAvailabilityCache()
	{
		foreach (KeyValuePair<ContentPrefab, bool> item in m_IsAvailable.ToList())
		{
			bool value = item.Value;
			bool flag = item.Key.IsAvailable();
			m_IsAvailable[item.Key] = flag;
			if (flag != value)
			{
				this.onContentAvailabilityChanged?.Invoke(item.Key);
			}
		}
	}

	public bool IsUnlockable(PrefabBase prefab)
	{
		if (m_IsUnlockable.TryGetValue(prefab, out var value))
		{
			return value;
		}
		if (prefab is UnlockRequirementPrefab)
		{
			m_IsUnlockable.Add(prefab, value: true);
			return true;
		}
		List<PrefabBase> dependencies = new List<PrefabBase>();
		List<ComponentBase> components = new List<ComponentBase>();
		value = IsUnlockableImpl(prefab, dependencies, components);
		m_IsUnlockable.Add(prefab, value);
		return value;
	}

	private bool IsUnlockableImpl(PrefabBase prefab, List<PrefabBase> dependencies, List<ComponentBase> components)
	{
		int count = dependencies.Count;
		try
		{
			try
			{
				bool canIgnoreUnlockDependencies = prefab.canIgnoreUnlockDependencies;
				prefab.GetComponents(components);
				for (int i = 0; i < components.Count; i++)
				{
					ComponentBase componentBase = components[i];
					if (componentBase is UnlockableBase)
					{
						return true;
					}
					if (!canIgnoreUnlockDependencies || !componentBase.ignoreUnlockDependencies)
					{
						componentBase.GetDependencies(dependencies);
					}
				}
			}
			finally
			{
				components.Clear();
			}
			for (int j = count; j < dependencies.Count; j++)
			{
				PrefabBase prefabBase = dependencies[j];
				if ((Object)(object)prefabBase == (Object)null)
				{
					continue;
				}
				if (m_IsUnlockable.TryGetValue(prefabBase, out var value))
				{
					if (value)
					{
						return true;
					}
					continue;
				}
				if (prefabBase is UnlockRequirementPrefab)
				{
					m_IsUnlockable.Add(prefabBase, value: true);
					return true;
				}
				value = IsUnlockableImpl(prefabBase, dependencies, components);
				m_IsUnlockable.Add(prefabBase, value);
				if (value)
				{
					return true;
				}
			}
			return false;
		}
		finally
		{
			dependencies.RemoveRange(count, dependencies.Count - count);
		}
	}

	public void AddUnlockRequirement(PrefabBase unlocker, PrefabBase unlocked)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (IsUnlockable(unlocker))
		{
			if (IsUnlockable(unlocked))
			{
				this.GetBuffer<UnlockRequirement>(unlocked, isReadOnly: false).Add(new UnlockRequirement(GetEntity(unlocker), UnlockFlags.RequireAll));
			}
			else
			{
				COSystemBase.baseLog.WarnFormat((Object)(object)unlocked, "{0} is trying to add unlock requirement to non-unlockable prefab {1}", (object)((Object)unlocker).name, (object)((Object)unlocked).name);
			}
		}
		else
		{
			COSystemBase.baseLog.WarnFormat((Object)(object)unlocker, "{0} is trying to add unlock requirements, but is non-unlockable", (object)((Object)unlocker).name);
		}
	}

	public void AddUnlockRequirement(PrefabBase unlocker, PrefabBase[] unlocked)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (IsUnlockable(unlocker))
		{
			Entity entity = GetEntity(unlocker);
			for (int i = 0; i < unlocked.Length; i++)
			{
				if (IsUnlockable(unlocked[i]))
				{
					this.GetBuffer<UnlockRequirement>(unlocked[i], isReadOnly: false).Add(new UnlockRequirement(entity, UnlockFlags.RequireAll));
				}
				else
				{
					COSystemBase.baseLog.WarnFormat((Object)(object)unlocked[i], "{0} is trying to add unlock requirement to non-unlockable prefab {1}", (object)((Object)unlocker).name, (object)((Object)unlocked[i]).name);
				}
			}
		}
		else
		{
			COSystemBase.baseLog.WarnFormat((Object)(object)unlocker, "{0} is trying to add unlock requirements, but is non-unlockable", (object)((Object)unlocker).name);
		}
	}

	public T GetPrefab<T>(PrefabData prefabData) where T : PrefabBase
	{
		return m_Prefabs[prefabData.m_Index] as T;
	}

	public T GetPrefab<T>(Entity entity) where T : PrefabBase
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return GetPrefab<T>(((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(entity));
	}

	public T GetPrefab<T>(PrefabRef refData) where T : PrefabBase
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return GetPrefab<T>(((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(refData.m_Prefab));
	}

	public bool TryGetPrefab<T>(PrefabData prefabData, out T prefab) where T : PrefabBase
	{
		if (prefabData.m_Index >= 0)
		{
			prefab = m_Prefabs[prefabData.m_Index] as T;
			return true;
		}
		prefab = null;
		return false;
	}

	public bool TryGetPrefab<T>(Entity entity, out T prefab) where T : PrefabBase
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		PrefabData prefabData = default(PrefabData);
		if (EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, entity, ref prefabData))
		{
			return TryGetPrefab<T>(prefabData, out prefab);
		}
		prefab = null;
		return false;
	}

	public bool TryGetPrefab<T>(PrefabRef refData, out T prefab) where T : PrefabBase
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		PrefabData prefabData = default(PrefabData);
		if (EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, refData.m_Prefab, ref prefabData))
		{
			return TryGetPrefab<T>(prefabData, out prefab);
		}
		prefab = null;
		return false;
	}

	public T GetSingletonPrefab<T>(EntityQuery group) where T : PrefabBase
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GetPrefab<T>(((EntityQuery)(ref group)).GetSingletonEntity());
	}

	public bool TryGetSingletonPrefab<T>(EntityQuery group, out T prefab) where T : PrefabBase
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref group)).IsEmptyIgnoreFilter)
		{
			prefab = GetPrefab<T>(((EntityQuery)(ref group)).GetSingletonEntity());
			return true;
		}
		prefab = null;
		return false;
	}

	public Entity GetEntity(PrefabBase prefab)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return m_Entities[prefab];
	}

	public bool TryGetEntity(PrefabBase prefab, out Entity entity)
	{
		return m_Entities.TryGetValue(prefab, out entity);
	}

	public bool HasComponent<T>(PrefabBase prefab) where T : unmanaged
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<T>(m_Entities[prefab]);
	}

	public bool HasEnabledComponent<T>(PrefabBase prefab) where T : unmanaged, IEnableableComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return EntitiesExtensions.HasEnabledComponent<T>(((ComponentSystemBase)this).EntityManager, m_Entities[prefab]);
	}

	public T GetComponentData<T>(PrefabBase prefab) where T : unmanaged, IComponentData
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).GetComponentData<T>(m_Entities[prefab]);
	}

	public bool TryGetComponentData<T>(PrefabBase prefab, out T component) where T : unmanaged, IComponentData
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return EntitiesExtensions.TryGetComponent<T>(((ComponentSystemBase)this).EntityManager, m_Entities[prefab], ref component);
	}

	public DynamicBuffer<T> GetBuffer<T>(PrefabBase prefab, bool isReadOnly) where T : unmanaged, IBufferElementData
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).GetBuffer<T>(m_Entities[prefab], isReadOnly);
	}

	public bool TryGetBuffer<T>(PrefabBase prefab, bool isReadOnly, out DynamicBuffer<T> buffer) where T : unmanaged, IBufferElementData
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return EntitiesExtensions.TryGetBuffer<T>(((ComponentSystemBase)this).EntityManager, m_Entities[prefab], isReadOnly, ref buffer);
	}

	public void AddComponentData<T>(PrefabBase prefab, T componentData) where T : unmanaged, IComponentData
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponentData<T>(m_Entities[prefab], componentData);
	}

	public void RemoveComponent<T>(PrefabBase prefab) where T : unmanaged, IComponentData
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent<T>(m_Entities[prefab]);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		bool num = UpdatePrefabs();
		m_UpdateSystem.Update(SystemUpdatePhase.PrefabUpdate);
		if (num)
		{
			((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ReplacePrefabSystem>().FinalizeReplaces();
		}
	}

	private bool UpdatePrefabs()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if (m_UpdateMap.Count == 0)
		{
			return false;
		}
		try
		{
			foreach (KeyValuePair<PrefabBase, Entity> item in m_UpdateMap)
			{
				PrefabBase key = item.Key;
				Entity value = item.Value;
				try
				{
					if (m_Entities.TryGetValue(key, out var value2))
					{
						EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).AddComponent<Deleted>(value2);
						List<ComponentBase> list = new List<ComponentBase>();
						key.GetComponents(list);
						HashSet<ComponentType> hashSet = new HashSet<ComponentType>();
						for (int i = 0; i < list.Count; i++)
						{
							list[i].GetPrefabComponents(hashSet);
						}
						bool num = IsUnlockable(key);
						if (num)
						{
							hashSet.Add(ComponentType.ReadWrite<UnlockRequirement>());
							hashSet.Add(ComponentType.ReadWrite<Locked>());
						}
						hashSet.Add(ComponentType.ReadWrite<Created>());
						hashSet.Add(ComponentType.ReadWrite<Updated>());
						entityManager = ((ComponentSystemBase)this).EntityManager;
						Entity val = ((EntityManager)(ref entityManager)).CreateEntity(PrefabUtils.ToArray(hashSet));
						entityManager = ((ComponentSystemBase)this).EntityManager;
						EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).SetComponentData<PrefabData>(val, ((EntityManager)(ref entityManager2)).GetComponentData<PrefabData>(value2));
						if (num && !EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, value2))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							((EntityManager)(ref entityManager)).SetComponentEnabled<Locked>(val, false);
						}
						m_Entities[key] = val;
						((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ReplacePrefabSystem>().ReplacePrefab(value2, val, value);
					}
				}
				catch (Exception ex)
				{
					COSystemBase.baseLog.ErrorFormat((Object)(object)key, ex, "Error when updating prefab: {0}", (object)((Object)key).name);
				}
			}
		}
		finally
		{
			m_UpdateMap.Clear();
		}
		return true;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		int count = m_Prefabs.Count;
		int count2 = m_ObsoleteIDs.Count;
		List<PrefabID> list = new List<PrefabID>(10000);
		List<PrefabID> list2 = new List<PrefabID>(100);
		EntityManager entityManager;
		for (int i = 0; i < count; i++)
		{
			PrefabBase prefabBase = m_Prefabs[i];
			Entity entity = GetEntity(prefabBase);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).IsComponentEnabled<PrefabData>(entity))
			{
				list.Add(prefabBase.GetPrefabID());
			}
		}
		for (int j = 0; j < count2; j++)
		{
			ObsoleteData obsoleteData = m_ObsoleteIDs[j];
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).IsComponentEnabled<PrefabData>(obsoleteData.m_Entity))
			{
				list2.Add(obsoleteData.m_ID);
			}
		}
		count = list.Count;
		count2 = list2.Count;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(count);
		((IWriter)writer/*cast due to .constrained prefix*/).Write(count2);
		for (int k = 0; k < count; k++)
		{
			PrefabID prefabID = list[k];
			((IWriter)writer/*cast due to .constrained prefix*/).Write<PrefabID>(prefabID);
		}
		for (int l = 0; l < count2; l++)
		{
			PrefabID prefabID2 = list2[l];
			((IWriter)writer/*cast due to .constrained prefix*/).Write<PrefabID>(prefabID2);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		m_ObsoleteIDs.Clear();
		m_LoadedObsoleteIDs.Clear();
		m_LoadedIndexData.Clear();
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		int num2 = 0;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.obsoletePrefabFix)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
		}
		PrefabID prefabID = default(PrefabID);
		for (int i = 0; i < num; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read<PrefabID>(ref prefabID);
			if (TryGetPrefab(prefabID, out var prefab))
			{
				m_LoadedIndexData.Add(new LoadedIndexData
				{
					m_Entity = GetEntity(prefab),
					m_Index = i
				});
			}
			else
			{
				m_LoadedObsoleteIDs.Add(i, prefabID);
			}
		}
		PrefabID prefabID2 = default(PrefabID);
		for (int j = 0; j < num2; j++)
		{
			int num3 = -1 - j;
			((IReader)reader/*cast due to .constrained prefix*/).Read<PrefabID>(ref prefabID2);
			if (TryGetPrefab(prefabID2, out var prefab2))
			{
				m_LoadedIndexData.Add(new LoadedIndexData
				{
					m_Entity = GetEntity(prefab2),
					m_Index = num3
				});
			}
			else
			{
				m_LoadedObsoleteIDs.Add(num3, prefabID2);
			}
		}
	}

	public void SetDefaults(Context context)
	{
		m_ObsoleteIDs.Clear();
		m_LoadedObsoleteIDs.Clear();
		m_LoadedIndexData.Clear();
	}

	public void UpdateLoadedIndices()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		int count = m_Prefabs.Count;
		EntityManager entityManager;
		for (int i = 0; i < count; i++)
		{
			PrefabBase prefab = m_Prefabs[i];
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).GetBuffer<LoadedIndex>(GetEntity(prefab), false).Clear();
		}
		int count2 = m_LoadedIndexData.Count;
		for (int j = 0; j < count2; j++)
		{
			LoadedIndexData loadedIndexData = m_LoadedIndexData[j];
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).GetBuffer<LoadedIndex>(loadedIndexData.m_Entity, false).Add(new LoadedIndex
			{
				m_Index = loadedIndexData.m_Index
			});
		}
	}

	public bool TryGetPrefab(PrefabID id, out PrefabBase prefab)
	{
		if (m_PrefabIndices.TryGetValue(id, out var value))
		{
			prefab = m_Prefabs[value];
			return true;
		}
		prefab = null;
		return false;
	}

	public PrefabID GetLoadedObsoleteID(int loadedIndex)
	{
		if (!m_LoadedObsoleteIDs.TryGetValue(loadedIndex, out var value))
		{
			value = new PrefabID("[Missing]", "[Missing]");
		}
		return value;
	}

	public void AddObsoleteID(Entity entity, PrefabID id)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		m_ObsoleteIDs.Add(new ObsoleteData
		{
			m_Entity = entity,
			m_ID = id
		});
		COSystemBase.baseLog.WarnFormat("Unknown prefab ID: {0}", (object)id);
	}

	public PrefabID GetObsoleteID(PrefabData prefabData)
	{
		return m_ObsoleteIDs[-1 - prefabData.m_Index].m_ID;
	}

	public PrefabID GetObsoleteID(Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return GetObsoleteID(((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(entity));
	}

	public unsafe string GetPrefabName(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		PrefabData prefabData = default(PrefabData);
		if (EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, entity, ref prefabData))
		{
			if (prefabData.m_Index >= 0)
			{
				return ((Object)m_Prefabs[prefabData.m_Index]).name;
			}
			return GetObsoleteID(prefabData).GetName();
		}
		return ((object)(*(Entity*)(&entity))/*cast due to .constrained prefix*/).ToString();
	}

	[Preserve]
	public PrefabSystem()
	{
	}
}
