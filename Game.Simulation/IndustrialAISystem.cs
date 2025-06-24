using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
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
public class IndustrialAISystem : GameSystemBase
{
	[BurstCompile]
	private struct CompanyAITickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourceType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeBufType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<WorkProvider> m_WorkProviderType;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> m_VehicleType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitDatas;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_IndustrialProcessDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		[ReadOnly]
		public ComponentLookup<PropertySeeker> m_PropertySeekers;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public BufferLookup<Efficiency> m_EfficiencyBuf;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_Layouts;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> m_Trucks;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenDatas;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameters;

		[ReadOnly]
		public CountCompanyDataSystem.IndustrialCompanyDatas m_IndustrialCompanyDatas;

		public uint m_UpdateFrameIndex;

		public RandomSeed m_Random;

		public ParallelWriter m_CommandBuffer;

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
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WorkProvider> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WorkProvider>(ref m_WorkProviderType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourceType);
			BufferAccessor<OwnedVehicle> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<OwnedVehicle>(ref m_VehicleType);
			BufferAccessor<Employee> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeBufType);
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				WorkProvider workProvider = nativeArray2[i];
				DynamicBuffer<Employee> val2 = bufferAccessor3[i];
				Entity prefab = m_Prefabs[val].m_Prefab;
				StorageLimitData storageLimitData = m_StorageLimitDatas[prefab];
				IndustrialProcessData processData = m_IndustrialProcessDatas[prefab];
				if (m_PropertyRenters.HasComponent(val))
				{
					Entity property = m_PropertyRenters[val].m_Property;
					Entity prefab2 = m_Prefabs[property].m_Prefab;
					int resourceIndex = EconomyUtils.GetResourceIndex(processData.m_Output.m_Resource);
					int num = m_IndustrialCompanyDatas.m_Demand[resourceIndex] - m_IndustrialCompanyDatas.m_Production[resourceIndex];
					int length = val2.Length;
					int level = 5;
					if (m_SpawnableBuildingDatas.TryGetComponent(prefab2, ref spawnableBuildingData))
					{
						level = spawnableBuildingData.m_Level;
					}
					int industrialAndOfficeFittingWorkers = CompanyUtils.GetIndustrialAndOfficeFittingWorkers(m_BuildingDatas[prefab2], m_BuildingPropertyDatas[prefab2], level, processData);
					int resources = EconomyUtils.GetResources(processData.m_Output.m_Resource, bufferAccessor[i]);
					if (EconomyUtils.GetWeight(processData.m_Output.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas) > 0f)
					{
						if (workProvider.m_MaxWorkers > kMinimumEmployee && resources >= storageLimitData.m_Limit / 2 && num < 0)
						{
							workProvider.m_MaxWorkers--;
						}
						else if (length == workProvider.m_MaxWorkers && industrialAndOfficeFittingWorkers - workProvider.m_MaxWorkers > 1 && resources <= storageLimitData.m_Limit / 4)
						{
							workProvider.m_MaxWorkers++;
						}
					}
					else if (workProvider.m_MaxWorkers > kMinimumEmployee && resources >= kMaxVirtualResourceStorage && num < 0)
					{
						workProvider.m_MaxWorkers--;
					}
					else if (length == workProvider.m_MaxWorkers && industrialAndOfficeFittingWorkers - workProvider.m_MaxWorkers > 1 && resources <= kMaxVirtualResourceStorage / 2)
					{
						workProvider.m_MaxWorkers++;
					}
					nativeArray2[i] = workProvider;
				}
				if (m_PropertySeekers.IsComponentEnabled(val))
				{
					continue;
				}
				if (m_PropertyRenters.HasComponent(val))
				{
					Random random = m_Random.GetRandom(val.Index);
					if (((Random)(ref random)).NextInt(4) != 0)
					{
						continue;
					}
				}
				if (EconomyUtils.GetCompanyTotalWorth(bufferAccessor[i], bufferAccessor2[i], ref m_Layouts, ref m_Trucks, m_ResourcePrefabs, ref m_ResourceDatas) > kLowestCompanyWorth)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponentEnabled<PropertySeeker>(unfilteredChunkIndex, val, true);
				}
				else if (!m_PropertyRenters.HasComponent(val))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, val, default(Deleted));
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
		public BufferTypeHandle<Resources> __Game_Economy_Resources_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		public ComponentTypeHandle<WorkProvider> __Game_Companies_WorkProvider_RW_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> __Game_Companies_StorageLimitData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertySeeker> __Game_Agents_PropertySeeker_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

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
			__Game_Economy_Resources_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(true);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Companies_WorkProvider_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkProvider>(false);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<OwnedVehicle>(true);
			__Game_Companies_StorageLimitData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StorageLimitData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
			__Game_Agents_PropertySeeker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertySeeker>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.DeliveryTruck>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 32;

	public static readonly int kLowestCompanyWorth = -10000;

	public static readonly int kMinimumEmployee = 5;

	public static readonly int kMaxVirtualResourceStorage = 100000;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private ResourceSystem m_ResourceSystem;

	private CountCompanyDataSystem m_CountCompanyDataSystem;

	private EntityQuery m_CompanyQuery;

	private EntityQuery m_EconomyParameterQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_CountCompanyDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountCompanyDataSystem>();
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_CompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[11]
		{
			ComponentType.ReadOnly<Game.Companies.ProcessingCompany>(),
			ComponentType.ReadOnly<BuyingCompany>(),
			ComponentType.ReadWrite<WorkProvider>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.Exclude<ServiceAvailable>(),
			ComponentType.Exclude<Game.Companies.ExtractorCompany>(),
			ComponentType.Exclude<Game.Companies.StorageCompany>(),
			ComponentType.Exclude<Created>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CompanyQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		((COSystemBase)this).OnStopRunning();
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
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		CompanyAITickJob companyAITickJob = new CompanyAITickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EmployeeBufType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WorkProviderType = InternalCompilerInterface.GetComponentTypeHandle<WorkProvider>(ref __TypeHandle.__Game_Companies_WorkProvider_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleType = InternalCompilerInterface.GetBufferTypeHandle<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StorageLimitDatas = InternalCompilerInterface.GetComponentLookup<StorageLimitData>(ref __TypeHandle.__Game_Companies_StorageLimitData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		companyAITickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		companyAITickJob.m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_PropertySeekers = InternalCompilerInterface.GetComponentLookup<PropertySeeker>(ref __TypeHandle.__Game_Agents_PropertySeeker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_SpawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_EfficiencyBuf = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_Layouts = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_Trucks = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_CitizenDatas = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		companyAITickJob.m_Random = RandomSeed.Next();
		companyAITickJob.m_ResourcePrefabs = m_ResourceSystem.GetPrefabs();
		companyAITickJob.m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>();
		companyAITickJob.m_IndustrialCompanyDatas = m_CountCompanyDataSystem.GetIndustrialCompanyDatas(out var deps);
		companyAITickJob.m_UpdateFrameIndex = updateFrame;
		CompanyAITickJob companyAITickJob2 = companyAITickJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CompanyAITickJob>(companyAITickJob2, m_CompanyQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
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
	public IndustrialAISystem()
	{
	}
}
