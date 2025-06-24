using System.Runtime.CompilerServices;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Objects;
using Game.Prefabs;
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
public class HouseholdPetSpawnSystem : GameSystemBase
{
	[BurstCompile]
	private struct HouseholdPetSpawnJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentTransport> m_CurrentTransportType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<PetData> m_PetDataType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<HouseholdPetData> m_HouseholdPetData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_ObjectData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_AnimalPrefabChunks;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_ResetTripArchetype;

		public ParallelWriter m_CommandBuffer;

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
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CurrentTransport> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentTransport>(ref m_CurrentTransportType);
			NativeArray<CurrentBuilding> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			NativeArray<Target> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			if (nativeArray3.Length == 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Target>(unfilteredChunkIndex, nativeArray[i]);
				}
				return;
			}
			if (nativeArray2.Length == 0)
			{
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					CurrentBuilding currentBuilding = nativeArray3[j];
					Target target = nativeArray4[j];
					PrefabRef prefabRef = nativeArray5[j];
					HouseholdPetData householdPetData = m_HouseholdPetData[prefabRef.m_Prefab];
					Random random = m_RandomSeed.GetRandom(val.Index);
					PseudoRandomSeed randomSeed;
					Entity val2 = ObjectEmergeSystem.SelectAnimalPrefab(ref random, householdPetData.m_Type, m_AnimalPrefabChunks, m_EntityType, m_PetDataType, out randomSeed);
					if (val2 != Entity.Null && m_TransformData.HasComponent(currentBuilding.m_CurrentBuilding))
					{
						Entity transport = SpawnPet(unfilteredChunkIndex, val, currentBuilding.m_CurrentBuilding, target.m_Target, val2, randomSeed);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentTransport>(unfilteredChunkIndex, val, new CurrentTransport(transport));
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentBuilding>(unfilteredChunkIndex, val);
					}
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Target>(unfilteredChunkIndex, val);
				}
				return;
			}
			for (int k = 0; k < nativeArray.Length; k++)
			{
				Entity val3 = nativeArray[k];
				CurrentBuilding currentBuilding2 = nativeArray3[k];
				CurrentTransport currentTransport = nativeArray2[k];
				Target target2 = nativeArray4[k];
				if (!m_DeletedData.HasComponent(currentTransport.m_CurrentTransport))
				{
					Entity val4 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, m_ResetTripArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(unfilteredChunkIndex, val4, new ResetTrip
					{
						m_Creature = currentTransport.m_CurrentTransport,
						m_Source = currentBuilding2.m_CurrentBuilding,
						m_Target = target2.m_Target,
						m_Delay = 512u
					});
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentBuilding>(unfilteredChunkIndex, val3);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Target>(unfilteredChunkIndex, val3);
				}
			}
		}

		private Entity SpawnPet(int jobIndex, Entity householdPet, Entity source, Entity target, Entity prefab, PseudoRandomSeed randomSeed)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			ObjectData objectData = m_ObjectData[prefab];
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(prefab));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val, m_TransformData[source]);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val, new Target(target));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Creatures.Pet>(jobIndex, val, new Game.Creatures.Pet(householdPet));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val, randomSeed);
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TripSource>(jobIndex, val, new TripSource(source, 512u));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Unspawned>(jobIndex, val, default(Unspawned));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<AnimalCurrentLane>(jobIndex, val, default(AnimalCurrentLane));
			return val;
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
		public ComponentTypeHandle<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PetData> __Game_Prefabs_PetData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HouseholdPetData> __Game_Prefabs_HouseholdPetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_CurrentTransport_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentTransport>(true);
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_PetData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PetData>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_HouseholdPetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HouseholdPetData>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
		}
	}

	private EntityQuery m_HouseholdPetQuery;

	private EntityQuery m_AnimalPrefabQuery;

	private EntityArchetype m_ResetTripArchetype;

	private EndFrameBarrier m_EndFrameBarrier;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_HouseholdPetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<HouseholdPet>(),
			ComponentType.ReadOnly<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_AnimalPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<AnimalData>(),
			ComponentType.ReadOnly<PetData>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ResetTripArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<ResetTrip>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_HouseholdPetQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> animalPrefabChunks = ((EntityQuery)(ref m_AnimalPrefabQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		HouseholdPetSpawnJob householdPetSpawnJob = new HouseholdPetSpawnJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportType = InternalCompilerInterface.GetComponentTypeHandle<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PetDataType = InternalCompilerInterface.GetComponentTypeHandle<PetData>(ref __TypeHandle.__Game_Prefabs_PetData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdPetData = InternalCompilerInterface.GetComponentLookup<HouseholdPetData>(ref __TypeHandle.__Game_Prefabs_HouseholdPetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalPrefabChunks = animalPrefabChunks,
			m_RandomSeed = RandomSeed.Next(),
			m_ResetTripArchetype = m_ResetTripArchetype
		};
		EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
		householdPetSpawnJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<HouseholdPetSpawnJob>(householdPetSpawnJob, m_HouseholdPetQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		animalPrefabChunks.Dispose(val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		((SystemBase)this).Dependency = val3;
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
	public HouseholdPetSpawnSystem()
	{
	}
}
