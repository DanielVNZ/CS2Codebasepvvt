using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Citizens;

[CompilerGenerated]
public class CompanyInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeCompanyJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.ProcessingCompany> m_ProcessingCompanyType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.ExtractorCompany> m_ExtractionCompanyType;

		public ComponentTypeHandle<CompanyData> m_CompanyType;

		public ComponentTypeHandle<Profitability> m_ProfitabilityType;

		public BufferTypeHandle<Resources> m_ResourcesType;

		public ComponentTypeHandle<ServiceAvailable> m_ServiceAvailableType;

		public ComponentTypeHandle<LodgingProvider> m_LodgingProviderType;

		[ReadOnly]
		public BufferLookup<CompanyBrandElement> m_Brands;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_ProcessDatas;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> m_ServiceCompanyDatas;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> m_PropertyRenters;

		public EconomyParameterData m_EconomyParameters;

		public ParallelWriter<RentAction> m_RentActionQueue;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		public RandomSeed m_RandomSeed;

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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<CompanyData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CompanyData>(ref m_CompanyType);
			NativeArray<Profitability> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Profitability>(ref m_ProfitabilityType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourcesType);
			NativeArray<ServiceAvailable> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceAvailable>(ref m_ServiceAvailableType);
			NativeArray<LodgingProvider> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<LodgingProvider>(ref m_LodgingProviderType);
			bool flag = nativeArray5.Length != 0;
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Game.Companies.ProcessingCompany>(ref m_ProcessingCompanyType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Game.Companies.ExtractorCompany>(ref m_ExtractionCompanyType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity val = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				Random random = m_RandomSeed.GetRandom(val.Index);
				DynamicBuffer<CompanyBrandElement> val2 = m_Brands[prefab];
				Entity brand = ((val2.Length != 0) ? val2[((Random)(ref random)).NextInt(val2.Length)].m_Brand : Entity.Null);
				nativeArray3[i] = new CompanyData
				{
					m_RandomSeed = random,
					m_Brand = brand
				};
				nativeArray4[i] = new Profitability
				{
					m_Profitability = 127
				};
				if (flag)
				{
					ServiceCompanyData serviceCompanyData = m_ServiceCompanyDatas[prefab];
					nativeArray5[i] = new ServiceAvailable
					{
						m_ServiceAvailable = serviceCompanyData.m_MaxService / 2,
						m_MeanPriority = 0f
					};
				}
				if (flag2)
				{
					IndustrialProcessData industrialProcessData = m_ProcessDatas[prefab];
					DynamicBuffer<Resources> buffer = bufferAccessor[i];
					if (flag)
					{
						AddStartingResources(buffer, industrialProcessData.m_Input1.m_Resource, 3000);
						AddStartingResources(buffer, industrialProcessData.m_Input2.m_Resource, 3000);
					}
					else
					{
						AddStartingResources(buffer, industrialProcessData.m_Input1.m_Resource, 15000);
						AddStartingResources(buffer, industrialProcessData.m_Input2.m_Resource, 15000);
						bool flag4 = EconomyUtils.IsMaterial(industrialProcessData.m_Output.m_Resource, m_ResourcePrefabs, ref m_ResourceDatas);
						if (!flag3)
						{
							AddStartingResources(buffer, industrialProcessData.m_Output.m_Resource, flag4 ? 1000 : 0);
						}
					}
				}
				if (m_PropertyRenters.HasComponent(val) && m_PropertyRenters[val].m_Property != Entity.Null)
				{
					m_RentActionQueue.Enqueue(new RentAction
					{
						m_Property = m_PropertyRenters[val].m_Property,
						m_Renter = val
					});
				}
			}
			for (int j = 0; j < nativeArray6.Length; j++)
			{
				nativeArray6[j] = new LodgingProvider
				{
					m_FreeRooms = 0,
					m_Price = -1
				};
			}
		}

		private void AddStartingResources(DynamicBuffer<Resources> buffer, Resource resource, int amount)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (resource != Resource.NoResource)
			{
				int num = (int)math.round((float)amount * EconomyUtils.GetIndustrialPrice(resource, m_ResourcePrefabs, ref m_ResourceDatas));
				EconomyUtils.AddResources(resource, amount, buffer);
				EconomyUtils.AddResources(Resource.Money, -num, buffer);
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
		public ComponentTypeHandle<Game.Companies.ProcessingCompany> __Game_Companies_ProcessingCompany_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.ExtractorCompany> __Game_Companies_ExtractorCompany_RO_ComponentTypeHandle;

		public ComponentTypeHandle<CompanyData> __Game_Companies_CompanyData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Profitability> __Game_Companies_Profitability_RW_ComponentTypeHandle;

		public BufferTypeHandle<Resources> __Game_Economy_Resources_RW_BufferTypeHandle;

		public ComponentTypeHandle<ServiceAvailable> __Game_Companies_ServiceAvailable_RW_ComponentTypeHandle;

		public ComponentTypeHandle<LodgingProvider> __Game_Companies_LodgingProvider_RW_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<CompanyBrandElement> __Game_Prefabs_CompanyBrandElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> __Game_Companies_ServiceCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

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
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Companies_ProcessingCompany_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Companies.ProcessingCompany>(true);
			__Game_Companies_ExtractorCompany_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Companies.ExtractorCompany>(true);
			__Game_Companies_CompanyData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyData>(false);
			__Game_Companies_Profitability_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Profitability>(false);
			__Game_Economy_Resources_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Resources>(false);
			__Game_Companies_ServiceAvailable_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ServiceAvailable>(false);
			__Game_Companies_LodgingProvider_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LodgingProvider>(false);
			__Game_Prefabs_CompanyBrandElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CompanyBrandElement>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Companies_ServiceCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceCompanyData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
		}
	}

	private ResourceSystem m_ResourceSystem;

	private PropertyProcessingSystem m_PropertyProcessingSystem;

	private EntityQuery m_CreatedGroup;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1030701297_0;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_PropertyProcessingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PropertyProcessingSystem>();
		m_CreatedGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<CompanyData>(),
			ComponentType.ReadWrite<Profitability>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedGroup);
		((ComponentSystemBase)this).RequireForUpdate<EconomyParameterData>();
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
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		InitializeCompanyJob initializeCompanyJob = new InitializeCompanyJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ProcessingCompanyType = InternalCompilerInterface.GetComponentTypeHandle<Game.Companies.ProcessingCompany>(ref __TypeHandle.__Game_Companies_ProcessingCompany_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractionCompanyType = InternalCompilerInterface.GetComponentTypeHandle<Game.Companies.ExtractorCompany>(ref __TypeHandle.__Game_Companies_ExtractorCompany_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyType = InternalCompilerInterface.GetComponentTypeHandle<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ProfitabilityType = InternalCompilerInterface.GetComponentTypeHandle<Profitability>(ref __TypeHandle.__Game_Companies_Profitability_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcesType = InternalCompilerInterface.GetBufferTypeHandle<Resources>(ref __TypeHandle.__Game_Economy_Resources_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceAvailableType = InternalCompilerInterface.GetComponentTypeHandle<ServiceAvailable>(ref __TypeHandle.__Game_Companies_ServiceAvailable_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LodgingProviderType = InternalCompilerInterface.GetComponentTypeHandle<LodgingProvider>(ref __TypeHandle.__Game_Companies_LodgingProvider_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Brands = InternalCompilerInterface.GetBufferLookup<CompanyBrandElement>(ref __TypeHandle.__Game_Prefabs_CompanyBrandElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceCompanyDatas = InternalCompilerInterface.GetComponentLookup<ServiceCompanyData>(ref __TypeHandle.__Game_Companies_ServiceCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_RandomSeed = RandomSeed.Next(),
			m_EconomyParameters = ((EntityQuery)(ref __query_1030701297_0)).GetSingleton<EconomyParameterData>(),
			m_RentActionQueue = m_PropertyProcessingSystem.GetRentActionQueue(out deps).AsParallelWriter()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<InitializeCompanyJob>(initializeCompanyJob, m_CreatedGroup, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_PropertyProcessingSystem.AddWriter(((SystemBase)this).Dependency);
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
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
		__query_1030701297_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public CompanyInitializeSystem()
	{
	}
}
