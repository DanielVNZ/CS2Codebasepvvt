using System.Runtime.CompilerServices;
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
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TouristSpawnSystem : GameSystemBase
{
	[BurstCompile]
	private struct SpawnTouristHouseholdJob : IJob
	{
		[ReadOnly]
		public NativeList<Entity> m_PrefabEntities;

		[ReadOnly]
		public NativeList<ArchetypeData> m_Archetypes;

		[ReadOnly]
		public NativeList<HouseholdData> m_HouseholdPrefabs;

		[ReadOnly]
		public NativeList<Entity> m_OutsideConnectionEntities;

		[ReadOnly]
		public ComponentLookup<Tourism> m_Tourisms;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionDatas;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefs;

		[ReadOnly]
		public AttractivenessParameterData m_AttractivenessParameter;

		[ReadOnly]
		public DemandParameterData m_DemandParameterData;

		public RandomSeed m_RandomSeed;

		public Entity m_City;

		public uint m_Frame;

		public EntityCommandBuffer m_CommandBuffer;

		public ClimateSystem.WeatherClassification m_WeatherClassification;

		public float m_Temperature;

		public float m_Precipitation;

		public bool m_IsRaining;

		public bool m_IsSnowing;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			if (!m_Tourisms.HasComponent(m_City))
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom((int)m_Frame);
			int attractiveness = m_Tourisms[m_City].m_Attractiveness;
			if (((Random)(ref random)).NextFloat() < TourismSystem.GetTouristProbability(m_AttractivenessParameter, attractiveness, m_WeatherClassification, m_Temperature, m_Precipitation, m_IsRaining, m_IsSnowing))
			{
				int num = ((Random)(ref random)).NextInt(m_HouseholdPrefabs.Length);
				Entity prefab = m_PrefabEntities[num];
				ArchetypeData archetypeData = m_Archetypes[num];
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(archetypeData.m_Archetype);
				PrefabRef prefabRef = new PrefabRef
				{
					m_Prefab = prefab
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, prefabRef);
				Household household = new Household
				{
					m_Flags = HouseholdFlags.Tourist
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Household>(val, household);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<TouristHousehold>(val, new TouristHousehold
				{
					m_Hotel = Entity.Null,
					m_LeavingTime = 0u
				});
				if (m_OutsideConnectionEntities.Length > 0 && BuildingUtils.GetRandomOutsideConnectionByParameters(ref m_OutsideConnectionEntities, ref m_OutsideConnectionDatas, ref m_PrefabRefs, random, m_DemandParameterData.m_TouristOCSpawnParameters, out var result))
				{
					CurrentBuilding currentBuilding = new CurrentBuilding
					{
						m_CurrentBuilding = result
					};
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CurrentBuilding>(val, currentBuilding);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Tourism> __Game_City_Tourism_RO_ComponentLookup;

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
			__Game_City_Tourism_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tourism>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
		}
	}

	private EntityQuery m_HouseholdPrefabQuery;

	private EntityQuery m_OutsideConnectionQuery;

	private EntityQuery m_AttractivenessParameterQuery;

	private EntityQuery m_DemandParameterQuery;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private ClimateSystem m_ClimateSystem;

	private CitySystem m_CitySystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_HouseholdPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ArchetypeData>(),
			ComponentType.ReadOnly<HouseholdData>()
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
		m_AttractivenessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AttractivenessParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_HouseholdPrefabQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_OutsideConnectionQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		JobHandle val2 = default(JobHandle);
		JobHandle val3 = default(JobHandle);
		JobHandle val4 = default(JobHandle);
		SpawnTouristHouseholdJob spawnTouristHouseholdJob = new SpawnTouristHouseholdJob
		{
			m_PrefabEntities = ((EntityQuery)(ref m_HouseholdPrefabQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
			m_Archetypes = ((EntityQuery)(ref m_HouseholdPrefabQuery)).ToComponentDataListAsync<ArchetypeData>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
			m_HouseholdPrefabs = ((EntityQuery)(ref m_HouseholdPrefabQuery)).ToComponentDataListAsync<HouseholdData>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val3),
			m_OutsideConnectionEntities = ((EntityQuery)(ref m_OutsideConnectionQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4),
			m_Tourisms = InternalCompilerInterface.GetComponentLookup<Tourism>(ref __TypeHandle.__Game_City_Tourism_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionDatas = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttractivenessParameter = ((EntityQuery)(ref m_AttractivenessParameterQuery)).GetSingleton<AttractivenessParameterData>(),
			m_DemandParameterData = ((EntityQuery)(ref m_DemandParameterQuery)).GetSingleton<DemandParameterData>(),
			m_WeatherClassification = m_ClimateSystem.classification,
			m_Temperature = m_ClimateSystem.temperature,
			m_Precipitation = m_ClimateSystem.precipitation,
			m_IsRaining = m_ClimateSystem.isRaining,
			m_IsSnowing = m_ClimateSystem.isSnowing,
			m_City = m_CitySystem.City,
			m_Frame = m_SimulationSystem.frameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<SpawnTouristHouseholdJob>(spawnTouristHouseholdJob, JobHandle.CombineDependencies(val, val2, JobHandle.CombineDependencies(val3, ((SystemBase)this).Dependency, val4)));
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public TouristSpawnSystem()
	{
	}
}
