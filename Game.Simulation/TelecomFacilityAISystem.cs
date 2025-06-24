using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TelecomFacilityAISystem : GameSystemBase
{
	[BurstCompile]
	private struct TelecomFacilityTickJob : IJobChunk
	{
		public ComponentTypeHandle<Game.Buildings.TelecomFacility> m_TelecomFacilityType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Buildings.TelecomFacility> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.TelecomFacility>(ref m_TelecomFacilityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Game.Buildings.TelecomFacility telecomFacility = nativeArray[i];
				telecomFacility.m_Flags |= TelecomFacilityFlags.HasCoverage;
				nativeArray[i] = telecomFacility;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		public ComponentTypeHandle<Game.Buildings.TelecomFacility> __Game_Buildings_TelecomFacility_RW_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Buildings_TelecomFacility_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TelecomFacility>(false);
		}
	}

	private EntityQuery m_BuildingQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 208;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Buildings.TelecomFacility>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependency = JobChunkExtensions.ScheduleParallel<TelecomFacilityTickJob>(new TelecomFacilityTickJob
		{
			m_TelecomFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TelecomFacility>(ref __TypeHandle.__Game_Buildings_TelecomFacility_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		}, m_BuildingQuery, ((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = dependency;
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
	public TelecomFacilityAISystem()
	{
	}
}
