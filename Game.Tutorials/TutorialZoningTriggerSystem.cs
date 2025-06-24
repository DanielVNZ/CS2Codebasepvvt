using System.Runtime.CompilerServices;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tutorials;

[CompilerGenerated]
public class TutorialZoningTriggerSystem : TutorialTriggerSystemBase
{
	[BurstCompile]
	private struct CheckZonesJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_ZoneChunks;

		[ReadOnly]
		public BufferTypeHandle<Cell> m_CellType;

		[ReadOnly]
		public BufferTypeHandle<ZoningTriggerData> m_ZoningTriggerType;

		[ReadOnly]
		public BufferLookup<UnlockRequirement> m_UnlockRequirementFromEntity;

		[ReadOnly]
		public BufferLookup<ForceUIGroupUnlockData> m_ForcedUnlockDataFromEntity;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ZonePrefabs m_ZonePrefabs;

		[ReadOnly]
		public EntityArchetype m_UnlockEventArchetype;

		public ParallelWriter m_CommandBuffer;

		public bool m_FirstTimeCheck;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<ZoningTriggerData> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ZoningTriggerData>(ref m_ZoningTriggerType);
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				if (Check(bufferAccessor[i]))
				{
					if (m_FirstTimeCheck)
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TriggerPreCompleted>(unfilteredChunkIndex, nativeArray[i]);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TriggerCompleted>(unfilteredChunkIndex, nativeArray[i]);
					}
					TutorialSystem.ManualUnlock(nativeArray[i], m_UnlockEventArchetype, ref m_ForcedUnlockDataFromEntity, ref m_UnlockRequirementFromEntity, m_CommandBuffer, unfilteredChunkIndex);
				}
			}
		}

		private bool Check(DynamicBuffer<ZoningTriggerData> triggerDatas)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_ZoneChunks.Length; i++)
			{
				ArchetypeChunk val = m_ZoneChunks[i];
				if (Check(triggerDatas, ((ArchetypeChunk)(ref val)).GetBufferAccessor<Cell>(ref m_CellType)))
				{
					return true;
				}
			}
			return false;
		}

		private bool Check(DynamicBuffer<ZoningTriggerData> triggerDatas, BufferAccessor<Cell> cellAccessor)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < cellAccessor.Length; i++)
			{
				if (Check(triggerDatas, cellAccessor[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool Check(DynamicBuffer<ZoningTriggerData> triggerDatas, DynamicBuffer<Cell> cells)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < cells.Length; i++)
			{
				if (Check(triggerDatas, m_ZonePrefabs[cells[i].m_Zone]))
				{
					return true;
				}
			}
			return false;
		}

		private bool Check(DynamicBuffer<ZoningTriggerData> triggerDatas, Entity zone)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < triggerDatas.Length; i++)
			{
				if (triggerDatas[i].m_Zone == zone)
				{
					return true;
				}
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Cell> __Game_Zones_Cell_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ZoningTriggerData> __Game_Tutorials_ZoningTriggerData_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferLookup<UnlockRequirement> __Game_Prefabs_UnlockRequirement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ForceUIGroupUnlockData> __Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup;

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
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			__Game_Zones_Cell_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Cell>(true);
			__Game_Tutorials_ZoningTriggerData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ZoningTriggerData>(true);
			__Game_Prefabs_UnlockRequirement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<UnlockRequirement>(true);
			__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ForceUIGroupUnlockData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
		}
	}

	private ZoneSystem m_ZoneSystem;

	private EntityQuery m_CreatedZonesQuery;

	private EntityQuery m_ZonesQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ActiveTriggerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ZoningTriggerData>(),
			ComponentType.ReadOnly<TriggerActive>(),
			ComponentType.Exclude<TriggerCompleted>()
		});
		m_CreatedZonesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Cell>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Created>(),
			ComponentType.Exclude<Native>()
		});
		m_ZonesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Cell>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Created>(),
			ComponentType.Exclude<Native>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		m_ZoneSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneSystem>();
		((ComponentSystemBase)this).RequireForUpdate(m_ActiveTriggerQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		base.OnUpdate();
		EntityCommandBuffer val2;
		if (base.triggersChanged && !((EntityQuery)(ref m_ZonesQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val = default(JobHandle);
			CheckZonesJob checkZonesJob = new CheckZonesJob
			{
				m_ZoneChunks = ((EntityQuery)(ref m_ZonesQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
				m_CellType = InternalCompilerInterface.GetBufferTypeHandle<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ZoningTriggerType = InternalCompilerInterface.GetBufferTypeHandle<ZoningTriggerData>(ref __TypeHandle.__Game_Tutorials_ZoningTriggerData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UnlockRequirementFromEntity = InternalCompilerInterface.GetBufferLookup<UnlockRequirement>(ref __TypeHandle.__Game_Prefabs_UnlockRequirement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ForcedUnlockDataFromEntity = InternalCompilerInterface.GetBufferLookup<ForceUIGroupUnlockData>(ref __TypeHandle.__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ZonePrefabs = m_ZoneSystem.GetPrefabs(),
				m_UnlockEventArchetype = m_UnlockEventArchetype
			};
			val2 = m_BarrierSystem.CreateCommandBuffer();
			checkZonesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkZonesJob.m_FirstTimeCheck = true;
			CheckZonesJob checkZonesJob2 = checkZonesJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckZonesJob>(checkZonesJob2, m_ActiveTriggerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			checkZonesJob2.m_ZoneChunks.Dispose(((SystemBase)this).Dependency);
			m_ZoneSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_BarrierSystem).AddJobHandleForProducer(((SystemBase)this).Dependency);
		}
		else if (!((EntityQuery)(ref m_CreatedZonesQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val3 = default(JobHandle);
			CheckZonesJob checkZonesJob = new CheckZonesJob
			{
				m_ZoneChunks = ((EntityQuery)(ref m_CreatedZonesQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3),
				m_CellType = InternalCompilerInterface.GetBufferTypeHandle<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ZoningTriggerType = InternalCompilerInterface.GetBufferTypeHandle<ZoningTriggerData>(ref __TypeHandle.__Game_Tutorials_ZoningTriggerData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UnlockRequirementFromEntity = InternalCompilerInterface.GetBufferLookup<UnlockRequirement>(ref __TypeHandle.__Game_Prefabs_UnlockRequirement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ForcedUnlockDataFromEntity = InternalCompilerInterface.GetBufferLookup<ForceUIGroupUnlockData>(ref __TypeHandle.__Game_Prefabs_ForceUIGroupUnlockData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ZonePrefabs = m_ZoneSystem.GetPrefabs(),
				m_UnlockEventArchetype = m_UnlockEventArchetype
			};
			val2 = m_BarrierSystem.CreateCommandBuffer();
			checkZonesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
			checkZonesJob.m_FirstTimeCheck = false;
			CheckZonesJob checkZonesJob3 = checkZonesJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CheckZonesJob>(checkZonesJob3, m_ActiveTriggerQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
			checkZonesJob3.m_ZoneChunks.Dispose(((SystemBase)this).Dependency);
			m_ZoneSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_BarrierSystem).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public TutorialZoningTriggerSystem()
	{
	}
}
