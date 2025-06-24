using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Agents;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Notifications;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ServiceCompanySystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateServiceJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		public ComponentTypeHandle<ServiceAvailable> m_ServiceAvailableType;

		[ReadOnly]
		public ComponentTypeHandle<LodgingProvider> m_LodgingProviderType;

		public ComponentTypeHandle<CompanyNotifications> m_CompanyNotificationsType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> m_ServiceCompanyDatas;

		[ReadOnly]
		public BufferLookup<Efficiency> m_BuildingEfficiencies;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public BufferLookup<Resources> m_Resources;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_Districts;

		[ReadOnly]
		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<TaxPayer> m_TaxPayers;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Building> m_Buildings;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameters;

		[ReadOnly]
		public CompanyNotificationParameterData m_CompanyNotificationParameters;

		[ReadOnly]
		public BuildingConfigurationData m_BuildingConfigurationData;

		public IconCommandBuffer m_IconCommandBuffer;

		public RandomSeed m_RandomSeed;

		public uint m_UpdateFrameIndex;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PropertyRenter> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			NativeArray<ServiceAvailable> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceAvailable>(ref m_ServiceAvailableType);
			NativeArray<LodgingProvider> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LodgingProvider>(ref m_LodgingProviderType);
			NativeArray<CompanyNotifications> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CompanyNotifications>(ref m_CompanyNotificationsType);
			BufferAccessor<Employee> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			BufferAccessor<Renter> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity property = nativeArray2[i].m_Property;
				if (!m_Buildings.HasComponent(property))
				{
					continue;
				}
				Entity prefab = m_Prefabs[val].m_Prefab;
				if (!m_ServiceCompanyDatas.HasComponent(prefab))
				{
					continue;
				}
				ServiceCompanyData serviceCompanyData = m_ServiceCompanyDatas[prefab];
				ServiceAvailable serviceAvailable = nativeArray3[i];
				CompanyNotifications companyNotifications = nativeArray5[i];
				Resource resource = m_IndustrialProcessDatas[prefab].m_Output.m_Resource;
				DynamicBuffer<Employee> employees = bufferAccessor[i];
				float buildingEfficiency = 1f;
				if (m_BuildingEfficiencies.TryGetBuffer(property, ref buffer))
				{
					buildingEfficiency = BuildingUtils.GetEfficiency(buffer);
				}
				int num = MathUtils.RoundToIntRandom(ref random, 1f * (float)EconomyUtils.GetCompanyProductionPerDay(buildingEfficiency, isIndustrial: false, employees, m_IndustrialProcessDatas[prefab], m_ResourcePrefabs, ref m_ResourceDatas, ref m_Citizens, ref m_EconomyParameters) / (float)EconomyUtils.kCompanyUpdatesPerDay);
				serviceAvailable.m_ServiceAvailable = math.min(serviceCompanyData.m_MaxService, serviceAvailable.m_ServiceAvailable + num);
				nativeArray3[i] = serviceAvailable;
				if (m_TaxPayers.HasComponent(val))
				{
					int num2;
					if (m_Districts.HasComponent(property))
					{
						Entity district = m_Districts[property].m_District;
						num2 = TaxSystem.GetModifiedCommercialTaxRate(resource, m_TaxRates, district, m_DistrictModifiers);
					}
					else
					{
						num2 = TaxSystem.GetCommercialTaxRate(resource, m_TaxRates);
					}
					TaxPayer taxPayer = m_TaxPayers[val];
					if ((float)num > 0f)
					{
						int num3 = (int)math.ceil(math.max(0f, (float)num * EconomyUtils.GetServicePrice(resource, m_ResourcePrefabs, ref m_ResourceDatas)));
						taxPayer.m_UntaxedIncome += num3;
						if (num3 > 0)
						{
							taxPayer.m_AverageTaxRate = Mathf.RoundToInt(math.lerp((float)taxPayer.m_AverageTaxRate, (float)num2, (float)num3 / (float)(num3 + taxPayer.m_UntaxedIncome)));
						}
						m_TaxPayers[val] = taxPayer;
					}
				}
				bool flag = (float)serviceAvailable.m_ServiceAvailable / math.max(1f, (float)serviceCompanyData.m_MaxService) > m_CompanyNotificationParameters.m_NoCustomersServiceLimit && (resource == Resource.NoResource || EconomyUtils.GetResources(resource, m_Resources[val]) > 200);
				if (flag && nativeArray4.Length > 0 && bufferAccessor2.Length > 0 && nativeArray4[i].m_FreeRooms > 0)
				{
					flag = 1f * (float)nativeArray4[i].m_FreeRooms / (float)(nativeArray4[i].m_FreeRooms + bufferAccessor2[i].Length) > m_CompanyNotificationParameters.m_NoCustomersHotelLimit;
				}
				if (companyNotifications.m_NoCustomersEntity == default(Entity))
				{
					if (flag)
					{
						if ((m_Buildings[property].m_Flags & Game.Buildings.BuildingFlags.HighRentWarning) != Game.Buildings.BuildingFlags.None)
						{
							Building building = m_Buildings[property];
							m_IconCommandBuffer.Remove(property, m_BuildingConfigurationData.m_HighRentNotification);
							building.m_Flags &= ~Game.Buildings.BuildingFlags.HighRentWarning;
							m_Buildings[property] = building;
						}
						m_IconCommandBuffer.Add(property, m_CompanyNotificationParameters.m_NoCustomersNotificationPrefab, IconPriority.Problem);
						companyNotifications.m_NoCustomersEntity = property;
						nativeArray5[i] = companyNotifications;
					}
				}
				else if (!flag)
				{
					m_IconCommandBuffer.Remove(companyNotifications.m_NoCustomersEntity, m_CompanyNotificationParameters.m_NoCustomersNotificationPrefab);
					companyNotifications.m_NoCustomersEntity = Entity.Null;
					nativeArray5[i] = companyNotifications;
				}
				else if (property != companyNotifications.m_NoCustomersEntity)
				{
					m_IconCommandBuffer.Remove(companyNotifications.m_NoCustomersEntity, m_CompanyNotificationParameters.m_NoCustomersNotificationPrefab);
					m_IconCommandBuffer.Add(property, m_CompanyNotificationParameters.m_NoCustomersNotificationPrefab, IconPriority.Problem);
					companyNotifications.m_NoCustomersEntity = property;
					nativeArray5[i] = companyNotifications;
				}
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
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LodgingProvider> __Game_Companies_LodgingProvider_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<ServiceAvailable> __Game_Companies_ServiceAvailable_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CompanyNotifications> __Game_Companies_CompanyNotifications_RW_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> __Game_Companies_ServiceCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		public ComponentLookup<Building> __Game_Buildings_Building_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Resources> __Game_Economy_Resources_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<DistrictModifier> __Game_Areas_DistrictModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		public ComponentLookup<TaxPayer> __Game_Agents_TaxPayer_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Companies_LodgingProvider_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LodgingProvider>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Companies_ServiceAvailable_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceAvailable>(false);
			__Game_Companies_CompanyNotifications_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyNotifications>(false);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Companies_ServiceCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceCompanyData>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Buildings_Building_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(false);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Economy_Resources_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Resources>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Areas_DistrictModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DistrictModifier>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Agents_TaxPayer_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxPayer>(false);
		}
	}

	private EntityQuery m_CompanyGroup;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_CompanyNotificationParameterQuery;

	private EntityQuery m_BuildingParameterQuery;

	private SimulationSystem m_SimulationSystem;

	private ResourceSystem m_ResourceSystem;

	private TaxSystem m_TaxSystem;

	private IconCommandSystem m_IconCommandSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (EconomyUtils.kCompanyUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_CompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.ReadWrite<ServiceAvailable>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<Employee>(),
			ComponentType.ReadOnly<UpdateFrame>()
		});
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_CompanyNotificationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CompanyNotificationParameterData>() });
		m_BuildingParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BuildingConfigurationData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_CompanyGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_CompanyNotificationParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingParameterQuery);
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
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, EconomyUtils.kCompanyUpdatesPerDay, 16);
		UpdateServiceJob updateServiceJob = new UpdateServiceJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LodgingProviderType = InternalCompilerInterface.GetComponentTypeHandle<LodgingProvider>(ref __TypeHandle.__Game_Companies_LodgingProvider_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceAvailableType = InternalCompilerInterface.GetComponentTypeHandle<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyNotificationsType = InternalCompilerInterface.GetComponentTypeHandle<CompanyNotifications>(ref __TypeHandle.__Game_Companies_CompanyNotifications_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCompanyDatas = InternalCompilerInterface.GetComponentLookup<ServiceCompanyData>(ref __TypeHandle.__Game_Companies_ServiceCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingEfficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Resources = InternalCompilerInterface.GetBufferLookup<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictModifiers = InternalCompilerInterface.GetBufferLookup<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Districts = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxPayers = InternalCompilerInterface.GetComponentLookup<TaxPayer>(ref __TypeHandle.__Game_Agents_TaxPayer_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxRates = m_TaxSystem.GetTaxRates(),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_CompanyNotificationParameters = ((EntityQuery)(ref m_CompanyNotificationParameterQuery)).GetSingleton<CompanyNotificationParameterData>(),
			m_BuildingConfigurationData = ((EntityQuery)(ref m_BuildingParameterQuery)).GetSingleton<BuildingConfigurationData>(),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer(),
			m_RandomSeed = RandomSeed.Next(),
			m_UpdateFrameIndex = updateFrame
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateServiceJob>(updateServiceJob, m_CompanyGroup, ((SystemBase)this).Dependency);
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
		m_TaxSystem.AddReader(((SystemBase)this).Dependency);
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
	public ServiceCompanySystem()
	{
	}
}
