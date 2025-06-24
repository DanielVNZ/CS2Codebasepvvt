using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Triggers;

[CompilerGenerated]
public class ChirpLikeCountSystem : GameSystemBase
{
	[BurstCompile]
	private struct LikeCountUpdateJob : IJobChunk
	{
		public ComponentTypeHandle<Chirp> m_ChirpType;

		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public uint m_SimulationFrame;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(0);
			NativeArray<Chirp> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Chirp>(ref m_ChirpType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				ref Chirp reference = ref CollectionUtils.ElementAt<Chirp>(nativeArray, i);
				if (reference.m_InactiveFrame > reference.m_CreationFrame && m_SimulationFrame <= reference.m_InactiveFrame && !(((Random)(ref random)).NextFloat() < reference.m_ContinuousFactor))
				{
					float num = (1f * (float)m_SimulationFrame - (float)reference.m_CreationFrame) / (float)(reference.m_InactiveFrame - reference.m_CreationFrame);
					reference.m_Likes = math.max(reference.m_Likes, (uint)((float)reference.m_TargetLikes * math.lerp(0f, 1f, 1f - math.pow(1f - num, (float)reference.m_ViralFactor))));
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
		public ComponentTypeHandle<Chirp> __Game_Triggers_Chirp_RW_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Triggers_Chirp_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Chirp>(false);
		}
	}

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_ChirpQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 64;
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
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ChirpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<Chirp>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ChirpQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		LikeCountUpdateJob likeCountUpdateJob = new LikeCountUpdateJob
		{
			m_ChirpType = InternalCompilerInterface.GetComponentTypeHandle<Chirp>(ref __TypeHandle.__Game_Triggers_Chirp_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next(),
			m_SimulationFrame = m_SimulationSystem.frameIndex
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<LikeCountUpdateJob>(likeCountUpdateJob, m_ChirpQuery, ((SystemBase)this).Dependency);
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
	public ChirpLikeCountSystem()
	{
	}
}
