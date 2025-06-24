using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Debug;
using Game.Economy;
using Game.Prefabs;
using Game.Prefabs.Modes;
using Game.Reflection;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CommercialDemandSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct UpdateCommercialDemandJob : IJob
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ZoneData> m_UnlockedZoneDatas;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_CommercialPropertyChunks;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyOnMarket> m_PropertyOnMarketType;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<CommercialCompany> m_CommercialCompanies;

		[ReadOnly]
		public ComponentLookup<Tourism> m_Tourisms;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public DemandParameterData m_DemandParameters;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		public NativeValue<int> m_CompanyDemand;

		public NativeValue<int> m_BuildingDemand;

		public NativeArray<int> m_DemandFactors;

		public NativeArray<int> m_FreeProperties;

		public NativeArray<int> m_ResourceDemands;

		public NativeArray<int> m_BuildingDemands;

		[ReadOnly]
		public NativeArray<int> m_ProduceCapacity;

		[ReadOnly]
		public NativeArray<int> m_CurrentAvailables;

		[ReadOnly]
		public NativeArray<int> m_Propertyless;

		public float m_CommercialTaxEffectDemandOffset;

		public void Execute()
		{
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			for (int i = 0; i < m_UnlockedZoneDatas.Length; i++)
			{
				if (m_UnlockedZoneDatas[i].m_AreaType == AreaType.Commercial)
				{
					flag = true;
					break;
				}
			}
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int resourceIndex = EconomyUtils.GetResourceIndex(iterator.resource);
				m_FreeProperties[resourceIndex] = 0;
				m_BuildingDemands[resourceIndex] = 0;
				m_ResourceDemands[resourceIndex] = 0;
			}
			for (int j = 0; j < m_DemandFactors.Length; j++)
			{
				m_DemandFactors[j] = 0;
			}
			for (int k = 0; k < m_CommercialPropertyChunks.Length; k++)
			{
				ArchetypeChunk val = m_CommercialPropertyChunks[k];
				if (!((ArchetypeChunk)(ref val)).Has<PropertyOnMarket>(ref m_PropertyOnMarketType))
				{
					continue;
				}
				NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabType);
				BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Renter>(ref m_RenterType);
				for (int l = 0; l < nativeArray.Length; l++)
				{
					Entity prefab = nativeArray[l].m_Prefab;
					if (!m_BuildingPropertyDatas.HasComponent(prefab))
					{
						continue;
					}
					bool flag2 = false;
					DynamicBuffer<Renter> val2 = bufferAccessor[l];
					for (int m = 0; m < val2.Length; m++)
					{
						if (m_CommercialCompanies.HasComponent(val2[m].m_Renter))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						continue;
					}
					BuildingPropertyData buildingPropertyData = m_BuildingPropertyDatas[prefab];
					ResourceIterator iterator2 = ResourceIterator.GetIterator();
					while (iterator2.Next())
					{
						if ((buildingPropertyData.m_AllowedSold & iterator2.resource) != Resource.NoResource)
						{
							ref NativeArray<int> reference = ref m_FreeProperties;
							int resourceIndex2 = EconomyUtils.GetResourceIndex(iterator2.resource);
							int num = reference[resourceIndex2];
							reference[resourceIndex2] = num + 1;
						}
					}
				}
			}
			m_CompanyDemand.value = 0;
			m_BuildingDemand.value = 0;
			int population = m_Populations[m_City].m_Population;
			iterator = ResourceIterator.GetIterator();
			int num2 = 0;
			while (iterator.Next())
			{
				int resourceIndex3 = EconomyUtils.GetResourceIndex(iterator.resource);
				if (!EconomyUtils.IsCommercialResource(iterator.resource) || !m_ResourceDatas.HasComponent(m_ResourcePrefabs[iterator.resource]))
				{
					continue;
				}
				float num3 = -0.05f * ((float)TaxSystem.GetCommercialTaxRate(iterator.resource, m_TaxRates) - 10f) * m_DemandParameters.m_TaxEffect.y;
				num3 += m_CommercialTaxEffectDemandOffset;
				if (iterator.resource != Resource.Lodging)
				{
					int num4 = ((population <= 1000) ? 2500 : (2500 * (int)Mathf.Log10(0.01f * (float)population)));
					m_ResourceDemands[resourceIndex3] = math.clamp(100 - (m_CurrentAvailables[resourceIndex3] - num4) / 25, 0, 100);
				}
				else if (math.max((int)((float)m_Tourisms[m_City].m_CurrentTourists * m_DemandParameters.m_HotelRoomPercentRequirement) - m_Tourisms[m_City].m_Lodging.y, 0) > 0)
				{
					m_ResourceDemands[resourceIndex3] = 100;
				}
				m_ResourceDemands[resourceIndex3] = Mathf.RoundToInt((1f + num3) * (float)m_ResourceDemands[resourceIndex3]);
				int num5 = Mathf.RoundToInt(100f * num3);
				ref NativeArray<int> reference2 = ref m_DemandFactors;
				reference2[11] = reference2[11] + num5;
				if (m_ResourceDemands[resourceIndex3] > 0)
				{
					ref NativeValue<int> reference3 = ref m_CompanyDemand;
					reference3.value += m_ResourceDemands[resourceIndex3];
					m_BuildingDemands[resourceIndex3] = ((m_FreeProperties[resourceIndex3] - m_Propertyless[resourceIndex3] <= 0) ? m_ResourceDemands[resourceIndex3] : 0);
					if (m_BuildingDemands[resourceIndex3] > 0)
					{
						ref NativeValue<int> reference4 = ref m_BuildingDemand;
						reference4.value += m_BuildingDemands[resourceIndex3];
					}
					int num6 = ((m_BuildingDemands[resourceIndex3] > 0) ? m_ResourceDemands[resourceIndex3] : 0);
					int num7 = m_ResourceDemands[resourceIndex3];
					int num8 = num7 + num5;
					if (iterator.resource == Resource.Lodging)
					{
						reference2 = ref m_DemandFactors;
						reference2[9] = reference2[9] + num7;
					}
					else if (iterator.resource == Resource.Petrochemicals)
					{
						reference2 = ref m_DemandFactors;
						reference2[16] = reference2[16] + num7;
					}
					else
					{
						reference2 = ref m_DemandFactors;
						reference2[4] = reference2[4] + num7;
					}
					reference2 = ref m_DemandFactors;
					reference2[13] = reference2[13] + math.min(0, num6 - num8);
					num2++;
				}
			}
			if (m_DemandFactors[4] == 0)
			{
				m_DemandFactors[4] = -10;
			}
			if (population <= 0)
			{
				m_DemandFactors[4] = 0;
			}
			if (m_CommercialPropertyChunks.Length == 0 && m_DemandFactors[13] > 0)
			{
				m_DemandFactors[13] = 0;
			}
			m_CompanyDemand.value = ((num2 != 0) ? math.clamp(m_CompanyDemand.value / num2, 0, 100) : 0);
			m_BuildingDemand.value = ((num2 != 0 && flag) ? math.clamp(m_BuildingDemand.value / num2, 0, 100) : 0);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyOnMarket> __Game_Buildings_PropertyOnMarket_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CommercialCompany> __Game_Companies_CommercialCompany_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tourism> __Game_City_Tourism_RO_ComponentLookup;

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
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Buildings_PropertyOnMarket_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyOnMarket>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Companies_CommercialCompany_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CommercialCompany>(true);
			__Game_City_Tourism_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tourism>(true);
		}
	}

	private ResourceSystem m_ResourceSystem;

	private TaxSystem m_TaxSystem;

	private CountCompanyDataSystem m_CountCompanyDataSystem;

	private CountHouseholdDataSystem m_CountHouseholdDataSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_DemandParameterQuery;

	private EntityQuery m_CommercialQuery;

	private EntityQuery m_CommercialProcessDataQuery;

	private EntityQuery m_UnlockedZoneDataQuery;

	private EntityQuery m_GameModeSettingQuery;

	private NativeValue<int> m_CompanyDemand;

	private NativeValue<int> m_BuildingDemand;

	[EnumArray(typeof(DemandFactor))]
	[DebugWatchValue]
	private NativeArray<int> m_DemandFactors;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_ResourceDemands;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_BuildingDemands;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_Consumption;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_FreeProperties;

	[DebugWatchDeps]
	private JobHandle m_WriteDependencies;

	private JobHandle m_ReadDependencies;

	private int m_LastCompanyDemand;

	private int m_LastBuildingDemand;

	private float m_CommercialTaxEffectDemandOffset;

	private TypeHandle __TypeHandle;

	[DebugWatchValue(color = "#008fff")]
	public int companyDemand => m_LastCompanyDemand;

	[DebugWatchValue(color = "#2b6795")]
	public int buildingDemand => m_LastBuildingDemand;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 4;
	}

	public NativeArray<int> GetDemandFactors(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_DemandFactors;
	}

	public NativeArray<int> GetResourceDemands(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_ResourceDemands;
	}

	public NativeArray<int> GetBuildingDemands(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_BuildingDemands;
	}

	public NativeArray<int> GetConsumption(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_WriteDependencies;
		return m_Consumption;
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		CollectionUtils.Fill<int>(m_Consumption, 0);
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			m_CommercialTaxEffectDemandOffset = 0f;
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		if (singleton.m_Enable)
		{
			m_CommercialTaxEffectDemandOffset = singleton.m_CommercialTaxEffectDemandOffset;
		}
		else
		{
			m_CommercialTaxEffectDemandOffset = 0f;
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_CountCompanyDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountCompanyDataSystem>();
		m_CountHouseholdDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountHouseholdDataSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		m_CommercialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<CommercialProperty>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Abandoned>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Condemned>(),
			ComponentType.Exclude<Temp>()
		});
		m_CommercialProcessDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.ReadOnly<ServiceCompanyData>()
		});
		m_UnlockedZoneDataQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ZoneData>(),
			ComponentType.Exclude<Locked>()
		});
		m_GameModeSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		m_CompanyDemand = new NativeValue<int>((Allocator)4);
		m_BuildingDemand = new NativeValue<int>((Allocator)4);
		m_DemandFactors = new NativeArray<int>(18, (Allocator)4, (NativeArrayOptions)1);
		int resourceCount = EconomyUtils.ResourceCount;
		m_ResourceDemands = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_BuildingDemands = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_Consumption = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_FreeProperties = new NativeArray<int>(resourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_CommercialTaxEffectDemandOffset = 0f;
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_DemandParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_CommercialProcessDataQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_CompanyDemand.Dispose();
		m_BuildingDemand.Dispose();
		m_DemandFactors.Dispose();
		m_ResourceDemands.Dispose();
		m_BuildingDemands.Dispose();
		m_Consumption.Dispose();
		m_FreeProperties.Dispose();
		base.OnDestroy();
	}

	public void SetDefaults(Context context)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		m_CompanyDemand.value = 0;
		m_BuildingDemand.value = 0;
		CollectionUtils.Fill<int>(m_DemandFactors, 0);
		CollectionUtils.Fill<int>(m_ResourceDemands, 0);
		CollectionUtils.Fill<int>(m_BuildingDemands, 0);
		CollectionUtils.Fill<int>(m_Consumption, 0);
		CollectionUtils.Fill<int>(m_FreeProperties, 0);
		m_LastCompanyDemand = 0;
		m_LastBuildingDemand = 0;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		int value = m_CompanyDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value);
		int value2 = m_BuildingDemand.value;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(value2);
		int length = m_DemandFactors.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		NativeArray<int> val = m_DemandFactors;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		NativeArray<int> val2 = m_ResourceDemands;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
		NativeArray<int> val3 = m_BuildingDemands;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val3);
		NativeArray<int> val4 = m_Consumption;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
		NativeArray<int> val5 = m_FreeProperties;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val5);
		int num = m_LastCompanyDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		int num2 = m_LastBuildingDemand;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		int value = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value);
		m_CompanyDemand.value = value;
		int value2 = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref value2);
		m_BuildingDemand.value = value2;
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.demandFactorCountSerialization)
		{
			NativeArray<int> val = default(NativeArray<int>);
			val._002Ector(13, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val2 = val;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
			CollectionUtils.CopySafe<int>(val, m_DemandFactors);
			val.Dispose();
		}
		else
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			if (num == m_DemandFactors.Length)
			{
				NativeArray<int> val3 = m_DemandFactors;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val3);
			}
			else
			{
				NativeArray<int> val4 = default(NativeArray<int>);
				val4._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
				NativeArray<int> val5 = val4;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val5);
				CollectionUtils.CopySafe<int>(val4, m_DemandFactors);
				val4.Dispose();
			}
		}
		context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			NativeArray<int> val6 = m_ResourceDemands;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val6);
			NativeArray<int> val7 = m_BuildingDemands;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val7);
		}
		else
		{
			NativeArray<int> subArray = m_ResourceDemands.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray);
			NativeArray<int> subArray2 = m_BuildingDemands.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray2);
			m_ResourceDemands[40] = 0;
			m_BuildingDemands[40] = 0;
		}
		NativeArray<int> val8 = default(NativeArray<int>);
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.companyDemandOptimization)
		{
			val8._002Ector(EconomyUtils.ResourceCount, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val9 = val8;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val9);
		}
		context = ((IReader)reader).context;
		format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			NativeArray<int> val10 = m_Consumption;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val10);
		}
		else
		{
			NativeArray<int> subArray3 = m_Consumption.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray3);
			m_Consumption[40] = 0;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.companyDemandOptimization)
		{
			NativeArray<int> val11 = val8;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val11);
			NativeArray<int> val12 = val8;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val12);
			NativeArray<int> val13 = val8;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val13);
		}
		context = ((IReader)reader).context;
		format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			NativeArray<int> val14 = m_FreeProperties;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val14);
		}
		else
		{
			NativeArray<int> subArray4 = m_FreeProperties.GetSubArray(0, 40);
			((IReader)reader/*cast due to .constrained prefix*/).Read(subArray4);
			m_FreeProperties[40] = 0;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version < Version.companyDemandOptimization)
		{
			NativeArray<int> val15 = val8;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val15);
			val8.Dispose();
		}
		ref int reference = ref m_LastCompanyDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		ref int reference2 = ref m_LastBuildingDemand;
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_DemandParameterQuery)).IsEmptyIgnoreFilter && !((EntityQuery)(ref m_EconomyParameterQuery)).IsEmptyIgnoreFilter)
		{
			m_LastCompanyDemand = m_CompanyDemand.value;
			m_LastBuildingDemand = m_BuildingDemand.value;
			JobHandle deps;
			CountCompanyDataSystem.CommercialCompanyDatas commercialCompanyDatas = m_CountCompanyDataSystem.GetCommercialCompanyDatas(out deps);
			JobHandle val = default(JobHandle);
			UpdateCommercialDemandJob updateCommercialDemandJob = new UpdateCommercialDemandJob
			{
				m_CommercialPropertyChunks = ((EntityQuery)(ref m_CommercialQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_UnlockedZoneDatas = ((EntityQuery)(ref m_UnlockedZoneDataQuery)).ToComponentDataArray<ZoneData>(AllocatorHandle.op_Implicit((Allocator)3)),
				m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyOnMarketType = InternalCompilerInterface.GetComponentTypeHandle<PropertyOnMarket>(ref __TypeHandle.__Game_Buildings_PropertyOnMarket_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommercialCompanies = InternalCompilerInterface.GetComponentLookup<CommercialCompany>(ref __TypeHandle.__Game_Companies_CommercialCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_DemandParameters = ((EntityQuery)(ref m_DemandParameterQuery)).GetSingleton<DemandParameterData>(),
				m_TaxRates = m_TaxSystem.GetTaxRates(),
				m_CompanyDemand = m_CompanyDemand,
				m_BuildingDemand = m_BuildingDemand,
				m_DemandFactors = m_DemandFactors,
				m_City = m_CitySystem.City,
				m_ResourceDemands = m_ResourceDemands,
				m_BuildingDemands = m_BuildingDemands,
				m_ProduceCapacity = commercialCompanyDatas.m_ProduceCapacity,
				m_CurrentAvailables = commercialCompanyDatas.m_CurrentAvailables,
				m_FreeProperties = m_FreeProperties,
				m_Propertyless = commercialCompanyDatas.m_ServicePropertyless,
				m_Tourisms = InternalCompilerInterface.GetComponentLookup<Tourism>(ref __TypeHandle.__Game_City_Tourism_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommercialTaxEffectDemandOffset = m_CommercialTaxEffectDemandOffset
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<UpdateCommercialDemandJob>(updateCommercialDemandJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, m_ReadDependencies, val, deps));
			m_WriteDependencies = ((SystemBase)this).Dependency;
			m_CountHouseholdDataSystem.AddHouseholdDataReader(((SystemBase)this).Dependency);
			m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			m_TaxSystem.AddReader(((SystemBase)this).Dependency);
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
	public CommercialDemandSystem()
	{
	}
}
