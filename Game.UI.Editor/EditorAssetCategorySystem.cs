using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class EditorAssetCategorySystem : GameSystemBase
{
	public interface IEditorAssetCategoryFilter
	{
		bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem prefabSystem);
	}

	public class ServiceTypeFilter : IEditorAssetCategoryFilter
	{
		private Entity m_Service;

		public ServiceTypeFilter(Entity service)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			m_Service = service;
		}

		public bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem _)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			ServiceObjectData serviceObjectData = default(ServiceObjectData);
			if (EntitiesExtensions.TryGetComponent<ServiceObjectData>(entityManager, prefab, ref serviceObjectData))
			{
				return serviceObjectData.m_Service == m_Service;
			}
			DynamicBuffer<ServiceUpgradeBuilding> val = default(DynamicBuffer<ServiceUpgradeBuilding>);
			if (EntitiesExtensions.TryGetBuffer<ServiceUpgradeBuilding>(entityManager, prefab, true, ref val))
			{
				Enumerator<ServiceUpgradeBuilding> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (EntitiesExtensions.TryGetComponent<ServiceObjectData>(entityManager, enumerator.Current.m_Building, ref serviceObjectData) && serviceObjectData.m_Service == m_Service)
						{
							return true;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
				return false;
			}
			return true;
		}
	}

	public class ZoneTypeFilter : IEditorAssetCategoryFilter
	{
		private Entity m_Zone;

		public ZoneTypeFilter(Entity zone)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			m_Zone = zone;
		}

		public bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem _)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			if (EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(entityManager, prefab, ref spawnableBuildingData))
			{
				return spawnableBuildingData.m_ZonePrefab == m_Zone;
			}
			PlaceholderBuildingData placeholderBuildingData = default(PlaceholderBuildingData);
			if (EntitiesExtensions.TryGetComponent<PlaceholderBuildingData>(entityManager, prefab, ref placeholderBuildingData))
			{
				return placeholderBuildingData.m_ZonePrefab == m_Zone;
			}
			return true;
		}
	}

	public class PassengerCountFilter : IEditorAssetCategoryFilter
	{
		public enum FilterType
		{
			Equals,
			NotEquals,
			MoreThan,
			LessThan
		}

		public FilterType m_Type;

		public int m_Count;

		public bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem _)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			PersonalCarData data = default(PersonalCarData);
			if (EntitiesExtensions.TryGetComponent<PersonalCarData>(entityManager, prefab, ref data))
			{
				return Check(data);
			}
			return true;
		}

		private bool Check(PersonalCarData data)
		{
			return m_Type switch
			{
				FilterType.Equals => data.m_PassengerCapacity == m_Count, 
				FilterType.NotEquals => data.m_PassengerCapacity != m_Count, 
				FilterType.MoreThan => data.m_PassengerCapacity > m_Count, 
				FilterType.LessThan => data.m_PassengerCapacity < m_Count, 
				_ => false, 
			};
		}
	}

	public class PublicTransportTypeFilter : IEditorAssetCategoryFilter
	{
		public TransportType m_TransportType;

		public PublicTransportPurpose m_Purpose;

		public bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem prefabSystem)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
			if (EntitiesExtensions.TryGetComponent<PublicTransportVehicleData>(entityManager, prefab, ref publicTransportVehicleData))
			{
				if (m_TransportType == TransportType.None || publicTransportVehicleData.m_TransportType == m_TransportType)
				{
					if (m_Purpose != 0)
					{
						return (publicTransportVehicleData.m_PurposeMask & m_Purpose) != 0;
					}
					return true;
				}
				return false;
			}
			return true;
		}
	}

	public class MaintenanceTypeFilter : IEditorAssetCategoryFilter
	{
		public MaintenanceType m_Type;

		public bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem prefabSystem)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			MaintenanceVehicleData maintenanceVehicleData = default(MaintenanceVehicleData);
			if (EntitiesExtensions.TryGetComponent<MaintenanceVehicleData>(entityManager, prefab, ref maintenanceVehicleData))
			{
				return (maintenanceVehicleData.m_MaintenanceType & m_Type) != 0;
			}
			return true;
		}
	}

	public class ThemeFilter : IEditorAssetCategoryFilter
	{
		public Entity m_Theme;

		public bool m_DefaultResult = true;

		public bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem prefabSystem)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (!prefabSystem.TryGetPrefab<PrefabBase>(prefab, out var prefab2))
			{
				return false;
			}
			ThemeObject component = prefab2.GetComponent<ThemeObject>();
			if ((Object)(object)component == (Object)null)
			{
				return m_DefaultResult;
			}
			return prefabSystem.GetEntity(component.m_Theme) == m_Theme;
		}
	}

	public class TrackTypeFilter : IEditorAssetCategoryFilter
	{
		public TrackTypes m_TrackType;

		public bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem prefabSystem)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if (!prefabSystem.TryGetPrefab<TrackPrefab>(prefab, out var prefab2))
			{
				return false;
			}
			if ((Object)(object)prefab2 != (Object)null)
			{
				return (prefab2.m_TrackType & m_TrackType) != 0;
			}
			return true;
		}
	}

	public class SignatureBuildingFilter : IEditorAssetCategoryFilter
	{
		public AreaType m_AreaType;

		public bool m_Office;

		public Entity m_Theme;

		public bool Contains(Entity prefab, EntityManager entityManager, PrefabSystem prefabSystem)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (!prefabSystem.TryGetPrefab<PrefabBase>(prefab, out var prefab2))
			{
				return false;
			}
			SignatureBuilding component = prefab2.GetComponent<SignatureBuilding>();
			if ((Object)(object)component != (Object)null)
			{
				ThemeObject component2 = prefab2.GetComponent<ThemeObject>();
				Entity val = Entity.Null;
				if ((Object)(object)component2 != (Object)null)
				{
					val = prefabSystem.GetEntity(component2.m_Theme);
				}
				if (m_Theme == val && component.m_ZoneType.m_AreaType == m_AreaType)
				{
					return component.m_ZoneType.m_Office == m_Office;
				}
				return false;
			}
			return true;
		}
	}

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

	private List<EditorAssetCategory> m_Categories = new List<EditorAssetCategory>();

	private Dictionary<string, EditorAssetCategory> m_PathMap = new Dictionary<string, EditorAssetCategory>();

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_ServiceQuery;

	private EntityQuery m_ZoneQuery;

	private EntityQuery m_ThemeQuery;

	private EntityQuery m_Overrides;

	private EntityQuery m_PrefabModificationQuery;

	private bool m_Dirty = true;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ServiceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceData>() });
		m_ZoneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ZoneData>() });
		m_ThemeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ThemeData>() });
		m_Overrides = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EditorAssetCategoryOverrideData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabData>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_PrefabModificationQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (!((EntityQuery)(ref m_PrefabModificationQuery)).IsEmptyIgnoreFilter)
		{
			m_Dirty = true;
		}
	}

	public IEnumerable<EditorAssetCategory> GetCategories(bool ignoreEmpty = true)
	{
		if (m_Dirty)
		{
			GenerateCategories();
		}
		foreach (var item in GetCategoriesImpl(m_Categories, 0, ignoreEmpty))
		{
			yield return item.Item1;
		}
	}

	public IEnumerable<HierarchyItem<EditorAssetCategory>> GetHierarchy(bool ignoreEmpty = true)
	{
		if (m_Dirty)
		{
			GenerateCategories();
		}
		foreach (var (editorAssetCategory, level) in GetCategoriesImpl(m_Categories, 0, ignoreEmpty))
		{
			yield return editorAssetCategory.ToHierarchyItem(level);
		}
	}

	private void AddCategory(EditorAssetCategory category, EditorAssetCategory parent = null)
	{
		string text = category.id.Trim('/');
		category.path = ((parent != null) ? (parent.path + "/" + text) : text);
		if (parent == null)
		{
			m_Categories.Add(category);
		}
		else
		{
			parent.AddSubCategory(category);
		}
		m_PathMap[category.path] = category;
	}

	private void ClearCategories()
	{
		m_Categories.Clear();
		m_PathMap.Clear();
	}

	private IEnumerable<(EditorAssetCategory, int)> GetCategoriesImpl(IEnumerable<EditorAssetCategory> categories, int level, bool ignoreEmpty)
	{
		foreach (EditorAssetCategory category in categories)
		{
			if (ignoreEmpty && category.IsEmpty(((ComponentSystemBase)this).EntityManager, m_PrefabSystem, InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef)))
			{
				continue;
			}
			yield return (category, level);
			foreach (var (item, item2) in GetCategoriesImpl(category.subCategories, level + 1, ignoreEmpty))
			{
				yield return (item, item2);
			}
		}
	}

	private void GenerateCategories()
	{
		ClearCategories();
		GenerateBuildingCategories();
		GenerateVehicleCategories();
		GeneratePropCategories();
		GenerateFoliageCategories();
		GenerateCharacterCategories();
		GenerateAreaCategories();
		GenerateSurfaceCategories();
		GenerateBridgeCategory();
		GenerateRoadCategory();
		GenerateTrackCategories();
		GenerateEffectCategories();
		GenerateLocationCategories();
		AddOverrides();
		m_Dirty = false;
	}

	private void GenerateBuildingCategories()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Buildings"
		};
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.ReadOnly<BuildingExtensionData>()
		};
		array[0] = val;
		editorAssetCategory.entityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		editorAssetCategory.includeChildCategories = false;
		EditorAssetCategory editorAssetCategory2 = editorAssetCategory;
		AddCategory(editorAssetCategory2);
		GenerateServiceBuildingCategories(editorAssetCategory2);
		GenerateSpawnableBuildingCategories(editorAssetCategory2);
		GenerateMiscBuildingCategory(editorAssetCategory2);
	}

	private void GenerateServiceBuildingCategories(EditorAssetCategory parent)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Services"
		};
		AddCategory(editorAssetCategory, parent);
		NativeArray<Entity> val = ((EntityQuery)(ref m_ServiceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < val.Length; i++)
		{
			if (m_PrefabSystem.TryGetPrefab<PrefabBase>(val[i], out var prefab))
			{
				EditorAssetCategory editorAssetCategory2 = new EditorAssetCategory
				{
					id = ((Object)prefab).name
				};
				EntityQueryDesc[] array = new EntityQueryDesc[2];
				EntityQueryDesc val2 = new EntityQueryDesc();
				val2.All = (ComponentType[])(object)new ComponentType[2]
				{
					ComponentType.ReadOnly<BuildingData>(),
					ComponentType.ReadOnly<ServiceObjectData>()
				};
				val2.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrafficSpawnerData>() };
				array[0] = val2;
				val2 = new EntityQueryDesc();
				val2.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ServiceUpgradeBuilding>() };
				val2.Any = (ComponentType[])(object)new ComponentType[2]
				{
					ComponentType.ReadOnly<BuildingData>(),
					ComponentType.ReadOnly<BuildingExtensionData>()
				};
				val2.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrafficSpawnerData>() };
				array[1] = val2;
				editorAssetCategory2.entityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
				editorAssetCategory2.filter = new ServiceTypeFilter(val[i]);
				editorAssetCategory2.icon = ImageSystem.GetIcon(prefab);
				EditorAssetCategory category = editorAssetCategory2;
				AddCategory(category, editorAssetCategory);
			}
		}
	}

	private void GenerateSpawnableBuildingCategories(EditorAssetCategory parent)
	{
		GenerateZoneCategories(AreaType.Residential, office: false, parent);
		GenerateZoneCategories(AreaType.Commercial, office: false, parent);
		GenerateZoneCategories(AreaType.Industrial, office: false, parent);
		GenerateZoneCategories(AreaType.Industrial, office: true, parent);
	}

	private void GenerateZoneCategories(AreaType areaType, bool office, EditorAssetCategory parent)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		string id = (office ? "Office" : areaType.ToString());
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = id
		};
		AddCategory(editorAssetCategory, parent);
		NativeArray<Entity> val = ((EntityQuery)(ref m_ZoneQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < val.Length; i++)
		{
			if (m_PrefabSystem.TryGetPrefab<ZonePrefab>(val[i], out var prefab) && prefab.m_AreaType == areaType && prefab.m_Office == office)
			{
				EditorAssetCategory editorAssetCategory2 = new EditorAssetCategory
				{
					id = ((Object)prefab).name
				};
				EditorAssetCategory editorAssetCategory3 = editorAssetCategory2;
				EntityQueryDesc[] array = new EntityQueryDesc[1];
				EntityQueryDesc val2 = new EntityQueryDesc();
				val2.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingData>() };
				val2.Any = (ComponentType[])(object)new ComponentType[2]
				{
					ComponentType.ReadOnly<SpawnableBuildingData>(),
					ComponentType.ReadOnly<PlaceholderBuildingData>()
				};
				array[0] = val2;
				editorAssetCategory3.entityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
				editorAssetCategory2.filter = new ZoneTypeFilter(val[i]);
				editorAssetCategory2.icon = ImageSystem.GetIcon(prefab);
				EditorAssetCategory category = editorAssetCategory2;
				AddCategory(category, editorAssetCategory);
			}
		}
		if (areaType == AreaType.Industrial && !office)
		{
			EditorAssetCategory category2 = new EditorAssetCategory
			{
				id = "Extractors",
				entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
				{
					ComponentType.ReadOnly<BuildingData>(),
					ComponentType.ReadOnly<ExtractorFacilityData>()
				})
			};
			AddCategory(category2, editorAssetCategory);
		}
		NativeArray<Entity> val3 = ((EntityQuery)(ref m_ThemeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		for (int j = 0; j < val3.Length; j++)
		{
			if (m_PrefabSystem.TryGetPrefab<ThemePrefab>(val3[j], out var prefab2))
			{
				EditorAssetCategory category3 = new EditorAssetCategory
				{
					id = prefab2.assetPrefix + " Signature",
					entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SignatureBuildingData>() }),
					filter = new SignatureBuildingFilter
					{
						m_AreaType = areaType,
						m_Office = office,
						m_Theme = val3[j]
					}
				};
				AddCategory(category3, editorAssetCategory);
			}
		}
		EditorAssetCategory category4 = new EditorAssetCategory
		{
			id = "Signature",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SignatureBuildingData>() }),
			filter = new SignatureBuildingFilter
			{
				m_AreaType = areaType,
				m_Office = office,
				m_Theme = Entity.Null
			}
		};
		AddCategory(category4, editorAssetCategory);
		val.Dispose();
	}

	private void GenerateMiscBuildingCategory(EditorAssetCategory parent)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Misc",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
			{
				ComponentType.ReadOnly<BuildingData>(),
				ComponentType.Exclude<ServiceObjectData>(),
				ComponentType.Exclude<SpawnableBuildingData>(),
				ComponentType.Exclude<SignatureBuildingData>(),
				ComponentType.Exclude<ServiceUpgradeBuilding>(),
				ComponentType.Exclude<ExtractorFacilityData>()
			})
		};
		AddCategory(category, parent);
	}

	private void GenerateVehicleCategories()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Vehicles",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<VehicleData>() }),
			includeChildCategories = false
		};
		AddCategory(editorAssetCategory);
		GenerateResidentialVehicleCategory(editorAssetCategory);
		GenerateIndustrialVehicleCategory(editorAssetCategory);
		GenerateServiceVehicleCategories(editorAssetCategory);
	}

	private void GenerateResidentialVehicleCategory(EditorAssetCategory parent)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Residential"
		};
		AddCategory(editorAssetCategory, parent);
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Cars",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<VehicleData>(),
				ComponentType.ReadOnly<PersonalCarData>()
			}),
			filter = new PassengerCountFilter
			{
				m_Type = PassengerCountFilter.FilterType.NotEquals,
				m_Count = 1
			},
			icon = "Media/Game/Icons/GenericVehicle.svg"
		};
		AddCategory(category, editorAssetCategory);
		EditorAssetCategory category2 = new EditorAssetCategory
		{
			id = "Bikes",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<VehicleData>(),
				ComponentType.ReadOnly<PersonalCarData>()
			}),
			filter = new PassengerCountFilter
			{
				m_Type = PassengerCountFilter.FilterType.Equals,
				m_Count = 1
			},
			icon = "Media/Game/Icons/Bicycle.svg"
		};
		AddCategory(category2, editorAssetCategory);
	}

	private void GenerateServiceVehicleCategories(EditorAssetCategory parent)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Services"
		};
		AddCategory(editorAssetCategory, parent);
		GeneratePublicTransportVehicleCategory(TransportType.Bus, editorAssetCategory);
		GeneratePublicTransportVehicleCategory(TransportType.Taxi, editorAssetCategory);
		GeneratePublicTransportVehicleCategory(TransportType.Tram, editorAssetCategory);
		GeneratePublicTransportVehicleCategory(TransportType.Train, editorAssetCategory);
		GeneratePublicTransportVehicleCategory(TransportType.Subway, editorAssetCategory);
		GeneratePublicTransportVehicleCategory(TransportType.Ship, editorAssetCategory);
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Aircraft",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<AircraftData>(),
				ComponentType.Exclude<CargoTransportVehicleData>()
			}),
			icon = "Media/Game/Icons/airplane.svg"
		};
		AddCategory(category, editorAssetCategory);
		EditorAssetCategory category2 = new EditorAssetCategory
		{
			id = "Healthcare",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AmbulanceData>() }),
			icon = "Media/Game/Icons/Healthcare.svg"
		};
		AddCategory(category2, editorAssetCategory);
		EditorAssetCategory editorAssetCategory2 = new EditorAssetCategory
		{
			id = "Police"
		};
		EditorAssetCategory editorAssetCategory3 = editorAssetCategory2;
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<VehicleData>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PoliceCarData>(),
			ComponentType.ReadOnly<PublicTransportVehicleData>()
		};
		array[0] = val;
		editorAssetCategory3.entityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		editorAssetCategory2.icon = "Media/Game/Icons/Police.svg";
		editorAssetCategory2.filter = new PublicTransportTypeFilter
		{
			m_TransportType = TransportType.None,
			m_Purpose = PublicTransportPurpose.PrisonerTransport
		};
		EditorAssetCategory category3 = editorAssetCategory2;
		AddCategory(category3, editorAssetCategory);
		EditorAssetCategory category4 = new EditorAssetCategory
		{
			id = "Deathcare",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HearseData>() }),
			icon = "Media/Game/Icons/Deathcare.svg"
		};
		AddCategory(category4, editorAssetCategory);
		EditorAssetCategory category5 = new EditorAssetCategory
		{
			id = "FireRescue",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FireEngineData>() }),
			icon = "Media/Game/Icons/FireSafety.svg"
		};
		AddCategory(category5, editorAssetCategory);
		EditorAssetCategory category6 = new EditorAssetCategory
		{
			id = "Garbage",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageTruckData>() }),
			icon = "Media/Game/Icons/Garbage.svg"
		};
		AddCategory(category6, editorAssetCategory);
		EditorAssetCategory category7 = new EditorAssetCategory
		{
			id = "Parks",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MaintenanceVehicleData>() }),
			icon = "Media/Game/Icons/ParksAndRecreation.svg",
			filter = new MaintenanceTypeFilter
			{
				m_Type = MaintenanceType.Park
			}
		};
		AddCategory(category7, editorAssetCategory);
		EditorAssetCategory category8 = new EditorAssetCategory
		{
			id = "Roads",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MaintenanceVehicleData>() }),
			icon = "Media/Game/Icons/Roads.svg",
			filter = new MaintenanceTypeFilter
			{
				m_Type = (MaintenanceType.Road | MaintenanceType.Snow | MaintenanceType.Vehicle)
			}
		};
		AddCategory(category8, editorAssetCategory);
	}

	private void GeneratePublicTransportVehicleCategory(TransportType transportType, EditorAssetCategory parent)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = transportType.ToString(),
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<VehicleData>(),
				ComponentType.ReadOnly<PublicTransportVehicleData>()
			}),
			icon = $"Media/Game/Icons/{transportType}.svg",
			filter = new PublicTransportTypeFilter
			{
				m_TransportType = transportType,
				m_Purpose = PublicTransportPurpose.TransportLine
			}
		};
		AddCategory(category, parent);
	}

	private void GenerateIndustrialVehicleCategory(EditorAssetCategory parent)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Industrial"
		};
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<VehicleData>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CargoTransportVehicleData>(),
			ComponentType.ReadOnly<DeliveryTruckData>(),
			ComponentType.ReadOnly<WorkVehicleData>()
		};
		array[0] = val;
		editorAssetCategory.entityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		editorAssetCategory.icon = "Media/Game/Icons/ZoneIndustrial.svg";
		EditorAssetCategory category = editorAssetCategory;
		AddCategory(category, parent);
	}

	private void GeneratePropCategories()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Props",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
			{
				ComponentType.ReadOnly<StaticObjectData>(),
				ComponentType.Exclude<BuildingData>(),
				ComponentType.Exclude<NetObjectData>(),
				ComponentType.Exclude<BuildingExtensionData>(),
				ComponentType.Exclude<PillarData>(),
				ComponentType.Exclude<PlantData>(),
				ComponentType.Exclude<BrandObjectData>()
			}),
			includeChildCategories = false
		};
		AddCategory(editorAssetCategory);
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Brand Graphics",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
			{
				ComponentType.ReadOnly<StaticObjectData>(),
				ComponentType.ReadOnly<BrandObjectData>(),
				ComponentType.Exclude<BuildingData>(),
				ComponentType.Exclude<NetObjectData>(),
				ComponentType.Exclude<BuildingExtensionData>(),
				ComponentType.Exclude<PillarData>(),
				ComponentType.Exclude<PlantData>()
			})
		};
		AddCategory(category, editorAssetCategory);
	}

	private void GenerateFoliageCategories()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Foliage",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PlantData>() }),
			icon = "Media/Game/Icons/Vegetation.svg",
			includeChildCategories = false
		};
		AddCategory(editorAssetCategory);
		GenerateTreeCategories(editorAssetCategory);
		GenerateBushCategories(editorAssetCategory);
	}

	private void GenerateTreeCategories(EditorAssetCategory parent)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Trees"
		};
		AddCategory(editorAssetCategory, parent);
		Enumerator<Entity> enumerator = ((EntityQuery)(ref m_ThemeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				if (m_PrefabSystem.TryGetPrefab<ThemePrefab>(current, out var prefab))
				{
					string icon = prefab.GetComponent<UIObject>()?.m_Icon;
					EditorAssetCategory category = new EditorAssetCategory
					{
						id = prefab.assetPrefix,
						entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TreeData>() }),
						icon = icon,
						filter = new ThemeFilter
						{
							m_Theme = current,
							m_DefaultResult = false
						}
					};
					AddCategory(category, editorAssetCategory);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		EditorAssetCategory category2 = new EditorAssetCategory
		{
			id = "Shared",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TreeData>() }),
			filter = new ThemeFilter
			{
				m_Theme = Entity.Null,
				m_DefaultResult = true
			}
		};
		AddCategory(category2, editorAssetCategory);
	}

	private void GenerateBushCategories(EditorAssetCategory parent)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Bushes",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<PlantData>(),
				ComponentType.Exclude<TreeData>()
			})
		};
		AddCategory(category, parent);
	}

	private void GenerateRoadCategory()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Roads"
		};
		AddCategory(editorAssetCategory);
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Roads",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<RoadData>(),
				ComponentType.Exclude<BridgeData>()
			}),
			icon = "Media/Game/Icons/Roads.svg"
		};
		AddCategory(category, editorAssetCategory);
		EditorAssetCategory category2 = new EditorAssetCategory
		{
			id = "Intersections",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<AssetStampData>(),
				ComponentType.ReadOnly<Game.Prefabs.SubNet>()
			})
		};
		AddCategory(category2, editorAssetCategory);
	}

	private void GenerateTrackCategories()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Tracks",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrackData>() }),
			includeChildCategories = false
		};
		AddCategory(editorAssetCategory);
		GenerateTrackTypeCategory(TrackTypes.Train, editorAssetCategory);
		GenerateTrackTypeCategory(TrackTypes.Tram, editorAssetCategory);
		GenerateTrackTypeCategory(TrackTypes.Subway, editorAssetCategory);
	}

	private void GenerateTrackTypeCategory(TrackTypes trackTypes, EditorAssetCategory parent)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = trackTypes.ToString(),
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrackData>() }),
			filter = new TrackTypeFilter
			{
				m_TrackType = trackTypes
			}
		};
		AddCategory(category, parent);
	}

	private void GenerateEffectCategories()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Effects",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EffectData>() })
		};
		AddCategory(editorAssetCategory);
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "VFX",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<VFXData>()
			})
		};
		AddCategory(category, editorAssetCategory);
		EditorAssetCategory category2 = new EditorAssetCategory
		{
			id = "Audio",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<AudioEffectData>()
			})
		};
		AddCategory(category2, editorAssetCategory);
		EditorAssetCategory category3 = new EditorAssetCategory
		{
			id = "Lights",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<LightEffectData>()
			})
		};
		AddCategory(category3, editorAssetCategory);
	}

	private void GenerateLocationCategories()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Locations"
		};
		AddCategory(editorAssetCategory);
		EditorAssetCategory editorAssetCategory2 = new EditorAssetCategory
		{
			id = "Spawners"
		};
		AddCategory(editorAssetCategory2, editorAssetCategory);
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Animals",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CreatureSpawnData>() })
		};
		AddCategory(category, editorAssetCategory2);
		EditorAssetCategory category2 = new EditorAssetCategory
		{
			id = "Vehicles",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TrafficSpawnerData>() })
		};
		AddCategory(category2, editorAssetCategory2);
	}

	private void GenerateBridgeCategory()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Bridges",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BridgeData>() }),
			icon = "Media/Game/Icons/CableStayed.svg"
		};
		AddCategory(category);
	}

	private void GenerateCharacterCategories()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory editorAssetCategory = new EditorAssetCategory
		{
			id = "Characters",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CreatureData>() }),
			includeChildCategories = false
		};
		AddCategory(editorAssetCategory);
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "People",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HumanData>() })
		};
		AddCategory(category, editorAssetCategory);
		EditorAssetCategory editorAssetCategory2 = new EditorAssetCategory
		{
			id = "Animals",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AnimalData>() })
		};
		AddCategory(editorAssetCategory2, editorAssetCategory);
		EditorAssetCategory category2 = new EditorAssetCategory
		{
			id = "Pets",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PetData>() })
		};
		AddCategory(category2, editorAssetCategory2);
		EditorAssetCategory category3 = new EditorAssetCategory
		{
			id = "Livestock",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DomesticatedData>() })
		};
		AddCategory(category3, editorAssetCategory2);
		EditorAssetCategory category4 = new EditorAssetCategory
		{
			id = "Wildlife",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WildlifeData>() })
		};
		AddCategory(category4, editorAssetCategory2);
	}

	private void GenerateAreaCategories()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Areas",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<AreaData>(),
				ComponentType.Exclude<SurfaceData>()
			})
		};
		AddCategory(category);
	}

	private void GenerateSurfaceCategories()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EditorAssetCategory category = new EditorAssetCategory
		{
			id = "Surfaces",
			entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SurfaceData>() })
		};
		AddCategory(category);
	}

	private void AddOverrides()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_Overrides)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < val.Length; i++)
		{
			if (!m_PrefabSystem.TryGetPrefab<PrefabBase>(val[i], out var prefab))
			{
				continue;
			}
			EditorAssetCategoryOverride component = prefab.GetComponent<EditorAssetCategoryOverride>();
			if (component.m_IncludeCategories != null)
			{
				for (int j = 0; j < component.m_IncludeCategories.Length; j++)
				{
					string text = component.m_IncludeCategories[j];
					if (m_PathMap.TryGetValue(text, out var value))
					{
						value.AddEntity(val[i]);
					}
					else
					{
						CreateCategory(text).AddEntity(val[i]);
					}
				}
			}
			if (component.m_ExcludeCategories == null)
			{
				continue;
			}
			for (int k = 0; k < component.m_ExcludeCategories.Length; k++)
			{
				string key = component.m_ExcludeCategories[k];
				if (m_PathMap.TryGetValue(key, out var value2))
				{
					value2.AddExclusion(val[i]);
				}
			}
		}
	}

	private EditorAssetCategory CreateCategory(string path)
	{
		string[] array = path.Split("/", StringSplitOptions.None);
		EditorAssetCategory editorAssetCategory = null;
		string text = null;
		for (int i = 0; i < array.Length; i++)
		{
			text = ((text != null) ? string.Join("/", text, array[i]) : array[i]);
			if (m_PathMap.TryGetValue(text, out var value))
			{
				editorAssetCategory = value;
				continue;
			}
			EditorAssetCategory editorAssetCategory2 = new EditorAssetCategory
			{
				id = array[i]
			};
			AddCategory(editorAssetCategory2, editorAssetCategory);
			editorAssetCategory = editorAssetCategory2;
		}
		return editorAssetCategory;
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
	public EditorAssetCategorySystem()
	{
	}
}
