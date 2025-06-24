using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Common;
using Game.Prefabs;
using Game.PSI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.City;

[CompilerGenerated]
public class DevTreeSystem : GameSystemBase
{
	[BurstCompile]
	private struct AppendPointsJob : IJob
	{
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public NativeList<MilestoneReachedEvent> m_MilestoneReached;

		[ReadOnly]
		public ComponentLookup<MilestoneData> m_Milestones;

		public ComponentTypeHandle<DevTreePoints> m_PointsType;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[0];
			NativeArray<DevTreePoints> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<DevTreePoints>(ref m_PointsType);
			int num = nativeArray[0].m_Points;
			for (int i = 0; i < m_MilestoneReached.Length; i++)
			{
				num += ((m_MilestoneReached[i].m_Milestone != Entity.Null) ? m_Milestones[m_MilestoneReached[i].m_Milestone].m_DevTreePoints : GetDefaultPoints(m_MilestoneReached[i].m_Index));
			}
			nativeArray[0] = new DevTreePoints
			{
				m_Points = num
			};
		}

		private int GetDefaultPoints(int level)
		{
			if (level <= 0)
			{
				return 0;
			}
			if (level >= 19)
			{
				return 10;
			}
			return (level + 1) / 2 + 1;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<MilestoneData> __Game_Prefabs_MilestoneData_RO_ComponentLookup;

		public ComponentTypeHandle<DevTreePoints> __Game_City_DevTreePoints_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<DevTreeNodeData> __Game_Prefabs_DevTreeNodeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

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
			__Game_Prefabs_MilestoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MilestoneData>(true);
			__Game_City_DevTreePoints_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DevTreePoints>(false);
			__Game_Prefabs_DevTreeNodeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DevTreeNodeData>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_MilestoneReachedQuery;

	private EntityQuery m_DevTreePointsQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private PrefabSystem m_PrefabSystem;

	private TypeHandle __TypeHandle;

	public int points
	{
		get
		{
			if (!((EntityQuery)(ref m_DevTreePointsQuery)).IsEmptyIgnoreFilter)
			{
				return ((EntityQuery)(ref m_DevTreePointsQuery)).GetSingleton<DevTreePoints>().m_Points;
			}
			return 0;
		}
		set
		{
			if (!((EntityQuery)(ref m_DevTreePointsQuery)).IsEmptyIgnoreFilter)
			{
				((EntityQuery)(ref m_DevTreePointsQuery)).SetSingleton<DevTreePoints>(new DevTreePoints
				{
					m_Points = value
				});
			}
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_MilestoneReachedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MilestoneReachedEvent>() });
		m_DevTreePointsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<DevTreePoints>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Unlock>(),
			ComponentType.ReadWrite<Event>()
		});
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		((ComponentSystemBase)this).RequireForUpdate(m_DevTreePointsQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_MilestoneReachedQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val = default(JobHandle);
			JobHandle val2 = default(JobHandle);
			AppendPointsJob appendPointsJob = new AppendPointsJob
			{
				m_Chunks = ((EntityQuery)(ref m_DevTreePointsQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
				m_MilestoneReached = ((EntityQuery)(ref m_MilestoneReachedQuery)).ToComponentDataListAsync<MilestoneReachedEvent>(AllocatorHandle.op_Implicit((Allocator)3), ref val2),
				m_Milestones = InternalCompilerInterface.GetComponentLookup<MilestoneData>(ref __TypeHandle.__Game_Prefabs_MilestoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PointsType = InternalCompilerInterface.GetComponentTypeHandle<DevTreePoints>(ref __TypeHandle.__Game_City_DevTreePoints_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<AppendPointsJob>(appendPointsJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, val2));
			appendPointsJob.m_Chunks.Dispose(((SystemBase)this).Dependency);
			appendPointsJob.m_MilestoneReached.Dispose(((SystemBase)this).Dependency);
		}
	}

	public void Purchase(DevTreeNodePrefab nodePrefab)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_DevTreePointsQuery)).IsEmptyIgnoreFilter)
		{
			Entity entity = m_PrefabSystem.GetEntity(nodePrefab);
			Purchase(entity);
		}
	}

	public void Purchase(Entity node)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_DevTreePointsQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		ComponentLookup<DevTreeNodeData> componentLookup = InternalCompilerInterface.GetComponentLookup<DevTreeNodeData>(ref __TypeHandle.__Game_Prefabs_DevTreeNodeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Locked> componentLookup2 = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		DevTreeNodeData devTreeNodeData = componentLookup[node];
		int num = points;
		if (devTreeNodeData.m_Cost > num || !EntitiesExtensions.HasEnabledComponent<Locked>(componentLookup2, node) || !CheckService(devTreeNodeData.m_Service, componentLookup2))
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<DevTreeNodeRequirement>(node))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!CheckRequirements(((EntityManager)(ref entityManager)).GetBuffer<DevTreeNodeRequirement>(node, true), componentLookup2))
			{
				return;
			}
		}
		num -= devTreeNodeData.m_Cost;
		points = num;
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		Entity val2 = ((EntityCommandBuffer)(ref val)).CreateEntity(m_UnlockEventArchetype);
		((EntityCommandBuffer)(ref val)).SetComponent<Unlock>(val2, new Unlock(node));
		Telemetry.DevNodePurchased(m_PrefabSystem.GetPrefab<DevTreeNodePrefab>(node));
	}

	private static bool CheckRequirements(DynamicBuffer<DevTreeNodeRequirement> requirements, ComponentLookup<Locked> locked)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		for (int i = 0; i < requirements.Length; i++)
		{
			if (requirements[i].m_Node != Entity.Null)
			{
				flag = true;
				if (!EntitiesExtensions.HasEnabledComponent<Locked>(locked, requirements[i].m_Node))
				{
					return true;
				}
			}
		}
		return !flag;
	}

	private static bool CheckService(Entity service, ComponentLookup<Locked> locked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!(service == Entity.Null))
		{
			return !EntitiesExtensions.HasEnabledComponent<Locked>(locked, service);
		}
		return true;
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
	public DevTreeSystem()
	{
	}
}
