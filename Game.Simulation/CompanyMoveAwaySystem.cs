using System.Runtime.CompilerServices;
using Game.Agents;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
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
public class CompanyMoveAwaySystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckMoveAwayJob : IJobChunk
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_DeliveryTrucks;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_WorkplaceDatas;

		[ReadOnly]
		public ComponentLookup<ServiceAvailable> m_ServiceAvailables;

		[ReadOnly]
		public ComponentLookup<OfficeProperty> m_OfficeProperties;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<WorkProvider> m_WorkProviders;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameters;

		[ReadOnly]
		public NativeArray<int> m_TaxRates;

		public RandomSeed m_RandomSeed;

		public uint m_UpdateFrameIndex;

		public ParallelWriter m_CommandBuffer;

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
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<PropertyRenter> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				DynamicBuffer<Resources> resources = bufferAccessor[i];
				Entity val = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				Entity property = nativeArray3[i].m_Property;
				if (m_WorkplaceDatas.HasComponent(prefab))
				{
					int companyTotalWorth;
					if (m_OwnedVehicles.HasBuffer(val))
					{
						DynamicBuffer<OwnedVehicle> vehicles = m_OwnedVehicles[val];
						companyTotalWorth = EconomyUtils.GetCompanyTotalWorth(resources, vehicles, ref m_LayoutElements, ref m_DeliveryTrucks, m_ResourcePrefabs, ref m_ResourceDatas);
					}
					else
					{
						companyTotalWorth = EconomyUtils.GetCompanyTotalWorth(resources, m_ResourcePrefabs, ref m_ResourceDatas);
					}
					int companyMoveAwayChance = CompanyUtils.GetCompanyMoveAwayChance(val, prefab, property, ref m_ServiceAvailables, ref m_OfficeProperties, ref m_IndustrialProcessDatas, ref m_WorkProviders, m_TaxRates);
					if (companyTotalWorth < m_EconomyParameters.m_CompanyBankruptcyLimit || ((Random)(ref random)).NextInt(100) < companyMoveAwayChance)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<MovingAway>(unfilteredChunkIndex, val, default(MovingAway));
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct MovingAwayJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_RenterType;

		[ReadOnly]
		public ComponentLookup<PropertyOnMarket> m_OnMarkets;

		[ReadOnly]
		public ComponentLookup<WorkProvider> m_WorkProviders;

		[ReadOnly]
		public ComponentLookup<Abandoned> m_Abandoneds;

		[ReadOnly]
		public EntityArchetype m_RentEventArchetype;

		[ReadOnly]
		public WorkProviderParameterData m_WorkProviderParameterData;

		public ParallelWriter m_CommandBuffer;

		public IconCommandBuffer m_IconCommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PropertyRenter> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_RenterType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Entity property = nativeArray2[i].m_Property;
				if (property != Entity.Null)
				{
					if (!m_OnMarkets.HasComponent(property) && !m_Abandoneds.HasComponent(property))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PropertyToBeOnMarket>(unfilteredChunkIndex, property, default(PropertyToBeOnMarket));
					}
					Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, m_RentEventArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RentersUpdated>(unfilteredChunkIndex, val2, new RentersUpdated(property));
				}
				if (m_WorkProviders.HasComponent(val))
				{
					WorkProvider workProvider = m_WorkProviders[val];
					if (workProvider.m_EducatedNotificationEntity != Entity.Null)
					{
						m_IconCommandBuffer.Remove(workProvider.m_EducatedNotificationEntity, m_WorkProviderParameterData.m_EducatedNotificationPrefab);
					}
					if (workProvider.m_UneducatedNotificationEntity != Entity.Null)
					{
						m_IconCommandBuffer.Remove(workProvider.m_UneducatedNotificationEntity, m_WorkProviderParameterData.m_UneducatedNotificationPrefab);
					}
				}
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Resources> __Game_Economy_Resources_RO_BufferTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceAvailable> __Game_Companies_ServiceAvailable_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OfficeProperty> __Game_Buildings_OfficeProperty_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkProvider> __Game_Companies_WorkProvider_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyOnMarket> __Game_Buildings_PropertyOnMarket_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Abandoned> __Game_Buildings_Abandoned_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Economy_Resources_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Companies_ServiceAvailable_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceAvailable>(true);
			__Game_Buildings_OfficeProperty_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OfficeProperty>(true);
			__Game_Companies_WorkProvider_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkProvider>(true);
			__Game_Buildings_PropertyOnMarket_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyOnMarket>(true);
			__Game_Buildings_Abandoned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Abandoned>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 16;

	private EntityQuery m_CompanyQuery;

	private EntityQuery m_MovingAwayQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityArchetype m_RentEventArchetype;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private ResourceSystem m_ResourceSystem;

	private TaxSystem m_TaxSystem;

	private IconCommandSystem m_IconCommandSystem;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_731167828_0;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_TaxSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_CompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[9]
		{
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<WorkProvider>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Game.Companies.ExtractorCompany>(),
			ComponentType.Exclude<MovingAway>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_MovingAwayQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.ReadOnly<MovingAway>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_RentEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<RentersUpdated>()
		});
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_CompanyQuery, m_MovingAwayQuery });
		((ComponentSystemBase)this).RequireForUpdate<WorkProviderParameterData>();
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		JobHandle val = default(JobHandle);
		EntityCommandBuffer val2;
		if (!((EntityQuery)(ref m_CompanyQuery)).IsEmptyIgnoreFilter)
		{
			CheckMoveAwayJob checkMoveAwayJob = new CheckMoveAwayJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeliveryTrucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WorkplaceDatas = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceAvailables = InternalCompilerInterface.GetComponentLookup<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OfficeProperties = InternalCompilerInterface.GetComponentLookup<OfficeProperty>(ref __TypeHandle.__Game_Buildings_OfficeProperty_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WorkProviders = InternalCompilerInterface.GetComponentLookup<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>()
			};
			val2 = m_EndFrameBarrier.CreateCommandBuffer();
			checkMoveAwayJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkMoveAwayJob.m_TaxRates = m_TaxSystem.GetTaxRates();
			checkMoveAwayJob.m_RandomSeed = RandomSeed.Next();
			checkMoveAwayJob.m_UpdateFrameIndex = updateFrame;
			val = JobChunkExtensions.ScheduleParallel<CheckMoveAwayJob>(checkMoveAwayJob, m_CompanyQuery, ((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(val);
			m_ResourceSystem.AddPrefabsReader(val);
			((SystemBase)this).Dependency = val;
		}
		if (!((EntityQuery)(ref m_MovingAwayQuery)).IsEmptyIgnoreFilter)
		{
			MovingAwayJob movingAwayJob = new MovingAwayJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OnMarkets = InternalCompilerInterface.GetComponentLookup<PropertyOnMarket>(ref __TypeHandle.__Game_Buildings_PropertyOnMarket_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WorkProviders = InternalCompilerInterface.GetComponentLookup<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Abandoneds = InternalCompilerInterface.GetComponentLookup<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RentEventArchetype = m_RentEventArchetype,
				m_WorkProviderParameterData = ((EntityQuery)(ref __query_731167828_0)).GetSingleton<WorkProviderParameterData>(),
				m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
			};
			val2 = m_EndFrameBarrier.CreateCommandBuffer();
			movingAwayJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			JobHandle val3 = JobChunkExtensions.ScheduleParallel<MovingAwayJob>(movingAwayJob, m_MovingAwayQuery, JobHandle.CombineDependencies(val, ((SystemBase)this).Dependency));
			m_EndFrameBarrier.AddJobHandleForProducer(val3);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(val3, val);
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<WorkProviderParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_731167828_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public CompanyMoveAwaySystem()
	{
	}
}
