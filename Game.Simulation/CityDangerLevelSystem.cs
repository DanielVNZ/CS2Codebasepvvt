using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.City;
using Game.Common;
using Game.Events;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CityDangerLevelSystem : GameSystemBase
{
	[BurstCompile]
	private struct DangerLevelJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Game.Events.DangerLevel> m_DangerLevelType;

		public ParallelWriter<MaxFloat> m_Result;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Events.DangerLevel> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Events.DangerLevel>(ref m_DangerLevelType);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				m_Result.Accumulate(new MaxFloat(nativeArray[i].m_DangerLevel));
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateCityJob : IJob
	{
		public ComponentLookup<Game.City.DangerLevel> m_DangerLevel;

		public Entity m_City;

		[ReadOnly]
		public NativeAccumulator<MaxFloat> m_Result;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			m_DangerLevel[m_City] = new Game.City.DangerLevel
			{
				m_DangerLevel = m_Result.GetResult(0).m_Value
			};
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Game.Events.DangerLevel> __Game_Events_DangerLevel_RO_ComponentTypeHandle;

		public ComponentLookup<Game.City.DangerLevel> __Game_City_DangerLevel_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_DangerLevel_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Events.DangerLevel>(true);
			__Game_City_DangerLevel_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.City.DangerLevel>(false);
		}
	}

	private CitySystem m_CitySystem;

	private EntityQuery m_DangerLevelQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 128;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_DangerLevelQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Events.DangerLevel>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate<Game.City.DangerLevel>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		NativeAccumulator<MaxFloat> result = default(NativeAccumulator<MaxFloat>);
		result._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		if (!((EntityQuery)(ref m_DangerLevelQuery)).IsEmptyIgnoreFilter)
		{
			DangerLevelJob dangerLevelJob = new DangerLevelJob
			{
				m_DangerLevelType = InternalCompilerInterface.GetComponentTypeHandle<Game.Events.DangerLevel>(ref __TypeHandle.__Game_Events_DangerLevel_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Result = result.AsParallelWriter()
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<DangerLevelJob>(dangerLevelJob, m_DangerLevelQuery, ((SystemBase)this).Dependency);
		}
		UpdateCityJob updateCityJob = new UpdateCityJob
		{
			m_DangerLevel = InternalCompilerInterface.GetComponentLookup<Game.City.DangerLevel>(ref __TypeHandle.__Game_City_DangerLevel_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_Result = result
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<UpdateCityJob>(updateCityJob, ((SystemBase)this).Dependency);
		result.Dispose(((SystemBase)this).Dependency);
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
	public CityDangerLevelSystem()
	{
	}
}
