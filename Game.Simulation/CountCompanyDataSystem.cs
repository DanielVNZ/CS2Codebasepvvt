using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Debug;
using Game.Economy;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CountCompanyDataSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public struct CommercialCompanyDatas
	{
		public NativeArray<int> m_CurrentServiceWorkers;

		public NativeArray<int> m_MaxServiceWorkers;

		public NativeArray<int> m_ProduceCapacity;

		public NativeArray<int> m_CurrentAvailables;

		public NativeArray<int> m_TotalAvailables;

		public NativeArray<int> m_ServiceCompanies;

		public NativeArray<int> m_ServicePropertyless;
	}

	public struct IndustrialCompanyDatas
	{
		public NativeArray<int> m_CurrentProductionWorkers;

		public NativeArray<int> m_MaxProductionWorkers;

		public NativeArray<int> m_Production;

		public NativeArray<int> m_Demand;

		public NativeArray<int> m_ProductionCompanies;

		public NativeArray<int> m_ProductionPropertyless;
	}

	private struct CompanyDataItem
	{
		public int m_Resource;

		public int m_CurrentProductionWorkers;

		public int m_MaxProductionWorkers;

		public int m_CurrentServiceWorkers;

		public int m_MaxServiceWorkers;

		public int m_Production;

		public int m_SalesCapacities;

		public int m_CurrentAvailables;

		public int m_TotalAvailables;

		public int m_Demand;

		public int m_ProductionCompanies;

		public int m_ServiceCompanies;

		public int m_ProductionPropertyless;

		public int m_ServicePropertyless;
	}

	[BurstCompile]
	private struct SumJob : IJob
	{
		public NativeArray<int> m_CurrentProductionWorkers;

		public NativeArray<int> m_MaxProductionWorkers;

		public NativeArray<int> m_CurrentServiceWorkers;

		public NativeArray<int> m_MaxServiceWorkers;

		public NativeArray<int> m_Production;

		public NativeArray<int> m_SalesCapacities;

		public NativeArray<int> m_CurrentAvailables;

		public NativeArray<int> m_TotalAvailables;

		public NativeArray<int> m_Demand;

		public NativeArray<int> m_ProductionCompanies;

		public NativeArray<int> m_ServiceCompanies;

		public NativeArray<int> m_ProductionPropertyless;

		public NativeArray<int> m_ServicePropertyless;

		public NativeQueue<CompanyDataItem> m_DataQueue;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			CollectionUtils.Fill<int>(m_CurrentProductionWorkers, 0);
			CollectionUtils.Fill<int>(m_MaxProductionWorkers, 0);
			CollectionUtils.Fill<int>(m_CurrentServiceWorkers, 0);
			CollectionUtils.Fill<int>(m_MaxServiceWorkers, 0);
			CollectionUtils.Fill<int>(m_Production, 0);
			CollectionUtils.Fill<int>(m_SalesCapacities, 0);
			CollectionUtils.Fill<int>(m_CurrentAvailables, 0);
			CollectionUtils.Fill<int>(m_TotalAvailables, 0);
			CollectionUtils.Fill<int>(m_Demand, 0);
			CollectionUtils.Fill<int>(m_ProductionCompanies, 0);
			CollectionUtils.Fill<int>(m_ServiceCompanies, 0);
			CollectionUtils.Fill<int>(m_ProductionPropertyless, 0);
			CollectionUtils.Fill<int>(m_ServicePropertyless, 0);
			CompanyDataItem companyDataItem = default(CompanyDataItem);
			while (m_DataQueue.TryDequeue(ref companyDataItem))
			{
				int num = companyDataItem.m_Resource;
				ref NativeArray<int> reference = ref m_CurrentProductionWorkers;
				int num2 = num;
				reference[num2] += companyDataItem.m_CurrentProductionWorkers;
				reference = ref m_MaxProductionWorkers;
				num2 = num;
				reference[num2] += companyDataItem.m_MaxProductionWorkers;
				reference = ref m_CurrentServiceWorkers;
				num2 = num;
				reference[num2] += companyDataItem.m_CurrentServiceWorkers;
				reference = ref m_MaxServiceWorkers;
				num2 = num;
				reference[num2] += companyDataItem.m_MaxServiceWorkers;
				reference = ref m_Production;
				num2 = num;
				reference[num2] += companyDataItem.m_Production;
				reference = ref m_SalesCapacities;
				num2 = num;
				reference[num2] += companyDataItem.m_SalesCapacities;
				reference = ref m_CurrentAvailables;
				num2 = num;
				reference[num2] += companyDataItem.m_CurrentAvailables;
				reference = ref m_TotalAvailables;
				num2 = num;
				reference[num2] += companyDataItem.m_TotalAvailables;
				reference = ref m_Demand;
				num2 = num;
				reference[num2] += companyDataItem.m_Demand;
				reference = ref m_ProductionCompanies;
				num2 = num;
				reference[num2] += companyDataItem.m_ProductionCompanies;
				reference = ref m_ServiceCompanies;
				num2 = num;
				reference[num2] += companyDataItem.m_ServiceCompanies;
				reference = ref m_ProductionPropertyless;
				num2 = num;
				reference[num2] += companyDataItem.m_ProductionPropertyless;
				reference = ref m_ServicePropertyless;
				num2 = num;
				reference[num2] += companyDataItem.m_ServicePropertyless;
			}
		}
	}

	[BurstCompile]
	private struct CountCompanyDataJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> m_WorkProviderType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceAvailable> m_ServiceAvailableType;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public BufferLookup<Employee> m_Employees;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public BufferLookup<Efficiency> m_BuildingEfficiencyBuf;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> m_ServiceCompanyDatas;

		public EconomyParameterData m_EconomyParameters;

		public ParallelWriter<CompanyDataItem> m_DataQueue;

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
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkProvider> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkProvider>(ref m_WorkProviderType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<ServiceAvailable> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceAvailable>(ref m_ServiceAvailableType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<ServiceAvailable>(ref m_ServiceAvailableType);
			int resourceCount = EconomyUtils.ResourceCount;
			NativeArray<int> val = default(NativeArray<int>);
			val._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val2 = default(NativeArray<int>);
			val2._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val3 = default(NativeArray<int>);
			val3._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val4 = default(NativeArray<int>);
			val4._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val5 = default(NativeArray<int>);
			val5._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val6 = default(NativeArray<int>);
			val6._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val7 = default(NativeArray<int>);
			val7._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val8 = default(NativeArray<int>);
			val8._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val9 = default(NativeArray<int>);
			val9._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val10 = default(NativeArray<int>);
			val10._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val11 = default(NativeArray<int>);
			val11._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val12 = default(NativeArray<int>);
			val12._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val13 = default(NativeArray<int>);
			val13._002Ector(resourceCount, (Allocator)2, (NativeArrayOptions)1);
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val14 = nativeArray[i];
				WorkProvider workProvider = nativeArray2[i];
				Entity prefab = nativeArray3[i].m_Prefab;
				if (!m_IndustrialProcessDatas.HasComponent(prefab))
				{
					continue;
				}
				IndustrialProcessData processData = m_IndustrialProcessDatas[prefab];
				Resource resource = processData.m_Output.m_Resource;
				if (m_PropertyRenters.HasComponent(val14))
				{
					if (resource != Resource.NoResource)
					{
						int resourceIndex = EconomyUtils.GetResourceIndex(resource);
						Entity property = m_PropertyRenters[val14].m_Property;
						DynamicBuffer<Employee> employees = m_Employees[val14];
						int maxWorkers = workProvider.m_MaxWorkers;
						if (flag)
						{
							ServiceCompanyData serviceCompanyData = m_ServiceCompanyDatas[prefab];
							ref NativeArray<int> reference = ref val7;
							int num = resourceIndex;
							reference[num] += math.clamp(nativeArray4[i].m_ServiceAvailable, 0, serviceCompanyData.m_MaxService);
							reference = ref val8;
							num = resourceIndex;
							reference[num] += serviceCompanyData.m_MaxService;
						}
						float buildingEfficiency = 1f;
						if (m_BuildingEfficiencyBuf.TryGetBuffer(property, ref buffer))
						{
							buildingEfficiency = BuildingUtils.GetEfficiency(buffer);
						}
						int companyProductionPerDay = EconomyUtils.GetCompanyProductionPerDay(buildingEfficiency, !flag, employees, processData, m_ResourcePrefabs, ref m_ResourceDatas, ref m_Citizens, ref m_EconomyParameters);
						if (processData.m_Input1.m_Resource != Resource.NoResource)
						{
							ref NativeArray<int> reference = ref val9;
							int num = EconomyUtils.GetResourceIndex(processData.m_Input1.m_Resource);
							reference[num] += GetRoundedConsumption(processData.m_Input1.m_Amount, processData.m_Output.m_Amount, companyProductionPerDay);
						}
						if (processData.m_Input2.m_Resource != Resource.NoResource)
						{
							ref NativeArray<int> reference = ref val9;
							int num = EconomyUtils.GetResourceIndex(processData.m_Input2.m_Resource);
							reference[num] += GetRoundedConsumption(processData.m_Input2.m_Amount, processData.m_Output.m_Amount, companyProductionPerDay);
						}
						if (flag)
						{
							ref NativeArray<int> reference = ref val4;
							int num = resourceIndex;
							reference[num] += maxWorkers;
							reference = ref val3;
							num = resourceIndex;
							reference[num] += employees.Length;
							reference = ref val6;
							num = resourceIndex;
							reference[num] += companyProductionPerDay;
						}
						else
						{
							ref NativeArray<int> reference = ref val2;
							int num = resourceIndex;
							reference[num] += maxWorkers;
							reference = ref val;
							num = resourceIndex;
							reference[num] += employees.Length;
							reference = ref val5;
							num = resourceIndex;
							reference[num] += companyProductionPerDay;
						}
					}
				}
				else if (resource != Resource.NoResource)
				{
					if (flag)
					{
						int num = EconomyUtils.GetResourceIndex(resource);
						int num2 = val13[num];
						val13[num] = num2 + 1;
					}
					else
					{
						int num2 = EconomyUtils.GetResourceIndex(resource);
						int num = val12[num2];
						val12[num2] = num + 1;
					}
				}
				if (resource != Resource.NoResource)
				{
					if (flag)
					{
						int num = EconomyUtils.GetResourceIndex(resource);
						int num2 = val11[num];
						val11[num] = num2 + 1;
					}
					else
					{
						int num2 = EconomyUtils.GetResourceIndex(resource);
						int num = val10[num2];
						val10[num2] = num + 1;
					}
				}
			}
			for (int j = 0; j < val5.Length; j++)
			{
				CompanyDataItem companyDataItem = new CompanyDataItem
				{
					m_Resource = j,
					m_Demand = val9[j],
					m_Production = val5[j],
					m_CurrentAvailables = val7[j],
					m_ProductionCompanies = val10[j],
					m_ProductionPropertyless = val12[j],
					m_SalesCapacities = val6[j],
					m_ServiceCompanies = val11[j],
					m_ServicePropertyless = val13[j],
					m_TotalAvailables = val8[j],
					m_CurrentProductionWorkers = val[j],
					m_CurrentServiceWorkers = val3[j],
					m_MaxProductionWorkers = val2[j],
					m_MaxServiceWorkers = val4[j]
				};
				m_DataQueue.Enqueue(companyDataItem);
			}
			val.Dispose();
			val2.Dispose();
			val3.Dispose();
			val4.Dispose();
			val5.Dispose();
			val6.Dispose();
			val7.Dispose();
			val8.Dispose();
			val9.Dispose();
			val10.Dispose();
			val11.Dispose();
			val12.Dispose();
			val13.Dispose();
		}

		private int GetRoundedConsumption(int inputAmount, int outputAmount, int production)
		{
			return (int)(((long)inputAmount * (long)production + (outputAmount >> 1)) / outputAmount);
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ServiceAvailable> __Game_Companies_ServiceAvailable_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> __Game_Companies_ServiceCompanyData_RO_ComponentLookup;

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
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Companies_WorkProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkProvider>(true);
			__Game_Companies_ServiceAvailable_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceAvailable>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Companies_ServiceCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceCompanyData>(true);
		}
	}

	private ResourceSystem m_ResourceSystem;

	private NativeQueue<CompanyDataItem> m_DataQueue;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_DemandParameterQuery;

	private EntityQuery m_FreeIndustrialQuery;

	private EntityQuery m_IndustrialCompanyQuery;

	private EntityQuery m_StorageCompanyQuery;

	private EntityQuery m_ProcessDataQuery;

	private EntityQuery m_CityServiceQuery;

	private EntityQuery m_SpawnableQuery;

	[DebugWatchDeps]
	private JobHandle m_WriteDependencies;

	private JobHandle m_ReadDependencies;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_CurrentProductionWorkers;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_MaxProductionWorkers;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_CurrentServiceWorkers;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_MaxServiceWorkers;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_Production;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_SalesCapacities;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_CurrentAvailables;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_TotalAvailables;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_Demand;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_ProductionCompanies;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_ServiceCompanies;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_ProductionPropertyless;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_ServicePropertyless;

	private EntityQuery m_CompanyQuery;

	private TypeHandle __TypeHandle;

	public CommercialCompanyDatas GetCommercialCompanyDatas(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return new CommercialCompanyDatas
		{
			m_CurrentAvailables = m_CurrentAvailables,
			m_ProduceCapacity = m_SalesCapacities,
			m_ServiceCompanies = m_ServiceCompanies,
			m_ServicePropertyless = m_ServicePropertyless,
			m_TotalAvailables = m_TotalAvailables,
			m_CurrentServiceWorkers = m_CurrentServiceWorkers,
			m_MaxServiceWorkers = m_MaxServiceWorkers
		};
	}

	public IndustrialCompanyDatas GetIndustrialCompanyDatas(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return new IndustrialCompanyDatas
		{
			m_Demand = m_Demand,
			m_Production = m_Production,
			m_ProductionCompanies = m_ProductionCompanies,
			m_ProductionPropertyless = m_ProductionPropertyless,
			m_CurrentProductionWorkers = m_CurrentProductionWorkers,
			m_MaxProductionWorkers = m_MaxProductionWorkers
		};
	}

	public NativeArray<int> GetProduction(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_Production;
	}

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 1;
	}

	public void AddReader(JobHandle reader)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, reader);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CollectionUtils.Fill<int>(m_Production, 0);
	}

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
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		m_CompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.Exclude<Game.Companies.StorageCompany>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ProcessDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.Exclude<ServiceCompanyData>()
		});
		int resourceCount = EconomyUtils.ResourceCount;
		m_CurrentProductionWorkers = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_MaxProductionWorkers = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_CurrentServiceWorkers = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_MaxServiceWorkers = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_Production = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_SalesCapacities = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_CurrentAvailables = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_TotalAvailables = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_Demand = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_ProductionCompanies = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_ServiceCompanies = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_ProductionPropertyless = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_ServicePropertyless = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_DataQueue = new NativeQueue<CompanyDataItem>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).RequireForUpdate(m_CompanyQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_DemandParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_ProcessDataQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_CurrentProductionWorkers.Dispose();
		m_MaxProductionWorkers.Dispose();
		m_CurrentServiceWorkers.Dispose();
		m_MaxServiceWorkers.Dispose();
		m_Production.Dispose();
		m_SalesCapacities.Dispose();
		m_CurrentAvailables.Dispose();
		m_TotalAvailables.Dispose();
		m_Demand.Dispose();
		m_ProductionCompanies.Dispose();
		m_ServiceCompanies.Dispose();
		m_ProductionPropertyless.Dispose();
		m_ServicePropertyless.Dispose();
		m_DataQueue.Dispose();
		base.OnDestroy();
	}

	public void SetDefaults(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		CollectionUtils.Fill<int>(m_CurrentProductionWorkers, 0);
		CollectionUtils.Fill<int>(m_MaxProductionWorkers, 0);
		CollectionUtils.Fill<int>(m_CurrentServiceWorkers, 0);
		CollectionUtils.Fill<int>(m_MaxServiceWorkers, 0);
		CollectionUtils.Fill<int>(m_Production, 0);
		CollectionUtils.Fill<int>(m_SalesCapacities, 0);
		CollectionUtils.Fill<int>(m_CurrentAvailables, 0);
		CollectionUtils.Fill<int>(m_TotalAvailables, 0);
		CollectionUtils.Fill<int>(m_Demand, 0);
		CollectionUtils.Fill<int>(m_ProductionCompanies, 0);
		CollectionUtils.Fill<int>(m_ServiceCompanies, 0);
		CollectionUtils.Fill<int>(m_ProductionPropertyless, 0);
		CollectionUtils.Fill<int>(m_ServicePropertyless, 0);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<int> val = m_CurrentProductionWorkers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		NativeArray<int> val2 = m_MaxProductionWorkers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
		NativeArray<int> val3 = m_CurrentServiceWorkers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val3);
		NativeArray<int> val4 = m_MaxServiceWorkers;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
		NativeArray<int> val5 = m_Production;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val5);
		NativeArray<int> val6 = m_SalesCapacities;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val6);
		NativeArray<int> val7 = m_CurrentAvailables;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val7);
		NativeArray<int> val8 = m_TotalAvailables;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val8);
		NativeArray<int> val9 = m_Demand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val9);
		NativeArray<int> val10 = m_ProductionCompanies;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val10);
		NativeArray<int> val11 = m_ServiceCompanies;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val11);
		NativeArray<int> val12 = m_ProductionPropertyless;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val12);
		NativeArray<int> val13 = m_ServicePropertyless;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val13);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		int num = EconomyUtils.ResourceCount;
		Context context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (!((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			num--;
			m_CurrentProductionWorkers[num] = 0;
			m_MaxProductionWorkers[num] = 0;
			m_CurrentServiceWorkers[num] = 0;
			m_MaxServiceWorkers[num] = 0;
			m_Production[num] = 0;
			m_SalesCapacities[num] = 0;
			m_CurrentAvailables[num] = 0;
			m_TotalAvailables[num] = 0;
			m_Demand[num] = 0;
			m_ProductionCompanies[num] = 0;
			m_ServiceCompanies[num] = 0;
			m_ProductionPropertyless[num] = 0;
			m_ServicePropertyless[num] = 0;
		}
		NativeArray<int> val = default(NativeArray<int>);
		val._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
		NativeArray<int> val2 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
		CollectionUtils.CopySafe<int>(val, m_CurrentProductionWorkers);
		NativeArray<int> val3 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val3);
		CollectionUtils.CopySafe<int>(val, m_MaxProductionWorkers);
		NativeArray<int> val4 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val4);
		CollectionUtils.CopySafe<int>(val, m_CurrentServiceWorkers);
		NativeArray<int> val5 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val5);
		CollectionUtils.CopySafe<int>(val, m_MaxServiceWorkers);
		NativeArray<int> val6 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val6);
		CollectionUtils.CopySafe<int>(val, m_Production);
		NativeArray<int> val7 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val7);
		CollectionUtils.CopySafe<int>(val, m_SalesCapacities);
		NativeArray<int> val8 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val8);
		CollectionUtils.CopySafe<int>(val, m_CurrentAvailables);
		NativeArray<int> val9 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val9);
		CollectionUtils.CopySafe<int>(val, m_TotalAvailables);
		NativeArray<int> val10 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val10);
		CollectionUtils.CopySafe<int>(val, m_Demand);
		NativeArray<int> val11 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val11);
		CollectionUtils.CopySafe<int>(val, m_ProductionCompanies);
		NativeArray<int> val12 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val12);
		CollectionUtils.CopySafe<int>(val, m_ServiceCompanies);
		NativeArray<int> val13 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val13);
		CollectionUtils.CopySafe<int>(val, m_ProductionPropertyless);
		NativeArray<int> val14 = val;
		((IReader)reader/*cast due to .constrained prefix*/).Read(val14);
		CollectionUtils.CopySafe<int>(val, m_ServicePropertyless);
		val.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = JobChunkExtensions.ScheduleParallel<CountCompanyDataJob>(new CountCompanyDataJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkProviderType = InternalCompilerInterface.GetComponentTypeHandle<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceAvailableType = InternalCompilerInterface.GetComponentTypeHandle<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingEfficiencyBuf = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCompanyDatas = InternalCompilerInterface.GetComponentLookup<ServiceCompanyData>(ref __TypeHandle.__Game_Companies_ServiceCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_DataQueue = m_DataQueue.AsParallelWriter()
		}, m_CompanyQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, m_WriteDependencies, m_ReadDependencies));
		SumJob sumJob = new SumJob
		{
			m_Demand = m_Demand,
			m_Production = m_Production,
			m_CurrentAvailables = m_CurrentAvailables,
			m_ProductionCompanies = m_ProductionCompanies,
			m_ProductionPropertyless = m_ProductionPropertyless,
			m_SalesCapacities = m_SalesCapacities,
			m_ServiceCompanies = m_ServiceCompanies,
			m_ServicePropertyless = m_ServicePropertyless,
			m_TotalAvailables = m_TotalAvailables,
			m_CurrentProductionWorkers = m_CurrentProductionWorkers,
			m_CurrentServiceWorkers = m_CurrentServiceWorkers,
			m_MaxProductionWorkers = m_MaxProductionWorkers,
			m_MaxServiceWorkers = m_MaxServiceWorkers,
			m_DataQueue = m_DataQueue
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<SumJob>(sumJob, val);
		m_WriteDependencies = ((SystemBase)this).Dependency;
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
	public CountCompanyDataSystem()
	{
	}
}
