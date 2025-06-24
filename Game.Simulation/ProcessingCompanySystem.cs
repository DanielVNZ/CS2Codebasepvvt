using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Serialization;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ProcessingCompanySystem : GameSystemBase, IDefaultSerializable, ISerializable, IPostDeserialize
{
	[BurstCompile]
	private struct UpdateProcessingJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceAvailable> m_ServiceAvailableType;

		public BufferTypeHandle<Resources> m_ResourceType;

		public ComponentTypeHandle<CompanyData> m_CompanyDataType;

		public ComponentTypeHandle<TaxPayer> m_TaxPayerType;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_Limits;

		[ReadOnly]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public BufferLookup<SpecializationBonus> m_Specializations;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Efficiency> m_BuildingEfficiencies;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		public NativeArray<long> m_ProducedResources;

		public ParallelWriter<ProductionSpecializationSystem.ProducedResource> m_ProductionQueue;

		public ParallelWriter m_CommandBuffer;

		public EconomyParameterData m_EconomyParameters;

		public RandomSeed m_RandomSeed;

		public Entity m_City;

		public uint m_UpdateFrameIndex;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			DynamicBuffer<SpecializationBonus> specializations = m_Specializations[m_City];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<PropertyRenter> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			BufferAccessor<Employee> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			NativeArray<CompanyData> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CompanyData>(ref m_CompanyDataType);
			NativeArray<TaxPayer> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TaxPayer>(ref m_TaxPayerType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<ServiceAvailable>(ref m_ServiceAvailableType);
			DynamicBuffer<Efficiency> val2 = default(DynamicBuffer<Efficiency>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				Entity property = nativeArray3[i].m_Property;
				ref CompanyData reference = ref CollectionUtils.ElementAt<CompanyData>(nativeArray4, i);
				if (!m_Buildings.HasComponent(property))
				{
					continue;
				}
				DynamicBuffer<Resources> resources = bufferAccessor[i];
				IndustrialProcessData industrialProcessData = m_IndustrialProcessDatas[prefab];
				StorageLimitData storageLimitData = m_Limits[prefab];
				float buildingEfficiency = 1f;
				if (m_BuildingEfficiencies.TryGetBuffer(property, ref val2))
				{
					UpdateEfficiencyFactors(industrialProcessData, flag, val2, cityModifiers, specializations);
					buildingEfficiency = BuildingUtils.GetEfficiency(val2);
				}
				int companyProductionPerDay = EconomyUtils.GetCompanyProductionPerDay(buildingEfficiency, !flag, bufferAccessor2[i], industrialProcessData, m_ResourcePrefabs, ref m_ResourceDatas, ref m_Citizens, ref m_EconomyParameters);
				int num = MathUtils.RoundToIntRandom(ref random, 1f * (float)companyProductionPerDay / (float)EconomyUtils.kCompanyUpdatesPerDay);
				ResourceStack input = industrialProcessData.m_Input1;
				ResourceStack input2 = industrialProcessData.m_Input2;
				ResourceStack output = industrialProcessData.m_Output;
				if (input.m_Resource == output.m_Resource && input2.m_Resource == Resource.NoResource && input.m_Amount == output.m_Amount)
				{
					continue;
				}
				float num2 = 1f;
				float num3 = 1f;
				int num4 = 0;
				int num5 = 0;
				if (input.m_Resource != Resource.NoResource && (float)input.m_Amount > 0f)
				{
					int resources2 = EconomyUtils.GetResources(input.m_Resource, resources);
					num2 = (float)input.m_Amount * 1f / (float)output.m_Amount;
					num = math.min(num, (int)((float)resources2 / num2));
				}
				if (input2.m_Resource != Resource.NoResource && (float)input2.m_Amount > 0f)
				{
					int resources3 = EconomyUtils.GetResources(input2.m_Resource, resources);
					num3 = (float)input2.m_Amount * 1f / (float)output.m_Amount;
					num = math.min(num, (int)((float)resources3 / num3));
				}
				int resources4;
				if ((float)num > 0f)
				{
					int num6 = 0;
					if (flag && EconomyUtils.GetResources(output.m_Resource, resources) > 5000)
					{
						continue;
					}
					if (input.m_Resource != Resource.NoResource)
					{
						num4 = -MathUtils.RoundToIntRandom(ref reference.m_RandomSeed, (float)num * num2);
						int num7 = EconomyUtils.AddResources(input.m_Resource, num4, resources);
						num6 += ((EconomyUtils.GetWeight(input.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas) > 0f) ? num7 : 0);
					}
					if (input2.m_Resource != Resource.NoResource)
					{
						num5 = -MathUtils.RoundToIntRandom(ref reference.m_RandomSeed, (float)num * num3);
						int num8 = EconomyUtils.AddResources(input2.m_Resource, num5, resources);
						num6 += ((EconomyUtils.GetWeight(input2.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas) > 0f) ? num8 : 0);
					}
					int num9 = storageLimitData.m_Limit - num6;
					if (EconomyUtils.GetWeight(output.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas) > 0f)
					{
						num = math.min(num9, num);
					}
					else
					{
						resources4 = EconomyUtils.GetResources(output.m_Resource, resources);
						num = math.clamp(IndustrialAISystem.kMaxVirtualResourceStorage - resources4, 0, num);
					}
					resources4 = EconomyUtils.AddResources(output.m_Resource, num, resources);
					AddProducedResource(output.m_Resource, num);
				}
				else
				{
					resources4 = EconomyUtils.GetResources(output.m_Resource, resources);
				}
				int num10 = EconomyUtils.GetCompanyProfitPerDay(buildingEfficiency, !flag, bufferAccessor2[i], industrialProcessData, m_ResourcePrefabs, ref m_ResourceDatas, ref m_Citizens, ref m_EconomyParameters) / EconomyUtils.kCompanyUpdatesPerDay;
				TaxPayer taxPayer = nativeArray5[i];
				int num11 = (flag ? TaxSystem.GetCommercialTaxRate(output.m_Resource, m_TaxRates) : TaxSystem.GetIndustrialTaxRate(output.m_Resource, m_TaxRates));
				if (input.m_Resource != output.m_Resource && (float)num10 > 0f)
				{
					if (num10 > 0)
					{
						taxPayer.m_AverageTaxRate = Mathf.RoundToInt(math.lerp((float)taxPayer.m_AverageTaxRate, (float)num11, (float)num10 / (float)(num10 + taxPayer.m_UntaxedIncome)));
					}
					taxPayer.m_UntaxedIncome += num10;
					nativeArray5[i] = taxPayer;
				}
				if (!flag && EconomyUtils.IsMaterial(output.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas) && resources4 > 0)
				{
					m_DeliveryTruckSelectData.TrySelectItem(ref random, output.m_Resource, resources4, out var item);
					if ((float)item.m_Cost / (float)math.min(resources4, item.m_Capacity) < 0.03f)
					{
						_ = 100;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<ResourceExporter>(unfilteredChunkIndex, val, new ResourceExporter
						{
							m_Resource = output.m_Resource,
							m_Amount = math.max(0, math.min(item.m_Capacity, resources4))
						});
					}
				}
			}
		}

		private void UpdateEfficiencyFactors(IndustrialProcessData process, bool isCommercial, DynamicBuffer<Efficiency> efficiencies, DynamicBuffer<CityModifier> cityModifiers, DynamicBuffer<SpecializationBonus> specializations)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			if (IsOffice(process))
			{
				float value = 100f;
				if (!isCommercial)
				{
					CityUtils.ApplyModifier(ref value, cityModifiers, CityModifierType.OfficeEfficiency);
				}
				BuildingUtils.SetEfficiencyFactor(efficiencies, EfficiencyFactor.CityModifierOfficeEfficiency, value / 100f);
			}
			else if (!isCommercial)
			{
				float value2 = 100f;
				CityUtils.ApplyModifier(ref value2, cityModifiers, CityModifierType.IndustrialEfficiency);
				BuildingUtils.SetEfficiencyFactor(efficiencies, EfficiencyFactor.CityModifierIndustrialEfficiency, value2 / 100f);
			}
			if (process.m_Input1.m_Resource == Resource.Fish || process.m_Input2.m_Resource == Resource.Fish)
			{
				float value3 = 100f;
				CityUtils.ApplyModifier(ref value3, cityModifiers, CityModifierType.IndustrialFishInputEfficiency);
				BuildingUtils.SetEfficiencyFactor(efficiencies, EfficiencyFactor.CityModifierFishInput, value3 / 100f);
			}
			if (process.m_Output.m_Resource == Resource.Software)
			{
				float value4 = 100f;
				CityUtils.ApplyModifier(ref value4, cityModifiers, CityModifierType.OfficeSoftwareEfficiency);
				BuildingUtils.SetEfficiencyFactor(efficiencies, EfficiencyFactor.CityModifierSoftware, value4 / 100f);
			}
			else if (process.m_Output.m_Resource == Resource.Electronics)
			{
				float value5 = 100f;
				CityUtils.ApplyModifier(ref value5, cityModifiers, CityModifierType.IndustrialElectronicsEfficiency);
				BuildingUtils.SetEfficiencyFactor(efficiencies, EfficiencyFactor.CityModifierElectronics, value5 / 100f);
			}
			int resourceIndex = EconomyUtils.GetResourceIndex(process.m_Output.m_Resource);
			if (specializations.Length > resourceIndex)
			{
				float efficiency = 1f + specializations[resourceIndex].GetBonus(m_EconomyParameters.m_MaxCitySpecializationBonus, m_EconomyParameters.m_ResourceProductionCoefficient);
				BuildingUtils.SetEfficiencyFactor(efficiencies, EfficiencyFactor.SpecializationBonus, efficiency);
			}
		}

		private bool IsOffice(IndustrialProcessData process)
		{
			return !EconomyUtils.IsMaterial(process.m_Output.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas);
		}

		private Resource GetRandomUpkeepResource(CompanyData companyData, Resource outputResource)
		{
			switch (((Random)(ref companyData.m_RandomSeed)).NextInt(4))
			{
			case 0:
				return Resource.Software;
			case 1:
				return Resource.Telecom;
			case 2:
				return Resource.Financial;
			case 3:
				if (EconomyUtils.IsMaterial(outputResource, m_ResourcePrefabs, ref m_ResourceDatas))
				{
					return Resource.Machinery;
				}
				if (!((Random)(ref companyData.m_RandomSeed)).NextBool())
				{
					return Resource.Furniture;
				}
				return Resource.Paper;
			default:
				return Resource.NoResource;
			}
		}

		private unsafe void AddProducedResource(Resource resource, int amount)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			if (resource != Resource.NoResource)
			{
				long* unsafePtr = (long*)NativeArrayUnsafeUtility.GetUnsafePtr<long>(m_ProducedResources);
				unsafePtr += EconomyUtils.GetResourceIndex(resource);
				Interlocked.Add(ref *unsafePtr, amount);
				m_ProductionQueue.Enqueue(new ProductionSpecializationSystem.ProducedResource
				{
					m_Resource = resource,
					m_Amount = amount
				});
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
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ServiceAvailable> __Game_Companies_ServiceAvailable_RO_ComponentTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public ComponentTypeHandle<CompanyData> __Game_Companies_CompanyData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TaxPayer> __Game_Agents_TaxPayer_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SpecializationBonus> __Game_City_SpecializationBonus_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Companies_ServiceAvailable_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceAvailable>(true);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Companies_CompanyData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyData>(false);
			__Game_Agents_TaxPayer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxPayer>(false);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_City_SpecializationBonus_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpecializationBonus>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Buildings_Efficiency_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(false);
		}
	}

	public const int kMaxCommercialOutputResource = 5000;

	public const float kMaximumTransportUnitCost = 0.03f;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private ResourceSystem m_ResourceSystem;

	private TaxSystem m_TaxSystem;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private ProductionSpecializationSystem m_ProductionSpecializationSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_CompanyGroup;

	private NativeArray<long> m_ProducedResources;

	private JobHandle m_ProducedResourcesDeps;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1038562630_0;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (EconomyUtils.kCompanyUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_ProductionSpecializationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProductionSpecializationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<CitySystem>();
		m_CompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[9]
		{
			ComponentType.ReadWrite<Game.Companies.ProcessingCompany>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Employee>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Game.Companies.ExtractorCompany>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CompanyGroup);
		((ComponentSystemBase)this).RequireForUpdate<EconomyParameterData>();
		m_ProducedResources = new NativeArray<long>(EconomyUtils.ResourceCount, (Allocator)4, (NativeArrayOptions)1);
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if (!(((Context)(ref context)).version < Version.officeFix))
		{
			return;
		}
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		ComponentLookup<ResourceData> componentLookup = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<Entity> val = ((EntityQuery)(ref m_CompanyGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < val.Length; i++)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val[i]).m_Prefab;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			IndustrialProcessData componentData = ((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(prefab);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<ServiceAvailable>(val[i]) && componentLookup[prefabs[componentData.m_Output.m_Resource]].m_Weight == 0f)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(val[i], false);
				if (EconomyUtils.GetResources(componentData.m_Output.m_Resource, buffer) >= 500)
				{
					EconomyUtils.AddResources(componentData.m_Output.m_Resource, -500, buffer);
				}
			}
		}
		val.Dispose();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ProducedResources.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, EconomyUtils.kCompanyUpdatesPerDay, 16);
		JobHandle deps;
		UpdateProcessingJob updateProcessingJob = new UpdateProcessingJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceAvailableType = InternalCompilerInterface.GetComponentTypeHandle<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyDataType = InternalCompilerInterface.GetComponentTypeHandle<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxPayerType = InternalCompilerInterface.GetComponentTypeHandle<TaxPayer>(ref __TypeHandle.__Game_Agents_TaxPayer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Limits = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Specializations = InternalCompilerInterface.GetBufferLookup<SpecializationBonus>(ref __TypeHandle.__Game_City_SpecializationBonus_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingEfficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData(),
			m_ProducedResources = m_ProducedResources,
			m_ProductionQueue = m_ProductionSpecializationSystem.GetQueue(out deps).AsParallelWriter()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		updateProcessingJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		updateProcessingJob.m_EconomyParameters = ((EntityQuery)(ref __query_1038562630_0)).GetSingleton<EconomyParameterData>();
		updateProcessingJob.m_RandomSeed = RandomSeed.Next();
		updateProcessingJob.m_City = m_CitySystem.City;
		updateProcessingJob.m_UpdateFrameIndex = updateFrame;
		UpdateProcessingJob updateProcessingJob2 = updateProcessingJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateProcessingJob>(updateProcessingJob2, m_CompanyGroup, JobHandle.CombineDependencies(m_ProducedResourcesDeps, deps, ((SystemBase)this).Dependency));
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
		m_ProductionSpecializationSystem.AddQueueWriter(((SystemBase)this).Dependency);
		m_TaxSystem.AddReader(((SystemBase)this).Dependency);
		m_ProducedResourcesDeps = default(JobHandle);
	}

	public NativeArray<long> GetProducedResourcesArray(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = ((SystemBase)this).Dependency;
		return m_ProducedResources;
	}

	public void AddProducedResourcesReader(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ProducedResourcesDeps = JobHandle.CombineDependencies(m_ProducedResourcesDeps, handle);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		byte num = (byte)m_ProducedResources.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		for (int i = 0; i < m_ProducedResources.Length; i++)
		{
			long num2 = m_ProducedResources[i];
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte b = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
		long num = default(long);
		for (int i = 0; i < b; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			if (i < m_ProducedResources.Length)
			{
				m_ProducedResources[i] = num;
			}
		}
		for (int j = b; j < m_ProducedResources.Length; j++)
		{
			m_ProducedResources[j] = 0L;
		}
	}

	public void SetDefaults(Context context)
	{
		for (int i = 0; i < m_ProducedResources.Length; i++)
		{
			m_ProducedResources[i] = 0L;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<EconomyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1038562630_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ProcessingCompanySystem()
	{
	}
}
