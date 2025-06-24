using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Debug;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TradeSystem : GameSystemBase, ITradeSystem, IDefaultSerializable, ISerializable
{
	[BurstCompile]
	private struct TradeJob : IJob
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		public BufferTypeHandle<Resources> m_ResourceType;

		public BufferTypeHandle<TradeCost> m_TradeCostType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> m_GarbageFacilityDatas;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageDatas;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_Limits;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityEffects;

		public Entity m_City;

		public NativeArray<int> m_TradeBalances;

		public NativeArray<float> m_CachedCosts;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		public OutsideTradeParameterData m_TradeParameters;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		private TradeCost GetCachedTradeCost(Resource resource, OutsideConnectionTransferType type, NativeArray<float> cache)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			for (OutsideConnectionTransferType outsideConnectionTransferType = OutsideConnectionTransferType.Road; outsideConnectionTransferType != OutsideConnectionTransferType.Last; outsideConnectionTransferType = (OutsideConnectionTransferType)((int)outsideConnectionTransferType << 1))
			{
				if ((outsideConnectionTransferType & type) != OutsideConnectionTransferType.None)
				{
					num = math.min(num, cache[GetCacheIndex(resource, type, import: true)]);
					num2 = math.min(num2, cache[GetCacheIndex(resource, type, import: false)]);
				}
			}
			return new TradeCost
			{
				m_Resource = resource,
				m_BuyCost = num,
				m_SellCost = num2
			};
		}

		public void Execute()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int resourceIndex = EconomyUtils.GetResourceIndex(iterator.resource);
				int num = Mathf.RoundToInt((1f - kRefreshRate) * (float)m_TradeBalances[resourceIndex]);
				m_TradeBalances[resourceIndex] = num;
				float weight = m_ResourceDatas[m_ResourcePrefabs[iterator.resource]].m_Weight;
				OutsideConnectionTransferType outsideConnectionTransferType = OutsideConnectionTransferType.Road;
				DynamicBuffer<CityModifier> cityEffects = m_CityEffects[m_City];
				while (outsideConnectionTransferType != OutsideConnectionTransferType.Last)
				{
					TradeCost tradeCost = CalculateTradeCost(iterator.resource, m_TradeBalances[resourceIndex], outsideConnectionTransferType, weight, ref m_TradeParameters, cityEffects);
					Assert.IsTrue(!float.IsNaN(tradeCost.m_SellCost));
					Assert.IsTrue(!float.IsNaN(tradeCost.m_BuyCost));
					m_CachedCosts[GetCacheIndex(iterator.resource, outsideConnectionTransferType, import: false)] = tradeCost.m_SellCost;
					m_CachedCosts[GetCacheIndex(iterator.resource, outsideConnectionTransferType, import: true)] = tradeCost.m_BuyCost;
					outsideConnectionTransferType = (OutsideConnectionTransferType)((int)outsideConnectionTransferType << 1);
				}
			}
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabType);
				BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Resources>(ref m_ResourceType);
				BufferAccessor<TradeCost> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<TradeCost>(ref m_TradeCostType);
				BufferAccessor<InstalledUpgrade> bufferAccessor3 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					DynamicBuffer<Resources> resources = bufferAccessor[j];
					Entity prefab = nativeArray2[j].m_Prefab;
					StorageCompanyData storageCompanyData = m_StorageDatas[prefab];
					DynamicBuffer<TradeCost> costs = bufferAccessor2[j];
					if (!m_Limits.HasComponent(prefab))
					{
						continue;
					}
					StorageLimitData data = m_Limits[prefab];
					if (bufferAccessor3.Length != 0)
					{
						UpgradeUtils.CombineStats<StorageLimitData>(ref data, bufferAccessor3[j], ref m_PrefabRefData, ref m_Limits);
					}
					iterator = ResourceIterator.GetIterator();
					int num2 = EconomyUtils.CountResources(storageCompanyData.m_StoredResources);
					while (iterator.Next())
					{
						if ((storageCompanyData.m_StoredResources & iterator.resource) == Resource.NoResource)
						{
							continue;
						}
						if (iterator.resource == Resource.OutgoingMail)
						{
							EconomyUtils.SetResources(Resource.OutgoingMail, resources, 0);
							continue;
						}
						int resources2 = EconomyUtils.GetResources(iterator.resource, resources);
						int num3 = data.m_Limit / num2;
						if (iterator.resource == Resource.Garbage && m_GarbageFacilityDatas.HasComponent(prefab))
						{
							num3 = m_GarbageFacilityDatas[prefab].m_GarbageCapacity;
						}
						int num4 = num3 / 2 - resources2;
						float num5 = (float)num4 / (float)num3;
						int num6 = (int)((float)num4 * num5) / (kUpdatesPerDay / 8);
						EconomyUtils.AddResources(iterator.resource, num6, resources);
						int resourceIndex2 = EconomyUtils.GetResourceIndex(iterator.resource);
						ref NativeArray<int> reference = ref m_TradeBalances;
						int num7 = resourceIndex2;
						reference[num7] -= num6;
						OutsideConnectionTransferType type = m_OutsideConnectionDatas[prefab].m_Type;
						EconomyUtils.SetTradeCost(iterator.resource, GetCachedTradeCost(iterator.resource, type, m_CachedCosts), costs, keepLastTime: false);
						if (num6 != 0 && (iterator.resource & (Resource)28672uL) == Resource.NoResource)
						{
							m_StatisticsEventQueue.Enqueue(new StatisticsEvent
							{
								m_Statistic = StatisticType.Trade,
								m_Change = num6,
								m_Parameter = EconomyUtils.GetResourceIndex(iterator.resource)
							});
						}
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		public BufferTypeHandle<TradeCost> __Game_Companies_TradeCost_RW_BufferTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> __Game_Prefabs_StorageCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> __Game_Prefabs_GarbageFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Companies_TradeCost_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TradeCost>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_StorageCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageCompanyData>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageFacilityData>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	private static readonly float kRefreshRate = 0.01f;

	public static readonly int kUpdatesPerDay = 128;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private EntityQuery m_StorageGroup;

	private EntityQuery m_TradeParameterQuery;

	private EntityQuery m_CityQuery;

	private ResourceSystem m_ResourceSystem;

	[DebugWatchDeps]
	private JobHandle m_DebugTradeBalanceDeps;

	[ResourceArray]
	[DebugWatchValue]
	private NativeArray<int> m_TradeBalances;

	private NativeArray<float> m_CachedCosts;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public float GetTradePrice(Resource resource, OutsideConnectionTransferType type, bool import, DynamicBuffer<CityModifier> cityEffects)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		OutsideConnectionTransferType outsideConnectionTransferType = OutsideConnectionTransferType.Road;
		float value = float.MaxValue;
		while (outsideConnectionTransferType != OutsideConnectionTransferType.Last)
		{
			if ((outsideConnectionTransferType & type) != OutsideConnectionTransferType.None)
			{
				value = math.min(value, m_CachedCosts[GetCacheIndex(resource, type, import)]);
			}
			outsideConnectionTransferType = (OutsideConnectionTransferType)((int)outsideConnectionTransferType << 1);
		}
		if (import)
		{
			CityUtils.ApplyModifier(ref value, cityEffects, CityModifierType.ImportCost);
		}
		else
		{
			CityUtils.ApplyModifier(ref value, cityEffects, CityModifierType.ExportCost);
		}
		return value;
	}

	private static int GetCacheIndex(Resource resource, OutsideConnectionTransferType type, bool import)
	{
		return Mathf.RoundToInt(math.log2((float)type) * 2f * (float)EconomyUtils.ResourceCount + (float)(2 * EconomyUtils.GetResourceIndex(resource)) + (float)(import ? 1 : 0));
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_TradeBalances = new NativeArray<int>(EconomyUtils.ResourceCount, (Allocator)4, (NativeArrayOptions)1);
		m_CachedCosts = new NativeArray<float>(2 * EconomyUtils.ResourceCount * Mathf.RoundToInt(math.log2(32f)), (Allocator)4, (NativeArrayOptions)1);
		m_StorageGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadOnly<TradeCost>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_TradeParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<OutsideTradeParameterData>() });
		m_CityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.City.City>() });
		((ComponentSystemBase)this).RequireForUpdate(m_StorageGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_TradeParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_CityQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_CachedCosts.Dispose();
		m_TradeBalances.Dispose();
		base.OnDestroy();
	}

	private static TradeCost CalculateTradeCost(Resource resource, int tradeBalance, OutsideConnectionTransferType type, float weight, ref OutsideTradeParameterData tradeParameters, DynamicBuffer<CityModifier> cityEffects)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		float value = tradeParameters.GetWeightCost(type) * weight;
		if ((float)tradeBalance < 0f)
		{
			value *= 1f + tradeParameters.GetDistanceCost(type) * math.max(50f, math.sqrt((float)(-tradeBalance)));
		}
		CityUtils.ApplyModifier(ref value, cityEffects, CityModifierType.ImportCost);
		float value2 = tradeParameters.GetWeightCost(type) * weight;
		if ((float)tradeBalance > 0f)
		{
			value2 *= 1f + tradeParameters.GetDistanceCost(type) * math.max(50f, math.sqrt((float)tradeBalance));
		}
		CityUtils.ApplyModifier(ref value2, cityEffects, CityModifierType.ExportCost);
		return new TradeCost
		{
			m_Resource = resource,
			m_BuyCost = value,
			m_SellCost = value2
		};
	}

	protected override void OnGameLoaded(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((Context)(ref context)).purpose != 1)
		{
			return;
		}
		NativeArray<Entity> val = ((EntityQuery)(ref m_StorageGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
		StorageCompanyData storageCompanyData = default(StorageCompanyData);
		StorageLimitData storageLimitData = default(StorageLimitData);
		for (int i = 0; i < val.Length; i++)
		{
			Entity val2 = val[i];
			if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val2, ref prefabRef) || !EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, val2, false, ref resources) || !EntitiesExtensions.TryGetComponent<StorageCompanyData>(((ComponentSystemBase)this).EntityManager, (Entity)prefabRef, ref storageCompanyData) || !EntitiesExtensions.TryGetComponent<StorageLimitData>(((ComponentSystemBase)this).EntityManager, (Entity)prefabRef, ref storageLimitData))
			{
				continue;
			}
			ResourceIterator iterator = ResourceIterator.GetIterator();
			int num = EconomyUtils.CountResources(storageCompanyData.m_StoredResources);
			while (iterator.Next())
			{
				if ((storageCompanyData.m_StoredResources & iterator.resource) != Resource.NoResource)
				{
					if (iterator.resource == Resource.OutgoingMail)
					{
						EconomyUtils.SetResources(Resource.OutgoingMail, resources, 0);
						continue;
					}
					int resources2 = EconomyUtils.GetResources(iterator.resource, resources);
					int amount = storageLimitData.m_Limit / num / 2 - resources2;
					EconomyUtils.AddResources(iterator.resource, amount, resources);
				}
			}
		}
		val.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		JobHandle val = IJobExtensions.Schedule<TradeJob>(new TradeJob
		{
			m_Chunks = ((EntityQuery)(ref m_StorageGroup)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3)),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TradeCostType = InternalCompilerInterface.GetBufferTypeHandle<TradeCost>(ref __TypeHandle.__Game_Companies_TradeCost_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Limits = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StorageDatas = InternalCompilerInterface.GetComponentLookup<StorageCompanyData>(ref __TypeHandle.__Game_Prefabs_StorageCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionDatas = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarbageFacilityDatas = InternalCompilerInterface.GetComponentLookup<GarbageFacilityData>(ref __TypeHandle.__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityEffects = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_City = ((EntityQuery)(ref m_CityQuery)).GetSingletonEntity(),
			m_TradeBalances = m_TradeBalances,
			m_CachedCosts = m_CachedCosts,
			m_TradeParameters = ((EntityQuery)(ref m_TradeParameterQuery)).GetSingleton<OutsideTradeParameterData>(),
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_CityStatisticsSystem.AddWriter(val);
		m_ResourceSystem.AddPrefabsReader(val);
		m_DebugTradeBalanceDeps = val;
		((SystemBase)this).Dependency = val;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((IWriter)writer/*cast due to .constrained prefix*/).Write(m_TradeBalances);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.tradeBalance)
		{
			context = ((IReader)reader).context;
			ContextFormat format = ((Context)(ref context)).format;
			if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
			{
				NativeArray<int> val = m_TradeBalances;
				((IReader)reader/*cast due to .constrained prefix*/).Read(val);
			}
			else
			{
				NativeArray<int> subArray = m_TradeBalances.GetSubArray(0, 40);
				((IReader)reader/*cast due to .constrained prefix*/).Read(subArray);
				m_TradeBalances[40] = 0;
			}
		}
	}

	public void SetDefaults()
	{
		for (int i = 0; i < m_TradeBalances.Length; i++)
		{
			m_TradeBalances[i] = 0;
		}
	}

	public void SetDefaults(Context context)
	{
		SetDefaults();
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
	public TradeSystem()
	{
	}
}
