using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Tools;
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
public class IndustrialSpawnSystem : GameSystemBase
{
	[BurstCompile]
	private struct CheckSpawnJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProcessData> m_ProcessType;

		[ReadOnly]
		public ComponentTypeHandle<ArchetypeData> m_ArchetypeType;

		[ReadOnly]
		public NativeArray<int> m_ResourceDemands;

		[ReadOnly]
		public NativeArray<int> m_WarehouseDemands;

		[ReadOnly]
		public ResourcePrefabs m_ResourcePrefabs;

		[ReadOnly]
		public ComponentLookup<ResourceData> m_ResourceDatas;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_ProcessDatas;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_StorageChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_IndustrialChunks;

		[ReadOnly]
		public NativeList<PrefabRef> m_ExistingExtractorPrefabs;

		[ReadOnly]
		public NativeList<PrefabRef> m_ExistingIndustrialPrefabs;

		public int m_EmptySignatureBuildingCount;

		public Entity m_City;

		public EntityCommandBuffer m_CommandBuffer;

		public uint m_SimulationFrame;

		private void SpawnCompany(NativeList<ArchetypeChunk> chunks, Resource resource, ref Random random)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < chunks.Length; i++)
			{
				ArchetypeChunk val = chunks[i];
				NativeArray<IndustrialProcessData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<IndustrialProcessData>(ref m_ProcessType);
				for (int j = 0; j < ((ArchetypeChunk)(ref val)).Count; j++)
				{
					if ((resource & nativeArray[j].m_Output.m_Resource) != Resource.NoResource)
					{
						num++;
					}
				}
			}
			if (num <= 0)
			{
				return;
			}
			int num2 = ((Random)(ref random)).NextInt(num);
			for (int k = 0; k < chunks.Length; k++)
			{
				ArchetypeChunk val2 = chunks[k];
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
				NativeArray<IndustrialProcessData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<IndustrialProcessData>(ref m_ProcessType);
				NativeArray<ArchetypeData> nativeArray4 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ArchetypeData>(ref m_ArchetypeType);
				for (int l = 0; l < ((ArchetypeChunk)(ref val2)).Count; l++)
				{
					if ((resource & nativeArray3[l].m_Output.m_Resource) != Resource.NoResource)
					{
						if (num2 == 0)
						{
							Spawn(nativeArray2[l], nativeArray4[l]);
							return;
						}
						num2--;
					}
				}
			}
		}

		private void Spawn(Entity prefab, ArchetypeData archetypeData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(archetypeData.m_Archetype);
			PrefabRef prefabRef = new PrefabRef
			{
				m_Prefab = prefab
			};
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, prefabRef);
		}

		public void Execute()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			Random random = default(Random);
			((Random)(ref random))._002Ector(m_SimulationFrame);
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int resourceIndex = EconomyUtils.GetResourceIndex(iterator.resource);
				int num = m_ResourceDemands[resourceIndex];
				ResourceData resourceData = m_ResourceDatas[m_ResourcePrefabs[iterator.resource]];
				if (resourceData.m_IsProduceable)
				{
					if (!resourceData.m_IsMaterial)
					{
						Population population = m_Populations[m_City];
						if (m_EmptySignatureBuildingCount > 0 || ((Random)(ref random)).NextInt(Mathf.RoundToInt(5000f / math.min(5f, math.max(1f, math.log10((float)(1 + population.m_Population)))))) < num)
						{
							bool flag = false;
							for (int i = 0; i < m_ExistingIndustrialPrefabs.Length; i++)
							{
								if (m_ProcessDatas[m_ExistingIndustrialPrefabs[i].m_Prefab].m_Output.m_Resource == iterator.resource)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								SpawnCompany(m_IndustrialChunks, iterator.resource, ref random);
							}
						}
					}
					else
					{
						bool flag2 = false;
						for (int j = 0; j < m_ExistingExtractorPrefabs.Length; j++)
						{
							if (m_ProcessDatas[m_ExistingExtractorPrefabs[j].m_Prefab].m_Output.m_Resource == iterator.resource)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							SpawnCompany(m_IndustrialChunks, iterator.resource, ref random);
							break;
						}
					}
				}
				if (resourceData.m_IsTradable && m_WarehouseDemands[resourceIndex] > 0)
				{
					SpawnCompany(m_StorageChunks, iterator.resource, ref random);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<ArchetypeData> __Game_Prefabs_ArchetypeData_RO_ComponentTypeHandle;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Prefabs_ArchetypeData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ArchetypeData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_IndustrialProcessData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<IndustrialProcessData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
		}
	}

	private EntityQuery m_IndustrialCompanyPrefabQuery;

	private EntityQuery m_StorageCompanyPrefabQuery;

	private EntityQuery m_ExtractorQuery;

	private EntityQuery m_ExtractorCompanyQuery;

	private EntityQuery m_ExistingIndustrialQuery;

	private EntityQuery m_ExistingExtractorQuery;

	private EndFrameBarrier m_EndFrameBarrier;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private SimulationSystem m_SimulationSystem;

	private ResourceSystem m_ResourceSystem;

	private CitySystem m_CitySystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_IndustrialCompanyPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<ArchetypeData>(),
			ComponentType.ReadOnly<IndustrialCompanyData>(),
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.Exclude<StorageCompanyData>()
		});
		m_StorageCompanyPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ArchetypeData>(),
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.ReadOnly<StorageCompanyData>()
		});
		m_ExtractorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ExtractorProperty>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ExtractorCompanyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Companies.ExtractorCompany>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ExistingIndustrialQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[7]
		{
			ComponentType.ReadOnly<IndustrialCompany>(),
			ComponentType.Exclude<Game.Companies.ExtractorCompany>(),
			ComponentType.Exclude<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<PropertyRenter>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_ExistingExtractorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Companies.ExtractorCompany>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<PropertyRenter>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_IndustrialCompanyPrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		if (m_SimulationSystem.frameIndex / 16 % 8 == 2 && m_IndustrialDemandSystem.industrialCompanyDemand + m_IndustrialDemandSystem.storageCompanyDemand + m_IndustrialDemandSystem.officeCompanyDemand > 0)
		{
			JobHandle val = default(JobHandle);
			JobHandle val2 = default(JobHandle);
			JobHandle val3 = default(JobHandle);
			JobHandle val4 = default(JobHandle);
			JobHandle deps;
			JobHandle deps2;
			CheckSpawnJob checkSpawnJob = new CheckSpawnJob
			{
				m_ArchetypeType = InternalCompilerInterface.GetComponentTypeHandle<ArchetypeData>(ref __TypeHandle.__Game_Prefabs_ArchetypeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ProcessType = InternalCompilerInterface.GetComponentTypeHandle<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_IndustrialChunks = ((EntityQuery)(ref m_IndustrialCompanyPrefabQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_StorageChunks = ((EntityQuery)(ref m_StorageCompanyPrefabQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
				m_ExistingExtractorPrefabs = ((EntityQuery)(ref m_ExistingExtractorQuery)).ToComponentDataListAsync<PrefabRef>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val3),
				m_ExistingIndustrialPrefabs = ((EntityQuery)(ref m_ExistingIndustrialQuery)).ToComponentDataListAsync<PrefabRef>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4),
				m_ProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDemands = m_IndustrialDemandSystem.GetResourceDemands(out deps),
				m_WarehouseDemands = m_IndustrialDemandSystem.GetStorageCompanyDemands(out deps2),
				m_City = m_CitySystem.City,
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_SimulationFrame = m_SimulationSystem.frameIndex,
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<CheckSpawnJob>(checkSpawnJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val, val2, val3, val4, deps, deps2));
			m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public IndustrialSpawnSystem()
	{
	}
}
