using System.Runtime.CompilerServices;
using Game.Common;
using Game.Pathfind;
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
public class ServiceRequestSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateRequestGroupJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<RequestGroup> m_RequestGroupType;

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
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<RequestGroup> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RequestGroup>(ref m_RequestGroupType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				uint index = ((Random)(ref random)).NextUInt(nativeArray2[i].m_GroupCount);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<RequestGroup>(unfilteredChunkIndex, val);
				((ParallelWriter)(ref m_CommandBuffer)).AddSharedComponent<UpdateFrame>(unfilteredChunkIndex, val, new UpdateFrame(index));
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct HandleRequestJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<HandleRequest> m_HandleRequestType;

		public ComponentLookup<Dispatched> m_DispatchedData;

		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				num += ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelHashMap<Entity, HandleRequest> val2 = default(NativeParallelHashMap<Entity, HandleRequest>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			HandleRequest handleRequest2 = default(HandleRequest);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val3 = m_Chunks[j];
				NativeArray<HandleRequest> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<HandleRequest>(ref m_HandleRequestType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					HandleRequest handleRequest = nativeArray[k];
					if (val2.TryGetValue(handleRequest.m_Request, ref handleRequest2))
					{
						if (handleRequest.m_Completed)
						{
							val2[handleRequest.m_Request] = handleRequest;
						}
						else if (handleRequest.m_PathConsumed)
						{
							handleRequest2.m_PathConsumed = true;
							val2[handleRequest.m_Request] = handleRequest2;
						}
					}
					else
					{
						val2.Add(handleRequest.m_Request, handleRequest);
					}
				}
			}
			Enumerator<Entity, HandleRequest> enumerator = val2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HandleRequest value = enumerator.Current.Value;
				if (!m_ServiceRequestData.HasComponent(value.m_Request))
				{
					continue;
				}
				if (value.m_Completed)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).DestroyEntity(value.m_Request);
				}
				else if (value.m_Handler != Entity.Null)
				{
					if (m_DispatchedData.HasComponent(value.m_Request))
					{
						m_DispatchedData[value.m_Request] = new Dispatched(value.m_Handler);
						ServiceRequest serviceRequest = m_ServiceRequestData[value.m_Request];
						serviceRequest.m_Cooldown = 0;
						m_ServiceRequestData[value.m_Request] = serviceRequest;
					}
					else
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Dispatched>(value.m_Request, new Dispatched(value.m_Handler));
					}
					if (value.m_PathConsumed)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(value.m_Request);
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<PathElement>(value.m_Request);
					}
				}
				else if (m_DispatchedData.HasComponent(value.m_Request))
				{
					ServiceRequest serviceRequest2 = m_ServiceRequestData[value.m_Request];
					SimulationUtils.ResetFailedRequest(ref serviceRequest2);
					m_ServiceRequestData[value.m_Request] = serviceRequest2;
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<PathInformation>(value.m_Request);
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<PathElement>(value.m_Request);
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Dispatched>(value.m_Request);
				}
			}
			enumerator.Dispose();
			val2.Dispose();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<RequestGroup> __Game_Simulation_RequestGroup_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HandleRequest> __Game_Simulation_HandleRequest_RO_ComponentTypeHandle;

		public ComponentLookup<Dispatched> __Game_Simulation_Dispatched_RW_ComponentLookup;

		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RW_ComponentLookup;

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
			__Game_Simulation_RequestGroup_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RequestGroup>(true);
			__Game_Simulation_HandleRequest_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HandleRequest>(true);
			__Game_Simulation_Dispatched_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Dispatched>(false);
			__Game_Simulation_ServiceRequest_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(false);
		}
	}

	private ModificationEndBarrier m_ModificationBarrier;

	private EntityQuery m_RequestGroupQuery;

	private EntityQuery m_HandleRequestQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationEndBarrier>();
		m_RequestGroupQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RequestGroup>() });
		m_HandleRequestQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HandleRequest>() });
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_RequestGroupQuery, m_HandleRequestQuery });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_RequestGroupQuery)).IsEmptyIgnoreFilter)
		{
			UpdateRequestGroupJob updateRequestGroupJob = new UpdateRequestGroupJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RequestGroupType = InternalCompilerInterface.GetComponentTypeHandle<RequestGroup>(ref __TypeHandle.__Game_Simulation_RequestGroup_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RandomSeed = RandomSeed.Next()
			};
			EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
			updateRequestGroupJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateRequestGroupJob>(updateRequestGroupJob, m_RequestGroupQuery, ((SystemBase)this).Dependency);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
			((SystemBase)this).Dependency = val2;
		}
		if (!((EntityQuery)(ref m_HandleRequestQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val4 = default(JobHandle);
			JobHandle val3 = IJobExtensions.Schedule<HandleRequestJob>(new HandleRequestJob
			{
				m_HandleRequestType = InternalCompilerInterface.GetComponentTypeHandle<HandleRequest>(ref __TypeHandle.__Game_Simulation_HandleRequest_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DispatchedData = InternalCompilerInterface.GetComponentLookup<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Chunks = ((EntityQuery)(ref m_HandleRequestQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4),
				m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
			}, JobHandle.CombineDependencies(val4, ((SystemBase)this).Dependency));
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
			((SystemBase)this).Dependency = val3;
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
	public ServiceRequestSystem()
	{
	}
}
