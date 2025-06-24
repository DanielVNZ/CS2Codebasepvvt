using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ProductionCompanyUISystem : UISystemBase
{
	[BurstCompile]
	private struct MapCompanyStatisticsJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<IndustrialCompany> m_IndustrialCompanyType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public Resource m_Resource;

		public ParallelWriter<ProductionCompanyInfo> m_Queue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			bool industrial = ((ArchetypeChunk)(ref chunk)).Has<IndustrialCompany>(ref m_IndustrialCompanyType);
			NativeArray<PropertyRenter> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			BufferAccessor<Employee> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (m_IndustrialProcessDatas[nativeArray2[i].m_Prefab].m_Output.m_Resource == m_Resource)
				{
					Entity property = nativeArray[i].m_Property;
					if (m_PrefabRefs.HasComponent(property))
					{
						SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildingDatas[m_PrefabRefs[nativeArray[i].m_Property].m_Prefab];
						m_Queue.Enqueue(new ProductionCompanyInfo
						{
							m_Industrial = industrial,
							m_Level = spawnableBuildingData.m_Level,
							m_Workers = bufferAccessor[i].Length
						});
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct ProductionCompanyInfo
	{
		public bool m_Industrial;

		public int m_Level;

		public int m_Workers;
	}

	private struct ProductionLevelInfo : IEquatable<ProductionLevelInfo>
	{
		public int m_IndustrialCompanies;

		public int m_IndustrialWorkers;

		public int m_CommercialCompanies;

		public int m_CommercialWorkers;

		public bool Equals(ProductionLevelInfo other)
		{
			if (other.m_IndustrialCompanies == m_IndustrialCompanies && other.m_IndustrialWorkers == m_IndustrialWorkers && other.m_CommercialCompanies == m_CommercialCompanies)
			{
				return other.m_CommercialWorkers == m_CommercialWorkers;
			}
			return false;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<IndustrialCompany> __Game_Companies_IndustrialCompany_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			__Game_Companies_IndustrialCompany_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<IndustrialCompany>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
		}
	}

	private static readonly string kGroup = "production";

	private static readonly int kLevels = 5;

	private IBudgetSystem m_BudgetSystem;

	private ResourceSystem m_ResourceSystem;

	private PrefabSystem m_PrefabSystem;

	private Dictionary<string, Resource> m_ResourceIDMap;

	private RawValueBinding m_ProductionCompanyInfoBinding;

	private ValueBinding<int> m_IndustrialCompanyWealthBinding;

	private ValueBinding<int> m_CommercialCompanyWealthBinding;

	private NativeArray<ProductionLevelInfo> m_CachedValues;

	private NativeArray<ProductionLevelInfo> m_Values;

	private Resource m_SelectedResource;

	private NativeQueue<ProductionCompanyInfo> m_ProductionCompanyInfoQueue;

	private EntityQuery m_CompanyQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0115: Expected O, but got Unknown
		base.OnCreate();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_BudgetSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BudgetSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<Employee>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<IndustrialCompany>(),
			ComponentType.ReadOnly<CommercialCompany>()
		};
		array[0] = val;
		m_CompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_SelectedResource = Resource.NoResource;
		m_ResourceIDMap = new Dictionary<string, Resource>();
		m_CachedValues = new NativeArray<ProductionLevelInfo>(kLevels, (Allocator)4, (NativeArrayOptions)1);
		m_Values = new NativeArray<ProductionLevelInfo>(kLevels, (Allocator)4, (NativeArrayOptions)1);
		m_ProductionCompanyInfoQueue = new NativeQueue<ProductionCompanyInfo>(AllocatorHandle.op_Implicit((Allocator)4));
		RawValueBinding val2 = new RawValueBinding(kGroup, "productionCompanyInfo", (Action<IJsonWriter>)UpdateProductionCompanyInfo);
		RawValueBinding binding = val2;
		m_ProductionCompanyInfoBinding = val2;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_IndustrialCompanyWealthBinding = new ValueBinding<int>(kGroup, "industrialCompanyWealth", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_CommercialCompanyWealthBinding = new ValueBinding<int>(kGroup, "commercialCompanyWealth", 0, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<string>(kGroup, "selectResource", (Action<string>)OnSelectResource, (IReader<string>)null));
		RebuildResourceIDMap();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_CachedValues.Dispose();
		m_Values.Dispose();
		m_ProductionCompanyInfoQueue.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (((EventBindingBase)m_ProductionCompanyInfoBinding).active)
		{
			PatchProductionCompanyInfo();
		}
		if (((EventBindingBase)m_IndustrialCompanyWealthBinding).active)
		{
			m_IndustrialCompanyWealthBinding.Update((m_SelectedResource != Resource.NoResource) ? m_BudgetSystem.GetCompanyWealth(service: false, m_SelectedResource) : 0);
		}
		if (((EventBindingBase)m_CommercialCompanyWealthBinding).active)
		{
			m_CommercialCompanyWealthBinding.Update((m_SelectedResource != Resource.NoResource) ? m_BudgetSystem.GetCompanyWealth(service: true, m_SelectedResource) : 0);
		}
	}

	private void UpdateProductionCompanyInfo(IJsonWriter binder)
	{
		JsonWriterExtensions.ArrayBegin(binder, kLevels);
		for (int i = 0; i < kLevels; i++)
		{
			binder.TypeBegin("production.ProductionCompanyInfo");
			binder.PropertyName("industrialCompanies");
			binder.Write(0);
			binder.PropertyName("industrialWorkers");
			binder.Write(0);
			binder.PropertyName("commercialCompanies");
			binder.Write(0);
			binder.PropertyName("commercialWorkers");
			binder.Write(0);
			binder.TypeEnd();
		}
		binder.ArrayEnd();
	}

	private void PatchProductionCompanyInfo()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		m_ProductionCompanyInfoQueue.Clear();
		for (int i = 0; i < m_Values.Length; i++)
		{
			m_Values[i] = default(ProductionLevelInfo);
		}
		if (m_SelectedResource != Resource.NoResource)
		{
			MapCompanyStatisticsJob mapCompanyStatisticsJob = new MapCompanyStatisticsJob
			{
				m_IndustrialCompanyType = InternalCompilerInterface.GetComponentTypeHandle<IndustrialCompany>(ref __TypeHandle.__Game_Companies_IndustrialCompany_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EmployeeType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Resource = m_SelectedResource,
				m_Queue = m_ProductionCompanyInfoQueue.AsParallelWriter()
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<MapCompanyStatisticsJob>(mapCompanyStatisticsJob, m_CompanyQuery, ((SystemBase)this).Dependency);
			JobHandle dependency = ((SystemBase)this).Dependency;
			((JobHandle)(ref dependency)).Complete();
			ProductionCompanyInfo productionCompanyInfo = default(ProductionCompanyInfo);
			while (m_ProductionCompanyInfoQueue.TryDequeue(ref productionCompanyInfo))
			{
				ProductionLevelInfo productionLevelInfo = m_Values[productionCompanyInfo.m_Level - 1];
				if (productionCompanyInfo.m_Industrial)
				{
					productionLevelInfo.m_IndustrialCompanies++;
					productionLevelInfo.m_IndustrialWorkers += productionCompanyInfo.m_Workers;
				}
				else
				{
					productionLevelInfo.m_CommercialCompanies++;
					productionLevelInfo.m_CommercialWorkers += productionCompanyInfo.m_Workers;
				}
				m_Values[productionCompanyInfo.m_Level - 1] = productionLevelInfo;
			}
		}
		for (int j = 0; j < m_CachedValues.Length; j++)
		{
			ProductionLevelInfo productionLevelInfo2 = m_CachedValues[j];
			ProductionLevelInfo productionLevelInfo3 = m_Values[j];
			m_CachedValues[j] = productionLevelInfo3;
			if (productionLevelInfo2.m_IndustrialCompanies != productionLevelInfo3.m_IndustrialCompanies)
			{
				Patch(j, "industrialCompanies", productionLevelInfo3.m_IndustrialCompanies);
			}
			if (productionLevelInfo2.m_IndustrialWorkers != productionLevelInfo3.m_IndustrialWorkers)
			{
				Patch(j, "industrialWorkers", productionLevelInfo3.m_IndustrialWorkers);
			}
			if (productionLevelInfo2.m_CommercialCompanies != productionLevelInfo3.m_CommercialCompanies)
			{
				Patch(j, "commercialCompanies", productionLevelInfo3.m_CommercialCompanies);
			}
			if (productionLevelInfo2.m_CommercialWorkers != productionLevelInfo3.m_CommercialWorkers)
			{
				Patch(j, "commercialWorkers", productionLevelInfo3.m_CommercialWorkers);
			}
		}
	}

	private void Patch(int index, string fieldName, int value)
	{
		IJsonWriter obj = m_ProductionCompanyInfoBinding.PatchBegin();
		obj.ArrayBegin(2u);
		obj.Write(index);
		obj.Write(fieldName);
		obj.ArrayEnd();
		obj.Write(value);
		m_ProductionCompanyInfoBinding.PatchEnd();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		RebuildResourceIDMap();
		for (int i = 0; i < m_CachedValues.Length; i++)
		{
			m_CachedValues[i] = default(ProductionLevelInfo);
		}
		for (int j = 0; j < m_Values.Length; j++)
		{
			m_Values[j] = default(ProductionLevelInfo);
		}
		m_ProductionCompanyInfoQueue.Clear();
	}

	private void RebuildResourceIDMap()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		m_ResourceIDMap.Clear();
		ResourceIterator iterator = ResourceIterator.GetIterator();
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		while (iterator.Next())
		{
			Entity val = prefabs[iterator.resource];
			if (val != Entity.Null)
			{
				ResourcePrefab prefab = m_PrefabSystem.GetPrefab<ResourcePrefab>(val);
				m_ResourceIDMap[((Object)prefab).name] = iterator.resource;
			}
		}
	}

	private void OnSelectResource(string resourceID)
	{
		if (m_ResourceIDMap.TryGetValue(resourceID, out var value))
		{
			m_SelectedResource = value;
		}
		else
		{
			m_SelectedResource = Resource.NoResource;
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
	public ProductionCompanyUISystem()
	{
	}
}
