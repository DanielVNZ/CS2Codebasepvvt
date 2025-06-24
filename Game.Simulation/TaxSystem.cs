using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Agents;
using Game.Areas;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Prefabs.Modes;
using Game.Serialization;
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
public class TaxSystem : GameSystemBase, ITaxSystem, IDefaultSerializable, ISerializable, IPostDeserialize
{
	[BurstCompile]
	private struct PayTaxJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<TaxPayer> m_TaxPayerType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public ComponentLookup<Worker> m_Workers;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_ProcessDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		public uint m_UpdateFrameIndex;

		public IncomeSource m_Type;

		public float m_PaidMultiplier;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		private void PayTax(ref TaxPayer taxPayer, Entity entity, DynamicBuffer<Resources> resources, IncomeSource taxType, ParallelWriter<StatisticsEvent> statisticsEventQueue)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			int tax = GetTax(taxPayer);
			int num = (int)math.round(m_PaidMultiplier * (float)tax);
			EconomyUtils.AddResources(Resource.Money, -num, resources);
			if (taxType == IncomeSource.TaxResidential)
			{
				int parameter = 0;
				if (m_HouseholdCitizens.HasBuffer(entity))
				{
					DynamicBuffer<HouseholdCitizen> val = m_HouseholdCitizens[entity];
					for (int i = 0; i < val.Length; i++)
					{
						Entity citizen = val[i].m_Citizen;
						if (m_Workers.HasComponent(citizen))
						{
							parameter = m_Workers[citizen].m_Level;
							break;
						}
					}
				}
				statisticsEventQueue.Enqueue(new StatisticsEvent
				{
					m_Statistic = StatisticType.ResidentialTaxableIncome,
					m_Change = taxPayer.m_UntaxedIncome * kUpdatesPerDay,
					m_Parameter = parameter
				});
			}
			else
			{
				int parameter2 = 0;
				StatisticType statisticType = ((taxType == IncomeSource.TaxCommercial) ? StatisticType.CommercialTaxableIncome : StatisticType.IndustrialTaxableIncome);
				if (m_Prefabs.HasComponent(entity))
				{
					Entity prefab = m_Prefabs[entity].m_Prefab;
					if (m_ProcessDatas.HasComponent(prefab))
					{
						Resource resource = m_ProcessDatas[prefab].m_Output.m_Resource;
						parameter2 = EconomyUtils.GetResourceIndex(resource);
						if (statisticType == StatisticType.IndustrialTaxableIncome && m_ResourceDatas[m_ResourcePrefabs[resource]].m_Weight == 0f)
						{
							taxType = IncomeSource.TaxOffice;
							statisticType = StatisticType.OfficeTaxableIncome;
						}
					}
				}
				statisticsEventQueue.Enqueue(new StatisticsEvent
				{
					m_Statistic = statisticType,
					m_Change = taxPayer.m_UntaxedIncome * kUpdatesPerDay,
					m_Parameter = parameter2
				});
			}
			taxPayer.m_UntaxedIncome = 0;
		}

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<TaxPayer> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TaxPayer>(ref m_TaxPayerType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				TaxPayer taxPayer = nativeArray2[i];
				DynamicBuffer<Resources> resources = bufferAccessor[i];
				if (taxPayer.m_UntaxedIncome != 0)
				{
					PayTax(ref taxPayer, nativeArray[i], resources, m_Type, m_StatisticsEventQueue);
					nativeArray2[i] = taxPayer;
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

		public ComponentTypeHandle<TaxPayer> __Game_Agents_TaxPayer_RW_ComponentTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

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
			__Game_Agents_TaxPayer_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxPayer>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 32;

	private NativeArray<int> m_TaxRates;

	private EntityQuery m_ResidentialTaxPayerGroup;

	private EntityQuery m_CommercialTaxPayerGroup;

	private EntityQuery m_IndustrialTaxPayerGroup;

	private EntityQuery m_TaxParameterGroup;

	private EntityQuery m_GameModeSettingQuery;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private SimulationSystem m_SimulationSystem;

	private ResourceSystem m_ResourceSystem;

	private TaxParameterData m_TaxParameterData;

	private float3 m_TaxPaidMultiplier;

	private JobHandle m_Readers;

	private TypeHandle __TypeHandle;

	public int TaxRate
	{
		get
		{
			return m_TaxRates[0];
		}
		set
		{
			m_TaxRates[0] = math.min(m_TaxParameterData.m_TotalTaxLimits.y, math.max(m_TaxParameterData.m_TotalTaxLimits.x, value));
			EnsureAreaTaxRateLimits(TaxAreaType.Residential);
			EnsureAreaTaxRateLimits(TaxAreaType.Commercial);
			EnsureAreaTaxRateLimits(TaxAreaType.Industrial);
			EnsureAreaTaxRateLimits(TaxAreaType.Office);
		}
	}

	public JobHandle Readers => m_Readers;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if (((EntityQuery)(ref m_GameModeSettingQuery)).IsEmptyIgnoreFilter)
		{
			m_TaxPaidMultiplier = new float3(1f, 1f, 1f);
			return;
		}
		ModeSettingData singleton = ((EntityQuery)(ref m_GameModeSettingQuery)).GetSingleton<ModeSettingData>();
		if (singleton.m_Enable)
		{
			m_TaxPaidMultiplier = singleton.m_TaxPaidMultiplier;
		}
		else
		{
			m_TaxPaidMultiplier = new float3(1f, 1f, 1f);
		}
	}

	public TaxParameterData GetTaxParameterData()
	{
		if (!((EntityQuery)(ref m_TaxParameterGroup)).IsEmptyIgnoreFilter)
		{
			EnsureTaxParameterData();
			return m_TaxParameterData;
		}
		return default(TaxParameterData);
	}

	public static int GetTax(TaxPayer payer)
	{
		return (int)math.round(0.01f * (float)payer.m_AverageTaxRate * (float)payer.m_UntaxedIncome);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		int length = m_TaxRates.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		NativeArray<int> val = m_TaxRates;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.averageTaxRate)
		{
			context = ((IReader)reader).context;
			int num = default(int);
			if (((Context)(ref context)).version >= Version.taxRateArrayLength)
			{
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			}
			else
			{
				num = 53;
			}
			NativeArray<int> val = default(NativeArray<int>);
			val._002Ector(num, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<int> val2 = val;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
			for (int i = 0; i < val.Length; i++)
			{
				m_TaxRates[i] = val[i];
			}
			val.Dispose();
		}
		context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (!((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			for (int num2 = 90; num2 > 50; num2--)
			{
				m_TaxRates[num2] = m_TaxRates[num2 - 1];
			}
			m_TaxRates[10 + EconomyUtils.GetResourceIndex(Resource.Fish)] = 0;
			m_TaxRates[51 + EconomyUtils.GetResourceIndex(Resource.Fish)] = 0;
		}
	}

	public void SetDefaults(Context context)
	{
		m_TaxRates[0] = 10;
		for (int i = 1; i < m_TaxRates.Length; i++)
		{
			m_TaxRates[i] = 0;
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_TaxRates.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_ResidentialTaxPayerGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<TaxPayer>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.ReadOnly<Household>()
		});
		m_CommercialTaxPayerGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<TaxPayer>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.ReadOnly<ServiceAvailable>()
		});
		m_IndustrialTaxPayerGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadWrite<TaxPayer>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<Resources>(),
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Game.Companies.StorageCompany>(),
			ComponentType.Exclude<ServiceAvailable>()
		});
		m_TaxParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TaxParameterData>() });
		m_GameModeSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		m_TaxRates = new NativeArray<int>(92, (Allocator)4, (NativeArrayOptions)1);
		m_TaxRates[0] = 10;
		m_TaxPaidMultiplier = new float3(1f, 1f, 1f);
		((ComponentSystemBase)this).RequireForUpdate(m_TaxParameterGroup);
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((Context)(ref context)).version < Version.averageTaxRate)
		{
			m_TaxRates[0] = 10;
			for (int i = 1; i < m_TaxRates.Length; i++)
			{
				m_TaxRates[i] = 0;
			}
		}
	}

	public NativeArray<int> GetTaxRates()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_TaxRates;
	}

	public void AddReader(JobHandle reader)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Readers = JobHandle.CombineDependencies(m_Readers, reader);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		EnsureTaxParameterData();
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		m_TaxParameterData = ((EntityQuery)(ref m_TaxParameterGroup)).GetSingleton<TaxParameterData>();
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		JobHandle deps;
		JobHandle val = JobChunkExtensions.ScheduleParallel<PayTaxJob>(new PayTaxJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxPayerType = InternalCompilerInterface.GetComponentTypeHandle<TaxPayer>(ref __TypeHandle.__Game_Agents_TaxPayer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = ((ComponentSystemBase)this).GetSharedComponentTypeHandle<UpdateFrame>(),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = prefabs,
			m_Type = IncomeSource.TaxResidential,
			m_UpdateFrameIndex = updateFrame,
			m_PaidMultiplier = m_TaxPaidMultiplier.x,
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter()
		}, m_ResidentialTaxPayerGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_CityStatisticsSystem.AddWriter(val);
		updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<PayTaxJob>(new PayTaxJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxPayerType = InternalCompilerInterface.GetComponentTypeHandle<TaxPayer>(ref __TypeHandle.__Game_Agents_TaxPayer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = ((ComponentSystemBase)this).GetSharedComponentTypeHandle<UpdateFrame>(),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = prefabs,
			m_Type = IncomeSource.TaxCommercial,
			m_UpdateFrameIndex = updateFrame,
			m_PaidMultiplier = m_TaxPaidMultiplier.y,
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter()
		}, m_CommercialTaxPayerGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_CityStatisticsSystem.AddWriter(val2);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<PayTaxJob>(new PayTaxJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxPayerType = InternalCompilerInterface.GetComponentTypeHandle<TaxPayer>(ref __TypeHandle.__Game_Agents_TaxPayer_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = ((ComponentSystemBase)this).GetSharedComponentTypeHandle<UpdateFrame>(),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Workers = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = prefabs,
			m_Type = IncomeSource.TaxIndustrial,
			m_UpdateFrameIndex = updateFrame,
			m_PaidMultiplier = m_TaxPaidMultiplier.z,
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter()
		}, m_IndustrialTaxPayerGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_CityStatisticsSystem.AddWriter(val3);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val, val2, val3);
	}

	public int GetTaxRate(TaxAreaType areaType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetTaxRate(areaType, m_TaxRates);
	}

	public static int GetTaxRate(TaxAreaType areaType, NativeArray<int> taxRates)
	{
		return taxRates[0] + taxRates[(int)areaType];
	}

	public int2 GetTaxRateRange(TaxAreaType areaType)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (areaType == TaxAreaType.Residential)
		{
			return GetTaxRate(areaType) + GetJobLevelTaxRateRange();
		}
		return GetTaxRate(areaType) + GetResourceTaxRateRange(areaType);
	}

	public int GetModifiedTaxRate(TaxAreaType areaType, Entity district, BufferLookup<DistrictModifier> policies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetModifiedTaxRate(areaType, m_TaxRates, district, policies);
	}

	public static int GetModifiedTaxRate(TaxAreaType areaType, NativeArray<int> taxRates, Entity district, BufferLookup<DistrictModifier> policies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		float value = GetTaxRate(areaType, taxRates);
		if (policies.HasBuffer(district))
		{
			DynamicBuffer<DistrictModifier> modifiers = policies[district];
			AreaUtils.ApplyModifier(ref value, modifiers, DistrictModifierType.LowCommercialTax);
		}
		return (int)math.round(value);
	}

	private void EnsureTaxParameterData()
	{
		if (m_TaxParameterData.m_TotalTaxLimits.x == m_TaxParameterData.m_TotalTaxLimits.y)
		{
			m_TaxParameterData = ((EntityQuery)(ref m_TaxParameterGroup)).GetSingleton<TaxParameterData>();
		}
	}

	public void SetTaxRate(TaxAreaType areaType, int rate)
	{
		m_TaxRates[(int)areaType] = rate - m_TaxRates[0];
		EnsureAreaTaxRateLimits(areaType);
	}

	private void EnsureAreaTaxRateLimits(TaxAreaType areaType)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		switch (areaType)
		{
		case TaxAreaType.Residential:
		{
			int2 officeTaxLimits = m_TaxParameterData.m_ResidentialTaxLimits;
			m_TaxRates[(int)areaType] = math.min(officeTaxLimits.y, math.max(officeTaxLimits.x, GetTaxRate(areaType))) - m_TaxRates[0];
			for (int i = 0; i < 5; i++)
			{
				EnsureJobLevelTaxRateLimits(i);
			}
			break;
		}
		case TaxAreaType.Commercial:
		{
			int2 officeTaxLimits = m_TaxParameterData.m_CommercialTaxLimits;
			m_TaxRates[(int)areaType] = math.min(officeTaxLimits.y, math.max(officeTaxLimits.x, GetTaxRate(areaType))) - m_TaxRates[0];
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				if (EconomyUtils.IsCommercialResource(iterator.resource))
				{
					EnsureResourceTaxRateLimits(areaType, iterator.resource);
				}
			}
			break;
		}
		case TaxAreaType.Industrial:
		{
			int2 officeTaxLimits = m_TaxParameterData.m_IndustrialTaxLimits;
			m_TaxRates[(int)areaType] = math.min(officeTaxLimits.y, math.max(officeTaxLimits.x, GetTaxRate(areaType))) - m_TaxRates[0];
			ResourceIterator iterator = ResourceIterator.GetIterator();
			ResourceData resourceData = default(ResourceData);
			while (iterator.Next())
			{
				if (!EntitiesExtensions.TryGetComponent<ResourceData>(((ComponentSystemBase)this).EntityManager, m_ResourceSystem.GetPrefab(iterator.resource), ref resourceData) || (resourceData.m_IsProduceable && resourceData.m_Weight > 0f))
				{
					EnsureResourceTaxRateLimits(areaType, iterator.resource);
				}
			}
			break;
		}
		case TaxAreaType.Office:
		{
			int2 officeTaxLimits = m_TaxParameterData.m_OfficeTaxLimits;
			m_TaxRates[(int)areaType] = math.min(officeTaxLimits.y, math.max(officeTaxLimits.x, GetTaxRate(areaType))) - m_TaxRates[0];
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				if (EconomyUtils.IsOfficeResource(iterator.resource))
				{
					EnsureResourceTaxRateLimits(areaType, iterator.resource);
				}
			}
			break;
		}
		}
	}

	private void EnsureJobLevelTaxRateLimits(int jobLevel)
	{
		m_TaxRates[5 + jobLevel] = math.min(m_TaxParameterData.m_JobLevelTaxLimits.y, math.max(m_TaxParameterData.m_JobLevelTaxLimits.x, GetResidentialTaxRate(jobLevel))) - GetTaxRate(TaxAreaType.Residential);
	}

	private void ClampResidentialTaxRates()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		int2 jobLevelTaxRateRange = GetJobLevelTaxRateRange();
		int num = 0;
		if (jobLevelTaxRateRange.x > 0)
		{
			num = jobLevelTaxRateRange.x;
		}
		else if (jobLevelTaxRateRange.y < 0)
		{
			num = jobLevelTaxRateRange.y;
		}
		if (num != 0)
		{
			ref NativeArray<int> reference = ref m_TaxRates;
			reference[1] = reference[1] + num;
			for (int i = 0; i < 5; i++)
			{
				reference = ref m_TaxRates;
				int num2 = 5 + i;
				reference[num2] -= num;
			}
		}
	}

	private int2 GetJobLevelTaxRateRange()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		int2 val = default(int2);
		((int2)(ref val))._002Ector(int.MaxValue, int.MinValue);
		for (int i = 0; i < 5; i++)
		{
			int num = m_TaxRates[5 + i];
			val.x = math.min(val.x, num);
			val.y = math.max(val.y, num);
		}
		return val;
	}

	private void EnsureResourceTaxRateLimits(TaxAreaType areaType, Resource resource)
	{
		switch (areaType)
		{
		case TaxAreaType.Commercial:
		{
			int taxRate = GetTaxRate(TaxAreaType.Commercial);
			m_TaxRates[10 + EconomyUtils.GetResourceIndex(resource)] = math.min(m_TaxParameterData.m_ResourceTaxLimits.y, math.max(m_TaxParameterData.m_ResourceTaxLimits.x, GetCommercialTaxRate(resource))) - taxRate;
			break;
		}
		case TaxAreaType.Industrial:
		{
			int taxRate = GetTaxRate(TaxAreaType.Industrial);
			m_TaxRates[51 + EconomyUtils.GetResourceIndex(resource)] = math.min(m_TaxParameterData.m_ResourceTaxLimits.y, math.max(m_TaxParameterData.m_ResourceTaxLimits.x, GetIndustrialTaxRate(resource))) - taxRate;
			break;
		}
		case TaxAreaType.Office:
		{
			int taxRate = GetTaxRate(TaxAreaType.Office);
			m_TaxRates[51 + EconomyUtils.GetResourceIndex(resource)] = math.min(m_TaxParameterData.m_ResourceTaxLimits.y, math.max(m_TaxParameterData.m_ResourceTaxLimits.x, GetOfficeTaxRate(resource))) - taxRate;
			break;
		}
		}
	}

	private void ClampResourceTaxRates(TaxAreaType areaType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		int2 resourceTaxRateRange = GetResourceTaxRateRange(areaType);
		int num = 0;
		if (resourceTaxRateRange.x > 0)
		{
			num = resourceTaxRateRange.x;
		}
		else if (resourceTaxRateRange.y < 0)
		{
			num = resourceTaxRateRange.y;
		}
		if (num == 0)
		{
			return;
		}
		ref NativeArray<int> reference = ref m_TaxRates;
		int num2 = (int)areaType;
		reference[num2] += num;
		int zeroOffset = GetZeroOffset(areaType);
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		ResourceIterator iterator = ResourceIterator.GetIterator();
		TaxableResourceData taxableResourceData = default(TaxableResourceData);
		while (iterator.Next())
		{
			Entity val = prefabs[iterator.resource];
			if (EntitiesExtensions.TryGetComponent<TaxableResourceData>(((ComponentSystemBase)this).EntityManager, val, ref taxableResourceData) && taxableResourceData.Contains(areaType))
			{
				reference = ref m_TaxRates;
				num2 = zeroOffset + EconomyUtils.GetResourceIndex(iterator.resource);
				reference[num2] -= num;
			}
		}
	}

	private int2 GetResourceTaxRateRange(TaxAreaType areaType)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		int2 val = default(int2);
		((int2)(ref val))._002Ector(int.MaxValue, int.MinValue);
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		ResourceIterator iterator = ResourceIterator.GetIterator();
		int zeroOffset = GetZeroOffset(areaType);
		TaxableResourceData taxableResourceData = default(TaxableResourceData);
		while (iterator.Next())
		{
			Entity val2 = prefabs[iterator.resource];
			if (EntitiesExtensions.TryGetComponent<TaxableResourceData>(((ComponentSystemBase)this).EntityManager, val2, ref taxableResourceData) && taxableResourceData.Contains(areaType))
			{
				int num = m_TaxRates[zeroOffset + EconomyUtils.GetResourceIndex(iterator.resource)];
				val.x = math.min(val.x, num);
				val.y = math.max(val.y, num);
			}
		}
		return val;
	}

	private int GetZeroOffset(TaxAreaType areaType)
	{
		switch (areaType)
		{
		case TaxAreaType.Commercial:
			return 10;
		case TaxAreaType.Industrial:
		case TaxAreaType.Office:
			return 51;
		default:
			throw new ArgumentOutOfRangeException("areaType", areaType, null);
		}
	}

	public int GetResidentialTaxRate(int jobLevel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetResidentialTaxRate(jobLevel, m_TaxRates);
	}

	public static int GetResidentialTaxRate(int jobLevel, NativeArray<int> taxRates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetTaxRate(TaxAreaType.Residential, taxRates) + taxRates[5 + jobLevel];
	}

	public void SetResidentialTaxRate(int jobLevel, int rate)
	{
		m_TaxRates[5 + jobLevel] = rate - GetTaxRate(TaxAreaType.Residential);
		EnsureJobLevelTaxRateLimits(jobLevel);
		ClampResidentialTaxRates();
	}

	public int GetCommercialTaxRate(Resource resource)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetCommercialTaxRate(resource, m_TaxRates);
	}

	public static int GetCommercialTaxRate(Resource resource, NativeArray<int> taxRates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetTaxRate(TaxAreaType.Commercial, taxRates) + taxRates[10 + EconomyUtils.GetResourceIndex(resource)];
	}

	public int GetModifiedCommercialTaxRate(Resource resource, Entity district, BufferLookup<DistrictModifier> policies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetModifiedCommercialTaxRate(resource, m_TaxRates, district, policies);
	}

	public static int GetModifiedCommercialTaxRate(Resource resource, NativeArray<int> taxRates, Entity district, BufferLookup<DistrictModifier> policies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GetModifiedTaxRate(TaxAreaType.Commercial, taxRates, district, policies) + taxRates[10 + EconomyUtils.GetResourceIndex(resource)];
	}

	public void SetCommercialTaxRate(Resource resource, int rate)
	{
		m_TaxRates[10 + EconomyUtils.GetResourceIndex(resource)] = rate - GetTaxRate(TaxAreaType.Commercial);
		EnsureResourceTaxRateLimits(TaxAreaType.Commercial, resource);
		ClampResourceTaxRates(TaxAreaType.Commercial);
	}

	public int GetIndustrialTaxRate(Resource resource)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetIndustrialTaxRate(resource, m_TaxRates);
	}

	public static int GetIndustrialTaxRate(Resource resource, NativeArray<int> taxRates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetTaxRate(TaxAreaType.Industrial, taxRates) + taxRates[51 + EconomyUtils.GetResourceIndex(resource)];
	}

	public void SetIndustrialTaxRate(Resource resource, int rate)
	{
		m_TaxRates[51 + EconomyUtils.GetResourceIndex(resource)] = rate - GetTaxRate(TaxAreaType.Industrial);
		EnsureResourceTaxRateLimits(TaxAreaType.Industrial, resource);
		ClampResourceTaxRates(TaxAreaType.Industrial);
	}

	public int GetOfficeTaxRate(Resource resource)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetOfficeTaxRate(resource, m_TaxRates);
	}

	public static int GetOfficeTaxRate(Resource resource, NativeArray<int> taxRates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetTaxRate(TaxAreaType.Office, taxRates) + taxRates[51 + EconomyUtils.GetResourceIndex(resource)];
	}

	public void SetOfficeTaxRate(Resource resource, int rate)
	{
		m_TaxRates[51 + EconomyUtils.GetResourceIndex(resource)] = rate - GetTaxRate(TaxAreaType.Office);
		EnsureResourceTaxRateLimits(TaxAreaType.Office, resource);
		ClampResourceTaxRates(TaxAreaType.Office);
	}

	public int GetTaxRateEffect(TaxAreaType areaType, int taxRate)
	{
		return 0;
	}

	public int GetEstimatedTaxAmount(TaxAreaType areaType, TaxResultType resultType, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetEstimatedTaxAmount(areaType, resultType, statisticsLookup, stats, m_TaxRates);
	}

	public static int GetEstimatedTaxAmount(TaxAreaType areaType, TaxResultType resultType, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, NativeArray<int> taxRates)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		switch (areaType)
		{
		case TaxAreaType.Residential:
		{
			for (int i = 0; i < 5; i++)
			{
				int estimatedResidentialTaxIncome = GetEstimatedResidentialTaxIncome(i, statisticsLookup, stats, taxRates);
				if (MatchesResultType(estimatedResidentialTaxIncome, resultType))
				{
					num += estimatedResidentialTaxIncome;
				}
			}
			return num;
		}
		case TaxAreaType.Commercial:
		{
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int estimatedCommercialTaxIncome = GetEstimatedCommercialTaxIncome(iterator.resource, statisticsLookup, stats, taxRates);
				if (MatchesResultType(estimatedCommercialTaxIncome, resultType))
				{
					num += estimatedCommercialTaxIncome;
				}
			}
			return num;
		}
		case TaxAreaType.Industrial:
		{
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int estimatedIndustrialTaxIncome = GetEstimatedIndustrialTaxIncome(iterator.resource, statisticsLookup, stats, taxRates);
				if (MatchesResultType(estimatedIndustrialTaxIncome, resultType))
				{
					num += estimatedIndustrialTaxIncome;
				}
			}
			return num;
		}
		case TaxAreaType.Office:
		{
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int estimatedOfficeTaxIncome = GetEstimatedOfficeTaxIncome(iterator.resource, statisticsLookup, stats, taxRates);
				if (MatchesResultType(estimatedOfficeTaxIncome, resultType))
				{
					num += estimatedOfficeTaxIncome;
				}
			}
			return num;
		}
		default:
			return 0;
		}
	}

	private static bool MatchesResultType(int amount, TaxResultType resultType)
	{
		if (resultType != TaxResultType.Any && (resultType != TaxResultType.Income || amount <= 0))
		{
			if (resultType == TaxResultType.Expense)
			{
				return amount < 0;
			}
			return false;
		}
		return true;
	}

	public int GetEstimatedResidentialTaxIncome(int jobLevel, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetEstimatedResidentialTaxIncome(jobLevel, statisticsLookup, stats, m_TaxRates);
	}

	public int GetEstimatedCommercialTaxIncome(Resource resource, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetEstimatedCommercialTaxIncome(resource, statisticsLookup, stats, m_TaxRates);
	}

	public int GetEstimatedIndustrialTaxIncome(Resource resource, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetEstimatedIndustrialTaxIncome(resource, statisticsLookup, stats, m_TaxRates);
	}

	public int GetEstimatedOfficeTaxIncome(Resource resource, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetEstimatedOfficeTaxIncome(resource, statisticsLookup, stats, m_TaxRates);
	}

	public static int GetEstimatedResidentialTaxIncome(int jobLevel, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, NativeArray<int> taxRates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return (int)((long)GetResidentialTaxRate(jobLevel, taxRates) * (long)CityStatisticsSystem.GetStatisticValue(statisticsLookup, stats, StatisticType.ResidentialTaxableIncome, jobLevel) / 100);
	}

	public static int GetEstimatedCommercialTaxIncome(Resource resource, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, NativeArray<int> taxRates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return (int)((long)GetCommercialTaxRate(resource, taxRates) * (long)CityStatisticsSystem.GetStatisticValue(statisticsLookup, stats, StatisticType.CommercialTaxableIncome, EconomyUtils.GetResourceIndex(resource)) / 100);
	}

	public static int GetEstimatedIndustrialTaxIncome(Resource resource, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, NativeArray<int> taxRates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return (int)((long)GetIndustrialTaxRate(resource, taxRates) * (long)CityStatisticsSystem.GetStatisticValue(statisticsLookup, stats, StatisticType.IndustrialTaxableIncome, EconomyUtils.GetResourceIndex(resource)) / 100);
	}

	public static int GetEstimatedOfficeTaxIncome(Resource resource, NativeParallelHashMap<CityStatisticsSystem.StatisticsKey, Entity> statisticsLookup, BufferLookup<CityStatistic> stats, NativeArray<int> taxRates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return (int)((long)GetOfficeTaxRate(resource, taxRates) * (long)CityStatisticsSystem.GetStatisticValue(statisticsLookup, stats, StatisticType.OfficeTaxableIncome, EconomyUtils.GetResourceIndex(resource)) / 100);
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
	public TaxSystem()
	{
	}
}
