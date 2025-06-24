using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class HouseholdSpawnSystem : GameSystemBase
{
	[BurstCompile]
	private struct SpawnHouseholdJob : IJob
	{
		[ReadOnly]
		public NativeList<Entity> m_PrefabEntities;

		[ReadOnly]
		public NativeList<ArchetypeData> m_Archetypes;

		[ReadOnly]
		public NativeList<Entity> m_OutsideConnectionEntities;

		[ReadOnly]
		public ComponentLookup<Population> m_Populations;

		[ReadOnly]
		public ComponentLookup<HouseholdData> m_HouseholdDatas;

		[ReadOnly]
		public ComponentLookup<DynamicHousehold> m_Dynamics;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public DemandParameterData m_DemandParameterData;

		public Entity m_City;

		public EntityCommandBuffer m_CommandBuffer;

		public int m_Demand;

		public Random m_Random;

		[ReadOnly]
		public NativeArray<int> m_LowFactors;

		[ReadOnly]
		public NativeArray<int> m_MedFactors;

		[ReadOnly]
		public NativeArray<int> m_HiFactors;

		[ReadOnly]
		public NativeArray<int> m_StudyPositions;

		private bool IsValidStudyPrefab(Entity householdPrefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			HouseholdData householdData = m_HouseholdDatas[householdPrefab];
			if ((m_StudyPositions[1] + m_StudyPositions[2] <= 0 || !((Random)(ref m_Random)).NextBool()) && m_StudyPositions[3] + m_StudyPositions[4] > 0)
			{
				return householdData.m_StudentCount > 0;
			}
			if (m_StudyPositions[1] + m_StudyPositions[2] > 0)
			{
				return householdData.m_ChildCount > 0;
			}
			return false;
		}

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			Population population = m_Populations[m_City];
			int num = Mathf.RoundToInt(300f / math.clamp(m_DemandParameterData.m_HouseholdSpawnSpeedFactor * math.log(1f + 0.001f * (float)population.m_Population), 0.5f, 20f));
			int num2 = ((Random)(ref m_Random)).NextInt(num);
			int num3 = 0;
			while (num2 < m_Demand)
			{
				num3++;
				m_Demand -= num2;
				num2 = ((Random)(ref m_Random)).NextInt(num);
			}
			if (num3 == 0)
			{
				return;
			}
			int num4 = m_LowFactors[6] + m_MedFactors[6] + m_HiFactors[6];
			int num5 = m_LowFactors[12] + m_MedFactors[12] + m_HiFactors[12];
			num4 = math.max(0, num4);
			num5 = math.max(0, num5);
			float num6 = (float)num5 / (float)(num5 + num4);
			for (int i = 0; i < num3; i++)
			{
				int num7 = 0;
				bool flag = ((Random)(ref m_Random)).NextFloat() < num6;
				for (int j = 0; j < m_PrefabEntities.Length; j++)
				{
					if (IsValidStudyPrefab(m_PrefabEntities[j]) == flag)
					{
						num7 += m_HouseholdDatas[m_PrefabEntities[j]].m_Weight;
					}
				}
				num7 = ((Random)(ref m_Random)).NextInt(num7);
				int num8 = 0;
				for (int k = 0; k < m_PrefabEntities.Length; k++)
				{
					if (IsValidStudyPrefab(m_PrefabEntities[k]) == flag)
					{
						num7 -= m_HouseholdDatas[m_PrefabEntities[k]].m_Weight;
					}
					if (num7 < 0)
					{
						num8 = k;
						break;
					}
				}
				Entity prefab = m_PrefabEntities[num8];
				ArchetypeData archetypeData = m_Archetypes[num8];
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(archetypeData.m_Archetype);
				PrefabRef prefabRef = new PrefabRef
				{
					m_Prefab = prefab
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, prefabRef);
				if (m_OutsideConnectionEntities.Length > 0 && BuildingUtils.GetRandomOutsideConnectionByParameters(ref m_OutsideConnectionEntities, ref m_OutsideConnectionDatas, ref m_PrefabRefs, m_Random, m_DemandParameterData.m_CitizenOCSpawnParameters, out var result))
				{
					CurrentBuilding currentBuilding = new CurrentBuilding
					{
						m_CurrentBuilding = result
					};
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CurrentBuilding>(val, currentBuilding);
				}
				else
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(val, default(Deleted));
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<HouseholdData> __Game_Prefabs_HouseholdData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DynamicHousehold> __Game_Prefabs_DynamicHousehold_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Population> __Game_City_Population_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

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
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_HouseholdData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdData>(true);
			__Game_Prefabs_DynamicHousehold_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DynamicHousehold>(true);
			__Game_City_Population_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Population>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
		}
	}

	private EntityQuery m_HouseholdPrefabQuery;

	private EntityQuery m_OutsideConnectionQuery;

	private EntityQuery m_DemandParameterQuery;

	private ResidentialDemandSystem m_ResidentialDemandSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private CountStudyPositionsSystem m_CountStudyPositionsSystem;

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
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ResidentialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResidentialDemandSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CountStudyPositionsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountStudyPositionsSystem>();
		m_HouseholdPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ArchetypeData>(),
			ComponentType.ReadOnly<HouseholdData>(),
			ComponentType.Exclude<DynamicHousehold>()
		});
		m_OutsideConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.Exclude<Game.Objects.WaterPipeOutsideConnection>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_HouseholdPrefabQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_OutsideConnectionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = ((SystemBase)this).Dependency;
		int householdDemand = m_ResidentialDemandSystem.householdDemand;
		if (householdDemand > 0)
		{
			JobHandle deps;
			NativeArray<int> lowDensityDemandFactors = m_ResidentialDemandSystem.GetLowDensityDemandFactors(out deps);
			JobHandle deps2;
			NativeArray<int> mediumDensityDemandFactors = m_ResidentialDemandSystem.GetMediumDensityDemandFactors(out deps2);
			JobHandle deps3;
			NativeArray<int> highDensityDemandFactors = m_ResidentialDemandSystem.GetHighDensityDemandFactors(out deps3);
			JobHandle val2 = default(JobHandle);
			JobHandle val3 = default(JobHandle);
			JobHandle val4 = default(JobHandle);
			val = IJobExtensions.Schedule<SpawnHouseholdJob>(new SpawnHouseholdJob
			{
				m_PrefabEntities = ((EntityQuery)(ref m_HouseholdPrefabQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
				m_Archetypes = ((EntityQuery)(ref m_HouseholdPrefabQuery)).ToComponentDataListAsync<ArchetypeData>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val3),
				m_OutsideConnectionEntities = ((EntityQuery)(ref m_OutsideConnectionQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4),
				m_HouseholdDatas = InternalCompilerInterface.GetComponentLookup<HouseholdData>(ref __TypeHandle.__Game_Prefabs_HouseholdData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Dynamics = InternalCompilerInterface.GetComponentLookup<DynamicHousehold>(ref __TypeHandle.__Game_Prefabs_DynamicHousehold_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Populations = InternalCompilerInterface.GetComponentLookup<Population>(ref __TypeHandle.__Game_City_Population_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnectionDatas = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DemandParameterData = ((EntityQuery)(ref m_DemandParameterQuery)).GetSingleton<DemandParameterData>(),
				m_LowFactors = lowDensityDemandFactors,
				m_MedFactors = mediumDensityDemandFactors,
				m_HiFactors = highDensityDemandFactors,
				m_StudyPositions = m_CountStudyPositionsSystem.GetStudyPositionsByEducation(out var deps4),
				m_City = m_CitySystem.City,
				m_Demand = householdDemand,
				m_Random = RandomSeed.Next().GetRandom((int)m_SimulationSystem.frameIndex),
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
			}, JobUtils.CombineDependencies(val2, val3, val, val4, deps, deps2, deps3, deps4));
			m_ResidentialDemandSystem.AddReader(val);
			m_EndFrameBarrier.AddJobHandleForProducer(val);
		}
		((SystemBase)this).Dependency = val;
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
	public HouseholdSpawnSystem()
	{
	}
}
