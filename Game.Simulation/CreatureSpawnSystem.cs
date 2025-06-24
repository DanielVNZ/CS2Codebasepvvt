using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Objects;
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
public class CreatureSpawnSystem : GameSystemBase
{
	private struct SpawnData : IComparable<SpawnData>
	{
		public Entity m_Source;

		public Entity m_Creature;

		public int m_Priority;

		public int CompareTo(SpawnData other)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			return math.select(m_Priority - other.m_Priority, m_Source.Index - other.m_Source.Index, m_Source.Index != other.m_Source.Index);
		}
	}

	private struct SpawnRange
	{
		public int m_Start;

		public int m_End;
	}

	[BurstCompile]
	private struct GroupSpawnSourcesJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<TripSource> m_TripSourceType;

		public NativeList<SpawnData> m_SpawnData;

		public NativeList<SpawnRange> m_SpawnGroups;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			SpawnData spawnData = default(SpawnData);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<TripSource> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<TripSource>(ref m_TripSourceType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					TripSource tripSource = nativeArray2[j];
					if (tripSource.m_Timer <= 0)
					{
						spawnData.m_Source = tripSource.m_Source;
						spawnData.m_Creature = nativeArray[j];
						spawnData.m_Priority = tripSource.m_Timer;
						m_SpawnData.Add(ref spawnData);
					}
					tripSource.m_Timer -= 16;
					nativeArray2[j] = tripSource;
				}
			}
			if (m_SpawnData.Length == 0)
			{
				return;
			}
			NativeSortExtension.Sort<SpawnData>(m_SpawnData);
			SpawnRange spawnRange = default(SpawnRange);
			spawnRange.m_Start = -1;
			Entity val2 = Entity.Null;
			for (int k = 0; k < m_SpawnData.Length; k++)
			{
				Entity val3 = m_SpawnData[k].m_Source;
				if (val3 != val2)
				{
					if (spawnRange.m_Start != -1)
					{
						spawnRange.m_End = k;
						m_SpawnGroups.Add(ref spawnRange);
					}
					spawnRange.m_Start = k;
					val2 = val3;
				}
			}
			if (spawnRange.m_Start != -1)
			{
				spawnRange.m_End = m_SpawnData.Length;
				m_SpawnGroups.Add(ref spawnRange);
			}
		}
	}

	[BurstCompile]
	private struct TrySpawnCreaturesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<SpawnData> m_SpawnData;

		[ReadOnly]
		public NativeArray<SpawnRange> m_SpawnGroups;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public ComponentLookup<Resident> m_ResidentData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Patient> m_Patients;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Occupant> m_Occupants;

		public void Execute(int index)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			SpawnRange spawnRange = m_SpawnGroups[index];
			Entity val = m_SpawnData[spawnRange.m_Start].m_Source;
			DynamicBuffer<Patient> val2 = default(DynamicBuffer<Patient>);
			DynamicBuffer<Occupant> val3 = default(DynamicBuffer<Occupant>);
			if (m_Patients.HasBuffer(val))
			{
				val2 = m_Patients[val];
			}
			if (m_Occupants.HasBuffer(val))
			{
				val3 = m_Occupants[val];
			}
			for (int i = spawnRange.m_Start; i < spawnRange.m_End; i++)
			{
				Entity val4 = m_SpawnData[i].m_Creature;
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TripSource>(index, val4);
				if (m_ResidentData.HasComponent(val4))
				{
					Resident resident = m_ResidentData[val4];
					if (val2.IsCreated)
					{
						CollectionUtils.RemoveValue<Patient>(val2, new Patient(resident.m_Citizen));
					}
					if (val3.IsCreated)
					{
						CollectionUtils.RemoveValue<Occupant>(val3, new Occupant(resident.m_Citizen));
					}
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public ComponentTypeHandle<TripSource> __Game_Objects_TripSource_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Resident> __Game_Creatures_Resident_RO_ComponentLookup;

		public BufferLookup<Patient> __Game_Buildings_Patient_RW_BufferLookup;

		public BufferLookup<Occupant> __Game_Buildings_Occupant_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_TripSource_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TripSource>(false);
			__Game_Creatures_Resident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Resident>(true);
			__Game_Buildings_Patient_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Patient>(false);
			__Game_Buildings_Occupant_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Occupant>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_CreatureQuery;

	private ComponentTypeSet m_TripSourceRemoveTypes;

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
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CreatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<TripSource>(),
			ComponentType.ReadOnly<Creature>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CreatureQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		NativeList<SpawnData> spawnData = default(NativeList<SpawnData>);
		spawnData._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<SpawnRange> val = default(NativeList<SpawnRange>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val2 = default(JobHandle);
		GroupSpawnSourcesJob groupSpawnSourcesJob = new GroupSpawnSourcesJob
		{
			m_Chunks = ((EntityQuery)(ref m_CreatureQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripSourceType = InternalCompilerInterface.GetComponentTypeHandle<TripSource>(ref __TypeHandle.__Game_Objects_TripSource_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnData = spawnData,
			m_SpawnGroups = val
		};
		TrySpawnCreaturesJob trySpawnCreaturesJob = new TrySpawnCreaturesJob
		{
			m_SpawnData = spawnData.AsDeferredJobArray(),
			m_SpawnGroups = val.AsDeferredJobArray()
		};
		EntityCommandBuffer val3 = m_EndFrameBarrier.CreateCommandBuffer();
		trySpawnCreaturesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val3)).AsParallelWriter();
		trySpawnCreaturesJob.m_ResidentData = InternalCompilerInterface.GetComponentLookup<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		trySpawnCreaturesJob.m_Patients = InternalCompilerInterface.GetBufferLookup<Patient>(ref __TypeHandle.__Game_Buildings_Patient_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		trySpawnCreaturesJob.m_Occupants = InternalCompilerInterface.GetBufferLookup<Occupant>(ref __TypeHandle.__Game_Buildings_Occupant_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		TrySpawnCreaturesJob trySpawnCreaturesJob2 = trySpawnCreaturesJob;
		JobHandle val4 = IJobExtensions.Schedule<GroupSpawnSourcesJob>(groupSpawnSourcesJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2));
		JobHandle val5 = IJobParallelForDeferExtensions.Schedule<TrySpawnCreaturesJob, SpawnRange>(trySpawnCreaturesJob2, val, 1, val4);
		spawnData.Dispose(val5);
		val.Dispose(val5);
		m_EndFrameBarrier.AddJobHandleForProducer(val5);
		((SystemBase)this).Dependency = val5;
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
	public CreatureSpawnSystem()
	{
	}
}
