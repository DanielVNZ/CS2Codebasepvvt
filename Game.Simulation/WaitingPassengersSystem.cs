using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Collections;
using Game.Common;
using Game.Creatures;
using Game.Pathfind;
using Game.Routes;
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
public class WaitingPassengersSystem : GameSystemBase
{
	[BurstCompile]
	private struct ClearWaitingPassengersJob : IJobChunk
	{
		public ComponentTypeHandle<WaitingPassengers> m_WaitingPassengersType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<WaitingPassengers> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaitingPassengers>(ref m_WaitingPassengersType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				ref WaitingPassengers reference = ref CollectionUtils.ElementAt<WaitingPassengers>(nativeArray, i);
				reference.m_Count = 0;
				reference.m_OngoingAccumulation = 0;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CountWaitingPassengersJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> m_HumanCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Resident> m_ResidentType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public BufferTypeHandle<GroupCreature> m_GroupCreatureType;

		[ReadOnly]
		public BufferTypeHandle<Queue> m_QueueType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<WaitingPassengers> m_WaitingPassengersData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
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
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<HumanCurrentLane> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_HumanCurrentLaneType);
			NativeArray<Resident> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Resident>(ref m_ResidentType);
			NativeArray<PathOwner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<GroupCreature> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<GroupCreature>(ref m_GroupCreatureType);
			BufferAccessor<Queue> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Queue>(ref m_QueueType);
			Entity lastStop = Entity.Null;
			int2 accumulation = int2.op_Implicit(0);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				HumanCurrentLane currentLane = nativeArray[i];
				DynamicBuffer<GroupCreature> groupCreatures = default(DynamicBuffer<GroupCreature>);
				if (bufferAccessor2.Length != 0)
				{
					groupCreatures = bufferAccessor2[i];
				}
				Resident resident = default(Resident);
				if (nativeArray2.Length != 0)
				{
					resident = nativeArray2[i];
				}
				if (CreatureUtils.TransportStopReached(currentLane))
				{
					PathOwner pathOwner = nativeArray3[i];
					DynamicBuffer<PathElement> val = bufferAccessor[i];
					if (val.Length >= pathOwner.m_ElementIndex + 2)
					{
						Entity target = val[pathOwner.m_ElementIndex].m_Target;
						if (target != Entity.Null)
						{
							AddPassengers(target, resident, groupCreatures, ref lastStop, ref accumulation);
						}
					}
					continue;
				}
				DynamicBuffer<Queue> val2 = bufferAccessor3[i];
				for (int j = 0; j < val2.Length; j++)
				{
					Queue queue = val2[j];
					if (!(queue.m_TargetArea.radius <= 0f))
					{
						Entity targetEntity = queue.m_TargetEntity;
						if (targetEntity != Entity.Null)
						{
							AddPassengers(targetEntity, resident, groupCreatures, ref lastStop, ref accumulation);
						}
					}
				}
			}
			if (lastStop != Entity.Null)
			{
				AddPassengers(Entity.Null, default(Resident), default(DynamicBuffer<GroupCreature>), ref lastStop, ref accumulation);
			}
		}

		private void AddPassengers(Entity stop, Resident resident, DynamicBuffer<GroupCreature> groupCreatures, ref Entity lastStop, ref int2 accumulation)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (stop != lastStop)
			{
				if (m_WaitingPassengersData.HasComponent(lastStop))
				{
					ref WaitingPassengers valueRW = ref m_WaitingPassengersData.GetRefRW(lastStop).ValueRW;
					Interlocked.Add(ref valueRW.m_Count, accumulation.x);
					Interlocked.Add(ref valueRW.m_OngoingAccumulation, accumulation.y);
				}
				lastStop = stop;
				accumulation = int2.op_Implicit(0);
			}
			int2 val = default(int2);
			val.x = 1;
			if (groupCreatures.IsCreated)
			{
				val.x += groupCreatures.Length;
			}
			val.y = (int)((float)(resident.m_Timer * val.x) * (2f / 15f));
			accumulation += val;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct TickWaitingPassengersJob : IJobChunk
	{
		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<WaitingPassengers> m_WaitingPassengersType;

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
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<WaitingPassengers> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaitingPassengers>(ref m_WaitingPassengersType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			int2 val2 = default(int2);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				ref WaitingPassengers reference = ref CollectionUtils.ElementAt<WaitingPassengers>(nativeArray2, i);
				if (((Random)(ref random)).NextInt(64) == 0 && reference.m_SuccessAccumulation < ushort.MaxValue)
				{
					reference.m_SuccessAccumulation++;
				}
				int2 val = new int2(reference.m_OngoingAccumulation, reference.m_ConcludedAccumulation);
				((int2)(ref val2))._002Ector(reference.m_Count, (int)reference.m_SuccessAccumulation);
				val2 = math.max(int2.op_Implicit(1), val2);
				int2 val3 = (val + val2 - 1) / val2;
				int num = math.cmax(val3);
				num = math.min(65535, num - num % 5);
				if (num != reference.m_AverageWaitingTime)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(unfilteredChunkIndex, nativeArray[i], default(PathfindUpdated));
				}
				int num2 = reference.m_SuccessAccumulation + ((Random)(ref random)).NextInt(256) >> 8;
				reference.m_ConcludedAccumulation = math.max(0, reference.m_ConcludedAccumulation - num2 * val3.y);
				reference.m_SuccessAccumulation -= (ushort)num2;
				reference.m_AverageWaitingTime = (ushort)num;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<WaitingPassengers> __Game_Routes_WaitingPassengers_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Resident> __Game_Creatures_Resident_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<GroupCreature> __Game_Creatures_GroupCreature_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Queue> __Game_Creatures_Queue_RO_BufferTypeHandle;

		public ComponentLookup<WaitingPassengers> __Game_Routes_WaitingPassengers_RW_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

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
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			__Game_Routes_WaitingPassengers_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaitingPassengers>(false);
			__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanCurrentLane>(true);
			__Game_Creatures_Resident_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Resident>(true);
			__Game_Pathfind_PathOwner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(true);
			__Game_Pathfind_PathElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(true);
			__Game_Creatures_GroupCreature_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<GroupCreature>(true);
			__Game_Creatures_Queue_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Queue>(true);
			__Game_Routes_WaitingPassengers_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaitingPassengers>(false);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_StopQuery;

	private EntityQuery m_ResidentQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetExistingSystemManaged<EndFrameBarrier>();
		m_StopQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<WaitingPassengers>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ResidentQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<HumanCurrentLane>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<GroupMember>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_StopQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		ClearWaitingPassengersJob clearWaitingPassengersJob = new ClearWaitingPassengersJob
		{
			m_WaitingPassengersType = InternalCompilerInterface.GetComponentTypeHandle<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		CountWaitingPassengersJob countWaitingPassengersJob = new CountWaitingPassengersJob
		{
			m_HumanCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentType = InternalCompilerInterface.GetComponentTypeHandle<Resident>(ref __TypeHandle.__Game_Creatures_Resident_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GroupCreatureType = InternalCompilerInterface.GetBufferTypeHandle<GroupCreature>(ref __TypeHandle.__Game_Creatures_GroupCreature_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_QueueType = InternalCompilerInterface.GetBufferTypeHandle<Queue>(ref __TypeHandle.__Game_Creatures_Queue_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaitingPassengersData = InternalCompilerInterface.GetComponentLookup<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		TickWaitingPassengersJob tickWaitingPassengersJob = new TickWaitingPassengersJob
		{
			m_RandomSeed = RandomSeed.Next(),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaitingPassengersType = InternalCompilerInterface.GetComponentTypeHandle<WaitingPassengers>(ref __TypeHandle.__Game_Routes_WaitingPassengers_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		tickWaitingPassengersJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		TickWaitingPassengersJob tickWaitingPassengersJob2 = tickWaitingPassengersJob;
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<ClearWaitingPassengersJob>(clearWaitingPassengersJob, m_StopQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<CountWaitingPassengersJob>(countWaitingPassengersJob, m_ResidentQuery, val2);
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<TickWaitingPassengersJob>(tickWaitingPassengersJob2, m_StopQuery, val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val4);
		((SystemBase)this).Dependency = val4;
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
	public WaitingPassengersSystem()
	{
	}
}
