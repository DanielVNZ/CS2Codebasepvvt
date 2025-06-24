using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ObsoleteChirpSystem : GameSystemBase
{
	[BurstCompile]
	private struct ObsoleteChirpJob : IJob
	{
		[DeallocateOnJobCompletion]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public ComponentLookup<Game.Triggers.Chirp> m_Chirps;

		[ReadOnly]
		public LimitSettingData m_LimitSettingData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			NativeSortExtension.Sort<Entity, ChirpComparer>(m_Entities, new ChirpComparer
			{
				m_Chirps = m_Chirps
			});
			for (int i = 0; i < m_Entities.Length - m_LimitSettingData.m_MaxChirpsLimit; i++)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(m_Entities[i]);
			}
		}
	}

	private struct ChirpComparer : IComparer<Entity>
	{
		[ReadOnly]
		public ComponentLookup<Game.Triggers.Chirp> m_Chirps;

		public int Compare(Entity x, Entity y)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			return m_Chirps[x].m_CreationFrame.CompareTo(m_Chirps[y].m_CreationFrame);
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<Game.Triggers.Chirp> __Game_Triggers_Chirp_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Triggers_Chirp_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Triggers.Chirp>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_ChirpQuery;

	private EntityQuery m_LimitSettingQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 65536;
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
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ChirpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Triggers.Chirp>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Game.Triggers.LifePathEvent>(),
			ComponentType.Exclude<Temp>()
		});
		m_LimitSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LimitSettingData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_ChirpQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_LimitSettingQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_ChirpQuery)).CalculateEntityCount() > ((EntityQuery)(ref m_LimitSettingQuery)).GetSingleton<LimitSettingData>().m_MaxChirpsLimit)
		{
			ObsoleteChirpJob obsoleteChirpJob = new ObsoleteChirpJob
			{
				m_Entities = ((EntityQuery)(ref m_ChirpQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3)),
				m_Chirps = InternalCompilerInterface.GetComponentLookup<Game.Triggers.Chirp>(ref __TypeHandle.__Game_Triggers_Chirp_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LimitSettingData = ((EntityQuery)(ref m_LimitSettingQuery)).GetSingleton<LimitSettingData>(),
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer()
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<ObsoleteChirpJob>(obsoleteChirpJob, ((SystemBase)this).Dependency);
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
	public ObsoleteChirpSystem()
	{
	}
}
