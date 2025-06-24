using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class WorkplacesInfoviewUISystem : InfoviewUISystemBase
{
	private enum Result
	{
		Workplaces,
		Employees,
		Count
	}

	[BurstCompile]
	private struct CalculateWorkplaceDataJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> m_WorkProviderHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefFromEntity;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_WorkplaceDataFromEntity;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingFromEntity;

		public NativeArray<int> m_IntResults;

		public NativeArray<EmploymentData> m_EmploymentDataResults;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityHandle);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefHandle);
			NativeArray<PropertyRenter> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterHandle);
			NativeArray<WorkProvider> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkProvider>(ref m_WorkProviderHandle);
			BufferAccessor<Employee> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeHandle);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				int buildingLevel = 1;
				WorkProvider workProvider = nativeArray4[i];
				DynamicBuffer<Employee> employees = bufferAccessor[i];
				PrefabRef prefabRef = nativeArray2[i];
				WorkplaceData workplaceData = m_WorkplaceDataFromEntity[prefabRef.m_Prefab];
				if (((ArchetypeChunk)(ref chunk)).Has<PropertyRenter>(ref m_PropertyRenterHandle))
				{
					PropertyRenter propertyRenter = nativeArray3[i];
					PrefabRef prefabRef2 = m_PrefabRefFromEntity[propertyRenter.m_Property];
					if (m_SpawnableBuildingFromEntity.HasComponent(prefabRef2.m_Prefab))
					{
						buildingLevel = m_SpawnableBuildingFromEntity[prefabRef2.m_Prefab].m_Level;
					}
				}
				EmploymentData workplacesData = EmploymentData.GetWorkplacesData(workProvider.m_MaxWorkers, buildingLevel, workplaceData.m_Complexity);
				EmploymentData employeesData = EmploymentData.GetEmployeesData(employees, workplacesData.total - employees.Length);
				ref NativeArray<int> reference = ref m_IntResults;
				reference[0] = reference[0] + workplacesData.total;
				reference = ref m_IntResults;
				reference[1] = reference[1] + employees.Length;
				ref NativeArray<EmploymentData> reference2 = ref m_EmploymentDataResults;
				reference2[0] = reference2[0] + workplacesData;
				reference2 = ref m_EmploymentDataResults;
				reference2[1] = reference2[1] + employeesData;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Companies_WorkProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkProvider>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
		}
	}

	private const string kGroup = "workplaces";

	private EntityQuery m_WorkplaceQuery;

	private EntityQuery m_WorkplaceModifiedQuery;

	private GetterValueBinding<EmploymentData> m_EmployeesData;

	private GetterValueBinding<EmploymentData> m_WorkplacesData;

	private GetterValueBinding<int> m_Workplaces;

	private GetterValueBinding<int> m_Workers;

	private NativeArray<int> m_IntResults;

	private NativeArray<EmploymentData> m_EmploymentDataResults;

	private TypeHandle __TypeHandle;

	protected override bool Active
	{
		get
		{
			if (!base.Active && !((EventBindingBase)m_EmployeesData).active)
			{
				return ((EventBindingBase)m_WorkplacesData).active;
			}
			return true;
		}
	}

	protected override bool Modified => !((EntityQuery)(ref m_WorkplaceModifiedQuery)).IsEmptyIgnoreFilter;

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
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Employee>(),
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<Building>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_WorkplaceQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Employee>(),
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		m_WorkplaceModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_IntResults = new NativeArray<int>(2, (Allocator)4, (NativeArrayOptions)1);
		m_EmploymentDataResults = new NativeArray<EmploymentData>(2, (Allocator)4, (NativeArrayOptions)1);
		AddBinding((IBinding)(object)(m_WorkplacesData = new GetterValueBinding<EmploymentData>("workplaces", "workplacesData", (Func<EmploymentData>)GetWorkplacesData, (IWriter<EmploymentData>)(object)new ValueWriter<EmploymentData>(), (EqualityComparer<EmploymentData>)null)));
		AddBinding((IBinding)(object)(m_EmployeesData = new GetterValueBinding<EmploymentData>("workplaces", "employeesData", (Func<EmploymentData>)GetEmployeesData, (IWriter<EmploymentData>)(object)new ValueWriter<EmploymentData>(), (EqualityComparer<EmploymentData>)null)));
		AddBinding((IBinding)(object)(m_Workplaces = new GetterValueBinding<int>("workplaces", "workplaces", (Func<int>)GetWorkplaces, (IWriter<int>)null, (EqualityComparer<int>)null)));
		AddBinding((IBinding)(object)(m_Workers = new GetterValueBinding<int>("workplaces", "employees", (Func<int>)GetWorkers, (IWriter<int>)null, (EqualityComparer<int>)null)));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_IntResults.Dispose();
		m_EmploymentDataResults.Dispose();
		base.OnDestroy();
	}

	protected override void PerformUpdate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		ResetResults();
		JobHandle val = JobChunkExtensions.Schedule<CalculateWorkplaceDataJob>(new CalculateWorkplaceDataJob
		{
			m_EntityHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeHandle = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkProviderHandle = InternalCompilerInterface.GetComponentTypeHandle<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterHandle = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkplaceDataFromEntity = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingFromEntity = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IntResults = m_IntResults,
			m_EmploymentDataResults = m_EmploymentDataResults
		}, m_WorkplaceQuery, ((SystemBase)this).Dependency);
		((JobHandle)(ref val)).Complete();
		m_EmployeesData.Update();
		m_WorkplacesData.Update();
		m_Workplaces.Update();
		m_Workers.Update();
	}

	private void ResetResults()
	{
		for (int i = 0; i < 2; i++)
		{
			m_EmploymentDataResults[i] = default(EmploymentData);
			m_IntResults[i] = 0;
		}
	}

	private int GetWorkplaces()
	{
		if (!m_IntResults.IsCreated || m_IntResults.Length != 2)
		{
			return 0;
		}
		return m_IntResults[0];
	}

	private int GetWorkers()
	{
		if (!m_IntResults.IsCreated || m_IntResults.Length != 2)
		{
			return 0;
		}
		return m_IntResults[1];
	}

	private EmploymentData GetWorkplacesData()
	{
		if (!m_EmploymentDataResults.IsCreated || m_EmploymentDataResults.Length != 2)
		{
			return default(EmploymentData);
		}
		return m_EmploymentDataResults[0];
	}

	private EmploymentData GetEmployeesData()
	{
		if (!m_EmploymentDataResults.IsCreated || m_EmploymentDataResults.Length != 2)
		{
			return default(EmploymentData);
		}
		return m_EmploymentDataResults[1];
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
	public WorkplacesInfoviewUISystem()
	{
	}
}
