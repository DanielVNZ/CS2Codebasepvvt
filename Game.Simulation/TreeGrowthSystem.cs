using System.Runtime.CompilerServices;
using Game.Common;
using Game.Objects;
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
public class TreeGrowthSystem : GameSystemBase
{
	[BurstCompile]
	private struct TreeGrowthJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		public ComponentTypeHandle<Tree> m_TreeType;

		public ComponentTypeHandle<Destroyed> m_DestroyedType;

		public ComponentTypeHandle<Damaged> m_DamagedType;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

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
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Tree> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Tree>(ref m_TreeType);
			NativeArray<Destroyed> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Destroyed>(ref m_DestroyedType);
			NativeArray<Damaged> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Damaged>(ref m_DamagedType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (nativeArray3.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Tree tree = nativeArray2[i];
					Destroyed destroyed = nativeArray3[i];
					if (TickTree(ref tree, ref destroyed, ref random))
					{
						Entity val = nativeArray[i];
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(unfilteredChunkIndex, val, default(BatchesUpdated));
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Destroyed>(unfilteredChunkIndex, val);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Damaged>(unfilteredChunkIndex, val);
					}
					nativeArray2[i] = tree;
					nativeArray3[i] = destroyed;
				}
				return;
			}
			if (nativeArray4.Length != 0)
			{
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Tree tree2 = nativeArray2[j];
					Damaged damaged = nativeArray4[j];
					if (TickTree(ref tree2, ref damaged, ref random, out var stateChanged))
					{
						Entity val2 = nativeArray[j];
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(unfilteredChunkIndex, val2, default(BatchesUpdated));
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Damaged>(unfilteredChunkIndex, val2);
					}
					if (stateChanged)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(unfilteredChunkIndex, nativeArray[j], default(BatchesUpdated));
					}
					nativeArray2[j] = tree2;
					nativeArray4[j] = damaged;
				}
				return;
			}
			for (int k = 0; k < nativeArray2.Length; k++)
			{
				Tree tree3 = nativeArray2[k];
				if (TickTree(ref tree3, ref random))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(unfilteredChunkIndex, nativeArray[k], default(BatchesUpdated));
				}
				nativeArray2[k] = tree3;
			}
		}

		private bool TickTree(ref Tree tree, ref Random random)
		{
			switch (tree.m_State & (TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Stump))
			{
			case TreeState.Teen:
				return TickTeen(ref tree, ref random);
			case TreeState.Adult:
				return TickAdult(ref tree, ref random);
			case TreeState.Elderly:
				return TickElderly(ref tree, ref random);
			case TreeState.Dead:
			case TreeState.Stump:
				return TickDead(ref tree, ref random);
			default:
				return TickChild(ref tree, ref random);
			}
		}

		private bool TickTree(ref Tree tree, ref Damaged damaged, ref Random random, out bool stateChanged)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			switch (tree.m_State & (TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Stump))
			{
			case TreeState.Elderly:
			{
				stateChanged = TickElderly(ref tree, ref random);
				ref float3 damage2 = ref damaged.m_Damage;
				damage2 -= ((Random)(ref random)).NextFloat3(float3.op_Implicit(0.03137255f));
				damaged.m_Damage = math.max(damaged.m_Damage, float3.zero);
				return ((float3)(ref damaged.m_Damage)).Equals(float3.zero);
			}
			case TreeState.Dead:
			case TreeState.Stump:
				stateChanged = TickDead(ref tree, ref random);
				return stateChanged;
			default:
			{
				stateChanged = false;
				ref float3 damage = ref damaged.m_Damage;
				damage -= ((Random)(ref random)).NextFloat3(float3.op_Implicit(0.03137255f));
				damaged.m_Damage = math.max(damaged.m_Damage, float3.zero);
				return ((float3)(ref damaged.m_Damage)).Equals(float3.zero);
			}
			}
		}

		private bool TickTree(ref Tree tree, ref Destroyed destroyed, ref Random random)
		{
			destroyed.m_Cleared += ((Random)(ref random)).NextFloat(0.03137255f);
			if (destroyed.m_Cleared < 1f)
			{
				return false;
			}
			tree.m_State &= ~(TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Stump);
			tree.m_Growth = 0;
			destroyed.m_Cleared = 1f;
			return true;
		}

		private bool TickChild(ref Tree tree, ref Random random)
		{
			int num = tree.m_Growth + (((Random)(ref random)).NextInt(1280) >> 8);
			if (num < 256)
			{
				tree.m_Growth = (byte)num;
				return false;
			}
			tree.m_State |= TreeState.Teen;
			tree.m_Growth = 0;
			return true;
		}

		private bool TickTeen(ref Tree tree, ref Random random)
		{
			int num = tree.m_Growth + (((Random)(ref random)).NextInt(938) >> 8);
			if (num < 256)
			{
				tree.m_Growth = (byte)num;
				return false;
			}
			tree.m_State = (tree.m_State & ~TreeState.Teen) | TreeState.Adult;
			tree.m_Growth = 0;
			return true;
		}

		private bool TickAdult(ref Tree tree, ref Random random)
		{
			int num = tree.m_Growth + (((Random)(ref random)).NextInt(548) >> 8);
			if (num < 256)
			{
				tree.m_Growth = (byte)num;
				return false;
			}
			tree.m_State = (tree.m_State & ~TreeState.Adult) | TreeState.Elderly;
			tree.m_Growth = 0;
			return true;
		}

		private bool TickElderly(ref Tree tree, ref Random random)
		{
			int num = tree.m_Growth + (((Random)(ref random)).NextInt(548) >> 8);
			if (num < 256)
			{
				tree.m_Growth = (byte)num;
				return false;
			}
			tree.m_State = (tree.m_State & ~TreeState.Elderly) | TreeState.Dead;
			tree.m_Growth = 0;
			return true;
		}

		private bool TickDead(ref Tree tree, ref Random random)
		{
			int num = tree.m_Growth + (((Random)(ref random)).NextInt(2304) >> 8);
			if (num < 256)
			{
				tree.m_Growth = (byte)num;
				return false;
			}
			tree.m_State &= ~(TreeState.Dead | TreeState.Stump);
			tree.m_Growth = 0;
			return true;
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

		public ComponentTypeHandle<Tree> __Game_Objects_Tree_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Destroyed> __Game_Common_Destroyed_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Damaged> __Game_Objects_Damaged_RW_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Tree_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Tree>(false);
			__Game_Common_Destroyed_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(false);
			__Game_Objects_Damaged_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Damaged>(false);
		}
	}

	public const int UPDATES_PER_DAY = 32;

	public const int TICK_SPEED_CHILD = 1280;

	public const int TICK_SPEED_TEEN = 938;

	public const int TICK_SPEED_ADULT = 548;

	public const int TICK_SPEED_ELDERLY = 548;

	public const int TICK_SPEED_DEAD = 2304;

	private SimulationSystem m_SimulationSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_TreeQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 512;
	}

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
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_TreeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<Tree>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Overridden>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_TreeQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, 32, 16);
		((EntityQuery)(ref m_TreeQuery)).ResetFilter();
		((EntityQuery)(ref m_TreeQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(updateFrame));
		TreeGrowthJob treeGrowthJob = new TreeGrowthJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TreeType = InternalCompilerInterface.GetComponentTypeHandle<Tree>(ref __TypeHandle.__Game_Objects_Tree_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedType = InternalCompilerInterface.GetComponentTypeHandle<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RandomSeed = RandomSeed.Next()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		treeGrowthJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<TreeGrowthJob>(treeGrowthJob, m_TreeQuery, ((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public TreeGrowthSystem()
	{
	}
}
