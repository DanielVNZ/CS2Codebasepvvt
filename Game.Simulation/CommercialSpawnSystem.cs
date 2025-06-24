using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CommercialSpawnSystem : GameSystemBase
{
	[BurstCompile]
	private struct SpawnCompanyJob : IJob
	{
		[ReadOnly]
		public NativeList<Entity> m_CompanyPrefabs;

		[ReadOnly]
		public NativeList<Entity> m_PropertyLessCompanies;

		[ReadOnly]
		public NativeArray<int> m_ResourceDemands;

		[ReadOnly]
		public Random m_Random;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public DemandParameterData m_DemandParameterData;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> m_Processes;

		[ReadOnly]
		public ComponentLookup<ArchetypeData> m_Archetypes;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		public int m_EmptySignatureBuildingCount;

		public NativeArray<uint> m_LastSpawnedCommercialFrame;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			ResourceIterator iterator = ResourceIterator.GetIterator();
			while (iterator.Next())
			{
				int resourceIndex = EconomyUtils.GetResourceIndex(iterator.resource);
				int num = m_ResourceDemands[resourceIndex];
				if (m_EmptySignatureBuildingCount <= 0 && (num <= 0 || m_FrameIndex - m_LastSpawnedCommercialFrame[resourceIndex] <= m_DemandParameterData.m_FrameIntervalForSpawning.y))
				{
					continue;
				}
				bool flag = false;
				for (int i = 0; i < m_PropertyLessCompanies.Length; i++)
				{
					Entity val = m_PropertyLessCompanies[i];
					if (m_Prefabs.HasComponent(val))
					{
						Entity prefab = m_Prefabs[val].m_Prefab;
						if (m_Processes.HasComponent(prefab) && m_Processes[prefab].m_Output.m_Resource == iterator.resource)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					SpawnCompany(iterator.resource);
					m_LastSpawnedCommercialFrame[resourceIndex] = m_FrameIndex;
				}
			}
		}

		private void SpawnCompany(Resource resource)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			if (m_CompanyPrefabs.Length <= 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < m_CompanyPrefabs.Length; i++)
			{
				if ((resource & m_Processes[m_CompanyPrefabs[i]].m_Output.m_Resource) != Resource.NoResource)
				{
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			int num2 = ((Random)(ref m_Random)).NextInt(num);
			for (int j = 0; j < m_CompanyPrefabs.Length; j++)
			{
				if ((resource & m_Processes[m_CompanyPrefabs[j]].m_Output.m_Resource) != Resource.NoResource)
				{
					if (num2 == 0)
					{
						Entity val = m_CompanyPrefabs[j];
						ArchetypeData archetypeData = m_Archetypes[val];
						Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(archetypeData.m_Archetype);
						PrefabRef prefabRef = new PrefabRef
						{
							m_Prefab = val
						};
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val2, prefabRef);
					}
					num2--;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<ArchetypeData> __Game_Prefabs_ArchetypeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_ArchetypeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ArchetypeData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
		}
	}

	private EntityQuery m_CommercialCompanyPrefabGroup;

	private EntityQuery m_PropertyLessCompanyGroup;

	private EntityQuery m_DemandParameterQuery;

	private CommercialDemandSystem m_CommercialDemandSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private NativeArray<uint> m_LastSpawnedCommercialFrame;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
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
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CommercialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CommercialDemandSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CommercialCompanyPrefabGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ArchetypeData>(),
			ComponentType.ReadOnly<CommercialCompanyData>(),
			ComponentType.ReadOnly<IndustrialProcessData>()
		});
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		m_PropertyLessCompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<CommercialCompany>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<PropertyRenter>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_LastSpawnedCommercialFrame = new NativeArray<uint>(EconomyUtils.ResourceCount, (Allocator)4, (NativeArrayOptions)1);
		((ComponentSystemBase)this).RequireForUpdate(m_CommercialCompanyPrefabGroup);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_LastSpawnedCommercialFrame.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		if (m_SimulationSystem.frameIndex / 16 % 8 == 1 && m_CommercialDemandSystem.companyDemand > 0)
		{
			JobHandle val = default(JobHandle);
			JobHandle val2 = default(JobHandle);
			JobHandle deps;
			SpawnCompanyJob spawnCompanyJob = new SpawnCompanyJob
			{
				m_CompanyPrefabs = ((EntityQuery)(ref m_CommercialCompanyPrefabGroup)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_PropertyLessCompanies = ((EntityQuery)(ref m_PropertyLessCompanyGroup)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
				m_Archetypes = InternalCompilerInterface.GetComponentLookup<ArchetypeData>(ref __TypeHandle.__Game_Prefabs_ArchetypeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Processes = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDemands = m_CommercialDemandSystem.GetResourceDemands(out deps),
				m_Random = RandomSeed.Next().GetRandom((int)m_SimulationSystem.frameIndex),
				m_FrameIndex = m_SimulationSystem.frameIndex,
				m_LastSpawnedCommercialFrame = m_LastSpawnedCommercialFrame,
				m_DemandParameterData = ((EntityQuery)(ref m_DemandParameterQuery)).GetSingleton<DemandParameterData>(),
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<SpawnCompanyJob>(spawnCompanyJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val, val2, deps));
			m_CommercialDemandSystem.AddReader(((SystemBase)this).Dependency);
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
	public CommercialSpawnSystem()
	{
	}
}
