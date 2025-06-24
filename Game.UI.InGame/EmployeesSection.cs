using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Tools;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class EmployeesSection : InfoSectionBase
{
	private EntityQuery m_DistrictBuildingQuery;

	protected override string group => "EmployeesSection";

	private int employeeCount { get; set; }

	private int maxEmployees { get; set; }

	private EmploymentData educationDataEmployees { get; set; }

	private EmploymentData educationDataWorkplaces { get; set; }

	private NativeList<Entity> districtBuildings { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<CurrentDistrict>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Renter>(),
			ComponentType.ReadOnly<Employee>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		};
		array[0] = val;
		m_DistrictBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		districtBuildings = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		districtBuildings.Dispose();
		base.OnDestroy();
	}

	protected override void Reset()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		employeeCount = 0;
		maxEmployees = 0;
		educationDataEmployees = default(EmploymentData);
		educationDataWorkplaces = default(EmploymentData);
		districtBuildings.Clear();
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Area>(selectedEntity))
			{
				return DisplayForDistrict();
			}
		}
		return HasEmployees(selectedEntity, selectedPrefab);
	}

	private bool HasEmployees(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		EntityManager entityManager;
		if (EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(entity))
			{
				SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
				if (val.Length == 0 && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefab, ref spawnableBuildingData))
				{
					m_PrefabSystem.TryGetPrefab<ZonePrefab>(spawnableBuildingData.m_ZonePrefab, out var prefab2);
					if ((Object)(object)prefab2 != (Object)null)
					{
						if (prefab2.m_AreaType != Game.Zones.AreaType.Commercial)
						{
							return prefab2.m_AreaType == Game.Zones.AreaType.Industrial;
						}
						return true;
					}
					return false;
				}
				for (int i = 0; i < val.Length; i++)
				{
					Entity renter = val[i].m_Renter;
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(renter))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<Employee>(renter))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							return ((EntityManager)(ref entityManager)).HasComponent<WorkProvider>(renter);
						}
						return false;
					}
				}
				return false;
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Employee>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<WorkProvider>(entity))
			{
				return ((ComponentSystemBase)this).Enabled;
			}
		}
		return false;
	}

	private bool DisplayForDistrict()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_DistrictBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<CurrentDistrict> val2 = ((EntityQuery)(ref m_DistrictBuildingQuery)).ToComponentDataArray<CurrentDistrict>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<PrefabRef> val3 = ((EntityQuery)(ref m_DistrictBuildingQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				if (!(val2[i].m_District != selectedEntity))
				{
					Entity entity = val[i];
					if (HasEmployees(entity, val3[i].m_Prefab))
					{
						return true;
					}
				}
			}
		}
		finally
		{
			val.Dispose();
			val2.Dispose();
			val3.Dispose();
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
		if (base.visible)
		{
			AddEmployees();
			base.visible = maxEmployees > 0;
		}
	}

	protected override void OnProcess()
	{
	}

	private void AddEmployees()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<ServiceUsage>(selectedEntity))
		{
			base.tooltipKeys.Add("ServiceUsage");
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Area>(selectedEntity))
			{
				UpdateForDistricts();
				return;
			}
		}
		AddEmployees(selectedEntity);
	}

	private void UpdateForDistricts()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_DistrictBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<CurrentDistrict> val2 = ((EntityQuery)(ref m_DistrictBuildingQuery)).ToComponentDataArray<CurrentDistrict>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<PrefabRef> val3 = ((EntityQuery)(ref m_DistrictBuildingQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				if (!(val2[i].m_District != selectedEntity))
				{
					Entity entity = val[i];
					if (HasEmployees(entity, val3[i].m_Prefab))
					{
						AddEmployees(entity);
					}
				}
			}
		}
		finally
		{
			val.Dispose();
			val2.Dispose();
			val3.Dispose();
		}
	}

	private void AddEmployees(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Entity prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity).m_Prefab;
		Entity entity2 = GetEntity(entity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Entity prefab2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity2).m_Prefab;
		int buildingLevel = 1;
		SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
		PropertyRenter propertyRenter = default(PropertyRenter);
		PrefabRef prefabRef = default(PrefabRef);
		SpawnableBuildingData spawnableBuildingData2 = default(SpawnableBuildingData);
		if (EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefab, ref spawnableBuildingData))
		{
			buildingLevel = spawnableBuildingData.m_Level;
		}
		else if (EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, entity, ref propertyRenter) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref prefabRef) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref spawnableBuildingData2))
		{
			buildingLevel = spawnableBuildingData2.m_Level;
		}
		DynamicBuffer<Employee> employees = default(DynamicBuffer<Employee>);
		WorkProvider workProvider = default(WorkProvider);
		if (EntitiesExtensions.TryGetBuffer<Employee>(((ComponentSystemBase)this).EntityManager, entity2, true, ref employees) && EntitiesExtensions.TryGetComponent<WorkProvider>(((ComponentSystemBase)this).EntityManager, entity2, ref workProvider))
		{
			employeeCount += employees.Length;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			WorkplaceComplexity complexity = ((EntityManager)(ref entityManager)).GetComponentData<WorkplaceData>(prefab2).m_Complexity;
			EmploymentData workplacesData = EmploymentData.GetWorkplacesData(workProvider.m_MaxWorkers, buildingLevel, complexity);
			maxEmployees += workplacesData.total;
			educationDataWorkplaces += workplacesData;
			educationDataEmployees += EmploymentData.GetEmployeesData(employees, workplacesData.total - employees.Length);
		}
	}

	private Entity GetEntity(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Renter> val = default(DynamicBuffer<Renter>);
		if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(entity) && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity renter = val[i].m_Renter;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(renter))
				{
					return renter;
				}
			}
		}
		return entity;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("employeeCount");
		writer.Write(employeeCount);
		writer.PropertyName("maxEmployees");
		writer.Write(maxEmployees);
		writer.PropertyName("educationDataEmployees");
		JsonWriterExtensions.Write<EmploymentData>(writer, educationDataEmployees);
		writer.PropertyName("educationDataWorkplaces");
		JsonWriterExtensions.Write<EmploymentData>(writer, educationDataWorkplaces);
	}

	[Preserve]
	public EmployeesSection()
	{
	}
}
